using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal class EasCapability : MobileServiceCapability
	{
		internal EasCapability(PartType supportedPartType, int segmentPerPart, IList<CodingSupportability> codingSupportabilities, FeatureSupportability featureSupportabilities) : base(supportedPartType, segmentPerPart, codingSupportabilities, featureSupportabilities)
		{
		}
	}
}
