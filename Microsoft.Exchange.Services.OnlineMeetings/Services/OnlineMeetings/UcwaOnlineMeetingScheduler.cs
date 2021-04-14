using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.Services.OnlineMeetings
{
	internal class UcwaOnlineMeetingScheduler : OnlineMeetingScheduler
	{
		public UcwaOnlineMeetingScheduler(string ucwaUrl, OAuthCredentials oauthCredentials, CultureInfo culture) : this(new Uri(ucwaUrl, UriKind.RelativeOrAbsolute), oauthCredentials, culture)
		{
		}

		public UcwaOnlineMeetingScheduler(Uri ucwaUrl, OAuthCredentials oauthCredentials, CultureInfo culture) : base(ucwaUrl.AbsoluteUri, oauthCredentials)
		{
			if (!Uri.UriSchemeHttps.Equals(ucwaUrl.Scheme))
			{
				throw new ArgumentException("The UCWA URL scheme must be '" + Uri.UriSchemeHttps + "'");
			}
			this.worker = new UcwaNewOnlineMeetingWorker(ucwaUrl, oauthCredentials, culture);
		}

		public UcwaOnlineMeetingScheduler(Uri ucwaUrl, string webTicket) : base(ucwaUrl.AbsoluteUri, null)
		{
			if (!Uri.UriSchemeHttps.Equals(ucwaUrl.Scheme))
			{
				throw new ArgumentException("The UCWA URL scheme must be '" + Uri.UriSchemeHttps + "'");
			}
			this.worker = new UcwaNewOnlineMeetingWorker(ucwaUrl, webTicket);
		}

		public UcwaOnlineMeetingScheduler(Uri ucwaUrl, UcwaRequestFactory requestFactory) : base(ucwaUrl.AbsoluteUri, null)
		{
			if (!Uri.UriSchemeHttps.Equals(ucwaUrl.Scheme))
			{
				throw new ArgumentException("The UCWA URL scheme must be '" + Uri.UriSchemeHttps + "'");
			}
			this.worker = new UcwaNewOnlineMeetingWorker(ucwaUrl, requestFactory);
		}

		public override Task<OnlineMeetingResult> GetMeetingAsync(string meetingId)
		{
			if (string.IsNullOrWhiteSpace(meetingId))
			{
				throw new ArgumentNullException("meetingId");
			}
			return this.worker.GetMeetingAsync(meetingId);
		}

		public override Task<OnlineMeetingResult> CreateMeetingAsync(OnlineMeetingSettings meetingSettings)
		{
			return this.worker.CreateDefaultMeetingAsync(meetingSettings);
		}

		public override Task<OnlineMeetingResult> UpdateMeetingAsync(string meetingId, OnlineMeetingSettings meetingSettings)
		{
			if (string.IsNullOrWhiteSpace(meetingId))
			{
				throw new ArgumentNullException("meetingId");
			}
			if (meetingSettings == null)
			{
				throw new ArgumentNullException("meetingSettings");
			}
			return this.worker.UpdatePrivateMeetingAsync(meetingId, meetingSettings);
		}

		public override Task DeleteMeetingAsync(string meetingId)
		{
			if (string.IsNullOrWhiteSpace(meetingId))
			{
				throw new ArgumentNullException("meetingId");
			}
			return this.worker.DeleteMeetingAsync(meetingId);
		}

		private readonly IOnlineMeetingWorker worker;
	}
}
