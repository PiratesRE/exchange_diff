using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AsyncOperationNotificationDataProvider : TenantStoreDataProvider
	{
		public AsyncOperationNotificationDataProvider(OrganizationId organizationId) : base(organizationId)
		{
		}

		public static void CreateNotification(OrganizationId organizationId, string id, AsyncOperationType type, LocalizedString displayName, ADRecipientOrAddress owner, KeyValuePair<string, LocalizedString>[] extendedAttributes, bool throwOnError)
		{
			AsyncOperationNotificationDataProvider.CreateNotification(organizationId, id, type, AsyncOperationStatus.Queued, displayName, owner, extendedAttributes, throwOnError);
		}

		public static void CreateNotification(OrganizationId organizationId, string id, AsyncOperationType type, AsyncOperationStatus status, LocalizedString displayName, ADRecipientOrAddress owner, KeyValuePair<string, LocalizedString>[] extendedAttributes, bool throwOnError)
		{
			if (AsyncOperationNotificationDataProvider.IsAsyncNotificationDisabled())
			{
				ExTraceGlobals.StorageTracer.TraceWarning(0L, "DisableAsyncNotification is set in registry, no notification will be created.");
				return;
			}
			if (string.IsNullOrEmpty(id))
			{
				throw new ArgumentNullException("id", "id is mandatory.");
			}
			try
			{
				AsyncOperationNotificationDataProvider asyncOperationNotificationDataProvider = new AsyncOperationNotificationDataProvider(organizationId);
				if (asyncOperationNotificationDataProvider.FindItemByAlternativeId(id) != null)
				{
					throw new StoragePermanentException(ServerStrings.ErrorNotificationAlreadyExists);
				}
				asyncOperationNotificationDataProvider.Save(new AsyncOperationNotification
				{
					AlternativeId = id,
					ExtendedAttributes = extendedAttributes,
					StartedByValue = owner,
					DisplayName = displayName,
					Type = type,
					Status = status
				});
			}
			catch (Exception ex)
			{
				string printableId = AsyncOperationNotificationDataProvider.GetPrintableId(organizationId, id);
				ExTraceGlobals.StorageTracer.TraceError<string, string>(0L, "AsyncOperationNotificationDataProvider::CreateNotification failed: {0}, message: {1}", printableId, ex.Message);
				StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_ErrorCreateNotification, printableId, new object[]
				{
					printableId,
					ex
				});
				if (throwOnError)
				{
					throw;
				}
			}
		}

		public static void UpdateNotification(OrganizationId organizationId, string id, AsyncOperationStatus? status, int? percentComplete, LocalizedString? message, bool throwOnError = false, IEnumerable<KeyValuePair<string, LocalizedString>> extendedAttributes = null)
		{
			if (AsyncOperationNotificationDataProvider.IsAsyncNotificationDisabled())
			{
				ExTraceGlobals.StorageTracer.TraceWarning(0L, "DisableAsyncNotification is set in registry, no notification will be updated.");
				return;
			}
			if (status == null && percentComplete == null && message == null)
			{
				if (extendedAttributes == null)
				{
					return;
				}
			}
			try
			{
				AsyncOperationNotificationDataProvider asyncOperationNotificationDataProvider = new AsyncOperationNotificationDataProvider(organizationId);
				asyncOperationNotificationDataProvider.CanRetry = false;
				if (asyncOperationNotificationDataProvider.IsUpdateRequired(id, status, percentComplete, message) || extendedAttributes != null)
				{
					bool flag = false;
					if (!asyncOperationNotificationDataProvider.Cache.BadItems.TryGetValue(id, out flag))
					{
						AsyncOperationNotification asyncOperationNotification = asyncOperationNotificationDataProvider.FindByAlternativeId<AsyncOperationNotification>(id);
						if (asyncOperationNotification != null)
						{
							if (status != null)
							{
								asyncOperationNotification.Status = status.Value;
								if (status == AsyncOperationStatus.Completed)
								{
									percentComplete = new int?(100);
								}
							}
							else if (asyncOperationNotification.Status == AsyncOperationStatus.Queued)
							{
								asyncOperationNotification.Status = AsyncOperationStatus.InProgress;
							}
							if (percentComplete != null)
							{
								asyncOperationNotification.PercentComplete = new int?(percentComplete.Value);
							}
							if (message != null)
							{
								asyncOperationNotification.Message = message.Value;
							}
							if (extendedAttributes != null)
							{
								Dictionary<string, LocalizedString> dictionary = asyncOperationNotification.ExtendedAttributes.ToDictionary((KeyValuePair<string, LocalizedString> x) => x.Key, (KeyValuePair<string, LocalizedString> x) => x.Value);
								foreach (KeyValuePair<string, LocalizedString> keyValuePair in extendedAttributes)
								{
									dictionary[keyValuePair.Key] = keyValuePair.Value;
								}
								asyncOperationNotification.ExtendedAttributes = dictionary.ToArray<KeyValuePair<string, LocalizedString>>();
							}
							asyncOperationNotificationDataProvider.Save(asyncOperationNotification);
						}
						else
						{
							ExTraceGlobals.StorageTracer.TraceError<string>(0L, "AsyncOperationNotificationDataProvider::UpdateNotification failed: Notification object '{0}' can't be found.", AsyncOperationNotificationDataProvider.GetPrintableId(organizationId, id));
							asyncOperationNotificationDataProvider.Cache.BadItems.Add(id, true);
						}
					}
				}
			}
			catch (Exception ex)
			{
				string printableId = AsyncOperationNotificationDataProvider.GetPrintableId(organizationId, id);
				ExTraceGlobals.StorageTracer.TraceError<string, string>(0L, "AsyncOperationNotificationDataProvider::UpdateNotification failed: Notification object: {0}, message: {1}", printableId, ex.Message);
				StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_ErrorUpdateNotification, printableId, new object[]
				{
					printableId,
					ex
				});
				if (throwOnError)
				{
					throw;
				}
			}
		}

		public static void CompleteNotification(OrganizationId organizationId, string id, LocalizedString? message, IEnumerable<LocalizedString> report, bool succeeded, int? percentComplete = null, bool throwOnError = false)
		{
			if (AsyncOperationNotificationDataProvider.IsAsyncNotificationDisabled())
			{
				ExTraceGlobals.StorageTracer.TraceWarning(0L, "DisableAsyncNotification is set in registry, no notification will be completed.");
				return;
			}
			if (succeeded)
			{
				percentComplete = new int?(100);
			}
			AsyncOperationNotificationDataProvider.UpdateNotification(organizationId, id, new AsyncOperationStatus?(succeeded ? AsyncOperationStatus.Completed : AsyncOperationStatus.Failed), percentComplete, message, throwOnError, null);
			AsyncOperationNotificationDataProvider.SendNotificationEmail(organizationId, id, false, succeeded ? null : report, throwOnError);
		}

		public static void RemoveNotification(OrganizationId organizationId, string id, bool throwOnError)
		{
			if (AsyncOperationNotificationDataProvider.IsAsyncNotificationDisabled())
			{
				ExTraceGlobals.StorageTracer.TraceWarning(0L, "DisableAsyncNotification is set in registry, no notification will be removed.");
				return;
			}
			try
			{
				AsyncOperationNotificationDataProvider asyncOperationNotificationDataProvider = new AsyncOperationNotificationDataProvider(organizationId);
				AsyncOperationNotification asyncOperationNotification = asyncOperationNotificationDataProvider.FindByAlternativeId<AsyncOperationNotification>(id);
				if (asyncOperationNotification != null)
				{
					asyncOperationNotificationDataProvider.Delete(asyncOperationNotification);
				}
				else
				{
					ExTraceGlobals.StorageTracer.TraceError<OrganizationId, string>(0L, "AsyncOperationNotificationDataProvider::RemoveNotification failed: Notification object {0}\\{1} can't be found.", organizationId, id);
				}
			}
			catch (Exception ex)
			{
				string printableId = AsyncOperationNotificationDataProvider.GetPrintableId(organizationId, id);
				ExTraceGlobals.StorageTracer.TraceError<string, string>(0L, "AsyncOperationNotificationDataProvider::RemoveNotification failed: {0}, message: {1}", printableId, ex.Message);
				StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_ErrorRemoveNotification, printableId, new object[]
				{
					printableId,
					ex
				});
				if (throwOnError)
				{
					throw;
				}
			}
		}

		public static void SendNotificationEmail(OrganizationId organizationId, string id, bool forceSendCreatedMail, IEnumerable<LocalizedString> report, bool throwOnError)
		{
			if (AsyncOperationNotificationDataProvider.IsAsyncNotificationDisabled())
			{
				ExTraceGlobals.StorageTracer.TraceWarning(0L, "DisableAsyncNotification is set in registry, no notification email will be sent.");
				return;
			}
			AsyncOperationNotificationDataProvider asyncOperationNotificationDataProvider = new AsyncOperationNotificationDataProvider(organizationId);
			bool flag = false;
			if (asyncOperationNotificationDataProvider.Cache.BadItems.TryGetValue(id, out flag))
			{
				return;
			}
			AsyncOperationNotification asyncOperationNotification = asyncOperationNotificationDataProvider.FindByAlternativeId<AsyncOperationNotification>(id);
			if (asyncOperationNotification != null)
			{
				asyncOperationNotificationDataProvider.SendNotificationEmail(asyncOperationNotification, forceSendCreatedMail, report, throwOnError);
			}
		}

		private static string GetPrintableId(OrganizationId organizationId, string id)
		{
			return TenantStoreDataProvider.GetOrganizationKey(organizationId) + "\\" + id;
		}

		private static bool IsAsyncNotificationDisabled()
		{
			string name = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\";
			bool result = false;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name))
			{
				if (registryKey != null)
				{
					object value = registryKey.GetValue("DisableAsyncNotification");
					if (value != null && value.ToString() == "1")
					{
						result = true;
					}
				}
			}
			return result;
		}

		public void SendNotificationEmail(AsyncOperationNotification notification, bool forceSendCreatedMail, IEnumerable<LocalizedString> report, bool throwOnError)
		{
			if (AsyncOperationNotificationDataProvider.IsAsyncNotificationDisabled())
			{
				ExTraceGlobals.StorageTracer.TraceWarning(0L, "DisableAsyncNotification is set in registry, no notification email will be sent.");
				return;
			}
			if (notification == null)
			{
				throw new ArgumentNullException("notification");
			}
			try
			{
				AsyncOperationNotificationEmail asyncOperationNotificationEmail = new AsyncOperationNotificationEmail(this, notification, forceSendCreatedMail);
				string alternativeId;
				if (AsyncOperationNotificationDataProvider.SettingsObjectIdentityMap.TryGetValue(notification.Type, out alternativeId))
				{
					AsyncOperationNotification asyncOperationNotification = this.FindByAlternativeId<AsyncOperationNotification>(alternativeId);
					if (asyncOperationNotification != null)
					{
						asyncOperationNotificationEmail.AppendRecipients(asyncOperationNotification.NotificationEmails);
					}
				}
				if (asyncOperationNotificationEmail.ToRecipients.Count<EmailAddress>() > 0)
				{
					if (report != null)
					{
						StringBuilder stringBuilder = new StringBuilder();
						foreach (LocalizedString value in report)
						{
							stringBuilder.AppendLine(value);
						}
						string s = stringBuilder.ToString();
						byte[] bytes = Encoding.Default.GetBytes(s);
						asyncOperationNotificationEmail.Attachments.AddFileAttachment("Report.txt", bytes);
					}
					int num = 0;
					bool flag;
					do
					{
						flag = asyncOperationNotificationEmail.Send();
						num++;
					}
					while (!flag && base.CanRetry && num < 3);
					if (forceSendCreatedMail && (notification.Status == AsyncOperationStatus.Completed || notification.Status == AsyncOperationStatus.Failed))
					{
						this.SendNotificationEmail(notification, false, report, throwOnError);
					}
				}
			}
			catch (Exception ex)
			{
				string printableId = AsyncOperationNotificationDataProvider.GetPrintableId(base.Mailbox.MailboxInfo.OrganizationId, notification.AlternativeId);
				ExTraceGlobals.StorageTracer.TraceError<string, string>(0L, "AsyncOperationNotificationDataProvider::SendNotificationEmail failed: {0}, message: {1}", printableId, ex.Message);
				StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_ErrorSendNotificationEmail, printableId, new object[]
				{
					printableId,
					ex
				});
				if (throwOnError)
				{
					throw;
				}
			}
		}

		public IEnumerable<AsyncOperationNotification> GetNotificationDetails(AsyncOperationType? type, ExDateTime? startDate, int? resultSize, params ProviderPropertyDefinition[] properties)
		{
			int pageSize = (resultSize != null && resultSize < 1000) ? (resultSize.Value + 1) : 1000;
			DateTime startDateUtc = this.GetStartDate(startDate);
			SortBy[] soryBy = new SortBy[]
			{
				new SortBy(AsyncOperationNotificationSchema.LastModified, SortOrder.Descending)
			};
			SearchFilter failedRequestFilter = this.BuildSearchFilter(type, startDateUtc, true, false);
			IEnumerable<AsyncOperationNotification> failedNotifications = this.InternalFindPaged<AsyncOperationNotification>(failedRequestFilter, null, false, soryBy, pageSize, properties);
			int count = 0;
			foreach (AsyncOperationNotification item in failedNotifications)
			{
				count++;
				yield return item;
			}
			if (resultSize == null || resultSize >= count)
			{
				if (resultSize != null)
				{
					int num = pageSize;
					if (num > resultSize - count)
					{
						pageSize = resultSize.Value - count + 1;
					}
				}
				SearchFilter nonFailedRequestFilter = this.BuildSearchFilter(type, startDateUtc, false, false);
				IEnumerable<AsyncOperationNotification> otherNotifications = this.InternalFindPaged<AsyncOperationNotification>(nonFailedRequestFilter, null, false, soryBy, pageSize, properties);
				foreach (AsyncOperationNotification item2 in otherNotifications)
				{
					yield return item2;
				}
			}
			yield break;
		}

		public override T FindByAlternativeId<T>(string alternativeId)
		{
			T t = base.FindByAlternativeId<T>(alternativeId);
			if (t == null)
			{
				KeyValuePair<AsyncOperationType, string> keyValuePair = AsyncOperationNotificationDataProvider.SettingsObjectIdentityMap.FirstOrDefault((KeyValuePair<AsyncOperationType, string> x) => x.Value == alternativeId);
				if (keyValuePair.Value == alternativeId)
				{
					AsyncOperationNotificationDataProvider.CreateNotification(base.Mailbox.MailboxInfo.OrganizationId, alternativeId, keyValuePair.Key, LocalizedString.Empty, null, null, false);
				}
				t = base.FindByAlternativeId<T>(alternativeId);
			}
			return t;
		}

		protected override EwsStoreObject FilterObject(EwsStoreObject ewsStoreObject)
		{
			ewsStoreObject = base.FilterObject(ewsStoreObject);
			if (ewsStoreObject != null && ((AsyncOperationNotification)ewsStoreObject).Type == AsyncOperationType.Unknown)
			{
				return null;
			}
			return ewsStoreObject;
		}

		private SearchFilter BuildSearchFilter(AsyncOperationType? type, DateTime startDate, bool failedRequestOnly, bool forAll)
		{
			SearchFilter searchFilter = null;
			if (failedRequestOnly)
			{
				searchFilter = new SearchFilter.SearchFilterCollection(0, new SearchFilter[]
				{
					new SearchFilter.IsEqualTo(ExtendedEwsStoreObjectSchema.Status, 4),
					new SearchFilter.IsGreaterThanOrEqualTo(ItemSchema.LastModifiedTime, startDate)
				});
			}
			else
			{
				SearchFilter.SearchFilterCollection searchFilterCollection = new SearchFilter.SearchFilterCollection(0, new SearchFilter[]
				{
					new SearchFilter.IsNotEqualTo(ExtendedEwsStoreObjectSchema.Status, 3),
					new SearchFilter.IsNotEqualTo(ExtendedEwsStoreObjectSchema.Status, 4)
				});
				SearchFilter searchFilter2 = forAll ? new SearchFilter.IsGreaterThanOrEqualTo(ItemSchema.LastModifiedTime, startDate) : new SearchFilter.SearchFilterCollection(0, new SearchFilter[]
				{
					new SearchFilter.IsEqualTo(ExtendedEwsStoreObjectSchema.Status, 3),
					new SearchFilter.IsGreaterThanOrEqualTo(ItemSchema.LastModifiedTime, startDate)
				});
				searchFilter = new SearchFilter.SearchFilterCollection(1, new SearchFilter[]
				{
					searchFilterCollection,
					searchFilter2
				});
			}
			if (type != null)
			{
				searchFilter = new SearchFilter.SearchFilterCollection(0, new SearchFilter[]
				{
					searchFilter,
					new SearchFilter.IsEqualTo(ItemSchema.ItemClass, "IPM.Notification." + type.ToString())
				});
			}
			List<SearchFilter> list = new List<SearchFilter>();
			foreach (string text in AsyncOperationNotificationDataProvider.SettingsObjectIdentityMap.Values)
			{
				list.Add(new SearchFilter.IsNotEqualTo(ExtendedEwsStoreObjectSchema.AlternativeId, text));
			}
			searchFilter = new SearchFilter.SearchFilterCollection(0, new SearchFilter[]
			{
				searchFilter,
				new SearchFilter.SearchFilterCollection(0, list.ToArray())
			});
			return searchFilter;
		}

		private DateTime GetStartDate(ExDateTime? startDate)
		{
			if (startDate == null)
			{
				startDate = new ExDateTime?(ExDateTime.Now.AddDays(-2.0));
			}
			return startDate.Value.Date.UniversalTime;
		}

		private bool IsUpdateRequired(string alternativeId, AsyncOperationStatus? status, int? percentComplete, LocalizedString? message)
		{
			Tuple<AsyncOperationStatus?, int?, LocalizedString?> tuple = null;
			bool flag;
			if (this.Cache.LatestNotifications.TryGetValue(alternativeId, out tuple))
			{
				flag = ((status != null && status != tuple.Item1) || (percentComplete != null && percentComplete != tuple.Item2) || (message != null && message != tuple.Item3));
				if (flag)
				{
					AsyncOperationStatus? asyncOperationStatus = status;
					AsyncOperationStatus? item = (asyncOperationStatus != null) ? new AsyncOperationStatus?(asyncOperationStatus.GetValueOrDefault()) : tuple.Item1;
					int? num = percentComplete;
					int? item2 = (num != null) ? new int?(num.GetValueOrDefault()) : tuple.Item2;
					LocalizedString? localizedString = message;
					tuple = new Tuple<AsyncOperationStatus?, int?, LocalizedString?>(item, item2, (localizedString != null) ? new LocalizedString?(localizedString.GetValueOrDefault()) : tuple.Item3);
					this.Cache.LatestNotifications.Add(alternativeId, tuple);
				}
			}
			else
			{
				flag = true;
				tuple = new Tuple<AsyncOperationStatus?, int?, LocalizedString?>(status, percentComplete, message);
				this.Cache.LatestNotifications.Add(alternativeId, tuple);
			}
			return flag;
		}

		protected override FolderId GetDefaultFolder()
		{
			if (this.Cache.NotificationFolderId != null)
			{
				return this.Cache.NotificationFolderId;
			}
			Folder orCreateFolder = base.GetOrCreateFolder("AsyncOperationNotification", new FolderId(10, new Mailbox(base.Mailbox.MailboxInfo.PrimarySmtpAddress.ToString())));
			this.Cache.NotificationFolderId = orCreateFolder.Id;
			base.ApplyPolicyTag(AsyncOperationNotificationDataProvider.NotifcationRetentionPolicyTagGuid, orCreateFolder, true);
			return orCreateFolder.Id;
		}

		protected override EwsStoreDataProviderCacheEntry CreateCacheEntry()
		{
			return new AsyncOperationNotificationDataProvider.CacheEntry();
		}

		private new AsyncOperationNotificationDataProvider.CacheEntry Cache
		{
			get
			{
				return (AsyncOperationNotificationDataProvider.CacheEntry)base.Cache;
			}
		}

		public const string DisableAsyncNotificationKey = "DisableAsyncNotification";

		public const string NotifcationRetentionPolicyTagName = "AsyncOperationNotification";

		private const string NotificationFolderName = "AsyncOperationNotification";

		private const int DefaultTimeWindowInDays = 2;

		public static readonly Guid NotifcationRetentionPolicyTagGuid = new Guid("df1429a2-cd71-4ff4-97da-aafe42b19237");

		public static readonly Guid AsyncOperationConfigCertificateExpiryGuid = new Guid("31e9b26b-3912-446e-80d9-03caf6a9ab8d");

		public static readonly Dictionary<AsyncOperationType, string> SettingsObjectIdentityMap = new Dictionary<AsyncOperationType, string>
		{
			{
				AsyncOperationType.CertExpiry,
				AsyncOperationNotificationDataProvider.AsyncOperationConfigCertificateExpiryGuid.ToString()
			}
		};

		private class CacheEntry : EwsStoreDataProviderCacheEntry
		{
			public MruDictionary<string, bool> BadItems = new MruDictionary<string, bool>(10, StringComparer.OrdinalIgnoreCase, null);

			public MruDictionary<string, Tuple<AsyncOperationStatus?, int?, LocalizedString?>> LatestNotifications = new MruDictionary<string, Tuple<AsyncOperationStatus?, int?, LocalizedString?>>(50, StringComparer.OrdinalIgnoreCase, null);

			public FolderId NotificationFolderId;
		}
	}
}
