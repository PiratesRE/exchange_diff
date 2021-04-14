using System;
using System.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;

namespace Microsoft.Exchange.InfoWorker.Common.ELC
{
	internal static class ProvisionedFolderCreator
	{
		internal static Folder CreateELCRootFolder(MailboxSession mailboxSession, string elcRootUrl)
		{
			string arg = mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString();
			ProvisionedFolderCreator.Tracer.TraceDebug<string>(0L, "'{0}': Attempting to create the ELC Root Folder.", arg);
			if (mailboxSession.CreateDefaultFolder(DefaultFolderType.ElcRoot) == null)
			{
				throw new ELCRootFailureException(Strings.descFailedToCreateELCRoot(mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString()), null);
			}
			Folder folder = Folder.Bind(mailboxSession, DefaultFolderType.ElcRoot, new PropertyDefinition[]
			{
				FolderSchema.FolderHomePageUrl
			});
			if (!string.IsNullOrEmpty(elcRootUrl))
			{
				ProvisionedFolderCreator.Tracer.TraceDebug<string, string>(0L, "'{0}': Attempting to set the ELC Root Folder URL to '{1}'.", arg, elcRootUrl);
				ElcMailboxHelper.SetPropAndTrace(folder, FolderSchema.FolderHomePageUrl, elcRootUrl);
				folder.ClassName = Globals.ElcRootFolderClass;
				FolderSaveResult folderSaveResult = folder.Save();
				if (folderSaveResult.OperationResult != OperationResult.Succeeded)
				{
					ProvisionedFolderCreator.Tracer.TraceError<SmtpAddress>(0L, "{0}: Unable to set the ELC Root Folder home page. Will attempt this again on the next run.", mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress);
				}
				folder.Load();
			}
			return folder;
		}

		internal static string NormalizeFolderName(string folderName)
		{
			return folderName.Replace("/", "\\/");
		}

		internal static string CreateOneELCFolderInMailbox(AdFolderData adFolderInfo, MailboxSession itemStore, Folder elcRootFolder)
		{
			Folder folder = null;
			ELCFolder folder2 = adFolderInfo.Folder;
			string text = itemStore.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString();
			string text2 = (folder2.FolderType == ElcFolderType.ManagedCustomFolder) ? folder2.FolderName : folder2.FolderType.ToString();
			string result;
			try
			{
				if (folder2.FolderType != ElcFolderType.ManagedCustomFolder)
				{
					ProvisionedFolderCreator.Tracer.TraceDebug<string, string>(0L, "Need to provision a default folder '{0}' in mailbox '{1}'.", text2, text);
					try
					{
						folder = ElcMailboxHelper.GetDefaultFolder(itemStore, folder2);
					}
					catch (ObjectNotFoundException innerException)
					{
						ProvisionedFolderCreator.Tracer.TraceDebug<string, string>(0L, "Default folder '{0}' does not exist in mailbox '{1}'.", text2, text);
						throw new ELCDefaultFolderNotFoundException(text2, text, innerException);
					}
					ProvisionedFolderCreator.Tracer.TraceDebug<string, string>(0L, "Default folder '{0}' exists in mailbox '{1}' and will be provisioned.", text2, text);
					result = '/' + ProvisionedFolderCreator.NormalizeFolderName(text2);
				}
				else
				{
					ProvisionedFolderCreator.Tracer.TraceDebug<string, string>(0L, "Creating organizational folder '{0}' in mailbox '{1}'.", text2, text);
					folder = Folder.Create(itemStore, elcRootFolder.Id, StoreObjectType.Folder, folder2.FolderName, CreateMode.OpenIfExists);
					ProvisionedFolderCreator.SaveELCFolder(folder, true);
					folder.Load();
					result = string.Concat(new object[]
					{
						'/',
						ProvisionedFolderCreator.NormalizeFolderName(elcRootFolder.DisplayName),
						'/',
						ProvisionedFolderCreator.NormalizeFolderName(text2)
					});
				}
				ELCFolderFlags elcfolderFlags = ELCFolderFlags.None;
				ulong num = folder2.StorageQuota.IsUnlimited ? 0UL : folder2.StorageQuota.Value.ToKB();
				if (folder2.FolderType == ElcFolderType.ManagedCustomFolder)
				{
					elcfolderFlags = ELCFolderFlags.Provisioned;
					if (adFolderInfo.LinkedToTemplate)
					{
						elcfolderFlags |= ELCFolderFlags.Protected;
					}
					if (folder.ItemCount == 0 && !folder.HasSubfolders)
					{
						elcfolderFlags |= ELCFolderFlags.TrackFolderSize;
					}
					if (num > 0UL)
					{
						elcfolderFlags |= ELCFolderFlags.Quota;
					}
				}
				if (folder2.MustDisplayCommentEnabled)
				{
					elcfolderFlags |= ELCFolderFlags.MustDisplayComment;
				}
				ElcMailboxHelper.SetPropAndTrace(folder, FolderSchema.AdminFolderFlags, elcfolderFlags);
				ElcMailboxHelper.SetPropAndTrace(folder, FolderSchema.ELCPolicyIds, folder2.Guid.ToString());
				string localizedFolderComment = folder2.GetLocalizedFolderComment(itemStore.MailboxOwner.PreferredCultures);
				if (!string.IsNullOrEmpty(localizedFolderComment))
				{
					ElcMailboxHelper.SetPropAndTrace(folder, FolderSchema.ELCFolderComment, localizedFolderComment);
				}
				else
				{
					ElcMailboxHelper.SetPropAndTrace(folder, FolderSchema.ELCFolderComment, Globals.BlankComment);
				}
				if (folder2.FolderType == ElcFolderType.ManagedCustomFolder && num > 0UL)
				{
					ElcMailboxHelper.SetPropAndTrace(folder, FolderSchema.FolderQuota, (int)num);
				}
				ProvisionedFolderCreator.SaveELCFolder(folder, true);
				if (folder2.FolderType == ElcFolderType.All)
				{
					ProvisionedFolderCreator.CreateAllOthersConfigMsg(adFolderInfo.Folder, itemStore);
				}
				ProvisionedFolderCreator.Tracer.TraceDebug<string, string>(0L, "Successfully created/provisioned folder '{0}' in mailbox '{1}'.", text2, text);
			}
			finally
			{
				if (folder != null)
				{
					folder.Dispose();
					folder = null;
				}
			}
			return result;
		}

