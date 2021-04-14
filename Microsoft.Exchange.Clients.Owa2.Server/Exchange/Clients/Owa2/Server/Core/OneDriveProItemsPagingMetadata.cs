using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class OneDriveProItemsPagingMetadata : AttachmentItemsPagingMetadata
	{
		[DataMember]
		public string ChangeToken { get; set; }

		[DataMember]
		public OneDriveProItemsPage[] PageCache
		{
			get
			{
				return this.PageMap.Values.ToArray<OneDriveProItemsPage>();
			}
			set
			{
				this.CreatePageMapFromArray(value);
			}
		}

		internal IEnumerable<IListItem> GetItems(UserContext userContext, string endPointUrl, string documentLibrary, string location, IndexedPageView requestedData, AttachmentItemsSort sort, out int totalItemCount, DataProviderCallLogEvent logEvent)
		{
			IEnumerable<IListItem> result;
			using (IClientContext clientContext = OneDriveProUtilities.CreateAndConfigureClientContext(userContext.LogonIdentity, endPointUrl))
			{
				totalItemCount = 0;
				IList documentsLibrary = OneDriveProUtilities.GetDocumentsLibrary(clientContext, documentLibrary);
				OneDriveProItemsPage page = this.UpdatePageCache(clientContext, userContext, documentsLibrary, documentLibrary, location, requestedData, sort, logEvent);
				CamlQuery camlDataQuery = this.GetCamlDataQuery(location, requestedData, this.GetListItemCollectionPosition(page), sort);
				IListItemCollection items = documentsLibrary.GetItems(camlDataQuery);
				IFolder folder = string.IsNullOrEmpty(location) ? documentsLibrary.RootFolder : clientContext.Web.GetFolderByServerRelativeUrl(location);
				items.Load(clientContext, new Expression<Func<ListItemCollection, object>>[0]);
				folder.Load(clientContext, new Expression<Func<Folder, object>>[]
				{
					(Folder x) => (object)x.ItemCount
				});
				OneDriveProUtilities.ExecuteQueryWithTraces(userContext, clientContext, logEvent, "GetItems");
				int startIndex = requestedData.Offset % 200;
				int endIndex = startIndex + requestedData.MaxRows;
				totalItemCount = folder.ItemCount;
				result = items.ToList<IListItem>().Where((IListItem item, int index) => index >= startIndex && index < endIndex);
			}
			return result;
		}

		private ConcurrentDictionary<int, OneDriveProItemsPage> PageMap
		{
			get
			{
				if (this.pageMap == null)
				{
					this.pageMap = new ConcurrentDictionary<int, OneDriveProItemsPage>();
				}
				return this.pageMap;
			}
		}

		private OneDriveProItemsPage UpdatePageCache(IClientContext clientContext, UserContext userContext, IList list, string listName, string location, IndexedPageView requestedData, AttachmentItemsSort sort, DataProviderCallLogEvent logEvent)
		{
			string changeToken;
			bool flag;
			this.GetListItemChangesSinceToken(clientContext, userContext.LogonIdentity, listName, location, out changeToken, out flag, logEvent);
			this.ChangeToken = changeToken;
			if (flag)
			{
				this.PageMap.Clear();
			}
			int num = this.ComputeStartPageIndex(requestedData);
			OneDriveProItemsPage nearestPage = this.GetNearestPage(num);
			int num2 = (nearestPage != null) ? nearestPage.PageIndex : -1;
			if (nearestPage == null || num != nearestPage.PageIndex)
			{
				ListItemCollectionPosition listItemCollectionPosition = this.GetListItemCollectionPosition(nearestPage);
				CamlQuery query = OneDriveProUtilities.CreatePagedCamlPageQuery(location, sort, listItemCollectionPosition, Math.Abs(num - num2) * 200 + 200);
				IListItemCollection items = list.GetItems(query);
				items.Load(clientContext, new Expression<Func<ListItemCollection, object>>[0]);
				OneDriveProUtilities.ExecuteQueryWithTraces(userContext, clientContext, logEvent, "UpdatePageCache");
				this.UpdateCache(items, nearestPage);
			}
			OneDriveProItemsPage result;
			this.PageMap.TryGetValue(num, out result);
			return result;
		}

		private CamlQuery GetCamlDataQuery(string location, IndexedPageView requestedData, ListItemCollectionPosition position, AttachmentItemsSort sort)
		{
			int numberOfItems = requestedData.Offset % 200 + requestedData.MaxRows;
			return OneDriveProUtilities.CreatePagedCamlDataQuery(location, sort, position, numberOfItems);
		}

		private void GetListItemChangesSinceToken(IClientContext context, OwaIdentity identity, string listName, string location, out string changeToken, out bool hasChanges, DataProviderCallLogEvent logEvent)
		{
			changeToken = null;
			hasChanges = false;
			DownloadResult downloadResult = OneDriveProUtilities.SendRestRequest("POST", string.Format("{0}/_vti_bin/client.svc/web/lists/getByTitle('{1}')/GetListItemChangesSinceToken", context.Url, listName), identity, this.GetRequestStream(location, this.ChangeToken), logEvent, "GetListItemChangesSinceToken");
			if (!downloadResult.IsSucceeded)
			{
				OneDriveProItemsPagingMetadata.TraceError(OneDriveProItemsPagingMetadata.LogMetadata.GetListItemChangesSinceToken, downloadResult.Exception);
				hasChanges = true;
				return;
			}
			using (XmlReader xmlReader = XmlReader.Create(downloadResult.ResponseStream))
			{
				while (xmlReader.Read())
				{
					if (xmlReader.NodeType == XmlNodeType.Element)
					{
						if (xmlReader.LocalName == "Changes")
						{
							changeToken = xmlReader.GetAttribute("LastChangeToken");
						}
						else if (xmlReader.LocalName == "row" && xmlReader.NamespaceURI == "#RowsetSchema")
						{
							hasChanges = true;
							break;
						}
					}
				}
			}
		}

		private Stream GetRequestStream(string location, string changeToken)
		{
			StringBuilder stringBuilder = new StringBuilder("{'query':{'__metadata':{'type':'SP.ChangeLogItemQuery'},'Query':'',");
			stringBuilder.Append("'ViewFields':'<ViewFields><FieldRef Name=\"ID\"/></ViewFields>',");
			stringBuilder.Append("'RowLimit':'1',");
			if (!string.IsNullOrEmpty(changeToken))
			{
				stringBuilder.AppendFormat("'ChangeToken':'{0}',", changeToken);
			}
			stringBuilder.Append("'QueryOptions':'");
			using (StringWriter stringWriter = new StringWriter(stringBuilder))
			{
				using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
				{
					xmlTextWriter.WriteStartElement("QueryOptions");
					xmlTextWriter.WriteElementString("DateInUtc", "TRUE");
					xmlTextWriter.WriteElementString("OptimizeFor", "FolderUrls");
					xmlTextWriter.WriteStartElement("ViewAttributes");
					xmlTextWriter.WriteAttributeString("Scope", "Default");
					xmlTextWriter.WriteEndElement();
					if (!string.IsNullOrEmpty(location))
					{
						xmlTextWriter.WriteElementString("Folder", location);
					}
					xmlTextWriter.WriteEndElement();
				}
			}
			stringBuilder.Append("'}}");
			byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
			return new MemoryStream(bytes, 0, bytes.Length);
		}

		private void UpdateCache(IListItemCollection itemCollection, OneDriveProItemsPage page)
		{
			int num = (page == null) ? -1 : page.PageIndex;
			for (int i = 199; i < itemCollection.Count(); i += 200)
			{
				IListItem item = itemCollection[i];
				int num2 = i / 200 + num + 1;
				this.PageMap[num2] = new OneDriveProItemsPage(num2, item);
			}
		}

		private ListItemCollectionPosition GetListItemCollectionPosition(OneDriveProItemsPage page)
		{
			if (page != null)
			{
				return new ListItemCollectionPosition
				{
					PagingInfo = string.Format("Paged=TRUE&p_SortBehavior={0}&p_FSObjType={1}&p_FileLeafRef={2}&p_ID={3}", new object[]
					{
						page.SortBehavior,
						page.ObjectType,
						page.Name,
						page.ID
					})
				};
			}
			return null;
		}

		private OneDriveProItemsPage GetNearestPage(int pageIndex)
		{
			IOrderedEnumerable<int> orderedEnumerable = from x in this.PageMap.Keys
			orderby x
			select x;
			int num = -1;
			foreach (int num2 in orderedEnumerable)
			{
				if (num2 > pageIndex)
				{
					break;
				}
				num = num2;
			}
			if (num == -1)
			{
				return null;
			}
			return this.PageMap[num];
		}

		private int ComputeStartPageIndex(IndexedPageView requestedData)
		{
			return requestedData.Offset / 200 - 1;
		}

		private void CreatePageMapFromArray(OneDriveProItemsPage[] pages)
		{
			this.PageMap.Clear();
			foreach (OneDriveProItemsPage oneDriveProItemsPage in pages)
			{
				this.PageMap[oneDriveProItemsPage.PageIndex] = oneDriveProItemsPage;
			}
		}

		private static void TraceError(OneDriveProItemsPagingMetadata.LogMetadata error, object data)
		{
			OwaApplication.GetRequestDetailsLogger.Set(error, data);
		}

		private const string ViewFieldsJson = "'ViewFields':'<ViewFields><FieldRef Name=\"ID\"/></ViewFields>',";

		private const string RowLimitJson = "'RowLimit':'1',";

		private const string QueryOptionsJsonPrefix = "'QueryOptions':'";

		private const string QueryOptionsElementName = "QueryOptions";

		private const string ChangeTokenJsonFormat = "'ChangeToken':'{0}',";

		private const string DateInUtcElementName = "DateInUtc";

		private const string OptimizeForElementName = "OptimizeFor";

		private const string ViewAttributesElementName = "ViewAttributes";

		private const string ScopeAttributeName = "Scope";

		private const string FolderElementName = "Folder";

		private const string ChangesElementName = "Changes";

		private const string RowElementName = "row";

		private const string RowsetSchemaNamespaceUri = "#RowsetSchema";

		private const string LastChangeTokenAttributeName = "LastChangeToken";

		private const string TrueValue = "TRUE";

		private const string FolderUrlsValue = "FolderUrls";

		private const string DefaultValue = "Default";

		private const string GetListItemChangesSinceLastTokenUrlFormat = "{0}/_vti_bin/client.svc/web/lists/getByTitle('{1}')/GetListItemChangesSinceToken";

		private const string GetListItemChangesSinceLastTokenJsonPrefix = "{'query':{'__metadata':{'type':'SP.ChangeLogItemQuery'},'Query':'',";

		private const string GetListItemChangesSinceLastTokenJsonSuffix = "'}}";

		private const string PostMethod = "POST";

		private const string ListItemCollectionPositionFormat = "Paged=TRUE&p_SortBehavior={0}&p_FSObjType={1}&p_FileLeafRef={2}&p_ID={3}";

		internal const int PageSize = 200;

		private ConcurrentDictionary<int, OneDriveProItemsPage> pageMap = new ConcurrentDictionary<int, OneDriveProItemsPage>();

		private enum LogMetadata
		{
			[DisplayName("SDPP.GLICST")]
			GetListItemChangesSinceToken
		}
	}
}
