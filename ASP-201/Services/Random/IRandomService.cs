﻿namespace ASP_201.Services.Random
{
    public interface IRandomService
    {
        String ConfirmCode(int length);
        String RandomString(int length);
        String RandomAvatarName(String fileName, int length);
    }
}
