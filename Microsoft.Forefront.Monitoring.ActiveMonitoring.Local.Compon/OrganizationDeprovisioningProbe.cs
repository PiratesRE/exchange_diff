using System;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Hygiene.Data.Directory;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public sealed class OrganizationDeprovisioningProbe : ProvisioningProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			base.Result.ExecutionStartTime = DateTime.UtcNow;
			base.TraceInformation("Starting {0} organization deprovisioning probe at {1}.", new object[]
			{
				base.Definition.Name,
				base.Result.ExecutionStartTime
			});
			try
			{
				this.DeprovisionOrganization();
			}
			catch (Exception ex)
			{
				base.TraceError("Encountered an exception during '{0}' probe execution: {1}.", new object[]
				{
					base.Definition.Name,
					ex.Message
				});
				throw;
			}
			base.TraceInformation("Completed {0} organization deprovisioning probe at {1}.", new object[]
			{
				base.Definition.Name,
				base.Result.ExecutionStartTime
			});
		}

		private void DeprovisionOrganization()
		{
			OrganizationInitializationDefinition organizationInitializationDefinition = new OrganizationInitializationDefinition(base.Definition.ExtensionAttributes);
			GlobalConfigSession globalConfigSession = new GlobalConfigSession();
			ProbeOrganizationInfo probeOrganizationInfo = null;
			try
			{
				probeOrganizationInfo = globalConfigSession.GetProbeOrganizations(organizationInitializationDefinition.FeatureTag).FirstOrDefault<ProbeOrganizationInfo>();
			}
			catch (Exception)
			{
			}
			string orgName;
			Guid guid;
			if (probeOrganizationInfo != null)
			{
				base.TraceInformation("Feature tag found for {0}.", new object[]
				{
					organizationInitializationDefinition.FeatureTag
				});
				guid = probeOrganizationInfo.ProbeOrganizationId.ObjectGuid;
				orgName = probeOrganizationInfo.OrganizationName;
			}
			else
			{
				base.TraceInformation("Feature tag not found. Using TenantId and OrgName from Probe Definition", new object[0]);
				guid = organizationInitializationDefinition.TenantId;
				orgName = organizationInitializationDefinition.DomainPrefix + ".msol-test.com";
			}
			if (guid == Guid.Empty)
			{
				throw new ArgumentException("Tenant Id not present and Feature Tag not found, cannot deprovision");
			}
			base.TraceInformation("Deprovisioning the organization with organization Id {0}.", new object[]
			{
				guid
			});
			organizationInitializationDefinition.CompanyManager.ForceDeleteCompany(guid);
			organizationInitializationDefinition.CompanyManager.ForceTransitiveReplication();
			if (organizationInitializationDefinition.WaitForDeprovisioning)
			{
				base.AwaitCompletion("Deleting from directory.. (not GLS, since that has a delay)", TimeSpan.FromSeconds(5.0), TimeSpan.FromMinutes(3.0), () => this.CheckTenantDeprovisioning(orgName));
			}
			if (probeOrganizationInfo != null)
			{
				base.TraceInformation("The organization has been deprovisioned; removing the feature tag {0} now for tenant Id {1}.", new object[]
				{
					probeOrganizationInfo.FeatureTag,
					probeOrganizationInfo.ProbeOrganizationId.ObjectGuid
				});
				globalConfigSession.Delete(probeOrganizationInfo);
			}
		}

		private bool CheckTenantDeprovisioning(string organizationName)
		{
			GlobalConfigSession globalConfigSession = new GlobalConfigSession();
			return globalConfigSession.GetTenantByName(organizationName) == null;
		}
	}
}
