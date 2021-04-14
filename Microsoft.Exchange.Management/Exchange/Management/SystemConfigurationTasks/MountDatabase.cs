using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Data.Storage.Cluster;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Rpc.ActiveManager;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("mount", "Database", SupportsShouldProcess = true)]
	public sealed class MountDatabase : DatabaseActionTaskBase<Database>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageMountDatabase(this.Identity.ToString());
			}
		}

		[Parameter]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		[Parameter]
		public SwitchParameter AcceptDataLoss
		{
			get
			{
				return (SwitchParameter)(base.Fields["AcceptDataLoss"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["AcceptDataLoss"] = value;
			}
		}

		private IRecipientSession RecipientSession
		{
			get
			{
				if (this.recipientSession == null)
				{
					IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 208, "RecipientSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\database\\ActionsOnDatabase.cs");
					tenantOrRootOrgRecipientSession.EnforceDefaultScope = false;
					this.recipientSession = tenantOrRootOrgRecipientSession;
				}
				return this.recipientSession;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			MapiTaskHelper.VerifyDatabaseAndItsOwningServerInScope(base.SessionSettings, this.DataObject, new Task.ErrorLoggerDelegate(base.ThrowTerminatingError));
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject
			});
			try
			{
				try
				{
					MailboxDatabase mailboxDatabase = this.ConfigurationSession.Read<MailboxDatabase>((ADObjectId)this.DataObject.Identity);
					Server server = null;
					ADComputer adcomputer = null;
					bool useConfigNC = this.ConfigurationSession.UseConfigNC;
					bool useGlobalCatalog = this.ConfigurationSession.UseGlobalCatalog;
					if (mailboxDatabase != null)
					{
						server = mailboxDatabase.GetServer();
						try
						{
							this.ConfigurationSession.UseConfigNC = false;
							this.ConfigurationSession.UseGlobalCatalog = true;
							adcomputer = ((ITopologyConfigurationSession)this.ConfigurationSession).FindComputerByHostName(server.Name);
						}
						finally
						{
							this.ConfigurationSession.UseConfigNC = useConfigNC;
							this.ConfigurationSession.UseGlobalCatalog = useGlobalCatalog;
						}
						if (adcomputer == null)
						{
							base.WriteError(new ManagementObjectNotFoundException(Strings.ErrorDBOwningServerNotFound(mailboxDatabase.Identity.ToString())), ErrorCategory.ObjectNotFound, server.Identity);
						}
						ADObjectId adobjectId = adcomputer.Id.DomainId;
						adobjectId = adobjectId.GetChildId("Microsoft Exchange System Objects");
						adobjectId = adobjectId.GetChildId("SystemMailbox" + mailboxDatabase.Guid.ToString("B"));
						string identity = adobjectId.ToDNString();
						GeneralMailboxIdParameter generalMailboxIdParameter = GeneralMailboxIdParameter.Parse(identity);
						base.WriteVerbose(TaskVerboseStringHelper.GetFindByIdParameterVerboseString(generalMailboxIdParameter, this.RecipientSession, typeof(ADRecipient), null));
						IEnumerable<ADSystemMailbox> objects = generalMailboxIdParameter.GetObjects<ADSystemMailbox>(adobjectId, this.RecipientSession);
						using (IEnumerator<ADSystemMailbox> enumerator = objects.GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								ADSystemMailbox adsystemMailbox = enumerator.Current;
							}
							else
							{
								NewMailboxDatabase.SaveSystemMailbox(mailboxDatabase, mailboxDatabase.GetServer(), base.RootOrgContainerId, (ITopologyConfigurationSession)this.ConfigurationSession, this.RecipientSession, null, new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
							}
						}
					}
					base.WriteVerbose(Strings.VerboseMountDatabase(this.Identity.ToString()));
					this.RequestMount(MountFlags.None);
					if (!this.DataObject.DatabaseCreated)
					{
						this.DataObject.DatabaseCreated = true;
						base.InternalProcessRecord();
					}
				}
				catch (AmServerException ex)
				{
					Exception ex2;
					if (ex.TryGetInnerExceptionOfType(out ex2))
					{
						TaskLogger.Trace("Database already mounted (database={0}, exception={1})", new object[]
						{
							this.DataObject.Name,
							ex2.Message
						});
					}
					else if (ex.TryGetInnerExceptionOfType(out ex2) || ex.TryGetInnerExceptionOfType(out ex2))
					{
						this.AttemptForcedMountIfNecessary(this.Force, Strings.ContinueMountWhenDBFilesNotExist, Strings.VerboseMountDatabaseForcely(this.Identity.ToString()), Strings.ErrorFailedToMountReplicatedDbWithMissingEdbFile(this.Identity.ToString()), ex, MountFlags.ForceDatabaseCreation);
					}
					else if (ex.TryGetInnerExceptionOfType(out ex2))
					{
						this.PromptForMountIfNecessary(this.AcceptDataLoss, Strings.ContinueMountWithDataLoss, Strings.VerboseMountDatabaseDataLoss(this.Identity.ToString()), MountFlags.AcceptDataLoss);
					}
					else
					{
						TaskLogger.Trace("MountDatabase.InternalProcessRecord raises exception while mounting database: {0}", new object[]
						{
							ex.Message
						});
						base.WriteError(new InvalidOperationException(Strings.ErrorFailedToMountDatabase(this.Identity.ToString(), ex.Message), ex), ErrorCategory.InvalidOperation, this.DataObject.Identity);
					}
				}
			}
			catch (AmServerException ex3)
			{
				TaskLogger.Trace("MountDatabase.InternalProcessRecord raises exception while mounting database: {0}", new object[]
				{
					ex3.Message
				});
				base.WriteError(new InvalidOperationException(Strings.ErrorFailedToMountDatabase(this.Identity.ToString(), ex3.Message), ex3), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			catch (AmServerTransientException ex4)
			{
				TaskLogger.Trace("MountDatabase.InternalProcessRecord raises exception while mounting database: {0}", new object[]
				{
					ex4.Message
				});
				base.WriteError(new InvalidOperationException(Strings.ErrorFailedToMountDatabase(this.Identity.ToString(), ex4.Message), ex4), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		private void AttemptForcedMountIfNecessary(bool suppressPrompt, LocalizedString promptMessage, LocalizedString verboseMessage, LocalizedString replicatedDbErrorMessage, Exception mountException, MountFlags mountFlags)
		{
			if (this.DataObject.ReplicationType == ReplicationType.Remote)
			{
				TaskLogger.Trace("MountDatabase.InternalProcessRecord raised exception while mounting database: {0}", new object[]
				{
					mountException.Message
				});
				base.WriteError(new InvalidOperationException(replicatedDbErrorMessage, mountException), ErrorCategory.InvalidOperation, this.DataObject.Identity);
				return;
			}
			this.PromptForMountIfNecessary(suppressPrompt, promptMessage, verboseMessage, mountFlags);
		}

		private void PromptForMountIfNecessary(bool suppressPrompt, LocalizedString promptMessage, LocalizedString verboseMessage, MountFlags mountFlags)
		{
			if (suppressPrompt || base.ShouldContinue(promptMessage))
			{
				base.WriteVerbose(verboseMessage);
				this.RequestMount(mountFlags);
			}
		}

		private void RequestMount(MountFlags storeMountFlags)
		{
			AmMountFlags amMountFlags = AmMountFlags.None;
			if (this.Force)
			{
				amMountFlags |= AmMountFlags.MountWithForce;
			}
			AmRpcClientHelper.MountDatabase(ADObjectWrapperFactory.CreateWrapper(this.DataObject), (int)storeMountFlags, (int)amMountFlags, 0);
		}

		internal const string paramForce = "Force";

		internal const string paramAcceptDataLoss = "AcceptDataLoss";

		private const int OnlineShortTimeout = 50;

		private const int OnlineLongTimeout = 200;

		private IRecipientSession recipientSession;
	}
}
