using System;
using Microsoft.Exchange.Hygiene.Data.Directory;

namespace Microsoft.Forefront.Reporting.OnDemandQuery
{
	public static class OnDemandQueryHelper
	{
		internal static string GetSchemaUrl(OnDemandQueryRequest request)
		{
			return OnDemandQueryHelper.GetSchemaUrl(request.CosmosResultUri, request.QueryType.ToString());
		}

		internal static string GetSchemaUrl(string cosmosResultUrl, string schemaName)
		{
			if (string.IsNullOrWhiteSpace(cosmosResultUrl))
			{
				return string.Empty;
			}
			string arg = cosmosResultUrl.Substring(0, cosmosResultUrl.LastIndexOf("/Results/"));
			return string.Format("{0}/{1}.schema", arg, schemaName);
		}
	}
}
