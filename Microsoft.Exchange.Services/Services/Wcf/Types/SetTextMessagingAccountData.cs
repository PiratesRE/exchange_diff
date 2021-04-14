using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SetTextMessagingAccountData : OptionsPropertyChangeTracker
	{
		[DataMember]
		public string CountryRegionId
		{
			get
			{
				return this.countryRegionId;
			}
			set
			{
				this.countryRegionId = value;
				base.TrackPropertyChanged("CountryRegionId");
			}
		}

		[DataMember]
		public int MobileOperatorId
		{
			get
			{
				return this.mobileOperatorId;
			}
			set
			{
				this.mobileOperatorId = value;
				base.TrackPropertyChanged("MobileOperatorId");
			}
		}

		[DataMember]
		public string NotificationPhoneNumber
		{
			get
			{
				return this.notificationPhoneNumber;
			}
			set
			{
				this.notificationPhoneNumber = value;
				base.TrackPropertyChanged("NotificationPhoneNumber");
			}
		}

		private string countryRegionId;

		private int mobileOperatorId;

		private string notificationPhoneNumber;
	}
}
