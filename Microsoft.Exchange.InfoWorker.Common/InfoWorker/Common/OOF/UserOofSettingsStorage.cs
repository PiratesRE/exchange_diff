using System;
using System.IO;
using System.Xml;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Availability;

namespace Microsoft.Exchange.InfoWorker.Common.OOF
{
	internal static class UserOofSettingsStorage
	{
		public static UserOofSettings LoadUserOofSettings(MailboxSession itemStore)
		{
			UserOofSettings userOofSettings = null;
			string text = null;
			string text2 = null;
			try
			{
				text2 = UserOofSettingsStorage.LoadUserOofSettingsByItemId(itemStore);
				if (string.IsNullOrEmpty(text2))
				{
					text2 = UserOofSettingsStorage.settingsStorageHandler.GetItemContent(itemStore);
				}
			}
			catch (ObjectNotFoundException arg)
			{
				UserOofSettingsStorage.Tracer.TraceError<IExchangePrincipal, ObjectNotFoundException>(0L, "Mailbox:{0}: Exception on loading useroofsettings, exception = {1}, re-creating default useroofsettings.", itemStore.MailboxOwner, arg);
			}
			catch (VirusException arg2)
			{
				UserOofSettingsStorage.Tracer.TraceError<IExchangePrincipal, VirusException>(0L, "Mailbox:{0}: Exception on loading useroofsettings, exception = {1}, re-creating default useroofsettings.", itemStore.MailboxOwner, arg2);
				text = Strings.descOOFVirusDetectedOofReplyMessage;
			}
			if (string.IsNullOrEmpty(text2))
			{
				userOofSettings = UserOofSettingsStorage.CreateDefaultUserOofSettings(itemStore);
				if (text != null)
				{
					userOofSettings.InternalReply.RawMessage = text;
				}
			}
			else
			{
				using (StringReader stringReader = new StringReader(text2))
				{
					using (XmlReader xmlReader = XmlReader.Create(stringReader, new XmlReaderSettings
					{
						CheckCharacters = false
					}))
					{
						try
						{
							UserOofSettingsSerializer userOofSettingsSerializer = (UserOofSettingsSerializer)UserOofSettingsSerializer.Serializer.Deserialize(xmlReader);
							userOofSettings = userOofSettingsSerializer.Deserialize();
							userOofSettings.InternalReply.SetByLegacyClient = userOofSettings.SetByLegacyClient;
							userOofSettings.ExternalReply.SetByLegacyClient = userOofSettings.SetByLegacyClient;
						}
						catch (InvalidOperationException)
						{
							UserOofSettingsStorage.Tracer.TraceError<IExchangePrincipal>(0L, "Mailbox:{0}: Cannot read user OOF settings, InvalidOperationException", itemStore.MailboxOwner);
							userOofSettings = UserOofSettingsStorage.CreateDefaultUserOofSettings(itemStore);
						}
						catch (InvalidUserOofSettingsXmlDocument)
						{
							userOofSettings = UserOofSettingsStorage.CreateDefaultUserOofSettings(itemStore);
						}
					}
				}
				UserOofSettingsStorage.Tracer.TraceDebug<IExchangePrincipal>(0L, "Mailbox:{0}: Loaded existing OOF settings", itemStore.MailboxOwner);
			}
			if (userOofSettings.OofState == OofState.Scheduled && userOofSettings.EndTime < DateTime.UtcNow)
			{
				userOofSettings.OofState = OofState.Disabled;
			}
			if (userOofSettings.OofState != OofState.Scheduled)
			{
				DateTime utcNow = DateTime.UtcNow;
				DateTime dateTime = utcNow.AddDays(1.0);
				Duration duration = new Duration(DateTime.SpecifyKind(new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, utcNow.Hour, 0, 0), DateTimeKind.Utc), DateTime.SpecifyKind(new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0), DateTimeKind.Utc));
				userOofSettings.Duration = duration;
			}
			return userOofSettings;
		}

		public static void SaveUserOofSettings(MailboxSession itemStore, UserOofSettings userOofSettings)
		{
			UserOofSettingsSerializer o = UserOofSettingsSerializer.Serialize(userOofSettings);
			using (StringWriter stringWriter = new StringWriter())
			{
				using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
				{
					CheckCharacters = false
				}))
				{
					UserOofSettingsSerializer.Serializer.Serialize(xmlWriter, o);
					string body = stringWriter.ToString();
					VersionedId versionedId = UserOofSettingsStorage.settingsStorageHandler.SetItemContent(itemStore, body);
					itemStore.Mailbox[MailboxSchema.UserOofSettingsItemId] = versionedId.ObjectId.ProviderLevelItemId;
				}
			}
			UserOofSettingsStorage.Tracer.TraceDebug<IExchangePrincipal>(0L, "Mailbox:{0}: Adding OOF rules", itemStore.MailboxOwner);
			using (RuleGenerator ruleGenerator = new RuleGenerator(itemStore, userOofSettings))
			{
				ruleGenerator.OnUserOofSettingsChanges();
			}
			UserOofSettingsStorage.Tracer.TraceDebug<IExchangePrincipal>(0L, "Mailbox:{0}: OOF rules added, setting OOF mailbox table properties.", itemStore.MailboxOwner);
			OofStateHandler.Set(itemStore, userOofSettings.OofState == OofState.Enabled, (userOofSettings.UserChangeTime != null) ? userOofSettings.UserChangeTime.Value : DateTime.UtcNow);
			itemStore.Mailbox[MailboxSchema.OofScheduleStart] = ((userOofSettings.OofState == OofState.Scheduled) ? ((ExDateTime)userOofSettings.StartTime) : UserOofSettingsStorage.Date1601Utc);
			itemStore.Mailbox[MailboxSchema.OofScheduleEnd] = ((userOofSettings.OofState == OofState.Scheduled) ? ((ExDateTime)userOofSettings.EndTime) : UserOofSettingsStorage.Date1601Utc);
			itemStore.Mailbox.Save();
			itemStore.Mailbox.Load();
		}

		private static string LoadUserOofSettingsByItemId(MailboxSession itemStore)
		{
			string result = null;
			try
			{
				object obj = itemStore.Mailbox.TryGetProperty(MailboxSchema.UserOofSettingsItemId);
				if (!(obj is PropertyError) && obj != null)
				{
					StoreObjectId itemId = StoreObjectId.FromProviderSpecificId((byte[])obj, StoreObjectType.Message);
					result = UserOofSettingsStorage.settingsStorageHandler.GetItemContent(itemStore, itemId);
				}
			}
			catch (ArgumentOutOfRangeException arg)
			{
				UserOofSettingsStorage.Tracer.TraceError<IExchangePrincipal, ArgumentOutOfRangeException>(0L, "Mailbox:{0}: Exception on converting OOF settings item id to StoreObjectId, exception = {1}, loading OOF settings item by folder query.", itemStore.MailboxOwner, arg);
			}
			catch (CorruptDataException arg2)
			{
				UserOofSettingsStorage.Tracer.TraceError<IExchangePrincipal, CorruptDataException>(0L, "Mailbox:{0}: Exception on converting OOF settings item id to StoreObjectId, exception = {1}, loading OOF settings item by folder query.", itemStore.MailboxOwner, arg2);
			}
			return result;
		}

		private static UserOofSettings CreateDefaultUserOofSettings(MailboxSession itemStore)
		{
			UserOofSettings userOofSettings = UserOofSettings.CreateDefault();
			userOofSettings.ExternalAudience = ExternalAudience.All;
			if (OofStateHandler.Get(itemStore))
			{
				userOofSettings.OofState = OofState.Enabled;
			}
			else
			{
				userOofSettings.OofState = OofState.Disabled;
			}
			UserOofSettingsStorage.Tracer.TraceDebug<IExchangePrincipal>(0L, "Mailbox:{0}: Created default OOF settings, saving to mailbox", itemStore.MailboxOwner);
			UserOofSettingsStorage.TracerPfd.TracePfd<int, IExchangePrincipal>(0L, "PFD IWO {0} Mailbox:{1}: Created default OOF settings, saving to mailbox", 17559, itemStore.MailboxOwner);
			try
			{
				UserOofSettingsStorage.SaveUserOofSettings(itemStore, userOofSettings);
			}
			catch (VirusException arg)
			{
				UserOofSettingsStorage.settingsStorageHandler.Delete(itemStore);
				UserOofSettingsStorage.Tracer.TraceError<IExchangePrincipal, VirusException>(0L, "Mailbox:{0}: Exception on create default useroofsettings, exception = {1}, re-creating default useroofsettings.", itemStore.MailboxOwner, arg);
				userOofSettings.InternalReply.RawMessage = Strings.descOOFVirusDetectedOofReplyMessage;
				userOofSettings.ExternalReply.RawMessage = null;
				userOofSettings.OofState = OofState.Disabled;
				UserOofSettingsStorage.SaveUserOofSettings(itemStore, userOofSettings);
			}
			return userOofSettings;
		}

		internal static ExternalAudience GetUserPolicy(IExchangePrincipal mailboxOwner)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(mailboxOwner.MailboxInfo.OrganizationId ?? OrganizationId.ForestWideOrgId);
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 322, "GetUserPolicy", "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\OOF\\UserOofSettingsStorage.cs");
			ADRecipient adrecipient = tenantOrRootOrgRecipientSession.Read(mailboxOwner.ObjectId);
			IADMailStorage iadmailStorage = adrecipient as IADMailStorage;
			if (iadmailStorage == null)
			{
				throw new MailStorageNotFoundException();
			}
			if (iadmailStorage.ExternalOofOptions == ExternalOofOptions.InternalOnly)
			{
				return ExternalAudience.None;
			}
			return ExternalAudience.All;
		}

		private static readonly SingleInstanceItemHandler settingsStorageHandler = new SingleInstanceItemHandler("IPM.Microsoft.OOF.UserOofSettings", DefaultFolderType.Configuration);

		private static readonly Trace Tracer = ExTraceGlobals.OOFTracer;

		private static readonly Trace TracerPfd = ExTraceGlobals.PFDTracer;

		private static readonly ExDateTime Date1601Utc = new ExDateTime(ExTimeZone.UtcTimeZone, 1601, 1, 1, 0, 0, 0);
	}
}
