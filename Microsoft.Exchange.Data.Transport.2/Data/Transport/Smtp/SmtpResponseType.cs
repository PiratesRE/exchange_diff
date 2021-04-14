using System;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public enum SmtpResponseType
	{
		Unknown,
		Success,
		IntermediateSuccess,
		TransientError,
		PermanentError
	}
}
