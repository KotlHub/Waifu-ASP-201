namespace ASP_201.Data.Entity
{
    public class Waifu
    {
        public int       Id             { get; set; }
        public String?   Name           { get; set; } = null!;
        public String?   Nickname       { get; set; } = null!;
        public String?   Image          { get; set; } = null!;
        public String?   WhereAreFrom   { get; set; } = null!;
        public String?   Type           { get; set; } = null!;
        public int?       Age           { get; set; } = null!;
        public String?   Characteristic { get; set; } = null!;
        public String?   Comment        { get; set; } = null!;
    }
}
