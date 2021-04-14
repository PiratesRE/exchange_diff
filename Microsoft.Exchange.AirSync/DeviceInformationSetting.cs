using System;
using System.Globalization;
using System.Net;
using System.Xml;
using Microsoft.Exchange.Configuration.SQM;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics.Components.AirSync;

namespace Microsoft.Exchange.AirSync
{
	internal class DeviceInformationSetting : SettingsBase
	{
		internal DeviceInformationSetting(XmlNode request, XmlNode response, Command command, ProtocolLogger protocolLogger) : base(request, response, protocolLogger)
		{
			this.command = command;
			this.mailboxSession = this.command.MailboxSession;
			this.syncStateStorage = this.command.SyncStateStorage;
		}

		public override void Execute()
		{
			using (this.command.Context.Tracker.Start(TimeId.DeviceInfoExecute))
			{
				try
				{
					XmlNode firstChild = base.Request.FirstChild;
					string localName;
					if ((localName = firstChild.LocalName) != null && localName == "Set")
					{
						this.ProcessSet(firstChild);
					}
					else
					{
						this.status = SettingsBase.ErrorCode.ProtocolError;
					}
					this.ReportStatus();
				}
				catch (Exception exception)
				{
					if (!this.ProcessException(exception))
					{
						throw;
					}
				}
			}
		}

		private void ReportStatus()
		{
			XmlNode xmlNode = base.Response.OwnerDocument.CreateElement("Status", "Settings:");
			int num = (int)this.status;
			xmlNode.InnerText = num.ToString(CultureInfo.InvariantCulture);
			base.Response.AppendChild(xmlNode);
		}

