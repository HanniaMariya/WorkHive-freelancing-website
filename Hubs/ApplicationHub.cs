using Microsoft.AspNetCore.SignalR;
using WorkHive.Services;
namespace WorkHive.Hubs
{
    public class ApplicationHub:Hub
    {
        private readonly ApplicationService _applicationService=new ApplicationService();
       
        public async Task UpdateApplicationStatus(int appID, string newStatus)
        {
            try
            {
                await _applicationService.UpdateAppStatus(appID, newStatus);
                await Clients.All.SendAsync("ReceiveUpdate", appID, newStatus);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateApplicationStatus: {ex.Message}");
                throw; // This will help debug the exact cause
            }
        }

    }
}