		internal static void SaveELCFolder(Folder folder, bool create)
		{
			string displayName = folder.DisplayName;
			bool flag = false;
			LocalizedException innerException = null;
			try
			{
				FolderSaveResult folderSaveResult = folder.Save();
				if (folderSaveResult.OperationResult == OperationResult.Succeeded)
				{
					return;
				}
				if (folderSaveResult.OperationResult == OperationResult.PartiallySucceeded)
				{
					ProvisionedFolderCreator.Tracer.TraceError<string>(0L, "Failed to set ELC props on folder '{0}'.", displayName);
					foreach (PropertyError propertyError in folderSaveResult.PropertyErrors)
					{
						ProvisionedFolderCreator.Tracer.TraceError<PropertyDefinition, PropertyErrorCode, string>(0L, "Failed to set '{0}', error '{1}', on folder '{2}'.", propertyError.PropertyDefinition, propertyError.PropertyErrorCode, displayName);
						if (propertyError.PropertyDefinition == FolderSchema.ELCPolicyIds || propertyError.PropertyDefinition == FolderSchema.AdminFolderFlags)
						{
							flag = true;
						}
					}
				}
				else if (folderSaveResult.OperationResult == OperationResult.Failed)
				{
					ProvisionedFolderCreator.Tracer.TraceError<string>(0L, "Failed to create or update the ELC folder '{0}'.", displayName);
					flag = true;
				}
			}
			catch (ObjectNotFoundException ex)
			{
				ProvisionedFolderCreator.Tracer.TraceError<string>(0L, "Failed to set ELC props on folder '{0}' - the folder was not found in the mailbox.", displayName);
				flag = true;
				innerException = ex;
			}
			catch (SaveConflictException ex2)
			{
				ProvisionedFolderCreator.Tracer.TraceError<string>(0L, "Failed to set ELC props on folder '{0}' - conflict during save.", displayName);
				flag = true;
				innerException = ex2;
			}
			if (flag)
			{
				if (create)
				{
					ProvisionedFolderCreator.Tracer.TraceError<string>(0L, "Unable to save the ELC folder '{0}' in the mailbox.", displayName);
					throw new ELCOrgFolderCreationException(((MailboxSession)folder.Session).MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString(), displayName, innerException);
				}
				ProvisionedFolderCreator.Tracer.TraceError<string>(0L, "Unable to sync the ELC folder '{0}' in the mailbox.", displayName);
				throw new ELCFolderSyncException(((MailboxSession)folder.Session).MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString(), displayName, innerException);
			}
		}

		internal static void CreateAllOthersConfigMsg(ELCFolder adFolder, MailboxSession session)
		{
			StoreId defaultFolderId = session.GetDefaultFolderId(DefaultFolderType.Inbox);
			string localizedFolderComment = adFolder.GetLocalizedFolderComment(session.MailboxOwner.PreferredCultures);
			try
			{
				using (UserConfiguration userConfiguration = session.UserConfigurationManager.CreateFolderConfiguration("ELC", UserConfigurationTypes.Stream | UserConfigurationTypes.XML | UserConfigurationTypes.Dictionary, defaultFolderId))
				{
					IDictionary dictionary = userConfiguration.GetDictionary();
					dictionary["elcFlags"] = (adFolder.MustDisplayCommentEnabled ? 4 : 0);
					dictionary["elcComment"] = localizedFolderComment;
					dictionary["policyId"] = adFolder.Guid.ToString();
					userConfiguration.Save();
				}
			}
			catch (ObjectExistedException)
			{
				using (UserConfiguration folderConfiguration = session.UserConfigurationManager.GetFolderConfiguration("ELC", UserConfigurationTypes.Stream | UserConfigurationTypes.XML | UserConfigurationTypes.Dictionary, defaultFolderId))
				{
					IDictionary dictionary2 = folderConfiguration.GetDictionary();
					dictionary2["elcFlags"] = (adFolder.MustDisplayCommentEnabled ? 4 : 0);
					dictionary2["elcComment"] = localizedFolderComment;
					dictionary2["policyId"] = adFolder.Guid.ToString();
					folderConfiguration.Save();
				}
			}
			catch (SaveConflictException innerException)
			{
				throw new ELCOrgFolderCreationException(session.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString(), adFolder.FolderName, innerException);
			}
			catch (QuotaExceededException innerException2)
			{
				throw new ELCOrgFolderCreationException(session.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString(), adFolder.FolderName, innerException2);
			}
		}

		private static readonly Trace Tracer = ExTraceGlobals.ELCTracer;
	}
}
