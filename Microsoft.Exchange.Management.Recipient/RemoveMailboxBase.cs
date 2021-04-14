using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Exchange.Data.Mapi.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public class RemoveMailboxBase<TIdentity> : RemoveRecipientObjectTask<TIdentity, ADUser> where TIdentity : IIdentityParameter
	{
		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public bool Permanent
		{
			get
			{
				return (bool)(base.Fields["Permanent"] ?? false);
			}
			set
			{
				base.Fields["Permanent"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "StoreMailboxIdentity")]
		public DatabaseIdParameter Database
		{
			get
			{
				return (DatabaseIdParameter)base.Fields["Database"];
			}
			set
			{
				base.Fields["Database"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "StoreMailboxIdentity")]
		public StoreMailboxIdParameter StoreMailboxIdentity
		{
			get
			{
				return (StoreMailboxIdParameter)base.Fields["StoreMailboxIdentity"];
			}
			set
			{
				base.Fields["StoreMailboxIdentity"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public SwitchParameter KeepWindowsLiveID
		{
			get
			{
				return (SwitchParameter)(base.Fields["KeepWindowsLiveID"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["KeepWindowsLiveID"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Arbitration
		{
			get
			{
				return (SwitchParameter)(base.Fields["Arbitration"] ?? false);
			}
			set
			{
				base.Fields["Arbitration"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter PublicFolder
		{
			get
			{
				return (SwitchParameter)(base.Fields["PublicFolder"] ?? false);
			}
			set
			{
				base.Fields["PublicFolder"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter RemoveLastArbitrationMailboxAllowed
		{
			get
			{
				return (SwitchParameter)(base.Fields["RemoveLastArbitrationMailboxAllowed"] ?? false);
			}
			set
			{
				base.Fields["RemoveLastArbitrationMailboxAllowed"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter RemoveArbitrationMailboxWithOABsAllowed
		{
			get
			{
				return (SwitchParameter)(base.Fields["RemoveArbitrationMailboxWithOABsAllowed"] ?? false);
			}
			set
			{
				base.Fields["RemoveArbitrationMailboxWithOABsAllowed"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IgnoreLegalHold
		{
			get
			{
				return (SwitchParameter)(base.Fields["IgnoreLegalHold"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["IgnoreLegalHold"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Force { get; set; }

		internal virtual bool ArbitrationMailboxUsageValidationRequired
		{
			get
			{
				return true;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Disconnect
		{
			get
			{
				return (SwitchParameter)(base.Fields["Disconnect"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Disconnect"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter AuditLog
		{
			get
			{
				return (SwitchParameter)(base.Fields["AuditLog"] ?? false);
			}
			set
			{
				base.Fields["AuditLog"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if ("StoreMailboxIdentity" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageRemoveMailboxStoreMailboxIdentity(this.Database.ToString(), this.StoreMailboxIdentity.ToString());
				}
				if (base.DataObject.NetID != null)
				{
					if (this.KeepWindowsLiveID)
					{
						if (this.Permanent)
						{
							TIdentity identity = this.Identity;
							return Strings.ConfirmationMessageRemoveMailboxPermanentAndNotLiveId(identity.ToString(), base.DataObject.WindowsLiveID.ToString());
						}
						TIdentity identity2 = this.Identity;
						return Strings.ConfirmationMessageRemoveMailboxIdentityAndNotLiveId(identity2.ToString(), base.DataObject.WindowsLiveID.ToString());
					}
					else
					{
						if (this.Permanent)
						{
							TIdentity identity3 = this.Identity;
							return Strings.ConfirmationMessageRemoveMailboxPermanentAndLiveId(identity3.ToString(), base.DataObject.WindowsLiveID.ToString());
						}
						TIdentity identity4 = this.Identity;
						return Strings.ConfirmationMessageRemoveMailboxIdentityAndLiveId(identity4.ToString(), base.DataObject.WindowsLiveID.ToString());
					}
				}
				else
				{
					if (this.Permanent)
					{
						TIdentity identity5 = this.Identity;
						return Strings.ConfirmationMessageRemoveMailboxPermanent(identity5.ToString());
					}
					TIdentity identity6 = this.Identity;
					return Strings.ConfirmationMessageRemoveMailboxIdentity(identity6.ToString());
				}
			}
		}

		private IConfigurationSession TenantLocalConfigurationSession
		{
			get
			{
				IConfigurationSession result;
				if ((result = this.tenantLocalConfigurationSession) == null)
				{
					result = (this.tenantLocalConfigurationSession = RecipientTaskHelper.GetTenantLocalConfigSession(base.CurrentOrganizationId, base.ExecutingUserOrganizationId, base.RootOrgContainerId, false, ((IRecipientSession)base.DataSession).LastUsedDc.ToString(), null));
				}
				return result;
			}
		}

		protected override void InternalStateReset()
		{
			base.InternalStateReset();
			this.DisposeMapiSession();
		}

		protected override IConfigDataProvider CreateSession()
		{
			IRecipientSession recipientSession = (IRecipientSession)base.CreateSession();
			if (this.Disconnect.IsPresent || base.ForReconciliation.IsPresent)
			{
				recipientSession = SoftDeletedTaskHelper.GetSessionForSoftDeletedObjects(recipientSession, null);
			}
			return recipientSession;
		}

		private void InternalValidateStoreMailboxIdentity()
		{
			TaskLogger.LogEnter();
			MailboxDatabase mailboxDatabase = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(this.Database, base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(this.Database.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(this.Database.ToString())), ExchangeErrorCategory.Client);
			if (mailboxDatabase.Recovery)
			{
				base.WriteError(new TaskArgumentException(Strings.ErrorInvalidOperationOnRecoveryMailboxDatabase(this.Database.ToString())), ExchangeErrorCategory.Client, this.StoreMailboxIdentity);
			}
			DatabaseLocationInfo databaseLocationInfo = null;
			try
			{
				databaseLocationInfo = ActiveManager.GetActiveManagerInstance().GetServerForDatabase(mailboxDatabase.Id.ObjectGuid);
			}
			catch (ObjectNotFoundException exception)
			{
				base.WriteError(exception, ExchangeErrorCategory.ServerOperation, null);
			}
			try
			{
				base.WriteVerbose(Strings.VerboseConnectionAdminRpcInterface(databaseLocationInfo.ServerFqdn));
				this.mapiSession = new MapiAdministrationSession(databaseLocationInfo.ServerLegacyDN, Fqdn.Parse(databaseLocationInfo.ServerFqdn));
			}
			catch (MapiPermanentException exception2)
			{
				base.WriteError(exception2, ExchangeErrorCategory.ServerOperation, null);
			}
			catch (MapiRetryableException exception3)
			{
				base.WriteError(exception3, ExchangeErrorCategory.ServerTransient, null);
			}
			this.database = mailboxDatabase;
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			this.isToInactiveMailbox = false;
			this.isDisconnectInactiveMailbox = false;
			if (this.Identity != null)
			{
				base.InternalValidate();
				this.isToInactiveMailbox = this.IsToInactiveMailbox();
				this.isDisconnectInactiveMailbox = this.IsDisconnectInactiveMailbox();
				if (!this.isToInactiveMailbox && !this.isDisconnectInactiveMailbox)
				{
					MailboxTaskHelper.BlockRemoveOrDisableIfLitigationHoldEnabled(base.DataObject, new Task.ErrorLoggerDelegate(base.WriteError), false, this.IgnoreLegalHold.ToBool());
					MailboxTaskHelper.BlockRemoveOrDisableIfDiscoveryHoldEnabled(base.DataObject, new Task.ErrorLoggerDelegate(base.WriteError), false, this.IgnoreLegalHold.ToBool());
				}
				MailboxTaskHelper.BlockRemoveOrDisableIfJournalNDRMailbox(base.DataObject, this.TenantLocalConfigurationSession, new Task.ErrorLoggerDelegate(base.WriteError), false);
				if (ComplianceConfigImpl.JournalArchivingHardeningEnabled && !this.skipJournalArchivingCheck)
				{
					MailboxTaskHelper.BlockRemoveOrDisableMailboxIfJournalArchiveEnabled(base.DataSession as IRecipientSession, this.ConfigurationSession, base.DataObject, new Task.ErrorLoggerDelegate(base.WriteError), false);
				}
				if (base.DataObject.RecipientTypeDetails == RecipientTypeDetails.ArbitrationMailbox && this.ArbitrationMailboxUsageValidationRequired)
				{
					ADUser dataObject = base.DataObject;
					Task.ErrorLoggerDelegate writeError = new Task.ErrorLoggerDelegate(base.WriteError);
					TIdentity identity = this.Identity;
					MailboxTaskHelper.ValidateNotBuiltInArbitrationMailbox(dataObject, writeError, Strings.ErrorRemoveArbitrationMailbox(identity.ToString()));
					ADUser dataObject2 = base.DataObject;
					IRecipientSession tenantGlobalCatalogSession = base.TenantGlobalCatalogSession;
					Task.ErrorLoggerDelegate writeError2 = new Task.ErrorLoggerDelegate(base.WriteError);
					TIdentity identity2 = this.Identity;
					MailboxTaskHelper.ValidateArbitrationMailboxHasNoGroups(dataObject2, tenantGlobalCatalogSession, writeError2, Strings.ErrorRemoveMailboxWithAssociatedApprovalRecipents(identity2.ToString()));
					ADUser dataObject3 = base.DataObject;
					bool overrideCheck = this.RemoveArbitrationMailboxWithOABsAllowed.ToBool();
					Task.ErrorLoggerDelegate writeError3 = new Task.ErrorLoggerDelegate(base.WriteError);
					TIdentity identity3 = this.Identity;
					MailboxTaskHelper.ValidateNoOABsAssignedToArbitrationMailbox(dataObject3, overrideCheck, writeError3, Strings.ErrorRemoveArbitrationMailboxWithOABsAssigned(identity3.ToString()));
					ADUser dataObject4 = base.DataObject;
					IRecipientSession tenantGlobalCatalogSession2 = base.TenantGlobalCatalogSession;
					ADObjectId rootOrgContainerId = base.RootOrgContainerId;
					bool isPresent = this.RemoveLastArbitrationMailboxAllowed.IsPresent;
					Task.ErrorLoggerDelegate writeError4 = new Task.ErrorLoggerDelegate(base.WriteError);
					TIdentity identity4 = this.Identity;
					MailboxTaskHelper.ValidateNotLastArbitrationMailbox(dataObject4, tenantGlobalCatalogSession2, rootOrgContainerId, isPresent, writeError4, Strings.ErrorCannotRemoveLastArbitrationMailboxInOrganization(identity4.ToString()));
				}
				if (this.AuditLog)
				{
					if (base.DataObject.RecipientTypeDetails != RecipientTypeDetails.AuditLogMailbox)
					{
						LocalizedException exception = new RecipientTaskException(Strings.ErrorSpecifiedMailboxShouldBeAuditLogMailbox(base.DataObject.Identity.ToString()));
						ExchangeErrorCategory category = ExchangeErrorCategory.Context;
						TIdentity identity5 = this.Identity;
						base.WriteError(exception, category, identity5.ToString());
					}
				}
				else if (base.DataObject.RecipientTypeDetails == RecipientTypeDetails.AuditLogMailbox)
				{
					LocalizedException exception2 = new RecipientTaskException(Strings.ErrorAuditLogMailboxShouldBeDeletedWithAuditLogSpecified(base.DataObject.Identity.ToString()));
					ExchangeErrorCategory category2 = ExchangeErrorCategory.Context;
					TIdentity identity6 = this.Identity;
					base.WriteError(exception2, category2, identity6.ToString());
				}
			}
			else
			{
				this.InternalValidateStoreMailboxIdentity();
				try
				{
					this.mailboxStatistics = (MailboxStatistics)base.GetDataObject<MailboxStatistics>(this.StoreMailboxIdentity, this.mapiSession, MapiTaskHelper.ConvertDatabaseADObjectToDatabaseId(this.database), new LocalizedString?(Strings.ErrorStoreMailboxNotFound(this.StoreMailboxIdentity.ToString(), this.Database.ToString())), new LocalizedString?(Strings.ErrorStoreMailboxNotUnique(this.StoreMailboxIdentity.ToString(), this.Database.ToString())), ExchangeErrorCategory.Client);
					MailboxTaskHelper.ValidateMailboxIsDisconnected(base.TenantGlobalCatalogSession, this.mailboxStatistics.MailboxGuid, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError));
					this.mailboxStatistics.Database = this.database.Identity;
				}
				catch (DataSourceTransientException exception3)
				{
					base.WriteError(exception3, ExchangeErrorCategory.ServerTransient, this.StoreMailboxIdentity);
				}
			}
			if (this.PublicFolder)
			{
				Organization orgContainer = this.TenantLocalConfigurationSession.GetOrgContainer();
				if (orgContainer.DefaultPublicFolderMailbox.HierarchyMailboxGuid == Guid.Empty && !this.Force)
				{
					LocalizedException exception4 = new RecipientTaskException(Strings.ErrorPrimaryPublicFolderMailboxNotFound);
					ExchangeErrorCategory category3 = ExchangeErrorCategory.Context;
					TIdentity identity7 = this.Identity;
					base.WriteError(exception4, category3, identity7.ToString());
				}
				if (this.currentOrganizationId == null || this.currentOrganizationId != base.DataObject.OrganizationId)
				{
					this.currentOrganizationId = base.DataObject.OrganizationId;
					TenantPublicFolderConfigurationCache.Instance.RemoveValue(base.DataObject.OrganizationId);
				}
				MailboxTaskHelper.RemoveOrDisablePublicFolderMailbox(base.DataObject, Guid.Empty, this.tenantLocalConfigurationSession, new Task.ErrorLoggerDelegate(base.WriteError), false, this.Force);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			ADObjectId adobjectId = null;
			Guid guid = Guid.Empty;
			string text = string.Empty;
			bool flag = this.ShouldSoftDeleteObject();
			ADUser dataObject = base.DataObject;
			if (Globals.IsMicrosoftHostedOnly)
			{
				if (flag)
				{
					bool flag2 = SoftDeletedTaskHelper.MSOSyncEnabled(this.ConfigurationSession, dataObject.OrganizationId);
					bool includeInGarbageCollection = (!flag2 || base.ForReconciliation) && !this.isToInactiveMailbox;
					SoftDeletedTaskHelper.UpdateRecipientForSoftDelete(base.DataSession as IRecipientSession, dataObject, includeInGarbageCollection, this.isToInactiveMailbox);
				}
				else
				{
					if (this.isDisconnectInactiveMailbox)
					{
						SoftDeletedTaskHelper.UpdateMailboxForDisconnectInactiveMailbox(dataObject);
						base.DataSession.Save(dataObject);
						TaskLogger.LogExit();
						this.LogRemoveMailboxDetails(dataObject);
						return;
					}
					dataObject.RecipientSoftDeletedStatus = 0;
				}
			}
			if (this.Identity != null)
			{
				adobjectId = base.DataObject.Database;
				guid = base.DataObject.ExchangeGuid;
				if (adobjectId == null)
				{
					TaskLogger.Trace("The homeMDB is empty for this user, we just try to remove the user AD object", new object[0]);
				}
				else if (guid == Guid.Empty)
				{
					TaskLogger.Trace("The ExchangeGuid is empty for this user, we just try to remove the user AD object", new object[0]);
				}
				else if (base.DataObject.RecipientTypeDetails == RecipientTypeDetails.MailboxPlan)
				{
					TaskLogger.Trace("This user is MailboxPlan, we just try to remove the user AD object", new object[0]);
				}
				else if (!flag)
				{
					try
					{
						DatabaseLocationInfo databaseLocationInfo = null;
						try
						{
							databaseLocationInfo = ActiveManager.GetActiveManagerInstance().GetServerForDatabase(adobjectId.ObjectGuid);
						}
						catch (ObjectNotFoundException exception)
						{
							base.WriteError(exception, ExchangeErrorCategory.ServerOperation, null);
						}
						if (databaseLocationInfo == null)
						{
							if (this.Permanent)
							{
								base.WriteError(new TaskInvalidOperationException(Strings.ErrorGetServerNameFromMailbox(base.DataObject.Identity.ToString())), ExchangeErrorCategory.ServerOperation, base.DataObject);
							}
							else
							{
								TaskLogger.Trace("cannot get the server name for mailbox {0}", new object[]
								{
									base.DataObject.Identity
								});
							}
						}
						else
						{
							text = databaseLocationInfo.ServerFqdn;
							base.WriteVerbose(Strings.VerboseConnectionAdminRpcInterface(text));
							this.mapiSession = new MapiAdministrationSession(databaseLocationInfo.ServerLegacyDN, Fqdn.Parse(text));
						}
					}
					catch (MapiPermanentException ex)
					{
						if (this.Permanent)
						{
							base.WriteError(ex, ExchangeErrorCategory.ServerOperation, this.Identity);
						}
						else
						{
							TaskLogger.Trace("Swallowing exception {0} from mapi.net", new object[]
							{
								ex
							});
						}
					}
					catch (MapiRetryableException ex2)
					{
						if (this.Permanent)
						{
							base.WriteError(ex2, ExchangeErrorCategory.ServerTransient, this.Identity);
						}
						else
						{
							TaskLogger.Trace("Swallowing exception {0} from mapi.net", new object[]
							{
								ex2
							});
						}
					}
					try
					{
						if (dataObject != null && base.ForReconciliation)
						{
							base.DataObject.ExternalDirectoryObjectId = string.Empty;
						}
						base.DataObject.PreviousDatabase = base.DataObject.Database;
						base.DataSession.Save(base.DataObject);
					}
					catch (DataSourceTransientException exception2)
					{
						base.WriteError(exception2, ExchangeErrorCategory.ServerTransient, null);
					}
					catch (InvalidObjectOperationException)
					{
					}
					catch (DataValidationException)
					{
					}
					catch (ADOperationException)
					{
					}
				}
				base.InternalProcessRecord();
				this.LogRemoveMailboxDetails(dataObject);
			}
			if (this.StoreMailboxIdentity != null || this.Permanent)
			{
				if (this.Permanent)
				{
					if (!(guid != Guid.Empty) || adobjectId == null)
					{
						goto IL_5A0;
					}
					try
					{
						base.WriteVerbose(Strings.VerboseDeleteMailboxInStore(guid.ToString(), adobjectId.ToString()));
						this.mapiSession.DeleteMailbox(new MailboxId(MapiTaskHelper.ConvertDatabaseADObjectIdToDatabaseId(adobjectId), guid));
						goto IL_5A0;
					}
					catch (Microsoft.Exchange.Data.Mapi.Common.MailboxNotFoundException ex3)
					{
						TaskLogger.Trace("Swallowing exception {0} from mapi.net", new object[]
						{
							ex3
						});
						base.WriteVerbose(ex3.LocalizedString);
						goto IL_5A0;
					}
					catch (DataSourceOperationException exception3)
					{
						base.WriteError(exception3, ExchangeErrorCategory.ServerOperation, base.DataObject);
						goto IL_5A0;
					}
				}
				try
				{
					base.WriteVerbose(Strings.VerboseDeleteMailboxInStore(this.mailboxStatistics.MailboxGuid.ToString(), this.mailboxStatistics.Database.ToString()));
					((IConfigDataProvider)this.mapiSession).Delete(this.mailboxStatistics);
					goto IL_5A0;
				}
				catch (DataSourceOperationException exception4)
				{
					base.WriteError(exception4, ExchangeErrorCategory.ServerOperation, (this.Identity == null) ? this.StoreMailboxIdentity : this.Identity);
					goto IL_5A0;
				}
			}
			if (this.mapiSession != null)
			{
				try
				{
					TIdentity identity = this.Identity;
					base.WriteVerbose(Strings.VerboseSyncMailboxWithDS(identity.ToString(), base.DataObject.Database.ToString(), text));
					bool flag3 = true;
					if (base.DataObject.Database == null || Guid.Empty == base.DataObject.Database.ObjectGuid)
					{
						flag3 = false;
						TaskLogger.Trace("Cannot get the database for mailbox '{0}'", new object[]
						{
							base.DataObject.Identity
						});
					}
					if (Guid.Empty == base.DataObject.ExchangeGuid)
					{
						flag3 = false;
						TaskLogger.Trace("Cannot get the mailbox guid for mailbox '{0}'", new object[]
						{
							base.DataObject.Identity
						});
					}
					if (flag3)
					{
						this.mapiSession.SyncMailboxWithDS(new MailboxId(MapiTaskHelper.ConvertDatabaseADObjectIdToDatabaseId(base.DataObject.Database), base.DataObject.ExchangeGuid));
					}
				}
				catch (Microsoft.Exchange.Data.Mapi.Common.MailboxNotFoundException ex4)
				{
					TaskLogger.Trace("Swallowing exception {0} from mapi.net", new object[]
					{
						ex4
					});
					base.WriteVerbose(ex4.LocalizedString);
				}
				catch (DataSourceTransientException ex5)
				{
					TaskLogger.Trace("Swallowing exception {0} from mapi.net", new object[]
					{
						ex5
					});
					this.WriteWarning(ex5.LocalizedString);
				}
				catch (DataSourceOperationException ex6)
				{
					TaskLogger.Trace("Swallowing exception {0} from mapi.net", new object[]
					{
						ex6
					});
					this.WriteWarning(ex6.LocalizedString);
				}
			}
			IL_5A0:
			if (!flag && this.mailboxStatistics != null)
			{
				this.mailboxStatistics.Dispose();
				this.mailboxStatistics = null;
			}
			this.DisposeMapiSession();
			TaskLogger.LogExit();
		}

		private void LogRemoveMailboxDetails(ADUser recipient)
		{
			Guid uniqueId = base.CurrentTaskContext.UniqueId;
			CmdletLogger.SafeAppendGenericInfo(uniqueId, "ExchangeGuid", recipient.ExchangeGuid.ToString());
			CmdletLogger.SafeAppendGenericInfo(uniqueId, "HomeMBD", (recipient.Database == null) ? string.Empty : recipient.Database.ToString());
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.DisposeMapiSession();
			}
			base.Dispose(disposing);
		}

		private void DisposeMapiSession()
		{
			if (this.mapiSession != null)
			{
				this.mapiSession.Dispose();
				this.mapiSession = null;
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return Mailbox.FromDataObject((ADUser)dataObject);
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || exception is StorageTransientException || exception is StoragePermanentException;
		}

		private bool IsToInactiveMailbox()
		{
			ADUser dataObject = base.DataObject;
			return this.ShouldSoftDeleteObject() && SoftDeletedTaskHelper.MSOSyncEnabled(this.ConfigurationSession, dataObject.OrganizationId) && dataObject.IsInLitigationHoldOrInplaceHold && !this.IgnoreLegalHold;
		}

		private bool IsDisconnectInactiveMailbox()
		{
			ADUser dataObject = base.DataObject;
			return dataObject.IsInactiveMailbox && this.Disconnect && !this.IgnoreLegalHold && (dataObject.LitigationHoldEnabled || (dataObject.InPlaceHolds != null && dataObject.InPlaceHolds.Count > 0));
		}

		private const string ParameterPermanent = "Permanent";

		private const string ParameterDatabase = "Database";

		private const string ParameterStoreMailboxIdentity = "StoreMailboxIdentity";

		private const string ParameterKeepWindowsLiveID = "KeepWindowsLiveID";

		private const string ParameterDisconnect = "Disconnect";

		internal const string ParameterSetStoreMailboxIdentity = "StoreMailboxIdentity";

		private MapiAdministrationSession mapiSession;

		private Database database;

		private MailboxStatistics mailboxStatistics;

		private bool isToInactiveMailbox;

		private bool isDisconnectInactiveMailbox;

		private OrganizationId currentOrganizationId;

		private IConfigurationSession tenantLocalConfigurationSession;

		protected bool skipJournalArchivingCheck;
	}
}
