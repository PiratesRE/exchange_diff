using System;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SoftDeletedMailboxRow : BaseRow
	{
		public SoftDeletedMailboxRow(Mailbox softDeletedMailbox) : base(softDeletedMailbox)
		{
			this.EmailAddress = (from x in softDeletedMailbox.EmailAddresses
			where x.IsPrimaryAddress && x is SmtpProxyAddress
			select x).First<ProxyAddress>().AddressString;
			this.DeletionDate = softDeletedMailbox.WhenSoftDeleted.Value.ToUniversalTime().UtcToUserDateTimeString();
			this.DeletionDateTime = ((softDeletedMailbox.WhenSoftDeleted != null) ? softDeletedMailbox.WhenSoftDeleted.Value : DateTime.MinValue);
		}

		[DataMember]
		public string EmailAddress { get; protected set; }

		[DataMember]
		public string DeletionDate { get; protected set; }

		public DateTime DeletionDateTime { get; protected set; }
	}
}
