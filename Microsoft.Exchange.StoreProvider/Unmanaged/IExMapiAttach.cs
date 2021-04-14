using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IExMapiAttach : IExMapiProp, IExInterface, IDisposeTrackable, IDisposable
	{
	}
}
