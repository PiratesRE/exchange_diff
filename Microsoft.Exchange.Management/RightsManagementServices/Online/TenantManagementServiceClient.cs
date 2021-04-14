using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.RightsManagementServices.Online
{
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	[DebuggerStepThrough]
	public class TenantManagementServiceClient : ClientBase<ITenantManagementService>, ITenantManagementService
	{
		public TenantManagementServiceClient()
		{
		}

		public TenantManagementServiceClient(string endpointConfigurationName) : base(endpointConfigurationName)
		{
		}

		public TenantManagementServiceClient(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}

		public TenantManagementServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}

		public TenantManagementServiceClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
		{
		}

		public TenantInfo EnrollTenant(TenantEnrollmentInfo enrollmentInfo)
		{
			return base.Channel.EnrollTenant(enrollmentInfo);
		}

		public TenantPublishingMessage CreateOrUpdateTenant(TenantProvisioningMessage provisioningMsg)
		{
			return base.Channel.CreateOrUpdateTenant(provisioningMsg);
		}

		public TenantInfo[] GetTenantInfoByCookie(out string nextCookie, string cookie, int maxTenants)
		{
			return base.Channel.GetTenantInfoByCookie(out nextCookie, cookie, maxTenants);
		}

		public TenantInfo[] GetTenantInfo(string[] tenantIds)
		{
			return base.Channel.GetTenantInfo(tenantIds);
		}
	}
}
