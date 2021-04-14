using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.GlobalLocatorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Hygiene.Data;
using Microsoft.Exchange.Hygiene.Data.Directory;
using Microsoft.Exchange.Hygiene.Provisioning;
using Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public sealed class OrganizationInitializationProbe : ProvisioningProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			base.TraceInformation("Starting {0} organization initialization probe.", new object[]
			{
				base.Definition.Name
			});
			base.Result.ExecutionStartTime = DateTime.UtcNow;
			try
			{
				this.InitializeOrganization();
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
			base.TraceInformation("Completed {0} organization initialization probe.", new object[]
			{
				base.Definition.Name
			});
		}

		private void InitializeOrganization()
		{
			OrganizationInitializationDefinition definition = new OrganizationInitializationDefinition(base.Definition.ExtensionAttributes);
			GlobalConfigSession globalSession = new GlobalConfigSession();
			IEnumerable<ProbeOrganizationInfo> probeOrganizations = globalSession.GetProbeOrganizations(definition.FeatureTag);
			if (probeOrganizations == null || !probeOrganizations.Any<ProbeOrganizationInfo>())
			{
				base.TraceInformation("Organizations do not exist for feature tag {0}; creating them now.", new object[]
				{
					definition.FeatureTag
				});
				Company company = this.CreateCompany(definition);
				User user = this.CreateAdmin(definition);
				string tenantName = company.DomainPrefix + ".msol-test.com";
				CustomerType customerType = (definition.CustomerType.Contains("EXCHANGE") || definition.CustomerType.Contains("ENTERPRISEPACK")) ? CustomerType.Hosted : CustomerType.FilteringOnly;
				try
				{
					base.TraceInformation("Creating organization '{0}' with admin '{1}'.", new object[]
					{
						tenantName,
						user.UserName + "@" + tenantName
					});
					ProvisionInfo provisionInfo = definition.CompanyManager.CreateCompany(company, user, null, Guid.NewGuid());
					if (string.IsNullOrEmpty(definition.Version) || definition.Version == "W15")
					{
						definition.CompanyManager.UpdateCompanyTags(provisionInfo.ContextId, new string[]
						{
							"o365.microsoft.com/version=15"
						});
					}
					Subscription subscription = this.CreateSubscription(definition, provisionInfo);
					definition.CompanyManager.CreateUpdateDeleteSubscription(subscription);
					definition.CompanyManager.ForceTransitiveReplication();
				}
				catch (Exception ex)
				{
					if (!Regex.IsMatch(ex.Message, "Domain name .* is not available", RegexOptions.IgnoreCase))
					{
						throw;
					}
					base.TraceInformation("The company {0} was already created in MSO; continuing.", new object[]
					{
						tenantName
					});
				}
				if (customerType == CustomerType.Hosted)
				{
					base.TraceInformation("For hosted tenants, adding a license to the admin.", new object[0]);
					ProvisioningActions provisioningActions = new ProvisioningActions("https://provisioningapi.msol-test.com/provisioningwebservice.svc", user.UserName + "@" + tenantName, "Pa$$w0rd");
					provisioningActions.AddExchangeUserLicense("admin");
				}
				base.TraceInformation("The organization {0} was created and replication forced. Will wait for provisioning to occur.", new object[]
				{
					tenantName
				});
				base.AwaitCompletion("Provisioning to Directory & GLS", TimeSpan.FromSeconds(30.0), TimeSpan.FromMinutes((double)definition.TimeoutInMinutes), () => this.CheckDomainProvisioning(tenantName, customerType == CustomerType.Hosted && definition.WaitForEXOProperties) && globalSession.GetTenantByName(tenantName) != null);
				base.TraceInformation("Provisioning is complete; establishing a featureTag ({0}) to {1} tenant relationship ({2}).", new object[]
				{
					definition.FeatureTag,
					customerType,
					tenantName
				});
				ProbeOrganizationInfo probeOrganizationInfo = new ProbeOrganizationInfo
				{
					FeatureTag = definition.FeatureTag,
					Name = definition.FeatureTag,
					Region = DatacenterRegistry.GetForefrontRegion(),
					OrganizationName = tenantName,
					LoginPassword = "Pa$$w0rd".ConvertToSecureString(),
					ProbeOrganizationId = globalSession.GetTenantByName(tenantName).OrganizationalUnitRoot,
					CustomerType = customerType
				};
				probeOrganizationInfo.SetId(new ADObjectId(DalHelper.GetTenantDistinguishedName(definition.FeatureTag), CombGuidGenerator.NewGuid()));
				globalSession.Save(probeOrganizationInfo);
				return;
			}
			base.TraceInformation("Organizations already exist for feature tag {0}; skipping creation and initialization.", new object[]
			{
				definition.FeatureTag
			});
		}

		private bool CheckDomainProvisioning(string domainName, bool checkEXOProperties)
		{
			GlsDirectorySession glsDirectorySession = new GlsDirectorySession(GlsCallerId.ForwardSyncService);
			if (checkEXOProperties)
			{
				return glsDirectorySession.DomainExists(domainName, new Namespace[]
				{
					Namespace.Ffo,
					Namespace.Exo
				});
			}
			return glsDirectorySession.DomainExists(domainName, new Namespace[]
			{
				Namespace.Ffo
			});
		}

		private Company CreateCompany(OrganizationInitializationDefinition definition)
		{
			Company company = definition.CompanyManager.BuildRandomCompany("msol-test.com");
			company.CompanyProfile.CountryLetterCode = "US";
			company.CompanyProfile.CommunicationCulture = "EN";
			company.DomainPrefix = definition.DomainPrefix;
			company.CompanyProfile.DisplayName = definition.FeatureTag;
			return company;
		}

		private User CreateAdmin(OrganizationInitializationDefinition definition)
		{
			User user = definition.CompanyManager.BuildRandomUser();
			user.UserName = "admin";
			user.NewUserPassword = "Pa$$w0rd";
			return user;
		}

		private Subscription CreateSubscription(OrganizationInitializationDefinition definition, ProvisionInfo companyInfo)
		{
			Subscription subscription = definition.CompanyManager.BuildRandomSubscription(companyInfo.ContextId, companyInfo.ContextId, definition.CompanyManager.GetSkuIdFromPartNumber(definition.CustomerType));
			subscription.PrepaidUnits = 50;
			subscription.StartDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(15.0));
			return subscription;
		}

		private const string DomainSuffix = "msol-test.com";

		private const string AdminName = "admin";

		private const string DefaultPassword = "Pa$$w0rd";
	}
}
