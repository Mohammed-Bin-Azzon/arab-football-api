using Microsoft.AspNetCore.Http;

namespace ArabFootball.Api.Shared.Helpers
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string folderName);

        void DeleteFile(string fileName, string folderName);
    }
}