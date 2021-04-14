using System;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IExImportHierarchyChanges : IExInterface, IDisposeTrackable, IDisposable
	{
		int Config(IStream iStream, int ulFlags);

		int UpdateState(IStream iStream);

		unsafe int ImportFolderChange(int cpvalChanges, SPropValue* ppvalChanges);

		unsafe int ImportFolderDeletion(int ulFlags, _SBinaryArray* lpSrcEntryList);
	}
}
