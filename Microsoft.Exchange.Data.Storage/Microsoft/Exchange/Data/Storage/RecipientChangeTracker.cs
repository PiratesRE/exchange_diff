using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RecipientChangeTracker : IRecipientChangeTracker
	{
		void IRecipientChangeTracker.AddRecipient(CoreRecipient coreRecipient, out bool considerRecipientModified)
		{
			considerRecipientModified = false;
			int num = this.removedRecipientIds.IndexOf(coreRecipient.RowId);
			if (num != -1)
			{
				this.removedRecipientIds.RemoveAt(num);
				this.modifiedRecipients.Add(coreRecipient);
				considerRecipientModified = true;
				return;
			}
			int num2 = this.addedRecipients.BinarySearch(coreRecipient, RecipientChangeTracker.CoreRecipientComparer.Default);
			this.addedRecipients.Insert(~num2, coreRecipient);
		}

		void IRecipientChangeTracker.RemoveAddedRecipient(CoreRecipient coreRecipient)
		{
			int index = this.addedRecipients.BinarySearch(coreRecipient, RecipientChangeTracker.CoreRecipientComparer.Default);
			this.addedRecipients.RemoveAt(index);
		}

		void IRecipientChangeTracker.RemoveUnchangedRecipient(CoreRecipient coreRecipient)
		{
			if (coreRecipient.Participant != null)
			{
				this.removedRecipients[coreRecipient.Participant] = coreRecipient;
			}
			this.removedRecipientIds.Add(coreRecipient.RowId);
		}

		void IRecipientChangeTracker.RemoveModifiedRecipient(CoreRecipient coreRecipient)
		{
			for (int i = 0; i < this.modifiedRecipients.Count; i++)
			{
				if (this.modifiedRecipients[i] == coreRecipient)
				{
					this.modifiedRecipients.RemoveAt(i);
					break;
				}
			}
			if (coreRecipient.Participant != null)
			{
				this.removedRecipients[coreRecipient.Participant] = coreRecipient;
			}
			this.removedRecipientIds.Add(coreRecipient.RowId);
		}

		void IRecipientChangeTracker.OnModifyRecipient(CoreRecipient coreRecipient)
		{
			this.modifiedRecipients.Add(coreRecipient);
		}

		internal bool IsDirty
		{
			get
			{
				return this.addedRecipients.Count + this.modifiedRecipients.Count + this.removedRecipientIds.Count > 0;
			}
		}

		internal void ClearRemovedRecipients()
		{
			this.removedRecipientIds.Clear();
			this.removedRecipients.Clear();
		}

		internal void ClearAddedRecipients()
		{
			this.addedRecipients.Clear();
		}

		internal IList<CoreRecipient> AddedRecipients
		{
			get
			{
				return this.addedRecipients;
			}
		}

		internal IList<int> RemovedRecipientIds
		{
			get
			{
				return this.removedRecipientIds;
			}
		}

		internal IList<CoreRecipient> ModifiedRecipients
		{
			get
			{
				return this.modifiedRecipients;
			}
		}

		internal bool FindRemovedRecipient(Participant participant, out CoreRecipient recipient)
		{
			return this.removedRecipients.TryGetValue(participant, out recipient);
		}

		private readonly List<CoreRecipient> addedRecipients = new List<CoreRecipient>();

		private readonly Dictionary<Participant, CoreRecipient> removedRecipients = new Dictionary<Participant, CoreRecipient>(Participant.AddressEqualityComparer);

		private readonly List<int> removedRecipientIds = new List<int>();

		private readonly List<CoreRecipient> modifiedRecipients = new List<CoreRecipient>();

		private class CoreRecipientComparer : IComparer<CoreRecipient>
		{
			private CoreRecipientComparer()
			{
			}

			int IComparer<CoreRecipient>.Compare(CoreRecipient x, CoreRecipient y)
			{
				return x.RowId.CompareTo(y.RowId);
			}

			internal static RecipientChangeTracker.CoreRecipientComparer Default = new RecipientChangeTracker.CoreRecipientComparer();
		}
	}
}
