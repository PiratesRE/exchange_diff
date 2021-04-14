using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class DatabaseInfo
	{
		internal DatabaseInfo(IADDatabase db, AmDbStateInfo stateInfo)
		{
			this.Database = db;
			this.StateInfo = stateInfo;
			this.StoreStatus = new Dictionary<AmServerName, MdbStatusFlags?>();
		}

		internal IADDatabase Database { get; private set; }

		internal AmDbStateInfo StateInfo { get; private set; }

		internal Dictionary<AmServerName, MdbStatusFlags?> StoreStatus { get; private set; }

		internal List<AmDbOperation> OperationsQueued { get; set; }

		internal AmServerName ActiveServer { get; private set; }

		internal bool IsMountedOnActive { get; private set; }

		internal bool IsActiveOnDisabledServer { get; private set; }

		internal bool IsMismounted { get; private set; }

		internal bool IsMountedButAdminRequestedDismount { get; private set; }

		internal List<AmServerName> MisMountedServerList { get; private set; }

		internal AmServerName OwningServer { get; private set; }

		internal bool IsAdPropertiesOutOfSync { get; private set; }

		internal bool IsClusterDatabaseOutOfSync { get; private set; }

		internal bool IsPeriodicMountRequired { get; private set; }

		internal void Analyze()
		{
			this.OwningServer = new AmServerName(this.Database.Server.Name);
			this.ActiveServer = this.StateInfo.ActiveServer;
			if (AmServerName.IsNullOrEmpty(this.ActiveServer))
			{
				this.ActiveServer = this.OwningServer;
			}
			if (!AmServerName.IsEqual(this.ActiveServer, this.OwningServer))
			{
				this.IsAdPropertiesOutOfSync = true;
			}
			if (this.Database.Servers.Length > 1 && AmBestCopySelectionHelper.IsActivationDisabled(this.ActiveServer))
			{
				this.IsActiveOnDisabledServer = true;
			}
			this.MisMountedServerList = new List<AmServerName>();
			foreach (AmServerName amServerName in this.StoreStatus.Keys)
			{
				if ((this.StoreStatus[amServerName] & MdbStatusFlags.Online) == MdbStatusFlags.Online)
				{
					if (AmServerName.IsEqual(amServerName, this.ActiveServer))
					{
						this.IsMountedOnActive = true;
					}
					else
					{
						this.MisMountedServerList.Add(amServerName);
					}
				}
				else if ((this.StoreStatus[amServerName] & MdbStatusFlags.MountInProgress) == MdbStatusFlags.MountInProgress && AmServerName.IsEqual(amServerName, this.ActiveServer))
				{
					this.IsMountedOnActive = true;
				}
			}
			this.IsMismounted = (this.MisMountedServerList != null && this.MisMountedServerList.Count > 0);
			this.IsMountedButAdminRequestedDismount = false;
			if (this.IsMountedOnActive)
			{
				if (this.StateInfo.IsAdminDismounted)
				{
					this.IsMountedButAdminRequestedDismount = true;
				}
				if (!this.StateInfo.IsMounted)
				{
					this.IsClusterDatabaseOutOfSync = true;
				}
			}
			else if (this.StateInfo.IsMounted)
			{
				this.IsClusterDatabaseOutOfSync = true;
			}
			this.IsPeriodicMountRequired = true;
			if (!this.IsMountedOnActive)
			{
				bool flag = AmSystemManager.Instance.StoreStateMarker.IsStoreGracefullyStoppedOn(this.ActiveServer);
				if (!this.Database.MountAtStartup || this.StateInfo.IsAdminDismounted || !this.StateInfo.IsMountAttemptedAtleastOnce || flag)
				{
					this.IsPeriodicMountRequired = false;
					ReplayCrimsonEvents.PeriodicCheckerSkippedMount.LogPeriodic<string, Guid, AmServerName, bool, bool, bool, bool>(this.Database.Guid, TimeSpan.FromMinutes(30.0), this.Database.Name, this.Database.Guid, this.ActiveServer, this.Database.MountAtStartup, this.StateInfo.IsAdminDismounted, this.StateInfo.IsMountAttemptedAtleastOnce, flag);
				}
			}
			else
			{
				this.IsPeriodicMountRequired = false;
			}
			if (this.StateInfo.IsMountSucceededAtleastOnce && !AmServerName.IsEqual(this.StateInfo.LastMountedServer, this.StateInfo.ActiveServer))
			{
				this.IsClusterDatabaseOutOfSync = true;
			}
		}

		internal bool IsActionsEqual(DatabaseInfo dbInfo)
		{
			return this.IsMismounted == dbInfo.IsMismounted && this.IsAdPropertiesOutOfSync == dbInfo.IsAdPropertiesOutOfSync && this.IsClusterDatabaseOutOfSync == dbInfo.IsClusterDatabaseOutOfSync && this.IsPeriodicMountRequired == dbInfo.IsPeriodicMountRequired && this.IsMountedOnActive == dbInfo.IsMountedOnActive && this.IsActiveOnDisabledServer == dbInfo.IsActiveOnDisabledServer && this.IsMountedButAdminRequestedDismount == dbInfo.IsMountedButAdminRequestedDismount;
		}

		internal bool IsActiveOnServerAndReplicated(AmServerName serverName)
		{
			return this.Database.ReplicationType == ReplicationType.Remote && AmServerName.IsEqual(this.ActiveServer, serverName);
		}
	}
}
