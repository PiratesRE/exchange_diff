using System;

namespace Microsoft.Exchange.Clients.Owa.Core.Transcoding
{
	internal enum TranscodeErrorCode
	{
		Succeeded,
		FatalFaultError,
		UnconvertibleError,
		WrongFileTypeError,
		InvalidPageNumberError
	}
}
