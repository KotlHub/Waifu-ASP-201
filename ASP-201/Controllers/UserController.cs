using ASP_201.Models.User;
using ASP_201.Data.Entity;
using ASP_201.Services.Hash;
using ASP_201.Services.Random;
using ASP_201.Services.Kdf;
using ASP_201.Services.Validation;
using ASP_201.Services.Email;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows;
using ASP_201.Data;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;
using ASP_201.Models;

namespace ASP_201.Controllers
{
    public class UserController : Controller
    {
        private readonly IHashService _hashService;
        private readonly ILogger<UserController> _logger;
        private readonly DataContext _dataContext;
        private readonly IRandomService _randomService;
        private readonly IKdfService _kdfService;
        private readonly IValidationService _validationService;
        private readonly IEmailService _emailService;
        public UserController(IHashService hashService, ILogger<UserController> logger, DataContext dataContext, 
            IRandomService randomService, IKdfService kdfService, 
            IValidationService validationService, IEmailService emailService)
        {
            _hashService = hashService;
            _logger = logger;
            _dataContext = dataContext;
            _randomService = randomService;
            _kdfService = kdfService;
            _validationService = validationService;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult RegistrationModel()
        {
            return View();
        }
        public IActionResult Register(RegistrationModel model)
        {
            /*ViewData["registrationModel"] = model;

            // спосіб перейти на View з іншою назвою, ніж метод
            return View("RegistrationModel");*/

            bool isModelValid = true;
            byte minPasswordLength = 3;
            RegistrationValidationModel registrationValidation = new();

            #region Login Validation
            if (String.IsNullOrEmpty(model.Login))
            {
                registrationValidation.LoginMessage = "Логін не може бути порожним";
                isModelValid = false;
            }
            if(_dataContext.Users.Any(u => u.Login == model.Login))
            {
                registrationValidation.LoginMessage = "Логін вже використовується";
                isModelValid = false;
            }
            #endregion

            #region Password Validation /RepeatPass
            if (String.IsNullOrEmpty(model.Password))
            {
                registrationValidation.PasswordMessage = "Пароль не може бути порожним";
                registrationValidation.RepeatPasswordMessage = "Пароль не може бути порожним";
                isModelValid = false;
            }
            else if(model.Password.Length < minPasswordLength)
            {
                registrationValidation.PasswordMessage = $"Пароль закороткий. Щонайменш символів - {minPasswordLength}";
                registrationValidation.RepeatPasswordMessage = "";
                isModelValid = false;
            }
            else if(!model.Password.Equals(model.RepeatPassword))
            {
                registrationValidation.PasswordMessage = 
                    registrationValidation.RepeatPasswordMessage = "Паролі не збігаються";
                isModelValid = false;
            }
            #endregion

            #region Email Validation
            if (!_validationService.Validate(model.Email, ValidationTerms.NotEmpty))
            {
                registrationValidation.EmailMessage = "Email не може бути порожним";
                isModelValid = false;
            }
            else if (!_validationService.Validate(model.Email, ValidationTerms.Email))
            {
                registrationValidation.EmailMessage = "Email не відповідає шаблону";
                isModelValid = false;
            }
            #endregion

            #region RealName Validation
            if (String.IsNullOrEmpty(model.RealName))
            {
                registrationValidation.RealNameMessage = "RealName не може бути порожним";
                isModelValid = false;
            }
            else
            {
                String nameRegex = @"^.+$";
                if (!Regex.IsMatch(model.RealName, nameRegex))
                {
                    registrationValidation.RealNameMessage = "Ім'я не відповідає шаблону";
                    isModelValid = false;
                }
            }
            #endregion

            #region IsAgree Validation
            if (model.IsAgree == false)
            {
                registrationValidation.IsAgreeMessage = "Для реєстрації слід прийняти правила сайту";
                isModelValid = false;
            }
            #endregion

            #region Avatar
            String savedName = null!;
            // будемо вважати аватар необов'язковим, обробляємо лише якщо він переданий
            if (model.Avatar is not null)  // є файл
            {
                if (model.Avatar.Length > 1024)
                {
                    // Генеруємо для файла нове імя, але зберігаємо розширення
                    String ext = Path.GetExtension(model.Avatar.FileName);
                    String path = "";
                    FileInfo fileInfo = null!;
                    do
                    {
                        // TODO : перевірити розширення на перелік дозволених
                        // savedName = _hashService.Hash(model.Avatar.FileName + DateTime.Now + Random.Shared.Next())[..16] + ext;
                        savedName = _randomService.RandomAvatarName(ext, 16);
                        path = "wwwroot/avatars/" + savedName;
                        fileInfo = new FileInfo(path);
                    } while (fileInfo.Exists);
                    using FileStream fs = new(path, FileMode.Create);
                    model.Avatar.CopyTo(fs);
                    ViewData["savedName"] = savedName;
                }
                else
                {
                    registrationValidation.AvatarMessage = "Зображення має бути більше ніж 1кБ";
                    isModelValid = false;
                }
            }
            #endregion

            // якщо всі перевірки пройдені, то переходимо на нову сторінку з вітаннями
            if (isModelValid)
            {
                String salt = _randomService.RandomString(16);
                String confirmEmailCode = _randomService.ConfirmCode(6);
                User user = new()
                {
                    Id = Guid.NewGuid(),
                    Login = model.Login,
                    RealName = model.RealName,
                    Email = model.Email,
                    EmailCode = confirmEmailCode,
                    PasswordSalt = salt,
                    PasswordHash = _kdfService.GetDerivedKey(model.Password, salt),
                    Avatar = savedName,
                    RegisterDt = DateTime.Now,
                    LastEnterDt = null
                };
                _dataContext.Users.Add(user);

                // welcome_email 
                _SendWelcomeEmail(user);

                // Якщо дані у БД додані, Надсилаємо код підтвердження на пошту
                // генеруємо токен автоматичного підтвердження
                var emailConfirmToken = _GenerateEmailConfirmToken(user);
                // Надсилаємо листа з токеном
                _SendConfirmEmail(user, emailConfirmToken);

                _dataContext.SaveChangesAsync();
                return View(model);
            }
            else  // не всі дані валідні - повертаємо на форму реєстрації
            {
                // передаємо дані щодо перевірок
                ViewData["registrationValidation"] = registrationValidation;
                // спосіб перейти на View з іншою назвою, ніж метод
                return View("RegistrationModel");
            }
        }
        private bool _SendWelcomeEmail(Data.Entity.User user)
        {
            return _emailService.Send("welcome_email",
                new Models.Email.WelcomeEmailModel
                {
                    Email = user.Email,
                    RealName = user.RealName
                });
        }
        private bool _SendConfirmEmail(Data.Entity.User              user, 
                                       Data.Entity.EmailConfirmToken emailConfirmToken)
        {
            // Формуємо посилання: схема://домен(хост)/User/ConfirmToken?token=...
            // схема - http або https домен(хост) - localhost:7572
            String confirmLink = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}/User/ConfirmToken?token={emailConfirmToken.Id}";
            return _emailService.Send(
                "confirm_email",
                new Models.Email.ConfirmEmailModel
                {
                    Email = user.Email,
                    RealName = user.RealName,
                    EmailCode = user.EmailCode,
                    ConfirmLink = confirmLink
                });
        }
        private EmailConfirmToken _GenerateEmailConfirmToken(Data.Entity.User user)
        {
            Data.Entity.EmailConfirmToken emailConfirmToken = new()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                UserEmail = user.Email,
                Moment = DateTime.Now,
                Used = 0
            };
            _dataContext.EmailConfirmTokens.Add(emailConfirmToken);
            return emailConfirmToken;
        }
        [HttpPost]
        public String AuthUser()
        {
            // альтернативний (до моделей) спосіб отримання параметрів запиту
            StringValues loginValues = Request.Form["user-login"];
            // колекція loginValues формується при будь-якому ключі, але для
            // неправильних (відсутніх) ключів вона порожня
            if (loginValues.Count == 0)
            {
                // немає логіну у складі полів
                return "Missed required parameter : user-login";
            }
            String login = loginValues[0] ?? "";

            StringValues passvalue = Request.Form["user-password"];
            if (passvalue.Count == 0)
            {
                return "Missed required parameter : user-password";
            }
            String password = passvalue[0] ?? "";

            User? user = _dataContext.Users.Where(u => u.Login == login).FirstOrDefault();
            if (user is not null)
            {
                // якщо знайшли, то перевіряємо пароль (derived key)
                if (user.PasswordHash == _kdfService.GetDerivedKey(password, user.PasswordSalt))
                {
                    // дані перевірено -=- користувач автентифікований
                    HttpContext.Session.SetString("authUserId", user.Id.ToString());
                    return "OK";
                }
            }
            return "Авторизацію відхилено";
            //return $"Auth User : Login '{login}', Password '{password}'";
        }
        public IActionResult LogOut()
        {
            HttpContext.Session.Remove("authUserId");
            Response.Redirect("Index");
            return View("Index");

           // return RedirectToAction("Index", "Home");
            /* Redirect та інші питання з перенаправлення
             * Browser            Server
             * GET /home --------> (routing)->Home::Index()->View()
             *   page    <-------- 200 OK <!doctype html>...
             *   
             * <a Logout> -------> User::Logout()->Redirect(...) 
             *   follow  <------- 302 (Redirect) Location: /home
             * GET /home --------> (routing)->Home::Index()->View()
             *   page    <-------- 200 OK <!doctype html>...           
             */
        }
        public IActionResult Profile([FromRoute]String id)
        {
            /*_logger.LogInformation(id);*/
            User? user = _dataContext.Users.FirstOrDefault(u => u.Login == id);
            if (user is not null) {
                Models.User.ProfieModel model = new(user);
                // ...
                if (HttpContext.User.Identity is not null && HttpContext.User.Identity.IsAuthenticated)
                {
                    String userLogin = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

                    if (userLogin == user.Login)
                    {
                        model.IsPersonal = true;
                    }

                }
                return View(model);
            }
            else {
                return NotFound();
            }
        }
        [HttpPut]
        public IActionResult Update([FromBody] UpdateRequestModel model)
        {
            UpdateResponseModel responseModel = new();
            try
            {
                if(model is null) { throw new Exception("No or empty data"); }
                if(HttpContext.User.Identity?.IsAuthenticated == false)  { throw new Exception("UnAuthenticated"); }

                User? user = _dataContext.Users.Find(
                    Guid.Parse(
                        HttpContext.User.Claims
                        .First(c => c.Type == ClaimTypes.Sid)
                        .Value));
                if (user is null) { throw new Exception("UnAuthenticated"); }
                switch(model.Field)
                {
                    case "realname":
                        if(_validationService.Validate(model.Value, ValidationTerms.RealName))
                        {
                            user.RealName = model.Value;
                            _dataContext.SaveChanges();
                        }
                        else
                        {
                            throw new Exception($"Validation error : field '{model.Field}' with value '{model.Value}'");
                        }
                        break;
                    case "email":
                        user.Email = model.Value;
                        _dataContext.SaveChanges();
                        ResendConfirmEmail();
                        break;

                    default: throw new Exception("Invalid 'Field' attribute");
                }

                responseModel.Status = "OK";
                responseModel.Data = $"Field '{model.Field}' update by value '{model.Value}' ";
            }
            catch (Exception ex)
            {
                responseModel.Status = "Error";
                responseModel.Data = ex.Message;
            }
            return Json(responseModel);


            /* 
             * 
             * 
             */
        }
        [HttpPost]
        public JsonResult ConfirmEmail([FromBody] String emailCode)
        {
            StatusDataModel model = new();

            if(String.IsNullOrEmpty(emailCode))
            {
                model.Status = "406";
                model.Data = "Empty code not acceptable";
            }
            else if(HttpContext.User.Identity?.IsAuthenticated == false)
            {
                model.Status = "401";
                model.Data = "Unauthenticated";
            }
            else
            {
                User? user = _dataContext.Users.Find(
                    Guid.Parse(
                        HttpContext.User.Claims
                        .First(c => c.Type == ClaimTypes.Sid)
                        .Value));
                if (user is null)
                {
                    model.Status = "403";
                    model.Data = "Forbidden (UnAthorized)";
                }
                else if(user.EmailCode is null)
                {
                    model.Status = "208";
                    model.Data = "Already confirmed";
                }
                else if(user.EmailCode != emailCode)
                {
                    model.Status = "406";
                    model.Data = "Code not Accepted";
                }
                else
                {
                    user.EmailCode = null;
                    _dataContext.SaveChanges();
                    model.Status = "200";
                    model.Data = "OK";
                }

            }

            return Json(new { model });
        }
        [HttpGet]
        public ViewResult ConfirmToken([FromBody] String token)
        {
            try
            {
                // шукаємо токен за отриманим Id
                EmailConfirmToken confirmToken = _dataContext.EmailConfirmTokens
                    .Find(Guid.Parse(token)) ?? throw new Exception();
                // 
                var user = _dataContext.Users.Find(
                    confirmToken.UserId) ?? throw new Exception();
                if(user.Email != confirmToken.UserEmail)
                    throw new Exception();
                // 
                user.EmailCode = null;  // 
                confirmToken.Used += 1; // 
                _dataContext.SaveChangesAsync();

                ViewData["result"] = "Вітаємо, пошта успішно підтверджена";
            }
            catch
            {
                ViewData["result"] = "Перевірка не пройдена, не змінюйте посилання з листа";
            }
            return View();
        }
        [HttpPatch]
        public String ResendConfirmEmail()
        {
            if (HttpContext.User.Identity?.IsAuthenticated == false)
            {
                return "Unauthenticated";
            }
            try
            {
                User? user = _dataContext.Users.Find(
                    Guid.Parse(
                        HttpContext.User.Claims
                        .First(c => c.Type == ClaimTypes.Sid)
                        .Value)) ?? throw new Exception();

                // формуємо новий код підтвердження пошти
                user.EmailCode = _randomService.ConfirmCode(6);
                
                // генеруємо токен автоматичного підтвердження
                var emailConfirmToken = _GenerateEmailConfirmToken(user);

                // зберігаємо новий код і токен
                _dataContext.SaveChangesAsync();

                // надсилаємо листа
                if (_SendConfirmEmail(user, emailConfirmToken)) { return "OK"; }
                else { return "Send error"; }
            }
            catch
            {
                return "Unauthenticated";
            }
        }
    }
}
