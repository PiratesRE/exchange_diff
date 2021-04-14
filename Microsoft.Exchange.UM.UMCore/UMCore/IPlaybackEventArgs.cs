using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal interface IPlaybackEventArgs
	{
		TimeSpan PlayTime { get; set; }

		int LastPrompt { get; set; }
	}
}
