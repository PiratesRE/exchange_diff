using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Office.Compliance.Audit.Schema.Mailbox;

namespace Microsoft.Exchange.Data.Storage.Auditing
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class AuditRecordFactory
	{
		public static ExchangeMailboxAuditRecord CreateMailboxItemRecord(MailboxSession mailboxSession, MailboxAuditOperations operation, COWSettings settings, OperationResult result, LogonType effectiveLogonType, bool externalAccess, StoreObjectId itemId, CoreItem item, ItemAuditInfo itemAuditInfo)
		{
			Util.ThrowOnNullArgument(settings, "settings");
			if (MailboxAuditOperations.FolderBind != operation && MailboxAuditOperations.SendAs != operation && MailboxAuditOperations.SendOnBehalf != operation && MailboxAuditOperations.Create != operation)
			{
				Util.ThrowOnNullArgument(itemId, "itemId");
			}
			ExchangeMailboxAuditRecord exchangeMailboxAuditRecord = new ExchangeMailboxAuditRecord();
			AuditRecordFactory.Fill(exchangeMailboxAuditRecord, mailboxSession, operation, result, effectiveLogonType, externalAccess);
			if (settings.CurrentFolderId != null)
			{
				exchangeMailboxAuditRecord.Item = (exchangeMailboxAuditRecord.Item ?? new ExchangeItem());
				exchangeMailboxAuditRecord.Item.ParentFolder = (exchangeMailboxAuditRecord.Item.ParentFolder ?? new ExchangeFolder());
				exchangeMailboxAuditRecord.Item.ParentFolder.Id = string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
				{
					settings.CurrentFolderId
				});
				if (itemAuditInfo != null && itemAuditInfo.ParentFolderPathName != null)
				{
					exchangeMailboxAuditRecord.Item.ParentFolder.PathName = itemAuditInfo.ParentFolderPathName;
				}
				else
				{
					string currentFolderPathName = AuditRecordFactory.GetCurrentFolderPathName(mailboxSession, settings);
					if (currentFolderPathName != null)
					{
						exchangeMailboxAuditRecord.Item.ParentFolder.PathName = currentFolderPathName;
					}
				}
			}
			if (MailboxAuditOperations.FolderBind == operation)
			{
				return exchangeMailboxAuditRecord;
			}
			if (MailboxAuditOperations.Update == operation || MailboxAuditOperations.Create == operation)
			{
				if (itemAuditInfo == null)
				{
					throw new InvalidOperationException("ItemOperationAuditEvent::itemAuditInfo should not be null for Update/Create operations.");
				}
				exchangeMailboxAuditRecord.Item = (exchangeMailboxAuditRecord.Item ?? new ExchangeItem());
				if (itemAuditInfo.Id != null)
				{
					exchangeMailboxAuditRecord.Item.Id = string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
					{
						itemAuditInfo.Id
					});
				}
				exchangeMailboxAuditRecord.Item.Subject = itemAuditInfo.Subject;
				if (MailboxAuditOperations.Update == operation)
				{
					List<string> dirtyProperties = itemAuditInfo.DirtyProperties;
					exchangeMailboxAuditRecord.ModifiedProperties = dirtyProperties;
				}
			}
			else if (MailboxAuditOperations.SendAs == operation || MailboxAuditOperations.SendOnBehalf == operation)
			{
				if (item.Id != null && item.Id.ObjectId != null)
				{
					exchangeMailboxAuditRecord.Item = (exchangeMailboxAuditRecord.Item ?? new ExchangeItem());
					exchangeMailboxAuditRecord.Item.Id = string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
					{
						item.Id.ObjectId
					});
				}
				string itemSubject = AuditRecordFactory.GetItemSubject(mailboxSession, item);
				if (itemSubject != null)
				{
					exchangeMailboxAuditRecord.Item = (exchangeMailboxAuditRecord.Item ?? new ExchangeItem());
					exchangeMailboxAuditRecord.Item.Subject = (item.PropertyBag.TryGetProperty(CoreItemSchema.Subject) as string);
				}
			}
			else if (item == null)
			{
				exchangeMailboxAuditRecord.Item = (exchangeMailboxAuditRecord.Item ?? new ExchangeItem());
				exchangeMailboxAuditRecord.Item.Id = string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
				{
					itemId
				});
			}
			else
			{
				if (item.Id != null && item.Id.ObjectId != null)
				{
					exchangeMailboxAuditRecord.Item = (exchangeMailboxAuditRecord.Item ?? new ExchangeItem());
					exchangeMailboxAuditRecord.Item.Id = string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
					{
						item.Id.ObjectId
					});
				}
				string itemSubject2 = AuditRecordFactory.GetItemSubject(mailboxSession, item);
				if (itemSubject2 != null)
				{
					exchangeMailboxAuditRecord.Item = (exchangeMailboxAuditRecord.Item ?? new ExchangeItem());
					exchangeMailboxAuditRecord.Item.Subject = (item.PropertyBag.TryGetProperty(CoreItemSchema.Subject) as string);
				}
			}
			return exchangeMailboxAuditRecord;
		}

		public static ExchangeMailboxAuditGroupRecord CreateMailboxGroupRecord(MailboxSession mailboxSession, MailboxAuditOperations operation, COWSettings settings, LogonType effectiveLogonType, bool externalAccess, StoreSession destinationSession, StoreObjectId destinationFolderId, StoreObjectId[] itemIds, GroupOperationResult result, IDictionary<StoreObjectId, FolderAuditInfo> folders, IDictionary<StoreObjectId, ItemAuditInfo> items, IDictionary<StoreObjectId, FolderAuditInfo> parentFolders)
		{
			Util.ThrowOnNullArgument(settings, "settings");
			ExchangeMailboxAuditGroupRecord exchangeMailboxAuditGroupRecord = new ExchangeMailboxAuditGroupRecord();
			AuditRecordFactory.Fill(exchangeMailboxAuditGroupRecord, mailboxSession, operation, (result == null) ? OperationResult.Failed : result.OperationResult, effectiveLogonType, externalAccess);
			if (settings.CurrentFolderId != null)
			{
				exchangeMailboxAuditGroupRecord.Folder = (exchangeMailboxAuditGroupRecord.Folder ?? new ExchangeFolder());
				exchangeMailboxAuditGroupRecord.Folder.Id = string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
				{
					settings.CurrentFolderId
				});
				FolderAuditInfo folderAuditInfo;
				parentFolders.TryGetValue(settings.CurrentFolderId, out folderAuditInfo);
				exchangeMailboxAuditGroupRecord.Folder.PathName = ((folderAuditInfo != null) ? folderAuditInfo.PathName : AuditRecordFactory.GetCurrentFolderPathName(mailboxSession, settings));
			}
			bool flag = destinationSession != null && mailboxSession != destinationSession;
			exchangeMailboxAuditGroupRecord.CrossMailboxOperation = new bool?(flag);
			MailboxSession mailboxSession2 = mailboxSession;
			if (flag && destinationSession is MailboxSession)
			{
				mailboxSession2 = (destinationSession as MailboxSession);
				exchangeMailboxAuditGroupRecord.DestMailboxGuid = new Guid?(mailboxSession2.MailboxOwner.MailboxInfo.MailboxGuid);
				exchangeMailboxAuditGroupRecord.DestMailboxOwnerUPN = string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
				{
					mailboxSession2.MailboxOwner.MailboxInfo.PrimarySmtpAddress
				});
				if (mailboxSession2.MailboxOwner.Sid != null)
				{
					exchangeMailboxAuditGroupRecord.DestMailboxOwnerSid = string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
					{
						mailboxSession2.MailboxOwner.Sid
					});
					if (mailboxSession2.MailboxOwner.MasterAccountSid != null)
					{
						exchangeMailboxAuditGroupRecord.DestMailboxOwnerMasterAccountSid = string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
						{
							mailboxSession2.MailboxOwner.MasterAccountSid
						});
					}
				}
			}
			if (destinationFolderId != null)
			{
				exchangeMailboxAuditGroupRecord.DestFolder = (exchangeMailboxAuditGroupRecord.DestFolder ?? new ExchangeFolder());
				exchangeMailboxAuditGroupRecord.DestFolder.Id = string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
				{
					destinationFolderId
				});
				string text = null;
				Exception ex = null;
				try
				{
					using (Folder folder = Folder.Bind(mailboxSession2, destinationFolderId, new PropertyDefinition[]
					{
						FolderSchema.FolderPathName
					}))
					{
						if (folder != null)
						{
							text = (folder.TryGetProperty(FolderSchema.FolderPathName) as string);
							if (text != null)
							{
								text = text.Replace(COWSettings.StoreIdSeparator, '\\');
							}
						}
					}
				}
				catch (StorageTransientException ex2)
				{
					ex = ex2;
				}
				catch (StoragePermanentException ex3)
				{
					ex = ex3;
				}
				if (ex != null)
				{
					ExTraceGlobals.SessionTracer.TraceError<StoreObjectId, Exception>((long)mailboxSession.GetHashCode(), "[GroupOperationAuditEventRecordAdapter::ToString] failed to get FolderPathName property of destination folder {0}. Exception: {1}", destinationFolderId, ex);
				}
				if (text != null)
				{
					exchangeMailboxAuditGroupRecord.DestFolder.PathName = text;
				}
			}
			foreach (KeyValuePair<StoreObjectId, FolderAuditInfo> keyValuePair in folders)
			{
				StoreObjectId key = keyValuePair.Key;
				FolderAuditInfo value = keyValuePair.Value;
				if (settings.CurrentFolderId == null || !key.Equals(settings.CurrentFolderId))
				{
					exchangeMailboxAuditGroupRecord.Folders = (exchangeMailboxAuditGroupRecord.Folders ?? new List<ExchangeFolder>(folders.Count));
					ExchangeFolder exchangeFolder = new ExchangeFolder();
					exchangeMailboxAuditGroupRecord.Folders.Add(exchangeFolder);
					exchangeFolder.Id = string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
					{
						key
					});
					if (value.PathName != null)
					{
						exchangeFolder.PathName = value.PathName;
					}
				}
			}
			foreach (KeyValuePair<StoreObjectId, ItemAuditInfo> keyValuePair2 in items)
			{
				StoreObjectId key2 = keyValuePair2.Key;
				ItemAuditInfo value2 = keyValuePair2.Value;
				exchangeMailboxAuditGroupRecord.SourceItems = (exchangeMailboxAuditGroupRecord.SourceItems ?? new List<ExchangeItem>(items.Count));
				ExchangeItem exchangeItem = new ExchangeItem();
				exchangeMailboxAuditGroupRecord.SourceItems.Add(exchangeItem);
				exchangeItem.Id = string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
				{
					key2
				});
				if (value2.Subject != null)
				{
					exchangeItem.Subject = value2.Subject;
				}
				FolderAuditInfo folderAuditInfo2;
				if (value2.ParentFolderId != null && parentFolders.TryGetValue(value2.ParentFolderId, out folderAuditInfo2))
				{
					exchangeItem.ParentFolder = (exchangeItem.ParentFolder ?? new ExchangeFolder());
					exchangeItem.ParentFolder.PathName = folderAuditInfo2.PathName;
				}
			}
			return exchangeMailboxAuditGroupRecord;
		}

		public static string GetOrgNameFromOrgId(OrganizationId orgId)
		{
			if (orgId == null || orgId.OrganizationalUnit == null)
			{
				return "First Org";
			}
			string text = orgId.OrganizationalUnit.ToCanonicalName();
			return text.Substring(text.LastIndexOf('/') + 1);
		}

		private static void Fill(ExchangeMailboxAuditBaseRecord record, MailboxSession session, MailboxAuditOperations operation, OperationResult result, LogonType logonType, bool externalAccess)
		{
			EnumValidator.ThrowIfInvalid<MailboxAuditOperations>(operation);
			EnumValidator.ThrowIfInvalid<OperationResult>(result, "result");
			EnumValidator.ThrowIfInvalid<LogonType>(logonType, "logonType");
			Util.ThrowOnNullArgument(session, "session");
			record.Operation = string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
			{
				operation
			});
			record.OperationResult = AuditRecordFactory.GetOperationResult(result);
			record.LogonType = AuditRecordFactory.GetLogonType(logonType);
			record.ExternalAccess = externalAccess;
			record.CreationTime = DateTime.UtcNow;
			record.InternalLogonType = AuditRecordFactory.GetLogonType(session.LogonType);
			record.MailboxGuid = session.MailboxOwner.MailboxInfo.MailboxGuid;
			OrganizationId organizationId = session.OrganizationId;
			record.OrganizationId = ((organizationId == null || organizationId.OrganizationalUnit == null || organizationId.ConfigurationUnit == null) ? null : Convert.ToBase64String(organizationId.GetBytes(Encoding.UTF8)));
			record.OrganizationName = AuditRecordFactory.GetOrgNameFromOrgId(organizationId);
			record.MailboxOwnerUPN = session.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString();
			if (session.MailboxOwner.Sid != null)
			{
				record.MailboxOwnerSid = string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
				{
					session.MailboxOwner.Sid
				});
				if (session.MailboxOwner.MasterAccountSid != null)
				{
					record.MailboxOwnerMasterAccountSid = string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
					{
						session.MailboxOwner.MasterAccountSid
					});
				}
			}
			IdentityPair userIdentityPair = AuditRecordFactory.GetUserIdentityPair(session, logonType);
			if (userIdentityPair.LogonUserSid != null)
			{
				record.LogonUserSid = string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
				{
					userIdentityPair.LogonUserSid
				});
				record.UserId = record.LogonUserSid;
			}
			else
			{
				record.UserId = session.MailboxOwner.MailboxInfo.MailboxGuid.ToString("D", CultureInfo.InvariantCulture);
			}
			if (userIdentityPair.LogonUserDisplayName != null)
			{
				record.LogonUserDisplayName = string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
				{
					userIdentityPair.LogonUserDisplayName
				});
			}
			string text = (session.RemoteClientSessionInfo == null) ? session.ClientInfoString : session.RemoteClientSessionInfo.ClientInfoString;
			if (!string.IsNullOrEmpty(text))
			{
				record.ClientInfoString = text;
			}
			record.ClientIPAddress = string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
			{
				session.ClientIPAddress
			});
			if (!string.IsNullOrEmpty(session.ClientMachineName))
			{
				record.ClientMachineName = session.ClientMachineName;
			}
			if (!string.IsNullOrEmpty(session.ClientProcessName))
			{
				record.ClientProcessName = session.ClientProcessName;
			}
			if (session.ClientVersion != 0L)
			{
				record.ClientVersion = AuditRecordFactory.GetVersionString(session.ClientVersion);
			}
			record.OriginatingServer = string.Format(CultureInfo.InvariantCulture, "{0} ({1})\r\n", new object[]
			{
				AuditRecordFactory.MachineName,
				"15.00.1497.012"
			});
		}

		private static OperationResult GetOperationResult(OperationResult result)
		{
			if (result == OperationResult.Succeeded)
			{
				return 0;
			}
			if (result != OperationResult.PartiallySucceeded)
			{
				return 1;
			}
			return 2;
		}

		private static IdentityPair GetUserIdentityPair(MailboxSession session, LogonType logonType)
		{
			IdentityPair result = default(IdentityPair);
			ClientSessionInfo remoteClientSessionInfo = session.RemoteClientSessionInfo;
			if (remoteClientSessionInfo != null)
			{
				result.LogonUserSid = remoteClientSessionInfo.LogonUserSid;
				result.LogonUserDisplayName = remoteClientSessionInfo.LogonUserDisplayName;
			}
			else
			{
				result = IdentityHelper.GetIdentityPair(session);
				if (result.LogonUserDisplayName == null && logonType == LogonType.Owner && session.MailboxOwner.MailboxInfo.IsArchive)
				{
					result.LogonUserDisplayName = AuditRecordFactory.ResolveMailboxOwnerName(session.MailboxOwner);
				}
			}
			return result;
		}

		private static string ResolveMailboxOwnerName(IExchangePrincipal owner)
		{
			string result;
			if (!string.IsNullOrEmpty(owner.MailboxInfo.DisplayName))
			{
				result = owner.MailboxInfo.DisplayName;
			}
			else if (!string.IsNullOrEmpty(owner.Alias))
			{
				result = owner.Alias;
			}
			else
			{
				result = ((owner.ObjectId == null) ? string.Empty : owner.ObjectId.ToString());
			}
			return result;
		}

		private static string GetVersionString(long versionNumber)
		{
			ushort num = (ushort)(versionNumber >> 48);
			ushort num2 = (ushort)((versionNumber & 281470681743360L) >> 32);
			ushort num3 = (ushort)((versionNumber & (long)((ulong)-65536)) >> 16);
			ushort num4 = (ushort)(versionNumber & 65535L);
			return string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}.{3}", new object[]
			{
				num,
				num2,
				num3,
				num4
			});
		}

		private static LogonType GetLogonType(LogonType logonType)
		{
			switch (logonType)
			{
			case LogonType.Owner:
				return 0;
			case LogonType.Admin:
				return 1;
			case LogonType.Delegated:
				return 2;
			case LogonType.Transport:
				return 3;
			case LogonType.SystemService:
				return 4;
			case LogonType.BestAccess:
				return 5;
			case LogonType.DelegatedAdmin:
				return 6;
			default:
				throw new ArgumentOutOfRangeException("logonType");
			}
		}

		private static string GetCurrentFolderPathName(MailboxSession session, COWSettings settings)
		{
			string text = null;
			Folder currentFolder = settings.GetCurrentFolder(session);
			if (currentFolder != null)
			{
				text = (currentFolder.TryGetProperty(FolderSchema.FolderPathName) as string);
				if (text != null)
				{
					text = text.Replace(COWSettings.StoreIdSeparator, '\\');
				}
			}
			return text;
		}

		private static string GetItemSubject(MailboxSession session, CoreItem item)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(item, "item");
			return item.PropertyBag.TryGetProperty(ItemSchema.Subject) as string;
		}

		private static readonly string MachineName = Environment.MachineName;
	}
}
