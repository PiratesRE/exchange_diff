using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	internal class ProcessorBatch : ConfigurableBatch
	{
		public ProcessorBatch()
		{
		}

		public ProcessorBatch(IEnumerable<Processor> items)
		{
			BatchPropertyTable batchPropertyTable = new BatchPropertyTable();
			foreach (Processor processor in items)
			{
				Guid identity = CombGuidGenerator.NewGuid();
				batchPropertyTable.AddPropertyValue(identity, Processor.ProcessorIDProperty, processor.ProcessorID);
				if (processor.PropertyName != null)
				{
					batchPropertyTable.AddPropertyValue(identity, ConfigurablePropertyTable.NameProperty, processor.PropertyName);
					if (processor.IntValue != null)
					{
						batchPropertyTable.AddPropertyValue(identity, ConfigurablePropertyTable.IntValueProperty, processor.IntValue);
					}
					if (processor.LongValue != null)
					{
						batchPropertyTable.AddPropertyValue(identity, ConfigurablePropertyTable.LongValueProperty, processor.LongValue);
					}
					if (processor.StringValue != null)
					{
						batchPropertyTable.AddPropertyValue(identity, ConfigurablePropertyTable.StringValueProperty, processor.StringValue);
					}
					if (processor.BlobValue != null)
					{
						batchPropertyTable.AddPropertyValue(identity, ConfigurablePropertyTable.BlobValueProperty, processor.BlobValue);
					}
					if (processor.DatetimeValue != null)
					{
						batchPropertyTable.AddPropertyValue(identity, ConfigurablePropertyTable.DatetimeValueProperty, processor.DatetimeValue);
					}
					if (processor.DecimalValue != null)
					{
						batchPropertyTable.AddPropertyValue(identity, ConfigurablePropertyTable.DecimalValueProperty, processor.DecimalValue);
					}
					if (processor.BoolValue != null)
					{
						batchPropertyTable.AddPropertyValue(identity, ConfigurablePropertyTable.BoolValueProperty, processor.BoolValue);
					}
					if (processor.GuidValue != null)
					{
						batchPropertyTable.AddPropertyValue(identity, ConfigurablePropertyTable.GuidValueProperty, processor.GuidValue);
					}
				}
			}
			base.Batch = batchPropertyTable;
		}
	}
}
