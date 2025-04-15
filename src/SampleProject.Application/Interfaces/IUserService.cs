using SampleProject.Shared.Models.Misc;
using SampleProject.Shared.Models.User;

namespace SampleProject.Application.Interfaces
{
    public interface IUserService
    {
        Task<APIResponseModel<string>> Login(LoginModel request, CancellationToken cancellationToken);
    }
}
