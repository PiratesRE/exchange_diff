using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class ProxyNotAllowedForPersonalRelationship : AvailabilityException
	{
		public ProxyNotAllowedForPersonalRelationship(EmailAddress recipient) : base(ErrorConstants.ProxyForPersonalNotAllowed, Strings.descProxyForPersonalNotAllowed(recipient.ToString()))
		{
		}
	}
}
