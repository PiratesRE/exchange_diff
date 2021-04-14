using System;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.Exchange.Services.OnlineMeetings
{
	internal abstract class OnlineMeetingScheduler : UcwaServerToServerClient
	{
		protected OnlineMeetingScheduler(string ucwaUrl, ICredentials oauthCredentials) : base(ucwaUrl, oauthCredentials)
		{
		}

		public abstract Task<OnlineMeetingResult> GetMeetingAsync(string meetingId);

		public abstract Task<OnlineMeetingResult> CreateMeetingAsync(OnlineMeetingSettings meetingSettings);

		public abstract Task<OnlineMeetingResult> UpdateMeetingAsync(string meetingId, OnlineMeetingSettings meetingSettings);

		public abstract Task DeleteMeetingAsync(string meetingId);
	}
}
