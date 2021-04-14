using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.ExchangeCertificate;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "SendConnector", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetSendConnector : SetTopologySystemConfigurationObjectTask<SendConnectorIdParameter, SmtpSendConnectorConfig>
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter Force
		{
			internal get
			{
				return this.force;
			}
			set
			{
				this.force = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<ServerIdParameter> SourceTransportServers
		{
			get
			{
				return (MultiValuedProperty<ServerIdParameter>)base.Fields["SourceTransportServers"];
			}
			set
			{
				base.Fields["SourceTransportServers"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetSendConnector(this.Identity.ToString());
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			SmtpSendConnectorConfig smtpSendConnectorConfig = (SmtpSendConnectorConfig)base.PrepareDataObject();
			try
			{
				this.localServer = ((ITopologyConfigurationSession)base.DataSession).ReadLocalServer();
			}
			catch (TransientException exception)
			{
				base.WriteError(exception, ErrorCategory.ResourceUnavailable, this.DataObject);
			}
			if (base.Fields.IsModified("SourceTransportServers"))
			{
				if (this.SourceTransportServers != null)
				{
					smtpSendConnectorConfig.SourceTransportServers = base.ResolveIdParameterCollection<ServerIdParameter, Server, ADObjectId>(this.SourceTransportServers, base.DataSession, this.RootId, null, (ExchangeErrorCategory)0, new Func<IIdentityParameter, LocalizedString>(Strings.ErrorServerNotFound), new Func<IIdentityParameter, LocalizedString>(Strings.ErrorServerNotUnique), null, null);
					if (smtpSendConnectorConfig.SourceTransportServers.Count > 0)
					{
						ManageSendConnectors.SetConnectorHomeMta(smtpSendConnectorConfig, (IConfigurationSession)base.DataSession);
					}
				}
				else
				{
					smtpSendConnectorConfig.SourceTransportServers = null;
				}
			}
			return smtpSendConnectorConfig;
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			SmtpSendConnectorConfig smtpSendConnectorConfig = dataObject as SmtpSendConnectorConfig;
			if (smtpSendConnectorConfig != null)
			{
				smtpSendConnectorConfig.DNSRoutingEnabled = string.IsNullOrEmpty(smtpSendConnectorConfig.SmartHostsString);
				smtpSendConnectorConfig.IsScopedConnector = smtpSendConnectorConfig.GetScopedConnector();
				smtpSendConnectorConfig.ResetChangeTracking();
				this.dashDashConnector = smtpSendConnectorConfig.IsInitialSendConnector();
			}
			base.StampChangesOn(dataObject);
		}

		protected override void InternalProcessRecord()
		{
			if (this.dashDashConnector && (this.DataObject.IsChanged(MailGatewaySchema.AddressSpaces) || this.DataObject.IsChanged(SmtpSendConnectorConfigSchema.SmartHostsString)) && !this.force && !base.ShouldContinue(Strings.ConfirmationMessageSetEdgeSyncSendConnectorAddressSpaceOrSmartHosts))
			{
				return;
			}
			Exception ex = null;
			if (this.DataObject.DNSRoutingEnabled)
			{
				ex = NewSendConnector.CheckDNSAndSmartHostParameters(this.DataObject);
				if (ex == null)
				{
					ex = NewSendConnector.CheckDNSAndSmartHostAuthMechanismParameters(this.DataObject);
				}
			}
			if (ex == null)
			{
				ex = NewSendConnector.CheckTLSParameters(this.DataObject);
			}
			if (this.DataObject.TlsCertificateName != null)
			{
				ex = this.ValidateCertificateForSmtp(this.DataObject);
			}
			if (ex != null)
			{
				base.WriteError(ex, ErrorCategory.InvalidOperation, this.DataObject.Identity);
				return;
			}
			NewSendConnector.ClearSmartHostsListIfNecessary(this.DataObject);
			NewSendConnector.SetSmartHostAuthMechanismIfNecessary(this.DataObject);
			ManageSendConnectors.AdjustAddressSpaces(this.DataObject);
			base.InternalProcessRecord();
			if (!TopologyProvider.IsAdamTopology())
			{
				ManageSendConnectors.UpdateGwartLastModified((ITopologyConfigurationSession)base.DataSession, this.DataObject.SourceRoutingGroup, new ManageSendConnectors.ThrowTerminatingErrorDelegate(base.ThrowTerminatingError));
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (Server.IsSubscribedGateway((ITopologyConfigurationSession)base.DataSession))
			{
				base.WriteError(new CannotRunOnSubscribedEdgeException(), ErrorCategory.InvalidOperation, null);
			}
			bool flag;
			LocalizedException ex = NewSendConnector.CrossObjectValidate(this.DataObject, (IConfigurationSession)base.DataSession, this.localServer, this, out flag);
			if (ex != null)
			{
				base.WriteError(ex, ErrorCategory.InvalidOperation, this.DataObject);
				return;
			}
			if (flag)
			{
				this.WriteWarning(Strings.WarningMultiSiteSourceServers);
			}
		}

		private Exception ValidateCertificateForSmtp(SmtpSendConnectorConfig sendConnector)
		{
			SmtpX509Identifier tlsCertificateName = sendConnector.TlsCertificateName;
			if (sendConnector.SourceTransportServers.Count > 0)
			{
				ADObjectId adobjectId = sendConnector.SourceTransportServers[0];
				ExchangeCertificateRpc exchangeCertificateRpc = new ExchangeCertificateRpc();
				ExchangeCertificateRpcVersion exchangeCertificateRpcVersion = ExchangeCertificateRpcVersion.Version1;
				byte[] outputBlob = null;
				try
				{
					byte[] inBlob = exchangeCertificateRpc.SerializeInputParameters(ExchangeCertificateRpcVersion.Version2);
					ExchangeCertificateRpcClient2 exchangeCertificateRpcClient = new ExchangeCertificateRpcClient2(adobjectId.Name);
					outputBlob = exchangeCertificateRpcClient.GetCertificate2(0, inBlob);
					exchangeCertificateRpcVersion = ExchangeCertificateRpcVersion.Version2;
				}
				catch (RpcException)
				{
					exchangeCertificateRpcVersion = ExchangeCertificateRpcVersion.Version1;
				}
				if (exchangeCertificateRpcVersion == ExchangeCertificateRpcVersion.Version1 && adobjectId.Name != null && adobjectId.DomainId != null && !string.IsNullOrEmpty(adobjectId.DistinguishedName))
				{
					try
					{
						byte[] inBlob2 = exchangeCertificateRpc.SerializeInputParameters(exchangeCertificateRpcVersion);
						ExchangeCertificateRpcClient exchangeCertificateRpcClient2 = new ExchangeCertificateRpcClient(adobjectId.Name);
						outputBlob = exchangeCertificateRpcClient2.GetCertificate(0, inBlob2);
					}
					catch (RpcException)
					{
						return null;
					}
				}
				ExchangeCertificateRpc exchangeCertificateRpc2 = new ExchangeCertificateRpc(exchangeCertificateRpcVersion, null, outputBlob);
				foreach (ExchangeCertificate exchangeCertificate in exchangeCertificateRpc2.ReturnCertList)
				{
					if (exchangeCertificate.Issuer.Equals(tlsCertificateName.CertificateIssuer) && exchangeCertificate.Subject.Equals(tlsCertificateName.CertificateSubject) && (exchangeCertificate.Services & AllowedServices.SMTP) != AllowedServices.SMTP)
					{
						return new InvalidOperationException(Strings.SMTPNotEnabledForTlsCertificate);
					}
				}
			}
			return null;
		}

		private Server localServer;

		private SwitchParameter force;

		private bool dashDashConnector;
	}
}
