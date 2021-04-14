using System;

namespace Microsoft.Exchange.Data
{
	public interface ISendAsSource
	{
		Guid SourceGuid { get; }

		SmtpAddress UserEmailAddress { get; }

		bool IsEnabled { get; }
	}
}
