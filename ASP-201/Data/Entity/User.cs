namespace ASP_201.Data.Entity
{
    public class User
    {
        public Guid      Id              { get; set; }
        public String    Login           { get; set; } = null!;
        public String    PasswordHash    { get; set; } = null!;
        public String    PasswordSalt    { get; set; } = null!;
        public String    Email           { get; set; } = null!;
        public String    RealName        { get; set; } = null!;
        public String?   Avatar          { get; set; }
                         
        public DateTime  RegisterDt      { get; set; }  // момент реєстрації
        public DateTime? LastEnterDt     { get; set; }  // остання авторизація на сайті
        public String?   EmailCode       { get; set; }

        //// Додано 19-04-2023 робота з Profile
        public Boolean IsEmailPublic    { get; set; } = false;
        public Boolean IsRealNamePublic { get; set; } = false;
        public Boolean IsDatetimePublic { get; set; } = false;

    }
}
