using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	internal enum GetUcwaUserConfigurationMetaData
	{
		[DisplayName("UCWA", "MS")]
		ManagerSipUri,
		[DisplayName("UCWA", "ORG")]
		Organization,
		[DisplayName("UCWA", "IUS")]
		IsUcwaSupported,
		[DisplayName("UCWA", "ALS")]
		AuthenticatedLyncAutodiscoverServer,
		[DisplayName("UCWA", "AFC")]
		IsAuthdServerFromCache,
		[DisplayName("UCWA", "URL")]
		UcwaUrl,
		[DisplayName("UCWA", "UFC")]
		IsUcwaUrlFromCache,
		[DisplayName("UCWA", "OCID")]
		OAuthCorrelationId,
		[DisplayName("UCWA", "URH")]
		UnauthenticatedRedirectHops,
		[DisplayName("UCWA", "ARH")]
		AuthenticatedRedirectHops,
		[DisplayName("UCWA", "ITC")]
		IsTaskCompleted,
		[DisplayName("UCWA", "EX")]
		Exceptions,
		[DisplayName("UCWA", "WEX")]
		WorkerExceptions,
		[DisplayName("UCWA", "RQH")]
		RequestHeaders,
		[DisplayName("UCWA", "RSH")]
		ResponseHeaders,
		[DisplayName("UCWA", "RSB")]
		ResponseBody,
		[DisplayName("UCWA", "CO")]
		CacheOperation
	}
}
