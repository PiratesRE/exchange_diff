using System;
using Microsoft.Exchange.Autodiscover;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AutoDiscoverV2
{
	public class AutoDiscoverV2Request
	{
		public SmtpAddress EmailAddress { get; set; }

		public string Protocol { get; set; }

		public string HostNameHint { get; set; }

		public uint RedirectCount { get; set; }

		internal void ValidateRequest(string emailAddress, string protocol, uint redirectCount, RequestDetailsLogger logger)
		{
			if (string.IsNullOrWhiteSpace(protocol))
			{
				throw AutoDiscoverResponseException.BadRequest("MandatoryParameterMissing", "A valid value must be provided for the query parameter 'Protocol'", null);
			}
			if (string.IsNullOrWhiteSpace(emailAddress))
			{
				throw AutoDiscoverResponseException.BadRequest("MandatoryParameterMissing", "A valid smtp address must be provided", null);
			}
			logger.Set(ServiceCommonMetadata.AuthenticatedUser, emailAddress);
			logger.AppendGenericInfo("RequestedProtocol", protocol);
			if (!SmtpAddress.IsValidSmtpAddress(emailAddress))
			{
				throw AutoDiscoverResponseException.BadRequest("InvalidUserId", string.Format("The given SMTP address is invalid '{0}'", emailAddress), null);
			}
			if (redirectCount >= 10U)
			{
				logger.AppendGenericError("RedirectCountExceeded", "Redirect count exceeded for the given user");
				throw AutoDiscoverResponseException.NotFound();
			}
		}
	}
}
