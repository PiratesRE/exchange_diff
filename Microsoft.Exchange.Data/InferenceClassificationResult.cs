using System;

namespace Microsoft.Exchange.Data
{
	[Flags]
	internal enum InferenceClassificationResult
	{
		None = 0,
		IsClutterFinal = 1,
		IsClutterModel = 2,
		IsOverridden = 28,
		ConversationActionOverride = 4,
		NeverClutterOverride = 8,
		AlwaysClutterOverride = 16,
		StopProcessingRulesOverride = 32
	}
}
