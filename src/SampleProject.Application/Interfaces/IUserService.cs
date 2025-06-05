using SampleProject.Infrastructure.EF.Entities;
using SampleProject.Shared.Models.Animal;
using SampleProject.Shared.Models.Misc;
using SampleProject.Shared.Models.User;

namespace SampleProject.Application.Interfaces
{
    public interface IUserService
    {
        Task<APIResponseModel<LoginResponseModel>> Login(LoginModel request, CancellationToken cancellationToken);
        Task<APIResponseModel<LoginResponseModel>> RegisterUser(RegisterRequestModel request, CancellationToken cancellationToken);
        Task<APIResponseModel<LoginResponseModel>> AddUserBooking(UserBookingsRequestModel request, CancellationToken cancellationToken);
        Task<APIResponseModel<bool>> ChangeStatus(int id, string status, CancellationToken cancellationToken);
        Task<APIResponseModel<List<LoginResponseModel>>> GetBookingUsers(int animalId, int userId, CancellationToken cancellationToken);
    }
}
