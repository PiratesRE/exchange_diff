using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal class MailFlowTask : SessionTask
	{
		public MailFlowTask() : base(HybridStrings.MailFlowTaskName, 2)
		{
		}

		private HybridConfiguration HybridConfiguration
		{
			get
			{
				return base.TaskContext.HybridConfigurationObject;
			}
		}

		private int ServiceInstance
		{
			get
			{
				return this.HybridConfiguration.ServiceInstance;
			}
		}

		private SmtpX509Identifier TlsCertificateName
		{
			get
			{
				return this.HybridConfiguration.TlsCertificateName;
			}
		}

		private SmtpReceiveDomainCapabilities TlsDomainCapabilities
		{
			get
			{
				return Configuration.TlsDomainCapabilities(this.ServiceInstance);
			}
		}

		private string TlsCertificateSubjectDomainName
		{
			get
			{
				return TaskCommon.GetDomainFromSubject(this.TlsCertificateName);
			}
		}

		private string FopeCertificateSubjectDomainName
		{
			get
			{
				return Configuration.FopeCertificateDomain(this.ServiceInstance);
			}
		}

		private MultiValuedProperty<ADObjectId> ReceivingTransportServers
		{
			get
			{
				return this.HybridConfiguration.ReceivingTransportServers;
			}
		}

		private MultiValuedProperty<ADObjectId> SendingTransportServers
		{
			get
			{
				return this.HybridConfiguration.SendingTransportServers;
			}
		}

		private IEnumerable<ADObjectId> SendingAndReceivingTransportServers
		{
			get
			{
				List<ADObjectId> list = this.SendingTransportServers.ToList<ADObjectId>();
				foreach (ADObjectId item in this.ReceivingTransportServers)
				{
					if (!list.Contains(item))
					{
						list.Add(item);
					}
				}
				return list;
			}
		}

		private MultiValuedProperty<ADObjectId> EdgeTransportServers
		{
			get
			{
				return this.HybridConfiguration.EdgeTransportServers;
			}
		}

		private bool EnableSecureMail
		{
			get
			{
				return this.HybridConfiguration.SecureMailEnabled;
			}
		}

		private MultiValuedProperty<SmtpDomain> HybridDomains
		{
			get
			{
				MultiValuedProperty<SmtpDomain> multiValuedProperty = new MultiValuedProperty<SmtpDomain>();
				foreach (AutoDiscoverSmtpDomain item in this.HybridConfiguration.Domains)
				{
					multiValuedProperty.Add(item);
				}
				return multiValuedProperty;
			}
		}

		private string SmartHost
		{
			get
			{
				SmtpDomain onPremisesSmartHost = this.HybridConfiguration.OnPremisesSmartHost;
				if (onPremisesSmartHost != null)
				{
					return new Fqdn(onPremisesSmartHost.Domain).Domain;
				}
				return null;
			}
		}

		private string TenantCoexistenceDomain
		{
			get
			{
				return base.TaskContext.Parameters.Get<string>("_hybridDomain");
			}
		}

		private string DefaultInboundConnectorName
		{
			get
			{
				return string.Format("Inbound from {0}", this.OnPremOrgConfig.Guid.ToString());
			}
		}

		private string DefaultOutboundConnectorName
		{
			get
			{
				return string.Format("Outbound to {0}", this.OnPremOrgConfig.Guid.ToString());
			}
		}

		private string DefaultSendConnectorName
		{
			get
			{
				return "Outbound to Office 365";
			}
		}

		private bool CentralizedTransportFeatureEnabled
		{
			get
			{
				return this.HybridConfiguration.Features.Contains(HybridFeature.CentralizedTransport);
			}
		}

		private OrganizationRelationship TenantOrganizationRelationship
		{
			get
			{
				return base.TaskContext.Parameters.Get<OrganizationRelationship>("_tenantOrgRel");
			}
		}

		private IOrganizationConfig OnPremOrgConfig
		{
			get
			{
				return base.OnPremisesSession.GetOrganizationConfig();
			}
		}

		public override bool CheckPrereqs(ITaskContext taskContext)
		{
			if (!base.CheckPrereqs(taskContext))
			{
				return false;
			}
			if (this.ReceivingTransportServers.Count == 0 && this.SendingTransportServers.Count == 0 && this.EdgeTransportServers.Count == 0)
			{
				throw new LocalizedException(HybridStrings.HybridErrorNoTransportServersSet);
			}
			if (this.EdgeTransportServers.Count > 0 && (this.ReceivingTransportServers.Count > 0 || this.SendingTransportServers.Count > 0))
			{
				throw new LocalizedException(HybridStrings.HybridErrorMixedTransportServersSet);
			}
			if (this.SmartHost == null)
			{
				throw new LocalizedException(HybridStrings.HybridErrorNoSmartHostSet);
			}
			if (this.TlsCertificateName == null)
			{
				throw new LocalizedException(HybridStrings.HybridErrorNoTlsCertificateNameSet);
			}
			return this.CheckCertPrereqs();
		}

		public override bool NeedsConfiguration(ITaskContext taskContext)
		{
			return base.NeedsConfiguration(taskContext) || this.CheckOrVerifyConfiguration(false);
		}

		public override bool Configure(ITaskContext taskContext)
		{
			if (!base.Configure(taskContext))
			{
				return false;
			}
			this.ConfigureMailFlow();
			return true;
		}

		public override bool ValidateConfiguration(ITaskContext taskContext)
		{
			return base.ValidateConfiguration(taskContext) && !this.CheckOrVerifyConfiguration(true);
		}

		private void Reset()
		{
			this.receiveConnectorsByTransportServer = new Dictionary<string, Tuple<MailFlowTask.Operation, IReceiveConnector>>();
			this.onpremisesRemoteDomains = new Dictionary<string, Tuple<MailFlowTask.RemoteDomainType, MailFlowTask.Operation, DomainContentConfig>>();
		}

		private IReceiveConnector BuildExpectedReceiveConnector(ADObjectId server)
		{
			return base.OnPremisesSession.BuildExpectedReceiveConnector(server, this.TlsCertificateName, this.TlsDomainCapabilities);
		}

		private ISendConnector BuildExpectedSendConnector()
		{
			MultiValuedProperty<ADObjectId> servers = (this.SendingTransportServers.Count > 0) ? this.SendingTransportServers : this.EdgeTransportServers;
			string fqdn = (this.EdgeTransportServers.Count == 0) ? null : this.TlsCertificateSubjectDomainName;
			return base.OnPremisesSession.BuildExpectedSendConnector(this.DefaultSendConnectorName, this.TenantCoexistenceDomain, servers, fqdn, this.FopeCertificateSubjectDomainName, this.TlsCertificateName, this.EnableSecureMail);
		}

		private IInboundConnector BuildExpectedInboundConnector(ADObjectId identity)
		{
			return base.TenantSession.BuildExpectedInboundConnector(identity, this.DefaultInboundConnectorName, this.TlsCertificateName);
		}

		private IOutboundConnector BuildExpectedOutboundConnector(ADObjectId identity)
		{
			return base.TenantSession.BuildExpectedOutboundConnector(identity, this.DefaultOutboundConnectorName, this.TlsCertificateSubjectDomainName, this.HybridDomains, this.SmartHost, this.CentralizedTransportFeatureEnabled);
		}

		private bool CheckCertPrereqs()
		{
			if (!Configuration.DisableCertificateChecks)
			{
				foreach (ADObjectId adobjectId in this.SendingAndReceivingTransportServers)
				{
					IExchangeCertificate exchangeCertificate = base.OnPremisesSession.GetExchangeCertificate(adobjectId.Name, this.TlsCertificateName);
					if (exchangeCertificate == null)
					{
						throw new LocalizedException(HybridStrings.ErrorSecureMailCertificateNotFound(this.HybridConfiguration.TlsCertificateName.CertificateSubject, this.HybridConfiguration.TlsCertificateName.CertificateIssuer, adobjectId.Name));
					}
					if (!exchangeCertificate.Services.ToString().ToUpper().Contains(AllowedServices.SMTP.ToString()))
					{
						throw new LocalizedException(HybridStrings.ErrorSecureMailCertificateNoSmtp(adobjectId.Name));
					}
					if (exchangeCertificate.IsSelfSigned)
					{
						throw new LocalizedException(HybridStrings.ErrorSecureMailCertificateSelfSigned(adobjectId.Name));
					}
					if (exchangeCertificate.NotBefore > (DateTime)ExDateTime.Now || exchangeCertificate.NotAfter < (DateTime)ExDateTime.Now)
					{
						throw new LocalizedException(HybridStrings.ErrorSecureMailCertificateInvalidDate(adobjectId.Name));
					}
				}
			}
			return true;
		}

		private bool DoOnPremisesSendConnectorNeedConfiguration()
		{
			this.sendConnectorOperation = MailFlowTask.Operation.NOP;
			this.sendConnector = this.GetMatchingSendConnector(this.TenantCoexistenceDomain);
			if (this.sendConnector == null)
			{
				this.sendConnectorOperation = MailFlowTask.Operation.New;
				return true;
			}
			if (!this.sendConnector.Equals(this.BuildExpectedSendConnector()))
			{
				this.sendConnectorOperation = MailFlowTask.Operation.Update;
				return true;
			}
			return false;
		}

		private bool DoOnPremisesReceiveConnectorNeedConfiguration()
		{
			this.receiveConnectorOperation = MailFlowTask.Operation.NOP;
			if (this.ReceivingTransportServers.Count > 0)
			{
				using (MultiValuedProperty<ADObjectId>.Enumerator enumerator = this.ReceivingTransportServers.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ADObjectId adobjectId = enumerator.Current;
						IReceiveConnector obj = this.BuildExpectedReceiveConnector(adobjectId);
						IReceiveConnector receiveConnector = base.OnPremisesSession.GetReceiveConnector(adobjectId);
						if (receiveConnector == null)
						{
							throw new LocalizedException(HybridStrings.ErrorDefaultReceieveConnectorNotFound(adobjectId.Name));
						}
						Tuple<MailFlowTask.Operation, IReceiveConnector> value;
						if (receiveConnector.Equals(obj))
						{
							value = new Tuple<MailFlowTask.Operation, IReceiveConnector>(MailFlowTask.Operation.NOP, receiveConnector);
						}
						else
						{
							this.receiveConnectorOperation = MailFlowTask.Operation.Update;
							value = new Tuple<MailFlowTask.Operation, IReceiveConnector>(MailFlowTask.Operation.Update, receiveConnector);
						}
						this.receiveConnectorsByTransportServer[adobjectId.Name] = value;
					}
					goto IL_C2;
				}
			}
			if (this.EdgeTransportServers.Count > 0 && !this.edgeReceiveConnectorsWarningDisplayed)
			{
				this.receiveConnectorOperation = MailFlowTask.Operation.Update;
			}
			IL_C2:
			return this.receiveConnectorOperation != MailFlowTask.Operation.NOP;
		}

		private bool DoTenantConnectorsNeedConfiguration()
		{
			this.onPremisesOrganizationOperation = MailFlowTask.Operation.NOP;
			this.inboundConnectorOperation = MailFlowTask.Operation.NOP;
			this.outboundConnectorOperation = MailFlowTask.Operation.NOP;
			this.onPremisesOrganization = base.TenantSession.GetOnPremisesOrganization(this.OnPremOrgConfig.Guid);
			string identity = this.DefaultInboundConnectorName;
			string identity2 = this.DefaultOutboundConnectorName;
			if (this.onPremisesOrganization == null)
			{
				this.onPremisesOrganizationOperation = MailFlowTask.Operation.New;
			}
			else
			{
				if (this.onPremisesOrganization.InboundConnector != null)
				{
					identity = this.onPremisesOrganization.InboundConnector.ToString();
				}
				if (this.onPremisesOrganization.OutboundConnector != null)
				{
					identity2 = this.onPremisesOrganization.OutboundConnector.ToString();
				}
			}
			this.inboundConnector = base.TenantSession.GetInboundConnector(identity);
			if (this.inboundConnector == null)
			{
				if (this.EnableSecureMail)
				{
					this.inboundConnectorOperation = MailFlowTask.Operation.New;
				}
			}
			else if (this.EnableSecureMail)
			{
				IInboundConnector obj = this.BuildExpectedInboundConnector(null);
				if (!this.inboundConnector.Equals(obj))
				{
					this.inboundConnectorOperation = MailFlowTask.Operation.Update;
				}
			}
			else
			{
				this.inboundConnectorOperation = MailFlowTask.Operation.Remove;
			}
			this.outboundConnector = base.TenantSession.GetOutboundConnector(identity2);
			if (this.outboundConnector == null)
			{
				if (this.EnableSecureMail)
				{
					this.outboundConnectorOperation = MailFlowTask.Operation.New;
				}
			}
			else if (this.EnableSecureMail)
			{
				IOutboundConnector obj2 = this.BuildExpectedOutboundConnector(null);
				if (!this.outboundConnector.Equals(obj2))
				{
					this.outboundConnectorOperation = MailFlowTask.Operation.Update;
				}
			}
			else
			{
				this.outboundConnectorOperation = MailFlowTask.Operation.Remove;
			}
			if (this.onPremisesOrganization != null)
			{
				ADObjectId b = (this.inboundConnector == null) ? null : this.inboundConnector.Identity;
				ADObjectId b2 = (this.outboundConnector == null) ? null : this.outboundConnector.Identity;
				if (this.inboundConnectorOperation == MailFlowTask.Operation.New || this.inboundConnectorOperation == MailFlowTask.Operation.Remove || this.outboundConnectorOperation == MailFlowTask.Operation.New || this.outboundConnectorOperation == MailFlowTask.Operation.Remove || !TaskCommon.AreEqual(this.onPremisesOrganization.InboundConnector, b) || !TaskCommon.AreEqual(this.onPremisesOrganization.OutboundConnector, b2) || !TaskCommon.ContainsSame<SmtpDomain>(this.onPremisesOrganization.HybridDomains, this.HybridDomains) || !string.Equals(this.onPremisesOrganization.OrganizationName, this.OnPremOrgConfig.Name, StringComparison.InvariantCultureIgnoreCase) || !TaskCommon.AreEqual(this.onPremisesOrganization.OrganizationRelationship, (ADObjectId)this.TenantOrganizationRelationship.Identity))
				{
					this.onPremisesOrganizationOperation = MailFlowTask.Operation.Update;
				}
			}
			return this.onPremisesOrganizationOperation != MailFlowTask.Operation.NOP || this.inboundConnectorOperation != MailFlowTask.Operation.NOP || this.outboundConnectorOperation != MailFlowTask.Operation.NOP;
		}

		private void ConfigureOnPremisesConnectors()
		{
			switch (this.sendConnectorOperation)
			{
			case MailFlowTask.Operation.New:
				this.sendConnector = base.OnPremisesSession.NewSendConnector(this.BuildExpectedSendConnector());
				break;
			case MailFlowTask.Operation.Update:
				this.sendConnector.UpdateFrom(this.BuildExpectedSendConnector());
				base.OnPremisesSession.SetSendConnector(this.sendConnector);
				break;
			}
			if (this.receiveConnectorOperation == MailFlowTask.Operation.Update)
			{
				foreach (ADObjectId adobjectId in this.ReceivingTransportServers)
				{
					Tuple<MailFlowTask.Operation, IReceiveConnector> tuple = this.receiveConnectorsByTransportServer[adobjectId.Name];
					MailFlowTask.Operation item = tuple.Item1;
					if (item == MailFlowTask.Operation.Update)
					{
						IReceiveConnector item2 = tuple.Item2;
						item2.UpdateFrom(this.BuildExpectedReceiveConnector(adobjectId));
						base.OnPremisesSession.SetReceiveConnector(item2);
					}
				}
				foreach (ADObjectId adobjectId2 in this.EdgeTransportServers)
				{
					string identity = string.Format("Default Frontend {0}", adobjectId2.ToString());
					base.TaskContext.Warnings.Add(HybridStrings.WarningEdgeReceiveConnector(adobjectId2.ToString(), identity, this.TlsCertificateSubjectDomainName.Replace("*", "mail")));
					this.edgeReceiveConnectorsWarningDisplayed = true;
				}
			}
		}

		private void ConfigureTenantConnectors()
		{
			switch (this.inboundConnectorOperation)
			{
			case MailFlowTask.Operation.New:
				this.inboundConnector = base.TenantSession.NewInboundConnector(this.BuildExpectedInboundConnector(null));
				break;
			case MailFlowTask.Operation.Update:
				base.TenantSession.SetInboundConnector(this.BuildExpectedInboundConnector(this.inboundConnector.Identity));
				break;
			case MailFlowTask.Operation.Remove:
				base.TenantSession.RemoveInboundConnector(this.inboundConnector.Identity);
				this.inboundConnector = null;
				break;
			}
			switch (this.outboundConnectorOperation)
			{
			case MailFlowTask.Operation.New:
				this.outboundConnector = base.TenantSession.NewOutboundConnector(this.BuildExpectedOutboundConnector(null));
				break;
			case MailFlowTask.Operation.Update:
				base.TenantSession.SetOutboundConnector(this.BuildExpectedOutboundConnector(this.outboundConnector.Identity));
				break;
			case MailFlowTask.Operation.Remove:
				base.TenantSession.RemoveOutboundConnector(this.outboundConnector.Identity);
				this.outboundConnector = null;
				break;
			}
			switch (this.onPremisesOrganizationOperation)
			{
			case MailFlowTask.Operation.New:
				this.onPremisesOrganization = base.TenantSession.NewOnPremisesOrganization(this.OnPremOrgConfig, this.HybridDomains, this.inboundConnector, this.outboundConnector, this.TenantOrganizationRelationship);
				return;
			case MailFlowTask.Operation.Update:
				base.TenantSession.SetOnPremisesOrganization(this.onPremisesOrganization, this.OnPremOrgConfig, this.HybridDomains, this.inboundConnector, this.outboundConnector, this.TenantOrganizationRelationship);
				return;
			default:
				return;
			}
		}

		private bool CheckOrVerifyConfiguration(bool verifyOnly)
		{
			bool result = false;
			this.Reset();
			if (this.DoOnPremisesSendConnectorNeedConfiguration())
			{
				result = true;
			}
			if (this.DoOnPremisesReceiveConnectorNeedConfiguration())
			{
				result = true;
			}
			if (this.DoTenantConnectorsNeedConfiguration())
			{
				result = true;
			}
			if (this.DoOnPremisesRemoteDomainsNeedConfiguration())
			{
				result = true;
			}
			return result;
		}

		private void ConfigureMailFlow()
		{
			this.ConfigureOnPremisesConnectors();
			this.ConfigureTenantConnectors();
			this.ConfigureOnPremisesRemoteDomains();
		}

		private ISendConnector GetMatchingSendConnector(string addressSpace)
		{
			ISendConnector sendConnector = null;
			IEnumerable<ISendConnector> enumerable = base.OnPremisesSession.GetSendConnector();
			foreach (ISendConnector sendConnector2 in enumerable)
			{
				foreach (AddressSpace addressSpace2 in sendConnector2.AddressSpaces)
				{
					if (addressSpace2.Address.Equals(addressSpace, StringComparison.InvariantCultureIgnoreCase))
					{
						if (sendConnector != null)
						{
							throw new LocalizedException(HybridStrings.ErrorDuplicateSendConnectorAddressSpace(addressSpace));
						}
						if (sendConnector2.AddressSpaces.Count > 1)
						{
							throw new LocalizedException(HybridStrings.ErrorSendConnectorAddressSpaceNotExclusive(addressSpace));
						}
						sendConnector = sendConnector2;
					}
				}
			}
			if (sendConnector == null)
			{
				foreach (ISendConnector sendConnector3 in enumerable)
				{
					if (string.Equals(sendConnector3.Name, this.DefaultSendConnectorName))
					{
						sendConnector = sendConnector3;
						break;
					}
				}
			}
			return sendConnector;
		}

		private bool DoOnPremisesRemoteDomainsNeedConfiguration()
		{
			if (this.EdgeTransportServers.Count == 0)
			{
				return false;
			}
			IEnumerable<DomainContentConfig> remoteDomain = base.OnPremisesSession.GetRemoteDomain();
			string domainName = this.TenantCoexistenceDomain;
			DomainContentConfig domainContentConfig = (from d in remoteDomain
			where string.Equals(d.DomainName.Domain, domainName, StringComparison.InvariantCultureIgnoreCase)
			select d).FirstOrDefault<DomainContentConfig>();
			MailFlowTask.Operation item;
			if (domainContentConfig == null)
			{
				item = MailFlowTask.Operation.New;
			}
			else if (domainContentConfig.AllowedOOFType == AllowedOOFType.InternalLegacy && domainContentConfig.TrustedMailOutboundEnabled && domainContentConfig.AutoReplyEnabled && domainContentConfig.AutoForwardEnabled && domainContentConfig.DeliveryReportEnabled && domainContentConfig.NDREnabled && domainContentConfig.DisplaySenderName && domainContentConfig.TNEFEnabled != null && domainContentConfig.TNEFEnabled.Value)
			{
				item = MailFlowTask.Operation.NOP;
			}
			else
			{
				item = MailFlowTask.Operation.Update;
			}
			this.onpremisesRemoteDomains[domainName] = new Tuple<MailFlowTask.RemoteDomainType, MailFlowTask.Operation, DomainContentConfig>(MailFlowTask.RemoteDomainType.Coexistence, item, domainContentConfig);
			foreach (SmtpDomain smtpDomain in this.HybridDomains)
			{
				string domainName = smtpDomain.Domain;
				DomainContentConfig domainContentConfig2 = (from d in remoteDomain
				where string.Equals(d.DomainName.Domain, domainName, StringComparison.InvariantCultureIgnoreCase)
				select d).FirstOrDefault<DomainContentConfig>();
				MailFlowTask.Operation item2 = MailFlowTask.Operation.NOP;
				if (domainContentConfig2 == null)
				{
					item2 = MailFlowTask.Operation.New;
				}
				else if (!domainContentConfig2.TrustedMailInboundEnabled)
				{
					item2 = MailFlowTask.Operation.Update;
				}
				this.onpremisesRemoteDomains[domainName] = new Tuple<MailFlowTask.RemoteDomainType, MailFlowTask.Operation, DomainContentConfig>(MailFlowTask.RemoteDomainType.HybridDomain, item2, domainContentConfig2);
			}
			return this.onpremisesRemoteDomains.Values.Any((Tuple<MailFlowTask.RemoteDomainType, MailFlowTask.Operation, DomainContentConfig> t) => t.Item2 != MailFlowTask.Operation.NOP);
		}

		private void ConfigureOnPremisesRemoteDomains()
		{
			foreach (KeyValuePair<string, Tuple<MailFlowTask.RemoteDomainType, MailFlowTask.Operation, DomainContentConfig>> keyValuePair in this.onpremisesRemoteDomains)
			{
				string key = keyValuePair.Key;
				MailFlowTask.RemoteDomainType item = keyValuePair.Value.Item1;
				MailFlowTask.Operation item2 = keyValuePair.Value.Item2;
				DomainContentConfig domainContentConfig = keyValuePair.Value.Item3;
				if (item2 == MailFlowTask.Operation.New)
				{
					string name = string.Format("Hybrid Domain - {0}", key);
					domainContentConfig = base.OnPremisesSession.NewRemoteDomain(key, name);
				}
				if (item2 == MailFlowTask.Operation.New || item2 == MailFlowTask.Operation.Update)
				{
					SessionParameters sessionParameters = new SessionParameters();
					if (item == MailFlowTask.RemoteDomainType.HybridDomain)
					{
						sessionParameters.Set("TrustedMailInbound", true);
					}
					else if (item == MailFlowTask.RemoteDomainType.Coexistence)
					{
						sessionParameters.Set("AllowedOOFType", AllowedOOFType.InternalLegacy);
						sessionParameters.Set("TrustedMailOutbound", true);
						sessionParameters.Set("AutoReplyEnabled", true);
						sessionParameters.Set("AutoForwardEnabled", true);
						sessionParameters.Set("DeliveryReportEnabled", true);
						sessionParameters.Set("NDREnabled", true);
						sessionParameters.Set("DisplaySenderName", true);
						sessionParameters.Set("TNEFEnabled", true);
					}
					base.OnPremisesSession.SetRemoteDomain(domainContentConfig.Identity.ToString(), sessionParameters);
				}
			}
		}

		private MailFlowTask.Operation sendConnectorOperation;

		private ISendConnector sendConnector;

		private MailFlowTask.Operation receiveConnectorOperation;

		private Dictionary<string, Tuple<MailFlowTask.Operation, IReceiveConnector>> receiveConnectorsByTransportServer;

		private MailFlowTask.Operation inboundConnectorOperation;

		private MailFlowTask.Operation outboundConnectorOperation;

		private MailFlowTask.Operation onPremisesOrganizationOperation;

		private IOnPremisesOrganization onPremisesOrganization;

		private IInboundConnector inboundConnector;

		private IOutboundConnector outboundConnector;

		private bool edgeReceiveConnectorsWarningDisplayed;

		private Dictionary<string, Tuple<MailFlowTask.RemoteDomainType, MailFlowTask.Operation, DomainContentConfig>> onpremisesRemoteDomains;

		private enum Operation
		{
			NOP,
			New,
			Update,
			Remove
		}

		private enum RemoteDomainType
		{
			Coexistence,
			HybridDomain
		}
	}
}
