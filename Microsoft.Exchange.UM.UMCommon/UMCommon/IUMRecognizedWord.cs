using System;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal interface IUMRecognizedWord
	{
		float Confidence { get; }

		string Text { get; set; }

		UMDisplayAttributes DisplayAttributes { get; set; }

		TimeSpan AudioPosition { get; }

		TimeSpan AudioDuration { get; }
	}
}
