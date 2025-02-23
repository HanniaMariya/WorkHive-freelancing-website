using Microsoft.AspNetCore.Mvc;
using WorkHive.Models;
using WorkHive.Models.Repositories;

namespace WorkHive.Services
{
    public class ApplicationService
    {
        private readonly ApplicationRepository _applicationRepository=new ApplicationRepository();
        private readonly JobRepository _jobRepository=new JobRepository();
        public async Task UpdateAppStatus(int appId, string newStatus)
        {
            await Task.Run(() =>
            {
                Application application = _applicationRepository.GetApplicationById(appId);
                if (application == null)
                {
                    Console.WriteLine($"Application with ID {appId} not found!");
                    return;
                }
                _applicationRepository.UpdateStatus(appId, newStatus);

                if (newStatus.Equals("Accepted"))
                {
                    _jobRepository.UpdateStatus(application.job_id, "Assigned");
                }
            });
        }
    }
}
