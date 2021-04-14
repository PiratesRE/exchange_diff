using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Optics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IDiagnosticsFrame : IForceReportDisposeTrackable, IDisposeTrackable, IDisposable
	{
	}
}
