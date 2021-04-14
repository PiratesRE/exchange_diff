using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Data.Storage.Cluster;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class RemoveDatabaseTask<TDataObject> : RemoveSystemConfigurationObjectTask<DatabaseIdParameter, TDataObject> where TDataObject : Database, new()
	{
		internal virtual IRecipientSession RecipientSessionForSystemMailbox
		{
			get
			{
				if (this.recipientSessionForSystemMailbox == null)
				{
					IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 62, "RecipientSessionForSystemMailbox", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\database\\RemoveDatabase.cs");
					tenantOrRootOrgRecipientSession.UseGlobalCatalog = false;
					this.recipientSessionForSystemMailbox = tenantOrRootOrgRecipientSession;
				}
				return this.recipientSessionForSystemMailbox;
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || SystemConfigurationTasksHelper.IsKnownWmiException(exception) || SystemConfigurationTasksHelper.IsKnownMapiDotNETException(exception) || SystemConfigurationTasksHelper.IsKnownClusterUpdateDatabaseResourceException(exception);
		}

		protected override void TranslateException(ref Exception e, out ErrorCategory category)
		{
			base.TranslateException(ref e, out category);
			category = (ErrorCategory)1001;
			if (SystemConfigurationTasksHelper.IsKnownMapiDotNETException(e))
			{
				e = new InvalidOperationException(Strings.ErrorFailedToGetDatabaseStatus(this.Identity.ToString()), e);
			}
		}

		protected void CheckDatabaseStatus()
		{
			TDataObject dataObject = base.DataObject;
			this.server = dataObject.GetServer();
			if (this.server == null)
			{
				TDataObject dataObject2 = base.DataObject;
				this.WriteWarning(Strings.ErrorDBOwningServerNotFound(dataObject2.Identity.ToString()));
				return;
			}
			TDataObject dataObject3 = base.DataObject;
			base.WriteVerbose(Strings.VerboseConnectionAdminRpcInterface(this.server.Fqdn));
			using (IStoreRpc newStoreControllerInstance = Dependencies.GetNewStoreControllerInstance(this.server.Fqdn))
			{
				base.WriteVerbose(Strings.VerboseCheckDatabaseStatus(dataObject3.Identity.ToString()));
				Guid[] dbGuids = new Guid[]
				{
					dataObject3.Guid
				};
				MdbStatus[] array;
				if (this.ListMdbStatus(newStoreControllerInstance, dbGuids, out array) == null)
				{
					if (array.Length == 0)
					{
						TaskLogger.Trace("The database being removed does not have storage", new object[0]);
					}
					else if (MdbStatusFlags.Backup == (array[0].Status & MdbStatusFlags.Backup))
					{
						base.WriteError(new InvalidOperationException(Strings.ErrorBackupInProgress(dataObject3.Name)), ErrorCategory.InvalidOperation, dataObject3.Identity);
					}
				}
			}
		}

		private Exception ListMdbStatus(IStoreRpc rpcAdmin, Guid[] dbGuids, out MdbStatus[] status)
		{
			Exception result = null;
			status = null;
			TDataObject dataObject = base.DataObject;
			try
			{
				status = rpcAdmin.ListMdbStatus(dbGuids);
			}
			catch (MapiExceptionNetworkError mapiExceptionNetworkError)
			{
				result = mapiExceptionNetworkError;
			}
			catch (MapiExceptionNotFound mapiExceptionNotFound)
			{
				result = mapiExceptionNotFound;
			}
			catch (MapiRetryableException ex)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorFailedToUnmountDatabase(dataObject.Identity.ToString(), ex.Message)), ErrorCategory.InvalidOperation, dataObject.Identity);
			}
			catch (MapiPermanentException ex2)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorFailedToUnmountDatabase(dataObject.Identity.ToString(), ex2.Message)), ErrorCategory.InvalidOperation, dataObject.Identity);
			}
			return result;
		}

		protected void DismountDatabase()
		{
			TDataObject dataObject = base.DataObject;
			base.WriteVerbose(Strings.VerboseConnectionAdminRpcInterface(this.server.Fqdn));
			using (IStoreRpc newStoreControllerInstance = Dependencies.GetNewStoreControllerInstance(this.server.Fqdn))
			{
				base.WriteVerbose(Strings.VerboseCheckDatabaseStatus(dataObject.Identity.ToString()));
				Guid[] dbGuids = new Guid[]
				{
					dataObject.Guid
				};
				bool flag = false;
				MdbStatus[] array;
				if (this.ListMdbStatus(newStoreControllerInstance, dbGuids, out array) == null)
				{
					if (array == null || array.Length == 0)
					{
						TaskLogger.Trace("The database being removed does not have storage", new object[0]);
						return;
					}
					if (MdbStatusFlags.Backup == (array[0].Status & MdbStatusFlags.Backup))
					{
						base.WriteError(new InvalidOperationException(Strings.ErrorBackupInProgress(dataObject.Name)), ErrorCategory.InvalidOperation, dataObject.Identity);
					}
					else if (MdbStatusFlags.Online == (array[0].Status & MdbStatusFlags.Online))
					{
						flag = true;
					}
				}
				if (flag)
				{
					base.WriteVerbose(Strings.VerboseUnmountDatabase(this.Identity.ToString()));
					TDataObject dataObject2 = base.DataObject;
					if (dataObject2.IsExchange2009OrLater)
					{
						try
						{
							AmRpcClientHelper.DismountDatabase(ADObjectWrapperFactory.CreateWrapper(base.DataObject), 0);
							goto IL_300;
						}
						catch (AmServerException ex)
						{
							Exception ex2;
							if (ex.TryGetExceptionOrInnerOfType(out ex2))
							{
								TaskLogger.Trace("RemoveDatabase.InternalProcessRecord raises exception while unmounting database: {0}", new object[]
								{
									ex2.Message
								});
							}
							else if (ex.TryGetExceptionOrInnerOfType(out ex2))
							{
								TaskLogger.Trace("RemoveDatabase.InternalProcessRecord raises exception while unmounting database: {0}", new object[]
								{
									ex2.Message
								});
							}
							else if (ex.TryGetExceptionOrInnerOfType(out ex2))
							{
								TaskLogger.Trace("RemoveDatabase.InternalProcessRecord raises exception while unmounting database: {0}", new object[]
								{
									ex2.Message
								});
							}
							else if (ex.TryGetExceptionOrInnerOfType(out ex2))
							{
								TaskLogger.Trace("RemoveDatabase.InternalProcessRecord raises exception while unmounting database: {0}", new object[]
								{
									ex2.Message
								});
							}
							else if (ex.TryGetExceptionOrInnerOfType(out ex2))
							{
								TaskLogger.Trace("RemoveDatabase.InternalProcessRecord raises exception while unmounting database: {0}", new object[]
								{
									ex2.Message
								});
							}
							else
							{
								TaskLogger.Trace("RemoveDatabase.InternalProcessRecord raises exception while unmounting database: {0}", new object[]
								{
									ex.Message
								});
								Exception exception = ex;
								ErrorCategory category = ErrorCategory.InvalidOperation;
								TDataObject dataObject3 = base.DataObject;
								base.WriteError(exception, category, dataObject3.Identity);
							}
							goto IL_300;
						}
						catch (AmServerTransientException ex3)
						{
							TaskLogger.Trace("RemoveDatabase.InternalProcessRecord raises exception while dismounting database: {0}", new object[]
							{
								ex3.Message
							});
							Exception exception2 = ex3;
							ErrorCategory category2 = ErrorCategory.InvalidOperation;
							TDataObject dataObject4 = base.DataObject;
							base.WriteError(exception2, category2, dataObject4.Identity);
							goto IL_300;
						}
					}
					IConfigDataProvider dataSession = base.DataSession;
					TDataObject dataObject5 = base.DataObject;
					LegacyPublicFolderDatabase legacyPublicFolderDatabase = (LegacyPublicFolderDatabase)dataSession.Read<LegacyPublicFolderDatabase>(dataObject5.Identity);
					StorageGroup storageGroup = (StorageGroup)base.DataSession.Read<StorageGroup>(legacyPublicFolderDatabase.StorageGroup);
					newStoreControllerInstance.UnmountDatabase(storageGroup.Guid, legacyPublicFolderDatabase.Guid, 0);
				}
				IL_300:;
			}
		}

		protected void RemoveMonitoringMailboxes()
		{
			MailboxDatabase mailboxDatabase = base.DataObject as MailboxDatabase;
			if (mailboxDatabase != null)
			{
				try
				{
					MailboxTaskHelper.RemoveMonitoringMailboxes(mailboxDatabase, new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
				}
				catch (DataSourceTransientException ex)
				{
					TaskLogger.Trace("The action of Removing monitoring mailbox objects of database '{0}' raises exception: {1}.", new object[]
					{
						mailboxDatabase.Identity.ToString(),
						ex.Message
					});
					this.WriteWarning(Strings.ErrorFailedToRemoveMonitoringMailbox(mailboxDatabase.Identity.ToString(), ex.Message));
				}
				catch (DataSourceOperationException ex2)
				{
					TaskLogger.Trace("The action of Removing monitoring mailbox objects of database '{0}' raises exception: {1}.", new object[]
					{
						mailboxDatabase.Identity.ToString(),
						ex2.Message
					});
					this.WriteWarning(Strings.ErrorFailedToRemoveMonitoringMailbox(mailboxDatabase.Identity.ToString(), ex2.Message));
				}
				catch (DataValidationException ex3)
				{
					TaskLogger.Trace("The action of Removing monitoring mailbox objects of database '{0}' raises exception: {1}.", new object[]
					{
						mailboxDatabase.Identity.ToString(),
						ex3.Message
					});
					this.WriteWarning(Strings.ErrorFailedToRemoveMonitoringMailbox(mailboxDatabase.Identity.ToString(), ex3.Message));
				}
			}
		}

		protected void RemoveSystemMailbox()
		{
			TDataObject dataObject = base.DataObject;
			ADObjectId adobjectId = ((ADObjectId)dataObject.Identity).DomainId;
			adobjectId = adobjectId.GetChildId("Microsoft Exchange System Objects");
			adobjectId = adobjectId.GetChildId("SystemMailbox" + dataObject.Guid.ToString("B"));
			try
			{
				string identity = adobjectId.ToDNString();
				GeneralMailboxIdParameter generalMailboxIdParameter = GeneralMailboxIdParameter.Parse(identity);
				base.WriteVerbose(TaskVerboseStringHelper.GetFindByIdParameterVerboseString(generalMailboxIdParameter, this.RecipientSessionForSystemMailbox, typeof(ADRecipient), null));
				IEnumerable<ADSystemMailbox> objects = generalMailboxIdParameter.GetObjects<ADSystemMailbox>(null, this.RecipientSessionForSystemMailbox);
				using (IEnumerator<ADSystemMailbox> enumerator = objects.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						ADSystemMailbox adsystemMailbox = enumerator.Current;
						base.WriteVerbose(TaskVerboseStringHelper.GetSourceVerboseString(this.RecipientSessionForSystemMailbox));
						base.WriteVerbose(Strings.VerboseDeleteSystemAttendantObject(adsystemMailbox.Identity.ToString()));
						this.RecipientSessionForSystemMailbox.Delete(adsystemMailbox);
					}
				}
			}
			catch (DataSourceTransientException ex)
			{
				TaskLogger.Trace("The action of Removing system mailbox object of database '{0}' raises exception: {1}.", new object[]
				{
					dataObject.Identity.ToString(),
					ex.Message
				});
				this.WriteWarning(Strings.ErrorFailedToRemoveSystemMailbox(dataObject.Identity.ToString(), ex.Message));
			}
			catch (DataSourceOperationException ex2)
			{
				TaskLogger.Trace("The action of Removing system mailbox object of database '{0}' raises exception: {1}.", new object[]
				{
					dataObject.Identity.ToString(),
					ex2.Message
				});
				this.WriteWarning(Strings.ErrorFailedToRemoveSystemMailbox(dataObject.Identity.ToString(), ex2.Message));
			}
			catch (DataValidationException ex3)
			{
				TaskLogger.Trace("The action of Removing system mailbox object of database '{0}' raises exception: {1}.", new object[]
				{
					dataObject.Identity.ToString(),
					ex3.Message
				});
				this.WriteWarning(Strings.ErrorFailedToRemoveSystemMailbox(dataObject.Identity.ToString(), ex3.Message));
			}
			finally
			{
				base.WriteVerbose(TaskVerboseStringHelper.GetSourceVerboseString(this.RecipientSessionForSystemMailbox));
			}
			TaskLogger.Trace("The action of Removing system mailbox object of database '{0}' succeeded.", new object[]
			{
				dataObject.Identity.ToString()
			});
		}

		protected override void InternalValidate()
		{
			if (this.Identity != null)
			{
				this.Identity.AllowInvalid = true;
			}
			base.InternalValidate();
			MapiTaskHelper.VerifyDatabaseIsWithinScope(base.SessionSettings, base.DataObject, new Task.ErrorLoggerDelegate(base.WriteError));
			TDataObject dataObject = base.DataObject;
			if (dataObject.Servers != null)
			{
				TDataObject dataObject2 = base.DataObject;
				if (dataObject2.Servers.Length > 0)
				{
					MapiTaskHelper.VerifyServerIsWithinScope(base.DataObject, new Task.ErrorLoggerDelegate(base.WriteError), (ITopologyConfigurationSession)this.ConfigurationSession);
				}
			}
			TDataObject dataObject3 = base.DataObject;
			if (dataObject3.IsExchange2009OrLater)
			{
				TDataObject dataObject4 = base.DataObject;
				this.copies = dataObject4.AllDatabaseCopies;
				if (this.copies != null && this.copies.Length > 1)
				{
					TDataObject dataObject5 = base.DataObject;
					Exception exception = new InvalidOperationException(Strings.ErrorMultipleDatabaseCopies(dataObject5.Identity.ToString()));
					ErrorCategory category = ErrorCategory.InvalidOperation;
					TDataObject dataObject6 = base.DataObject;
					base.WriteError(exception, category, dataObject6.Identity);
				}
			}
		}

		protected override void InternalProcessRecord()
		{
			if (this.server != null)
			{
				this.DismountDatabase();
			}
			this.RemoveSystemMailbox();
			this.RemoveMonitoringMailboxes();
			if (this.copies != null && this.copies.Length == 1)
			{
				base.DataSession.Delete(this.copies[0]);
			}
			base.InternalProcessRecord();
			TDataObject dataObject = base.DataObject;
			string name = dataObject.Name;
			TDataObject dataObject2 = base.DataObject;
			this.WriteWarning(Strings.NeedRemoveDatabaseFileManually(name, dataObject2.EdbFilePath.ToString()));
			DatabaseTasksHelper.RemoveDatabaseFromClusterDB((ITopologyConfigurationSession)base.DataSession, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskErrorLoggingDelegate(base.WriteError), base.DataObject);
		}

		private IRecipientSession recipientSessionForSystemMailbox;

		private DatabaseCopy[] copies;

		protected Server server;
	}
}
