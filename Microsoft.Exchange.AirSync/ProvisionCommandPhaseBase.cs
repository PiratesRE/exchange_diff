using System;
using System.Xml;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal abstract class ProvisionCommandPhaseBase
	{
		private static XmlNamespaceManager CreateNamespaceManager()
		{
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
			xmlNamespaceManager.AddNamespace("p", "Provision:");
			xmlNamespaceManager.AddNamespace("s", "Settings:");
			return xmlNamespaceManager;
		}

		internal static bool IsValidPolicyType(string requestedPolicyType)
		{
			return string.Equals(requestedPolicyType, "MS-WAP-Provisioning-XML", StringComparison.OrdinalIgnoreCase) || string.Equals(requestedPolicyType, "MS-EAS-Provisioning-WBXML", StringComparison.OrdinalIgnoreCase);
		}

		public ProvisionCommandPhaseBase(IProvisionCommandHost owningCommand)
		{
			if (owningCommand == null)
			{
				throw new ArgumentNullException("owningCommand");
			}
			this.owningCommand = owningCommand;
		}

		protected XmlNode XmlRequest
		{
			get
			{
				return this.owningCommand.XmlRequest;
			}
		}

		protected XmlDocument XmlResponse
		{
			get
			{
				return this.owningCommand.XmlResponse;
			}
		}

		protected IGlobalInfo GlobalInfo
		{
			get
			{
				return this.owningCommand.GlobalInfo;
			}
		}

		internal static ProvisionCommandPhaseBase.ProvisionPhase DetermineCallPhase(XmlNode requestNode)
		{
			XmlNode xmlNode = requestNode.SelectSingleNode("/p:Provision/p:Policies/p:Policy", ProvisionCommandPhaseBase.namespaceManager);
			if (xmlNode != null)
			{
				XmlNode xmlNode2 = xmlNode["PolicyKey", "Provision:"];
				XmlNode xmlNode3 = xmlNode["Status", "Provision:"];
				if (xmlNode2 != null || xmlNode3 != null)
				{
					return ProvisionCommandPhaseBase.ProvisionPhase.PhaseTwo;
				}
			}
			xmlNode = requestNode.SelectSingleNode("/p:Provision/p:RemoteWipe/p:Status", ProvisionCommandPhaseBase.namespaceManager);
			if (xmlNode != null)
			{
				return ProvisionCommandPhaseBase.ProvisionPhase.PhaseTwo;
			}
			return ProvisionCommandPhaseBase.ProvisionPhase.PhaseOne;
		}

		internal abstract void Process(XmlNode provisionResponseNode);

		protected void GenerateRemoteWipeResponse(XmlNode provisionResponseNode, ProvisionCommand.ProvisionStatusCode statusCode)
		{
			if (provisionResponseNode["Status", "Provision:"] == null)
			{
				XmlNode xmlNode = this.owningCommand.XmlResponse.CreateElement("Status", "Provision:");
				XmlNode xmlNode2 = xmlNode;
				int num = (int)statusCode;
				xmlNode2.InnerText = num.ToString();
				provisionResponseNode.AppendChild(xmlNode);
			}
			XmlNode newChild = this.owningCommand.XmlResponse.CreateElement("RemoteWipe", "Provision:");
			provisionResponseNode.AppendChild(newChild);
		}

		protected bool RemoteWipeRequired
		{
			get
			{
				ExDateTime? remoteWipeRequestedTime = this.owningCommand.GlobalInfo.RemoteWipeRequestedTime;
				if (remoteWipeRequestedTime != null)
				{
					ExDateTime? remoteWipeAckTime = this.owningCommand.GlobalInfo.RemoteWipeAckTime;
					return remoteWipeAckTime == null || !(remoteWipeAckTime.Value >= remoteWipeRequestedTime.Value);
				}
				return false;
			}
		}

		protected bool GenerateRemoteWipeIfNeeded(XmlNode provisionResponseNode)
		{
			if (this.RemoteWipeRequired)
			{
				ExTraceGlobals.RequestsTracer.TraceDebug<ExDateTime?>((long)this.GetHashCode(), "[ProvisionCommandPhaseBase.GenerateRemoteWipeIfNeeded] WipeRequestTime '{0}' indicates we need to return a remote wipe response.", this.owningCommand.GlobalInfo.RemoteWipeRequestedTime);
				ExDateTime? remoteWipeSentTime = this.owningCommand.GlobalInfo.RemoteWipeSentTime;
				if (remoteWipeSentTime == null || remoteWipeSentTime.Value < this.owningCommand.GlobalInfo.RemoteWipeRequestedTime.Value)
				{
					this.owningCommand.GlobalInfo.RemoteWipeSentTime = new ExDateTime?(ExDateTime.UtcNow);
					this.owningCommand.ResetMobileServiceSelector();
				}
				this.GenerateRemoteWipeResponse(provisionResponseNode, ProvisionCommand.ProvisionStatusCode.Success);
				return true;
			}
			return false;
		}

		internal static XmlNamespaceManager namespaceManager = ProvisionCommandPhaseBase.CreateNamespaceManager();

		protected IProvisionCommandHost owningCommand;

		internal enum ProvisionPhase
		{
			Unknown,
			PhaseOne,
			PhaseTwo
		}
	}
}
