using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class Win32InteropException : AvailabilityException
	{
		public Win32InteropException(Exception innerException) : base(ErrorConstants.Win32InteropError, Strings.descWin32InteropError, innerException)
		{
		}
	}
}
