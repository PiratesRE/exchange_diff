using System;
using System.CodeDom.Compiler;
using System.ServiceModel;

namespace Microsoft.RightsManagementServices.Online
{
	[ServiceContract(Namespace = "http://microsoft.com/RightsManagementServiceOnline/2011/04", ConfigurationName = "Microsoft.RightsManagementServices.Online.ITenantManagementService")]
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	public interface ITenantManagementService
	{
		[FaultContract(typeof(ArgumentException), Action = "http://microsoft.com/RightsManagementServiceOnline/2011/04/ITenantManagementService/EnrollTenantArgumentExceptionFault", Name = "ArgumentException", Namespace = "http://schemas.datacontract.org/2004/07/System")]
		[OperationContract(Action = "http://microsoft.com/RightsManagementServiceOnline/2011/04/ITenantManagementService/EnrollTenant", ReplyAction = "http://microsoft.com/RightsManagementServiceOnline/2011/04/ITenantManagementService/EnrollTenantResponse")]
		TenantInfo EnrollTenant(TenantEnrollmentInfo enrollmentInfo);

		[OperationContract(Action = "http://microsoft.com/RightsManagementServiceOnline/2011/04/ITenantManagementService/CreateOrUpdateTenant", ReplyAction = "http://microsoft.com/RightsManagementServiceOnline/2011/04/ITenantManagementService/CreateOrUpdateTenantResponse")]
		[FaultContract(typeof(CommonFault), Action = "http://microsoft.com/RightsManagementServiceOnline/2011/04/ITenantManagementService/CreateOrUpdateTenantCommonFaultFault", Name = "CommonFault")]
		TenantPublishingMessage CreateOrUpdateTenant(TenantProvisioningMessage provisioningMsg);

		[FaultContract(typeof(ArgumentException), Action = "http://microsoft.com/RightsManagementServiceOnline/2011/04/ITenantManagementService/GetTenantInfoByCookieArgumentExceptionFault", Name = "ArgumentException", Namespace = "http://schemas.datacontract.org/2004/07/System")]
		[OperationContract(Action = "http://microsoft.com/RightsManagementServiceOnline/2011/04/ITenantManagementService/GetTenantInfoByCookie", ReplyAction = "http://microsoft.com/RightsManagementServiceOnline/2011/04/ITenantManagementService/GetTenantInfoByCookieResponse")]
		TenantInfo[] GetTenantInfoByCookie(out string nextCookie, string cookie, int maxTenants);

		[FaultContract(typeof(ArgumentException), Action = "http://microsoft.com/RightsManagementServiceOnline/2011/04/ITenantManagementService/GetTenantInfoArgumentExceptionFault", Name = "ArgumentException", Namespace = "http://schemas.datacontract.org/2004/07/System")]
		[OperationContract(Action = "http://microsoft.com/RightsManagementServiceOnline/2011/04/ITenantManagementService/GetTenantInfo", ReplyAction = "http://microsoft.com/RightsManagementServiceOnline/2011/04/ITenantManagementService/GetTenantInfoResponse")]
		TenantInfo[] GetTenantInfo(string[] tenantIds);
	}
}
