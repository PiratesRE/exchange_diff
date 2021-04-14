using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.UnifiedPolicy;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AuditPolicyCacheEntry : IUpdatableItem
	{
		public PolicyLoadStatus LoadStatus { get; private set; }

		public MailboxAuditOperations AuditOperationsDelegate { get; private set; }

		public AuditPolicyCacheEntry(MailboxAuditOperations auditDelegateOperations = MailboxAuditOperations.None, PolicyLoadStatus loadStatus = PolicyLoadStatus.Unknown)
		{
			this.AuditOperationsDelegate = auditDelegateOperations;
			this.LoadStatus = loadStatus;
		}

		public bool IsExist()
		{
			return this.LoadStatus == PolicyLoadStatus.FailedToLoad || this.LoadStatus == PolicyLoadStatus.Loaded;
		}

		public bool UpdateWith(IUpdatableItem newItem)
		{
			AuditPolicyCacheEntry auditPolicyCacheEntry = newItem as AuditPolicyCacheEntry;
			if (auditPolicyCacheEntry == null)
			{
				return false;
			}
			if (AuditPolicyCacheEntry.CanUpdate(auditPolicyCacheEntry.LoadStatus, this.LoadStatus))
			{
				this.LoadStatus = auditPolicyCacheEntry.LoadStatus;
				this.AuditOperationsDelegate = auditPolicyCacheEntry.AuditOperationsDelegate;
				return true;
			}
			return false;
		}

		public static bool CanUpdate(PolicyLoadStatus loadStatus, PolicyLoadStatus cachedStatus)
		{
			return AuditPolicyCacheEntry.CanUpdateCachedEntry[(int)loadStatus][(int)cachedStatus];
		}

		// Note: this type is marked as 'beforefieldinit'.
		static AuditPolicyCacheEntry()
		{
			bool[][] array = new bool[4][];
			bool[][] array2 = array;
			int num = 0;
			bool[] array3 = new bool[4];
			array2[num] = array3;
			array[1] = new bool[]
			{
				true,
				false,
				true,
				true
			};
			bool[][] array4 = array;
			int num2 = 2;
			bool[] array5 = new bool[4];
			array5[0] = true;
			array5[1] = true;
			array4[num2] = array5;
			array[3] = new bool[]
			{
				true,
				true,
				true,
				true
			};
			AuditPolicyCacheEntry.CanUpdateCachedEntry = array;
		}

		private static readonly bool[][] CanUpdateCachedEntry;
	}
}
