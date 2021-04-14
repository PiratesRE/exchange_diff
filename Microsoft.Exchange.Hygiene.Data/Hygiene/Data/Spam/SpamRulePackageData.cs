using System;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	public class SpamRulePackageData
	{
		public SpamRuleData[] Rules { get; set; }

		public ProcessorData[] Processors { get; set; }
	}
}
