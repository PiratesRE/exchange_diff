using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal static class OneDriveProUtilities
	{
		internal static string UserAgentString
		{
			get
			{
				if (string.IsNullOrEmpty(OneDriveProUtilities.userAgentString))
				{
					OneDriveProUtilities.userAgentString = "Exchange/" + Globals.ApplicationVersion + "/Outlook Web App";
				}
				return OneDriveProUtilities.userAgentString;
			}
		}

		internal static WebHeaderCollection GetOAuthRequestHeaders()
		{
			WebHeaderCollection webHeaderCollection = new WebHeaderCollection();
			webHeaderCollection.Add(HttpRequestHeader.Authorization, "Bearer");
			webHeaderCollection["X-RequestForceAuthentication"] = "true";
			webHeaderCollection["client-request-id"] = Guid.NewGuid().ToString();
			webHeaderCollection["return-client-request-id"] = "true";
			return webHeaderCollection;
		}

		internal static CamlQuery CreatePagedCamlPageQuery(string location, AttachmentItemsSort sort, ListItemCollectionPosition listItemCollectionPosition, int numberOfItems)
		{
			return OneDriveProUtilities.CreatePagedCamlQuery(location, sort, listItemCollectionPosition, numberOfItems, "<View>\r\n                                                    <Query>\r\n                                                        <OrderBy>\r\n                                                            <FieldRef Name='FSObjType' Ascending='FALSE' />\r\n                                                            <FieldRef Name='{0}' Ascending='{1}' />\r\n                                                        </OrderBy>\r\n                                                    </Query>\r\n                                                    <ViewFields>\r\n                                                        <FieldRef Name='ID' />\r\n                                                        <FieldRef Name='FileLeafRef' />\r\n                                                        <FieldRef Name='FSObjType' />\r\n                                                        <FieldRef Name='SortBehavior' />\r\n                                                    </ViewFields>\r\n                                                    <RowLimit>{2}</RowLimit>\r\n                                                </View>");
		}

		internal static CamlQuery CreatePagedCamlDataQuery(string location, AttachmentItemsSort sort, ListItemCollectionPosition listItemCollectionPosition, int numberOfItems)
		{
			return OneDriveProUtilities.CreatePagedCamlQuery(location, sort, listItemCollectionPosition, numberOfItems, "<View>\r\n                                                <Query>\r\n                                                    <OrderBy>\r\n                                                        <FieldRef Name='FSObjType' Ascending='FALSE' />\r\n                                                        <FieldRef Name='{0}' Ascending='{1}' />\r\n                                                    </OrderBy>\r\n                                                </Query>\r\n                                                <ViewFields>\r\n                                                    <FieldRef Name='ID' />\r\n                                                    <FieldRef Name='FileLeafRef' />\r\n                                                    <FieldRef Name='FSObjType' />\r\n                                                    <FieldRef Name='FileRef' />\r\n                                                    <FieldRef Name='File_x0020_Size' />\r\n                                                    <FieldRef Name='Modified' />\r\n                                                    <FieldRef Name='Editor' />\r\n                                                    <FieldRef Name='ItemChildCount' />\r\n                                                    <FieldRef Name='FolderChildCount' />\r\n                                                    <FieldRef Name='ProgId' />\r\n                                                </ViewFields><RowLimit>{2}</RowLimit></View>");
		}

		internal static CamlQuery CreateCamlDataQuery(string location, AttachmentItemsSort sort)
		{
			return OneDriveProUtilities.CreateCamlQuery(location, null, string.Format("<View>\r\n                                                <Query>\r\n                                                    <OrderBy>\r\n                                                        <FieldRef Name='FSObjType' Ascending='FALSE' />\r\n                                                        <FieldRef Name='{0}' Ascending='{1}' />\r\n                                                    </OrderBy>\r\n                                                </Query>\r\n                                                <ViewFields>\r\n                                                    <FieldRef Name='ID' />\r\n                                                    <FieldRef Name='FileLeafRef' />\r\n                                                    <FieldRef Name='FSObjType' />\r\n                                                    <FieldRef Name='FileRef' />\r\n                                                    <FieldRef Name='File_x0020_Size' />\r\n                                                    <FieldRef Name='Modified' />\r\n                                                    <FieldRef Name='Editor' />\r\n                                                    <FieldRef Name='ItemChildCount' />\r\n                                                    <FieldRef Name='FolderChildCount' />\r\n                                                    <FieldRef Name='ProgId' />\r\n                                                </ViewFields></View>", OneDriveProUtilities.GetSortColumn(sort.SortColumn), OneDriveProUtilities.GetSortOrder(sort.SortOrder)));
		}

		internal static IClientContext CreateAndConfigureClientContext(OwaIdentity identity, string url)
		{
			ICredentials oneDriveProCredentials = OneDriveProUtilities.GetOneDriveProCredentials(identity);
			IClientContext clientContext = ClientContextFactory.Create(url);
			clientContext.Credentials = oneDriveProCredentials;
			clientContext.FormDigestHandlingEnabled = false;
			clientContext.ExecutingWebRequest += delegate(object sender, WebRequestEventArgs args)
			{
				if (args != null)
				{
					args.WebRequestExecutor.RequestHeaders.Add(OneDriveProUtilities.GetOAuthRequestHeaders());
					args.WebRequestExecutor.WebRequest.PreAuthenticate = true;
					args.WebRequestExecutor.WebRequest.UserAgent = OneDriveProUtilities.UserAgentString;
				}
			};
			return clientContext;
		}

		internal static ICredentials GetOneDriveProCredentials(OwaIdentity identity)
		{
			return OauthUtils.GetOauthCredential(identity.GetOWAMiniRecipient());
		}

		internal static DownloadResult SendRestRequest(string requestMethod, string requestUri, OwaIdentity identity, Stream requestStream, DataProviderCallLogEvent logEvent, string spCallName)
		{
			DownloadResult result;
			using (HttpClient httpClient = new HttpClient())
			{
				HttpSessionConfig httpSessionConfig = new HttpSessionConfig
				{
					Method = requestMethod,
					Credentials = OauthUtils.GetOauthCredential(identity.GetOWAMiniRecipient()),
					UserAgent = OneDriveProUtilities.UserAgentString,
					RequestStream = requestStream,
					ContentType = "application/json;odata=verbose",
					PreAuthenticate = true
				};
				httpSessionConfig.Headers = OneDriveProUtilities.GetOAuthRequestHeaders();
				if (logEvent != null)
				{
					logEvent.TrackSPCallBegin();
				}
				ICancelableAsyncResult cancelableAsyncResult = httpClient.BeginDownload(new Uri(requestUri), httpSessionConfig, null, null);
				cancelableAsyncResult.AsyncWaitHandle.WaitOne();
				DownloadResult downloadResult = httpClient.EndDownload(cancelableAsyncResult);
				if (logEvent != null)
				{
					string correlationId = (downloadResult.ResponseHeaders == null) ? null : downloadResult.ResponseHeaders["SPRequestGuid"];
					logEvent.TrackSPCallEnd(spCallName, correlationId);
				}
				result = downloadResult;
			}
			return result;
		}

		internal static bool IsDurableUrlFormat(string documentUrl)
		{
			return !string.IsNullOrEmpty(documentUrl) && documentUrl.LastIndexOf(".", StringComparison.InvariantCulture) < documentUrl.LastIndexOf("?d=", StringComparison.InvariantCulture);
		}

		internal static string GetWacUrl(OwaIdentity identity, string endPointUrl, string documentUrl, bool isEdit)
		{
			string arg = isEdit ? "2" : "4";
			string text = string.Format("{0}/_api/Microsoft.SharePoint.Yammer.WACAPI.GetWacToken(fileUrl=@p, wopiAction={2})?@p='{1}'", endPointUrl, documentUrl, arg);
			string result;
			using (HttpClient httpClient = new HttpClient())
			{
				OWAMiniRecipient owaminiRecipient = identity.GetOWAMiniRecipient();
				ICredentials oauthCredential = OauthUtils.GetOauthCredential(owaminiRecipient);
				WebHeaderCollection oauthRequestHeaders = OneDriveProUtilities.GetOAuthRequestHeaders();
				HttpSessionConfig sessionConfig = new HttpSessionConfig
				{
					Method = "GET",
					Credentials = oauthCredential,
					UserAgent = OneDriveProUtilities.UserAgentString,
					ContentType = "application/json;odata=verbose",
					PreAuthenticate = true,
					Headers = oauthRequestHeaders
				};
				DownloadResult downloadResult;
				try
				{
					downloadResult = OneDriveProUtilities.TryTwice(httpClient, sessionConfig, text);
				}
				catch (WebException ex)
				{
					if (!OneDriveProUtilities.IsDurableUrlFormat(documentUrl))
					{
						throw ex;
					}
					ExTraceGlobals.AttachmentHandlingTracer.TraceWarning<string>(0L, "OneDriveProUtilities.GetWacUrl Exception while trying to get wac token using durable url. : {0}", ex.StackTrace);
					documentUrl = documentUrl.Substring(0, documentUrl.LastIndexOf("?", StringComparison.InvariantCulture));
					text = string.Format("{0}/_api/Microsoft.SharePoint.Yammer.WACAPI.GetWacToken(fileUrl=@p, wopiAction={2})?@p='{1}'", endPointUrl, documentUrl, arg);
					ExTraceGlobals.AttachmentHandlingTracer.TraceWarning<string>(0L, "OneDriveProUtilities.GetWacUrl Fallback to canonical url format: {0}", text);
					OwaServerTraceLogger.AppendToLog(new TraceLogEvent("SP.GWT", null, "GetWacToken", string.Format("Error getting WAC Token fallback to canonical format:{0}", text)));
					downloadResult = OneDriveProUtilities.TryTwice(httpClient, sessionConfig, text);
				}
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(downloadResult.ResponseStream);
				string namespaceURI = "http://schemas.microsoft.com/ado/2007/08/dataservices";
				string text2 = null;
				string text3 = null;
				string text4 = null;
				foreach (object obj in xmlDocument.GetElementsByTagName("*", namespaceURI))
				{
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode is XmlElement)
					{
						if (text2 != null && text3 != null && text4 != null)
						{
							break;
						}
						if (string.CompareOrdinal(xmlNode.LocalName, "AppUrl") == 0)
						{
							text2 = xmlNode.InnerText;
						}
						else if (string.CompareOrdinal(xmlNode.LocalName, "AccessToken") == 0)
						{
							text3 = xmlNode.InnerText;
						}
						else if (string.CompareOrdinal(xmlNode.LocalName, "AccessTokenTtl") == 0)
						{
							text4 = xmlNode.InnerText;
						}
					}
				}
				if (text2 == null || text3 == null || text4 == null)
				{
					throw new OwaException("SharePoint's GetWacToken response is not usable.");
				}
				string text5 = isEdit ? "OwaEdit" : "OwaView";
				result = string.Format("{0}&access_token={1}&access_token_ttl={2}&sc={3}", new object[]
				{
					text2,
					text3,
					text4,
					text5
				});
			}
			return result;
		}

		internal static void ExecuteQueryWithTraces(UserContext userContext, IClientContext context, DataProviderCallLogEvent logEvent, string spCallName)
		{
			try
			{
				if (logEvent != null)
				{
					logEvent.TrackSPCallBegin();
				}
				context.ExecuteQuery();
			}
			finally
			{
				if (logEvent != null)
				{
					logEvent.TrackSPCallEnd(spCallName, context.TraceCorrelationId);
				}
				OneDriveProUtilities.SendPendingGetNotification(userContext, new AttachmentOperationCorrelationIdNotificationPayload
				{
					CorrelationId = context.TraceCorrelationId,
					SharePointCallName = spCallName
				});
			}
		}

		internal static IList GetDocumentsLibrary(IClientContext context, string documentLibraryName)
		{
			string text = new Uri(context.Url).LocalPath;
			if (!text.EndsWith("/"))
			{
				text += "/";
			}
			return context.Web.GetList(text + documentLibraryName);
		}

		private static void SendPendingGetNotification(UserContext userContext, AttachmentOperationCorrelationIdNotificationPayload payload)
		{
			if (userContext.IsGroupUserContext)
			{
				return;
			}
			if (!userContext.IsDisposed)
			{
				AttachmentOperationCorrelationIdNotifier attachmentOperationCorrelationIdNotifier = new AttachmentOperationCorrelationIdNotifier(userContext, payload.SubscriptionId);
				try
				{
					attachmentOperationCorrelationIdNotifier.RegisterWithPendingRequestNotifier();
					attachmentOperationCorrelationIdNotifier.Payload = payload;
					attachmentOperationCorrelationIdNotifier.PickupData();
				}
				finally
				{
					attachmentOperationCorrelationIdNotifier.UnregisterWithPendingRequestNotifier();
				}
			}
		}

		private static DownloadResult TryTwice(HttpClient httpClient, HttpSessionConfig sessionConfig, string url)
		{
			ICancelableAsyncResult cancelableAsyncResult = httpClient.BeginDownload(new Uri(url), sessionConfig, null, null);
			cancelableAsyncResult.AsyncWaitHandle.WaitOne();
			DownloadResult result = httpClient.EndDownload(cancelableAsyncResult);
			if (result.Exception != null)
			{
				if (!result.IsRetryable)
				{
					throw result.Exception;
				}
				cancelableAsyncResult = httpClient.BeginDownload(new Uri(url), sessionConfig, null, null);
				cancelableAsyncResult.AsyncWaitHandle.WaitOne();
				result = httpClient.EndDownload(cancelableAsyncResult);
				if (result.Exception != null)
				{
					throw result.Exception;
				}
			}
			return result;
		}

		private static CamlQuery CreatePagedCamlQuery(string location, AttachmentItemsSort sort, ListItemCollectionPosition listItemCollectionPosition, int numberOfItems, string viewXmlFormat)
		{
			return OneDriveProUtilities.CreateCamlQuery(location, listItemCollectionPosition, string.Format(viewXmlFormat, OneDriveProUtilities.GetSortColumn(sort.SortColumn), OneDriveProUtilities.GetSortOrder(sort.SortOrder), numberOfItems));
		}

		private static CamlQuery CreateCamlQuery(string location, ListItemCollectionPosition listItemCollectionPosition, string viewXml)
		{
			CamlQuery camlQuery = new CamlQuery
			{
				ViewXml = viewXml
			};
			if (!string.IsNullOrEmpty(location))
			{
				camlQuery.FolderServerRelativeUrl = location;
			}
			camlQuery.ListItemCollectionPosition = listItemCollectionPosition;
			return camlQuery;
		}

		private static string GetSortColumn(AttachmentItemsSortColumn column)
		{
			OneDriveProUtilities.EnsureColumnMap();
			return OneDriveProUtilities.columnToOneDriveProColumn[column];
		}

		private static string GetSortOrder(AttachmentItemsSortOrder sortOrder)
		{
			if (sortOrder != AttachmentItemsSortOrder.Ascending)
			{
				return "false";
			}
			return "true";
		}

		private static void EnsureColumnMap()
		{
			if (OneDriveProUtilities.columnToOneDriveProColumn == null)
			{
				lock (OneDriveProUtilities.syncRoot)
				{
					if (OneDriveProUtilities.columnToOneDriveProColumn == null)
					{
						OneDriveProUtilities.columnToOneDriveProColumn = new Dictionary<AttachmentItemsSortColumn, string>();
						OneDriveProUtilities.columnToOneDriveProColumn[AttachmentItemsSortColumn.LastModified] = "Modified";
						OneDriveProUtilities.columnToOneDriveProColumn[AttachmentItemsSortColumn.Name] = "FileLeafRef";
						OneDriveProUtilities.columnToOneDriveProColumn[AttachmentItemsSortColumn.Size] = "File_x0020_Size";
					}
				}
			}
		}

		internal const string Post = "POST";

		internal const string Get = "GET";

		internal const string SPRequestIdHeader = "SPRequestGuid";

		private const string Bearer = "Bearer";

		private const string XRequestForceAuthenticationHeader = "X-RequestForceAuthentication";

		private const string True = "true";

		private const string False = "false";

		private const string ReturnClientRequestIdHeader = "return-client-request-id";

		private const string CamlDataQueryStart = "<View>\r\n                                                <Query>\r\n                                                    <OrderBy>\r\n                                                        <FieldRef Name='FSObjType' Ascending='FALSE' />\r\n                                                        <FieldRef Name='{0}' Ascending='{1}' />\r\n                                                    </OrderBy>\r\n                                                </Query>\r\n                                                <ViewFields>\r\n                                                    <FieldRef Name='ID' />\r\n                                                    <FieldRef Name='FileLeafRef' />\r\n                                                    <FieldRef Name='FSObjType' />\r\n                                                    <FieldRef Name='FileRef' />\r\n                                                    <FieldRef Name='File_x0020_Size' />\r\n                                                    <FieldRef Name='Modified' />\r\n                                                    <FieldRef Name='Editor' />\r\n                                                    <FieldRef Name='ItemChildCount' />\r\n                                                    <FieldRef Name='FolderChildCount' />\r\n                                                    <FieldRef Name='ProgId' />\r\n                                                </ViewFields>";

		private const string CamlDataQueryEnd = "</View>";

		private const string RowLimit = "<RowLimit>{2}</RowLimit>";

		private const string CamlDataQuery = "<View>\r\n                                                <Query>\r\n                                                    <OrderBy>\r\n                                                        <FieldRef Name='FSObjType' Ascending='FALSE' />\r\n                                                        <FieldRef Name='{0}' Ascending='{1}' />\r\n                                                    </OrderBy>\r\n                                                </Query>\r\n                                                <ViewFields>\r\n                                                    <FieldRef Name='ID' />\r\n                                                    <FieldRef Name='FileLeafRef' />\r\n                                                    <FieldRef Name='FSObjType' />\r\n                                                    <FieldRef Name='FileRef' />\r\n                                                    <FieldRef Name='File_x0020_Size' />\r\n                                                    <FieldRef Name='Modified' />\r\n                                                    <FieldRef Name='Editor' />\r\n                                                    <FieldRef Name='ItemChildCount' />\r\n                                                    <FieldRef Name='FolderChildCount' />\r\n                                                    <FieldRef Name='ProgId' />\r\n                                                </ViewFields></View>";

		private const string PagedCamlDataQuery = "<View>\r\n                                                <Query>\r\n                                                    <OrderBy>\r\n                                                        <FieldRef Name='FSObjType' Ascending='FALSE' />\r\n                                                        <FieldRef Name='{0}' Ascending='{1}' />\r\n                                                    </OrderBy>\r\n                                                </Query>\r\n                                                <ViewFields>\r\n                                                    <FieldRef Name='ID' />\r\n                                                    <FieldRef Name='FileLeafRef' />\r\n                                                    <FieldRef Name='FSObjType' />\r\n                                                    <FieldRef Name='FileRef' />\r\n                                                    <FieldRef Name='File_x0020_Size' />\r\n                                                    <FieldRef Name='Modified' />\r\n                                                    <FieldRef Name='Editor' />\r\n                                                    <FieldRef Name='ItemChildCount' />\r\n                                                    <FieldRef Name='FolderChildCount' />\r\n                                                    <FieldRef Name='ProgId' />\r\n                                                </ViewFields><RowLimit>{2}</RowLimit></View>";

		private const string PagedCamlPagingQuery = "<View>\r\n                                                    <Query>\r\n                                                        <OrderBy>\r\n                                                            <FieldRef Name='FSObjType' Ascending='FALSE' />\r\n                                                            <FieldRef Name='{0}' Ascending='{1}' />\r\n                                                        </OrderBy>\r\n                                                    </Query>\r\n                                                    <ViewFields>\r\n                                                        <FieldRef Name='ID' />\r\n                                                        <FieldRef Name='FileLeafRef' />\r\n                                                        <FieldRef Name='FSObjType' />\r\n                                                        <FieldRef Name='SortBehavior' />\r\n                                                    </ViewFields>\r\n                                                    <RowLimit>{2}</RowLimit>\r\n                                                </View>";

		private const string RestContentType = "application/json;odata=verbose";

		private static Dictionary<AttachmentItemsSortColumn, string> columnToOneDriveProColumn;

		private static object syncRoot = new object();

		private static string userAgentString;
	}
}
