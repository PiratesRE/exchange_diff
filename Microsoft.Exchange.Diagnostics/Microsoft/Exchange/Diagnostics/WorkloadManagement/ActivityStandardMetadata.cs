using System;

namespace Microsoft.Exchange.Diagnostics.WorkloadManagement
{
	internal enum ActivityStandardMetadata
	{
		UserId,
		Puid,
		UserEmail,
		AuthenticationType,
		AuthenticationToken,
		TenantId,
		TenantType,
		Component,
		ComponentInstance,
		Feature,
		Protocol,
		ClientInfo,
		Action,
		ClientRequestId,
		ReturnClientRequestId,
		TenantStatus
	}
}
