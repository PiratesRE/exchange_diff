using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.ActiveManager;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal abstract class AmSystemMoveBase : AmBatchOperationBase
	{
		internal AmSystemMoveBase(AmServerName nodeName)
		{
			this.m_nodeName = nodeName;
		}

		internal void TPRMoveCompletion(object unused)
		{
			lock (this.m_tprLocker)
			{
				this.m_tprNotificationsAcknowledged++;
				if (this.m_tprNotificationsAcknowledged == this.m_moveRequests)
				{
					base.MarkAllDone();
				}
			}
		}

		protected IADDatabase[] GetDatabases()
		{
			IADToplogyConfigurationSession iadtoplogyConfigurationSession = ADSessionFactory.CreateIgnoreInvalidRootOrgSession(true);
			IADServer iadserver = iadtoplogyConfigurationSession.FindServerByName(this.m_nodeName.NetbiosName);
			if (iadserver == null)
			{
				throw new ServerNotFoundException(this.m_nodeName.NetbiosName);
			}
			return iadtoplogyConfigurationSession.GetAllDatabases(iadserver).ToArray<IADDatabase>();
		}

		protected void MoveDatabases(AmDbActionCode actionCode)
		{
			IADDatabase[] databases = this.GetDatabases();
			this.MoveDatabases(actionCode, databases);
		}

		protected void MoveDatabases(AmDbActionCode actionCode, IADDatabase[] dbList)
		{
			if (dbList == null || dbList.Length <= 0)
			{
				AmTrace.Info("Server '{0}' does not have any databases that need to be moved", new object[]
				{
					this.m_nodeName
				});
				return;
			}
			if (!this.m_amConfig.DagConfig.IsThirdPartyReplEnabled)
			{
				ThreadPoolThreadCountHelper.IncreaseForDatabaseOperations(dbList.Length);
				this.MoveDatabasesNormally(actionCode, dbList);
				return;
			}
			this.SendThirdPartyNotifications(dbList);
		}

		protected abstract void PrepareMoveArguments(ref AmDbMoveArguments moveArgs);

		private void MoveDatabasesNormally(AmDbActionCode actionCode, IADDatabase[] dbList)
		{
			AmDbNodeAttemptTable dbNodeAttemptTable = AmSystemManager.Instance.DbNodeAttemptTable;
			LocalizedString reason = LocalizedString.Empty;
			foreach (IADDatabase iaddatabase in dbList)
			{
				if (iaddatabase.ReplicationType == ReplicationType.Remote && this.IsActiveOnServer(iaddatabase, this.m_nodeName, out reason))
				{
					dbNodeAttemptTable.ClearFailedTime(iaddatabase.Guid);
					AmDbMoveOperation amDbMoveOperation = new AmDbMoveOperation(iaddatabase, actionCode);
					amDbMoveOperation.Arguments.SourceServer = this.m_nodeName;
					AmDbMoveArguments arguments = amDbMoveOperation.Arguments;
					this.PrepareMoveArguments(ref arguments);
					amDbMoveOperation.Arguments = arguments;
					this.m_moveRequests++;
					base.EnqueueDatabaseOperation(amDbMoveOperation);
				}
				else
				{
					if (iaddatabase.ReplicationType != ReplicationType.Remote)
					{
						reason = ReplayStrings.AmDbMoveOperationNotSupportedException(iaddatabase.Name);
					}
					AmDbSkippedMoveOperation operation = new AmDbSkippedMoveOperation(iaddatabase, reason);
					this.m_moveRequests++;
					base.EnqueueDatabaseOperation(operation);
				}
			}
			base.StartDatabaseOperations();
		}

		private void SendThirdPartyNotifications(IADDatabase[] dbList)
		{
			this.m_derivedManagesAllDone = true;
			int num = 0;
			try
			{
				this.m_tprMoveList = new List<AmActiveThirdPartyMove>();
				LocalizedString empty = LocalizedString.Empty;
				foreach (IADDatabase iaddatabase in dbList)
				{
					if (this.IsActiveOnServer(iaddatabase, this.m_nodeName, out empty))
					{
						AmDbStateInfo amDbStateInfo = this.m_amConfig.DbState.Read(iaddatabase.Guid);
						bool mountDesired = !amDbStateInfo.IsAdminDismounted;
						AmActiveThirdPartyMove item = new AmActiveThirdPartyMove(iaddatabase, this.m_nodeName.Fqdn, mountDesired);
						this.m_tprMoveList.Add(item);
						this.m_moveRequests++;
					}
				}
				foreach (AmActiveThirdPartyMove amActiveThirdPartyMove in this.m_tprMoveList)
				{
					amActiveThirdPartyMove.Notify(new WaitCallback(this.TPRMoveCompletion));
					num++;
				}
			}
			finally
			{
				lock (this.m_tprLocker)
				{
					if (num < this.m_moveRequests)
					{
						this.m_moveRequests -= num;
					}
					if (this.m_tprNotificationsAcknowledged == this.m_moveRequests)
					{
						base.MarkAllDone();
					}
				}
			}
		}

		private bool IsActiveOnServer(IADDatabase db, AmServerName movingFromServer, out LocalizedString skipReason)
		{
			bool result = false;
			AmDbStateInfo amDbStateInfo = this.m_amConfig.DbState.Read(db.Guid);
			if (amDbStateInfo.IsEntryExist)
			{
				if (AmServerName.IsEqual(movingFromServer, amDbStateInfo.ActiveServer))
				{
					result = true;
					skipReason = LocalizedString.Empty;
				}
				else
				{
					AmTrace.Info("IsActiveOnServer: Excluding database {0} from moving since it is not active on {1} (active={2})", new object[]
					{
						db.Name,
						movingFromServer,
						amDbStateInfo.ActiveServer
					});
					skipReason = ReplayStrings.DbMoveSkippedBecauseNotActive(db.Name, movingFromServer.NetbiosName, amDbStateInfo.ActiveServer.NetbiosName);
				}
			}
			else
			{
				AmTrace.Error("IsActiveOnServer: Failed to find db state info from clusdb (db={0})", new object[]
				{
					db.Name
				});
				skipReason = ReplayStrings.DbMoveSkippedBecauseNotFoundInClusDb(db.Name);
			}
			return result;
		}

		protected override List<AmServerName> GetServers()
		{
			return new List<AmServerName>
			{
				this.m_nodeName
			};
		}

		protected AmServerName m_nodeName;

		private List<AmActiveThirdPartyMove> m_tprMoveList;

		private int m_tprNotificationsAcknowledged;

		private object m_tprLocker = new object();
	}
}
