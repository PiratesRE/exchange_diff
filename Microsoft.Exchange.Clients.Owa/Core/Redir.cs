using System;
using System.Globalization;
using System.Net;
using System.Text;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa.Premium;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.DocumentLibrary;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.TextConverters.Internal;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class Redir : OwaPage
	{
		public string SafeUrl
		{
			get
			{
				return this.safeUrl;
			}
		}

		public bool NewMailCreated
		{
			get
			{
				return this.isNewMailLinkCreated;
			}
		}

		protected bool OpenWebReadyForm
		{
			get
			{
				return this.openWebReadyForm;
			}
		}

		internal static string BuildRedirUrl(UserContext userContext, string unencodedUrl)
		{
			return Redir.BuildRedirUrl("redir.aspx?", userContext, unencodedUrl);
		}

		internal static string BuildRedirUrlForSMime(UserContext userContext, string unencodedUrl)
		{
			return Redir.BuildRedirUrl("redir.aspx?", userContext, unencodedUrl, true);
		}

		internal static string BuildExplicitRedirUrl(OwaContext owaContext, string unencodedUrl)
		{
			return Redir.BuildRedirUrl(OwaUrl.RedirectionPage.GetExplicitUrl(owaContext) + "?", owaContext.UserContext, unencodedUrl);
		}

		private static string BuildRedirUrl(string redirUrl, UserContext userContext, string unencodedUrl)
		{
			return Redir.BuildRedirUrl(redirUrl, userContext, unencodedUrl, false);
		}

		private static bool IsHttpOrHttps(string uriScheme)
		{
			for (int i = 0; i < Redir.hostNameCheckProtocols.Length; i++)
			{
				if (CultureInfo.InvariantCulture.CompareInfo.Compare(uriScheme, Redir.hostNameCheckProtocols[i], CompareOptions.IgnoreCase) == 0)
				{
					return true;
				}
			}
			return false;
		}

		private static bool IsUrlRefererFBALogonPage(Uri urlReferer)
		{
			if (urlReferer == null)
			{
				return false;
			}
			string absolutePath = urlReferer.AbsolutePath;
			if (absolutePath != null && absolutePath.EndsWith(OwaUrl.LogonFBA.ImplicitUrl, StringComparison.OrdinalIgnoreCase))
			{
				string originalString = urlReferer.OriginalString;
				if (originalString != null && originalString.IndexOf("/auth/logon.aspx?replacecurrent=1&url=", StringComparison.OrdinalIgnoreCase) != -1)
				{
					return true;
				}
			}
			return false;
		}

		private static bool CheckHostNameWithHttpHost(string urlString, Uri siteUri, string httpHostfromHeader)
		{
			string host = siteUri.Host;
			if (CultureInfo.InvariantCulture.CompareInfo.Compare(host, httpHostfromHeader, CompareOptions.IgnoreCase) == 0)
			{
				string absolutePath = siteUri.AbsolutePath;
				int num = absolutePath.IndexOf(Redir.appDomainPath, StringComparison.OrdinalIgnoreCase);
				if (num == 0 || (num > 0 && absolutePath.Substring(0, num).Trim(new char[]
				{
					'/'
				}) == string.Empty))
				{
					int num2 = num + Redir.appDomainPath.Length;
					if (absolutePath.Length == num2)
					{
						return false;
					}
					if (absolutePath.IndexOf(Redir.calendarVDirPath, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return true;
					}
					char c = absolutePath[num2];
					if (c == '\\' || c == '/' || c == '?')
					{
						return false;
					}
				}
			}
			return true;
		}

		private static string BuildRedirUrl(string redirUrl, UserContext userContext, string unencodedUrl, bool clientIsSMime)
		{
			StringBuilder stringBuilder = new StringBuilder(125);
			stringBuilder.Append(redirUrl);
			stringBuilder.Append(Redir.BuildSecUrl(unencodedUrl, userContext));
			if (clientIsSMime)
			{
				stringBuilder.Append("&smime=");
			}
			return stringBuilder.ToString();
		}

		private static string BuildSecUrl(string url, UserContext userContext)
		{
			CryptoMessage cryptoMessage = new CryptoMessage(ExDateTime.Now.UniversalTime, url, userContext.Key.Canary.UserContextIdGuid, userContext.Key.Canary.LogonUniqueKey);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("REF");
			stringBuilder.Append("=");
			stringBuilder.Append(cryptoMessage.ToString());
			return stringBuilder.ToString();
		}

		protected override void OnLoad(EventArgs e)
		{
			if (Redir.IsUrlRefererFBALogonPage(base.Request.UrlReferrer))
			{
				Utilities.EndResponse(this.Context, HttpStatusCode.Forbidden);
			}
			string queryStringParameter;
			bool signedUrl = Redir.GetSignedUrl(base.Request, base.UserContext.Key.Canary.UserContextIdGuid, base.UserContext.Key.Canary.LogonUniqueKey, out queryStringParameter);
			if (!signedUrl)
			{
				queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "URL");
			}
			string queryStringParameter2 = Utilities.GetQueryStringParameter(base.Request, "TranslatedURL", false);
			bool flag = !string.IsNullOrEmpty(queryStringParameter2);
			bool flag2 = string.IsNullOrEmpty(Utilities.GetQueryStringParameter(base.Request, "NoDocLnkCls", false));
			if (Redir.IsSafeUrl(queryStringParameter, base.Request))
			{
				ErrorInformation errorInformation = null;
				Uri uri;
				if (null == (uri = Utilities.TryParseUri(queryStringParameter)))
				{
					Utilities.EndResponse(this.Context, HttpStatusCode.Forbidden);
				}
				string scheme = uri.Scheme;
				if (CultureInfo.InvariantCulture.CompareInfo.Compare(scheme, "mailto", CompareOptions.IgnoreCase) == 0)
				{
					StringBuilder stringBuilder = new StringBuilder(512);
					stringBuilder.Append(OwaUrl.ApplicationRoot.GetExplicitUrl(base.OwaContext));
					stringBuilder.Append("?ae=Item&a=New&t=");
					string value = "IPM.Note";
					if (base.UserContext.IsSmsEnabled)
					{
						int length = "mailto:".Length;
						if (queryStringParameter.Length > length)
						{
							string inputString = queryStringParameter.Substring(length);
							Participant participant;
							ProxyAddress proxyAddress;
							if (Participant.TryParse(inputString, out participant) && ImceaAddress.IsImceaAddress(participant.EmailAddress) && SmtpProxyAddress.TryDeencapsulate(participant.EmailAddress, out proxyAddress) && Utilities.IsMobileRoutingType(proxyAddress.PrefixString))
							{
								value = "IPM.Note.Mobile.SMS";
							}
						}
					}
					stringBuilder.Append(value);
					stringBuilder.Append('&');
					stringBuilder.Append("email");
					stringBuilder.Append('=');
					stringBuilder.Append(Utilities.UrlEncode(queryStringParameter));
					this.safeUrl = stringBuilder.ToString();
					this.isNewMailLinkCreated = true;
					return;
				}
				if (flag2)
				{
					this.safeUrl = this.TryNavigateToInternalWssUnc(queryStringParameter, out errorInformation);
				}
				if (this.safeUrl == null)
				{
					if (flag && Redir.IsSafeUrl(queryStringParameter2, base.Request))
					{
						this.safeUrl = queryStringParameter2;
					}
					else
					{
						if (errorInformation != null)
						{
							Utilities.TransferToErrorPage(base.OwaContext, errorInformation);
							return;
						}
						this.safeUrl = queryStringParameter;
					}
				}
			}
			else if (flag && Redir.IsSafeUrl(queryStringParameter2, base.Request))
			{
				this.safeUrl = queryStringParameter2;
			}
			else
			{
				Utilities.EndResponse(this.Context, HttpStatusCode.Forbidden);
			}
			if (!signedUrl)
			{
				throw new OwaInvalidCanary14Exception(null, "Invalid canary in redir.aspx query.");
			}
		}

		private static bool GetSignedUrl(HttpRequest Request, Guid UserContextIdGuid, string LogonUniqueKey, out string signedUrl)
		{
			string queryStringParameter = Utilities.GetQueryStringParameter(Request, "REF", false);
			bool legacy = false;
			if (queryStringParameter == null)
			{
				queryStringParameter = Utilities.GetQueryStringParameter(Request, "SURL", false);
				legacy = true;
			}
			if (queryStringParameter != null)
			{
				byte[] hiddenMessage = CryptoMessage.GetHiddenMessage(UserContextIdGuid, LogonUniqueKey);
				DateTime dateTime;
				byte[] message;
				if (CryptoMessage.ParseMessage(queryStringParameter, hiddenMessage, out dateTime, out message))
				{
					signedUrl = CryptoMessage.DecodeToString(message, legacy);
					return true;
				}
				ExTraceGlobals.CoreTracer.TraceDebug<string, Guid, string>(0L, "Invalid RedirSecUrl. HashAndMessage:'{0}', UserContextIdGuid:'{1}', UserLogonId:{2}", queryStringParameter, UserContextIdGuid, LogonUniqueKey);
			}
			signedUrl = null;
			return false;
		}

		private string TryNavigateToInternalWssUnc(string uriParam, out ErrorInformation errorInformation)
		{
			errorInformation = null;
			if (base.UserContext.IsBasicExperience)
			{
				return null;
			}
			Uri uri = Utilities.TryParseUri(uriParam);
			if (uri == null || string.IsNullOrEmpty(uri.Scheme) || string.IsNullOrEmpty(uri.Host))
			{
				return null;
			}
			if (!DocumentLibraryUtilities.IsTrustedProtocol(uri.Scheme))
			{
				return null;
			}
			if (!DocumentLibraryUtilities.IsInternalUri(uri.Host, base.UserContext))
			{
				return null;
			}
			if (DocumentLibraryUtilities.IsBlockedHostName(uri.Host, base.UserContext))
			{
				return null;
			}
			if (!DocumentLibraryUtilities.IsDocumentsAccessEnabled(base.UserContext))
			{
				return null;
			}
			bool flag = DocumentLibraryUtilities.IsNavigationToWSSAllowed(base.UserContext);
			bool flag2 = DocumentLibraryUtilities.IsNavigationToUNCAllowed(base.UserContext);
			bool flag3 = Redir.IsHttpOrHttps(uri.Scheme);
			bool flag4 = string.Equals(uri.Scheme, Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase);
			if ((flag3 && !flag) || (flag4 && !flag2))
			{
				return null;
			}
			ClassifyResult documentLibraryObjectId = DocumentLibraryUtilities.GetDocumentLibraryObjectId(uri, base.UserContext);
			if (documentLibraryObjectId == null || documentLibraryObjectId.Error != ClassificationError.None)
			{
				return null;
			}
			DocumentLibraryObjectId objectId = documentLibraryObjectId.ObjectId;
			if (objectId == null)
			{
				return null;
			}
			if (objectId.UriFlags == UriFlags.Other)
			{
				return null;
			}
			UriFlags uriFlags = objectId.UriFlags;
			bool flag5 = (uriFlags & UriFlags.SharepointDocument) == UriFlags.SharepointDocument;
			bool flag6 = (uriFlags & UriFlags.UncDocument) == UriFlags.UncDocument;
			if ((uriFlags & UriFlags.DocumentLibrary) == UriFlags.DocumentLibrary || (uriFlags & UriFlags.Folder) == UriFlags.Folder || uriFlags == UriFlags.Sharepoint || uriFlags == UriFlags.Unc)
			{
				return string.Concat(new string[]
				{
					OwaUrl.ApplicationRoot.GetExplicitUrl(base.OwaContext),
					"?ae=Folder&t=IPF.DocumentLibrary&id=",
					Utilities.UrlEncode(objectId.ToBase64String()),
					"&URL=",
					Utilities.UrlEncode(uriParam)
				});
			}
			if (flag5)
			{
				if (!base.UserContext.IsBasicExperience && DocumentLibraryUtilities.IsWebReadyDocument(objectId, base.UserContext))
				{
					this.openWebReadyForm = true;
					return "WebReadyView.aspx?t=wss&id=" + Utilities.UrlEncode(objectId.ToBase64String()) + "&URL=" + Utilities.UrlEncode(uriParam);
				}
				return string.Concat(new string[]
				{
					"ev.owa?ns=SharepointDocument&ev=GetDoc&id=",
					Utilities.UrlEncode(objectId.ToBase64String()),
					"&URL=",
					Utilities.UrlEncode(uriParam),
					Utilities.GetCanaryRequestParameter()
				});
			}
			else
			{
				if (!flag6)
				{
					return null;
				}
				if (!base.UserContext.IsBasicExperience && DocumentLibraryUtilities.IsWebReadyDocument(objectId, base.UserContext))
				{
					this.openWebReadyForm = true;
					return "WebReadyView.aspx?t=unc&id=" + Utilities.UrlEncode(objectId.ToBase64String()) + "&URL=" + Utilities.UrlEncode(uriParam);
				}
				return string.Concat(new string[]
				{
					"ev.owa?ns=UncDocument&ev=GetDoc&id=",
					Utilities.UrlEncode(objectId.ToBase64String()),
					"&URL=",
					Utilities.UrlEncode(uriParam),
					Utilities.GetCanaryRequestParameter()
				});
			}
		}

		internal static bool IsSafeUrl(string urlString, HttpRequest httpRequest)
		{
			if (string.IsNullOrEmpty(urlString))
			{
				return false;
			}
			Uri uri;
			if (null == (uri = Utilities.TryParseUri(urlString)))
			{
				return false;
			}
			string scheme = uri.Scheme;
			if (string.IsNullOrEmpty(scheme))
			{
				return false;
			}
			if (!Uri.CheckSchemeName(scheme) || !TextConvertersInternalHelpers.IsUrlSchemaSafe(scheme))
			{
				return false;
			}
			if (Redir.IsHttpOrHttps(scheme))
			{
				string text = httpRequest.ServerVariables["HTTP_HOST"];
				return !string.IsNullOrEmpty(text) && Redir.CheckHostNameWithHttpHost(urlString, uri, text);
			}
			return true;
		}

		protected void RenderGlobalJavascriptVariables()
		{
			Utilities.RenderGlobalJavascriptVariables(base.Response.Output, base.UserContext);
		}

		internal const string RedirUrlParamInUrl = "URL=";

		internal const string RedirSignedUrlParam = "SURL";

		internal const string RedirEncodedRefParam = "REF";

		internal const string RedirPrefix = "redir.aspx?";

		private static readonly string[] hostNameCheckProtocols = new string[]
		{
			"http",
			"https",
			"mhtml"
		};

		private static string appDomainPath = HttpRuntime.AppDomainAppVirtualPath.ToLowerInvariant();

		private static string calendarVDirPath = Redir.appDomainPath + "/calendar/";

		private string safeUrl;

		private bool isNewMailLinkCreated;

		private bool openWebReadyForm;
	}
}
