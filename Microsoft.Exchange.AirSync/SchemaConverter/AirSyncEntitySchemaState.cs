using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.AirSync.SchemaConverter.Entity;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.AirSync.SchemaConverter
{
	internal class AirSyncEntitySchemaState : AirSyncSchemaState, IDataObjectGenerator, IClassFilter
	{
		public AirSyncEntitySchemaState(QueryFilter supportedClassFilter)
		{
			this.supportedClassFilter = supportedClassFilter;
		}

		public QueryFilter SupportedClassFilter
		{
			get
			{
				return this.supportedClassFilter;
			}
		}

		public EntityDataObject GetEntityDataObject()
		{
			return new EntityDataObject(base.GetSchema(1), this);
		}

		private static readonly QueryFilter falseFilterInstance = new FalseFilter();

		private readonly QueryFilter supportedClassFilter;
	}
}
