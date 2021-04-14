using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.ImageAnalysis
{
	public interface ISalientObjectAnalysis
	{
		KeyValuePair<List<RegionRect>, ImageAnalysisResult> GetSalientRectsAsList();

		KeyValuePair<byte[], ImageAnalysisResult> GetSalientRectsAsByteArray();
	}
}
