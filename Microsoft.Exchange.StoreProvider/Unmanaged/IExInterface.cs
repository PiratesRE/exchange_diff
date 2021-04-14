using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IExInterface : IDisposeTrackable, IDisposable
	{
		bool IsInvalid { get; }

		int QueryInterface(Guid riid, out IExInterface iObj);
	}
}
