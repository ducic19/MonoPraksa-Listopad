using gymap.Model; // prilagodi namespace po potrebi

namespace gymap.Service.Common
{
    public interface ITokenService
    {
        string CreateToken(Member user);
    }
}