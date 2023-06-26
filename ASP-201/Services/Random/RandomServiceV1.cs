namespace ASP_201.Services.Random
{
    public class RandomServiceV1 : IRandomService
    {
        private readonly String _codeChars = "abcdefghijklmnopqrstuvwxyz0123456789";
        private readonly String _safeChars = new String(Enumerable.Range(20, 107).Select(x => (char)x).ToArray());
        private readonly System.Random _random = new();

        public String ConfirmCode(int length)
        {
            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = _codeChars[_random.Next(_codeChars.Length)];
            }
            return new String(chars);
        }
        public String RandomString(int length)
        {
            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = _safeChars[_random.Next(_safeChars.Length)];
            }
            return new String(chars);
        }
        public String RandomAvatarName(string fileName, int length)
        {
            return ConfirmCode(length) + '_' + fileName;
        }
    }
}
