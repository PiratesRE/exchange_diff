using System;
using System.Threading.Tasks;

namespace Microsoft.Exchange.Services.OnlineMeetings
{
	internal interface IOnlineMeetingWorker
	{
		Task<OnlineMeetingResult> CreateDefaultMeetingAsync(OnlineMeetingSettings meetingSettings);

		Task<OnlineMeetingResult> CreatePrivateMeetingAsync(OnlineMeetingSettings meetingSettings);

		Task DeleteMeetingAsync(string meetingId);

		Task<OnlineMeetingResult> GetMeetingAsync(string meetingId);

		Task<OnlineMeetingResult> UpdatePrivateMeetingAsync(string meetingId, OnlineMeetingSettings meetingSettings);

		Task<OnlineMeetingResult> GetOrCreatePublicMeetingAsync();
	}
}
