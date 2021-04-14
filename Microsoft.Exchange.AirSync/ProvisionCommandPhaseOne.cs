using System;
using System.Globalization;
using System.Net;
using System.Security;
using System.Text;
using System.Xml;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.AirSync
{
	internal sealed class ProvisionCommandPhaseOne : ProvisionCommandPhaseBase
	{
		public ProvisionCommandPhaseOne(IProvisionCommandHost owningCommand) : base(owningCommand)
		{
		}

		internal override void Process(XmlNode provisionResponseNode)
		{
			this.ParseProvisionRequest();
			this.owningCommand.ProcessDeviceInformationSettings(this.deviceInformationNode, provisionResponseNode);
			if (this.ProcessPolicy(provisionResponseNode))
			{
				this.BuildResponse(provisionResponseNode);
			}
		}

		private static bool BuildEAS25And120Provisions(StringBuilder xml, int deviceVersion, IPolicyData mailboxPolicy)
		{
			bool flag = true;
			bool allowNonProvisionableDevices = mailboxPolicy.AllowNonProvisionableDevices;
			flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "DevicePasswordEnabled", mailboxPolicy.DevicePasswordEnabled, allowNonProvisionableDevices, deviceVersion, 20, true);
			flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "AlphanumericDevicePasswordRequired", mailboxPolicy.AlphanumericDevicePasswordRequired, allowNonProvisionableDevices, deviceVersion, 20, false);
			flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "PasswordRecoveryEnabled", mailboxPolicy.PasswordRecoveryEnabled, allowNonProvisionableDevices, deviceVersion, 20, false);
			flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "RequireStorageCardEncryption", mailboxPolicy.RequireStorageCardEncryption, allowNonProvisionableDevices, deviceVersion, 20, false);
			flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "AttachmentsEnabled", mailboxPolicy.AttachmentsEnabled, allowNonProvisionableDevices, deviceVersion, 20, true);
			flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "MinDevicePasswordLength", mailboxPolicy.MinDevicePasswordLength, allowNonProvisionableDevices, deviceVersion, 20, 0);
			object value = null;
			if (!mailboxPolicy.MaxInactivityTimeDeviceLock.IsUnlimited)
			{
				value = (int)mailboxPolicy.MaxInactivityTimeDeviceLock.Value.TotalSeconds;
			}
			flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "MaxInactivityTimeDeviceLock", value, allowNonProvisionableDevices, deviceVersion, 20, null);
			flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "MaxDevicePasswordFailedAttempts", mailboxPolicy.MaxDevicePasswordFailedAttempts, allowNonProvisionableDevices, deviceVersion, 20, null);
			value = null;
			if (!mailboxPolicy.MaxAttachmentSize.IsUnlimited)
			{
				value = mailboxPolicy.MaxAttachmentSize.Value.ToBytes();
			}
			flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "MaxAttachmentSize", value, allowNonProvisionableDevices, deviceVersion, 20, null);
			flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "AllowSimpleDevicePassword", mailboxPolicy.AllowSimpleDevicePassword, allowNonProvisionableDevices, deviceVersion, 20, true);
			value = null;
			if (!mailboxPolicy.DevicePasswordExpiration.IsUnlimited)
			{
				value = (int)mailboxPolicy.DevicePasswordExpiration.Value.TotalDays;
			}
			flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "DevicePasswordExpiration", value, allowNonProvisionableDevices, deviceVersion, 20, null);
			return flag & ProvisionCommandPhaseOne.AppendEASAttribute(xml, "DevicePasswordHistory", mailboxPolicy.DevicePasswordHistory, allowNonProvisionableDevices, deviceVersion, 20, 0);
		}

		internal static string BuildEASProvisionDoc(int deviceVersion, out bool policyIsCompatibleWithDevice, IPolicyData mailboxPolicy)
		{
			policyIsCompatibleWithDevice = true;
			StringBuilder stringBuilder = new StringBuilder(300);
			stringBuilder.Append("<eas-provisioningdoc xmlns=\"Provision:\">");
			policyIsCompatibleWithDevice = ProvisionCommandPhaseOne.BuildEAS25And120Provisions(stringBuilder, deviceVersion, mailboxPolicy);
			policyIsCompatibleWithDevice = (policyIsCompatibleWithDevice && ProvisionCommandPhaseOne.BuildEAS121Provisions(stringBuilder, deviceVersion, mailboxPolicy));
			stringBuilder.Append("</eas-provisioningdoc>");
			return stringBuilder.ToString();
		}

		private static bool AppendEASAttribute(StringBuilder xml, string name, object value, bool allowNonProvisionableDevices, int deviceVersion, int policyVersion, object ignoreValue)
		{
			if (deviceVersion < policyVersion)
			{
				return value == null || (ignoreValue != null && value.Equals(ignoreValue)) || allowNonProvisionableDevices;
			}
			if (value == null || (value is int? && (int?)value == null) || (value is Unlimited<int> && ((Unlimited<int>)value).IsUnlimited) || (value is Unlimited<TimeSpan> && ((Unlimited<TimeSpan>)value).IsUnlimited) || (value is Unlimited<ByteQuantifiedSize> && ((Unlimited<ByteQuantifiedSize>)value).IsUnlimited))
			{
				xml.AppendFormat("<{0} />", name);
			}
			else if (value is bool)
			{
				xml.AppendFormat("<{0}>{1}</{0}>", name, Convert.ToInt32(value, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture));
			}
			else
			{
				xml.AppendFormat("<{0}>{1}</{0}>", name, value.ToString());
			}
			return true;
		}

		private void ParseProvisionRequest()
		{
			this.deviceInformationNode = base.XmlRequest["DeviceInformation", "Settings:"];
			XmlNode xmlRequest = base.XmlRequest;
			XmlNode xmlNode = xmlRequest["Policies", "Provision:"];
			if (xmlNode != null)
			{
				XmlNode xmlNode2 = xmlNode["Policy", "Provision:"];
				XmlNode xmlNode3 = xmlNode2["PolicyType", "Provision:"];
				this.requestPolicyType = xmlNode3.InnerText;
			}
		}

		private static bool BuildEAS121Provisions(StringBuilder xml, int deviceVersion, IPolicyData mailboxPolicy)
		{
			bool flag = true;
			bool allowNonProvisionableDevices = mailboxPolicy.AllowNonProvisionableDevices;
			flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "AllowStorageCard", mailboxPolicy.AllowStorageCard, allowNonProvisionableDevices, deviceVersion, 121, true);
			flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "AllowCamera", mailboxPolicy.AllowCamera, allowNonProvisionableDevices, deviceVersion, 121, true);
			flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "RequireDeviceEncryption", mailboxPolicy.RequireDeviceEncryption, allowNonProvisionableDevices, deviceVersion, 121, false);
			flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "AllowUnsignedApplications", mailboxPolicy.AllowUnsignedApplications, allowNonProvisionableDevices, deviceVersion, 121, true);
			flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "AllowUnsignedInstallationPackages", mailboxPolicy.AllowUnsignedInstallationPackages, allowNonProvisionableDevices, deviceVersion, 121, true);
			flag &= (ProvisionCommandPhaseOne.AppendEASAttribute(xml, "MinDevicePasswordComplexCharacters", mailboxPolicy.MinDevicePasswordComplexCharacters, allowNonProvisionableDevices, deviceVersion, 121, 1) || !mailboxPolicy.AlphanumericDevicePasswordRequired);
			flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "AllowWiFi", mailboxPolicy.AllowWiFi, allowNonProvisionableDevices, deviceVersion, 121, true);
			flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "AllowTextMessaging", mailboxPolicy.AllowTextMessaging, allowNonProvisionableDevices, deviceVersion, 121, true);
			flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "AllowPOPIMAPEmail", mailboxPolicy.AllowPOPIMAPEmail, allowNonProvisionableDevices, deviceVersion, 121, true);
			flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "AllowBluetooth", (int)mailboxPolicy.AllowBluetooth, allowNonProvisionableDevices, deviceVersion, 121, 2);
			flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "AllowIrDA", mailboxPolicy.AllowIrDA, allowNonProvisionableDevices, deviceVersion, 121, true);
			flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "RequireManualSyncWhenRoaming", mailboxPolicy.RequireManualSyncWhenRoaming, allowNonProvisionableDevices, deviceVersion, 121, false);
			flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "AllowDesktopSync", mailboxPolicy.AllowDesktopSync, allowNonProvisionableDevices, deviceVersion, 121, true);
			flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "MaxCalendarAgeFilter", (int)mailboxPolicy.MaxCalendarAgeFilter, allowNonProvisionableDevices, deviceVersion, 121, 0);
			flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "AllowHTMLEmail", mailboxPolicy.AllowHTMLEmail, allowNonProvisionableDevices, deviceVersion, 121, true);
			flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "MaxEmailAgeFilter", (int)mailboxPolicy.MaxEmailAgeFilter, allowNonProvisionableDevices, deviceVersion, 121, 0);
			if (!mailboxPolicy.MaxEmailBodyTruncationSize.IsUnlimited)
			{
				flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "MaxEmailBodyTruncationSize", mailboxPolicy.MaxEmailBodyTruncationSize.Value, allowNonProvisionableDevices, deviceVersion, 121, null);
			}
			else
			{
				flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "MaxEmailBodyTruncationSize", -1, allowNonProvisionableDevices, deviceVersion, 121, -1);
			}
			if (!mailboxPolicy.MaxEmailHTMLBodyTruncationSize.IsUnlimited)
			{
				flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "MaxEmailHTMLBodyTruncationSize", mailboxPolicy.MaxEmailHTMLBodyTruncationSize.Value, allowNonProvisionableDevices, deviceVersion, 121, null);
			}
			else
			{
				flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "MaxEmailHTMLBodyTruncationSize", -1, allowNonProvisionableDevices, deviceVersion, 121, -1);
			}
			flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "RequireSignedSMIMEMessages", mailboxPolicy.RequireSignedSMIMEMessages, allowNonProvisionableDevices, deviceVersion, 121, false);
			flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "RequireEncryptedSMIMEMessages", mailboxPolicy.RequireEncryptedSMIMEMessages, allowNonProvisionableDevices, deviceVersion, 121, false);
			ProvisionCommandPhaseOne.AppendEASAttribute(xml, "RequireSignedSMIMEAlgorithm", (int)mailboxPolicy.RequireSignedSMIMEAlgorithm, allowNonProvisionableDevices, deviceVersion, 121, false);
			ProvisionCommandPhaseOne.AppendEASAttribute(xml, "RequireEncryptionSMIMEAlgorithm", (int)mailboxPolicy.RequireEncryptionSMIMEAlgorithm, allowNonProvisionableDevices, deviceVersion, 121, false);
			flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "AllowSMIMEEncryptionAlgorithmNegotiation", (int)mailboxPolicy.AllowSMIMEEncryptionAlgorithmNegotiation, allowNonProvisionableDevices, deviceVersion, 121, 2);
			flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "AllowSMIMESoftCerts", mailboxPolicy.AllowSMIMESoftCerts, allowNonProvisionableDevices, deviceVersion, 121, true);
			flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "AllowBrowser", mailboxPolicy.AllowBrowser, allowNonProvisionableDevices, deviceVersion, 121, true);
			flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "AllowConsumerEmail", mailboxPolicy.AllowConsumerEmail, allowNonProvisionableDevices, deviceVersion, 121, true);
			flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "AllowRemoteDesktop", mailboxPolicy.AllowRemoteDesktop, allowNonProvisionableDevices, deviceVersion, 121, true);
			flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "AllowInternetSharing", mailboxPolicy.AllowInternetSharing, allowNonProvisionableDevices, deviceVersion, 121, true);
			StringBuilder stringBuilder = new StringBuilder(300);
			MultiValuedProperty<string> unapprovedInROMApplicationList = mailboxPolicy.UnapprovedInROMApplicationList;
			if (unapprovedInROMApplicationList != null)
			{
				foreach (string str in unapprovedInROMApplicationList)
				{
					stringBuilder.AppendFormat("<{0}>{1}</{0}>", "ApplicationName", SecurityElement.Escape(str));
				}
			}
			flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "UnapprovedInROMApplicationList", stringBuilder.ToString(), allowNonProvisionableDevices, deviceVersion, 121, string.Empty);
			stringBuilder.Length = 0;
			ApprovedApplicationCollection approvedApplicationList = mailboxPolicy.ApprovedApplicationList;
			if (approvedApplicationList != null)
			{
				foreach (ApprovedApplication approvedApplication in approvedApplicationList)
				{
					stringBuilder.AppendFormat("<{0}>{1}</{0}>", "Hash", Convert.ToBase64String(HexStringConverter.GetBytes(approvedApplication.AppHash, false)));
				}
			}
			flag &= ProvisionCommandPhaseOne.AppendEASAttribute(xml, "ApprovedApplicationList", stringBuilder.ToString(), allowNonProvisionableDevices, deviceVersion, 121, string.Empty);
			return flag;
		}

		private static string BuildEASProvisionDoc(IPolicyData mailboxPolicy, int deviceVersion, out bool policyIsCompatibleWithDevice)
		{
			policyIsCompatibleWithDevice = true;
			if (mailboxPolicy == null)
			{
				return null;
			}
			return ProvisionCommandPhaseOne.BuildEASProvisionDoc(deviceVersion, out policyIsCompatibleWithDevice, mailboxPolicy);
		}

		private bool ProcessPolicy(XmlNode response)
		{
			uint? headerPolicyKey = this.owningCommand.HeaderPolicyKey;
			base.GlobalInfo.ProvisionSupported = true;
			bool flag;
			Command.DetectPolicyChange(this.owningCommand.PolicyData, base.GlobalInfo, this.owningCommand.ProtocolVersion, out flag);
			if (!flag)
			{
				this.owningCommand.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "DeviceNotFullyProvisionable");
				this.owningCommand.SetErrorResponse(HttpStatusCode.Forbidden, StatusCode.DeviceNotFullyProvisionable);
				base.GlobalInfo.DevicePolicyApplicationStatus = DevicePolicyApplicationStatus.NotApplied;
				return false;
			}
			if (this.requestPolicyType != null)
			{
				if (this.deviceInformationNode == null && this.owningCommand.ProtocolVersion >= 141)
				{
					throw new AirSyncPermanentException(StatusCode.DeviceInformationRequired, false)
					{
						ErrorStringForProtocolLogger = "DeviceInfoRequiredInProvision"
					};
				}
				if (string.Equals(this.requestPolicyType, "MS-WAP-Provisioning-XML", StringComparison.OrdinalIgnoreCase))
				{
					if (this.owningCommand.ProtocolVersion > 25)
					{
						this.owningCommand.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "CannotUseWAP");
						this.responseProvisionStatus = ProvisionCommand.ProvisionStatusCode.ProtocolError;
						base.GlobalInfo.DevicePolicyApplicationStatus = DevicePolicyApplicationStatus.NotApplied;
						return true;
					}
					if (base.GlobalInfo.PolicyKeyNeeded != 0U || (string.Equals(this.requestPolicyType, "MS-WAP-Provisioning-XML", StringComparison.OrdinalIgnoreCase) && headerPolicyKey != null && headerPolicyKey != 0U))
					{
						this.responsePolicyType = this.requestPolicyType;
						IPolicyData policyData = this.owningCommand.PolicyData;
						this.responsePolicyData = ProvisionCommandPhaseOne.BuildWAPProvisionDoc(policyData);
						this.responsePolicyStatus = ProvisionCommand.PolicyStatusCode.Success;
						this.responsePolicyKey = new uint?(base.GlobalInfo.PolicyKeyWaitingAck);
					}
					else
					{
						this.responsePolicyStatus = ProvisionCommand.PolicyStatusCode.NoPolicy;
						this.responsePolicyType = this.requestPolicyType;
					}
				}
				else if (string.Equals(this.requestPolicyType, "MS-EAS-Provisioning-WBXML", StringComparison.OrdinalIgnoreCase))
				{
					if (base.GlobalInfo.PolicyKeyNeeded != 0U)
					{
						this.responsePolicyType = this.requestPolicyType;
						IPolicyData policyData2 = this.owningCommand.PolicyData;
						this.responsePolicyData = ProvisionCommandPhaseOne.BuildEASProvisionDoc(policyData2, this.owningCommand.ProtocolVersion, out flag);
						this.responsePolicyStatus = ProvisionCommand.PolicyStatusCode.Success;
						this.responsePolicyKey = new uint?(base.GlobalInfo.PolicyKeyWaitingAck);
					}
					else if (base.GlobalInfo.PolicyKeyOnDevice != 0U || (string.Equals(this.requestPolicyType, "MS-EAS-Provisioning-WBXML", StringComparison.OrdinalIgnoreCase) && headerPolicyKey != null && headerPolicyKey != 0U))
					{
						this.responsePolicyType = this.requestPolicyType;
						this.responsePolicyData = "<eas-provisioningdoc xmlns=\"Provision:\"><DevicePasswordEnabled>0</DevicePasswordEnabled></eas-provisioningdoc>";
						this.responsePolicyStatus = ProvisionCommand.PolicyStatusCode.Success;
						this.responsePolicyKey = new uint?(base.GlobalInfo.PolicyKeyWaitingAck);
					}
					else
					{
						this.responsePolicyStatus = ProvisionCommand.PolicyStatusCode.NoPolicy;
						this.responsePolicyType = this.requestPolicyType;
					}
				}
				else
				{
					if (this.requestPolicyType.Length == 0)
					{
						base.GlobalInfo.DevicePolicyApplicationStatus = DevicePolicyApplicationStatus.NotApplied;
						this.responseProvisionStatus = ProvisionCommand.ProvisionStatusCode.ProtocolError;
						return true;
					}
					base.GlobalInfo.DevicePolicyApplicationStatus = DevicePolicyApplicationStatus.NotApplied;
					this.responsePolicyType = this.requestPolicyType;
					this.responsePolicyStatus = ProvisionCommand.PolicyStatusCode.UnknownPolicyType;
					this.responseProvisionStatus = ProvisionCommand.ProvisionStatusCode.Success;
					return true;
				}
			}
			this.responseProvisionStatus = ProvisionCommand.ProvisionStatusCode.Success;
			return true;
		}

		private void BuildResponse(XmlNode provisionNode)
		{
			if (this.responseProvisionStatus == ProvisionCommand.ProvisionStatusCode.NotPresent)
			{
				AirSyncDiagnostics.Assert(this.responseProvisionStatus != ProvisionCommand.ProvisionStatusCode.NotPresent, "[ProvisionCommandPhaseOne.BuildResponse] ReponseProvisionStatus should NOT be NotPresent.", new object[0]);
			}
			XmlNode xmlNode = this.owningCommand.XmlResponse.CreateElement("Status", "Provision:");
			XmlNode xmlNode2 = xmlNode;
			int num = (int)this.responseProvisionStatus;
			xmlNode2.InnerText = num.ToString(CultureInfo.InvariantCulture);
			provisionNode.AppendChild(xmlNode);
			this.owningCommand.ProtocolLogger.SetValue(ProtocolLoggerData.StatusCode, (int)this.responseProvisionStatus);
			if (this.responseProvisionStatus == ProvisionCommand.ProvisionStatusCode.Success && this.responsePolicyType != null)
			{
				XmlNode xmlNode3 = this.owningCommand.XmlResponse.CreateElement("Policies", "Provision:");
				provisionNode.AppendChild(xmlNode3);
				XmlNode xmlNode4 = this.owningCommand.XmlResponse.CreateElement("Policy", "Provision:");
				xmlNode3.AppendChild(xmlNode4);
				XmlNode xmlNode5 = this.owningCommand.XmlResponse.CreateElement("PolicyType", "Provision:");
				xmlNode5.InnerText = this.responsePolicyType;
				xmlNode4.AppendChild(xmlNode5);
				AirSyncDiagnostics.Assert(this.responsePolicyStatus != ProvisionCommand.PolicyStatusCode.NotPresent);
				XmlNode xmlNode6 = this.owningCommand.XmlResponse.CreateElement("Status", "Provision:");
				XmlNode xmlNode7 = xmlNode6;
				int num2 = (int)this.responsePolicyStatus;
				xmlNode7.InnerText = num2.ToString(CultureInfo.InvariantCulture);
				xmlNode4.AppendChild(xmlNode6);
				if (this.responsePolicyKey != null)
				{
					XmlNode xmlNode8 = this.owningCommand.XmlResponse.CreateElement("PolicyKey", "Provision:");
					xmlNode8.InnerText = this.responsePolicyKey.ToString();
					xmlNode4.AppendChild(xmlNode8);
				}
				if (this.responsePolicyData != null)
				{
					XmlNode xmlNode9 = this.owningCommand.XmlResponse.CreateElement("Data", "Provision:");
					xmlNode4.AppendChild(xmlNode9);
					if (string.Equals(this.responsePolicyType, "MS-WAP-Provisioning-XML", StringComparison.OrdinalIgnoreCase))
					{
						xmlNode9.InnerText = this.responsePolicyData;
					}
					else if (string.Equals(this.responsePolicyType, "MS-EAS-Provisioning-WBXML", StringComparison.OrdinalIgnoreCase))
					{
						xmlNode9.InnerXml = this.responsePolicyData;
					}
					else
					{
						AirSyncDiagnostics.Assert(false, "Unknown policy type", new object[0]);
					}
				}
			}
			base.GenerateRemoteWipeIfNeeded(provisionNode);
		}

		private static string BuildWAPProvisionDoc(IPolicyData mailboxPolicy)
		{
			StringBuilder stringBuilder = new StringBuilder(300);
			stringBuilder.Append("<wap-provisioningdoc>");
			stringBuilder.Append("<characteristic type=\"SecurityPolicy\">");
			bool flag = false;
			bool flag2 = false;
			if (mailboxPolicy != null)
			{
				flag = mailboxPolicy.DevicePasswordEnabled;
				flag2 = !mailboxPolicy.MaxDevicePasswordFailedAttempts.IsUnlimited;
				bool requireStorageCardEncryption = mailboxPolicy.RequireStorageCardEncryption;
			}
			stringBuilder.AppendFormat("<parm name=\"4131\" value=\"{0}\"/>", flag ? 0 : 1);
			stringBuilder.AppendFormat("<parm name=\"4133\" value=\"{0}\"/>", (flag && flag2) ? 0 : 1);
			stringBuilder.Append("</characteristic>");
			if (flag)
			{
				stringBuilder.Append("<characteristic type=\"Registry\">");
				bool flag3 = false;
				int num = 0;
				if (!mailboxPolicy.MaxInactivityTimeDeviceLock.IsUnlimited)
				{
					flag3 = true;
					num = (int)mailboxPolicy.MaxInactivityTimeDeviceLock.Value.TotalMinutes;
					if (mailboxPolicy.MaxInactivityTimeDeviceLock.Value.Seconds > 0)
					{
						num++;
					}
					if (num < 1)
					{
						num = 1;
					}
					if (num > 9999)
					{
						num = 9999;
					}
				}
				stringBuilder.Append("<characteristic type=\"HKLM\\Comm\\Security\\Policy\\LASSD\\AE\\{50C13377-C66D-400C-889E-C316FC4AB374}\">");
				stringBuilder.AppendFormat("<parm name=\"AEFrequencyType\" value=\"{0}\"/>", flag3 ? 1 : 0);
				stringBuilder.AppendFormat("<parm name=\"AEFrequencyValue\" value=\"{0}\"/>", num);
				stringBuilder.Append("</characteristic>");
				int num2 = 0;
				int num3 = -1;
				if (!mailboxPolicy.MaxDevicePasswordFailedAttempts.IsUnlimited)
				{
					num2 = mailboxPolicy.MaxDevicePasswordFailedAttempts.Value;
					if (num2 < 2)
					{
						num2 = 2;
					}
					num3 = num2 / 2;
					if (num3 > 8)
					{
						num3 = 8;
					}
				}
				stringBuilder.Append("<characteristic type=\"HKLM\\Comm\\Security\\Policy\\LASSD\">");
				stringBuilder.AppendFormat("<parm name=\"DeviceWipeThreshold\" value=\"{0}\"/>", num2);
				stringBuilder.Append("</characteristic>");
				stringBuilder.Append("<characteristic type=\"HKLM\\Comm\\Security\\Policy\\LASSD\">");
				stringBuilder.AppendFormat("<parm name=\"CodewordFrequency\" value=\"{0}\"/>", num3);
				stringBuilder.Append("</characteristic>");
				int num4 = 1;
				if (mailboxPolicy.MinDevicePasswordLength != null)
				{
					num4 = mailboxPolicy.MinDevicePasswordLength.Value;
					if (num4 < 1)
					{
						num4 = 1;
					}
				}
				stringBuilder.Append("<characteristic type=\"HKLM\\Comm\\Security\\Policy\\LASSD\\LAP\\lap_pw\">");
				stringBuilder.AppendFormat("<parm name=\"MinimumPasswordLength\" value=\"{0}\"/>", num4);
				stringBuilder.Append("</characteristic>");
				stringBuilder.Append("<characteristic type=\"HKLM\\Comm\\Security\\Policy\\LASSD\\LAP\\lap_pw\">");
				stringBuilder.AppendFormat("<parm name=\"PasswordComplexity\" value=\"{0}\"/>", mailboxPolicy.AlphanumericDevicePasswordRequired ? 0 : 2);
				stringBuilder.Append("</characteristic>");
				stringBuilder.Append("</characteristic>");
			}
			stringBuilder.Append("</wap-provisioningdoc>");
			return stringBuilder.ToString();
		}

		private const string EasUndoPolicy = "<eas-provisioningdoc xmlns=\"Provision:\"><DevicePasswordEnabled>0</DevicePasswordEnabled></eas-provisioningdoc>";

		private XmlNode deviceInformationNode;

		private string requestPolicyType;

		private string responsePolicyData;

		private uint? responsePolicyKey;

		private ProvisionCommand.PolicyStatusCode responsePolicyStatus;

		private string responsePolicyType;

		private ProvisionCommand.ProvisionStatusCode responseProvisionStatus;
	}
}
