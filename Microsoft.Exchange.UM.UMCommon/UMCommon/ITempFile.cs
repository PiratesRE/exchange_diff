using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal interface ITempFile : IDisposeTrackable, IDisposable
	{
		string FilePath { get; }

		void KeepAlive();
	}
}
