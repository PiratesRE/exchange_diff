using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Hygiene.Data.Spam;

namespace Microsoft.Exchange.Hygiene.Data.Kes
{
	internal class SyncWatermarkBatch : ConfigurableBatch
	{
		public SyncWatermarkBatch()
		{
		}

		public SyncWatermarkBatch(IEnumerable<SyncWatermark> items)
		{
			BatchPropertyTable batchPropertyTable = new BatchPropertyTable();
			foreach (SyncWatermark syncWatermark in items)
			{
				Guid identity = CombGuidGenerator.NewGuid();
				batchPropertyTable.AddPropertyValue(identity, SyncWatermark.IdProperty, syncWatermark.Id);
				batchPropertyTable.AddPropertyValue(identity, SyncWatermark.SyncContextProperty, syncWatermark.SyncContext);
				batchPropertyTable.AddPropertyValue(identity, SyncWatermark.DataProperty, syncWatermark.Data);
				batchPropertyTable.AddPropertyValue(identity, SyncWatermark.OwnerProperty, syncWatermark.Owner);
			}
			base.Batch = batchPropertyTable;
		}
	}
}
