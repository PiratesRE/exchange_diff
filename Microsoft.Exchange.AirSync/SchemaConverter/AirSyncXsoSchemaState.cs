using System;
using System.Collections.Generic;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.AirSync.SchemaConverter.XSO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter
{
	internal class AirSyncXsoSchemaState : AirSyncSchemaState, IXsoDataObjectGenerator, IDataObjectGenerator, IClassFilter
	{
		public AirSyncXsoSchemaState(QueryFilter supportedClassFilter)
		{
			this.supportedClassFilter = supportedClassFilter;
		}

		protected AirSyncXsoSchemaState(QueryFilter supportedClassFilter, AirSyncXsoSchemaState innerSchemaState) : base(innerSchemaState)
		{
			this.innerSchemaState = innerSchemaState;
			this.supportedClassFilter = supportedClassFilter;
		}

		public QueryFilter SupportedClassFilter
		{
			get
			{
				return this.supportedClassFilter;
			}
		}

		public static QueryFilter BuildMessageClassFilter(IList<string> supportedIpmTypes)
		{
			QueryFilter result;
			switch (supportedIpmTypes.Count)
			{
			case 0:
				result = AirSyncXsoSchemaState.FalseFilterInstance;
				break;
			case 1:
				result = new TextFilter(StoreObjectSchema.ItemClass, supportedIpmTypes[0], MatchOptions.PrefixOnWords, MatchFlags.IgnoreCase);
				break;
			default:
			{
				QueryFilter[] array = new QueryFilter[supportedIpmTypes.Count];
				for (int i = 0; i < supportedIpmTypes.Count; i++)
				{
					array[i] = new TextFilter(StoreObjectSchema.ItemClass, supportedIpmTypes[i], MatchOptions.PrefixOnWords, MatchFlags.IgnoreCase);
				}
				result = new OrFilter(array);
				break;
			}
			}
			return result;
		}

		public XsoDataObject GetXsoDataObject()
		{
			return new XsoDataObject(base.GetSchema(1), this, this.SupportedClassFilter);
		}

		public XsoDataObject GetInnerXsoDataObject()
		{
			if (this.innerSchemaState == null)
			{
				return null;
			}
			return new XsoDataObject(this.innerSchemaState.GetSchema(1), null, null);
		}

		public static readonly QueryFilter FalseFilterInstance = new FalseFilter();

		private readonly QueryFilter supportedClassFilter;

		private AirSyncXsoSchemaState innerSchemaState;
	}
}
