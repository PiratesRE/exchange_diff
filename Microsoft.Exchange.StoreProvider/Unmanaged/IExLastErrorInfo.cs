using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IExLastErrorInfo : IExInterface, IDisposeTrackable, IDisposable
	{
		int GetLastError(int hResult, out int lpMapiError);

		int GetExtendedErrorInfo(out DiagnosticContext pExtendedErrorInfo);
	}
}
