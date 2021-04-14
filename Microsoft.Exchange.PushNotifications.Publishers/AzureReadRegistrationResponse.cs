using System;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzureReadRegistrationResponse : AzureResponse
	{
		public AzureReadRegistrationResponse(string responseBody, WebHeaderCollection responseHeaders = null) : base(responseBody, responseHeaders)
		{
			this.ProcessResponse();
		}

		public AzureReadRegistrationResponse(Exception exception, Uri targetUri, string responseBody) : base(exception, targetUri, responseBody)
		{
		}

		public AzureReadRegistrationResponse(WebException webException, Uri targetUri, string responseBody) : base(webException, targetUri, responseBody)
		{
		}

		public string RegistrationId { get; private set; }

		public ExDateTime ExpirationTimeUtc { get; private set; }

		public bool HasRegistration { get; private set; }

		protected override string InternalToTraceString()
		{
			return string.Format("HasRegistration:{0}; ExpirationTimeUtc:{1}; RegistrationId:{2}", this.HasRegistration, this.ExpirationTimeUtc, this.RegistrationId);
		}

		private void ProcessResponse()
		{
			if (!base.HasSucceeded)
			{
				return;
			}
			this.HasRegistration = (base.OriginalBody.IndexOf("<content type=\"application/xml\">", StringComparison.InvariantCultureIgnoreCase) != -1);
			if (this.HasRegistration)
			{
				Match match = AzureReadRegistrationResponse.RegistrationIdRegex.Match(base.OriginalBody);
				if (match.Success)
				{
					this.RegistrationId = match.Value;
				}
				match = AzureReadRegistrationResponse.ExpirationTimeRegex.Match(base.OriginalBody);
				if (match.Success)
				{
					this.ExpirationTimeUtc = ExDateTime.Parse(match.Value);
				}
			}
		}

		private const string RegistrationXMLContentTag = "<content type=\"application/xml\">";

		private static readonly Regex RegistrationIdRegex = new Regex("(?<=RegistrationId>)((?i)[A-F0-9\\-])*(?=\\<\\/RegistrationId)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static readonly Regex ExpirationTimeRegex = new Regex("(?<=ExpirationTime>)[\\S]*?(?=\\<\\/ExpirationTime)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
	}
}
