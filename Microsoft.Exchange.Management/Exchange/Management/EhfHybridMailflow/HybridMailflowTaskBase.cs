using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Monitoring;

namespace Microsoft.Exchange.Management.EhfHybridMailflow
{
	public abstract class HybridMailflowTaskBase : Task
	{
		[Parameter(Position = 0, Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
			}
		}

		private void WriteErrorAndMonitoringEvent(Exception exception, ExchangeErrorCategory errorCategory, object target, int eventId, string eventSource)
		{
			this.monitoringData.Events.Add(new MonitoringEvent(eventSource, eventId, EventTypeEnumeration.Error, exception.Message));
			base.WriteError(exception, (ErrorCategory)errorCategory, target);
		}

		private Runspace GetConnectorPowershellRunspace()
		{
			RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();
			PSSnapInException ex = null;
			runspaceConfiguration.AddPSSnapIn("Microsoft.Exchange.Management.PowerShell.E2010", out ex);
			if (ex != null)
			{
				throw ex;
			}
			return RunspaceFactory.CreateRunspace(runspaceConfiguration);
		}

		private void AppendCommonParameters(ref List<CommandParameter> parameters)
		{
			Dictionary<string, object> boundParameters = base.MyInvocation.BoundParameters;
			string[] array = new string[]
			{
				"Verbose",
				"Debug",
				"ErrorAction",
				"WarningAction",
				"ErrorVariable",
				"WarningVariable",
				"OutVariable",
				"OutBuffer"
			};
			foreach (string text in array)
			{
				object value = null;
				if (boundParameters.TryGetValue(text, out value))
				{
					parameters.Add(new CommandParameter(text, value));
				}
			}
		}

		protected TenantInboundConnector OriginalInboundConnector
		{
			get
			{
				if (this.originalInboundConnector == null)
				{
					this.originalInboundConnector = this.GetHybridMailflowInboundConnector();
				}
				return this.originalInboundConnector;
			}
		}

		protected TenantOutboundConnector OriginalOutboundConnector
		{
			get
			{
				if (this.originalOutboundConnector == null)
				{
					this.originalOutboundConnector = this.GetHybridMailflowOutboundConnector();
				}
				return this.originalOutboundConnector;
			}
		}

