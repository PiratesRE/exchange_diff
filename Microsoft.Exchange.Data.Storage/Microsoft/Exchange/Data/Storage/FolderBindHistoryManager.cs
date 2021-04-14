using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Principal;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Auditing;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class FolderBindHistoryManager
	{
		private static TimeSpan HistoryRefreshTime
		{
			get
			{
				if (!AuditFeatureManager.IsFolderBindExtendedThrottlingEnabled())
				{
					return TimeSpan.FromHours(3.0);
				}
				return TimeSpan.FromHours(24.0);
			}
		}

		public FolderBindHistoryManager(CoreFolder folder)
		{
			Util.ThrowOnNullArgument(folder, "folder");
			this.currentFolder = folder;
			this.ShouldSkipAudit = false;
			this.bindingHistory.Add(this.GenerateBindingEntry());
			string[] array = this.ReadRecentHistory();
			if (array == null)
			{
				return;
			}
			string bindingEntryIdentity = this.GetBindingEntryIdentity();
			long num = (long)(DateTime.UtcNow - FolderBindHistoryManager.Epoch).TotalMinutes;
			foreach (string text in array)
			{
				if (text != null)
				{
					string[] array3 = text.Split(new char[0]);
					long num2;
					if (array3.Length == 2 && long.TryParse(array3[0], out num2))
					{
						if (bindingEntryIdentity.Equals(array3[1], StringComparison.OrdinalIgnoreCase))
						{
							if ((double)(num - num2) < FolderBindHistoryManager.HistoryRefreshTime.TotalMinutes)
							{
								this.ShouldSkipAudit = true;
							}
						}
						else if ((double)(num - num2) < FolderBindHistoryManager.HistoryRefreshTime.TotalMinutes)
						{
							this.bindingHistory.Add(text);
						}
					}
				}
			}
		}

		public void UpdateHistory(CallbackContext callbackContext)
		{
			if (this.ShouldSkipAudit)
			{
				return;
			}
			MailboxSession mailboxSession = (MailboxSession)this.currentFolder.Session;
			CoreFolder coreFolder = null;
			StoreObjectId objectId = this.currentFolder.Id.ObjectId;
			if (mailboxSession.LogonType == LogonType.Delegated || mailboxSession.LogonType == LogonType.DelegatedAdmin)
			{
				Exception ex = null;
				try
				{
					coreFolder = CoreFolder.Bind(callbackContext.SessionWithBestAccess, objectId);
					this.currentFolder = coreFolder;
				}
				catch (StoragePermanentException ex2)
				{
					ex = ex2;
				}
				catch (StorageTransientException ex3)
				{
					ex = ex3;
				}
				if (ex != null)
				{
					ExTraceGlobals.SessionTracer.TraceWarning<StoreObjectId, Exception>((long)this.currentFolder.Session.GetHashCode(), "Failed to rebind folder {0} with Admin logon. The cached RecentBindingHistory data will not be updated. Error: {1}", objectId, ex);
					ProcessInfoEventLogger.Log(StorageEventLogConstants.Tuple_ErrorBindingFolderForFolderBindHistory, objectId.ToString(), new object[]
					{
						objectId,
						mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString(),
						mailboxSession.MailboxGuid,
						mailboxSession.LogonType,
						IdentityHelper.SidFromLogonIdentity(mailboxSession.Identity),
						COWTriggerAction.FolderBind,
						ex
					});
					this.currentFolder = null;
				}
			}
			if (this.currentFolder != null)
			{
				try
				{
					this.currentFolder.PropertyBag.SetProperty(FolderSchema.RecentBindingHistory, this.bindingHistory.ToArray());
					FolderSaveResult folderSaveResult = this.currentFolder.Save(SaveMode.NoConflictResolutionForceSave);
					if (coreFolder == null)
					{
						this.currentFolder.PropertyBag.Load(null);
					}
					if (folderSaveResult.OperationResult != OperationResult.Succeeded)
					{
						ExTraceGlobals.SessionTracer.TraceWarning<StoreObjectId, LocalizedException>((long)this.currentFolder.Session.GetHashCode(), "Failed to save RecentBindingHistory on folder {0}. Error: {1}.", objectId, folderSaveResult.Exception);
						ProcessInfoEventLogger.Log(StorageEventLogConstants.Tuple_ErrorSavingFolderBindHistory, objectId.ToString(), new object[]
						{
							objectId,
							mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString(),
							mailboxSession.MailboxGuid,
							mailboxSession.LogonType,
							IdentityHelper.SidFromLogonIdentity(mailboxSession.Identity),
							COWTriggerAction.FolderBind,
							folderSaveResult.Exception
						});
					}
				}
				finally
				{
					if (coreFolder != null)
					{
						coreFolder.Dispose();
					}
				}
			}
		}

		public bool ShouldSkipAudit { get; private set; }

		private string[] ReadRecentHistory()
		{
			string[] result = null;
			try
			{
				result = (this.currentFolder.PropertyBag.TryGetProperty(CoreFolderSchema.RecentBindingHistory) as string[]);
			}
			catch (NotInBagPropertyErrorException)
			{
			}
			return result;
		}

		private string GetBindingEntryIdentity()
		{
			SecurityIdentifier effectiveLogonSid = IdentityHelper.GetEffectiveLogonSid((MailboxSession)this.currentFolder.Session);
			return effectiveLogonSid.Value;
		}

		private string GenerateBindingEntry()
		{
			string bindingEntryIdentity = this.GetBindingEntryIdentity();
			return string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[]
			{
				(long)(DateTime.UtcNow - FolderBindHistoryManager.Epoch).TotalMinutes,
				bindingEntryIdentity
			});
		}

		private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		private CoreFolder currentFolder;

		private List<string> bindingHistory = new List<string>();
	}
}
