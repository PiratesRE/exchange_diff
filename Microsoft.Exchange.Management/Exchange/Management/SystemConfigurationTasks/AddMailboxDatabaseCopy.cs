using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Add", "MailboxDatabaseCopy", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class AddMailboxDatabaseCopy : AddDatabaseCopyTaskBase<Database>
	{
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "Identity", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 1)]
		public MailboxServerIdParameter MailboxServer
		{
			get
			{
				return (MailboxServerIdParameter)base.Fields["MailboxServer"];
			}
			set
			{
				base.Fields["MailboxServer"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public uint ActivationPreference
		{
			get
			{
				if (base.Fields["ActivationPreference"] == null)
				{
					return 0U;
				}
				return (uint)base.Fields["ActivationPreference"];
			}
			set
			{
				base.Fields["ActivationPreference"] = value;
			}
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			this.m_taskName = "Add-MailboxDatabaseCopy";
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageAddMailboxDatabaseCopy(this.DataObject.Name, this.m_server.Name);
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.HasErrors)
			{
				TaskLogger.LogExit();
				return;
			}
			this.m_server = (Server)base.GetDataObject<Server>(this.MailboxServer, base.DataSession, null, new LocalizedString?(Strings.ErrorMailboxServerNotFound(this.MailboxServer.ToString())), new LocalizedString?(Strings.ErrorMailboxServerNotUnique(this.MailboxServer.ToString())));
			if (!this.m_server.IsE14OrLater)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorServerNotE14OrLater(this.MailboxServer.ToString())), ErrorCategory.InvalidOperation, this.MailboxServer);
			}
			if (!Datacenter.IsMicrosoftHostedOnly(true) && this.DataObject.IsPublicFolderDatabase)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorInvalidOperationOnAddDBCopyForPublicFolder(this.DataObject.Name)), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			if (this.DataObject.Recovery)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorInvalidOperationOnAddDBCopyForRecoveryDB(this.DataObject.Name)), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			this.m_databaseCopies = this.DataObject.GetDatabaseCopies();
			if (this.m_databaseCopies == null || this.m_databaseCopies.Length == 0)
			{
				base.WriteError(new CopyConfigurationErrorException(Strings.ErrorCouldNotReadDatabaseCopy(this.DataObject.Name)), ErrorCategory.ReadError, this.DataObject.Identity);
			}
			else
			{
				if (this.DataObject.AllDatabaseCopies.Length == 1)
				{
					this.m_firstCopy = true;
					if (this.DataObject.CircularLoggingEnabled)
					{
						base.WriteError(new InvalidOperationException(Strings.ErrorInvalidAddOperationOnDBCopyForCircularLoggingEnabledDB(this.DataObject.Name)), ErrorCategory.InvalidOperation, this.DataObject.Identity);
					}
				}
				foreach (DatabaseCopy databaseCopy in this.m_databaseCopies)
				{
					if (databaseCopy.HostServer != null && databaseCopy.HostServer.ObjectGuid == this.m_server.Guid)
					{
						base.WriteError(new DbCopyAlreadyHostedOnServerException(this.DataObject.Identity.ToString(), this.m_server.Identity.ToString()), ErrorCategory.InvalidOperation, this.DataObject.Identity);
					}
				}
				foreach (DatabaseCopy databaseCopy2 in this.DataObject.InvalidDatabaseCopies)
				{
					if (databaseCopy2.Name.Equals(this.m_server.Name, StringComparison.OrdinalIgnoreCase))
					{
						this.m_invalidDbCopy = databaseCopy2;
						break;
					}
				}
			}
			if (base.Fields["ActivationPreference"] != null && (this.ActivationPreference < 1U || (ulong)this.ActivationPreference > (ulong)((long)(this.DataObject.AllDatabaseCopies.Length + 1))))
			{
				base.WriteError(new ArgumentException(Strings.ErrorActivationPreferenceInvalid(this.ActivationPreference, this.DataObject.AllDatabaseCopies.Length + 1)), ErrorCategory.InvalidArgument, null);
			}
			this.RcrValidation();
			base.VerifyIsWithinScopes((IConfigurationSession)base.DataSession, this.m_server, true, new DataAccessTask<Database>.ADObjectOutOfScopeString(Strings.ErrorServerOutOfScope));
			MapiTaskHelper.VerifyDatabaseIsWithinScope(base.SessionSettings, this.DataObject, new Task.ErrorLoggerDelegate(base.WriteError));
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			if (this.m_invalidDbCopy == null && !this.m_fConfigOnly)
			{
				ReplayState.DeleteState(this.m_server.Fqdn, this.DataObject, true);
			}
			DatabaseCopy databaseCopy = this.m_invalidDbCopy ?? new DatabaseCopy();
			databaseCopy.HostServer = (ADObjectId)this.m_server.Identity;
			if (base.Fields["ActivationPreference"] == null)
			{
				if (this.m_invalidDbCopy == null)
				{
					this.ActivationPreference = (uint)(this.DataObject.AllDatabaseCopies.Length + 1);
				}
				else
				{
					this.ActivationPreference = (uint)this.m_invalidDbCopy.ActivationPreference;
				}
			}
			databaseCopy.ActivationPreference = (int)this.ActivationPreference;
			DatabaseCopy databaseCopy2 = null;
			if (databaseCopy.ActivationPreference == 1)
			{
				databaseCopy2 = databaseCopy;
			}
			else if (this.ActivationPreference != 0U)
			{
				databaseCopy2 = SetMailboxDatabaseCopy.GetDatabaseCopyOfPreference1(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), this.DataObject, databaseCopy);
			}
			ActivationPreferenceSetter<DatabaseCopy> activationPreferenceSetter = new ActivationPreferenceSetter<DatabaseCopy>(this.DataObject.AllDatabaseCopies, databaseCopy, (this.m_invalidDbCopy == null) ? EntryAction.Insert : EntryAction.Modify);
			UpdateResult updateResult = activationPreferenceSetter.UpdateCachedValues();
			if (updateResult == UpdateResult.AllChanged)
			{
				activationPreferenceSetter.SaveAllUpdatedValues(base.DataSession);
			}
			databaseCopy.ReplayLagTime = this.m_replayLagTime;
			databaseCopy.TruncationLagTime = this.m_truncationLagTime;
			databaseCopy.SetId(this.DataObject.Id.GetChildId(this.m_server.Name.ToUpperInvariant()));
			databaseCopy.ParentObjectClass = (this.DataObject.IsPublicFolderDatabase ? PublicFolderDatabase.MostDerivedClass : MailboxDatabase.MostDerivedClass);
			if (databaseCopy2 != null)
			{
				base.WriteVerbose(Strings.UpdatingLegDnForDatabaseCopy(databaseCopy.Name));
				ITopologyConfigurationSession adSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.IgnoreInvalid, base.SessionSettings, 338, "InternalProcessRecord", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\database\\AddMailboxDatabaseCopy.cs");
				SetMailboxDatabaseCopy.UpdateServerLegdnForDatabaseSite(new Task.TaskErrorLoggingDelegate(base.WriteError), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), adSession, databaseCopy2);
			}
			base.WriteVerbose(Strings.UpdatingDatabaseCopyObject(databaseCopy.HostServer.Name, databaseCopy.ReplayLagTime.ToString(), databaseCopy.TruncationLagTime.ToString()));
			base.DataSession.Save(databaseCopy);
			MailboxDatabase database = databaseCopy.GetDatabase<MailboxDatabase>();
			if (this.m_firstCopy && database.DataMoveReplicationConstraint == DataMoveReplicationConstraintParameter.None)
			{
				database.DataMoveReplicationConstraint = DataMoveReplicationConstraintParameter.SecondCopy;
				base.DataSession.Save(database);
				base.WriteVerbose(Strings.ConstraintUpgrade(database.Identity.ToString(), DataMoveReplicationConstraintParameter.None, DataMoveReplicationConstraintParameter.SecondCopy));
			}
			ITopologyConfigurationSession topologyConfigurationSession = (ITopologyConfigurationSession)base.DataSession;
			ADObjectId[] array = DagTaskHelper.DetermineRemoteSites(topologyConfigurationSession, databaseCopy.OriginatingServer, this.m_server);
			if (array != null)
			{
				DagTaskHelper.ForceReplication(topologyConfigurationSession, databaseCopy, array, this.DataObject.Identity.ToString(), new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
			}
			if (!this.m_fConfigOnly)
			{
				this.RunSourceConfigurationUpdaterRpc();
			}
			if (!this.IsThirdPartyReplicationEnabled && !this.m_SeedingPostponedSpecified && !base.SeedingPostponed && !this.m_fConfigOnly)
			{
				base.CreateTargetEdbDirectory();
				base.PerformSeedIfNecessary();
			}
			if (!this.m_fConfigOnly)
			{
				this.RunTargetConfigurationUpdaterRpc();
			}
			this.WriteWarning(Strings.WarnAdministratorToRestartService(this.m_server.Name));
			TaskLogger.LogExit();
		}

		private void RunSourceConfigurationUpdaterRpc()
		{
			string text = this.DataObject.ServerName;
			MiniServer miniServer = (MiniServer)base.DataSession.Read<MiniServer>(this.DataObject.Server);
			if (miniServer == null)
			{
				this.WriteWarning(Strings.ErrorMailboxServerNotFound(text));
				return;
			}
			text = miniServer.Fqdn;
			DatabaseTasksHelper.RunConfigurationUpdaterRpc(text, this.DataObject, miniServer.AdminDisplayVersion, ReplayConfigChangeHints.DbCopyAdded, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning));
		}

		private void RunTargetConfigurationUpdaterRpc()
		{
			string fqdn = this.m_server.Fqdn;
			DatabaseTasksHelper.RunConfigurationUpdaterRpc(fqdn, this.DataObject, this.m_server.AdminDisplayVersion, ReplayConfigChangeHints.DbCopyAdded, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning));
		}

		private void RcrValidation()
		{
			int num = 0;
			try
			{
				Server server = this.DataObject.GetServer();
				if (server == null)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorDBOwningServerNotFound(this.DataObject.Identity.ToString())), ErrorCategory.InvalidOperation, this.DataObject.Identity);
				}
				else
				{
					RemoteReplayConfiguration.IsServerValidRcrTarget(ADObjectWrapperFactory.CreateWrapper(this.DataObject), ADObjectWrapperFactory.CreateWrapper(this.m_server), out num, server.Domain, true);
				}
			}
			catch (TransientException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			this.CheckDagMembership();
			base.InitializeLagTimes(this.m_invalidDbCopy);
			Database[] databases = this.m_server.GetDatabases();
			if (databases != null && databases.Length >= num)
			{
				base.WriteError(new RcrExceedDbLimitException(this.m_server.Name, num), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			string[] checkValue = new string[]
			{
				this.DataObject.LogFolderPath.PathName,
				this.DataObject.SystemFolderPath.PathName,
				this.DataObject.EdbFilePath.PathName
			};
			try
			{
				this.PathUniquenessValidationForRcr(databases, checkValue);
				if (!this.IsThirdPartyReplicationEnabled)
				{
					this.PathExistenceValidationForRcr(this.m_server.Fqdn, checkValue);
				}
			}
			catch (ArgumentException exception2)
			{
				base.WriteError(exception2, ErrorCategory.InvalidArgument, this.DataObject.Identity);
			}
		}

		private bool IsThirdPartyReplicationEnabled
		{
			get
			{
				return this.m_isThirdPartyReplicationEnabled;
			}
		}

		private void CheckDagMembership()
		{
			ADObjectId databaseAvailabilityGroup = this.m_server.DatabaseAvailabilityGroup;
			if (ADObjectId.IsNullOrEmpty(databaseAvailabilityGroup))
			{
				base.WriteError(new AddDatabaseCopyNewCopyMustBeInDagException(this.m_server.Name, this.DataObject.Name), ErrorCategory.InvalidArgument, null);
			}
			DatabaseAvailabilityGroup databaseAvailabilityGroup2 = (DatabaseAvailabilityGroup)base.DataSession.Read<DatabaseAvailabilityGroup>(databaseAvailabilityGroup);
			if (databaseAvailabilityGroup2 == null)
			{
				base.WriteError(new AddDatabaseCopyNewCopyMustBeInDagException(this.m_server.Name, this.DataObject.Name), ErrorCategory.InvalidArgument, null);
			}
			if (ThirdPartyReplicationMode.Enabled == databaseAvailabilityGroup2.ThirdPartyReplication)
			{
				this.m_isThirdPartyReplicationEnabled = true;
			}
			ITopologyConfigurationSession topologyConfigurationSession = (ITopologyConfigurationSession)base.DataSession;
			foreach (DatabaseCopy databaseCopy in this.m_databaseCopies)
			{
				ADObjectId hostServer = databaseCopy.HostServer;
				MiniServer miniServer = topologyConfigurationSession.ReadMiniServer(hostServer, AddMailboxDatabaseCopy.s_miniServerProperties);
				ADObjectId databaseAvailabilityGroup3 = miniServer.DatabaseAvailabilityGroup;
				if (ADObjectId.IsNullOrEmpty(databaseAvailabilityGroup3))
				{
					base.WriteError(new AddDatabaseCopyAllCopiesMustBeInTheDagException(this.DataObject.Name, this.m_server.Name, databaseAvailabilityGroup2.Name, miniServer.Name), ErrorCategory.InvalidArgument, null);
				}
				else if (!databaseAvailabilityGroup3.Equals(databaseAvailabilityGroup))
				{
					base.WriteError(new AddDatabaseCopyAllCopiesMustBeInSameDagException(this.DataObject.Name, this.m_server.Name, databaseAvailabilityGroup2.Name, miniServer.Name, databaseAvailabilityGroup3.Name), ErrorCategory.InvalidArgument, null);
				}
			}
		}

		private void PathUniquenessValidationForRcr(Database[] databases, string[] checkValue)
		{
			List<List<string>> list = new List<List<string>>(checkValue.Length);
			List<string> list2 = new List<string>(databases.Length * 2);
			List<string> list3 = new List<string>(databases.Length * 2);
			List<string> list4 = new List<string>(databases.Length * 2);
			foreach (Database database in databases)
			{
				list2.Add(database.LogFolderPath.PathName);
				list3.Add(database.SystemFolderPath.PathName);
				list4.Add(database.EdbFilePath.PathName);
			}
			list.Add(list2);
			list.Add(list3);
			list.Add(list4);
			for (int j = 0; j < checkValue.Length; j++)
			{
				if (!PathUniqueForRcrTargetCondition.Verify(checkValue[j], list[j]))
				{
					throw new ArgumentException(Strings.ErrorPathNotUniqueAmongExistingDbCopies(checkValue[j], AddMailboxDatabaseCopy.s_fieldInfo[j]));
				}
			}
		}

		private void PathExistenceValidationForRcr(string machine, string[] checkValue)
		{
			for (int i = 0; i < checkValue.Length; i++)
			{
				if (checkValue[i] != null && !new PathOnFixedOrNetworkDriveCondition(machine, checkValue[i]).Verify())
				{
					throw new ArgumentException(Strings.ErrorPathIsNotOnFixedDrive(AddMailboxDatabaseCopy.s_fieldInfo[i]));
				}
			}
		}

		internal const string ActivationPrefName = "ActivationPreference";

		private bool m_firstCopy;

		private DatabaseCopy[] m_databaseCopies;

		private DatabaseCopy m_invalidDbCopy;

		private static readonly PropertyDefinition[] s_miniServerProperties = new PropertyDefinition[]
		{
			ServerSchema.DatabaseAvailabilityGroup
		};

		private static string[] s_fieldInfo = new string[]
		{
			"LogFolderPath",
			"SystemFolderPath",
			"EdbFilePath"
		};

		private bool m_isThirdPartyReplicationEnabled;
	}
}
