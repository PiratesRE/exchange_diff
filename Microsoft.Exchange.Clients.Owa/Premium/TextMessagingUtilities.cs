using System;
using System.Collections;
using System.Configuration;
using System.Web.Configuration;
using Microsoft.Exchange.AirSync;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	internal static class TextMessagingUtilities
	{
		public static bool IsSmsSyncEnabled(UserContext userContext, out E164Number smsSyncPhoneNumber, out string smsSyncDeviceProtocol, out string smsSyncDeviceType, out string smsSyncDeviceId, out string smsSyncDeviceFriendlyName)
		{
			smsSyncPhoneNumber = null;
			smsSyncDeviceProtocol = null;
			smsSyncDeviceType = null;
			smsSyncDeviceId = null;
			smsSyncDeviceFriendlyName = null;
			bool result;
			using (VersionedXmlDataProvider versionedXmlDataProvider = new VersionedXmlDataProvider(userContext.MailboxSession))
			{
				TextMessagingAccount textMessagingAccount = (TextMessagingAccount)versionedXmlDataProvider.Read<TextMessagingAccount>(userContext.ExchangePrincipal.ObjectId);
				if (textMessagingAccount.EasEnabled && null != textMessagingAccount.EasPhoneNumber)
				{
					smsSyncPhoneNumber = textMessagingAccount.EasPhoneNumber;
					smsSyncDeviceProtocol = textMessagingAccount.EasDeviceProtocol;
					smsSyncDeviceType = textMessagingAccount.EasDeviceType;
					smsSyncDeviceId = textMessagingAccount.EasDeviceId;
					smsSyncDeviceFriendlyName = textMessagingAccount.EasDeviceName;
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		private static void GetSmsSyncDeviceInactiveDetectionCheckPoints(out TimeSpan stuckDetectionNoEarlierThan, out TimeSpan stuckDetectionNoLaterThan, out TimeSpan inactiveDetectionNoEarlierThan)
		{
			if (!TextMessagingUtilities.testOverrideHasBeenRead)
			{
				System.Configuration.Configuration configuration = WebConfigurationManager.OpenWebConfiguration("~");
				if (0 < configuration.AppSettings.Settings.Count)
				{
					KeyValueConfigurationElement keyValueConfigurationElement = configuration.AppSettings.Settings["SmsStuckNoEarlierThanHowLongBefore"];
					KeyValueConfigurationElement keyValueConfigurationElement2 = configuration.AppSettings.Settings["SmsStuckNoLaterThanHowLongBefore"];
					TimeSpan zero = TimeSpan.Zero;
					TimeSpan zero2 = TimeSpan.Zero;
					if (keyValueConfigurationElement != null && keyValueConfigurationElement2 != null && TimeSpan.TryParse(keyValueConfigurationElement.Value, out zero) && TimeSpan.TryParse(keyValueConfigurationElement2.Value, out zero2) && zero > zero2 && TextMessagingUtilities.DefaultStuckNoEarlierThanHowLongBefore >= zero && TimeSpan.Zero < zero2)
					{
						TextMessagingUtilities.stuckNoEarlierThanHowLongBefore = zero;
						TextMessagingUtilities.stuckNoLaterThanHowLongBefore = zero2;
					}
					KeyValueConfigurationElement keyValueConfigurationElement3 = configuration.AppSettings.Settings["SmsInactiveNoEarlierThanHowLongBefore"];
					TimeSpan zero3 = TimeSpan.Zero;
					if (keyValueConfigurationElement3 != null && TimeSpan.TryParse(keyValueConfigurationElement3.Value, out zero3) && TimeSpan.Zero < zero3 && TextMessagingUtilities.DefaultInactiveNoEarlierThanHowLongBefore >= zero3)
					{
						TextMessagingUtilities.inactiveNoEarlierThanHowLongBefore = zero3;
					}
				}
				TextMessagingUtilities.testOverrideHasBeenRead = true;
			}
			stuckDetectionNoEarlierThan = TextMessagingUtilities.stuckNoEarlierThanHowLongBefore;
			stuckDetectionNoLaterThan = TextMessagingUtilities.stuckNoLaterThanHowLongBefore;
			inactiveDetectionNoEarlierThan = TextMessagingUtilities.inactiveNoEarlierThanHowLongBefore;
		}

		public static bool IsSmsSyncDeviceInactive(UserContext userContext, E164Number smsSyncPhoneNumber, string smsSyncDeviceProtocol, string smsSyncDeviceType, string smsSyncDeviceId, out string deviceName)
		{
			deviceName = null;
			TimeSpan zero = TimeSpan.Zero;
			TimeSpan zero2 = TimeSpan.Zero;
			TimeSpan zero3 = TimeSpan.Zero;
			TextMessagingUtilities.GetSmsSyncDeviceInactiveDetectionCheckPoints(out zero, out zero2, out zero3);
			ExDateTime utcNow = ExDateTime.UtcNow;
			ExDateTime oldestBoundary = utcNow - zero;
			ExDateTime newestBoundary = utcNow - zero2;
			ExDateTime boundary = utcNow - zero3;
			return TextMessagingUtilities.IsTextMessageStuckInOutbox(userContext.MailboxSession, smsSyncPhoneNumber, oldestBoundary, newestBoundary) || TextMessagingUtilities.TryGetInactiveSmsSyncDeviceName(userContext.MailboxSession, smsSyncPhoneNumber, smsSyncDeviceProtocol, smsSyncDeviceType, smsSyncDeviceId, boundary, out deviceName);
		}

		public static bool NeedToAddUnsyncedMessageInfobar(string messageClass, IStorePropertyBag storePropertyBag, MailboxSession session)
		{
			int num = 0;
			return TextMessagingUtilities.TryGetDeliveryStatus(messageClass, storePropertyBag, session, out num) && 50 > num && 0 < num;
		}

		private static bool IsTextMessageStuckInOutbox(MailboxSession session, E164Number smsSyncPhoneNumber, ExDateTime oldestBoundary, ExDateTime newestBoundary)
		{
			bool result;
			using (Folder folder = Folder.Bind(session, DefaultFolderType.Outbox))
			{
				if (folder.ItemCount == 0)
				{
					result = false;
				}
				else
				{
					using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, new SortBy[]
					{
						new SortBy(StoreObjectSchema.LastModifiedTime, SortOrder.Ascending)
					}, new PropertyDefinition[]
					{
						StoreObjectSchema.LastModifiedTime,
						StoreObjectSchema.ItemClass,
						ItemSchema.SentRepresentingType,
						ItemSchema.SentRepresentingEmailAddress
					}))
					{
						if (!queryResult.SeekToCondition(SeekReference.OriginBeginning, new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, StoreObjectSchema.LastModifiedTime, oldestBoundary)))
						{
							result = false;
						}
						else
						{
							while (queryResult.SeekToCondition(SeekReference.OriginCurrent, new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, "IPM.Note.Mobile.SMS")))
							{
								IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(1);
								if (propertyBags == null || propertyBags.Length == 0)
								{
									return false;
								}
								string b = propertyBags[0].TryGetProperty(ItemSchema.SentRepresentingType) as string;
								if (string.Equals("MOBILE", b, StringComparison.OrdinalIgnoreCase))
								{
									string number = propertyBags[0].TryGetProperty(ItemSchema.SentRepresentingEmailAddress) as string;
									E164Number e164Number = null;
									if (E164Number.TryParse(number, out e164Number) && e164Number.Equals(smsSyncPhoneNumber, true))
									{
										ExDateTime? exDateTime = propertyBags[0].TryGetProperty(StoreObjectSchema.LastModifiedTime) as ExDateTime?;
										if (exDateTime != null)
										{
											return exDateTime <= newestBoundary;
										}
									}
								}
							}
							result = false;
						}
					}
				}
			}
			return result;
		}

		private static bool TryGetSmsSyncDeviceLastSyncTime(SyncStateStorage syncStateStorage, E164Number smsSyncPhoneNumber, out ExDateTime lastSyncTime, out string deviceName)
		{
			lastSyncTime = ExDateTime.MinValue;
			deviceName = null;
			using (CustomSyncState customSyncState = syncStateStorage.GetCustomSyncState(new GlobalSyncStateInfo
			{
				ReadOnly = true
			}, new PropertyDefinition[0]))
			{
				if (customSyncState == null)
				{
					return false;
				}
				if (!customSyncState.GetData<BooleanData, bool>("DeviceEnableOutboundSMS", false))
				{
					return false;
				}
				string data = customSyncState.GetData<StringData, string>("DevicePhoneNumber", null);
				if (string.IsNullOrEmpty(data))
				{
					return false;
				}
				E164Number e164Number = null;
				if (!E164Number.TryParse(data, out e164Number))
				{
					return false;
				}
				if (!e164Number.Equals(smsSyncPhoneNumber, true))
				{
					return false;
				}
				deviceName = customSyncState.GetData<StringData, string>("DeviceFriendlyName", null);
			}
			lastSyncTime = syncStateStorage.CreationTime;
			using (CustomSyncState customSyncState2 = syncStateStorage.GetCustomSyncState(new SyncStatusSyncStateInfo
			{
				ReadOnly = true
			}, new PropertyDefinition[0]))
			{
				if (customSyncState2 != null)
				{
					lastSyncTime = customSyncState2.GetData<DateTimeData, ExDateTime>("LastSyncAttemptTime", ExDateTime.MinValue);
					if (ExDateTime.MinValue == lastSyncTime)
					{
						lastSyncTime = customSyncState2.GetData<DateTimeData, ExDateTime>("LastSyncSuccessTime", syncStateStorage.CreationTime);
					}
				}
			}
			return true;
		}

		private static bool TryGetInactiveSmsSyncDeviceName(MailboxSession session, E164Number smsSyncPhoneNumber, string smsSyncDeviceProtocol, string smsSyncDeviceType, string smsSyncDeviceId, ExDateTime boundary, out string deviceName)
		{
			deviceName = null;
			ExDateTime minValue = ExDateTime.MinValue;
			if (string.IsNullOrEmpty(smsSyncDeviceProtocol) || string.IsNullOrEmpty(smsSyncDeviceType) || string.IsNullOrEmpty(smsSyncDeviceId))
			{
				IEnumerator enumerator = SyncStateStorage.GetEnumerator(session, null);
				using (enumerator as IDisposable)
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						using (SyncStateStorage syncStateStorage = (SyncStateStorage)obj)
						{
							if (syncStateStorage.DeviceIdentity.IsProtocol("AirSync"))
							{
								if (TextMessagingUtilities.TryGetSmsSyncDeviceLastSyncTime(syncStateStorage, smsSyncPhoneNumber, out minValue, out deviceName))
								{
									break;
								}
							}
						}
					}
					goto IL_BB;
				}
			}
			DeviceIdentity deviceIdentity = new DeviceIdentity(smsSyncDeviceId, smsSyncDeviceType, smsSyncDeviceProtocol);
			using (SyncStateStorage syncStateStorage2 = SyncStateStorage.Bind(session, deviceIdentity, null))
			{
				if (!TextMessagingUtilities.TryGetSmsSyncDeviceLastSyncTime(syncStateStorage2, smsSyncPhoneNumber, out minValue, out deviceName))
				{
					return true;
				}
			}
			IL_BB:
			return minValue <= boundary;
		}

		private static bool TryGetDeliveryStatus(string messageClass, IStorePropertyBag storePropertyBag, MailboxSession session, out int deliveryStatus)
		{
			deliveryStatus = 0;
			if (!ObjectClass.IsSmsMessage(messageClass) || !Utilities.IsItemInDefaultFolder(storePropertyBag, DefaultFolderType.SentItems, session))
			{
				return false;
			}
			deliveryStatus = ItemUtility.GetProperty<int>(storePropertyBag, MessageItemSchema.TextMessageDeliveryStatus, 0);
			return true;
		}

		public const string SmsSyncHelpURL = "http://go.microsoft.com/fwlink/?LinkId=186816";

		private static readonly TimeSpan DefaultStuckNoEarlierThanHowLongBefore = TimeSpan.FromDays(3.0);

		private static readonly TimeSpan DefaultStuckNoLaterThanHowLongBefore = TimeSpan.FromMinutes(20.0);

		private static readonly TimeSpan DefaultInactiveNoEarlierThanHowLongBefore = TimeSpan.FromMinutes(20.0);

		private static bool testOverrideHasBeenRead;

		private static TimeSpan stuckNoEarlierThanHowLongBefore = TextMessagingUtilities.DefaultStuckNoEarlierThanHowLongBefore;

		private static TimeSpan stuckNoLaterThanHowLongBefore = TextMessagingUtilities.DefaultStuckNoLaterThanHowLongBefore;

		private static TimeSpan inactiveNoEarlierThanHowLongBefore = TextMessagingUtilities.DefaultInactiveNoEarlierThanHowLongBefore;
	}
}
