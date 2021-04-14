using System;
using System.Data;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal sealed class TransportQueueLogSaveDataSet : ConfigurablePropertyBag
	{
		public TransportQueueLogSaveDataSet()
		{
			this.identity = new ConfigObjectId(CombGuidGenerator.NewGuid().ToString());
		}

		public override ObjectId Identity
		{
			get
			{
				return this.identity;
			}
		}

		public static TransportQueueLogSaveDataSet CreateDataSet(TransportQueueLogBatch batch)
		{
			TransportQueueLogSaveDataSet transportQueueLogSaveDataSet = new TransportQueueLogSaveDataSet();
			transportQueueLogSaveDataSet[TransportQueueLogSaveDataSetSchema.ForestIdProperty] = batch.ForestId;
			transportQueueLogSaveDataSet[TransportQueueLogSaveDataSetSchema.ServerIdProperty] = batch.ServerId;
			transportQueueLogSaveDataSet[TransportQueueLogSaveDataSetSchema.SnapshotDatetimeProperty] = batch.SnapshotDatetime;
			transportQueueLogSaveDataSet[TransportQueueLogSaveDataSetSchema.ServerPropertiesTableProperty] = TransportQueueLogSaveDataSet.GetServerProperties(batch);
			transportQueueLogSaveDataSet[TransportQueueLogSaveDataSetSchema.QueueLogPropertiesTableProperty] = TransportQueueLogSaveDataSet.GetQueueLogProperties(batch);
			return transportQueueLogSaveDataSet;
		}

		public override Type GetSchemaType()
		{
			return typeof(TransportQueueLogSaveDataSetSchema);
		}

		private static DataTable GetServerProperties(TransportQueueLogBatch batch)
		{
			PropertyTable propertyTable = new PropertyTable();
			foreach (PropertyDefinition propertyDefinition in batch.GetPropertyDefinitions(false))
			{
				if (string.Compare(propertyDefinition.Name, TransportQueueLogSaveDataSetSchema.QueueLogPropertiesTableProperty.Name, StringComparison.OrdinalIgnoreCase) != 0)
				{
					propertyTable.AddPropertyValue(propertyDefinition, batch[propertyDefinition]);
				}
			}
			return propertyTable;
		}

		private static DataTable GetQueueLogProperties(TransportQueueLogBatch batch)
		{
			BatchPropertyTable batchPropertyTable = new BatchPropertyTable();
			if (batch.QueueLogs != null)
			{
				foreach (TransportQueueLog transportQueueLog in batch.QueueLogs)
				{
					foreach (PropertyDefinition propertyDefinition in DalHelper.GetPropertyDefinitions(transportQueueLog, true))
					{
						if (!propertyDefinition.Type.Equals(typeof(DataTable)))
						{
							batchPropertyTable.AddPropertyValue(transportQueueLog.QueueId, propertyDefinition, transportQueueLog[propertyDefinition]);
						}
					}
				}
			}
			return batchPropertyTable;
		}

		private ObjectId identity;
	}
}
