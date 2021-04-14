using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AmMultiNodeReplicaNotifier : AmMultiNodeRpcMap
	{
		public AmMultiNodeReplicaNotifier(IADDatabase database, AmDbActionCode actionCode, bool isHighPriority) : base("AmMultiNodeReplicaNotifier")
		{
			this.Database = database;
			this.ActionCode = actionCode;
			this.IsHighPriority = isHighPriority;
		}

		private bool IsHighPriority { get; set; }

		private AmDbActionCode ActionCode { get; set; }

		private IADDatabase Database
		{
			get
			{
				return this.m_database;
			}
			set
			{
				this.m_database = value;
			}
		}

		public void SendAllNotifications()
		{
			ThreadPool.QueueUserWorkItem(delegate(object stateNotUsed)
			{
				this.SendAllNotificationsInternal();
			});
		}

		protected override Exception RunServerRpc(AmServerName node, out object result)
		{
			result = null;
			Exception result2 = null;
			try
			{
				Dependencies.ReplayRpcClientWrapper.NotifyChangedReplayConfiguration(node.Fqdn, this.Database.Guid, AmHelper.GetServerVersion(node), false, this.IsHighPriority, ReplayConfigChangeHints.AmMultiNodeReplicaNotifier);
			}
			catch (TransientException ex)
			{
				result2 = ex;
				AmTrace.Error("RunServerRpc(): Exception occurred: {0}", new object[]
				{
					ex
				});
			}
			catch (AmServerException ex2)
			{
				result2 = ex2;
				AmTrace.Error("RunServerRpc(): Exception occurred: {0}", new object[]
				{
					ex2
				});
			}
			catch (TaskServerException ex3)
			{
				result2 = ex3;
				AmTrace.Error("RunServerRpc(): Exception occurred: {0}", new object[]
				{
					ex3
				});
			}
			return result2;
		}

		protected override void UpdateStatus(AmServerName node, object result)
		{
		}

		private void SendAllNotificationsInternal()
		{
			try
			{
				if (!this.m_fInitialized)
				{
					List<AmServerName> nodeList = this.DetermineServersToContact();
					base.Initialize(nodeList);
					this.m_fInitialized = true;
				}
				base.RunAllRpcs();
			}
			catch (TransientException ex)
			{
				AmTrace.Error("SendAllNotificationsInternal(): Exception occurred: {0}", new object[]
				{
					ex
				});
			}
			catch (AmServerException ex2)
			{
				AmTrace.Error("SendAllNotificationsInternal(): Exception occurred: {0}", new object[]
				{
					ex2
				});
			}
		}

		private List<AmServerName> DetermineServersToContact()
		{
			Guid guid = this.Database.Guid;
			IADDatabase db = this.Database;
			IADDatabaseCopy[] databaseCopies = AmBestCopySelectionHelper.GetDatabaseCopies(guid, ref db);
			if (db != null)
			{
				this.Database = db;
			}
			AmConfig amConfig = AmSystemManager.Instance.Config;
			if (amConfig.IsUnknown)
			{
				AmTrace.Error("AmMultiNodeRpcNotifier: DB {0}: Invalid AM configuration", new object[]
				{
					db.Name
				});
				throw new AmInvalidConfiguration(amConfig.LastError ?? string.Empty);
			}
			IAmBcsErrorLogger errorLogger = new AmBcsSingleCopyFailureLogger();
			AmBcsServerChecks checksToRun = AmBcsServerChecks.ClusterNodeUp;
			if (this.ActionCode.IsAutomaticOperation)
			{
				checksToRun |= AmBcsServerChecks.DebugOptionDisabled;
			}
			IEnumerable<AmServerName> source = from dbCopy in databaseCopies
			where this.ValidateServer(new AmServerName(dbCopy.HostServerName), db, amConfig, checksToRun, errorLogger)
			select new AmServerName(dbCopy.HostServerName);
			return source.ToList<AmServerName>();
		}

		private bool ValidateServer(AmServerName serverName, IADDatabase db, AmConfig amConfig, AmBcsServerChecks checksToRun, IAmBcsErrorLogger errorLogger)
		{
			if (serverName.IsLocalComputerName)
			{
				return true;
			}
			LocalizedString empty = LocalizedString.Empty;
			AmBcsServerValidation amBcsServerValidation = new AmBcsServerValidation(serverName, null, db, amConfig, errorLogger, null);
			bool flag = amBcsServerValidation.RunChecks(checksToRun, ref empty);
			if (!flag)
			{
				AmTrace.Error("AmMultiNodeRpcNotifier: DB {0}: ValidateServer() returned error: {1}", new object[]
				{
					db.Name,
					empty
				});
			}
			return flag;
		}

		private bool m_fInitialized;

		private IADDatabase m_database;
	}
}
