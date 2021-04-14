using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.ExchangeTopology;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.FederationProvisioning;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Test", "FederationTrustCertificate", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class TestFederationTrustCertificate : DataAccessTask<FederationTrust>
	{
		[Parameter(Mandatory = false)]
		public new Fqdn DomainController
		{
			get
			{
				return base.DomainController;
			}
			set
			{
				base.DomainController = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageTestFederationTrustCertificate;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return base.RootOrgGlobalConfigSession;
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			if (base.HasErrors)
			{
				return;
			}
			Dictionary<TopologySite, List<TopologyServer>> dictionary = null;
			TopologySite topologySite = null;
			FederationCertificate.DiscoverServers(base.RootOrgGlobalConfigSession, false, out dictionary, out topologySite);
			if (topologySite == null)
			{
				base.WriteError(new CannotGetLocalSiteException(), ErrorCategory.ReadError, null);
			}
			foreach (KeyValuePair<TopologySite, List<TopologyServer>> keyValuePair in dictionary)
			{
				foreach (TopologyServer topologyServer in keyValuePair.Value)
				{
					foreach (CertificateRecord certificateRecord in FederationCertificate.FederationCertificates(base.RootOrgGlobalConfigSession))
					{
						FederationTrustCertificateState state = FederationCertificate.TestForCertificate(topologyServer.Name, certificateRecord.Thumbprint);
						FederationTrustCertificateStatus sendToPipeline = new FederationTrustCertificateStatus(keyValuePair.Key, topologyServer, state, certificateRecord.Thumbprint);
						base.WriteObject(sendToPipeline);
					}
				}
			}
		}
	}
}
