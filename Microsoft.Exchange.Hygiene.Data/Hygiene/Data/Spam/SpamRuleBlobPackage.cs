using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	internal class SpamRuleBlobPackage
	{
		public List<SpamRuleBlob> SpamRuleBlobs { get; set; }

		public List<SpamRuleProcessorBlob> SpamRuleProcessorBlobs { get; set; }
	}
}
