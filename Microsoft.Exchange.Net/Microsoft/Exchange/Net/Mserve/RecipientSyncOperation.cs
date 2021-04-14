using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Net.Mserve
{
	internal sealed class RecipientSyncOperation
	{
		public RecipientSyncOperation(string distinguishedName, int partnerId, RecipientSyncState recipientSyncState, bool suppressSyncStateUpdate)
		{
			this.distinguishedName = distinguishedName;
			this.partnerId = partnerId;
			this.suppressSyncStateUpdate = suppressSyncStateUpdate;
			this.recipientSyncState = recipientSyncState;
			this.retryableEntries.Add(OperationType.Add, new List<FailedAddress>());
			this.retryableEntries.Add(OperationType.Delete, new List<FailedAddress>());
			this.retryableEntries.Add(OperationType.Read, new List<FailedAddress>());
			this.nonRetryableEntries.Add(OperationType.Add, new List<FailedAddress>());
			this.nonRetryableEntries.Add(OperationType.Delete, new List<FailedAddress>());
			this.nonRetryableEntries.Add(OperationType.Read, new List<FailedAddress>());
			this.pendingSyncStateCommitEntries.Add(OperationType.Add, new List<string>());
			this.pendingSyncStateCommitEntries.Add(OperationType.Delete, new List<string>());
		}

		public RecipientSyncOperation() : this(null, -1, null, true)
		{
		}

		public List<string> ReadEntries
		{
			get
			{
				return this.readEntries;
			}
		}

		public List<string> AddedEntries
		{
			get
			{
				return this.addedEntries;
			}
		}

		public List<string> RemovedEntries
		{
			get
			{
				return this.removedEntries;
			}
		}

		public Dictionary<OperationType, List<string>> PendingSyncStateCommitEntries
		{
			get
			{
				return this.pendingSyncStateCommitEntries;
			}
		}

		public int TotalPendingSyncStateCommitEntries
		{
			get
			{
				int num = 0;
				foreach (List<string> list in this.pendingSyncStateCommitEntries.Values)
				{
					num += list.Count;
				}
				return num;
			}
		}

		public Dictionary<OperationType, List<FailedAddress>> RetryableEntries
		{
			get
			{
				return this.retryableEntries;
			}
		}

		public Dictionary<OperationType, List<FailedAddress>> NonRetryableEntries
		{
			get
			{
				return this.nonRetryableEntries;
			}
		}

		public Dictionary<string, string> AddressTypeTable
		{
			get
			{
				return this.addressTypeTable;
			}
		}

		public List<string> DuplicatedAddEntries
		{
			get
			{
				return this.duplicatedAddEntries;
			}
		}

		public int PartnerId
		{
			get
			{
				return this.partnerId;
			}
			set
			{
				this.partnerId = value;
			}
		}

		public string DistinguishedName
		{
			get
			{
				return this.distinguishedName;
			}
		}

		public RecipientSyncState RecipientSyncState
		{
			get
			{
				return this.recipientSyncState;
			}
		}

		public bool SuppressSyncStateUpdate
		{
			get
			{
				return this.suppressSyncStateUpdate;
			}
		}

		public bool Synchronized
		{
			get
			{
				return this.completedSyncCount == this.addedEntries.Count + this.removedEntries.Count + this.readEntries.Count;
			}
		}

		public int CompletedSyncCount
		{
			get
			{
				return this.completedSyncCount;
			}
			set
			{
				this.completedSyncCount = value;
			}
		}

		public bool HasRetryableErrors
		{
			get
			{
				return this.retryableEntries[OperationType.Add].Count > 0 || this.retryableEntries[OperationType.Delete].Count > 0 || this.retryableEntries[OperationType.Read].Count > 0;
			}
		}

		public bool HasNonRetryableErrors
		{
			get
			{
				return this.nonRetryableEntries[OperationType.Add].Count > 0 || this.nonRetryableEntries[OperationType.Delete].Count > 0 || this.nonRetryableEntries[OperationType.Read].Count > 0;
			}
		}

		private readonly List<string> addedEntries = new List<string>();

		private readonly List<string> removedEntries = new List<string>();

		private readonly List<string> readEntries = new List<string>();

		private readonly Dictionary<OperationType, List<FailedAddress>> retryableEntries = new Dictionary<OperationType, List<FailedAddress>>();

		private readonly Dictionary<OperationType, List<FailedAddress>> nonRetryableEntries = new Dictionary<OperationType, List<FailedAddress>>();

		private readonly Dictionary<OperationType, List<string>> pendingSyncStateCommitEntries = new Dictionary<OperationType, List<string>>();

		private readonly Dictionary<string, string> addressTypeTable = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		private readonly List<string> duplicatedAddEntries = new List<string>();

		private readonly string distinguishedName;

		private readonly RecipientSyncState recipientSyncState;

		private readonly bool suppressSyncStateUpdate;

		private int completedSyncCount;

		private int partnerId;
	}
}
