using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.AirSync.SchemaConverter.AirSync;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter
{
	internal class AirSyncSchemaState : SchemaState, IAirSyncDataObjectGenerator, IDataObjectGenerator
	{
		public AirSyncSchemaState()
		{
		}

		protected AirSyncSchemaState(AirSyncSchemaState innerSchemaState)
		{
			this.innerSchemaState = innerSchemaState;
		}

		public IDictionary Options
		{
			get
			{
				return this.options;
			}
		}

		public AirSyncDataObject GetAirSyncDataObject(IDictionary options, IAirSyncMissingPropertyStrategy missingPropertyStrategy)
		{
			if (options == null)
			{
				throw new ArgumentNullException("options");
			}
			if (missingPropertyStrategy == null)
			{
				throw new ArgumentNullException("missingPropertyStrategy");
			}
			this.options = options;
			this.missingPropertyStrategy = missingPropertyStrategy;
			List<IProperty> schema = base.GetSchema(0);
			for (int i = 0; i < schema.Count; i++)
			{
				AirSyncProperty airSyncProperty = (AirSyncProperty)schema[i];
				airSyncProperty.Options = this.options;
			}
			return new AirSyncDataObject(schema, this.missingPropertyStrategy, this);
		}

		public AirSyncDataObject GetInnerAirSyncDataObject(IAirSyncMissingPropertyStrategy strategy)
		{
			if (this.innerSchemaState == null)
			{
				return null;
			}
			List<IProperty> schema = this.innerSchemaState.GetSchema(0);
			for (int i = 0; i < schema.Count; i++)
			{
				AirSyncProperty airSyncProperty = (AirSyncProperty)schema[i];
				airSyncProperty.Options = this.options;
			}
			return new AirSyncDataObject(schema, strategy, null);
		}

		private AirSyncSchemaState innerSchemaState;

		private IDictionary options;

		private IAirSyncMissingPropertyStrategy missingPropertyStrategy;

		public enum SchemaEnum
		{
			ClientSide,
			ServerSide,
			Count
		}
	}
}