		private bool ProcessException(Exception exception)
		{
			bool result;
			using (this.command.Context.Tracker.Start(TimeId.DeviceInfoProcessException))
			{
				Command.CurrentCommand.PartialFailure = true;
				if (exception is FormatException)
				{
					base.ProtocolLogger.AppendValue(ProtocolLoggerData.Error, "DIS:FormatException");
					this.status = SettingsBase.ErrorCode.ProtocolError;
					this.ReportStatus();
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		private void ProcessSet(XmlNode setNode)
		{
			using (this.command.Context.Tracker.Start(TimeId.DeviceInfoProcessSet))
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Processing DeviceInformation - Set");
				foreach (object obj in setNode.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					string innerText = xmlNode.InnerText;
					string localName;
					switch (localName = xmlNode.LocalName)
					{
					case "Model":
						this.deviceModel = DeviceClassCache.NormalizeDeviceClass(innerText);
						break;
					case "IMEI":
						this.deviceImei = innerText;
						break;
					case "FriendlyName":
						this.deviceFriendlyName = innerText;
						break;
					case "OS":
						this.deviceOS = innerText;
						break;
					case "OSLanguage":
						this.deviceOSLanguage = innerText;
						break;
					case "PhoneNumber":
						this.devicePhoneNumber = innerText;
						break;
					case "UserAgent":
						this.deviceUserAgent = innerText;
						break;
					case "EnableOutboundSMS":
					{
						string a;
						if (this.command.User.IsConsumerOrganizationUser)
						{
							this.deviceEnableOutboundSMS = false;
						}
						else if ((a = innerText) != null)
						{
							if (!(a == "0"))
							{
								if (a == "1")
								{
									this.deviceEnableOutboundSMS = true;
								}
							}
							else
							{
								this.deviceEnableOutboundSMS = false;
							}
						}
						break;
					}
					case "MobileOperator":
						this.deviceMobileOperator = innerText;
						break;
					case "Annotations":
						this.command.RequestAnnotations.ParseWLAnnotations(xmlNode, "DeviceInformation");
						break;
					}
				}
				if (this.command.RequestAnnotations.ContainsAnnotation("CreateChatsFolder", "DeviceInformation"))
				{
					this.CreateSmsAndChatsSyncFolder();
				}
				bool flag = false;
				GlobalInfo globalInfo = this.command.GlobalInfo;
				globalInfo.DeviceModel = this.deviceModel;
				globalInfo.DeviceImei = this.deviceImei;
				globalInfo.DeviceFriendlyName = this.deviceFriendlyName;
				globalInfo.UserAgent = this.deviceUserAgent;
				string text;
				if (this.command.Context.Request.DeviceIdentity.DeviceType.ToUpper().Contains("SAMSUNG") && this.command.TryParseDeviceOSFromUserAgent(out text))
				{
					this.deviceOS = text;
				}
				globalInfo.DeviceOS = this.deviceOS;
				globalInfo.DeviceOSLanguage = this.deviceOSLanguage;
				globalInfo.DevicePhoneNumber = this.devicePhoneNumber;
				string text2 = string.IsNullOrEmpty(this.devicePhoneNumber) ? globalInfo.DevicePhoneNumberForSms : this.devicePhoneNumber;
				if (string.IsNullOrEmpty(text2))
				{
					text2 = Guid.NewGuid().GetHashCode().ToString("D3", CultureInfo.InvariantCulture);
					globalInfo.DevicePhoneNumberForSms = text2;
				}
				else
				{
					flag |= (string.Compare(text2, globalInfo.DevicePhoneNumberForSms, StringComparison.Ordinal) != 0);
				}
				flag |= (this.deviceEnableOutboundSMS != globalInfo.DeviceEnableOutboundSMS);
				globalInfo.DeviceMobileOperator = this.deviceMobileOperator;
				globalInfo.DeviceInformationReceived = true;
				SmsSqmDataPointHelper.AddDeviceInfoReceivedDataPoint(SmsSqmSession.Instance, this.mailboxSession.MailboxOwner.ObjectId, this.mailboxSession.MailboxOwner.LegacyDn, this.command.Request.DeviceIdentity.DeviceType, this.command.Request.VersionString);
				if (flag)
				{
					try
					{
						using (VersionedXmlDataProvider versionedXmlDataProvider = new VersionedXmlDataProvider(this.mailboxSession))
						{
							TextMessagingAccount textMessagingAccount = (TextMessagingAccount)versionedXmlDataProvider.Read<TextMessagingAccount>(this.mailboxSession.MailboxOwner.ObjectId);
							IRecipientSession adrecipientSession = this.mailboxSession.GetADRecipientSession(false, ConsistencyMode.FullyConsistent);
							ADRecipient adrecipient = adrecipientSession.Read(this.mailboxSession.MailboxOwner.ObjectId);
							this.command.Context.ProtocolLogger.SetValue(ProtocolLoggerData.DomainController, adrecipientSession.LastUsedDc);
							if (adrecipient == null)
							{
								throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.ServerError, new LocalizedString("Cannot find AD Recipient correlated to the text messaging account"), false)
								{
									ErrorStringForProtocolLogger = "NoUserAccountForSms"
								};
							}
							E164Number e164Number = null;
							E164Number.TryParse(text2, out e164Number);
							if (this.deviceEnableOutboundSMS)
							{
								if (e164Number == null)
								{
									throw new AirSyncPermanentException(StatusCode.Sync_ServerError, new LocalizedString(string.Format("Cannot parse phone number {0} into a E164 number.", text2)), false)
									{
										ErrorStringForProtocolLogger = "BadSmsPhoneNum"
									};
								}
								bool notificationEnabled = textMessagingAccount.NotificationPhoneNumber != null && textMessagingAccount.NotificationPhoneNumberVerified;
								textMessagingAccount.SetEasEnabled(e164Number, this.syncStateStorage.DeviceIdentity.Protocol, this.syncStateStorage.DeviceIdentity.DeviceType, this.syncStateStorage.DeviceIdentity.DeviceId, this.deviceFriendlyName);
								TextMessagingHelper.SaveTextMessagingAccount(textMessagingAccount, versionedXmlDataProvider, adrecipient, adrecipientSession);
								this.command.Context.ProtocolLogger.SetValue(ProtocolLoggerData.DomainController, adrecipientSession.LastUsedDc);
								SmsSqmDataPointHelper.AddEasConfigurationDataPoint(SmsSqmSession.Instance, this.mailboxSession.MailboxOwner.ObjectId, this.mailboxSession.MailboxOwner.LegacyDn, this.command.Request.DeviceIdentity.DeviceType, notificationEnabled, this.command.Request.VersionString);
							}
							else if (textMessagingAccount.EasEnabled && textMessagingAccount.EasPhoneNumber == e164Number)
							{
								textMessagingAccount.SetEasDisabled();
								TextMessagingHelper.SaveTextMessagingAccount(textMessagingAccount, versionedXmlDataProvider, adrecipient, adrecipientSession);
								this.command.Context.ProtocolLogger.SetValue(ProtocolLoggerData.DomainController, adrecipientSession.LastUsedDc);
							}
						}
						globalInfo.DevicePhoneNumberForSms = text2;
						globalInfo.DeviceEnableOutboundSMS = this.deviceEnableOutboundSMS;
					}
					catch (StoragePermanentException innerException)
					{
						throw new AirSyncPermanentException(StatusCode.ServerError, new LocalizedString("Server Error when trying to update SMS settings."), innerException, false)
						{
							ErrorStringForProtocolLogger = "SmsSettingsSaveError"
						};
					}
				}
				this.OutputToIISLog();
			}
		}

		private void CreateSmsAndChatsSyncFolder()
		{
			if (this.mailboxSession.GetDefaultFolderId(DefaultFolderType.SmsAndChatsSync) == null && this.mailboxSession.CreateDefaultFolder(DefaultFolderType.SmsAndChatsSync) == null)
			{
				throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.ServerError, new LocalizedString("Failed to create the SmsAndChatsSync folder"), false)
				{
					ErrorStringForProtocolLogger = "CreateSmsAndChatsSyncFolderFailure"
				};
			}
		}

