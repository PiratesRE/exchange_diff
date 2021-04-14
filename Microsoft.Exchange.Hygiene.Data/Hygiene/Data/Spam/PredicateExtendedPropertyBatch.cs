using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	internal class PredicateExtendedPropertyBatch : ConfigurableBatch
	{
		public PredicateExtendedPropertyBatch()
		{
		}

		public PredicateExtendedPropertyBatch(IEnumerable<PredicateExtendedProperty> items)
		{
			BatchPropertyTable batchPropertyTable = new BatchPropertyTable();
			foreach (PredicateExtendedProperty predicateExtendedProperty in items)
			{
				Guid identity = CombGuidGenerator.NewGuid();
				batchPropertyTable.AddPropertyValue(identity, PredicateExtendedProperty.IDProperty, predicateExtendedProperty.ID);
				batchPropertyTable.AddPropertyValue(identity, PredicateExtendedProperty.PredicateIDProperty, predicateExtendedProperty.PredicateID);
				if (predicateExtendedProperty.PropertyName != null)
				{
					batchPropertyTable.AddPropertyValue(identity, ConfigurablePropertyTable.NameProperty, predicateExtendedProperty.PropertyName);
					if (predicateExtendedProperty.IntValue != null)
					{
						batchPropertyTable.AddPropertyValue(identity, ConfigurablePropertyTable.IntValueProperty, predicateExtendedProperty.IntValue);
					}
					if (predicateExtendedProperty.LongValue != null)
					{
						batchPropertyTable.AddPropertyValue(identity, ConfigurablePropertyTable.LongValueProperty, predicateExtendedProperty.LongValue);
					}
					if (predicateExtendedProperty.StringValue != null)
					{
						batchPropertyTable.AddPropertyValue(identity, ConfigurablePropertyTable.StringValueProperty, predicateExtendedProperty.StringValue);
					}
					if (predicateExtendedProperty.BlobValue != null)
					{
						batchPropertyTable.AddPropertyValue(identity, ConfigurablePropertyTable.BlobValueProperty, predicateExtendedProperty.BlobValue);
					}
					if (predicateExtendedProperty.DatetimeValue != null)
					{
						batchPropertyTable.AddPropertyValue(identity, ConfigurablePropertyTable.DatetimeValueProperty, predicateExtendedProperty.DatetimeValue);
					}
					if (predicateExtendedProperty.DecimalValue != null)
					{
						batchPropertyTable.AddPropertyValue(identity, ConfigurablePropertyTable.DecimalValueProperty, predicateExtendedProperty.DecimalValue);
					}
					if (predicateExtendedProperty.BoolValue != null)
					{
						batchPropertyTable.AddPropertyValue(identity, ConfigurablePropertyTable.BoolValueProperty, predicateExtendedProperty.BoolValue);
					}
					if (predicateExtendedProperty.GuidValue != null)
					{
						batchPropertyTable.AddPropertyValue(identity, ConfigurablePropertyTable.GuidValueProperty, predicateExtendedProperty.GuidValue);
					}
				}
			}
			base.Batch = batchPropertyTable;
		}
	}
}
