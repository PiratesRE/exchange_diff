using System;
using System.Diagnostics;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data
{
	[DebuggerDisplay("Property Name = {PropertyName, nq}; Value = {GetValue()};")]
	internal class AuditHistoryResult : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public string PropertyName
		{
			get
			{
				return this[AuditHistoryResult.PropertyNameDefinition] as string;
			}
			set
			{
				this[AuditHistoryResult.PropertyNameDefinition] = value;
			}
		}

		public int? PropertyId
		{
			get
			{
				return this[AuditHistoryResult.PropertyIdDefinition] as int?;
			}
			set
			{
				this[AuditHistoryResult.PropertyIdDefinition] = value;
			}
		}

		public Guid? PropertyValueGuid
		{
			get
			{
				return this[AuditHistoryResult.PropertyValueGuidDefinition] as Guid?;
			}
			set
			{
				this[AuditHistoryResult.PropertyValueGuidDefinition] = value;
			}
		}

		public int? PropertyValueInteger
		{
			get
			{
				return this[AuditHistoryResult.PropertyValueIntegerDefinition] as int?;
			}
			set
			{
				this[AuditHistoryResult.PropertyValueIntegerDefinition] = value;
			}
		}

		public long? PropertyValueLong
		{
			get
			{
				return this[AuditHistoryResult.PropertyValueLongDefinition] as long?;
			}
			set
			{
				this[AuditHistoryResult.PropertyValueLongDefinition] = value;
			}
		}

		public string PropertyValueString
		{
			get
			{
				return this[AuditHistoryResult.PropertyValueStringDefinition] as string;
			}
			set
			{
				this[AuditHistoryResult.PropertyValueLongDefinition] = value;
			}
		}

		public DateTime? PropertyValueDateTime
		{
			get
			{
				return this[AuditHistoryResult.PropertyValueDateTimeDefinition] as DateTime?;
			}
			set
			{
				this[AuditHistoryResult.PropertyValueDateTimeDefinition] = value;
			}
		}

		public bool? PropertyValueBit
		{
			get
			{
				return this[AuditHistoryResult.PropertyValueBitDefinition] as bool?;
			}
			set
			{
				this[AuditHistoryResult.PropertyValueBitDefinition] = value;
			}
		}

		public decimal? PropertyValueDecimal
		{
			get
			{
				return this[AuditHistoryResult.PropertyValueDecimalDefinition] as decimal?;
			}
			set
			{
				this[AuditHistoryResult.PropertyValueDecimalDefinition] = value;
			}
		}

		public string PropertyValueBlob
		{
			get
			{
				return this[AuditHistoryResult.PropertyValueBlobDefinition] as string;
			}
			set
			{
				this[AuditHistoryResult.PropertyValueBlobDefinition] = value;
			}
		}

		public DateTime? ChangedDateTime
		{
			get
			{
				return this[AuditHistoryResult.ChangedDateTimeDefinition] as DateTime?;
			}
			set
			{
				this[AuditHistoryResult.ChangedDateTimeDefinition] = value;
			}
		}

		public DateTime? DeletedDateTime
		{
			get
			{
				return this[AuditHistoryResult.DeletedDateTimeDefinition] as DateTime?;
			}
			set
			{
				this[AuditHistoryResult.DeletedDateTimeDefinition] = value;
			}
		}

		public object GetValue()
		{
			object result = null;
			if (this.PropertyValueGuid != null)
			{
				result = this.PropertyValueGuid;
			}
			else if (this.PropertyValueInteger != null)
			{
				result = this.PropertyValueInteger;
			}
			else if (this.PropertyValueLong != null)
			{
				result = this.PropertyValueLong;
			}
			else if (!string.IsNullOrEmpty(this.PropertyValueString))
			{
				result = this.PropertyValueString;
			}
			else if (this.PropertyValueDateTime != null)
			{
				result = this.PropertyValueDateTime;
			}
			else if (this.PropertyValueBit != null)
			{
				result = this.PropertyValueBit;
			}
			else if (this.PropertyValueDecimal != null)
			{
				result = this.PropertyValueDecimal;
			}
			else if (!string.IsNullOrEmpty(this.PropertyValueBlob))
			{
				result = this.PropertyValueBlob;
			}
			return result;
		}

		public static readonly HygienePropertyDefinition EntityInstanceIdParameterDefinition = new HygienePropertyDefinition("id_EntityInstanceId", typeof(Guid?));

		public static readonly HygienePropertyDefinition EntityNameParameterDefinition = new HygienePropertyDefinition("nvc_EntityName", typeof(string));

		public static readonly HygienePropertyDefinition PartitionIdParameterDefinition = new HygienePropertyDefinition("id_PartitionId", typeof(Guid));

		public static readonly HygienePropertyDefinition StartTimeParameterDefinition = new HygienePropertyDefinition("dt_StartTime", typeof(DateTime?));

		public static readonly HygienePropertyDefinition EndTimeParameterDefinition = new HygienePropertyDefinition("dt_EndTime", typeof(DateTime?));

		public static readonly HygienePropertyDefinition PropertyIdDefinition = new HygienePropertyDefinition("i_PropertyId", typeof(int?));

		public static readonly HygienePropertyDefinition PropertyNameDefinition = new HygienePropertyDefinition("nvc_PropertyName", typeof(string));

		public static readonly HygienePropertyDefinition ChangedDateTimeDefinition = new HygienePropertyDefinition("dt_ChangedDatetime", typeof(DateTime?));

		public static readonly HygienePropertyDefinition DeletedDateTimeDefinition = new HygienePropertyDefinition("dt_DeletedDatetime", typeof(DateTime?));

		public static readonly HygienePropertyDefinition PropertyValueGuidDefinition = new HygienePropertyDefinition("id_PropertyValueGuid", typeof(Guid?));

		public static readonly HygienePropertyDefinition PropertyValueIntegerDefinition = new HygienePropertyDefinition("i_PropertyValueInteger", typeof(int?));

		public static readonly HygienePropertyDefinition PropertyValueLongDefinition = new HygienePropertyDefinition("bi_PropertyValueLong", typeof(long?));

		public static readonly HygienePropertyDefinition PropertyValueStringDefinition = new HygienePropertyDefinition("nvc_PropertyValueString", typeof(string));

		public static readonly HygienePropertyDefinition PropertyValueDateTimeDefinition = new HygienePropertyDefinition("dt_PropertyValueDateTime", typeof(DateTime?));

		public static readonly HygienePropertyDefinition PropertyValueBitDefinition = new HygienePropertyDefinition("f_PropertyValueBit", typeof(bool?));

		public static readonly HygienePropertyDefinition PropertyValueDecimalDefinition = new HygienePropertyDefinition("d_PropertyValueDecimal", typeof(decimal?));

		public static readonly HygienePropertyDefinition PropertyValueBlobDefinition = new HygienePropertyDefinition("nvc_PropertyValueBlob", typeof(string));
	}
}
