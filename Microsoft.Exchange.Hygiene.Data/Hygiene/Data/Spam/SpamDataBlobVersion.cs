using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	internal class SpamDataBlobVersion : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					this.DataID,
					this.DataTypeID,
					this.MajorVersion,
					this.MinorVersion
				}));
			}
		}

		internal SpamDataBlobDataID DataID
		{
			get
			{
				return (SpamDataBlobDataID)this[SpamDataBlobVersion.DataIDProperty];
			}
			set
			{
				this[SpamDataBlobVersion.DataIDProperty] = (byte)value;
			}
		}

		internal byte DataTypeID
		{
			get
			{
				return (byte)this[SpamDataBlobVersion.DataTypeIDProperty];
			}
			set
			{
				this[SpamDataBlobVersion.DataTypeIDProperty] = value;
			}
		}

		internal int MajorVersion
		{
			get
			{
				return (int)this[SpamDataBlobVersion.MajorVersionProperty];
			}
			set
			{
				this[SpamDataBlobVersion.MajorVersionProperty] = value;
			}
		}

		internal int MinorVersion
		{
			get
			{
				return (int)this[SpamDataBlobVersion.MinorVersionProperty];
			}
			set
			{
				this[SpamDataBlobVersion.MinorVersionProperty] = value;
			}
		}

		internal long SpamDataTotalSize
		{
			get
			{
				return (long)this[SpamDataBlobVersion.SpamDataTotalSizeProperty];
			}
			set
			{
				this[SpamDataBlobVersion.SpamDataTotalSizeProperty] = value;
			}
		}

		public DateTime ChangedDateTime
		{
			get
			{
				return (DateTime)this[SpamDataBlobVersion.ChangedDateTimeProperty];
			}
		}

		public static readonly HygienePropertyDefinition DataIDProperty = new HygienePropertyDefinition("ti_DataId", typeof(byte), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition DataTypeIDProperty = new HygienePropertyDefinition("ti_DataTypeId", typeof(byte), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition MajorVersionProperty = new HygienePropertyDefinition("i_MajorVersion", typeof(int), int.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition MinorVersionProperty = new HygienePropertyDefinition("i_MinorVersion", typeof(int), int.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition SpamDataTotalSizeProperty = new HygienePropertyDefinition("bi_SpamDataTotalSize", typeof(long), long.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition ChangedDateTimeProperty = new HygienePropertyDefinition("dt_ChangedDateTime", typeof(DateTime?));
	}
}
