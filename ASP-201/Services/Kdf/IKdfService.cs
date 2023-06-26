namespace ASP_201.Services.Kdf
{
    public interface IKdfService
    {
        ///
        /// Mixing password and salt to make a derived key
        ///
        /// <param name="password">Password</param>
        /// <param name="salt">Salt</param>
        /// <returns>Derived Key as string</returns>
        String GetDerivedKey(String password, String salt);
    }
}
