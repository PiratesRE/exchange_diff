using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	internal class SubobjectReferenceState : IComponentData
	{
		private SubobjectReferenceState()
		{
			this.lockObject = new object();
		}

		internal static void Initialize()
		{
			if (SubobjectReferenceState.subobjectReferenceStateSlot == -1)
			{
				SubobjectReferenceState.subobjectReferenceStateSlot = MailboxState.AllocateComponentDataSlot(false);
			}
		}

		internal static SubobjectReferenceState GetState(Mailbox mailbox, bool create)
		{
			SubobjectReferenceState subobjectReferenceState = (SubobjectReferenceState)mailbox.SharedState.GetComponentData(SubobjectReferenceState.subobjectReferenceStateSlot);
			if (subobjectReferenceState == null && create)
			{
				subobjectReferenceState = new SubobjectReferenceState();
				SubobjectReferenceState subobjectReferenceState2 = (SubobjectReferenceState)mailbox.SharedState.CompareExchangeComponentData(SubobjectReferenceState.subobjectReferenceStateSlot, null, subobjectReferenceState);
				if (subobjectReferenceState2 != null)
				{
					subobjectReferenceState = subobjectReferenceState2;
				}
			}
			return subobjectReferenceState;
		}

		internal static SubobjectReferenceState GetState(MailboxState mailboxState)
		{
			return (SubobjectReferenceState)mailboxState.GetComponentData(SubobjectReferenceState.subobjectReferenceStateSlot);
		}

		bool IComponentData.DoCleanup(Context context)
		{
			bool result;
			using (LockManager.Lock(this.lockObject, LockManager.LockType.LeafMonitorLock, context.Diagnostics))
			{
				result = (this.inmemoryReferences == null || this.inmemoryReferences.Count == 0);
			}
			return result;
		}

		public void Addref(long inid)
		{
			using (LockManager.Lock(this.lockObject, LockManager.LockType.LeafMonitorLock))
			{
				if (this.inmemoryReferences == null)
				{
					this.inmemoryReferences = new Dictionary<long, SubobjectReferenceState.ReferenceState>(4);
				}
				if (this.inmemoryReferences.ContainsKey(inid))
				{
					SubobjectReferenceState.ReferenceState referenceState = this.inmemoryReferences[inid];
					if (referenceState.RefCounter == 2147483647)
					{
						throw new InvalidOperationException("refcount overflow");
					}
					this.inmemoryReferences[inid] = new SubobjectReferenceState.ReferenceState(referenceState.RefCounter + 1, referenceState.InTombStone);
				}
				else
				{
					this.inmemoryReferences.Add(inid, new SubobjectReferenceState.ReferenceState(1, false));
				}
			}
		}

		public void Release(Context context, long inid, Mailbox mailbox)
		{
			ILockStatistics lockStats = (context != null) ? context.Diagnostics : null;
			bool flag = false;
			using (LockManager.Lock(this.lockObject, LockManager.LockType.LeafMonitorLock, lockStats))
			{
				SubobjectReferenceState.ReferenceState referenceState;
				if (this.inmemoryReferences != null && this.inmemoryReferences.TryGetValue(inid, out referenceState))
				{
					if (referenceState.RefCounter == 1)
					{
						this.inmemoryReferences.Remove(inid);
						if (this.inmemoryReferences.Count == 0)
						{
							this.inmemoryReferences = null;
						}
						if (context != null && referenceState.InTombStone)
						{
							flag = true;
						}
					}
					else
					{
						this.inmemoryReferences[inid] = new SubobjectReferenceState.ReferenceState(referenceState.RefCounter - 1, referenceState.InTombStone);
					}
				}
			}
			if (flag)
			{
				SubobjectCleanup.NotifyCleanupMaintenanceIsRequired(context);
			}
		}

		public bool IsReferenced(long inid)
		{
			bool result;
			using (LockManager.Lock(this.lockObject, LockManager.LockType.LeafMonitorLock))
			{
				SubobjectReferenceState.ReferenceState referenceState;
				if (this.inmemoryReferences != null && this.inmemoryReferences.TryGetValue(inid, out referenceState))
				{
					result = (referenceState.RefCounter > 0);
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public bool IsInTombstone(long inid)
		{
			bool result;
			using (LockManager.Lock(this.lockObject, LockManager.LockType.LeafMonitorLock))
			{
				SubobjectReferenceState.ReferenceState referenceState;
				if (this.inmemoryReferences != null && this.inmemoryReferences.TryGetValue(inid, out referenceState))
				{
					result = referenceState.InTombStone;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public void OnAddToTombstone(long inid)
		{
			using (LockManager.Lock(this.lockObject, LockManager.LockType.LeafMonitorLock))
			{
				SubobjectReferenceState.ReferenceState referenceState;
				if (this.inmemoryReferences.TryGetValue(inid, out referenceState))
				{
					this.inmemoryReferences[inid] = new SubobjectReferenceState.ReferenceState(referenceState.RefCounter, true);
				}
			}
		}

		public void OnRemoveFromTombstone(long inid)
		{
			using (LockManager.Lock(this.lockObject, LockManager.LockType.LeafMonitorLock))
			{
				SubobjectReferenceState.ReferenceState referenceState;
				if (this.inmemoryReferences != null && this.inmemoryReferences.TryGetValue(inid, out referenceState))
				{
					this.inmemoryReferences[inid] = new SubobjectReferenceState.ReferenceState(referenceState.RefCounter, false);
				}
			}
		}

		private static int subobjectReferenceStateSlot = -1;

		private Dictionary<long, SubobjectReferenceState.ReferenceState> inmemoryReferences;

		private object lockObject;

		private struct ReferenceState
		{
			internal ReferenceState(int refCounter, bool inTombstone)
			{
				this.refCounter = refCounter;
				this.inTombStone = inTombstone;
			}

			internal int RefCounter
			{
				get
				{
					return this.refCounter;
				}
			}

			internal bool InTombStone
			{
				get
				{
					return this.inTombStone;
				}
			}

			private int refCounter;

			private bool inTombStone;
		}
	}
}
