using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class CalendarProcessingOptions : OptionsPropertyChangeTracker
	{
		[DataMember]
		public bool RemoveOldMeetingMessages
		{
			get
			{
				return this.removeOldMeetingMessages;
			}
			set
			{
				this.removeOldMeetingMessages = value;
				base.TrackPropertyChanged("RemoveOldMeetingMessages");
			}
		}

		[DataMember]
		public bool RemoveForwardedMeetingNotifications
		{
			get
			{
				return this.removeForwardedMeetingNotifications;
			}
			set
			{
				this.removeForwardedMeetingNotifications = value;
				base.TrackPropertyChanged("RemoveForwardedMeetingNotifications");
			}
		}

		private bool removeOldMeetingMessages;

		private bool removeForwardedMeetingNotifications;
	}
}
