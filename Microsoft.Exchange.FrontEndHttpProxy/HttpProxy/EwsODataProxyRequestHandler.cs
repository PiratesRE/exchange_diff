using System;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.HttpProxy.Common;

namespace Microsoft.Exchange.HttpProxy
{
	internal sealed class EwsODataProxyRequestHandler : EwsProxyRequestHandler
	{
		internal static bool IsODataRequest(HttpRequest request)
		{
			return ProtocolHelper.IsEwsODataRequest(request.Path);
		}

		protected override AnchorMailbox ResolveAnchorMailbox()
		{
			string text = this.TryResolveTargetMailbox();
			if (!string.IsNullOrEmpty(text))
			{
				base.Logger.Set(HttpProxyMetadata.RoutingHint, "TargetMailbox-SMTP");
				return new SmtpAnchorMailbox(text, this);
			}
			return base.ResolveAnchorMailbox();
		}

		private string TryResolveTargetMailbox()
		{
			Match match = Constants.UsersEntityRegex.Match(base.ClientRequest.Url.PathAndQuery);
			if (match.Success)
			{
				string text = match.Result("${address}");
				if (SmtpAddress.IsValidSmtpAddress(text))
				{
					return text;
				}
			}
			return null;
		}
	}
}
