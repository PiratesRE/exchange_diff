using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.EhfHybridMailflow
{
	[Cmdlet("Set", "HybridMailflow", SupportsShouldProcess = true)]
	public sealed class SetHybridMailflow : HybridMailflowTaskBase
	{
		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
		public SmtpDomainWithSubdomains[] OutboundDomains
		{
			get
			{
				return (SmtpDomainWithSubdomains[])base.Fields["OutboundDomains"];
			}
			set
			{
				base.Fields["OutboundDomains"] = value;
			}
		}

		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
		public IPRange[] InboundIPs
		{
			get
			{
				return (IPRange[])base.Fields["InboundIPs"];
			}
			set
			{
				base.Fields["InboundIPs"] = value;
			}
		}

		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
		public Fqdn OnPremisesFQDN
		{
			get
			{
				return (Fqdn)base.Fields["OnPremisesFQDN"];
			}
			set
			{
				base.Fields["OnPremisesFQDN"] = value;
			}
		}

		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
		public string CertificateSubject
		{
			get
			{
				return (string)base.Fields["CertificateSubject"];
			}
			set
			{
				base.Fields["CertificateSubject"] = value;
			}
		}

		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
		public bool? SecureMailEnabled
		{
			get
			{
				return (bool?)base.Fields["SecureMailEnabled"];
			}
			set
			{
				base.Fields["SecureMailEnabled"] = value;
			}
		}

		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
		public bool? CentralizedTransportEnabled
		{
			get
			{
				return (bool?)base.Fields["CentralizedTransportEnabled"];
			}
			set
			{
				base.Fields["CentralizedTransportEnabled"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetHybridMailflow;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
		}

		protected override void InternalProcessRecord()
		{
			this.SetHybridMailflowSettingsWithConnectors();
			base.InternalProcessRecord();
		}

		private void SetHybridMailflowSettingsWithConnectors()
		{
			bool flag = null == base.OriginalInboundConnector;
			List<CommandParameter> inboundConnectorParameters = this.GetInboundConnectorParameters(flag);
			if (inboundConnectorParameters.Count > 1)
			{
				base.InvokeConnectorCmdlet(flag ? "New-InboundConnector" : "Set-InboundConnector", inboundConnectorParameters);
			}
			bool flag2 = null == base.OriginalOutboundConnector;
			List<CommandParameter> outboundConnectorParameters = this.GetOutboundConnectorParameters(flag2);
			if (outboundConnectorParameters.Count > 1)
			{
				base.InvokeConnectorCmdlet(flag2 ? "New-OutboundConnector" : "Set-OutboundConnector", outboundConnectorParameters);
			}
		}

		private List<CommandParameter> GetInboundConnectorParameters(bool isNew)
		{
			List<CommandParameter> list = new List<CommandParameter>();
			if (this.InboundIPs != null)
			{
				List<string> list2 = new List<string>();
				foreach (IPRange iprange in this.InboundIPs)
				{
					list2.Add(iprange.ToString());
				}
				list.Add(new CommandParameter("SenderIPAddresses", list2));
			}
			if (this.CertificateSubject != null)
			{
				list.Add(new CommandParameter("TlsSenderCertificateName", this.CertificateSubject));
			}
			if (this.SecureMailEnabled != null)
			{
				list.Add(new CommandParameter("RequireTls", this.SecureMailEnabled.Value));
			}
			if (isNew)
			{
				if (base.OrganizationName != null)
				{
					list.Add(new CommandParameter("Organization", base.OrganizationName));
				}
				list.Add(new CommandParameter("Name", "Hybrid Mail Flow Inbound Connector"));
				list.Add(new CommandParameter("Comment", Strings.HybridMailflowInboundConnectorComment));
				list.Add(new CommandParameter("SenderDomains", "*"));
				list.Add(new CommandParameter("Enabled", true));
				list.Add(new CommandParameter("ConnectorType", "OnPremises"));
				list.Add(new CommandParameter("ConnectorSource", "HybridWizard"));
				list.Add(new CommandParameter("CloudServicesMailEnabled", true));
			}
			else
			{
				list.Add(new CommandParameter("Identity", base.OriginalInboundConnector.Identity));
			}
			return list;
		}

		private List<CommandParameter> GetOutboundConnectorParameters(bool isNew)
		{
			List<CommandParameter> list = new List<CommandParameter>();
			if (this.CentralizedTransportEnabled != null || this.OutboundDomains != null)
			{
				bool addWildcard = false;
				if (this.CentralizedTransportEnabled != null)
				{
					addWildcard = this.CentralizedTransportEnabled.Value;
				}
				else if (base.OriginalInboundConnector != null && base.OriginalOutboundConnector != null)
				{
					HybridMailflowConfiguration hybridMailflowConfiguration = HybridMailflowTaskBase.ConvertToHybridMailflowConfiguration(base.OriginalInboundConnector, base.OriginalOutboundConnector);
					addWildcard = (hybridMailflowConfiguration.CentralizedTransportEnabled != null && hybridMailflowConfiguration.CentralizedTransportEnabled.Value);
				}
				if (this.OutboundDomains != null && HybridMailflowTaskBase.IsRecipientDomainsWildcard(this.OutboundDomains))
				{
					addWildcard = true;
				}
				List<SmtpDomainWithSubdomains> value = (this.OutboundDomains != null) ? HybridMailflowTaskBase.GetRecipientDomains(this.OutboundDomains, addWildcard) : HybridMailflowTaskBase.GetRecipientDomains(base.OriginalOutboundConnector, addWildcard);
				list.Add(new CommandParameter("RecipientDomains", value));
			}
			if (this.CentralizedTransportEnabled != null && this.CentralizedTransportEnabled.Value)
			{
				list.Add(new CommandParameter("RouteAllMessagesViaOnPremises", this.CentralizedTransportEnabled.Value));
			}
			if (this.OnPremisesFQDN != null)
			{
				list.Add(new CommandParameter("SmartHosts", this.OnPremisesFQDN.ToString()));
				list.Add(new CommandParameter("UseMxRecord", false));
			}
			if (this.CertificateSubject != null)
			{
				list.Add(new CommandParameter("TlsDomain", this.CertificateSubject));
			}
			if (this.SecureMailEnabled != null)
			{
				list.Add(new CommandParameter("TlsSettings", this.SecureMailEnabled.Value ? new TlsAuthLevel?(TlsAuthLevel.DomainValidation) : null));
			}
			if (isNew)
			{
				if (base.OrganizationName != null)
				{
					list.Add(new CommandParameter("Organization", base.OrganizationName));
				}
				list.Add(new CommandParameter("Name", "Hybrid Mail Flow Outbound Connector"));
				list.Add(new CommandParameter("Comment", Strings.HybridMailflowOutboundConnectorComment));
				list.Add(new CommandParameter("Enabled", true));
				list.Add(new CommandParameter("ConnectorType", "OnPremises"));
				list.Add(new CommandParameter("ConnectorSource", "HybridWizard"));
				list.Add(new CommandParameter("CloudServicesMailEnabled", true));
			}
			else
			{
				list.Add(new CommandParameter("Identity", base.OriginalOutboundConnector.Identity));
			}
			return list;
		}

		private static string[] ConvertToStringArray<T>(T[] taskParameter)
		{
			string[] array = null;
			if (taskParameter != null)
			{
				array = new string[taskParameter.Length];
				int num = 0;
				foreach (T t in taskParameter)
				{
					array[num++] = t.ToString();
				}
			}
			return array;
		}
	}
}
