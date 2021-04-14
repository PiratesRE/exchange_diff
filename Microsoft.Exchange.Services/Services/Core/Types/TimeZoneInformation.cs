using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class TimeZoneInformation : OptionsPropertyChangeTracker
	{
		[DataMember]
		public string TimeZoneId
		{
			get
			{
				return this.timeZoneId;
			}
			set
			{
				this.timeZoneId = value;
				base.TrackPropertyChanged("TimeZoneId");
			}
		}

		[DataMember]
		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
			set
			{
				this.displayName = value;
				base.TrackPropertyChanged("DisplayName");
			}
		}

		private string timeZoneId;

		private string displayName;
	}
}