		protected string OrganizationName
		{
			get
			{
				if (this.organizationName == null)
				{
					if (this.Organization != null)
					{
						this.organizationName = this.Organization.ToString();
					}
					else if (base.ExecutingUserOrganizationId != null && base.ExecutingUserOrganizationId.ConfigurationUnit != null)
					{
						this.organizationName = base.ExecutingUserOrganizationId.ConfigurationUnit.ToString();
					}
					else
					{
						base.WriteError(new ApplicationException(Strings.HybridMailflowNoOrganizationError), ErrorCategory.InvalidData, null);
					}
				}
				return this.organizationName;
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || DataAccessHelper.IsDataAccessKnownException(exception) || MonitoringHelper.IsKnownExceptionForMonitoring(exception);
		}

		protected override void InternalEndProcessing()
		{
			base.InternalEndProcessing();
		}

		protected static HybridMailflowConfiguration ConvertToHybridMailflowConfiguration(TenantInboundConnector inbound, TenantOutboundConnector outbound)
		{
			ArgumentValidator.ThrowIfNull("inbound", inbound);
			ArgumentValidator.ThrowIfNull("outbound", outbound);
			HybridMailflowConfiguration hybridMailflowConfiguration = new HybridMailflowConfiguration();
			hybridMailflowConfiguration.OutboundDomains = new List<SmtpDomainWithSubdomains>();
			foreach (SmtpDomainWithSubdomains smtpDomainWithSubdomains in outbound.RecipientDomains)
			{
				if (!HybridMailflowTaskBase.IsWildcardDomain(smtpDomainWithSubdomains))
				{
					hybridMailflowConfiguration.OutboundDomains.Add(new SmtpDomainWithSubdomains(smtpDomainWithSubdomains.Address));
				}
			}
			if (inbound.SenderIPAddresses != null)
			{
				hybridMailflowConfiguration.InboundIPs = new List<IPRange>();
				foreach (IPRange item in inbound.SenderIPAddresses)
				{
					hybridMailflowConfiguration.InboundIPs.Add(item);
				}
			}
			if (!MultiValuedPropertyBase.IsNullOrEmpty(outbound.SmartHosts))
			{
				SmartHost smartHost = outbound.SmartHosts[0];
				if (smartHost.IsIPAddress)
				{
					hybridMailflowConfiguration.OnPremisesFQDN = new Fqdn(smartHost.Address.ToString());
				}
				else
				{
					hybridMailflowConfiguration.OnPremisesFQDN = new Fqdn(smartHost.ToString());
				}
			}
			if (inbound.TlsSenderCertificateName != null)
			{
				hybridMailflowConfiguration.CertificateSubject = inbound.TlsSenderCertificateName.ToString();
			}
			hybridMailflowConfiguration.SecureMailEnabled = new bool?(inbound.RequireTls && outbound.TlsSettings != null && outbound.TlsSettings == TlsAuthLevel.DomainValidation);
			hybridMailflowConfiguration.CentralizedTransportEnabled = new bool?((inbound.RestrictDomainsToIPAddresses && HybridMailflowTaskBase.IsRecipientDomainsWildcard(outbound.RecipientDomains)) || outbound.RouteAllMessagesViaOnPremises);
			return hybridMailflowConfiguration;
		}

		protected static bool IsRecipientDomainsWildcard(IEnumerable<SmtpDomainWithSubdomains> domains)
		{
			if (domains != null)
			{
				foreach (SmtpDomainWithSubdomains domain in domains)
				{
					if (HybridMailflowTaskBase.IsWildcardDomain(domain))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		protected static bool IsWildcardDomain(SmtpDomainWithSubdomains domain)
		{
			return domain != null && domain.Equals(SmtpDomainWithSubdomains.StarDomain);
		}

		protected static List<SmtpDomainWithSubdomains> GetRecipientDomains(TenantOutboundConnector outboundConnector, bool addWildcard)
		{
			IEnumerable<SmtpDomainWithSubdomains> domains = null;
			if (outboundConnector != null && outboundConnector.RecipientDomains != null)
			{
				domains = outboundConnector.RecipientDomains;
			}
			return HybridMailflowTaskBase.GetRecipientDomains(domains, addWildcard);
		}

		protected static List<SmtpDomainWithSubdomains> GetRecipientDomains(IEnumerable<SmtpDomainWithSubdomains> domains, bool addWildcard)
		{
			List<SmtpDomainWithSubdomains> result = new List<SmtpDomainWithSubdomains>();
			if (addWildcard)
			{
				HybridMailflowTaskBase.AddRecipientDomain(ref result, SmtpDomainWithSubdomains.StarDomain);
			}
			if (domains != null)
			{
				foreach (SmtpDomainWithSubdomains domain in domains)
				{
					if (!HybridMailflowTaskBase.IsWildcardDomain(domain))
					{
						HybridMailflowTaskBase.AddRecipientDomain(ref result, domain);
					}
				}
			}
			return result;
		}

		protected static void AddRecipientDomain(ref List<SmtpDomainWithSubdomains> recipientDomains, SmtpDomainWithSubdomains domain)
		{
			if (!recipientDomains.Contains(domain))
			{
				recipientDomains.Add(domain);
			}
		}

		protected TenantInboundConnector GetHybridMailflowInboundConnector()
		{
			Collection<PSObject> collection = this.InvokeConnectorCmdletWithOrganization("Get-InboundConnector", "Hybrid Mail Flow Inbound Connector", true);
			TenantInboundConnector result = null;
			if (collection != null && collection.Count > 0)
			{
				foreach (PSObject psobject in collection)
				{
					if (psobject.BaseObject is TenantInboundConnector)
					{
						TenantInboundConnector tenantInboundConnector = psobject.BaseObject as TenantInboundConnector;
						if (tenantInboundConnector.ConnectorType == TenantConnectorType.OnPremises)
						{
							result = tenantInboundConnector;
							break;
						}
						this.WriteWarning(Strings.HybridMailflowConnectorNameAndTypeWarning("Hybrid Mail Flow Inbound Connector"));
					}
					else
					{
						base.WriteError(new ApplicationException(Strings.HybridMailflowUnexpectedType("Get-InboundConnector")), ErrorCategory.InvalidData, null);
					}
				}
			}
			return result;
		}

		protected TenantOutboundConnector GetHybridMailflowOutboundConnector()
		{
			Collection<PSObject> collection = this.InvokeConnectorCmdletWithOrganization("Get-OutboundConnector", "Hybrid Mail Flow Outbound Connector", true);
			TenantOutboundConnector result = null;
			if (collection != null && collection.Count > 0)
			{
				foreach (PSObject psobject in collection)
				{
					if (psobject.BaseObject is TenantOutboundConnector)
					{
						TenantOutboundConnector tenantOutboundConnector = psobject.BaseObject as TenantOutboundConnector;
						if (tenantOutboundConnector.ConnectorType == TenantConnectorType.OnPremises)
						{
							result = tenantOutboundConnector;
							break;
						}
						this.WriteWarning(Strings.HybridMailflowConnectorNameAndTypeWarning("Hybrid Mail Flow Outbound Connector"));
					}
					else
					{
						base.WriteError(new ApplicationException(Strings.HybridMailflowUnexpectedType("Get-OutboundConnector")), ErrorCategory.InvalidData, null);
					}
				}
			}
			return result;
		}

		protected Collection<PSObject> InvokeConnectorCmdlet(string CmdletName, string Identity)
		{
			return this.InvokeConnectorCmdlet(CmdletName, Identity, false);
		}

		protected Collection<PSObject> InvokeConnectorCmdlet(string CmdletName, string Identity, bool IgnoreErrors)
		{
			return this.InvokeConnectorCmdlet(CmdletName, new List<CommandParameter>
			{
				new CommandParameter("identity", Identity)
			}, IgnoreErrors);
		}

		protected Collection<PSObject> InvokeConnectorCmdletWithOrganization(string CmdletName, string Identity, bool IgnoreErrors)
		{
			return this.InvokeConnectorCmdlet(CmdletName, new List<CommandParameter>
			{
				new CommandParameter("identity", Identity),
				new CommandParameter("Organization", this.OrganizationName)
			}, IgnoreErrors);
		}

		protected Collection<PSObject> InvokeConnectorCmdlet(string CmdletName, List<CommandParameter> Parameters)
		{
			return this.InvokeConnectorCmdlet(CmdletName, Parameters, false);
		}

		protected Collection<PSObject> InvokeConnectorCmdlet(string CmdletName, List<CommandParameter> Parameters, bool IgnoreErrors)
		{
			Collection<PSObject> result = null;
			Runspace connectorPowershellRunspace = this.GetConnectorPowershellRunspace();
			connectorPowershellRunspace.Open();
			Pipeline pipeline = connectorPowershellRunspace.CreatePipeline();
			Command command = new Command(CmdletName);
			this.AppendCommonParameters(ref Parameters);
			base.WriteDebug(string.Format("Invoking {0}", CmdletName));
			foreach (CommandParameter commandParameter in Parameters)
			{
				base.WriteDebug(string.Format("  -{0}:{1}", commandParameter.Name, (commandParameter.Value == null) ? "null" : commandParameter.Value.ToString()));
				command.Parameters.Add(commandParameter);
			}
			pipeline.Commands.Add(command);
			try
			{
				result = pipeline.Invoke();
				if (!IgnoreErrors && pipeline.Error.Count > 0)
				{
					while (!pipeline.Error.EndOfPipeline)
					{
						PSObject psobject = pipeline.Error.Read() as PSObject;
						if (psobject != null)
						{
							ErrorRecord errorRecord = psobject.BaseObject as ErrorRecord;
							if (errorRecord != null)
							{
								base.WriteError(errorRecord.Exception, errorRecord.CategoryInfo.Category, errorRecord.TargetObject);
							}
						}
					}
				}
			}
			finally
			{
				if (connectorPowershellRunspace.RunspaceStateInfo.State == RunspaceState.Opened)
				{
					connectorPowershellRunspace.Close();
					connectorPowershellRunspace.Dispose();
				}
			}
			return result;
		}

		protected static LocalizedString HybridMailflowUnexpectedTypeMessage(string CmdletName)
		{
			return Strings.HybridMailflowUnexpectedType(CmdletName);
		}

		protected LocalizedString NullInboundConnectorMessage
		{
			get
			{
				return Strings.HybridMailflowNullConnector("Get-InboundConnector", "Hybrid Mail Flow Inbound Connector");
			}
		}

		protected LocalizedString NullOutboundConnectorMessage
		{
			get
			{
				return Strings.HybridMailflowNullConnector("Get-OutboundConnector", "Hybrid Mail Flow Outbound Connector");
			}
		}

		private const string CmdletNoun = "HybridMailflow";

		private const string CmdletMonitoringEventSource = "MSExchange Monitoring HybridMailflow";

		private MonitoringData monitoringData = new MonitoringData();

		private TenantInboundConnector originalInboundConnector;

		private TenantOutboundConnector originalOutboundConnector;

		private string organizationName;
	}
}
