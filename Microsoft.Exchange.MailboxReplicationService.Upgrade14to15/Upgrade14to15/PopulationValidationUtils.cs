using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.MailboxReplicationService.Upgrade14to15.TestTenantManagement;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	public static class PopulationValidationUtils
	{
		public static List<Guid> GetTenantsBasedOnPopulationStatus(PopulationStatus status, string testTenantManagementUrl = "https://tws.365upgrade.microsoftonline.com/TestTenantManagementService.svc", string serviceInstancePrefix = "Exchange/namprd03")
		{
			List<Guid> list = new List<Guid>();
			using (ProxyWrapper<TestTenantManagementClient, ITestTenantManagement> proxyWrapper = new ProxyWrapper<TestTenantManagementClient, ITestTenantManagement>(new Uri(testTenantManagementUrl), PopulationValidationUtils.TestTenantManagementCert))
			{
				Tenant[] array = proxyWrapper.Proxy.QueryTenantsToPopulate(status);
				foreach (Tenant tenant in array)
				{
					if (string.IsNullOrEmpty(serviceInstancePrefix) || tenant.ExchangeServiceInstance.StartsWith(serviceInstancePrefix, true, CultureInfo.InvariantCulture))
					{
						list.Add(tenant.TenantId);
					}
				}
			}
			return list;
		}

		public static void SetPopulationStatus(Guid tenantId, PopulationStatus status, string testTenantManagementUrl = "https://tws.365upgrade.microsoftonline.com/TestTenantManagementService.svc")
		{
			using (ProxyWrapper<TestTenantManagementClient, ITestTenantManagement> proxyWrapper = new ProxyWrapper<TestTenantManagementClient, ITestTenantManagement>(new Uri(testTenantManagementUrl), PopulationValidationUtils.TestTenantManagementCert))
			{
				proxyWrapper.Proxy.UpdateTenantPopulationStatus(tenantId, status);
			}
		}

		public static List<Guid> GetTenantsBasedOnValidationStatus(ValidationStatus status, string testTenantManagementUrl = "https://tws.365upgrade.microsoftonline.com/TestTenantManagementService.svc", string serviceInstancePrefix = "Exchange/namprd03")
		{
			List<Guid> list = new List<Guid>();
			using (ProxyWrapper<TestTenantManagementClient, ITestTenantManagement> proxyWrapper = new ProxyWrapper<TestTenantManagementClient, ITestTenantManagement>(new Uri(testTenantManagementUrl), PopulationValidationUtils.TestTenantManagementCert))
			{
				Tenant[] array = proxyWrapper.Proxy.QueryTenantsToValidate(status);
				foreach (Tenant tenant in array)
				{
					if (string.IsNullOrEmpty(serviceInstancePrefix) || tenant.ExchangeServiceInstance.StartsWith(serviceInstancePrefix, true, CultureInfo.InvariantCulture))
					{
						list.Add(tenant.TenantId);
					}
				}
			}
			return list;
		}

		public static void SetValidationStatus(Guid tenantId, ValidationStatus status, int? bugsId, string testTenantManagementUrl = "https://tws.365upgrade.microsoftonline.com/TestTenantManagementService.svc")
		{
			using (ProxyWrapper<TestTenantManagementClient, ITestTenantManagement> proxyWrapper = new ProxyWrapper<TestTenantManagementClient, ITestTenantManagement>(new Uri(testTenantManagementUrl), PopulationValidationUtils.TestTenantManagementCert))
			{
				proxyWrapper.Proxy.UpdateTenantValidationStatus(tenantId, status, bugsId);
			}
		}

		private const string TestTenantCertificateSubject = "CN=auth.outlook.com, OU=Exchange, O=Microsoft Corporation, L=Redmond, S=Washington, C=US";

		private static readonly X509Certificate2 TestTenantManagementCert = CertificateHelper.GetExchangeCertificate("CN=auth.outlook.com, OU=Exchange, O=Microsoft Corporation, L=Redmond, S=Washington, C=US");
	}
}
