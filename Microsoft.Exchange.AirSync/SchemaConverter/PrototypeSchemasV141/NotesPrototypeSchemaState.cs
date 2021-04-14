using System;
using Microsoft.Exchange.AirSync.SchemaConverter.AirSync;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.AirSync.SchemaConverter.XSO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.PrototypeSchemasV141
{
	internal class NotesPrototypeSchemaState : AirSyncXsoSchemaState
	{
		public NotesPrototypeSchemaState() : base(NotesPrototypeSchemaState.supportedClassFilter)
		{
			base.InitConversionTable(2);
			this.CreatePropertyConversionTable();
		}

		internal static QueryFilter SupportedClassQueryFilter
		{
			get
			{
				return NotesPrototypeSchemaState.supportedClassFilter;
			}
		}

		private void CreatePropertyConversionTable()
		{
			string xmlNodeNamespace = "Notes:";
			string xmlNodeNamespace2 = "AirSyncBase:";
			base.AddProperty(new IProperty[]
			{
				new AirSyncContent14Property(xmlNodeNamespace2, "Body", true),
				new XsoContent14Property()
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncStringProperty(xmlNodeNamespace, "Subject", true),
				new XsoStringProperty(ItemSchema.Subject)
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncStringProperty(xmlNodeNamespace, "MessageClass", true),
				new XsoStringProperty(StoreObjectSchema.ItemClass)
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncUtcDateTimeProperty(xmlNodeNamespace, "LastModifiedDate", true),
				new XsoUtcDateTimeProperty(StoreObjectSchema.LastModifiedTime)
			});
			base.AddProperty(new IProperty[]
			{
				new AirSyncMultiValuedStringProperty(xmlNodeNamespace, "Categories", "Category", true),
				new XsoCategoriesProperty()
			});
		}

		private static readonly string[] supportedClassTypes = new string[]
		{
			"IPM.STICKYNOTE"
		};

		private static readonly QueryFilter supportedClassFilter = AirSyncXsoSchemaState.BuildMessageClassFilter(NotesPrototypeSchemaState.supportedClassTypes);
	}
}
