using System;
using System.Net;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzureNewRegistrationIdResponse : AzureResponse
	{
		public AzureNewRegistrationIdResponse(string responseBody, WebHeaderCollection responseHeaders) : base(responseBody, responseHeaders)
		{
			this.ProcessResponse();
		}

		public AzureNewRegistrationIdResponse(Exception exception, Uri targetUri, string responseBody) : base(exception, targetUri, responseBody)
		{
		}

		public AzureNewRegistrationIdResponse(WebException webException, Uri targetUri, string responseBody) : base(webException, targetUri, responseBody)
		{
		}

		public string RegistrationId { get; private set; }

		protected override string InternalToTraceString()
		{
			return string.Format("RegistrationId: {0}", this.RegistrationId);
		}

		private void ProcessResponse()
		{
			if (!base.HasSucceeded)
			{
				return;
			}
			if (base.ResponseHeaders != null)
			{
				string text = base.ResponseHeaders["Content-Location"];
				if (string.IsNullOrWhiteSpace(text))
				{
					text = base.ResponseHeaders["Location"];
				}
				if (string.IsNullOrWhiteSpace(text))
				{
					return;
				}
				int num = text.LastIndexOf("/");
				text = ((text.Length > ++num) ? text.Substring(num, text.Length - num) : null);
				num = text.IndexOf("?");
				if (num > 0)
				{
					text = text.Substring(0, num);
				}
				this.RegistrationId = text;
			}
		}

		public const string ContentLocationHeaderName = "Content-Location";

		public const string ContentAltLocationHeaderName = "Location";
	}
}
