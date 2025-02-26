
using AuthenticationApi.Application.DTOs;
using eCommerce.SharedLibrary.Responses;


namespace AuthenticationApi.Application.Interfaces
{
   public interface IUser
    {
        Task<Response> Register(AppUserDto appUserDto);
        Task<Response> Login(LoginDTO loginDTO);
        Task<GetUserDTO> GetUser(int userId);
    }
}
