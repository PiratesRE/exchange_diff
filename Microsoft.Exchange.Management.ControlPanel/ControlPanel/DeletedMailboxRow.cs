using System;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class DeletedMailboxRow : BaseRow
	{
		public DeletedMailboxRow(RemovedMailbox removedMailbox) : base(removedMailbox)
		{
			this.EmailAddress = (from x in removedMailbox.EmailAddresses
			where x.IsPrimaryAddress && x is SmtpProxyAddress
			select x).First<ProxyAddress>().AddressString;
			this.DeletionDate = removedMailbox.WhenChangedUTC.UtcToUserDateTimeString();
			this.DeletionDateTime = ((removedMailbox.WhenChangedUTC != null) ? removedMailbox.WhenChangedUTC.Value : DateTime.MinValue);
		}

		[DataMember]
		public string EmailAddress { get; protected set; }

		[DataMember]
		public string DeletionDate { get; protected set; }

		public DateTime DeletionDateTime { get; protected set; }
	}
}
