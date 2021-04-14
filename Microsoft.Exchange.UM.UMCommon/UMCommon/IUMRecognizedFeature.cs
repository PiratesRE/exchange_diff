using System;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal interface IUMRecognizedFeature
	{
		string Name { get; }

		string Value { get; }

		int FirstWordIndex { get; }

		int CountOfWords { get; }
	}
}
