using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.LoggingCommon
{
	internal static class TenantSettingSchema
	{
		static TenantSettingSchema()
		{
			Array values = Enum.GetValues(typeof(TenantSettingSchemaFields));
			if (values.Length > 0 && (int)values.GetValue(values.Length - 1) != values.Length - 1)
			{
				throw new InvalidOperationException("TenantSettingSchemaFields has invalid enum definitions");
			}
			CsvField[] array = new CsvField[values.Length];
			array[0] = new CsvField(TenantSettingSchemaFields.DateTime.ToString(), typeof(DateTime), TenantSettingSchema.E15Version);
			array[1] = new CsvField(TenantSettingSchemaFields.ChangeType.ToString(), typeof(int), TenantSettingSchema.E15Version);
			array[2] = new CsvField(TenantSettingSchemaFields.TenantID.ToString(), typeof(string), TenantSettingSchema.E15Version);
			array[3] = new CsvField(TenantSettingSchemaFields.SettingId.ToString(), typeof(string), TenantSettingSchema.E15Version);
			array[4] = new CsvField(TenantSettingSchemaFields.Name.ToString(), typeof(string), TenantSettingSchema.E15Version);
			array[5] = new CsvField(TenantSettingSchemaFields.CustomData.ToString(), typeof(KeyValuePair<string, object>[]), TenantSettingSchema.E15Version);
			TenantSettingSchema.schema = new CsvTable(array);
		}

		internal static Version E15FirstVersion
		{
			get
			{
				return TenantSettingSchema.E15Version;
			}
		}

		internal static CsvTable Schema
		{
			get
			{
				return TenantSettingSchema.schema;
			}
		}

		internal static List<Version> SupportedVersionsInAscendingOrder
		{
			get
			{
				return TenantSettingSchema.supportedVersionsInAscendingOrder;
			}
		}

		private static readonly Version E15Version = new Version(15, 0, 0, 0);

		private static readonly List<Version> supportedVersionsInAscendingOrder = new List<Version>
		{
			TenantSettingSchema.E15Version
		};

		private static readonly CsvTable schema;
	}
}
