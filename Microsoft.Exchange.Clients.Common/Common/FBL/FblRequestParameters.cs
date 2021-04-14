using System;
using System.Collections.Specialized;
using System.Web;

namespace Microsoft.Exchange.Clients.Common.FBL
{
	internal class FblRequestParameters
	{
		public FblRequestParameters(NameValueCollection queryParams)
		{
			if (!string.IsNullOrEmpty(queryParams["pd"]) && !ulong.TryParse(HttpUtility.UrlDecode(queryParams["pd"]), out this.Puid))
			{
				this.Puid = 0UL;
			}
			if (!string.IsNullOrEmpty(queryParams["aid"]))
			{
				this.PrimaryEmail = HttpUtility.UrlDecode(queryParams["aid"]);
			}
			this.CustomerGuid = new Guid(Convert.FromBase64String(AuthkeyAuthenticationRequest.UrlDecodeBase64String(queryParams["cid"])));
			this.MessageClass = HttpUtility.UrlDecode(queryParams["mc"]);
			this.MailGuid = new Guid(Convert.FromBase64String(AuthkeyAuthenticationRequest.UrlDecodeBase64String(queryParams["mg"])));
			this.OptIn = "1".Equals(queryParams["opt"]);
		}

		public FblRequestParameters(ulong puid, string primaryEmail, Guid customerGuid, bool optIn)
		{
			this.Puid = puid;
			this.PrimaryEmail = primaryEmail;
			this.CustomerGuid = customerGuid;
			this.OptIn = optIn;
			this.MailGuid = Guid.Empty;
			this.MessageClass = null;
		}

		public FblRequestParameters(ulong puid, string primaryEmail, Guid customerGuid, Guid mailGuid, string messageClass)
		{
			this.Puid = puid;
			this.PrimaryEmail = primaryEmail;
			this.CustomerGuid = customerGuid;
			this.MessageClass = messageClass;
			this.MailGuid = mailGuid;
			this.OptIn = false;
		}

		public bool IsClassifyRequest()
		{
			return !string.IsNullOrEmpty(this.MessageClass);
		}

		public string ToQueryStringFragment()
		{
			NameValueCollection nameValueCollection = new NameValueCollection();
			if (this.Puid != 0UL)
			{
				nameValueCollection.Add("pd", this.Puid.ToString());
			}
			if (!string.IsNullOrEmpty(this.PrimaryEmail))
			{
				nameValueCollection.Add("aid", this.PrimaryEmail);
			}
			nameValueCollection.Add("cid", AuthkeyAuthenticationRequest.UrlEncodeBase64String(Convert.ToBase64String(this.CustomerGuid.ToByteArray())));
			nameValueCollection.Add("mc", this.MessageClass);
			nameValueCollection.Add("mg", AuthkeyAuthenticationRequest.UrlEncodeBase64String(Convert.ToBase64String(this.MailGuid.ToByteArray())));
			nameValueCollection.Add("opt", this.OptIn ? "1" : "0");
			return AuthkeyAuthenticationRequest.ConstructQueryString(nameValueCollection);
		}

		public readonly string PrimaryEmail;

		public readonly ulong Puid;

		public readonly Guid CustomerGuid;

		public readonly string MessageClass;

		public readonly Guid MailGuid;

		public readonly bool OptIn;
	}
}