		private void OutputToIISLog()
		{
			base.ProtocolLogger.SetTrimmedValue(ProtocolLoggerData.DeviceInfoModel, this.deviceModel, 50);
			base.ProtocolLogger.SetTrimmedValue(ProtocolLoggerData.DeviceInfoIMEI, this.deviceImei, 50);
			base.ProtocolLogger.SetTrimmedValue(ProtocolLoggerData.DeviceInfoFriendlyName, this.deviceFriendlyName, 50);
			base.ProtocolLogger.SetTrimmedValue(ProtocolLoggerData.DeviceInfoOS, this.deviceOS, 50);
			base.ProtocolLogger.SetTrimmedValue(ProtocolLoggerData.DeviceInfoOSLanguage, this.deviceOSLanguage, 50);
			base.ProtocolLogger.SetTrimmedValue(ProtocolLoggerData.DeviceInfoUserAgent, this.deviceUserAgent, 50);
			base.ProtocolLogger.SetValue(ProtocolLoggerData.DeviceInfoEnableOutboundSMS, this.deviceEnableOutboundSMS ? "1" : "0");
			base.ProtocolLogger.SetTrimmedValue(ProtocolLoggerData.DeviceInfoMobileOperator, this.deviceMobileOperator, 50);
		}

		internal const int MaxParamLength = 50;

		internal const string DeviceInfoAnnotationManagerGroupName = "DeviceInformation";

		private SettingsBase.ErrorCode status = SettingsBase.ErrorCode.Success;

		private MailboxSession mailboxSession;

		private SyncStateStorage syncStateStorage;

		private string deviceModel;

		private string deviceImei;

		private string deviceFriendlyName;

		private string deviceOS;

		private string deviceOSLanguage;

		private string devicePhoneNumber;

		private string deviceUserAgent;

		private bool deviceEnableOutboundSMS;

		private string deviceMobileOperator;

		private Command command;
	}
}
