using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMapiFxProxy : IDisposeTrackable, IDisposable
	{
		void ProcessRequest(FxOpcodes opCode, byte[] request);

		byte[] GetObjectData();
	}
}
