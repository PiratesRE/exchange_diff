using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class COWSettings
	{
		static COWSettings()
		{
			COWSettings.SetGlobalDefaults();
		}

		public COWSettings(MailboxSession session)
		{
			Util.ThrowOnNullArgument(session, "session");
			this.session = session;
			this.currentFolderId = null;
			this.currentFolder = null;
			this.temporaryDisableHold = false;
			this.temporaryDisableCalendarLogging = false;
			this.BuildUpdateStampMetadata();
		}

		public static TimeSpan COWCacheLifeTime
		{
			get
			{
				return COWSettings.cowInfoCache.COWCacheLifeTime;
			}
		}

		public MailboxSession Session
		{
			get
			{
				return this.session;
			}
		}

		internal static bool DumpsterEnabledOverwrite
		{
			get
			{
				return COWSettings.dumpsterEnabled;
			}
			set
			{
				COWSettings.dumpsterEnabled = value;
			}
		}

		internal static bool DumpsterNonIPMOverwrite
		{
			get
			{
				return COWSettings.dumpsterNonIPM;
			}
			set
			{
				COWSettings.dumpsterNonIPM = value;
			}
		}

		internal static bool CalendarLoggingEnabledOverwrite
		{
			get
			{
				return COWSettings.calendarLoggingEnabled;
			}
			set
			{
				COWSettings.calendarLoggingEnabled = value;
			}
		}

		internal static bool IsImapPoisonMessage(bool onBeforeNotification, COWTriggerAction operation, StoreSession session, CoreItem item)
		{
			return onBeforeNotification && operation == COWTriggerAction.Update && session.ClientInfoString.Equals("Client=IMAP4") && item != null && item.PropertyBag.GetValueOrDefault<bool>(InternalSchema.MimeConversionFailed);
		}

		public bool TemporaryDisableHold
		{
			get
			{
				return this.temporaryDisableHold;
			}
			set
			{
				if (this.session.LogonType == LogonType.Admin || this.session.LogonType == LogonType.Transport)
				{
					ExTraceGlobals.SessionTracer.TraceError<bool>((long)this.session.GetHashCode(), "Setting TemporaryDisableHold to {0}", value);
					this.temporaryDisableHold = value;
				}
			}
		}

		public bool TemporaryDisableCalendarLogging
		{
			get
			{
				return this.temporaryDisableCalendarLogging;
			}
			set
			{
				if (this.session.LogonType == LogonType.Admin || this.session.LogonType == LogonType.Transport)
				{
					ExTraceGlobals.SessionTracer.TraceError<bool>((long)this.session.GetHashCode(), "Setting TemporaryDisableCalendarLogging to {0}", value);
					this.temporaryDisableCalendarLogging = value;
				}
			}
		}

		public Unlimited<ByteQuantifiedSize> DumpsterWarningQuota
		{
			get
			{
				return this.GetMailboxInfo().DumpsterWarningQuota;
			}
		}

		public Unlimited<ByteQuantifiedSize> CalendarLoggingQuota
		{
			get
			{
				return this.GetMailboxInfo().CalendarLoggingQuota;
			}
		}

		public static bool IsPermissibleInferenceAction(MailboxSession session, CoreItem item)
		{
			return session != null && item != null && ((session.LogonType == LogonType.Admin || session.LogonType == LogonType.SystemService) && session.ClientInfoString.Contains("InferenceTrainingAssistant")) && !item.IsLegallyDirty;
		}

		public static bool IsMrmAction(MailboxSession session)
		{
			return session != null && (session.LogonType == LogonType.Admin || session.LogonType == LogonType.SystemService) && session.ClientInfoString.Contains("ELCAssistant");
		}

		public static bool IsCalendarRepairAssistantAction(StoreSession session)
		{
			return session != null && (session.LogonType == LogonType.Admin || session.LogonType == LogonType.SystemService) && session.ClientInfoString.Contains("CalendarRepairAssistant");
		}

		internal static void SetGlobalDefaults()
		{
			COWSettings.dumpsterEnabled = true;
			COWSettings.dumpsterNonIPM = false;
			COWSettings.calendarLoggingEnabled = true;
			bool flag = true;
			int capacity = 100;
			int expireTimeInMinutes = 1;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\RecoverableItems", RegistryKeyPermissionCheck.ReadSubTree))
			{
				if (registryKey != null)
				{
					object value = registryKey.GetValue("NonIPM");
					if (value != null && value is int && (int)value == 1)
					{
						COWSettings.dumpsterNonIPM = true;
						ExTraceGlobals.SessionTracer.TraceDebug(0L, "COWSettings using the registry dumpster nonIPM override value!");
					}
					else
					{
						ExTraceGlobals.SessionTracer.TraceDebug(0L, "COWSettings failed opening the registry dumpster nonIPM override value!");
					}
					value = registryKey.GetValue("AdminMailboxSessionCacheEnabled");
					if (value != null && value is int)
					{
						flag = ((int)value != 0);
						ExTraceGlobals.SessionTracer.TraceDebug(0L, "COWSettings using the registry admin mailbox session cache enabled!");
					}
					else
					{
						ExTraceGlobals.SessionTracer.TraceDebug(0L, "COWSettings failed opening the registry admin mailbox session cache enabled override value!");
					}
					value = registryKey.GetValue("AdminMailboxSessionCacheExpireTime");
					if (value != null && value is int)
					{
						expireTimeInMinutes = (int)value;
						ExTraceGlobals.SessionTracer.TraceDebug(0L, "COWSettings using the registry admin mailbox session cache expire time override value!");
					}
					else
					{
						ExTraceGlobals.SessionTracer.TraceDebug(0L, "COWSettings failed opening the registry admin mailbox session cache expire time override value!");
					}
					value = registryKey.GetValue("AdminMailboxSessionCacheSize");
					if (value != null && value is int)
					{
						capacity = (int)value;
						ExTraceGlobals.SessionTracer.TraceDebug(0L, "COWSettings using the registry admin mailbox session cache size override value!");
					}
					else
					{
						ExTraceGlobals.SessionTracer.TraceDebug(0L, "COWSettings failed opening the registry admin mailbox session cache size override value!");
					}
				}
			}
			if (flag)
			{
				COWSettings.adminMailboxSessionCache = new MruDictionaryListCache<Guid, MailboxSession>(capacity, expireTimeInMinutes, (MailboxSession session) => session.MailboxGuid);
			}
		}

		internal static void PurgeCache()
		{
			COWSettings.cowInfoCache.PurgeCache();
			if (COWSettings.adminMailboxSessionCache != null)
			{
				COWSettings.adminMailboxSessionCache.Dispose();
				COWSettings.adminMailboxSessionCache = null;
			}
			COWSettings.SetGlobalDefaults();
		}

		internal static MailboxSession GetAdminMailboxSession(MailboxSession session)
		{
			MailboxSession mailboxSession = null;
			if (COWSettings.adminMailboxSessionCache != null && COWSettings.adminMailboxSessionCache.TryGetAndRemoveValue(session.MailboxGuid, out mailboxSession))
			{
				if (!mailboxSession.IsConnected)
				{
					mailboxSession.ConnectWithStatus();
				}
			}
			else
			{
				mailboxSession = MailboxSession.OpenAsAdmin(session.MailboxOwner, session.InternalCulture, session.ClientInfoString + ";COW=Delegate");
			}
			return mailboxSession;
		}

		internal static void ReturnAdminMailboxSession(MailboxSession session)
		{
			if (COWSettings.adminMailboxSessionCache != null)
			{
				COWSettings.adminMailboxSessionCache.Add(session.MailboxGuid, session);
				return;
			}
			session.Dispose();
		}

		internal static void AddMetadata(COWSettings settings, ICoreItem item, COWTriggerAction action)
		{
			Dictionary<PropertyDefinition, object> actionMetadata = settings.GetSessionMetadata();
			foreach (KeyValuePair<PropertyDefinition, object> keyValuePair in actionMetadata)
			{
				if (keyValuePair.Key == InternalSchema.ClientInfoString)
				{
					IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
					if (currentActivityScope != null && !string.IsNullOrEmpty(currentActivityScope.ClientInfo) && !string.IsNullOrEmpty(currentActivityScope.Component) && string.Equals(currentActivityScope.Component, "ActiveSync"))
					{
						item.PropertyBag[keyValuePair.Key] = currentActivityScope.ClientInfo;
						continue;
					}
				}
				item.PropertyBag[keyValuePair.Key] = keyValuePair.Value;
			}
			actionMetadata = COWSettings.GetActionMetadata(item, action);
			foreach (KeyValuePair<PropertyDefinition, object> keyValuePair2 in actionMetadata)
			{
				item.PropertyBag[keyValuePair2.Key] = keyValuePair2.Value;
			}
		}

		internal static Dictionary<PropertyDefinition, object> GetActionMetadata(ICoreItem item, COWTriggerAction action)
		{
			return new Dictionary<PropertyDefinition, object>(2)
			{
				{
					InternalSchema.CalendarLogTriggerAction,
					action.ToString()
				},
				{
					InternalSchema.OriginalLastModifiedTime,
					ExDateTime.Now
				}
			};
		}

		internal StoreObjectId CurrentFolderId
		{
			get
			{
				return this.currentFolderId;
			}
			set
			{
				if ((value == null && this.currentFolder != null) || (value != null && this.currentFolder != null && !this.currentFolderId.Equals(value)))
				{
					this.ResetCurrentFolder();
				}
				this.currentFolderId = value;
			}
		}

		public static bool HoldEnabled(MailboxSession mailboxSession)
		{
			COWSettings.COWMailboxInfo cowmailboxInfo = COWSettings.cowInfoCache.GetCOWMailboxInfo(mailboxSession);
			return cowmailboxInfo.SingleItemRecovery || cowmailboxInfo.LitigationHold || cowmailboxInfo.InPlaceHoldEnabled;
		}

		public bool LegalHoldEnabled()
		{
			return this.LitigationHoldEnabled() || this.InPlaceHoldEnabled();
		}

		public bool HoldEnabled()
		{
			return !this.TemporaryDisableHold && (this.GetMailboxInfo().SingleItemRecovery || this.GetMailboxInfo().LitigationHold || this.GetMailboxInfo().InPlaceHoldEnabled);
		}

		public bool IsOnlyInPlaceHoldEnabled()
		{
			return !this.LitigationHoldEnabled() && !this.SIREnabled() && this.InPlaceHoldEnabled();
		}

		public bool IsCalendarLoggingEnabled()
		{
			return COWSettings.calendarLoggingEnabled && !this.TemporaryDisableCalendarLogging && this.GetMailboxInfo().CalendarLogging;
		}

		public bool IsSiteMailboxMessageDedupEnabled()
		{
			return this.GetMailboxInfo().SiteMailboxMessageDedup;
		}

		public bool IsMrmAction()
		{
			return COWSettings.IsMrmAction(this.session);
		}

		public bool IsPermissibleInferenceAction(CoreItem item)
		{
			return COWSettings.IsPermissibleInferenceAction(this.session, item);
		}

		private bool SIREnabled()
		{
			return !this.TemporaryDisableHold && this.GetMailboxInfo().SingleItemRecovery;
		}

		private bool InPlaceHoldEnabled()
		{
			return !this.TemporaryDisableHold && this.GetMailboxInfo().InPlaceHoldEnabled;
		}

		private bool LitigationHoldEnabled()
		{
			return !this.TemporaryDisableHold && this.GetMailboxInfo().LitigationHold;
		}

		public ulong? GetCalendarLoggingFolderSize(MailboxSession session)
		{
			return COWSettings.cowInfoCache.GetCalendarLoggingFolderSize(session);
		}

		internal Dictionary<PropertyDefinition, object> GetSessionMetadata()
		{
			return this.sessionMetadata;
		}

		internal bool IsCurrentFolderExcludedFromAuditing(MailboxSession sessionWithBestAccess)
		{
			if (this.currentFolderId == null)
			{
				return false;
			}
			if (this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.Inbox)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.Outbox)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.Calendar)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.Contacts)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.SentItems)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.DeletedItems)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.Contacts)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.Drafts)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.Tasks)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.Notes)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.Journal)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.Root)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.RecoverableItemsRoot)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.RecoverableItemsVersions)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.RecoverableItemsDeletions)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.RecoverableItemsPurges)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.CalendarLogging)))
			{
				return false;
			}
			StoreObjectId auditsFolderId = null;
			sessionWithBestAccess.BypassAuditsFolderAccessChecking(delegate
			{
				auditsFolderId = sessionWithBestAccess.GetAuditsFolderId();
			});
			if (this.currentFolderId.Equals(auditsFolderId))
			{
				ExTraceGlobals.SessionTracer.TraceDebug((long)sessionWithBestAccess.GetHashCode(), "COWSettings::IsCurrentFolderExcludedFromAuditing returns true since the current folder is audit folder.");
				return true;
			}
			if (this.currentFolderId.ObjectType == StoreObjectType.SearchFolder || this.currentFolderId.ObjectType == StoreObjectType.OutlookSearchFolder)
			{
				ExTraceGlobals.SessionTracer.TraceDebug<StoreObjectType>((long)this.session.GetHashCode(), "COWSettings::IsCurrentFolderExcludedFromAuditing returns true since the current folder id indicates a search folder of type {0}.", this.currentFolderId.ObjectType);
				return true;
			}
			try
			{
				if (this.GetCurrentFolder(sessionWithBestAccess) == null)
				{
					return false;
				}
			}
			catch (TooManyObjectsOpenedException)
			{
				ExTraceGlobals.SessionTracer.TraceDebug((long)this.session.GetHashCode(), "COWSettings::IsCurrentFolderExcludedFromAuditing returns true since there are too many objects opened in this session.  Auditing will fail anyway.");
				return true;
			}
			if (this.currentFolder.Id.ObjectId.ObjectType == StoreObjectType.SearchFolder || this.currentFolder.Id.ObjectId.ObjectType == StoreObjectType.OutlookSearchFolder)
			{
				ExTraceGlobals.SessionTracer.TraceDebug<StoreObjectType>((long)this.session.GetHashCode(), "COWSettings::IsCurrentFolderExcludedFromAuditing returns true since the current folder is search folder of type {0}.", this.currentFolder.Id.ObjectId.ObjectType);
				return true;
			}
			StoreFolderFlags valueOrDefault = (StoreFolderFlags)this.currentFolder.GetValueOrDefault<int>(FolderSchema.FolderFlags, 1);
			bool flag = (valueOrDefault & StoreFolderFlags.FolderIPM) == StoreFolderFlags.FolderIPM;
			if (!flag)
			{
				ExTraceGlobals.SessionTracer.TraceDebug((long)this.session.GetHashCode(), "COWSettings::IsCurrentFolderExcludedFromAuditing returns true since the current folder is non-IPM.");
			}
			return !flag;
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

		private void BuildUpdateStampMetadata()
		{
			this.sessionMetadata = new Dictionary<PropertyDefinition, object>(11);
			this.sessionMetadata.Add(InternalSchema.ClientProcessName, this.session.ClientProcessName ?? string.Empty);
			this.sessionMetadata.Add(InternalSchema.ClientMachineName, this.session.ClientMachineName ?? string.Empty);
			this.sessionMetadata.Add(InternalSchema.ClientBuildVersion, COWSettings.GetVersionString(this.session.ClientVersion));
			this.sessionMetadata.Add(InternalSchema.MiddleTierProcessName, COWSettings.middleTierProcessName);
			this.sessionMetadata.Add(InternalSchema.MiddleTierServerName, COWSettings.middleTierServerName);
			this.sessionMetadata.Add(InternalSchema.MiddleTierServerBuildVersion, COWSettings.middleTierServerBuildVersion);
			this.sessionMetadata.Add(InternalSchema.ClientInfoString, this.session.ClientInfoString ?? string.Empty);
			this.sessionMetadata.Add(InternalSchema.ResponsibleUserName, this.session.UserLegacyDN);
			MailboxSession mailboxSession = this.session;
			if (mailboxSession != null && mailboxSession.MailboxOwner != null && !mailboxSession.IsRemote)
			{
				this.sessionMetadata.Add(InternalSchema.MailboxServerName, mailboxSession.MailboxOwner.MailboxInfo.Location.ServerFqdn);
				ServerVersion serverVersion = new ServerVersion(mailboxSession.MailboxOwner.MailboxInfo.Location.ServerVersion);
				this.sessionMetadata.Add(InternalSchema.MailboxServerBuildVersion, serverVersion.ToString());
				this.sessionMetadata.Add(InternalSchema.MailboxDatabaseName, mailboxSession.MailboxOwner.MailboxInfo.Location.DatabaseLegacyDn);
				return;
			}
			this.sessionMetadata.Add(InternalSchema.MailboxServerName, string.Empty);
			this.sessionMetadata.Add(InternalSchema.MailboxServerBuildVersion, string.Empty);
			this.sessionMetadata.Add(InternalSchema.MailboxDatabaseName, string.Empty);
		}

		internal Folder GetCurrentFolder(MailboxSession sessionWithBestAccess)
		{
			if (this.currentFolder == null || this.currentFolder.IsDisposed)
			{
				if (this.currentFolderId != null)
				{
					try
					{
						this.currentFolder = Folder.Bind(sessionWithBestAccess, this.currentFolderId, COWSettings.folderLoadProperties);
						goto IL_61;
					}
					catch (ObjectNotFoundException)
					{
						ExTraceGlobals.SessionTracer.TraceWarning<StoreObjectId>((long)this.session.GetHashCode(), "Folder Id {0} was not found", this.currentFolderId);
						goto IL_61;
					}
				}
				this.currentFolder = null;
			}
			IL_61:
			return this.currentFolder;
		}

		internal void ResetCurrentFolder()
		{
			if (this.currentFolder != null)
			{
				this.currentFolder.Dispose();
				this.currentFolder = null;
			}
		}

		internal bool IsCurrentFolderEnabled(MailboxSession sessionWithBestAccess)
		{
			return this.InternalIsCurrentFolderEnabled(sessionWithBestAccess, null);
		}

		internal bool IsCurrentFolderItemEnabled(MailboxSession sessionWithBestAccess, ICoreItem item)
		{
			return this.InternalIsCurrentFolderEnabled(sessionWithBestAccess, item);
		}

		private bool InternalIsCurrentFolderEnabled(MailboxSession sessionWithBestAccess, ICoreItem item)
		{
			if (this.currentFolderId == null)
			{
				return true;
			}
			if (this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.Inbox)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.Outbox)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.Calendar)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.Contacts)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.SentItems)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.DeletedItems)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.Contacts)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.Drafts)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.Tasks)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.Notes)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.Journal)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.Root)))
			{
				return true;
			}
			if (this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.RecoverableItemsRoot)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.RecoverableItemsVersions)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.RecoverableItemsDeletions)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.RecoverableItemsPurges)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.RecoverableItemsDiscoveryHolds)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.CalendarLogging)))
			{
				return true;
			}
			if (this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.Configuration)))
			{
				return COWSettings.dumpsterNonIPM;
			}
			if (this.currentFolderId.ObjectType == StoreObjectType.SearchFolder || this.currentFolderId.ObjectType == StoreObjectType.OutlookSearchFolder)
			{
				return this.IsParentFolderEnabled(sessionWithBestAccess, item);
			}
			if (this.GetCurrentFolder(sessionWithBestAccess) == null)
			{
				return false;
			}
			if (this.currentFolderId.ObjectType == StoreObjectType.Unknown && (this.currentFolder.Id.ObjectId.ObjectType == StoreObjectType.SearchFolder || this.currentFolder.Id.ObjectId.ObjectType == StoreObjectType.OutlookSearchFolder))
			{
				return this.IsParentFolderEnabled(sessionWithBestAccess, item);
			}
			StoreFolderFlags valueOrDefault = (StoreFolderFlags)this.currentFolder.GetValueOrDefault<int>(FolderSchema.FolderFlags, 1);
			return COWSettings.dumpsterNonIPM || (valueOrDefault & StoreFolderFlags.FolderIPM) == StoreFolderFlags.FolderIPM;
		}

		private bool IsParentFolderEnabled(MailboxSession sessionWithBestAccess, ICoreItem item)
		{
			if (item != null)
			{
				StoreObjectId folderId;
				StoreObjectId storeObjectId;
				this.GetRealParentFolderForItem(item, out folderId, out storeObjectId);
				return this.IsFolderEnabled(sessionWithBestAccess, folderId);
			}
			return false;
		}

		private bool IsFolderEnabled(MailboxSession sessionWithBestAccess, StoreObjectId folderId)
		{
			if (this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.Inbox)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.Outbox)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.Calendar)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.Contacts)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.SentItems)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.DeletedItems)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.Contacts)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.Drafts)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.Tasks)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.Notes)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.Journal)) || this.currentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.Root)))
			{
				return true;
			}
			if (folderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.RecoverableItemsRoot)) || folderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.RecoverableItemsVersions)) || folderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.RecoverableItemsDeletions)) || folderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.RecoverableItemsPurges)) || folderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.CalendarLogging)))
			{
				return true;
			}
			try
			{
				using (Folder.Bind(sessionWithBestAccess, folderId, COWSettings.folderLoadProperties))
				{
					StoreFolderFlags valueOrDefault = (StoreFolderFlags)this.currentFolder.GetValueOrDefault<int>(FolderSchema.FolderFlags, 1);
					return COWSettings.dumpsterNonIPM || (valueOrDefault & StoreFolderFlags.FolderIPM) == StoreFolderFlags.FolderIPM;
				}
			}
			catch (ObjectNotFoundException)
			{
				ExTraceGlobals.SessionTracer.TraceWarning<StoreObjectId>((long)sessionWithBestAccess.GetHashCode(), "Folder Id {0} was not found", folderId);
			}
			return false;
		}

		internal void GetRealParentFolderForItem(ICoreItem item, out StoreObjectId realParentId, out StoreObjectId realItemId)
		{
			realItemId = item.StoreObjectId;
			realParentId = IdConverter.GetParentIdFromMessageId(item.StoreObjectId);
			long valueOrDefault = item.PropertyBag.GetValueOrDefault<long>(ItemSchema.Fid, 0L);
			if (valueOrDefault != 0L)
			{
				long midFromMessageId = this.session.IdConverter.GetMidFromMessageId(item.Id.ObjectId);
				realParentId = this.session.IdConverter.CreateFolderId(valueOrDefault);
				realItemId = this.session.IdConverter.CreateMessageId(valueOrDefault, midFromMessageId);
				ExTraceGlobals.SessionTracer.TraceDebug<string, string, string>((long)this.session.GetHashCode(), "GroupItemsPerParent: Bind id {0}, real id {1}, real parent id {2}", item.Id.ObjectId.ToHexEntryId(), realItemId.ToHexEntryId(), realParentId.ToHexEntryId());
			}
		}

		internal COWSettings.COWMailboxInfo GetMailboxInfo()
		{
			if (this.mailboxInfo == null)
			{
				this.mailboxInfo = new COWSettings.COWMailboxInfo?(COWSettings.cowInfoCache.GetCOWMailboxInfo(this.session));
			}
			return this.mailboxInfo.Value;
		}

		internal void ResetMailboxInfo()
		{
			this.mailboxInfo = null;
		}

		internal void RemoveMailboxInfoCache(Guid mailboxGuid)
		{
			COWSettings.cowInfoCache.RemoveCacheEntryForMailbox(mailboxGuid);
		}

		internal static COWSettings CreateDummyInstance()
		{
			return new COWSettings();
		}

		private COWSettings()
		{
		}

		internal const string RegKeyStringDumpster = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\RecoverableItems";

		internal const string RegValueStringNonIPM = "NonIPM";

		internal const string RegValueStringAdminMailboxSessionCacheEnabled = "AdminMailboxSessionCacheEnabled";

		internal const string RegValueStringAdminMailboxSessionCacheExpireTime = "AdminMailboxSessionCacheExpireTime";

		internal const string RegValueStringAdminMailboxSessionCacheSize = "AdminMailboxSessionCacheSize";

		internal const string MrmClientString = "ELCAssistant";

		internal const string HoldErrorComponentNameForCrimsonChannel = "Hold.HoldErrors.Monitor";

		internal const string InferenceClientString = "InferenceTrainingAssistant";

		internal const string CalendarRepairClientString = "CalendarRepairAssistant";

		internal static readonly char StoreIdSeparator = '￾';

		private static COWSettings.COWInfoCache cowInfoCache = new COWSettings.COWInfoCache();

		private static MruDictionaryListCache<Guid, MailboxSession> adminMailboxSessionCache;

		private static string middleTierProcessName = Process.GetCurrentProcess().ProcessName;

		private static string middleTierServerName = Util.GetMachineName();

		private static string middleTierServerBuildVersion = StorageGlobals.BuildVersionString;

		private static ICollection<PropertyDefinition> folderLoadProperties = new PropertyDefinition[]
		{
			FolderSchema.FolderFlags,
			FolderSchema.FolderPathName
		};

		private static ICollection<PropertyDefinition> requiredItemsProperties = new PropertyDefinition[]
		{
			InternalSchema.ItemVersion
		};

		private static bool dumpsterEnabled = true;

		private static bool dumpsterNonIPM = false;

		private static bool calendarLoggingEnabled = true;

		private MailboxSession session;

		private StoreObjectId currentFolderId;

		private Folder currentFolder;

		private bool temporaryDisableHold;

		private bool temporaryDisableCalendarLogging;

		private Dictionary<PropertyDefinition, object> sessionMetadata;

		private COWSettings.COWMailboxInfo? mailboxInfo;

		internal interface IExpirableCacheEntry
		{
			ExDateTime InfoExpireTime { get; set; }
		}

		internal struct COWMailboxInfo : COWSettings.IExpirableCacheEntry
		{
			public ExDateTime InfoExpireTime { get; set; }

			public bool LitigationHold;

			public bool CalendarLogging;

			public bool SiteMailboxMessageDedup;

			public bool SingleItemRecovery;

			public Unlimited<ByteQuantifiedSize> DumpsterWarningQuota;

			public Unlimited<ByteQuantifiedSize> CalendarLoggingQuota;

			public bool InPlaceHoldEnabled;
		}

		internal struct COWDatabaseInfo : COWSettings.IExpirableCacheEntry
		{
			public ExDateTime InfoExpireTime { get; set; }

			public Unlimited<ByteQuantifiedSize> DumpsterWarningQuota;

			public Unlimited<ByteQuantifiedSize> CalendarLoggingQuota;

			public Unlimited<ByteQuantifiedSize> DumpsterQuota;
		}

		internal struct COWTenantInfo : COWSettings.IExpirableCacheEntry
		{
			public ExDateTime InfoExpireTime { get; set; }

			public bool CalendarVersionStoreEnabled;
		}

		internal struct CalendarLoggingFolderInfo
		{
			public ulong Size;

			public uint AccessCount;
		}

		internal class COWInfoCache
		{
			public COWInfoCache() : this(TimeSpan.FromHours(1.0))
			{
			}

			public COWInfoCache(TimeSpan cacheLifeTime)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<TimeSpan>((long)this.GetHashCode(), "COWInfoCache constructor with life time {0}", cacheLifeTime);
				this.cowCacheLifeTime = cacheLifeTime;
				this.cowCacheAccessLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
				this.cowMailboxInfoCache = new Dictionary<Guid, COWSettings.COWMailboxInfo>();
				this.cowDatabaseInfoCache = new Dictionary<ADObjectId, COWSettings.COWDatabaseInfo>();
				this.cowTenantInfoCache = new Dictionary<OrganizationId, COWSettings.COWTenantInfo>();
				this.calendarLoggingFolderInfoCache = new Dictionary<Guid, COWSettings.CalendarLoggingFolderInfo>();
				this.ComputeNextCleanupCacheTime();
			}

			public TimeSpan COWCacheLifeTime
			{
				get
				{
					return this.cowCacheLifeTime;
				}
			}

			public bool LegalHoldEnabled(StoreSession session)
			{
				return this.GetCOWMailboxInfo(session).LitigationHold || this.GetCOWMailboxInfo(session).InPlaceHoldEnabled;
			}

			public bool CalendarLoggingEnabled(StoreSession session)
			{
				return this.GetCOWMailboxInfo(session).CalendarLogging;
			}

			public bool SingleItemRecoveryEnabled(StoreSession session)
			{
				return this.GetCOWMailboxInfo(session).SingleItemRecovery;
			}

			internal void RemoveCacheEntryForMailbox(Guid mailboxGuid)
			{
				this.cowMailboxInfoCache.Remove(mailboxGuid);
			}

			internal Dictionary<Guid, COWSettings.COWMailboxInfo> GetCache()
			{
				return this.cowMailboxInfoCache;
			}

			private void TimedOperation(bool writer, COWSettings.COWInfoCache.LockedMethod m)
			{
				ExDateTime now = ExDateTime.Now;
				ExDateTime now2 = ExDateTime.Now;
				try
				{
					now2 = ExDateTime.Now;
					bool flag;
					if (writer)
					{
						flag = this.cowCacheAccessLock.TryEnterWriteLock(this.timeoutLimit);
					}
					else
					{
						flag = this.cowCacheAccessLock.TryEnterReadLock(this.timeoutLimit);
					}
					if (!flag)
					{
						this.timeoutEventsTotal++;
						ExTraceGlobals.SessionTracer.TraceError<double, int>(0L, "COWInfoCache lock wait more than {0} seconds, {1} times.", this.timeoutLimit.TotalSeconds, this.timeoutEventsTotal);
						throw new DumpsterOperationException(ServerStrings.COWMailboxInfoCacheTimeout(this.timeoutLimit.TotalSeconds, this.timeoutEventsTotal), null);
					}
					m();
				}
				finally
				{
					try
					{
						if (writer)
						{
							this.cowCacheAccessLock.ExitWriteLock();
						}
						else
						{
							this.cowCacheAccessLock.ExitReadLock();
						}
					}
					catch (SynchronizationLockException)
					{
					}
					if (ExDateTime.Now > now + this.longWaitLimit)
					{
						this.longWaitEventsTotal++;
						ExTraceGlobals.SessionTracer.TraceError(0L, "Long wait on the mailbox information cache. The limit of {0} seconds was hit {1} times, timeout of {2} seconds was hit {3} times. Execution time {4} seconds", new object[]
						{
							this.longWaitLimit.TotalSeconds,
							this.longWaitEventsTotal,
							this.timeoutLimit.TotalSeconds,
							this.timeoutEventsTotal,
							(ExDateTime.Now - now2).TotalSeconds
						});
						StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_ErrorCOWCacheWaitTimeout, null, new object[]
						{
							COWSettings.middleTierProcessName,
							this.longWaitLimit.TotalSeconds,
							this.longWaitEventsTotal,
							this.timeoutLimit.TotalSeconds,
							this.timeoutEventsTotal,
							(ExDateTime.Now - now2).TotalSeconds
						});
					}
				}
			}

			internal void PurgeCache()
			{
				this.TimedOperation(false, delegate
				{
					this.cowMailboxInfoCache.Clear();
					this.cowDatabaseInfoCache.Clear();
					this.cowTenantInfoCache.Clear();
					this.calendarLoggingFolderInfoCache.Clear();
					this.perfCounters.DumpsterADSettingCacheSize.RawValue = 0L;
				});
			}

			internal COWSettings.COWMailboxInfo GetCOWMailboxInfo(StoreSession session)
			{
				Util.ThrowOnNullArgument(session, "session");
				bool resultFound = false;
				bool cacheCleanupNeeded = false;
				COWSettings.COWMailboxInfo result = default(COWSettings.COWMailboxInfo);
				this.perfCounters.DumpsterADSettingCacheMisses_Base.Increment();
				this.TimedOperation(false, delegate
				{
					resultFound = this.cowMailboxInfoCache.TryGetValue(session.MailboxGuid, out result);
					cacheCleanupNeeded = (this.timeNextCacheCleanup < ExDateTime.Now);
				});
				if (!resultFound || result.InfoExpireTime < ExDateTime.Now)
				{
					ExTraceGlobals.SessionTracer.TraceDebug<string>((long)session.GetHashCode(), "Reading CopyOnWrite info from AD (reason {0}).", resultFound ? "cache outdated" : "missing from cache");
					COWSettings.COWMailboxInfo updateInfo = this.GetCOWMailboxInfoFromAD(session);
					if (!resultFound)
					{
						this.perfCounters.DumpsterADSettingCacheSize.Increment();
					}
					this.perfCounters.DumpsterADSettingCacheMisses.Increment();
					this.TimedOperation(true, delegate
					{
						this.cowMailboxInfoCache[session.MailboxGuid] = updateInfo;
					});
					result = updateInfo;
					resultFound = true;
				}
				if (cacheCleanupNeeded)
				{
					this.CleanupCacheTimeBased();
				}
				return result;
			}

			internal ulong? GetCalendarLoggingFolderSize(MailboxSession session)
			{
				COWSettings.COWInfoCache.<>c__DisplayClassf CS$<>8__locals1 = new COWSettings.COWInfoCache.<>c__DisplayClassf();
				CS$<>8__locals1.session = session;
				CS$<>8__locals1.<>4__this = this;
				Util.ThrowOnNullArgument(CS$<>8__locals1.session, "session");
				CS$<>8__locals1.resultFound = false;
				CS$<>8__locals1.calendarLoggingFolderInfo = default(COWSettings.CalendarLoggingFolderInfo);
				this.TimedOperation(false, delegate
				{
					CS$<>8__locals1.resultFound = CS$<>8__locals1.<>4__this.calendarLoggingFolderInfoCache.TryGetValue(CS$<>8__locals1.session.MailboxGuid, out CS$<>8__locals1.calendarLoggingFolderInfo);
				});
				if (!CS$<>8__locals1.resultFound || CS$<>8__locals1.calendarLoggingFolderInfo.AccessCount > this.maxCachedFolderSizeAccessCount)
				{
					using (Folder folder = Folder.Bind(CS$<>8__locals1.session, DefaultFolderType.CalendarLogging, new PropertyDefinition[]
					{
						FolderSchema.ExtendedSize
					}))
					{
						object obj = folder.TryGetProperty(FolderSchema.ExtendedSize);
						if (obj is PropertyError)
						{
							ExTraceGlobals.SessionTracer.TraceError<COWSettings.COWInfoCache, PropertyError>((long)this.GetHashCode(), "{0}: We could not get size of the calendar logging folder due to PropertyError {1}. Skipping it.", this, (PropertyError)obj);
							return null;
						}
						CS$<>8__locals1.calendarLoggingFolderInfo = new COWSettings.CalendarLoggingFolderInfo
						{
							Size = (ulong)((long)obj),
							AccessCount = 0U
						};
						goto IL_10B;
					}
				}
				COWSettings.COWInfoCache.<>c__DisplayClassf CS$<>8__locals2 = CS$<>8__locals1;
				CS$<>8__locals2.calendarLoggingFolderInfo.AccessCount = CS$<>8__locals2.calendarLoggingFolderInfo.AccessCount + 1U;
				IL_10B:
				this.TimedOperation(true, delegate
				{
					CS$<>8__locals1.<>4__this.calendarLoggingFolderInfoCache[CS$<>8__locals1.session.MailboxGuid] = CS$<>8__locals1.calendarLoggingFolderInfo;
				});
				return new ulong?(CS$<>8__locals1.calendarLoggingFolderInfo.Size);
			}

			private void ComputeNextCleanupCacheTime()
			{
				this.timeNextCacheCleanup = ExDateTime.Now + this.cowCacheLifeTime;
			}

			private void CleanupCacheTimeBased()
			{
				ExDateTime cleanupLimit = ExDateTime.Now;
				int initialCacheSize = 0;
				int finalCacheSize = 0;
				Dictionary<Guid, COWSettings.COWMailboxInfo> cleanCowMailboxInfoCache = new Dictionary<Guid, COWSettings.COWMailboxInfo>();
				Dictionary<ADObjectId, COWSettings.COWDatabaseInfo> cleanCowDatabaseInfoCache = new Dictionary<ADObjectId, COWSettings.COWDatabaseInfo>();
				Dictionary<OrganizationId, COWSettings.COWTenantInfo> cleanCowTenantInfoCache = new Dictionary<OrganizationId, COWSettings.COWTenantInfo>();
				Dictionary<Guid, COWSettings.CalendarLoggingFolderInfo> cleanCalendarLoggingFolderInfoCache = new Dictionary<Guid, COWSettings.CalendarLoggingFolderInfo>();
				bool cleanupDone = false;
				this.TimedOperation(false, delegate
				{
					initialCacheSize = this.cowMailboxInfoCache.Count + this.cowDatabaseInfoCache.Count + this.cowTenantInfoCache.Count;
					cleanCowMailboxInfoCache = this.GetNonExpiredCacheEntries<Guid, COWSettings.COWMailboxInfo>(this.cowMailboxInfoCache, cleanupLimit);
					cleanCowDatabaseInfoCache = this.GetNonExpiredCacheEntries<ADObjectId, COWSettings.COWDatabaseInfo>(this.cowDatabaseInfoCache, cleanupLimit);
					cleanCowTenantInfoCache = this.GetNonExpiredCacheEntries<OrganizationId, COWSettings.COWTenantInfo>(this.cowTenantInfoCache, cleanupLimit);
					foreach (KeyValuePair<Guid, COWSettings.CalendarLoggingFolderInfo> keyValuePair in this.calendarLoggingFolderInfoCache)
					{
						if (keyValuePair.Value.AccessCount < this.maxCachedFolderSizeAccessCount)
						{
							cleanCalendarLoggingFolderInfoCache[keyValuePair.Key] = keyValuePair.Value;
						}
					}
				});
				this.TimedOperation(false, delegate
				{
					cleanupDone = (this.timeNextCacheCleanup > ExDateTime.Now);
				});
				if (!cleanupDone)
				{
					this.TimedOperation(true, delegate
					{
						if (this.timeNextCacheCleanup < ExDateTime.Now)
						{
							initialCacheSize = this.cowMailboxInfoCache.Count + this.cowDatabaseInfoCache.Count + this.cowTenantInfoCache.Count;
							this.cowMailboxInfoCache = cleanCowMailboxInfoCache;
							this.cowDatabaseInfoCache = cleanCowDatabaseInfoCache;
							this.cowTenantInfoCache = cleanCowTenantInfoCache;
							this.calendarLoggingFolderInfoCache = cleanCalendarLoggingFolderInfoCache;
							finalCacheSize = cleanCowMailboxInfoCache.Count + cleanCowDatabaseInfoCache.Count + cleanCowTenantInfoCache.Count;
							this.perfCounters.DumpsterADSettingCacheSize.RawValue = (long)finalCacheSize;
							this.ComputeNextCleanupCacheTime();
							cleanupDone = true;
						}
					});
					if (cleanupDone)
					{
						ExTraceGlobals.SessionTracer.TraceWarning<int, ExDateTime, ExDateTime>(0L, "CleanupCacheTimeBased removed {0} entries, start time {1}, end time {2}.", finalCacheSize - initialCacheSize, cleanupLimit, ExDateTime.Now);
					}
				}
				if (!cleanupDone)
				{
					ExTraceGlobals.SessionTracer.TraceWarning(0L, "CleanupCacheTimeBased useless cleanup: other thread did it.");
				}
			}

			private Dictionary<TKey, TValue> GetNonExpiredCacheEntries<TKey, TValue>(Dictionary<TKey, TValue> cache, ExDateTime expirationTime) where TValue : COWSettings.IExpirableCacheEntry
			{
				Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>(cache.Count);
				foreach (KeyValuePair<TKey, TValue> keyValuePair in cache)
				{
					TValue value = keyValuePair.Value;
					if (value.InfoExpireTime >= expirationTime)
					{
						dictionary[keyValuePair.Key] = keyValuePair.Value;
					}
				}
				return dictionary;
			}

			private COWSettings.COWDatabaseInfo GetInterestingDatabaseQuotas(ADObjectId databaseId)
			{
				Util.ThrowOnNullArgument(databaseId, "databaseId");
				bool resultFound = false;
				COWSettings.COWDatabaseInfo databaseInfo = default(COWSettings.COWDatabaseInfo);
				this.perfCounters.DumpsterADSettingCacheMisses_Base.Increment();
				this.TimedOperation(false, delegate
				{
					resultFound = this.cowDatabaseInfoCache.TryGetValue(databaseId, out databaseInfo);
				});
				if (!resultFound || databaseInfo.InfoExpireTime < ExDateTime.Now)
				{
					if (!resultFound)
					{
						this.perfCounters.DumpsterADSettingCacheSize.Increment();
					}
					this.perfCounters.DumpsterADSettingCacheMisses.Increment();
					ExDateTime infoExpireTime;
					ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
					{
						ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 2086, "GetInterestingDatabaseQuotas", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\CopyOnWrite\\COWSettings.cs");
						MailboxDatabase mailboxDatabase = topologyConfigurationSession.Read<MailboxDatabase>(databaseId);
						if (mailboxDatabase != null)
						{
							infoExpireTime = ExDateTime.Now + this.cowCacheLifeTime;
							databaseInfo = new COWSettings.COWDatabaseInfo
							{
								DumpsterWarningQuota = mailboxDatabase.RecoverableItemsWarningQuota,
								CalendarLoggingQuota = mailboxDatabase.CalendarLoggingQuota,
								DumpsterQuota = mailboxDatabase.RecoverableItemsQuota,
								InfoExpireTime = infoExpireTime
							};
						}
					});
					if (!adoperationResult.Succeeded)
					{
						LocalizedException ex = adoperationResult.Exception as LocalizedException;
						if (ex != null)
						{
							throw StorageGlobals.TranslateDirectoryException(ServerStrings.ADException, ex, null, "GetInterestingDatabaseQuotas failed due to directory exception {0}.", new object[]
							{
								ex
							});
						}
						throw adoperationResult.Exception;
					}
					else
					{
						this.TimedOperation(true, delegate
						{
							this.cowDatabaseInfoCache[databaseId] = databaseInfo;
						});
					}
				}
				return databaseInfo;
			}

			private bool IsCalendarLoggingEnabledOnTenant(OrganizationId orgId)
			{
				Util.ThrowOnNullArgument(orgId, "orgId");
				bool resultFound = false;
				COWSettings.COWTenantInfo tenantInfo = default(COWSettings.COWTenantInfo);
				this.perfCounters.DumpsterADSettingCacheMisses_Base.Increment();
				this.TimedOperation(false, delegate
				{
					resultFound = this.cowTenantInfoCache.TryGetValue(orgId, out tenantInfo);
				});
				if (!resultFound || tenantInfo.InfoExpireTime < ExDateTime.Now)
				{
					if (!resultFound)
					{
						this.perfCounters.DumpsterADSettingCacheSize.Increment();
					}
					this.perfCounters.DumpsterADSettingCacheMisses.Increment();
					ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSessionSettings.SessionSettingsFactory.Default.GetRootOrgContainerId(orgId.PartitionId), orgId, null, false), 2166, "IsCalendarLoggingEnabledOnTenant", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\CopyOnWrite\\COWSettings.cs");
					ExDateTime infoExpireTime = ExDateTime.Now + this.cowCacheLifeTime;
					tenantInfo = new COWSettings.COWTenantInfo
					{
						CalendarVersionStoreEnabled = tenantConfigurationSession.GetOrgContainer().CalendarVersionStoreEnabled,
						InfoExpireTime = infoExpireTime
					};
					this.TimedOperation(true, delegate
					{
						this.cowTenantInfoCache[orgId] = tenantInfo;
					});
				}
				return tenantInfo.CalendarVersionStoreEnabled;
			}

			private COWSettings.COWMailboxInfo GetCOWMailboxInfoFromAD(StoreSession session)
			{
				ADUser user = null;
				bool litigationHold = false;
				bool calendarLogging = false;
				bool siteMailboxMessageDedup = false;
				bool singleItemRecovery = false;
				bool inPlaceHoldEnabled = false;
				bool? tenantCalendarLoggingEnabled = null;
				Unlimited<ByteQuantifiedSize> dumpsterWarningQuota = Unlimited<ByteQuantifiedSize>.UnlimitedValue;
				Unlimited<ByteQuantifiedSize> calendarLoggingQuota = Unlimited<ByteQuantifiedSize>.UnlimitedValue;
				Unlimited<ByteQuantifiedSize> unlimited = Unlimited<ByteQuantifiedSize>.UnlimitedValue;
				ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
				{
					IRecipientSession adrecipientSession = session.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid);
					ADRecipient adrecipient;
					if (session.MailboxOwner.MailboxInfo.MailboxGuid == Guid.Empty && session is MailboxSession)
					{
						adrecipient = adrecipientSession.FindByLegacyExchangeDN(((MailboxSession)session).MailboxOwnerLegacyDN);
					}
					else
					{
						adrecipient = adrecipientSession.FindByExchangeGuidIncludingAlternate(session.MailboxOwner.MailboxInfo.MailboxGuid);
					}
					user = (adrecipient as ADUser);
					if (this.IsDatacenter() && adrecipientSession.SessionSettings.CurrentOrganizationId != OrganizationId.ForestWideOrgId)
					{
						tenantCalendarLoggingEnabled = new bool?(this.IsCalendarLoggingEnabledOnTenant(adrecipientSession.SessionSettings.CurrentOrganizationId));
					}
				});
				if (adoperationResult.Succeeded)
				{
					ExDateTime infoExpireTime = ExDateTime.Now + this.cowCacheLifeTime;
					if (user == null)
					{
						ExTraceGlobals.SessionTracer.TraceError((long)session.GetHashCode(), "User not found in AD.");
						litigationHold = false;
						singleItemRecovery = false;
						inPlaceHoldEnabled = false;
					}
					else
					{
						if (user.LitigationHoldEnabled)
						{
							litigationHold = true;
						}
						if (tenantCalendarLoggingEnabled != null && tenantCalendarLoggingEnabled.Value && !user.CalendarVersionStoreDisabled)
						{
							calendarLogging = true;
						}
						else if (!user.CalendarVersionStoreDisabled)
						{
							calendarLogging = true;
						}
						if (user.SiteMailboxMessageDedupEnabled)
						{
							siteMailboxMessageDedup = true;
						}
						if (user.SingleItemRecoveryEnabled)
						{
							singleItemRecovery = true;
						}
						if (user.UseDatabaseQuotaDefaults != null && !user.UseDatabaseQuotaDefaults.Value)
						{
							if (!user.RecoverableItemsQuota.IsUnlimited)
							{
								unlimited = ByteQuantifiedSize.FromBytes((ulong)(user.RecoverableItemsQuota.Value.ToBytes() * 0.2));
							}
							dumpsterWarningQuota = user.RecoverableItemsWarningQuota;
							calendarLoggingQuota = (user.CalendarLoggingQuota.IsUnlimited ? unlimited : user.CalendarLoggingQuota);
						}
						else
						{
							ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromADUser(user, RemotingOptions.AllowCrossSite | RemotingOptions.AllowCrossPremise);
							if (!exchangePrincipal.MailboxInfo.IsRemote)
							{
								COWSettings.COWDatabaseInfo interestingDatabaseQuotas = this.GetInterestingDatabaseQuotas(exchangePrincipal.MailboxInfo.MailboxDatabase);
								if (!interestingDatabaseQuotas.DumpsterQuota.IsUnlimited)
								{
									unlimited = ByteQuantifiedSize.FromBytes((ulong)(interestingDatabaseQuotas.DumpsterQuota.Value.ToBytes() * 0.2));
								}
								dumpsterWarningQuota = interestingDatabaseQuotas.DumpsterWarningQuota;
								calendarLoggingQuota = (interestingDatabaseQuotas.CalendarLoggingQuota.IsUnlimited ? unlimited : interestingDatabaseQuotas.CalendarLoggingQuota);
							}
						}
						if (user.InPlaceHolds != null && user.InPlaceHolds.Count > 0)
						{
							inPlaceHoldEnabled = true;
						}
					}
					this.perfCounters.DumpsterADSettingRefreshRate.Increment();
					return new COWSettings.COWMailboxInfo
					{
						LitigationHold = litigationHold,
						CalendarLogging = calendarLogging,
						SiteMailboxMessageDedup = siteMailboxMessageDedup,
						SingleItemRecovery = singleItemRecovery,
						InfoExpireTime = infoExpireTime,
						InPlaceHoldEnabled = inPlaceHoldEnabled,
						DumpsterWarningQuota = dumpsterWarningQuota,
						CalendarLoggingQuota = calendarLoggingQuota
					};
				}
				LocalizedException ex = adoperationResult.Exception as LocalizedException;
				if (ex != null)
				{
					throw StorageGlobals.TranslateDirectoryException(ServerStrings.ADException, ex, null, "GetDumpsterSessionInfoFromAD failed due to directory exception {0}.", new object[]
					{
						ex
					});
				}
				throw adoperationResult.Exception;
			}

			private bool IsDatacenter()
			{
				if (this.isDatacenter == null)
				{
					this.isDatacenter = new bool?(VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled);
				}
				return this.isDatacenter.Value;
			}

			private readonly MiddleTierStoragePerformanceCountersInstance perfCounters = DumpsterFolderHelper.GetPerfCounters();

			private readonly TimeSpan longWaitLimit = TimeSpan.FromMinutes(2.0);

			private readonly TimeSpan timeoutLimit = TimeSpan.FromMinutes(10.0);

			private readonly TimeSpan cowCacheLifeTime;

			private readonly uint maxCachedFolderSizeAccessCount = 50U;

			private int longWaitEventsTotal;

			private int timeoutEventsTotal;

			private ReaderWriterLockSlim cowCacheAccessLock;

			private ExDateTime timeNextCacheCleanup;

			private Dictionary<Guid, COWSettings.COWMailboxInfo> cowMailboxInfoCache;

			private Dictionary<ADObjectId, COWSettings.COWDatabaseInfo> cowDatabaseInfoCache;

			private Dictionary<OrganizationId, COWSettings.COWTenantInfo> cowTenantInfoCache;

			private Dictionary<Guid, COWSettings.CalendarLoggingFolderInfo> calendarLoggingFolderInfoCache;

			private bool? isDatacenter = null;

			private delegate void LockedMethod();
		}
	}
}
