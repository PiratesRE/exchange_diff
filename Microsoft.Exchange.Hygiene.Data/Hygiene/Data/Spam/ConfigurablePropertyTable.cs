using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	internal class ConfigurablePropertyTable : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(Guid.NewGuid().ToString());
			}
		}

		public string PropertyName
		{
			get
			{
				return (string)this[ConfigurablePropertyTable.NameProperty];
			}
			set
			{
				this[ConfigurablePropertyTable.NameProperty] = value;
			}
		}

		public int? IntValue
		{
			get
			{
				return (int?)this[ConfigurablePropertyTable.IntValueProperty];
			}
			set
			{
				this[ConfigurablePropertyTable.IntValueProperty] = value;
			}
		}

		public string StringValue
		{
			get
			{
				if (!string.IsNullOrEmpty(this[ConfigurablePropertyTable.BlobValueProperty] as string))
				{
					return (string)this[ConfigurablePropertyTable.BlobValueProperty];
				}
				return (string)this[ConfigurablePropertyTable.StringValueProperty];
			}
			set
			{
				if (!string.IsNullOrEmpty(value) && value.Length > 255)
				{
					this[ConfigurablePropertyTable.BlobValueProperty] = value;
					return;
				}
				this[ConfigurablePropertyTable.StringValueProperty] = value;
			}
		}

		public string BlobValue
		{
			get
			{
				return (string)this[ConfigurablePropertyTable.BlobValueProperty];
			}
			set
			{
				this[ConfigurablePropertyTable.BlobValueProperty] = value;
			}
		}

		public Guid? GuidValue
		{
			get
			{
				return (Guid?)this[ConfigurablePropertyTable.GuidValueProperty];
			}
			set
			{
				this[ConfigurablePropertyTable.GuidValueProperty] = value;
			}
		}

		public bool? BoolValue
		{
			get
			{
				return (bool?)this[ConfigurablePropertyTable.BoolValueProperty];
			}
			set
			{
				this[ConfigurablePropertyTable.BoolValueProperty] = value;
			}
		}

		public DateTime? DatetimeValue
		{
			get
			{
				return (DateTime?)this[ConfigurablePropertyTable.DatetimeValueProperty];
			}
			set
			{
				this[ConfigurablePropertyTable.DatetimeValueProperty] = value;
			}
		}

		public decimal? DecimalValue
		{
			get
			{
				return (decimal?)this[ConfigurablePropertyTable.DecimalValueProperty];
			}
			set
			{
				this[ConfigurablePropertyTable.DecimalValueProperty] = value;
			}
		}

		public long? LongValue
		{
			get
			{
				return (long?)this[ConfigurablePropertyTable.LongValueProperty];
			}
			set
			{
				this[ConfigurablePropertyTable.LongValueProperty] = value;
			}
		}

		public static readonly HygienePropertyDefinition NameProperty = new HygienePropertyDefinition("nvc_PropertyName", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition IndexProperty = new HygienePropertyDefinition("i_PropertyIndex", typeof(int?));

		public static readonly HygienePropertyDefinition IntValueProperty = new HygienePropertyDefinition("i_PropertyValueInteger", typeof(int?));

		public static readonly HygienePropertyDefinition StringValueProperty = new HygienePropertyDefinition("nvc_PropertyValueString", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition BoolValueProperty = new HygienePropertyDefinition("f_PropertyValueBit", typeof(bool?));

		public static readonly HygienePropertyDefinition DatetimeValueProperty = new HygienePropertyDefinition("dt_PropertyValueDateTime", typeof(DateTime?));

		public static readonly HygienePropertyDefinition DecimalValueProperty = new HygienePropertyDefinition("d_PropertyValueDecimal", typeof(decimal?));

		public static readonly HygienePropertyDefinition LongValueProperty = new HygienePropertyDefinition("bi_PropertyValueLong", typeof(long?));

		public static readonly HygienePropertyDefinition GuidValueProperty = new HygienePropertyDefinition("id_PropertyValueGuid", typeof(Guid?));

		public static readonly HygienePropertyDefinition BlobValueProperty = new HygienePropertyDefinition("nvc_PropertyValueBlob", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
