using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Management.DDIService
{
	public interface IEacRbacPrincipal
	{
		ADObjectId ExecutingUserId { get; }

		string Name { get; }

		SmtpAddress ExecutingUserPrimarySmtpAddress { get; }

		ExTimeZone UserTimeZone { get; }

		string DateFormat { get; }

		string TimeFormat { get; }
	}
}
