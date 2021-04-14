using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Optics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MailboxAssociationPerformanceTracker : PerformanceTrackerBase, IMailboxAssociationPerformanceTracker, IMailboxPerformanceTracker, IPerformanceTracker
	{
		public void IncrementAssociationsRead()
		{
			this.associationsRead++;
		}

		public void IncrementAssociationsCreated()
		{
			this.associationsCreated++;
		}

		public void IncrementAssociationsUpdated()
		{
			this.associationsUpdated++;
		}

		public void IncrementAssociationsDeleted()
		{
			this.associationsDeleted++;
		}

		public void IncrementAssociationReplicationAttempts()
		{
			this.associationReplicationAttempts++;
		}

		public void IncrementFailedAssociationReplications()
		{
			this.failedAssociationReplications++;
		}

		public void IncrementFailedAssociationsSearch()
		{
			this.failedAssociationsSearch++;
		}

		public void IncrementNonUniqueAssociationsFound()
		{
			this.nonUniqueAssociationsFound++;
		}

		public void IncrementAutoSubscribedMembers()
		{
			this.autoSubscribedMembers++;
		}

		public void IncrementMissingLegacyDns()
		{
			this.missingLegacyDns++;
		}

		public void SetNewSessionRequired(bool isRequired)
		{
			this.isNewSessionRequired = isRequired;
		}

		public void SetNewSessionWrongServer(bool isWrongServer)
		{
			this.isNewSessionWrongServer = isWrongServer;
		}

		public void SetNewSessionLatency(long milliseconds)
		{
			this.newSessionLatency = milliseconds;
		}

		public void SetAADQueryLatency(long milliseconds)
		{
			this.aadQueryLatency = milliseconds;
		}

		public ILogEvent GetLogEvent(string operationName)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("operationName", operationName);
			base.EnforceInternalState(PerformanceTrackerBase.InternalState.Stopped, "GetLogEvent");
			return new SchemaBasedLogEvent<MailboxAssociationLogSchema.OperationEnd>
			{
				{
					MailboxAssociationLogSchema.OperationEnd.OperationName,
					operationName
				},
				{
					MailboxAssociationLogSchema.OperationEnd.Elapsed,
					base.ElapsedTime.TotalMilliseconds
				},
				{
					MailboxAssociationLogSchema.OperationEnd.CPU,
					base.CpuTime.TotalMilliseconds
				},
				{
					MailboxAssociationLogSchema.OperationEnd.RPCCount,
					base.StoreRpcCount
				},
				{
					MailboxAssociationLogSchema.OperationEnd.RPCLatency,
					base.StoreRpcLatency.TotalMilliseconds
				},
				{
					MailboxAssociationLogSchema.OperationEnd.DirectoryCount,
					base.DirectoryCount
				},
				{
					MailboxAssociationLogSchema.OperationEnd.DirectoryLatency,
					base.DirectoryLatency.TotalMilliseconds
				},
				{
					MailboxAssociationLogSchema.OperationEnd.StoreTimeInServer,
					base.StoreTimeInServer.TotalMilliseconds
				},
				{
					MailboxAssociationLogSchema.OperationEnd.StoreTimeInCPU,
					base.StoreTimeInCPU.TotalMilliseconds
				},
				{
					MailboxAssociationLogSchema.OperationEnd.StorePagesRead,
					base.StorePagesRead
				},
				{
					MailboxAssociationLogSchema.OperationEnd.StorePagesPreRead,
					base.StorePagesPreread
				},
				{
					MailboxAssociationLogSchema.OperationEnd.StoreLogRecords,
					base.StoreLogRecords
				},
				{
					MailboxAssociationLogSchema.OperationEnd.StoreLogBytes,
					base.StoreLogBytes
				},
				{
					MailboxAssociationLogSchema.OperationEnd.NewSessionRequired,
					this.isNewSessionRequired
				},
				{
					MailboxAssociationLogSchema.OperationEnd.NewSessionWrong,
					this.isNewSessionWrongServer
				},
				{
					MailboxAssociationLogSchema.OperationEnd.NewSessionLatency,
					this.newSessionLatency
				},
				{
					MailboxAssociationLogSchema.OperationEnd.AssociationsRead,
					this.associationsRead
				},
				{
					MailboxAssociationLogSchema.OperationEnd.AssociationsCreated,
					this.associationsCreated
				},
				{
					MailboxAssociationLogSchema.OperationEnd.AssociationsUpdated,
					this.associationsUpdated
				},
				{
					MailboxAssociationLogSchema.OperationEnd.AssociationsDeleted,
					this.associationsDeleted
				},
				{
					MailboxAssociationLogSchema.OperationEnd.FailedAssociationsSearch,
					this.failedAssociationsSearch
				},
				{
					MailboxAssociationLogSchema.OperationEnd.MissingLegacyDns,
					this.missingLegacyDns
				},
				{
					MailboxAssociationLogSchema.OperationEnd.NonUniqueAssociationsFound,
					this.nonUniqueAssociationsFound
				},
				{
					MailboxAssociationLogSchema.OperationEnd.AutoSubscribedMembers,
					this.autoSubscribedMembers
				},
				{
					MailboxAssociationLogSchema.OperationEnd.AssociationReplicationAttempts,
					this.associationReplicationAttempts
				},
				{
					MailboxAssociationLogSchema.OperationEnd.FailedAssociationReplications,
					this.failedAssociationReplications
				},
				{
					MailboxAssociationLogSchema.OperationEnd.AADQueryLatency,
					this.aadQueryLatency
				}
			};
		}

		private int associationsRead;

		private int associationsCreated;

		private int associationsUpdated;

		private int associationsDeleted;

		private int associationReplicationAttempts;

		private int failedAssociationReplications;

		private int failedAssociationsSearch;

		private int nonUniqueAssociationsFound;

		private int autoSubscribedMembers;

		private int missingLegacyDns;

		private bool isNewSessionRequired;

		private bool isNewSessionWrongServer;

		private long newSessionLatency;

		private long aadQueryLatency;
	}
}
