using System;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IExExportChanges : IExInterface, IDisposeTrackable, IDisposable
	{
		unsafe int Config(IStream iStream, int ulFlags, IntPtr iCollector, SRestriction* lpRestriction, PropTag[] lpIncludeProps, PropTag[] lpExcludeProps, int ulBufferSize);

		int Synchronize(out int lpulSteps, out int lpulProgress);

		int UpdateState(IStream iStream);
	}
}
