using Gymap.Common;

namespace Gymap.Service.Auth;

public interface ITokenService
{
    string CreateToken(Member user);
}
