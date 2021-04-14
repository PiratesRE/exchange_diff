using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ContactLinkingPerformanceTracker : PerformanceTrackerBase, IContactLinkingPerformanceTracker, IPerformanceTracker
	{
		public ContactLinkingPerformanceTracker(IMailboxSession mailboxSession) : base(mailboxSession)
		{
		}

		public void IncrementContactsCreated()
		{
			this.contactsCreated++;
		}

		public void IncrementContactsUpdated()
		{
			this.contactsUpdated++;
		}

		public void IncrementContactsRead()
		{
			this.contactsRead++;
		}

		public void IncrementContactsProcessed()
		{
			this.contactsProcessed++;
		}

		public ILogEvent GetLogEvent()
		{
			base.EnforceInternalState(PerformanceTrackerBase.InternalState.Stopped, "GetLogEvent");
			return new SchemaBasedLogEvent<ContactLinkingLogSchema.PerformanceData>
			{
				{
					ContactLinkingLogSchema.PerformanceData.Elapsed,
					base.ElapsedTime.TotalMilliseconds
				},
				{
					ContactLinkingLogSchema.PerformanceData.CPU,
					base.CpuTime.TotalMilliseconds
				},
				{
					ContactLinkingLogSchema.PerformanceData.RPCCount,
					base.StoreRpcCount
				},
				{
					ContactLinkingLogSchema.PerformanceData.RPCLatency,
					base.StoreRpcLatency.TotalMilliseconds
				},
				{
					ContactLinkingLogSchema.PerformanceData.DirectoryCount,
					base.DirectoryCount
				},
				{
					ContactLinkingLogSchema.PerformanceData.DirectoryLatency,
					base.DirectoryLatency.TotalMilliseconds
				},
				{
					ContactLinkingLogSchema.PerformanceData.StoreTimeInServer,
					base.StoreTimeInServer.TotalMilliseconds
				},
				{
					ContactLinkingLogSchema.PerformanceData.StoreTimeInCPU,
					base.StoreTimeInCPU.TotalMilliseconds
				},
				{
					ContactLinkingLogSchema.PerformanceData.StorePagesRead,
					base.StorePagesRead
				},
				{
					ContactLinkingLogSchema.PerformanceData.StorePagesPreRead,
					base.StorePagesPreread
				},
				{
					ContactLinkingLogSchema.PerformanceData.StoreLogRecords,
					base.StoreLogRecords
				},
				{
					ContactLinkingLogSchema.PerformanceData.StoreLogBytes,
					base.StoreLogBytes
				},
				{
					ContactLinkingLogSchema.PerformanceData.ContactsCreated,
					this.contactsCreated
				},
				{
					ContactLinkingLogSchema.PerformanceData.ContactsUpdated,
					this.contactsUpdated
				},
				{
					ContactLinkingLogSchema.PerformanceData.ContactsRead,
					this.contactsRead
				},
				{
					ContactLinkingLogSchema.PerformanceData.ContactsProcessed,
					this.contactsProcessed
				}
			};
		}

		private int contactsCreated;

		private int contactsUpdated;

		private int contactsRead;

		private int contactsProcessed;
	}
}
