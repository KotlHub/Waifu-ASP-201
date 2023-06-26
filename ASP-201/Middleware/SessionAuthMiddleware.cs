using ASP_201.Data;
using ASP_201.Data.Entity;
using System.Security.Claims;

namespace ASP_201.Middleware
{
    public class SessionAuthMiddleware
    {
        /* Для утворення ланцюга кожна ланка (об'єкт Middleware) отримує
         * посилання на наступну ланку. Це посилання передається через конструктор
         * і має бути збережено у об'єкті
         */
        private readonly RequestDelegate _next;

        /* Middleware існує портягом всієї роботи (Singltone), тому
         * інжекція сервісів через конструктор не здійснюється
         */
        public SessionAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /* Обов'язковий для Middleware метод InvokeAsync або Invoke (старий підхід)
         * Перший параметр - завжди HttpContext context,
         * за потреби, наступні параметри - інжекції
         */
        public async Task InvokeAsync(HttpContext context, ILogger<SessionAuthMiddleware> logger,
            DataContext dataContext)
        {
            // logger.LogInformation("SessionAuthMiddleware works");
            // перевіряємо наявність у сесії "authUserId" (встановлюється при
            // автентифікації у UserController::AuthUser() )
            String? userId = context.Session.GetString("authUserId");
            if (userId is not null)
            {
                try
                {
                    User? authUser = dataContext.Users.Find(Guid.Parse(userId));
                    if (authUser is not null)
                    {
                        context.Items.Add("authUser", authUser);
                        /* Передача відомостей про користувача шляхом посилання 
                         * на об'єкт-сутність (Entity) підвищує зчеплення
                         * (залежність від реалізації), а також поширює відомості 
                         * про "технічну" сутність User, потрібну для ORM, на 
                         * увесь проект, де потрібна авторизація.
                         * 
                         * Для уніфікації ...
                         */
                        Claim[] claims = new Claim[]
                        {
                            new Claim(ClaimTypes.Sid, userId),
                            new Claim(ClaimTypes.Name, authUser.RealName),
                            new Claim(ClaimTypes.NameIdentifier, authUser.Login),
                            new Claim(ClaimTypes.UserData, authUser.Avatar ?? String.Empty),
                            new Claim(ClaimTypes.Email, authUser.EmailCode ?? String.Empty)
                        };
                        /*Створюємо власника () із даними твердженнями */
                        var principal = new ClaimsPrincipal(
                            new ClaimsIdentity(claims,
                            nameof(SessionAuthMiddleware)
                            ));
                        /* У HttpContext є вбудоване поле User з типом ClaimsPrincipal
                         * Встановлення його дозволить задіяти ASP механізм авторизації */
                        context.User = principal;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "SessionAuthMiddleware");
                }
            }
            await _next(context);
        }
    }

    public static class SessionAuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseSessionAuth(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SessionAuthMiddleware>();
        }
    }
}
