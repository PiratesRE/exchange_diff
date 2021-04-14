using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.MailboxLoadBalance.SoftDeletedRemoval
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SoftDeletedMoveHistory
	{
		public SoftDeletedMoveHistory(int badItemsEncountered, int largeItemsEncountered, int missingItemsEncountered)
		{
			this.BadItemsEncountered = badItemsEncountered;
			this.LargeItemsEncountered = largeItemsEncountered;
			this.MissingItemsEncountered = missingItemsEncountered;
		}

		[DataMember]
		public int BadItemsEncountered { get; private set; }

		[DataMember]
		public int LargeItemsEncountered { get; private set; }

		[DataMember]
		public int MissingItemsEncountered { get; private set; }

		public static SoftDeletedMoveHistory GetHistoryForSourceDatabase(Guid mailboxGuid, Guid currentDatabase, Guid sourceDatabaseGuid)
		{
			List<MoveHistoryEntryInternal> source = MoveHistoryEntryInternal.LoadMoveHistory(mailboxGuid, currentDatabase, UserMailboxFlags.None);
			MoveHistoryEntryInternal moveHistoryEntryInternal = source.FirstOrDefault((MoveHistoryEntryInternal m) => m.SourceDatabase.ObjectGuid == sourceDatabaseGuid);
			if (moveHistoryEntryInternal == null)
			{
				return null;
			}
			return new SoftDeletedMoveHistory(moveHistoryEntryInternal.BadItemsEncountered, moveHistoryEntryInternal.LargeItemsEncountered, moveHistoryEntryInternal.MissingItemsEncountered);
		}

		public override bool Equals(object obj)
		{
			return !object.ReferenceEquals(null, obj) && (object.ReferenceEquals(this, obj) || (!(obj.GetType() != base.GetType()) && this.Equals((SoftDeletedMoveHistory)obj)));
		}

		public override int GetHashCode()
		{
			int num = this.BadItemsEncountered;
			num = (num * 397 ^ this.LargeItemsEncountered);
			return num * 397 ^ this.MissingItemsEncountered;
		}

		protected bool Equals(SoftDeletedMoveHistory other)
		{
			return this.BadItemsEncountered == other.BadItemsEncountered && this.LargeItemsEncountered == other.LargeItemsEncountered && this.MissingItemsEncountered == other.MissingItemsEncountered;
		}
	}
}
