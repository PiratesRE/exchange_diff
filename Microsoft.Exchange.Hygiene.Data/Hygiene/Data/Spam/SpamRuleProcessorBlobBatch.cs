using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	internal class SpamRuleProcessorBlobBatch : ConfigurableBatch
	{
		public SpamRuleProcessorBlobBatch()
		{
		}

		public SpamRuleProcessorBlobBatch(IEnumerable<SpamRuleProcessorBlob> items)
		{
			BatchPropertyTable batchPropertyTable = new BatchPropertyTable();
			foreach (SpamRuleProcessorBlob spamRuleProcessorBlob in items)
			{
				Guid identity = CombGuidGenerator.NewGuid();
				batchPropertyTable.AddPropertyValue(identity, SpamRuleProcessorBlobSchema.IdProperty, spamRuleProcessorBlob.Id);
				batchPropertyTable.AddPropertyValue(identity, SpamRuleProcessorBlobSchema.ProcessorIdProperty, spamRuleProcessorBlob.ProcessorId);
				batchPropertyTable.AddPropertyValue(identity, SpamRuleProcessorBlobSchema.DataProperty, spamRuleProcessorBlob.Data);
			}
			base.Batch = batchPropertyTable;
		}
	}
}
