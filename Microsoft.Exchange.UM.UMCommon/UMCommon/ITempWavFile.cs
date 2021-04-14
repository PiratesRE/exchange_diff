using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal interface ITempWavFile : ITempFile, IDisposeTrackable, IDisposable
	{
		string ExtraInfo { get; set; }
	}
}
