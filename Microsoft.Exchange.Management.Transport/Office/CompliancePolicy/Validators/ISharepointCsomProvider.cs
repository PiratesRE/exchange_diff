using System;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Search.Query;

namespace Microsoft.Office.CompliancePolicy.Validators
{
	internal interface ISharepointCsomProvider
	{
		void LoadWebInfo(ClientContext context, out string webUrl, out string webTitle, out Guid siteId, out Guid webId);

		ResultTableCollection ExecuteSearch(ClientContext context, string location, bool searchOnlySiteCollection);

		ResultTableCollection ExecuteSearch(ClientContext context, Guid webId, Guid siteId);
	}
}
