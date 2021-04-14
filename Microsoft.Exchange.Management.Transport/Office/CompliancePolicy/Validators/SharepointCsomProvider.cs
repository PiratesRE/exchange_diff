using System;
using System.Globalization;
using System.Linq.Expressions;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Search.Query;

namespace Microsoft.Office.CompliancePolicy.Validators
{
	internal class SharepointCsomProvider : ISharepointCsomProvider
	{
		public void LoadWebInfo(ClientContext context, out string webUrl, out string webTitle, out Guid siteId, out Guid webId)
		{
			Web web = context.Web;
			Site site = context.Site;
			context.Load<Web>(web, new Expression<Func<Web, object>>[0]);
			context.Load<Site>(site, new Expression<Func<Site, object>>[0]);
			context.ExecuteQuery();
			webUrl = web.Url;
			webTitle = web.Title;
			siteId = site.Id;
			webId = web.Id;
		}

		public ResultTableCollection ExecuteSearch(ClientContext context, string location, bool searchOnlySiteCollection)
		{
			SearchExecutor searchExecutor = new SearchExecutor(context);
			KeywordQuery keywordQuery = searchOnlySiteCollection ? SharepointCsomProvider.GetKeywordQueryForSiteCollectionOnly(location, context) : SharepointCsomProvider.GetKeywordQuery(location, context);
			ClientResult<ResultTableCollection> clientResult = searchExecutor.ExecuteQuery(keywordQuery);
			context.ExecuteQuery();
			return clientResult.Value;
		}

		public ResultTableCollection ExecuteSearch(ClientContext context, Guid webId, Guid siteId)
		{
			SearchExecutor searchExecutor = new SearchExecutor(context);
			KeywordQuery keywordQuery = SharepointCsomProvider.GetKeywordQuery(webId, siteId, context);
			ClientResult<ResultTableCollection> clientResult = searchExecutor.ExecuteQuery(keywordQuery);
			context.ExecuteQuery();
			return clientResult.Value;
		}

		private static KeywordQuery GetKeywordQuery(string location, ClientRuntimeContext context)
		{
			KeywordQuery keywordQuery = new KeywordQuery(context);
			keywordQuery.QueryText = string.Format(CultureInfo.InvariantCulture, "Path=\"{0}\"", new object[]
			{
				location
			});
			keywordQuery.RowLimit = 5;
			keywordQuery.SelectProperties.Add("WebId");
			keywordQuery.SelectProperties.Add("SiteId");
			keywordQuery.SelectProperties.Add("contentclass");
			keywordQuery.SelectProperties.Add("Path");
			keywordQuery.SelectProperties.Add("Title");
			SharepointCsomProvider.PopulateQueryDefaults(keywordQuery);
			return keywordQuery;
		}

		private static KeywordQuery GetKeywordQuery(Guid webId, Guid siteId, ClientContext context)
		{
			KeywordQuery keywordQuery = new KeywordQuery(context);
			keywordQuery.RowLimit = 1;
			keywordQuery.QueryText = string.Format(CultureInfo.InvariantCulture, "SiteId:\"{0}\" AND WebId:\"{1}\" AND (contentclass=\"STS_Web\" OR contentclass=\"STS_Site\")", new object[]
			{
				siteId.ToString("D"),
				webId.ToString("D")
			});
			keywordQuery.SelectProperties.Add("contentclass");
			keywordQuery.SelectProperties.Add("Path");
			keywordQuery.SelectProperties.Add("Title");
			SharepointCsomProvider.PopulateQueryDefaults(keywordQuery);
			return keywordQuery;
		}

		private static KeywordQuery GetKeywordQueryForSiteCollectionOnly(string location, ClientContext context)
		{
			KeywordQuery keywordQuery = new KeywordQuery(context);
			keywordQuery.QueryText = string.Format(CultureInfo.InvariantCulture, "Path:\"{0}\" AND contentclass=\"STS_Site\"", new object[]
			{
				location
			});
			keywordQuery.RowLimit = 2;
			SharepointCsomProvider.PopulateQueryDefaults(keywordQuery);
			return keywordQuery;
		}

		private static void PopulateQueryDefaults(KeywordQuery query)
		{
			query.ProcessBestBets = false;
			query.BypassResultTypes = true;
			query.Properties["EnableStacking"] = false;
			query.EnableStemming = false;
			query.EnableQueryRules = false;
		}

		private const string PathMatchFormat = "Path=\"{0}\"";

		private const string SiteAndWebExactMatchFormat = "SiteId:\"{0}\" AND WebId:\"{1}\" AND (contentclass=\"STS_Web\" OR contentclass=\"STS_Site\")";

		private const string SiteCollectionOnlyPathMatchFormat = "Path:\"{0}\" AND contentclass=\"STS_Site\"";

		private const string EnableResultTableStackingPropertyName = "EnableStacking";
	}
}
