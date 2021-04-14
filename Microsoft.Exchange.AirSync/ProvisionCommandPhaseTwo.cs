using System;
using System.Globalization;
using System.Net;
using System.Xml;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal sealed class ProvisionCommandPhaseTwo : ProvisionCommandPhaseBase
	{
		public ProvisionCommandPhaseTwo(IProvisionCommandHost owningCommand) : base(owningCommand)
		{
		}

		internal override void Process(XmlNode provisionResponseNode)
		{
			ProvisionCommandPhaseTwo.AcknowledgementType acknowledgementType = this.ParseProvisionRequest();
			if ((acknowledgementType & ProvisionCommandPhaseTwo.AcknowledgementType.ForceRemoteWipe) == ProvisionCommandPhaseTwo.AcknowledgementType.ForceRemoteWipe)
			{
				base.GenerateRemoteWipeResponse(provisionResponseNode, ProvisionCommand.ProvisionStatusCode.ProtocolError);
				return;
			}
			bool flag = (acknowledgementType & ProvisionCommandPhaseTwo.AcknowledgementType.RemoteWipe) == ProvisionCommandPhaseTwo.AcknowledgementType.RemoteWipe;
			bool flag2 = (acknowledgementType & ProvisionCommandPhaseTwo.AcknowledgementType.Policy) == ProvisionCommandPhaseTwo.AcknowledgementType.Policy;
			if (flag)
			{
				this.ProcessRemoteWipeAck(provisionResponseNode);
				if (this.remoteWipeServerStatusCode != ProvisionCommandPhaseTwo.RemoteWipeServerStatusCode.Success)
				{
					base.GenerateRemoteWipeResponse(provisionResponseNode, ProvisionCommand.ProvisionStatusCode.ProtocolError);
					return;
				}
				if (!flag2)
				{
					XmlNode xmlNode = this.owningCommand.XmlResponse.CreateElement("Status", "Provision:");
					provisionResponseNode.AppendChild(xmlNode);
					xmlNode.InnerText = 1.ToString(CultureInfo.InvariantCulture);
					return;
				}
			}
			if (flag2)
			{
				this.ProcessPolicy(provisionResponseNode);
			}
		}

		private ProvisionCommandPhaseTwo.AcknowledgementType ParseProvisionRequest()
		{
			ProvisionCommandPhaseTwo.AcknowledgementType acknowledgementType = ProvisionCommandPhaseTwo.AcknowledgementType.None;
			XmlNode xmlRequest = base.XmlRequest;
			uint num = 0U;
			XmlNode xmlNode = xmlRequest["RemoteWipe", "Provision:"];
			if (xmlNode != null)
			{
				if (!base.RemoteWipeRequired)
				{
					throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, false)
					{
						ErrorStringForProtocolLogger = "RemoteWipeWasNotRequested"
					};
				}
				XmlNode xmlNode2 = xmlNode["Status", "Provision:"];
				if (xmlNode2 != null)
				{
					if (!uint.TryParse(xmlNode2.InnerText, out num) || num < 1U || num > 2U)
					{
						this.requestRemoteWipeStatus = ProvisionCommandPhaseTwo.RemoteWipeStatusCodeFromClient.Invalid;
					}
					else
					{
						this.requestRemoteWipeStatus = (ProvisionCommandPhaseTwo.RemoteWipeStatusCodeFromClient)num;
					}
				}
				acknowledgementType = ProvisionCommandPhaseTwo.AcknowledgementType.RemoteWipe;
			}
			if (base.RemoteWipeRequired && this.requestRemoteWipeStatus == ProvisionCommandPhaseTwo.RemoteWipeStatusCodeFromClient.NotPresent)
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "[ProvisionCommandPhaseTwo.ParseProvisionRequest] Client responded to policy ack, but there is a remoteWipe request pending.");
				return ProvisionCommandPhaseTwo.AcknowledgementType.ForceRemoteWipe;
			}
			XmlNode xmlNode3 = xmlRequest["Policies", "Provision:"];
			if (xmlNode3 != null)
			{
				XmlNode xmlNode4 = xmlNode3["Policy", "Provision:"];
				XmlNode xmlNode5 = xmlNode4["PolicyKey", "Provision:"];
				XmlNode xmlNode6 = xmlNode4["Status", "Provision:"];
				if (xmlNode5 != null && uint.TryParse(xmlNode5.InnerText, out num))
				{
					this.requestPolicyKey = new uint?(num);
				}
				XmlNode xmlNode7 = xmlNode4["PolicyType", "Provision:"];
				this.requestPolicyType = xmlNode7.InnerText;
				if (xmlNode6 != null)
				{
					if (!uint.TryParse(xmlNode6.InnerText, out num) || num < 1U || num > 4U)
					{
						this.requestPolicyStatus = ProvisionCommandPhaseTwo.PolicyStatusCodeFromClient.Invalid;
					}
					else
					{
						this.requestPolicyStatus = (ProvisionCommandPhaseTwo.PolicyStatusCodeFromClient)num;
					}
					this.owningCommand.ProtocolLogger.SetValue(ProtocolLoggerData.PolicyAckStatus, (int)this.requestPolicyStatus);
				}
				acknowledgementType |= ProvisionCommandPhaseTwo.AcknowledgementType.Policy;
			}
			return acknowledgementType;
		}

		private void ProcessPolicy(XmlNode response)
		{
			uint? headerPolicyKey = this.owningCommand.HeaderPolicyKey;
			this.owningCommand.GlobalInfo.ProvisionSupported = true;
			bool flag;
			Command.DetectPolicyChange(this.owningCommand.PolicyData, this.owningCommand.GlobalInfo, this.owningCommand.ProtocolVersion, out flag);
			if (!flag)
			{
				this.owningCommand.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "DeviceNotFullyProvisionable");
				this.owningCommand.SetErrorResponse(HttpStatusCode.Forbidden, StatusCode.DeviceNotFullyProvisionable);
				this.owningCommand.GlobalInfo.DevicePolicyApplicationStatus = DevicePolicyApplicationStatus.NotApplied;
				return;
			}
			if (!string.IsNullOrEmpty(this.requestPolicyType) && this.requestPolicyStatus != ProvisionCommandPhaseTwo.PolicyStatusCodeFromClient.NotPresent && this.requestPolicyKey != null)
			{
				if (!ProvisionCommandPhaseBase.IsValidPolicyType(this.requestPolicyType))
				{
					this.owningCommand.GlobalInfo.DevicePolicyApplicationStatus = DevicePolicyApplicationStatus.NotApplied;
					this.responsePolicyType = this.requestPolicyType;
					this.responsePolicyStatus = ProvisionCommand.PolicyStatusCode.UnknownPolicyType;
				}
				else
				{
					if (this.requestPolicyStatus < ProvisionCommandPhaseTwo.PolicyStatusCodeFromClient.MinValue || this.requestPolicyStatus > ProvisionCommandPhaseTwo.PolicyStatusCodeFromClient.AllowExternalDeviceManagement)
					{
						this.owningCommand.GlobalInfo.DevicePolicyApplicationStatus = DevicePolicyApplicationStatus.NotApplied;
						this.responseProvisionStatus = ProvisionCommand.ProvisionStatusCode.ProtocolError;
						this.BuildPolicyResponse(response);
						return;
					}
					if (this.requestPolicyStatus >= ProvisionCommandPhaseTwo.PolicyStatusCodeFromClient.AllowExternalDeviceManagement && this.owningCommand.ProtocolVersion < 121)
					{
						this.responseProvisionStatus = ProvisionCommand.ProvisionStatusCode.ProtocolError;
						this.owningCommand.GlobalInfo.DevicePolicyApplicationStatus = DevicePolicyApplicationStatus.NotApplied;
						this.BuildPolicyResponse(response);
						return;
					}
					if (this.requestPolicyKey == null)
					{
						this.owningCommand.GlobalInfo.DevicePolicyApplicationStatus = DevicePolicyApplicationStatus.NotApplied;
						this.responseProvisionStatus = ProvisionCommand.ProvisionStatusCode.ProtocolError;
						this.BuildPolicyResponse(response);
						return;
					}
					if (this.requestPolicyKey == this.owningCommand.GlobalInfo.PolicyKeyWaitingAck)
					{
						this.responsePolicyType = this.requestPolicyType;
						this.responsePolicyStatus = ProvisionCommand.PolicyStatusCode.Success;
						this.owningCommand.GlobalInfo.DevicePolicyApplicationStatus = ProvisionCommandPhaseTwo.MapPolicyStatusCodeFromClientToDevicePolicyApplicationStatus(this.requestPolicyStatus);
						IPolicyData policyData = this.owningCommand.PolicyData;
						if (policyData != null)
						{
							if (this.requestPolicyStatus == ProvisionCommandPhaseTwo.PolicyStatusCodeFromClient.AllowExternalDeviceManagement && !policyData.AllowExternalDeviceManagement)
							{
								this.owningCommand.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "ExternallyManagedDevicesNotAllowed");
								this.owningCommand.SetErrorResponse(HttpStatusCode.Forbidden, StatusCode.ExternallyManagedDevicesNotAllowed);
								return;
							}
							if (this.requestPolicyStatus == ProvisionCommandPhaseTwo.PolicyStatusCodeFromClient.MinValue || this.requestPolicyStatus == ProvisionCommandPhaseTwo.PolicyStatusCodeFromClient.AllowExternalDeviceManagement || (this.requestPolicyStatus == ProvisionCommandPhaseTwo.PolicyStatusCodeFromClient.PartialError && policyData.AllowNonProvisionableDevices))
							{
								this.responsePolicyKey = new uint?(this.owningCommand.GlobalInfo.PolicyKeyNeeded);
								this.owningCommand.GlobalInfo.LastPolicyTime = new ExDateTime?(ExDateTime.UtcNow);
								this.owningCommand.GlobalInfo.PolicyKeyOnDevice = this.owningCommand.GlobalInfo.PolicyKeyNeeded;
							}
							else if (this.requestPolicyStatus == ProvisionCommandPhaseTwo.PolicyStatusCodeFromClient.PartialError && !policyData.AllowNonProvisionableDevices)
							{
								this.owningCommand.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "DevicePartiallyProvisionableStrictPolicy");
								this.owningCommand.SetErrorResponse(HttpStatusCode.Forbidden, StatusCode.DeviceNotFullyProvisionable);
								this.owningCommand.GlobalInfo.DevicePolicyApplicationStatus = DevicePolicyApplicationStatus.NotApplied;
								return;
							}
						}
						else
						{
							this.responsePolicyKey = new uint?(0U);
							this.owningCommand.GlobalInfo.PolicyKeyOnDevice = 0U;
							this.owningCommand.GlobalInfo.LastPolicyTime = new ExDateTime?(ExDateTime.UtcNow);
						}
					}
					else
					{
						this.responsePolicyStatus = ProvisionCommand.PolicyStatusCode.PolicyKeyMismatch;
						this.responsePolicyType = this.requestPolicyType;
					}
				}
				this.responseProvisionStatus = ProvisionCommand.ProvisionStatusCode.Success;
				this.BuildPolicyResponse(response);
				return;
			}
			this.owningCommand.GlobalInfo.DevicePolicyApplicationStatus = DevicePolicyApplicationStatus.NotApplied;
			this.responseProvisionStatus = ProvisionCommand.ProvisionStatusCode.ProtocolError;
			this.BuildPolicyResponse(response);
		}

		private void BuildPolicyResponse(XmlNode provisionNode)
		{
			AirSyncDiagnostics.Assert(this.responseProvisionStatus != ProvisionCommand.ProvisionStatusCode.NotPresent);
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
			}
		}

		private void ProcessRemoteWipeAck(XmlNode response)
		{
			if (this.requestRemoteWipeStatus == ProvisionCommandPhaseTwo.RemoteWipeStatusCodeFromClient.MinValue)
			{
				this.owningCommand.GlobalInfo.RemoteWipeAckTime = new ExDateTime?(ExDateTime.UtcNow);
				this.remoteWipeServerStatusCode = ProvisionCommandPhaseTwo.RemoteWipeServerStatusCode.Success;
				this.owningCommand.SendRemoteWipeConfirmationMessage(ExDateTime.Now);
				return;
			}
			AirSyncDiagnostics.TraceDebug<ProvisionCommandPhaseTwo.RemoteWipeStatusCodeFromClient>(ExTraceGlobals.RequestsTracer, this, "[ProvisionCommandPhaseTwo.ProcessRemoteWipeAck] Client returned {0} for RemoteWipe status.  Failing Provision.", this.requestRemoteWipeStatus);
			this.remoteWipeServerStatusCode = ProvisionCommandPhaseTwo.RemoteWipeServerStatusCode.Failure;
		}

		private static DevicePolicyApplicationStatus MapPolicyStatusCodeFromClientToDevicePolicyApplicationStatus(ProvisionCommandPhaseTwo.PolicyStatusCodeFromClient statusCode)
		{
			switch (statusCode)
			{
			case ProvisionCommandPhaseTwo.PolicyStatusCodeFromClient.MinValue:
				return DevicePolicyApplicationStatus.AppliedInFull;
			case ProvisionCommandPhaseTwo.PolicyStatusCodeFromClient.PartialError:
				return DevicePolicyApplicationStatus.PartiallyApplied;
			case ProvisionCommandPhaseTwo.PolicyStatusCodeFromClient.PolicyIgnored:
				return DevicePolicyApplicationStatus.NotApplied;
			case ProvisionCommandPhaseTwo.PolicyStatusCodeFromClient.AllowExternalDeviceManagement:
				return DevicePolicyApplicationStatus.ExternallyManaged;
			default:
				return DevicePolicyApplicationStatus.Unknown;
			}
		}

		private uint? responsePolicyKey;

		private ProvisionCommandPhaseTwo.RemoteWipeServerStatusCode remoteWipeServerStatusCode;

		private ProvisionCommand.PolicyStatusCode responsePolicyStatus;

		private string responsePolicyType;

		private ProvisionCommand.ProvisionStatusCode responseProvisionStatus;

		private string requestPolicyType;

		private uint? requestPolicyKey;

		private ProvisionCommandPhaseTwo.PolicyStatusCodeFromClient requestPolicyStatus;

		private ProvisionCommandPhaseTwo.RemoteWipeStatusCodeFromClient requestRemoteWipeStatus;

		[Flags]
		private enum AcknowledgementType
		{
			None = 0,
			Policy = 1,
			RemoteWipe = 2,
			ForceRemoteWipe = 4
		}

		private enum PolicyStatusCodeFromClient
		{
			NotPresent,
			MinValue,
			Success = 1,
			PartialError,
			PolicyIgnored,
			AllowExternalDeviceManagement,
			MaxValue = 4,
			Invalid
		}

		private enum RemoteWipeStatusCodeFromClient
		{
			NotPresent,
			MinValue,
			Success = 1,
			Failure,
			MaxValue = 2,
			Invalid
		}

		private enum RemoteWipeServerStatusCode
		{
			NotPresent,
			Success,
			Failure
		}
	}
}
