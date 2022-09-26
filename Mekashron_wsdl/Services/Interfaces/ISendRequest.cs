using Mekashron_wsdl.Models;

namespace Mekashron_wsdl.Services.Interfaces
{
    public interface ISendRequest
    {
        public UserData Send(User user);
    }
}
