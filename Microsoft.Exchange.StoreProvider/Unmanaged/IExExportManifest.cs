using System;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IExExportManifest : IExInterface, IDisposeTrackable, IDisposable
	{
		unsafe int Config(IStream iStream, SyncConfigFlags flags, IExchangeManifestCallback pCallback, SRestriction* lpRestriction, PropTag[] lpIncludeProps);

		int Synchronize(int ulFlags);

		int Checkpoint(IStream iStream, bool clearCnsets, long[] changeMids, long[] changeCns, long[] changeAssociatedCns, long[] deleteMids, long[] readCns);
	}
}
