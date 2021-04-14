using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;

namespace Microsoft.Exchange.HttpProxy
{
	internal class OwaExtensibilityProxyRequestHandler : ProxyRequestHandler
	{
		internal static bool IsOwaExtensibilityRequest(HttpRequest request)
		{
			return OwaExtensibilityProxyRequestHandler.ExtPathRegex.IsMatch(request.RawUrl) || OwaExtensibilityProxyRequestHandler.ScriptsPathRegex.IsMatch(request.RawUrl) || OwaExtensibilityProxyRequestHandler.StylesPathRegex.IsMatch(request.RawUrl);
		}

		protected override AnchorMailbox ResolveAnchorMailbox()
		{
			ExTraceGlobals.VerboseTracer.TraceDebug<string, Uri>((long)this.GetHashCode(), "[OwaExtensibilityProxyRequestHandler::ResolveAnchorMailbox]: Method {0}; Url {1};", base.ClientRequest.HttpMethod, base.ClientRequest.Url);
			Match match = OwaExtensibilityProxyRequestHandler.ExtPathRegex.Match(base.ClientRequest.RawUrl);
			if (!match.Success)
			{
				match = OwaExtensibilityProxyRequestHandler.ScriptsPathRegex.Match(base.ClientRequest.RawUrl);
				if (!match.Success)
				{
					match = OwaExtensibilityProxyRequestHandler.StylesPathRegex.Match(base.ClientRequest.RawUrl);
				}
			}
			Guid guid;
			string text;
			if (match.Success && RegexUtilities.TryGetMailboxGuidAddressFromRegexMatch(match, out guid, out text))
			{
				this.routingHint = string.Format("{0}@{1}", guid, text);
				AnchorMailbox result = new MailboxGuidAnchorMailbox(guid, text, this);
				base.Logger.Set(HttpProxyMetadata.RoutingHint, "OwaExtension-MailboxGuidWithDomain");
				return result;
			}
			throw new HttpProxyException(HttpStatusCode.NotFound, HttpProxySubErrorCode.ServerNotFound, string.Format("Unable to find target server for url: {0}", base.ClientRequest.Url));
		}

		protected override UriBuilder GetClientUrlForProxy()
		{
			UriBuilder uriBuilder = new UriBuilder(base.ClientRequest.Url);
			if (!string.IsNullOrEmpty(this.routingHint))
			{
				string text = base.ClientRequest.Url.AbsolutePath;
				text = HttpUtility.UrlDecode(text);
				string text2 = "/" + this.routingHint;
				int num = text.IndexOf(text2);
				if (num != -1)
				{
					string path = text.Substring(0, num) + text.Substring(num + text2.Length);
					uriBuilder.Path = path;
				}
			}
			return uriBuilder;
		}

		protected override Uri GetTargetBackEndServerUrl()
		{
			Uri targetBackEndServerUrl = base.GetTargetBackEndServerUrl();
			return UrlUtilities.FixIntegratedAuthUrlForBackEnd(targetBackEndServerUrl);
		}

		private static readonly Regex ExtPathRegex = new Regex("/owa/((?<hint>[a-fA-F0-9]{8}-([a-fA-F0-9]{4}-){3}[a-fA-F0-9]{12}@[A-Z0-9.-]+\\.[A-Z]{2,4})/)prem/\\d{2}\\.\\d{1,}\\.\\d{1,}\\.\\d{1,}/ext/def/.*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private static readonly Regex ScriptsPathRegex = new Regex("/owa/((?<hint>[a-fA-F0-9]{8}-([a-fA-F0-9]{4}-){3}[a-fA-F0-9]{12}@[A-Z0-9.-]+\\.[A-Z]{2,4})/)prem/\\d{2}\\.\\d{1,}\\.\\d{1,}\\.\\d{1,}/scripts/.*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private static readonly Regex StylesPathRegex = new Regex("/owa/((?<hint>[a-fA-F0-9]{8}-([a-fA-F0-9]{4}-){3}[a-fA-F0-9]{12}@[A-Z0-9.-]+\\.[A-Z]{2,4})/)prem/\\d{2}\\.\\d{1,}\\.\\d{1,}\\.\\d{1,}/resources/styles/.*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private string routingHint;
	}
}
