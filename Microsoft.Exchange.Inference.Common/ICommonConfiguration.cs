using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Inference.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ICommonConfiguration
	{
		bool OutlookActivityProcessingEnabled { get; }

		bool OutlookActivityProcessingEnabledInEba { get; }

		TimeSpan OutlookActivityProcessingCutoffWindow { get; }

		bool PersistedLabelsEnabled { get; }
	}
}
