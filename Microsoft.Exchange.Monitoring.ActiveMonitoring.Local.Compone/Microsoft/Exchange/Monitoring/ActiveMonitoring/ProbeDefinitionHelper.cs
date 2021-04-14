using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring
{
	internal abstract class ProbeDefinitionHelper : DefinitionHelperBase
	{
		internal string Account { get; set; }

		internal string AccountPassword { get; set; }

		internal string AccountDisplayName { get; set; }

		internal string Endpoint { get; set; }

		internal string SecondaryAccount { get; set; }

		internal string SecondaryAccountPassword { get; set; }

		internal string SecondaryAccountDisplayName { get; set; }

		internal string SecondaryEndpoint { get; set; }

		internal int Version { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(base.ToString());
			stringBuilder.AppendLine("Account: " + this.Account);
			stringBuilder.AppendLine("AccountPassword: " + this.AccountPassword);
			stringBuilder.AppendLine("AccountDisplayName: " + this.AccountDisplayName);
			stringBuilder.AppendLine("Endpoint: " + this.Endpoint);
			stringBuilder.AppendLine("SecondaryAccount: " + this.SecondaryAccount);
			stringBuilder.AppendLine("SecondaryAccountPassword: " + this.SecondaryAccountPassword);
			stringBuilder.AppendLine("SecondaryAccountDisplayName: " + this.SecondaryAccountDisplayName);
			stringBuilder.AppendLine("SecondaryEndpoint: " + this.SecondaryEndpoint);
			return stringBuilder.ToString();
		}

		internal abstract List<ProbeDefinition> CreateDefinition();

		internal override void ReadDiscoveryXml()
		{
			base.ReadDiscoveryXml();
			this.Account = base.GetOptionalXmlAttribute<string>("Account", string.Empty);
			this.AccountPassword = base.GetOptionalXmlAttribute<string>("AccountPassword", string.Empty);
			this.AccountDisplayName = base.GetOptionalXmlAttribute<string>("AccountDisplayName", string.Empty);
			this.Endpoint = base.GetOptionalXmlAttribute<string>("Endpoint", string.Empty);
			this.SecondaryAccount = base.GetOptionalXmlAttribute<string>("SecondaryAccount", string.Empty);
			this.SecondaryAccountPassword = base.GetOptionalXmlAttribute<string>("SecondaryAccountPassword", string.Empty);
			this.SecondaryAccountDisplayName = base.GetOptionalXmlAttribute<string>("SecondaryAccountDisplayName", string.Empty);
			this.SecondaryEndpoint = base.GetOptionalXmlAttribute<string>("SecondaryEndpoint", string.Empty);
		}

		internal override string ToString(WorkDefinition workItem)
		{
			ProbeDefinition probeDefinition = (ProbeDefinition)workItem;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(base.ToString(workItem));
			stringBuilder.AppendLine("Account: " + probeDefinition.Account);
			stringBuilder.AppendLine("AccountPassword: " + probeDefinition.AccountPassword);
			stringBuilder.AppendLine("AccountDisplayName: " + probeDefinition.AccountDisplayName);
			stringBuilder.AppendLine("Endpoint: " + probeDefinition.Endpoint);
			stringBuilder.AppendLine("SecondaryAccount: " + probeDefinition.SecondaryAccount);
			stringBuilder.AppendLine("SecondaryAccountPassword: " + probeDefinition.SecondaryAccountPassword);
			stringBuilder.AppendLine("SecondaryAccountDisplayName: " + probeDefinition.SecondaryAccountDisplayName);
			stringBuilder.AppendLine("SecondaryEndpoint: " + probeDefinition.SecondaryEndpoint);
			return stringBuilder.ToString();
		}

		protected ProbeDefinition CreateProbeDefinition()
		{
			ProbeDefinition probeDefinition = new ProbeDefinition();
			base.CreateBaseWorkDefinition(probeDefinition);
			probeDefinition.Account = this.Account;
			probeDefinition.AccountPassword = this.AccountPassword;
			probeDefinition.AccountDisplayName = this.AccountDisplayName;
			probeDefinition.Endpoint = this.Endpoint;
			probeDefinition.SecondaryAccount = this.SecondaryAccount;
			probeDefinition.SecondaryAccountPassword = this.SecondaryAccountPassword;
			probeDefinition.SecondaryAccountDisplayName = this.SecondaryAccountDisplayName;
			probeDefinition.SecondaryEndpoint = this.SecondaryEndpoint;
			return probeDefinition;
		}

		protected ProbeDefinition CreateProbeDefinition(XmlNode extensionNode)
		{
			ProbeDefinition probeDefinition = this.CreateProbeDefinition();
			if (extensionNode != null)
			{
				probeDefinition.ExtensionAttributes = extensionNode.OuterXml;
				probeDefinition.ParseExtensionAttributes(false);
			}
			return probeDefinition;
		}
	}
}
