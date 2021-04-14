using System;

namespace Microsoft.Exchange.Data.ApplicationLogic.Diagnostics
{
	public class GetConditionMetadataResult
	{
		public ActiveConditionalMetadataResult[] ActiveConditions { get; set; }

		public ConditionalMetadataResult[] CompletedConditions { get; set; }
	}
}
