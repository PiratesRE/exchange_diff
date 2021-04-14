using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class MailboxOptions : OptionsPropertyChangeTracker
	{
		[DataMember]
		public string AddressString
		{
			get
			{
				return this.addressString;
			}
			set
			{
				this.addressString = value;
				base.TrackPropertyChanged("AddressString");
			}
		}

		[DataMember]
		public bool DeliverToMailboxAndForward
		{
			get
			{
				return this.deliverToMailboxAndForward;
			}
			set
			{
				this.deliverToMailboxAndForward = value;
				base.TrackPropertyChanged("DeliverToMailboxAndForward");
			}
		}

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

		private string addressString;

		private bool deliverToMailboxAndForward;

		private Identity identity;
	}
}
