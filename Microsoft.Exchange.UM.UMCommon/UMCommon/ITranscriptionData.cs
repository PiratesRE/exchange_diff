using System;
using System.Globalization;
using System.Xml;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal interface ITranscriptionData
	{
		RecoResultType RecognitionResult { get; }

		RecoErrorType RecognitionError { get; }

		string ErrorInformation { get; }

		float Confidence { get; }

		CultureInfo Language { get; }

		ConfidenceBandType ConfidenceBand { get; }

		XmlDocument TranscriptionXml { get; }
	}
}
