using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.CapacityData
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class BatchCapacityDatum : IExtensibleDataObject, IComparable<BatchCapacityDatum>
	{
		public ExtensionDataObject ExtensionData { get; set; }

		[DataMember]
		public int MaximumNumberOfMailboxes { get; set; }

		public int CompareTo(BatchCapacityDatum other)
		{
			return this.MaximumNumberOfMailboxes - other.MaximumNumberOfMailboxes;
		}

		public override string ToString()
		{
			return string.Format("BatchDatum[{0} mailboxes]", this.MaximumNumberOfMailboxes);
		}
	}
}
