using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SetHotmailSubscriptionData : OptionsPropertyChangeTracker
	{
		[DataMember]
		public Identity Identity
		{
			get
			{
				return this.identity;
			}
			set
			{
				this.identity = value;
				base.TrackPropertyChanged("Identity");
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

		[DataMember]
		public string Password
		{
			get
			{
				return this.password;
			}
			set
			{
				this.password = value;
				base.TrackPropertyChanged("Password");
			}
		}

		private string displayName;

		private Identity identity;

		private string password;
	}
}
