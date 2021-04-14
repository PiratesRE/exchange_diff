using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.SoftDeletedRemoval
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SoftDeleteMailboxRemovalCheckRemoval : IExtensibleDataObject
	{
		private SoftDeleteMailboxRemovalCheckRemoval(bool canRemove, string reason)
		{
			this.CanRemove = canRemove;
			this.Reason = reason;
		}

		[DataMember]
		public bool CanRemove { get; set; }

		public ExtensionDataObject ExtensionData { get; set; }

		[DataMember]
		public string Reason { get; set; }

		public static SoftDeleteMailboxRemovalCheckRemoval AllowRemoval()
		{
			return new SoftDeleteMailboxRemovalCheckRemoval(true, string.Empty);
		}

		public static SoftDeleteMailboxRemovalCheckRemoval DisallowRemoval(string reasonMessage, params object[] formatArgs)
		{
			return new SoftDeleteMailboxRemovalCheckRemoval(false, string.Format(reasonMessage, formatArgs));
		}
	}
}
