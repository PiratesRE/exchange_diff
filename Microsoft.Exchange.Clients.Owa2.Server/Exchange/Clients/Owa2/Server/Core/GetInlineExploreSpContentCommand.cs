using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class GetInlineExploreSpContentCommand : ServiceCommand<InlineExploreSpResultListType>
	{
		internal string TargetUrl { get; private set; }

		internal GetInlineExploreSpContentCommand(CallContext callContext, string query, string targetUrl) : base(callContext)
		{
			if (callContext == null)
			{
				throw new OwaInvalidRequestException("callContext parameter was null");
			}
			if (string.IsNullOrEmpty(query))
			{
				throw new OwaInvalidRequestException("query parameter was null");
			}
			if (string.IsNullOrEmpty(targetUrl))
			{
				throw new OwaInvalidRequestException("targetUrl parameter was null");
			}
			this.currentCallContext = callContext;
			this.TargetUrl = this.ConstructTargetUrl(targetUrl, query);
		}

		protected override InlineExploreSpResultListType InternalExecute()
		{
			return this.GetQueryResults();
		}

		private InlineExploreSpResultListType GetQueryResults()
		{
			OAuthCredentials oauthCredentials = this.GetOAuthCredentials();
			string resultText = this.QuerySharePoint(oauthCredentials);
			return this.ParseQueryResults(resultText);
		}

		internal InlineExploreSpResultListType ParseQueryResults(string resultText)
		{
			InlineExploreSpResultListType inlineExploreSpResultListType = new InlineExploreSpResultListType();
			List<InlineExploreSpResultItemType> list = new List<InlineExploreSpResultItemType>();
			inlineExploreSpResultListType.ResultCount = 0;
			inlineExploreSpResultListType.Status = "Success";
			if (string.IsNullOrEmpty(resultText))
			{
				this.LogError("[GetInlineExploreSpContent.ParseQueryResults(string resultText)]:XML Parsing Error", "Input is empty");
				inlineExploreSpResultListType.Status = "Error: SharePoint query returned empty";
			}
			else
			{
				try
				{
					XElement xelement = XElement.Parse(resultText);
					string value = xelement.Descendants(this.d + "RelevantResults").Elements(this.d + "TotalRows").First<XElement>().Value;
					inlineExploreSpResultListType.ResultCount = int.Parse(value);
					List<XElement> list2 = xelement.Descendants(this.d + "RelevantResults").Elements(this.d + "Table").Elements(this.d + "Rows").Elements(this.d + "element").ToList<XElement>();
					foreach (XElement xelement2 in list2)
					{
						IEnumerable<XElement> source = xelement2.Element(this.d + "Cells").Descendants(this.d + "element");
						InlineExploreSpResultItemType inlineExploreSpResultItemType = new InlineExploreSpResultItemType();
						inlineExploreSpResultItemType.Title = source.First((XElement a) => a.Element(this.d + "Key").Value == "Title").Element(this.d + "Value").Value;
						inlineExploreSpResultItemType.Url = source.First((XElement a) => a.Element(this.d + "Key").Value == "Path").Element(this.d + "Value").Value;
						inlineExploreSpResultItemType.FileType = source.First((XElement a) => a.Element(this.d + "Key").Value == "FileType").Element(this.d + "Value").Value;
						inlineExploreSpResultItemType.LastModifiedTime = source.First((XElement a) => a.Element(this.d + "Key").Value == "LastModifiedTime").Element(this.d + "Value").Value;
						inlineExploreSpResultItemType.Summary = source.First((XElement a) => a.Element(this.d + "Key").Value == "HitHighlightedSummary").Element(this.d + "Value").Value;
						InlineExploreSpResultItemType inlineExploreSpResultItemType2 = inlineExploreSpResultItemType;
						if (inlineExploreSpResultItemType2.Title != null && inlineExploreSpResultItemType2.Url != null)
						{
							list.Add(inlineExploreSpResultItemType2);
						}
						else
						{
							this.LogError("[GetInlineExploreSpContent.ParseQueryResults(string resultText)]:XML Parsing Error", "Linq returned empty result fields");
							inlineExploreSpResultListType.Status = "Error: Empty result fields";
						}
					}
				}
				catch (Exception ex)
				{
					this.LogError("[GetInlineExploreSpContent.ParseQueryResults(string resultText)]:XML Parsing Exception", ex.ToString());
					inlineExploreSpResultListType.Status = "Error: XML parsing";
				}
			}
			inlineExploreSpResultListType.ResultItems = list.ToArray();
			return inlineExploreSpResultListType;
		}

		private OAuthCredentials GetOAuthCredentials()
		{
			ADUser accessingADUser = this.currentCallContext.AccessingADUser;
			if (accessingADUser == null)
			{
				this.LogError("[GetInlineExploreSpContent.GetOAuthCredentials()]:OAuth error", "AccessingADUser is null");
				return null;
			}
			return OAuthCredentials.GetOAuthCredentialsForAppActAsToken(accessingADUser.OrganizationId, accessingADUser, null);
		}

		private string QuerySharePoint(OAuthCredentials oAuthCredential)
		{
			string text = string.Empty;
			if (oAuthCredential == null)
			{
				this.LogError("[GetInlineExploreSpContent.QuerySharePoint(OAuthCredentials oAuthCredential)]", "Credentials missing");
				return text;
			}
			if (string.IsNullOrEmpty(this.TargetUrl))
			{
				this.LogError("[GetInlineExploreSpContent.QuerySharePoint(OAuthCredentials oAuthCredential)]", "TargetUrl missing");
				return text;
			}
			Guid value = Guid.NewGuid();
			oAuthCredential.ClientRequestId = new Guid?(value);
			oAuthCredential.Tracer = new GetInlineExploreSpContentCommand.GetInlineExploreSpContentOauthOutboundTracer();
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(this.TargetUrl);
			httpWebRequest.Method = "GET";
			httpWebRequest.ContentType = "text/xml";
			httpWebRequest.UserAgent = "Exchange/15.00.0000.000/InlineExplore";
			httpWebRequest.Headers.Add("client-request-id", value.ToString());
			httpWebRequest.Headers.Add("return-client-request-id", "true");
			httpWebRequest.Credentials = oAuthCredential;
			httpWebRequest.PreAuthenticate = true;
			httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Bearer");
			httpWebRequest.Timeout = 3000;
			try
			{
				using (HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse)
				{
					if (httpWebResponse != null && httpWebResponse.StatusCode != HttpStatusCode.OK)
					{
						throw new Exception(string.Format("Http status code is {0}", httpWebResponse.StatusCode));
					}
					using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
					{
						text = streamReader.ReadToEnd();
					}
				}
			}
			catch (OAuthTokenRequestFailedException ex)
			{
				this.LogError("[GetInlineExploreSpContent.QuerySharePoint(OAuthCredentials oAuthCredential)]:OAuthException", oAuthCredential.Tracer.ToString() + ex.ToString(), value.ToString());
			}
			catch (WebException ex2)
			{
				this.LogError("[GetInlineExploreSpContent.QuerySharePoint(OAuthCredentials oAuthCredential)]:WebException", ex2.ToString(), value.ToString());
			}
			catch (Exception ex3)
			{
				this.LogError("[GetInlineExploreSpContent.QuerySharePoint(OAuthCredentials oAuthCredential)]:Exception", ex3.ToString(), value.ToString());
			}
			if (string.IsNullOrEmpty(text))
			{
				this.LogError("[GetInlineExploreSpContent.QuerySharePoint(OAuthCredentials oAuthCredential)]:No content in response", "Expected response from SharePoint", value.ToString());
			}
			return text;
		}

		private string ConstructTargetUrl(string url, string query)
		{
			string result = string.Empty;
			if (url == null)
			{
				this.LogError("[ConstructTargetUrl(string url, string query)]", "Empty url");
				return result;
			}
			query = query.Replace("'", "''");
			try
			{
				Uri uri = new Uri(url);
				result = string.Format("{0}://{1}/_api/search/query?querytext='{2}'", uri.Scheme, uri.Authority, query);
			}
			catch (UriFormatException ex)
			{
				this.LogError("[ConstructTargetUrl(string url, string query)]:UriFormatException", ex.ToString());
				return string.Empty;
			}
			return result;
		}

		private void LogError(string errorType, string errorDetails)
		{
			this.LogError(errorType, errorDetails, null);
		}

		private void LogError(string errorType, string errorDetails, string clientRequestId)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Diagnostics for GetInlineExploreSpContentCommand:");
			if (clientRequestId != null)
			{
				stringBuilder.AppendLine(string.Format("Client RequestId: {0}", clientRequestId));
			}
			stringBuilder.AppendLine(errorType);
			stringBuilder.AppendLine(errorDetails);
			ExTraceGlobals.InlineExploreTracer.TraceError((long)this.GetHashCode(), stringBuilder.ToString());
		}

		private const string componentDiagnosticIdentifier = "Diagnostics for GetInlineExploreSpContentCommand:";

		private readonly XNamespace d = "http://schemas.microsoft.com/ado/2007/08/dataservices";

		private readonly CallContext currentCallContext;

		private sealed class GetInlineExploreSpContentOauthOutboundTracer : IOutboundTracer
		{
			private void Log(string prefix, int hashCode, string formatString, params object[] args)
			{
				this.result.Append(prefix);
				this.result.AppendLine(string.Format(formatString, args));
			}

			public void LogInformation(int hashCode, string formatString, params object[] args)
			{
				this.Log("Information: ", hashCode, formatString, args);
			}

			public void LogWarning(int hashCode, string formatString, params object[] args)
			{
				this.Log("Warning: ", hashCode, formatString, args);
			}

			public void LogError(int hashCode, string formatString, params object[] args)
			{
				this.Log("Error: ", hashCode, formatString, args);
			}

			public void LogToken(int hashCode, string tokenString)
			{
				this.result.Append("Token: ");
				this.result.AppendLine(tokenString);
			}

			public override string ToString()
			{
				return this.result.ToString();
			}

			private readonly StringBuilder result = new StringBuilder();
		}
	}
}
