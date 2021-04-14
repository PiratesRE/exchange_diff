using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	internal class DeleteAll : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(Guid.NewGuid().ToString());
			}
		}

		public string EntityName
		{
			get
			{
				return (string)this[DeleteAll.EntityNameProperty];
			}
			set
			{
				this[DeleteAll.EntityNameProperty] = value;
			}
		}

		public string PropertyName
		{
			get
			{
				return (string)this[DeleteAll.PropertyNameProperty];
			}
			set
			{
				this[DeleteAll.PropertyNameProperty] = value;
			}
		}

		public int? IntValue
		{
			get
			{
				return (int?)this[DeleteAll.IntValueProperty];
			}
			set
			{
				this[DeleteAll.IntValueProperty] = value;
			}
		}

		public Guid? GuidValue
		{
			get
			{
				return (Guid?)this[DeleteAll.GuidValueProperty];
			}
			set
			{
				this[DeleteAll.GuidValueProperty] = value;
			}
		}

		public string StringValue
		{
			get
			{
				return (string)this[DeleteAll.StringValueProperty];
			}
			set
			{
				this[DeleteAll.StringValueProperty] = value;
			}
		}

		public bool? BoolValue
		{
			get
			{
				return (bool?)this[DeleteAll.BoolValueProperty];
			}
			set
			{
				this[DeleteAll.BoolValueProperty] = value;
			}
		}

		public DateTime? DatetimeValue
		{
			get
			{
				return (DateTime?)this[DeleteAll.DatetimeValueProperty];
			}
			set
			{
				this[DeleteAll.DatetimeValueProperty] = value;
			}
		}

		public decimal? DecimalValue
		{
			get
			{
				return (decimal?)this[DeleteAll.DecimalValueProperty];
			}
			set
			{
				this[DeleteAll.DecimalValueProperty] = value;
			}
		}

		public long? LongValue
		{
			get
			{
				return (long?)this[DeleteAll.LongValueProperty];
			}
			set
			{
				this[DeleteAll.LongValueProperty] = value;
			}
		}

		public bool? HardDelete
		{
			get
			{
				return (bool?)this[DeleteAll.HardDeleteProperty];
			}
			set
			{
				this[DeleteAll.HardDeleteProperty] = value;
			}
		}

		public static readonly HygienePropertyDefinition EntityNameProperty = new HygienePropertyDefinition("nvc_EntityName", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition PropertyNameProperty = new HygienePropertyDefinition("nvc_PropertyName", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition IntValueProperty = new HygienePropertyDefinition("i_PropertyValueInteger", typeof(int?));

		public static readonly HygienePropertyDefinition GuidValueProperty = new HygienePropertyDefinition("id_PropertyValueGuid", typeof(Guid?));

		public static readonly HygienePropertyDefinition StringValueProperty = new HygienePropertyDefinition("nvc_PropertyValueString", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition BoolValueProperty = new HygienePropertyDefinition("f_PropertyValueBit", typeof(bool?));

		public static readonly HygienePropertyDefinition DatetimeValueProperty = new HygienePropertyDefinition("dt_PropertyValueDateTime", typeof(DateTime?));

		public static readonly HygienePropertyDefinition DecimalValueProperty = new HygienePropertyDefinition("d_PropertyValueDecimal", typeof(decimal?));

		public static readonly HygienePropertyDefinition LongValueProperty = new HygienePropertyDefinition("bi_PropertyValueLong", typeof(long?));

		public static readonly HygienePropertyDefinition HardDeleteProperty = new HygienePropertyDefinition("f_HardDelete", typeof(bool?));

		public static class Entity
		{
			public const string PredicateExtendedProperties = "PredicateExtendedProperties";

			public const string Predicates = "Predicates";

			public const string Processors = "Processors";

			public const string RuleExtendedProperties = "RuleExtendedProperties";

			public const string Rules = "Rules";

			public const string SpamRules = "SpamRules";

			public const string SpamRuleProcessors = "SpamRuleProcessors";

			public const string SyncWatermark = "SyncWatermark";

			public const string SpamDataBlobs = "SpamDataBlobs";

			public const string SpamExclusionData = "SpamExclusionData";
		}

		public static class Property
		{
			public const string IdPredicateId = "id_PredicateId";

			public const string IdRuleId = "id_RuleId";

			public const string BiRuleId = "bi_RuleId";

			public const string BiProcessorId = "bi_ProcessorId";

			public const string IdIdentity = "id_Identity";

			public const string NvcSyncContext = "nvc_SyncContext";

			public const string IdSpamExclusionDataId = "id_SpamExclusionDataId";

			public const string IdSpamDataBlobId = "IdSpamDataBlobId";

			public const string TiDataId = "ti_DataId";
		}
	}
}
