using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	public sealed class DeviceInfo
	{
		internal DeviceInfo()
		{
		}

		internal DeviceIdentity DeviceIdentity { get; private set; }

		internal string DeviceModel
		{
			get
			{
				return this.deviceModel;
			}
		}

		internal string DeviceImei
		{
			get
			{
				return this.deviceImei;
			}
		}

		internal string DeviceFriendlyName
		{
			get
			{
				return this.deviceFriendlyName;
			}
		}

		internal string DeviceOS
		{
			get
			{
				return this.deviceOS;
			}
		}

		internal string DeviceOSLanguage
		{
			get
			{
				return this.deviceOSLanguage;
			}
		}

		internal string DevicePhoneNumber
		{
			get
			{
				return this.devicePhoneNumber;
			}
		}

		internal bool DeviceEnableOutboundSMS
		{
			get
			{
				return this.deviceEnableOutboundSMS;
			}
		}

		internal string DeviceMobileOperator
		{
			get
			{
				return this.deviceMobileOperator;
			}
		}

		internal ExDateTime? FirstSyncTime
		{
			get
			{
				return this.firstSyncTime;
			}
		}

		internal ExDateTime? LastSyncAttemptTime
		{
			get
			{
				return this.lastSyncAttemptTime;
			}
		}

		internal ExDateTime? LastSyncSuccessTime
		{
			get
			{
				return this.lastSyncSuccessTime;
			}
		}

		internal string UserAgent
		{
			get
			{
				return this.userAgent;
			}
		}

		internal ExDateTime? WipeRequestTime
		{
			get
			{
				return this.wipeRequestTime;
			}
		}

		internal ExDateTime? WipeSentTime
		{
			get
			{
				return this.wipeSentTime;
			}
		}

		internal ExDateTime? WipeAckTime
		{
			get
			{
				return this.wipeAckTime;
			}
		}

		internal string[] RemoteWipeConfirmationAddresses
		{
			get
			{
				return this.remoteWipeConfirmationAddresses;
			}
		}

		internal ExDateTime? LastPolicyUpdateTime
		{
			get
			{
				return this.lastPolicyUpdateTime;
			}
		}

		internal uint? LastPingHeartbeat
		{
			get
			{
				return this.lastPingHeartbeat;
			}
		}

		internal string RecoveryPassword
		{
			get
			{
				return this.recoveryPassword;
			}
		}

		internal bool IsRemoteWipeSupported
		{
			get
			{
				return this.remoteWipeSupported;
			}
		}

		internal DeviceAccessState DeviceAccessState { get; private set; }

		internal DeviceAccessStateReason DeviceAccessStateReason { get; private set; }

		internal ExDateTime? SSUpgradeDateTime { get; private set; }

		internal bool HaveSentBoostrapMailForWM61 { get; private set; }

		internal ADObjectId DevicePolicyApplied { get; private set; }

		internal DevicePolicyApplicationStatus DevicePolicyApplicationStatus { get; private set; }

		internal ADObjectId DeviceAccessControlRule { get; private set; }

		internal string LastDeviceWipeRequestor { get; private set; }

		internal string ClientVersion { get; private set; }

		internal int NumberOfFoldersSynced { get; private set; }

		internal ADObjectId DeviceADObjectId { get; private set; }

		internal ADObjectId UserADObjectId { get; private set; }

		internal static DeviceInfo GetDeviceInfo(MailboxSession mailboxSession, DeviceIdentity deviceIdentity)
		{
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			ArgumentValidator.ThrowIfNull("deviceIdentity", deviceIdentity);
			if (deviceIdentity.IsDnMangled)
			{
				AirSyncDiagnostics.TraceDebug<DeviceIdentity>(ExTraceGlobals.RequestsTracer, null, "[DeviceInfo.RemoveDeviceFromMailbox] Device Id was mangled due to naming conflicts.  Cannot remove device since we cannot generate the composite sync state name.  Device identity: '{0}'", deviceIdentity);
				return null;
			}
			DeviceInfo result;
			using (SyncStateStorage syncStateStorage = SyncStateStorage.Bind(mailboxSession, deviceIdentity, null))
			{
				result = ((syncStateStorage == null) ? null : DeviceInfo.GetDeviceInfo(syncStateStorage));
			}
			return result;
		}

		internal static bool CleanUpMobileDevice(MailboxSession mailboxSession, DeviceIdentity deviceIdentity, TimeSpan inactivityPeriod)
		{
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			ArgumentValidator.ThrowIfNull("deviceIdentity", deviceIdentity);
			bool result;
			using (SyncStateStorage syncStateStorage = SyncStateStorage.Bind(mailboxSession, deviceIdentity, null))
			{
				if (syncStateStorage == null)
				{
					result = true;
				}
				else
				{
					DeviceInfo deviceInfo = DeviceInfo.GetDeviceInfo(syncStateStorage);
					if (deviceInfo == null)
					{
						result = true;
					}
					else
					{
						ExDateTime? exDateTime = (deviceInfo.LastSyncAttemptTime != null) ? deviceInfo.LastSyncAttemptTime : deviceInfo.LastSyncSuccessTime;
						if (exDateTime == null || ExDateTime.UtcNow.Subtract(exDateTime.Value) > inactivityPeriod)
						{
							AirSyncDiagnostics.TraceDebug<DeviceIdentity>(ExTraceGlobals.RequestsTracer, null, "Cleaning up device with device identity: '{0}'", deviceInfo.DeviceIdentity);
							DeviceInfo.RemoveDeviceFromMailbox(mailboxSession, syncStateStorage, deviceIdentity);
							result = true;
						}
						else
						{
							result = false;
						}
					}
				}
			}
			return result;
		}

		internal static DeviceInfo[] GetAllDeviceInfo(MailboxSession mailboxSession, bool loadPreE14SyncStates = false)
		{
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			List<DeviceInfo> list = new List<DeviceInfo>(4);
			IEnumerator enumerator = SyncStateStorage.GetEnumerator(mailboxSession, null);
			using (enumerator as IDisposable)
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					SyncStateStorage syncStateStorage = (SyncStateStorage)obj;
					if (string.Equals(syncStateStorage.DeviceIdentity.Protocol, "AirSync", StringComparison.OrdinalIgnoreCase) || string.Equals(syncStateStorage.DeviceIdentity.Protocol, "MOWA", StringComparison.OrdinalIgnoreCase))
					{
						SyncStatusSyncStateInfo syncStateInfo = new SyncStatusSyncStateInfo();
						using (CustomSyncState customSyncState = syncStateStorage.GetCustomSyncState(syncStateInfo, new PropertyDefinition[0]))
						{
							bool flag = SyncStatusSyncStateInfo.IsPreE14SyncState(customSyncState);
							AirSyncDiagnostics.TraceDebug<bool, DeviceIdentity>(ExTraceGlobals.RequestsTracer, null, "DeviceInfo::GetAllDeviceInfo- isPreE14SyncState: {0}, device identity: {1}", flag, syncStateStorage.DeviceIdentity);
							if (!flag || loadPreE14SyncStates)
							{
								list.Add(DeviceInfo.GetDeviceInfo(syncStateStorage, customSyncState, false));
							}
						}
					}
				}
			}
			if (list.Count == 0)
			{
				return null;
			}
			return list.ToArray();
		}

		internal static DeviceInfo[] GetAllDeviceInfo(MailboxSession mailboxSession, MobileClientType mobileClientType)
		{
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			List<DeviceInfo> deviceInfos = new List<DeviceInfo>(4);
			IEnumerator enumerator = SyncStateStorage.GetEnumerator(mailboxSession, null);
			using (enumerator as IDisposable)
			{
				while (enumerator.MoveNext())
				{
					SyncStateStorage syncStateStorage = (SyncStateStorage)enumerator.Current;
					if ((syncStateStorage.DeviceIdentity.IsProtocol("AirSync") && (mobileClientType & MobileClientType.EAS) == MobileClientType.EAS) || (syncStateStorage.DeviceIdentity.IsProtocol("MOWA") && (mobileClientType & MobileClientType.MOWA) == MobileClientType.MOWA))
					{
						DeviceInfo.CorruptionSafeAction("GetAllDeviceInfo(MailboxSession)", delegate
						{
							SyncStatusSyncStateInfo syncStateInfo = new SyncStatusSyncStateInfo();
							using (CustomSyncState customSyncState = syncStateStorage.GetCustomSyncState(syncStateInfo, new PropertyDefinition[0]))
							{
								if (!SyncStatusSyncStateInfo.IsPreE14SyncState(customSyncState))
								{
									DeviceInfo deviceInfo = DeviceInfo.GetDeviceInfo(syncStateStorage, customSyncState, false);
									if (deviceInfo != null)
									{
										deviceInfos.Add(deviceInfo);
									}
								}
							}
						});
					}
					else
					{
						AirSyncDiagnostics.TraceDebug<DeviceIdentity>(ExTraceGlobals.ValidationTracer, null, "[DeviceInfo.GetAllDeviceInfo] Ignoring device '{0}' because it is not an AirSync device.", syncStateStorage.DeviceIdentity);
					}
				}
			}
			if (deviceInfos.Count != 0)
			{
				return deviceInfos.ToArray();
			}
			return null;
		}

		internal static DeviceInfo GetDeviceInfo(SyncStateStorage syncStateStorage)
		{
			return DeviceInfo.GetDeviceInfo(syncStateStorage, null, true);
		}

		internal static DeviceInfo GetDeviceInfo(SyncStateStorage syncStateStorage, CustomSyncState syncStatusSyncState, bool shouldDisposeSyncState)
		{
			DeviceInfo currentInfo = new DeviceInfo();
			currentInfo.firstSyncTime = new ExDateTime?(syncStateStorage.CreationTime);
			currentInfo.DeviceIdentity = syncStateStorage.DeviceIdentity;
			if (!DeviceInfo.CorruptionSafeAction("GetDeviceInfo", delegate
			{
				SyncStatusSyncStateInfo syncStatusSyncStateInfo = new SyncStatusSyncStateInfo();
				syncStatusSyncStateInfo.ReadOnly = true;
				if (syncStatusSyncState == null)
				{
					syncStatusSyncState = syncStateStorage.GetCustomSyncState(syncStatusSyncStateInfo, new PropertyDefinition[0]);
				}
				if (syncStatusSyncState != null)
				{
					currentInfo.userAgent = DeviceInfo.GetObjectProperty<ConstStringData, string>(syncStatusSyncState, CustomStateDatumType.UserAgent, null);
					currentInfo.lastSyncAttemptTime = DeviceInfo.GetValueProperty<DateTimeData, ExDateTime>(syncStatusSyncState, CustomStateDatumType.LastSyncAttemptTime, null);
					currentInfo.lastSyncSuccessTime = DeviceInfo.GetValueProperty<DateTimeData, ExDateTime>(syncStatusSyncState, CustomStateDatumType.LastSyncSuccessTime, null);
					if (shouldDisposeSyncState)
					{
						syncStatusSyncState.Dispose();
					}
				}
				using (AutdStatusData autdStatusData = AutdStatusData.Load(syncStateStorage, true, false))
				{
					if (autdStatusData != null)
					{
						currentInfo.lastPingHeartbeat = ((autdStatusData.LastPingHeartbeat != null) ? new uint?((uint)autdStatusData.LastPingHeartbeat.Value) : null);
					}
				}
				GlobalSyncStateInfo globalSyncStateInfo = new GlobalSyncStateInfo();
				globalSyncStateInfo.ReadOnly = true;
				using (CustomSyncState customSyncState = syncStateStorage.GetCustomSyncState(globalSyncStateInfo, new PropertyDefinition[0]))
				{
					if (customSyncState != null)
					{
						currentInfo.wipeRequestTime = DeviceInfo.GetValueProperty<DateTimeData, ExDateTime>(customSyncState, CustomStateDatumType.WipeRequestTime, null);
						currentInfo.wipeSentTime = DeviceInfo.GetValueProperty<DateTimeData, ExDateTime>(customSyncState, CustomStateDatumType.WipeSendTime, null);
						currentInfo.wipeAckTime = DeviceInfo.GetValueProperty<DateTimeData, ExDateTime>(customSyncState, CustomStateDatumType.WipeAckTime, null);
						currentInfo.remoteWipeConfirmationAddresses = DeviceInfo.GetObjectProperty<ArrayData<StringData, string>, string[]>(customSyncState, CustomStateDatumType.WipeConfirmationAddresses, null);
						currentInfo.lastPolicyUpdateTime = DeviceInfo.GetValueProperty<DateTimeData, ExDateTime>(customSyncState, CustomStateDatumType.LastPolicyTime, null);
						currentInfo.recoveryPassword = DeviceInfo.GetObjectProperty<StringData, string>(customSyncState, CustomStateDatumType.RecoveryPassword, null);
						currentInfo.deviceModel = DeviceInfo.GetObjectProperty<StringData, string>(customSyncState, CustomStateDatumType.DeviceModel, null);
						currentInfo.deviceImei = DeviceInfo.GetObjectProperty<StringData, string>(customSyncState, CustomStateDatumType.DeviceImei, null);
						currentInfo.deviceFriendlyName = DeviceInfo.GetObjectProperty<StringData, string>(customSyncState, CustomStateDatumType.DeviceFriendlyName, null);
						currentInfo.deviceOS = DeviceInfo.GetObjectProperty<StringData, string>(customSyncState, CustomStateDatumType.DeviceOS, null);
						currentInfo.deviceOSLanguage = DeviceInfo.GetObjectProperty<StringData, string>(customSyncState, CustomStateDatumType.DeviceOSLanguage, null);
						currentInfo.devicePhoneNumber = DeviceInfo.GetObjectProperty<StringData, string>(customSyncState, CustomStateDatumType.DevicePhoneNumber, null);
						string objectProperty = DeviceInfo.GetObjectProperty<StringData, string>(customSyncState, CustomStateDatumType.UserAgent, null);
						if (!string.IsNullOrEmpty(objectProperty))
						{
							currentInfo.userAgent = objectProperty;
						}
						currentInfo.deviceEnableOutboundSMS = DeviceInfo.GetValueProperty<BooleanData, bool>(customSyncState, CustomStateDatumType.DeviceEnableOutboundSMS, new bool?(false)).Value;
						currentInfo.deviceMobileOperator = DeviceInfo.GetObjectProperty<StringData, string>(customSyncState, CustomStateDatumType.DeviceMobileOperator, null);
						currentInfo.ClientVersion = DeviceInfo.GetObjectProperty<StringData, string>(customSyncState, CustomStateDatumType.DeviceActiveSyncVersion, null);
						double num = -1.0;
						if (!double.TryParse(currentInfo.ClientVersion, out num))
						{
							num = -1.0;
						}
						currentInfo.remoteWipeSupported = (num >= 14.0 || customSyncState.GetData<BooleanData, bool>(CustomStateDatumType.ProvisionSupported, false));
						currentInfo.SSUpgradeDateTime = DeviceInfo.GetValueProperty<DateTimeData, ExDateTime>(customSyncState, CustomStateDatumType.SSUpgradeDateTime, null);
						currentInfo.HaveSentBoostrapMailForWM61 = DeviceInfo.GetValueProperty<BooleanData, bool>(customSyncState, CustomStateDatumType.HaveSentBoostrapMailForWM61, new bool?(false)).Value;
						currentInfo.DeviceAccessState = (DeviceAccessState)DeviceInfo.GetValueProperty<Int32Data, int>(customSyncState, CustomStateDatumType.DeviceAccessState, new int?(0)).Value;
						currentInfo.DeviceAccessStateReason = (DeviceAccessStateReason)DeviceInfo.GetValueProperty<Int32Data, int>(customSyncState, CustomStateDatumType.DeviceAccessStateReason, new int?(0)).Value;
						currentInfo.DevicePolicyApplied = DeviceInfo.GetObjectProperty<ADObjectIdData, ADObjectId>(customSyncState, CustomStateDatumType.DevicePolicyApplied, null);
						currentInfo.DevicePolicyApplicationStatus = (DevicePolicyApplicationStatus)DeviceInfo.GetValueProperty<Int32Data, int>(customSyncState, CustomStateDatumType.DevicePolicyApplicationStatus, new int?(1)).Value;
						currentInfo.LastDeviceWipeRequestor = DeviceInfo.GetObjectProperty<StringData, string>(customSyncState, CustomStateDatumType.LastDeviceWipeRequestor, null);
						currentInfo.DeviceAccessControlRule = DeviceInfo.GetObjectProperty<ADObjectIdData, ADObjectId>(customSyncState, CustomStateDatumType.DeviceAccessControlRule, null);
						currentInfo.DeviceADObjectId = DeviceInfo.GetObjectProperty<ADObjectIdData, ADObjectId>(customSyncState, CustomStateDatumType.DeviceADObjectId, null);
						currentInfo.UserADObjectId = DeviceInfo.GetObjectProperty<ADObjectIdData, ADObjectId>(customSyncState, CustomStateDatumType.UserADObjectId, null);
						if (currentInfo.lastSyncAttemptTime != null)
						{
							ExDateTime value = currentInfo.lastSyncAttemptTime.Value;
						}
						else
						{
							currentInfo.lastSyncAttemptTime = DeviceInfo.GetValueProperty<DateTimeData, ExDateTime>(customSyncState, CustomStateDatumType.LastSyncAttemptTime, null);
							currentInfo.lastSyncSuccessTime = DeviceInfo.GetValueProperty<DateTimeData, ExDateTime>(customSyncState, CustomStateDatumType.LastSyncSuccessTime, null);
						}
					}
					currentInfo.NumberOfFoldersSynced = syncStateStorage.CountFolderSyncStates();
				}
			}))
			{
				return null;
			}
			return currentInfo;
		}

		internal static void SendMeMailboxLog(MailboxSession mailboxSession, DeviceIdentity deviceIdentity)
		{
			DeviceInfo deviceInfo = DeviceInfo.GetDeviceInfo(mailboxSession, deviceIdentity);
			if (deviceInfo != null)
			{
				List<string> list = new List<string>(1);
				list.Add(mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString());
				bool flag;
				DeviceInfo.SendMailboxLog(mailboxSession, mailboxSession.MailboxOwner.Alias, new DeviceInfo[]
				{
					deviceInfo
				}, list, out flag);
			}
		}

		internal static bool SendMailboxLog(MailboxSession mailboxSession, string userName, DeviceInfo[] deviceInfos, List<string> addresses, out bool logsRetrieved)
		{
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			if (deviceInfos == null)
			{
				throw new ArgumentNullException("deviceInfos");
			}
			if (addresses == null)
			{
				throw new ArgumentNullException("addresses");
			}
			bool flag = false;
			MessageItem messageItem = null;
			byte[] array = null;
			logsRetrieved = false;
			try
			{
				StringBuilder stringBuilder = new StringBuilder();
				CultureInfo preferedCulture = mailboxSession.PreferedCulture;
				StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Drafts);
				messageItem = MessageItem.Create(mailboxSession, defaultFolderId);
				messageItem.ClassName = "IPM.Note.Exchange.ActiveSync.MailboxLog";
				for (int i = 0; i < deviceInfos.Length; i++)
				{
					using (MailboxLogger mailboxLogger = new MailboxLogger(mailboxSession, deviceInfos[i].DeviceIdentity))
					{
						if (mailboxLogger.LogsExist)
						{
							logsRetrieved = true;
							string text = string.Format(CultureInfo.InvariantCulture, "EASMailboxLog_{0}_{1}_{2}.txt", new object[]
							{
								userName,
								deviceInfos[i].DeviceIdentity.DeviceType,
								deviceInfos[i].DeviceIdentity.DeviceId
							});
							stringBuilder.Append(AirSyncUtility.HtmlEncode(string.Format(CultureInfo.InvariantCulture, Strings.DeviceType.ToString(preferedCulture), new object[]
							{
								deviceInfos[i].DeviceIdentity.DeviceType
							}), false));
							stringBuilder.Append("<br>");
							stringBuilder.Append(AirSyncUtility.HtmlEncode(string.Format(CultureInfo.InvariantCulture, Strings.DeviceId.ToString(preferedCulture), new object[]
							{
								deviceInfos[i].DeviceIdentity.DeviceId
							}), false));
							stringBuilder.Append("<br><br>");
							using (StreamAttachment streamAttachment = (StreamAttachment)messageItem.AttachmentCollection.Create(AttachmentType.Stream))
							{
								streamAttachment.FileName = text;
								using (Stream contentStream = streamAttachment.GetContentStream())
								{
									if (deviceInfos[i].mailboxLogReport == null)
									{
										mailboxLogger.GenerateReport(contentStream);
									}
									else
									{
										if (array == null)
										{
											array = new byte[4100];
										}
										string text2 = deviceInfos[i].mailboxLogReport;
										int num;
										for (int j = 0; j < text2.Length; j += num)
										{
											num = ((text2.Length - j > 1024) ? 1024 : (text2.Length - j));
											int bytes = Encoding.UTF8.GetBytes(text2, j, num, array, 0);
											contentStream.Write(array, 0, bytes);
										}
									}
									if (contentStream.Length > 5242880L)
									{
										streamAttachment.FileName = string.Format(CultureInfo.InvariantCulture, "EASMailboxLog_{0}_{1}_{2}.gz", new object[]
										{
											userName,
											deviceInfos[i].DeviceIdentity.DeviceType,
											deviceInfos[i].DeviceIdentity.DeviceId
										});
										using (MemoryStream memoryStream = new MemoryStream((int)(contentStream.Length / 10L)))
										{
											byte[] array2 = new byte[]
											{
												31,
												139,
												8,
												8,
												0,
												0,
												0,
												0,
												4,
												0
											};
											memoryStream.Write(array2, 0, array2.Length);
											byte[] bytes2 = Encoding.UTF8.GetBytes(text);
											memoryStream.Write(bytes2, 0, bytes2.Length);
											memoryStream.WriteByte(0);
											long length = contentStream.Length;
											uint num2 = 0U;
											using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress, true))
											{
												StreamHelper.CopyStream(contentStream, deflateStream, (int)length, out num2);
											}
											memoryStream.WriteByte((byte)(num2 & 255U));
											memoryStream.WriteByte((byte)(num2 >> 8 & 255U));
											memoryStream.WriteByte((byte)(num2 >> 16 & 255U));
											memoryStream.WriteByte((byte)(num2 >> 24 & 255U));
											memoryStream.WriteByte((byte)(length & 255L));
											memoryStream.WriteByte((byte)(length >> 8 & 255L));
											memoryStream.WriteByte((byte)(length >> 16 & 255L));
											memoryStream.WriteByte((byte)(length >> 24 & 255L));
											contentStream.Flush();
											contentStream.Dispose();
											using (Stream contentStream2 = streamAttachment.GetContentStream(PropertyOpenMode.Create))
											{
												StreamHelper.CopyStream(memoryStream, contentStream2, (int)memoryStream.Length);
											}
										}
									}
								}
								streamAttachment.Save();
							}
						}
					}
				}
				if (logsRetrieved)
				{
					messageItem.Subject = string.Format(CultureInfo.InvariantCulture, Strings.DeviceStatisticsTaskMailboxLogSubject.ToString(preferedCulture), new object[]
					{
						userName
					});
					using (TextWriter textWriter = messageItem.Body.OpenTextWriter(BodyFormat.TextHtml))
					{
						string input = string.Format(CultureInfo.InvariantCulture, Strings.Date.ToString(preferedCulture), new object[]
						{
							ExDateTime.Now.ToString(preferedCulture)
						});
						string input2 = string.Format(CultureInfo.InvariantCulture, Strings.UserName.ToString(preferedCulture), new object[]
						{
							userName.ToString(preferedCulture)
						});
						textWriter.Write("\r\n            <html>\r\n                {0}\r\n                <body>\r\n                    <h1>{1}</h1>\r\n                    <p>\r\n                        <br>\r\n                        {2}\r\n                        <br><br>\r\n                        {3}\r\n                        <br><br>\r\n                        {4}\r\n                    </p>\r\n                    <font color=\"red\">\r\n                    {5}\r\n                    </font>\r\n                </body>\r\n            </html>\r\n            ", new object[]
						{
							"\r\n                <style>\r\n                    body\r\n                    {\r\n                        font-family: Tahoma;\r\n                        background-color: rgb(255,255,255);\r\n                        color: #000000;\r\n                        font-size:x-small;\r\n                        width: 600px\r\n                    }\r\n                    p\r\n                    {\r\n                        margin:0in;\r\n                    }\r\n                    h1\r\n                    {\r\n                        font-family: Arial;\r\n                        color: #000066;\r\n                        margin: 0in;\r\n                        font-size: medium; font-weight:bold\r\n                    }\r\n                </style>\r\n                ",
							AirSyncUtility.HtmlEncode(messageItem.Subject, false),
							AirSyncUtility.HtmlEncode(input, false),
							AirSyncUtility.HtmlEncode(input2, false),
							stringBuilder.ToString(),
							AirSyncUtility.HtmlEncode(Strings.DeviceStatisticsTaskMailboxLogAttachmentNote.ToString(preferedCulture), false)
						});
					}
					messageItem.From = null;
					if (addresses.Count > 0)
					{
						for (int k = 0; k < addresses.Count; k++)
						{
							Participant participant = new Participant(null, addresses[k], "SMTP");
							messageItem.Recipients.Add(participant, RecipientItemType.To);
						}
						messageItem.Send();
						flag = true;
					}
				}
			}
			finally
			{
				if (messageItem != null)
				{
					if (!flag)
					{
						messageItem.Load();
						if (messageItem.Id != null)
						{
							AggregateOperationResult aggregateOperationResult = mailboxSession.Delete(DeleteItemFlags.HardDelete, new StoreId[]
							{
								messageItem.Id.ObjectId
							});
							if (OperationResult.Succeeded != aggregateOperationResult.OperationResult)
							{
								AirSyncDiagnostics.TraceDebug<MessageItem>(ExTraceGlobals.RequestsTracer, null, "Failed to delete {0}", messageItem);
							}
						}
					}
					messageItem.Dispose();
				}
			}
			return flag;
		}

		internal static void RemoveDevice(MailboxSession mailboxSession, DeviceIdentity deviceIdentity, bool updateThrottlingData = true)
		{
			if (mailboxSession == null)
			{
				return;
			}
			ActiveSyncDevices activeSyncDevices = null;
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(false, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(mailboxSession.MailboxOwner.MailboxInfo.OrganizationId), 1312, "RemoveDevice", "f:\\15.00.1497\\sources\\dev\\AirSync\\src\\AirSync\\DeviceInfo.cs");
			tenantOrTopologyConfigurationSession.UseConfigNC = false;
			if (updateThrottlingData && DeviceInfo.IsThrottlingLimitsExceeded(tenantOrTopologyConfigurationSession, mailboxSession.MailboxOwner, out activeSyncDevices))
			{
				return;
			}
			DeviceInfo.RemoveDeviceFromMailbox(mailboxSession, deviceIdentity);
			MobileDevice[] array = DeviceInfo.FindAllADDevice(tenantOrTopologyConfigurationSession, mailboxSession.MailboxOwner.ObjectId, deviceIdentity);
			if (array == null)
			{
				return;
			}
			foreach (MobileDevice instance in array)
			{
				tenantOrTopologyConfigurationSession.Delete(instance);
			}
			if (Command.CurrentCommand != null)
			{
				Command.CurrentCommand.Context.ProtocolLogger.SetValue(ProtocolLoggerData.DomainController, tenantOrTopologyConfigurationSession.LastUsedDc);
			}
			if (updateThrottlingData)
			{
				DeviceInfo.UpdateThrottlingData(tenantOrTopologyConfigurationSession, activeSyncDevices);
			}
		}

		internal static bool RemoveDeviceFromMailbox(MailboxSession mailboxSession, DeviceIdentity deviceIdentity)
		{
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			ArgumentValidator.ThrowIfNull("deviceIdentity", deviceIdentity);
			if (deviceIdentity.IsDnMangled)
			{
				AirSyncDiagnostics.TraceDebug<DeviceIdentity>(ExTraceGlobals.RequestsTracer, null, "[DeviceInfo.RemoveDeviceFromMailbox] Device Id was mangled due to naming conflicts.  Cannot remove device since we cannot generate the composite sync state name.  Device Identity: '{0}'", deviceIdentity);
				return false;
			}
			bool result;
			using (SyncStateStorage syncStateStorage = SyncStateStorage.Bind(mailboxSession, deviceIdentity, null))
			{
				result = DeviceInfo.RemoveDeviceFromMailbox(mailboxSession, syncStateStorage, deviceIdentity);
			}
			return result;
		}

		internal static bool RemoveDeviceFromMailbox(MailboxSession mailboxSession, SyncStateStorage syncStateStorage, DeviceIdentity deviceIdentity)
		{
			ArgumentValidator.ThrowIfNull("deviceIdentity", deviceIdentity);
			if (syncStateStorage != null)
			{
				DeviceInfo.ResetMobileServiceSelector(mailboxSession, syncStateStorage);
				bool flag = SyncStateStorage.DeleteSyncStateStorage(mailboxSession, syncStateStorage.FolderId, deviceIdentity, null);
				AirSyncDiagnostics.TraceDebug<DeviceIdentity, bool>(ExTraceGlobals.RequestsTracer, null, "[DeviceInfo.RemoveDeviceFromMailbox] DeletedSyncStateStorage {0}, AnyOtherDevices:{1}", deviceIdentity, flag);
				DeviceInfo.UpdateDeviceHasPartnership(mailboxSession, flag);
				return true;
			}
			return false;
		}

		internal static void UpdateDeviceHasPartnership(MailboxSession mailboxSession, bool hasDevicePartnership)
		{
			IRecipientSession recipientSession = mailboxSession.GetADRecipientSession(false, ConsistencyMode.FullyConsistent);
			recipientSession.UseGlobalCatalog = false;
			ADRecipient recipient = recipientSession.Read(mailboxSession.MailboxOwner.ObjectId);
			if (Command.CurrentCommand != null)
			{
				Command.CurrentCommand.Context.ProtocolLogger.SetValue(ProtocolLoggerData.DomainController, recipientSession.LastUsedDc);
			}
			ADUser aduser = (ADUser)recipient;
			bool flag = (aduser.MobileMailboxFlags & MobileMailboxFlags.HasDevicePartnership) != MobileMailboxFlags.None;
			if (flag != hasDevicePartnership)
			{
				if (hasDevicePartnership)
				{
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, "[DeviceInfo.RemoveDeviceFromMailbox] Update HasDevicePartnership to true");
					aduser.MobileMailboxFlags |= MobileMailboxFlags.HasDevicePartnership;
				}
				else
				{
					aduser.MobileMailboxFlags &= ~MobileMailboxFlags.HasDevicePartnership;
				}
				ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
				{
					recipientSession.Save(recipient);
				});
				if (!adoperationResult.Succeeded)
				{
					AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, null, "Exception occurred during AD Operation. Message:{0}", adoperationResult.Exception.Message);
					if (Command.CurrentCommand != null)
					{
						Command.CurrentCommand.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "UpdateUserHasDeviceADException");
					}
				}
				if (Command.CurrentCommand != null)
				{
					Command.CurrentCommand.Context.ProtocolLogger.SetValue(ProtocolLoggerData.DomainController, recipientSession.LastUsedDc);
					Command.CurrentCommand.Context.ProtocolLogger.SetValue(ProtocolLoggerData.UpdateUserHasPartnerships, hasDevicePartnership ? "T" : "F");
				}
			}
		}

		internal static bool CancelRemoteWipe(SyncStateStorage syncStateStorage)
		{
			if (syncStateStorage == null)
			{
				throw new ArgumentNullException("syncStateStorage");
			}
			bool success = false;
			bool flag = DeviceInfo.CorruptionSafeAction("CancelRemoteWipe", delegate
			{
				using (CustomSyncState orCreateGlobalSyncState = AirSyncUtility.GetOrCreateGlobalSyncState(syncStateStorage))
				{
					success = DeviceInfo.CancelRemoteWipeFromMailbox(orCreateGlobalSyncState);
					if (success)
					{
						orCreateGlobalSyncState.Commit();
					}
				}
			});
			return success && flag;
		}

		internal static bool CancelRemoteWipeFromMailbox(CustomSyncState globalSyncState)
		{
			if (globalSyncState == null)
			{
				throw new ArgumentNullException("globalSyncState");
			}
			if (globalSyncState[CustomStateDatumType.WipeAckTime] != null || globalSyncState[CustomStateDatumType.WipeSendTime] != null)
			{
				return false;
			}
			globalSyncState[CustomStateDatumType.WipeRequestTime] = null;
			return true;
		}

		internal static void StartRemoteWipe(SyncStateStorage syncStateStorage, ExDateTime wipeRequestTime, string requestorSMTP)
		{
			DeviceInfo.StartRemoteWipe(syncStateStorage, wipeRequestTime, null, requestorSMTP);
		}

		internal static void StartRemoteWipe(SyncStateStorage syncStateStorage, ExDateTime wipeRequestTime, IList<string> additionalAddresses, string requestorSMTP)
		{
			if (syncStateStorage == null)
			{
				throw new ArgumentNullException("syncStateStorage");
			}
			DeviceInfo.CorruptionSafeAction("StartRemoteWipe", delegate
			{
				using (CustomSyncState orCreateGlobalSyncState = AirSyncUtility.GetOrCreateGlobalSyncState(syncStateStorage))
				{
					DeviceInfo.StartRemoteWipeFromMailbox(syncStateStorage, orCreateGlobalSyncState, wipeRequestTime, additionalAddresses, requestorSMTP);
					orCreateGlobalSyncState.Commit();
				}
			});
		}

		internal static void StartRemoteWipeFromMailbox(SyncStateStorage syncStateStorage, CustomSyncState globalSyncState, ExDateTime wipeRequestTime, IList<string> additionalAddresses, string requestorSMTP)
		{
			if (syncStateStorage == null)
			{
				throw new ArgumentNullException("syncStateStorage");
			}
			if (globalSyncState == null)
			{
				throw new ArgumentNullException("globalSyncState");
			}
			globalSyncState[CustomStateDatumType.WipeRequestTime] = new DateTimeData(wipeRequestTime);
			globalSyncState[CustomStateDatumType.WipeSendTime] = null;
			globalSyncState[CustomStateDatumType.WipeAckTime] = null;
			globalSyncState[CustomStateDatumType.LastDeviceWipeRequestor] = new StringData(requestorSMTP);
			if (additionalAddresses != null)
			{
				string[] array = new string[additionalAddresses.Count];
				additionalAddresses.CopyTo(array, 0);
				globalSyncState[CustomStateDatumType.WipeConfirmationAddresses] = new ArrayData<StringData, string>(array);
			}
			else
			{
				globalSyncState[CustomStateDatumType.WipeConfirmationAddresses] = null;
			}
			AutdTriggerSyncStateInfo syncStateInfo = new AutdTriggerSyncStateInfo();
			CustomSyncState customSyncState = syncStateStorage.GetCustomSyncState(syncStateInfo, new PropertyDefinition[0]);
			if (customSyncState == null)
			{
				customSyncState = syncStateStorage.CreateCustomSyncState(syncStateInfo);
			}
			customSyncState.Dispose();
			syncStateStorage.DeleteCustomSyncState(syncStateInfo);
		}

		internal static void ResetMobileServiceSelector(MailboxSession mailboxSession, SyncStateStorage syncStateStorage)
		{
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			if (syncStateStorage == null)
			{
				throw new ArgumentNullException("syncStateStorage");
			}
			using (VersionedXmlDataProvider versionedXmlDataProvider = new VersionedXmlDataProvider(mailboxSession))
			{
				TextMessagingAccount textMessagingAccount = (TextMessagingAccount)versionedXmlDataProvider.Read<TextMessagingAccount>(mailboxSession.MailboxOwner.ObjectId);
				if (textMessagingAccount != null && textMessagingAccount.EasEnabled)
				{
					DeviceInfo deviceInfo = DeviceInfo.GetDeviceInfo(syncStateStorage);
					if (deviceInfo != null)
					{
						E164Number b = null;
						E164Number.TryParse(deviceInfo.DevicePhoneNumber, out b);
						if (textMessagingAccount.EasPhoneNumber == b)
						{
							IRecipientSession adrecipientSession = mailboxSession.GetADRecipientSession(false, ConsistencyMode.FullyConsistent);
							ADRecipient adrecipient = adrecipientSession.Read(mailboxSession.MailboxOwner.ObjectId);
							if (Command.CurrentCommand != null)
							{
								Command.CurrentCommand.Context.ProtocolLogger.SetValue(ProtocolLoggerData.DomainController, adrecipientSession.LastUsedDc);
							}
							if (adrecipient == null)
							{
								AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, "Error:  Could not find the AD person correlated to text messaging account");
							}
							else
							{
								textMessagingAccount.SetEasDisabled();
								TextMessagingHelper.SaveTextMessagingAccount(textMessagingAccount, versionedXmlDataProvider, adrecipient, adrecipientSession);
							}
						}
					}
				}
			}
		}

		internal static string ObfuscatePhoneNumber(string phoneNumber)
		{
			if (phoneNumber == null)
			{
				return null;
			}
			char[] array = phoneNumber.Trim().ToCharArray();
			for (int i = 0; i < array.Length - 4; i++)
			{
				array[i] = '*';
			}
			return new string(array);
		}

		internal static bool IsThrottlingLimitsExceeded(IConfigurationSession session, IExchangePrincipal exchangePrincipal, out ActiveSyncDevices activeSyncDevices)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (exchangePrincipal == null)
			{
				throw new ArgumentNullException("exchangePrincipal");
			}
			activeSyncDevices = null;
			using (IBudget budget = StandardBudget.Acquire(exchangePrincipal.Sid, BudgetType.Eas, exchangePrincipal.MailboxInfo.OrganizationId.ToADSessionSettings()))
			{
				if (budget != null)
				{
					IThrottlingPolicy throttlingPolicy = budget.ThrottlingPolicy;
					if (throttlingPolicy != null && !throttlingPolicy.EasMaxDeviceDeletesPerMonth.IsUnlimited)
					{
						ExDateTime utcNow = ExDateTime.UtcNow;
						ADObjectId rootId = MobileDevice.GetRootId(exchangePrincipal.ObjectId);
						activeSyncDevices = session.Read<ActiveSyncDevices>(rootId);
						if (activeSyncDevices == null)
						{
							AirSyncDiagnostics.TraceError(ExTraceGlobals.RequestsTracer, null, "Error: Cannot load ActiveSyncDevices! Skipping delete count check.");
						}
						else if ((long)activeSyncDevices.ObjectsDeletedThisPeriod >= (long)((ulong)throttlingPolicy.EasMaxDeviceDeletesPerMonth.Value) && activeSyncDevices.DeletionPeriod != null && activeSyncDevices.DeletionPeriod.Value.Year == utcNow.Year && activeSyncDevices.DeletionPeriod.Value.Month == utcNow.Month)
						{
							AirSyncDiagnostics.TraceDebug<int, Unlimited<uint>, DateTime>(ExTraceGlobals.RequestsTracer, null, "Error: EASMaxDeviceDeletesPerMonth exceeded. ObjectsDeletedThisPeriod {0}, MaxDeviceDeletesPerMonth {1}, DeletionPeriod {2}", activeSyncDevices.ObjectsDeletedThisPeriod, throttlingPolicy.EasMaxDeviceDeletesPerMonth, activeSyncDevices.DeletionPeriod.Value);
							return true;
						}
					}
					else
					{
						AirSyncDiagnostics.TraceError(ExTraceGlobals.RequestsTracer, null, "Error: Not throttling value set! Skipping delete count check.");
					}
				}
				else
				{
					AirSyncDiagnostics.TraceError(ExTraceGlobals.RequestsTracer, null, "Error: Cannot load budget! Skipping delete count check.");
				}
			}
			return false;
		}

		internal static void UpdateThrottlingData(IConfigurationSession session, ActiveSyncDevices activeSyncDevices)
		{
			if (session != null && activeSyncDevices != null)
			{
				ExDateTime utcNow = ExDateTime.UtcNow;
				if (activeSyncDevices.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2010))
				{
					activeSyncDevices.SetExchangeVersion(ExchangeObjectVersion.Exchange2010);
				}
				if (activeSyncDevices.DeletionPeriod != null && activeSyncDevices.DeletionPeriod.Value.Year == utcNow.Year && activeSyncDevices.DeletionPeriod.Value.Month == utcNow.Month)
				{
					activeSyncDevices.ObjectsDeletedThisPeriod++;
				}
				else
				{
					activeSyncDevices.ObjectsDeletedThisPeriod = 1;
					activeSyncDevices.DeletionPeriod = new DateTime?((DateTime)utcNow.ToUtc());
				}
				session.Save(activeSyncDevices);
			}
		}

		internal string GetOrCreateMailboxLogReport(MailboxSession mailboxSession)
		{
			if (this.mailboxLogReport != null)
			{
				return this.mailboxLogReport;
			}
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			using (MailboxLogger mailboxLogger = new MailboxLogger(mailboxSession, this.DeviceIdentity))
			{
				this.mailboxLogReport = mailboxLogger.GenerateReport();
			}
			return this.mailboxLogReport;
		}

		internal bool IsMailboxLogAvailable(MailboxSession mailboxSession)
		{
			if (this.mailboxLogAvailable != null)
			{
				return this.mailboxLogAvailable.Value;
			}
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			using (MailboxLogger mailboxLogger = new MailboxLogger(mailboxSession, this.DeviceIdentity))
			{
				this.mailboxLogAvailable = new bool?(mailboxLogger.LogsExist);
			}
			return this.mailboxLogAvailable.Value;
		}

		private static RawT GetObjectProperty<T, RawT>(SyncState syncState, string name, RawT defaultProperty) where T : ComponentData<RawT>, new() where RawT : class
		{
			return syncState.GetData<T, RawT>(name, defaultProperty);
		}

		private static RawT? GetValueProperty<T, RawT>(SyncState syncState, string name, RawT? defaultProperty) where T : ComponentData<RawT>, new() where RawT : struct
		{
			ICustomSerializable customSerializable = syncState[name];
			if (customSerializable != null)
			{
				if (customSerializable is T)
				{
					T t = (T)((object)customSerializable);
					return new RawT?(t.Data);
				}
				if (customSerializable is NullableData<T, RawT>)
				{
					return ((NullableData<T, RawT>)customSerializable).Data;
				}
			}
			return defaultProperty;
		}

		private static MobileDevice[] FindAllADDevice(IConfigurationSession session, ADObjectId mailboxOwnerADObjectId, DeviceIdentity deviceIdentity)
		{
			QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, MobileDeviceSchema.DeviceId, deviceIdentity.DeviceId);
			QueryFilter queryFilter2 = new ComparisonFilter(ComparisonOperator.Equal, MobileDeviceSchema.DeviceType, deviceIdentity.DeviceType);
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				queryFilter,
				queryFilter2
			});
			MobileDevice[] array = session.Find<MobileDevice>(MobileDevice.GetRootId(mailboxOwnerADObjectId), QueryScope.OneLevel, filter, null, 2);
			if (Command.CurrentCommand != null)
			{
				Command.CurrentCommand.Context.ProtocolLogger.SetValue(ProtocolLoggerData.DomainController, session.LastUsedDc);
			}
			if (array.Length == 0)
			{
				AirSyncDiagnostics.TraceInfo<string, string>(ExTraceGlobals.ValidationTracer, null, "No ActiveSyncDevice object found in AD for user {0}, device id {1}.", mailboxOwnerADObjectId.Rdn.UnescapedName, deviceIdentity.DeviceId);
				return null;
			}
			return array;
		}

		private static bool CorruptionSafeAction(string info, Action action)
		{
			bool result = false;
			try
			{
				action();
				result = true;
			}
			catch (CorruptSyncStateException arg)
			{
				AirSyncDiagnostics.TraceError<string, CorruptSyncStateException>(ExTraceGlobals.ValidationTracer, null, "[DeviceInfo.{0}] Sync state exception was caught: {1}", info, arg);
			}
			catch (InvalidSyncStateVersionException arg2)
			{
				AirSyncDiagnostics.TraceError<string, InvalidSyncStateVersionException>(ExTraceGlobals.ValidationTracer, null, "[DeviceInfo.{0}] Sync state exception was caught: {1}", info, arg2);
			}
			return result;
		}

		private const string ActiveSyncLogReportBody = "\r\n            <html>\r\n                {0}\r\n                <body>\r\n                    <h1>{1}</h1>\r\n                    <p>\r\n                        <br>\r\n                        {2}\r\n                        <br><br>\r\n                        {3}\r\n                        <br><br>\r\n                        {4}\r\n                    </p>\r\n                    <font color=\"red\">\r\n                    {5}\r\n                    </font>\r\n                </body>\r\n            </html>\r\n            ";

		private const string MailboxLogFileName = "ActiveSyncMailboxLog.txt";

		private const string MailboxLogReportStyle = "\r\n                <style>\r\n                    body\r\n                    {\r\n                        font-family: Tahoma;\r\n                        background-color: rgb(255,255,255);\r\n                        color: #000000;\r\n                        font-size:x-small;\r\n                        width: 600px\r\n                    }\r\n                    p\r\n                    {\r\n                        margin:0in;\r\n                    }\r\n                    h1\r\n                    {\r\n                        font-family: Arial;\r\n                        color: #000066;\r\n                        margin: 0in;\r\n                        font-size: medium; font-weight:bold\r\n                    }\r\n                </style>\r\n                ";

		private const string MailboxLogReportBody = "\r\n            <html>\r\n                {0}\r\n                <body>\r\n                    <h1>{1}</h1>\r\n                    <br>\r\n                    <p>\r\n                    {2}\r\n                    </p>\r\n                </body>\r\n            </html>\r\n            ";

		private const int MinLogSizeToStartCompress = 5242880;

		private string deviceModel;

		private string deviceImei;

		private string deviceFriendlyName;

		private string deviceOS;

		private string deviceOSLanguage;

		private string devicePhoneNumber;

		private bool deviceEnableOutboundSMS;

		private string deviceMobileOperator;

		private ExDateTime? firstSyncTime;

		private ExDateTime? lastSyncAttemptTime;

		private ExDateTime? lastSyncSuccessTime;

		private string userAgent;

		private ExDateTime? wipeRequestTime;

		private ExDateTime? wipeSentTime;

		private ExDateTime? wipeAckTime;

		private string[] remoteWipeConfirmationAddresses;

		private ExDateTime? lastPolicyUpdateTime;

		private uint? lastPingHeartbeat;

		private string recoveryPassword;

		private bool? mailboxLogAvailable;

		private string mailboxLogReport;

		private bool remoteWipeSupported;
	}
}
