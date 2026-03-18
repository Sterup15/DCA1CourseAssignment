namespace MoveMeLater;

public interface IEmailInUseChecker
{
    Task<bool> IsEmailInUseAsync(string email);
}