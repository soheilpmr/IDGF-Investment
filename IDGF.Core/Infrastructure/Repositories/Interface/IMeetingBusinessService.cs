using BackEndInfrastructure.DynamicLinqCore;
using IDGF.Core.Controllers.Dtos;
using IDGF.Core.Domain;

namespace IDGF.Core.Infrastructure.Repositories.Interface
{
    public interface IMeetingBusinessService
    {
        Task<int> CreateMeetingWithFilesAsync(CreateMeetingRequestDto dto);

        Task<LinqDataResult<MeetingResponseDto>> GetMeetingListAsync(LinqDataRequest request);

        Task<Meeting> GetMeetingByIdAsync(int id);

        Task RemoveMeetingAndFilesAsync(int id);

        Task<(Stream FileStream, string ContentType, string FileName)> GetFileForDownloadAsync(int meetingId, string fileType);
    }
}
