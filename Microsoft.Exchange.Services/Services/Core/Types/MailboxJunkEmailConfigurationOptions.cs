using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class MailboxJunkEmailConfigurationOptions : OptionsPropertyChangeTracker
	{
		[DataMember]
		public string[] TrustedSendersAndDomains
		{
			get
			{
				return this.trustedSendersAndDomains;
			}
			set
			{
				this.trustedSendersAndDomains = value;
				base.TrackPropertyChanged("TrustedSendersAndDomains");
			}
		}

		[DataMember]
		public string[] BlockedSendersAndDomains
		{
			get
			{
				return this.blockedSendersAndDomains;
			}
			set
			{
				this.blockedSendersAndDomains = value;
				base.TrackPropertyChanged("BlockedSendersAndDomains");
			}
		}

		[DataMember]
		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				this.enabled = value;
				base.TrackPropertyChanged("Enabled");
			}
		}

		[DataMember]
		public bool TrustedListsOnly
		{
			get
			{
				return this.trustedListsOnly;
			}
			set
			{
				this.trustedListsOnly = value;
				base.TrackPropertyChanged("TrustedListsOnly");
			}
		}

		[DataMember]
		public bool ContactsTrusted
		{
			get
			{
				return this.contactsTrusted;
			}
			set
			{
				this.contactsTrusted = value;
				base.TrackPropertyChanged("ContactsTrusted");
			}
		}

		private string[] trustedSendersAndDomains;

		private string[] blockedSendersAndDomains;

		private bool enabled;

		private bool contactsTrusted;

		private bool trustedListsOnly;
	}
}
