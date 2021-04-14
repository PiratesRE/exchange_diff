using System;
using System.Net;
using System.Web;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Net.Wopi;

namespace Microsoft.Exchange.HttpProxy
{
	internal class WopiProxyRequestHandler : ProxyRequestHandler
	{
		protected override AnchorMailbox ResolveAnchorMailbox()
		{
			UriBuilder uriBuilder = new UriBuilder(base.ClientRequest.Url);
			uriBuilder.Scheme = "https";
			uriBuilder.Port = 444;
			string userEmailAddress = WopiRequestPathHandler.GetUserEmailAddress(base.ClientRequest);
			if (string.IsNullOrEmpty(userEmailAddress))
			{
				return base.ResolveAnchorMailbox();
			}
			this.targetMailboxId = userEmailAddress;
			AnchorMailbox result;
			if (AnchorMailboxFactory.TryCreateFromMailboxGuid(this, userEmailAddress, out result))
			{
				return result;
			}
			base.Logger.Set(HttpProxyMetadata.RoutingHint, "Url-SMTP");
			return new SmtpAnchorMailbox(userEmailAddress, this);
		}

		protected override Uri GetTargetBackEndServerUrl()
		{
			Uri uri = base.GetTargetBackEndServerUrl();
			if (base.AnchoredRoutingTarget.BackEndServer.Version < Server.E15MinVersion)
			{
				throw new HttpException(500, string.Format("Version < E14 and a WOPI request?  Should not happen....  AnchorMailbox: {0}", base.AnchoredRoutingTarget.AnchorMailbox));
			}
			if (uri.Query.Length == 0)
			{
				throw new HttpException(400, "Unexpected query string format");
			}
			if (!string.IsNullOrEmpty(this.targetMailboxId))
			{
				UriBuilder uriBuilder = new UriBuilder(uri);
				uriBuilder.Path = WopiRequestPathHandler.StripEmailAddress(HttpUtility.UrlDecode(uriBuilder.Path), this.targetMailboxId);
				uriBuilder.Query = uri.Query.Substring(1) + "&UserEmail=" + HttpUtility.UrlEncode(this.targetMailboxId);
				uri = uriBuilder.Uri;
			}
			if (HttpProxySettings.DFPOWAVdirProxyEnabled.Value)
			{
				return UrlUtilities.FixDFPOWAVdirUrlForBackEnd(uri, HttpUtility.ParseQueryString(uri.Query)["vdir"]);
			}
			return uri;
		}

		protected override void AddProtocolSpecificHeadersToServerRequest(WebHeaderCollection headers)
		{
			OwaProxyRequestHandler.AddProxyUriHeader(base.ClientRequest, headers);
			base.AddProtocolSpecificHeadersToServerRequest(headers);
		}

		private string targetMailboxId;
	}
}
