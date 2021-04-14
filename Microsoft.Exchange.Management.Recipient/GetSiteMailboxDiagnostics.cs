using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.LinkedFolder;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "SiteMailboxDiagnostics", SupportsShouldProcess = true)]
	public sealed class GetSiteMailboxDiagnostics : TeamMailboxDiagnosticsBase
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter SendMeEmail
		{
			get
			{
				return (SwitchParameter)(base.Fields["SendMeEmail"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["SendMeEmail"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageGetTeamMailboxDiagnostics(base.Identity.ToString());
			}
		}

		protected override void InternalProcessRecord()
		{
			foreach (KeyValuePair<ADUser, ExchangePrincipal> entry in base.TMPrincipals)
			{
				TeamMailboxDiagnosticsInfo teamMailboxDiagnosticsInfo = null;
				try
				{
					teamMailboxDiagnosticsInfo = this.GetOneTeamMailboxDiagnosticsInfo(entry);
				}
				catch (StorageTransientException ex)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorGetSiteMailboxDiagnostics(entry.Value.MailboxInfo.DisplayName, ex.Message), ex), ExchangeErrorCategory.ServerTransient, null);
				}
				catch (StoragePermanentException ex2)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorGetSiteMailboxDiagnostics(entry.Value.MailboxInfo.DisplayName, ex2.Message), ex2), ExchangeErrorCategory.ServerOperation, null);
				}
				base.WriteObject(teamMailboxDiagnosticsInfo);
				if (base.Fields.IsModified("SendMeEmail"))
				{
					ADObjectId item;
					if (!base.TryGetExecutingUserId(out item))
					{
						base.WriteError(new RecipientTaskException(Strings.ErrorTeamMailboxCannotIdentifyTheUser), ExchangeErrorCategory.Client, null);
					}
					TeamMailbox teamMailbox = TeamMailbox.FromDataObject(entry.Key);
					TeamMailboxNotificationHelper teamMailboxNotificationHelper = new TeamMailboxNotificationHelper(teamMailbox, (IRecipientSession)base.DataSession);
					try
					{
						teamMailboxNotificationHelper.SendNotification(new List<ADObjectId>
						{
							item
						}, Strings.SiteMailboxDiagnosticsEmailSubject(teamMailbox.DisplayName), teamMailboxDiagnosticsInfo.ToString(), RemotingOptions.AllowCrossSite);
					}
					catch (StorageTransientException ex3)
					{
						base.WriteError(new RecipientTaskException(Strings.ErrorSendNotificationForSiteMailbox(teamMailbox.DisplayName, ex3.Message), ex3), ExchangeErrorCategory.ServerTransient, null);
					}
					catch (StoragePermanentException ex4)
					{
						base.WriteError(new RecipientTaskException(Strings.ErrorSendNotificationForSiteMailbox(teamMailbox.DisplayName, ex4.Message), ex4), ExchangeErrorCategory.ServerOperation, null);
					}
				}
			}
		}

		private TeamMailboxDiagnosticsInfo GetOneTeamMailboxDiagnosticsInfo(KeyValuePair<ADUser, ExchangePrincipal> entry)
		{
			TeamMailboxDiagnosticsInfo teamMailboxDiagnosticsInfo = new TeamMailboxDiagnosticsInfo(entry.Value.MailboxInfo.DisplayName);
			MultiValuedProperty<SyncInfo> multiValuedProperty = new MultiValuedProperty<SyncInfo>();
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			TeamMailboxDiagnosticsInfo result;
			using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(entry.Value, CultureInfo.InvariantCulture, "Client=TeamMailbox;Action=GetDiagnostics;Interactive=False"))
			{
				using (UserConfiguration mailboxConfiguration = UserConfigurationHelper.GetMailboxConfiguration(mailboxSession, "SiteSynchronizerConfigurations", UserConfigurationTypes.Dictionary, false))
				{
					if (mailboxConfiguration != null)
					{
						teamMailboxDiagnosticsInfo.HierarchySyncInfo = GetSiteMailboxDiagnostics.GetSyncInfoFromMetadata(entry.Value.MailboxInfo.DisplayName, (entry.Key.SharePointUrl != null) ? entry.Key.SharePointUrl.AbsoluteUri : string.Empty, mailboxConfiguration);
						if (!string.IsNullOrEmpty(teamMailboxDiagnosticsInfo.HierarchySyncInfo.LastSyncFailure))
						{
							flag = true;
						}
					}
				}
				using (Folder folder = Folder.Bind(mailboxSession, DefaultFolderType.Root))
				{
					using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.None, new ExistsFilter(FolderSchema.LinkedId), null, new PropertyDefinition[]
					{
						FolderSchema.Id,
						FolderSchema.DisplayName,
						FolderSchema.LinkedUrl
					}))
					{
						object[][] array = null;
						do
						{
							array = queryResult.GetRows(10000);
							for (int i = 0; i < array.Length; i++)
							{
								StoreObjectId objectId = ((VersionedId)array[i][0]).ObjectId;
								string displayName = array[i][1] as string;
								string url = array[i][2] as string;
								using (UserConfiguration folderConfiguration = UserConfigurationHelper.GetFolderConfiguration(mailboxSession, objectId, "DocumentLibSynchronizerConfigurations", UserConfigurationTypes.Dictionary, false, false))
								{
									if (folderConfiguration != null)
									{
										SyncInfo syncInfoFromMetadata = GetSiteMailboxDiagnostics.GetSyncInfoFromMetadata(displayName, url, folderConfiguration);
										multiValuedProperty.Add(syncInfoFromMetadata);
										if (!string.IsNullOrEmpty(syncInfoFromMetadata.LastSyncFailure))
										{
											flag = true;
										}
									}
								}
							}
						}
						while (array.Length != 0);
					}
				}
				teamMailboxDiagnosticsInfo.DocLibSyncInfos = multiValuedProperty;
				using (UserConfiguration mailboxConfiguration2 = UserConfigurationHelper.GetMailboxConfiguration(mailboxSession, "TeamMailboxDocumentLastSyncCycleLog", UserConfigurationTypes.Stream, false))
				{
					if (mailboxConfiguration2 != null)
					{
						using (Stream stream = mailboxConfiguration2.GetStream())
						{
							byte[] array2 = new byte[stream.Length];
							stream.Read(array2, 0, array2.Length);
							teamMailboxDiagnosticsInfo.LastDocumentSyncCycleLog = Encoding.ASCII.GetString(array2);
						}
					}
				}
				using (UserConfiguration mailboxConfiguration3 = UserConfigurationHelper.GetMailboxConfiguration(mailboxSession, "MembershipSynchronizerConfigurations", UserConfigurationTypes.Dictionary, false))
				{
					if (mailboxConfiguration3 != null)
					{
						teamMailboxDiagnosticsInfo.MembershipSyncInfo = GetSiteMailboxDiagnostics.GetSyncInfoFromMetadata(entry.Value.MailboxInfo.DisplayName, (entry.Key.SharePointUrl != null) ? entry.Key.SharePointUrl.AbsoluteUri : string.Empty, mailboxConfiguration3);
						if (!string.IsNullOrEmpty(teamMailboxDiagnosticsInfo.MembershipSyncInfo.LastSyncFailure))
						{
							flag2 = true;
						}
					}
				}
				using (UserConfiguration mailboxConfiguration4 = UserConfigurationHelper.GetMailboxConfiguration(mailboxSession, "TeamMailboxMembershipLastSyncCycleLog", UserConfigurationTypes.Stream, false))
				{
					if (mailboxConfiguration4 != null)
					{
						using (Stream stream2 = mailboxConfiguration4.GetStream())
						{
							byte[] array3 = new byte[stream2.Length];
							stream2.Read(array3, 0, array3.Length);
							teamMailboxDiagnosticsInfo.LastMembershipSyncCycleLog = Encoding.ASCII.GetString(array3);
						}
					}
				}
				using (UserConfiguration mailboxConfiguration5 = UserConfigurationHelper.GetMailboxConfiguration(mailboxSession, "MaintenanceSynchronizerConfigurations", UserConfigurationTypes.Dictionary, false))
				{
					if (mailboxConfiguration5 != null)
					{
						teamMailboxDiagnosticsInfo.MaintenanceSyncInfo = GetSiteMailboxDiagnostics.GetSyncInfoFromMetadata(entry.Value.MailboxInfo.DisplayName, (entry.Key.SharePointUrl != null) ? entry.Key.SharePointUrl.AbsoluteUri : string.Empty, mailboxConfiguration5);
						if (!string.IsNullOrEmpty(teamMailboxDiagnosticsInfo.MaintenanceSyncInfo.LastSyncFailure))
						{
							flag3 = true;
						}
					}
				}
				using (UserConfiguration mailboxConfiguration6 = UserConfigurationHelper.GetMailboxConfiguration(mailboxSession, "TeamMailboxMaintenanceLastSyncCycleLog", UserConfigurationTypes.Stream, false))
				{
					if (mailboxConfiguration6 != null)
					{
						using (Stream stream3 = mailboxConfiguration6.GetStream())
						{
							byte[] array4 = new byte[stream3.Length];
							stream3.Read(array4, 0, array4.Length);
							teamMailboxDiagnosticsInfo.LastMaintenanceSyncCycleLog = Encoding.ASCII.GetString(array4);
						}
					}
				}
				if (!flag && !flag2 && !flag3)
				{
					teamMailboxDiagnosticsInfo.Status = TeamMailboxSyncStatus.Succeeded;
				}
				else if (flag && !flag2 && !flag3)
				{
					teamMailboxDiagnosticsInfo.Status = TeamMailboxSyncStatus.DocumentSyncFailureOnly;
				}
				else if (!flag && flag2 && !flag3)
				{
					teamMailboxDiagnosticsInfo.Status = TeamMailboxSyncStatus.MembershipSyncFailureOnly;
				}
				else if (!flag && !flag2 && flag3)
				{
					teamMailboxDiagnosticsInfo.Status = TeamMailboxSyncStatus.MaintenanceSyncFailureOnly;
				}
				else if (flag && flag2 && !flag3)
				{
					teamMailboxDiagnosticsInfo.Status = TeamMailboxSyncStatus.DocumentAndMembershipSyncFailure;
				}
				else if (!flag && flag2 && flag3)
				{
					teamMailboxDiagnosticsInfo.Status = TeamMailboxSyncStatus.MembershipAndMaintenanceSyncFailure;
				}
				else if (flag && !flag2 && flag3)
				{
					teamMailboxDiagnosticsInfo.Status = TeamMailboxSyncStatus.DocumentAndMaintenanceSyncFailure;
				}
				else
				{
					teamMailboxDiagnosticsInfo.Status = TeamMailboxSyncStatus.Failed;
				}
				result = teamMailboxDiagnosticsInfo;
			}
			return result;
		}

		private static SyncInfo GetSyncInfoFromMetadata(string displayName, string url, UserConfiguration metadata)
		{
			return new SyncInfo(displayName, url)
			{
				FirstAttemptedSyncTime = (GetSiteMailboxDiagnostics.GetMetadataValue(metadata, "FirstAttemptedSyncTime") as ExDateTime?),
				LastAttemptedSyncTime = (GetSiteMailboxDiagnostics.GetMetadataValue(metadata, "LastAttemptedSyncTime") as ExDateTime?),
				LastFailedSyncEmailTime = (GetSiteMailboxDiagnostics.GetMetadataValue(metadata, "LastFailedSyncEmailTime") as ExDateTime?),
				LastFailedSyncTime = (GetSiteMailboxDiagnostics.GetMetadataValue(metadata, "LastFailedSyncTime") as ExDateTime?),
				LastSuccessfulSyncTime = (GetSiteMailboxDiagnostics.GetMetadataValue(metadata, "LastSuccessfulSyncTime") as ExDateTime?),
				LastSyncFailure = (GetSiteMailboxDiagnostics.GetMetadataValue(metadata, "LastSyncFailure") as string)
			};
		}

		private static object GetMetadataValue(UserConfiguration metadata, string name)
		{
			IDictionary dictionary = metadata.GetDictionary();
			if (dictionary.Contains(name))
			{
				return dictionary[name];
			}
			return null;
		}
	}
}
