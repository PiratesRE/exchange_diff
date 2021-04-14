using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.SchemaConverter;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	[Serializable]
	internal class XsoUidProperty : XsoStringProperty
	{
		public XsoUidProperty(StorePropertyDefinition propertyDef) : base(propertyDef)
		{
		}

		public XsoUidProperty(StorePropertyDefinition propertyDef, PropertyType type) : base(propertyDef, type)
		{
		}

		public override string StringData
		{
			get
			{
				byte[] array = base.XsoItem.TryGetProperty(base.PropertyDef) as byte[];
				if (array == null)
				{
					AirSyncDiagnostics.Assert(false, "GlobalObjectId property not returned by XSO", new object[0]);
					return string.Empty;
				}
				GlobalObjectId globalObjectId = null;
				try
				{
					globalObjectId = new GlobalObjectId(array);
				}
				catch (CorruptDataException)
				{
					return string.Empty;
				}
				string text = globalObjectId.Uid;
				int num = text.IndexOf('\0');
				if (num != -1)
				{
					text = text.Substring(0, num);
				}
				return text;
			}
		}

		protected override void InternalCopyFromModified(IProperty srcProperty)
		{
			IStringProperty stringProperty = (IStringProperty)srcProperty;
			byte[] array = base.XsoItem.TryGetProperty(base.PropertyDef) as byte[];
			if (array == null)
			{
				array = new GlobalObjectId(stringProperty.StringData).Bytes;
				base.XsoItem[CalendarItemBaseSchema.GlobalObjectId] = array;
				base.XsoItem[CalendarItemBaseSchema.CleanGlobalObjectId] = array;
				return;
			}
			GlobalObjectId globalObjectId = new GlobalObjectId(array);
			if (globalObjectId.Uid != stringProperty.StringData)
			{
				AirSyncDiagnostics.TraceDebug<string, string>(ExTraceGlobals.XsoTracer, this, "Client overwrote Uid from {0} to {1}", globalObjectId.Uid, stringProperty.StringData);
			}
			array = new GlobalObjectId(stringProperty.StringData).Bytes;
			base.XsoItem[CalendarItemBaseSchema.GlobalObjectId] = array;
			base.XsoItem[CalendarItemBaseSchema.CleanGlobalObjectId] = array;
		}
	}
}
