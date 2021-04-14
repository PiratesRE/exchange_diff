using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class DelegateSessionCache : IEnumerable<DelegateSessionEntry>, IEnumerable
	{
		internal DelegateSessionCache(int capacity)
		{
			if (capacity > 10 || capacity < 1)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			capacity = TestBridgeManager.GetSize(capacity);
			this.capacity = capacity;
			this.dataSet = new List<DelegateSessionEntry>(this.capacity);
		}

		internal static void SetTestBridge(ITestBridgeDelegateSessionCache bridge)
		{
			TestBridgeManager.SetBridge(bridge);
		}

		internal static void UnSetTestBridge()
		{
			TestBridgeManager.SetBridge(null);
		}

		internal bool TryGet(IExchangePrincipal principal, OpenBy openBy, out DelegateSessionEntry entry)
		{
			entry = null;
			if (principal == null || string.IsNullOrEmpty(principal.LegacyDn))
			{
				return false;
			}
			entry = this.FindEntry(principal);
			if (entry == null)
			{
				return false;
			}
			entry.Access(openBy);
			return true;
		}

		internal DelegateSessionEntry Add(DelegateSessionEntry entry)
		{
			if (entry == null)
			{
				throw new ArgumentNullException("entry");
			}
			return this.AddNewEntry(entry);
		}

		internal void Clear()
		{
			foreach (DelegateSessionEntry delegateSessionEntry in this.dataSet)
			{
				delegateSessionEntry.ForceDispose();
			}
			this.dataSet.Clear();
		}

		internal void RemoveEntry(DelegateSessionEntry entry)
		{
			this.dataSet.Remove(entry);
			entry.ForceDispose();
		}

		public IEnumerator<DelegateSessionEntry> GetEnumerator()
		{
			return this.dataSet.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private DelegateSessionEntry AddNewEntry(DelegateSessionEntry entry)
		{
			DelegateSessionEntry delegateSessionEntry = null;
			if (this.dataSet.Count >= this.capacity)
			{
				ExTraceGlobals.SessionTracer.TraceDebug<int, int>((long)this.GetHashCode(), "DelegateSessionCache::AddNewEntry. Overflow process. Capacity = {0}, Count = {1}.", this.capacity, this.dataSet.Count);
				foreach (DelegateSessionEntry delegateSessionEntry2 in this.dataSet)
				{
					if (delegateSessionEntry2.ExternalRefCount <= 0 && (delegateSessionEntry == null || delegateSessionEntry2.LastAccessed < delegateSessionEntry.LastAccessed))
					{
						delegateSessionEntry = delegateSessionEntry2;
					}
				}
				if (delegateSessionEntry == null)
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.AppendLine("CallStack for active DelegateSessionEntry:");
					foreach (DelegateSessionEntry delegateSessionEntry3 in this.dataSet)
					{
						if (delegateSessionEntry3.ExternalRefCount > 0)
						{
							stringBuilder.AppendLine(delegateSessionEntry3.GetCallStack());
						}
					}
					InvalidOperationException ex = new InvalidOperationException(string.Format("We cannot find a candidate from our internal cache. The consumer must remove unused sessions. Size = {0}, Capacity = {1}.", this.dataSet.Count, this.capacity));
					ex.Data["ActiveEntryCallStack"] = stringBuilder.ToString();
					throw ex;
				}
				TestBridgeManager.Removed(delegateSessionEntry.LegacyDn);
				this.dataSet.Remove(delegateSessionEntry);
				delegateSessionEntry.ForceDispose();
				delegateSessionEntry = null;
			}
			this.dataSet.Add(entry);
			TestBridgeManager.Added(entry.LegacyDn);
			return entry;
		}

		[Conditional("DEBUG")]
		private void DebugCheckNewEntry(IExchangePrincipal principal)
		{
			this.FindEntry(principal);
		}

		private DelegateSessionEntry FindEntry(IExchangePrincipal principal)
		{
			return this.dataSet.Find((DelegateSessionEntry e) => string.Compare(e.MailboxSession.MailboxOwnerLegacyDN, principal.LegacyDn, StringComparison.OrdinalIgnoreCase) == 0 && e.MailboxSession.MailboxOwner.IsCrossSiteAccessAllowed == principal.IsCrossSiteAccessAllowed);
		}

		private const int MaxSize = 10;

		private int capacity;

		private readonly List<DelegateSessionEntry> dataSet;
	}
}
