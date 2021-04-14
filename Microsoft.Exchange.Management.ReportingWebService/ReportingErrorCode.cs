using System;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	internal enum ReportingErrorCode
	{
		ErrorMissingTenantDomainInRequest,
		ErrorTenantNotInOrgScope,
		ErrorTenantNotResolved,
		InvalidFormatQuery,
		ErrorSchemaInitializationFail,
		ErrorOAuthPartnerApplicationNotLinkToServiceAccount,
		ErrorOAuthAuthorizationNoAccount,
		ErrorVersionAmbiguous,
		ErrorInvalidVersion,
		ErrorOverBudget,
		ADTransientError,
		ADOperationError,
		CreateRunspaceConfigTimeoutError,
		ConnectionFailedException,
		InvalidQueryException,
		DataMartTimeoutException,
		UnknownError
	}
}
