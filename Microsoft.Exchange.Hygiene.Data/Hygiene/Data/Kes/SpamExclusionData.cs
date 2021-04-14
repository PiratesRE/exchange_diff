using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Kes
{
	internal class SpamExclusionData : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.SpamExclusionDataID.ToString());
			}
		}

		internal Guid SpamExclusionDataID
		{
			get
			{
				return (Guid)this[SpamExclusionData.SpamExclusionDataIDProperty];
			}
			set
			{
				this[SpamExclusionData.SpamExclusionDataIDProperty] = value;
			}
		}

		internal SpamExclusionDataID DataID
		{
			get
			{
				return (SpamExclusionDataID)this[SpamExclusionData.DataIDProperty];
			}
			set
			{
				this[SpamExclusionData.DataIDProperty] = (byte)value;
			}
		}

		internal byte DataTypeID
		{
			get
			{
				return (byte)this[SpamExclusionData.DataTypeIDProperty];
			}
			set
			{
				this[SpamExclusionData.DataTypeIDProperty] = value;
			}
		}

		internal string ExclusionDataTag
		{
			get
			{
				return (string)this[SpamExclusionData.ExclusionDataTagProperty];
			}
			set
			{
				this[SpamExclusionData.ExclusionDataTagProperty] = value;
			}
		}

		internal string ExclusionData
		{
			get
			{
				return (string)this[SpamExclusionData.ExclusionDataProperty];
			}
			set
			{
				this[SpamExclusionData.ExclusionDataProperty] = value;
			}
		}

		internal bool IsPersistent
		{
			get
			{
				return (bool)this[SpamExclusionData.IsPersistentProperty];
			}
			set
			{
				this[SpamExclusionData.IsPersistentProperty] = value;
			}
		}

		internal bool IsListed
		{
			get
			{
				return (bool)this[SpamExclusionData.IsListedProperty];
			}
			set
			{
				this[SpamExclusionData.IsListedProperty] = value;
			}
		}

		internal string CreatedBy
		{
			get
			{
				return (string)this[SpamExclusionData.CreatedByProperty];
			}
			set
			{
				this[SpamExclusionData.CreatedByProperty] = value;
			}
		}

		internal DateTime? ExpirationDate
		{
			get
			{
				return (DateTime?)this[SpamExclusionData.ExpirationDateProperty];
			}
			set
			{
				this[SpamExclusionData.ExpirationDateProperty] = value;
			}
		}

		internal string Comment
		{
			get
			{
				return (string)this[SpamExclusionData.CommentProperty];
			}
			set
			{
				this[SpamExclusionData.CommentProperty] = value;
			}
		}

		public static readonly HygienePropertyDefinition SpamExclusionDataIDProperty = new HygienePropertyDefinition("id_SpamExclusionDataId", typeof(Guid));

		public static readonly HygienePropertyDefinition DataIDProperty = new HygienePropertyDefinition("ti_DataId", typeof(byte), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition DataTypeIDProperty = new HygienePropertyDefinition("ti_DataTypeId", typeof(byte), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition ExclusionDataTagProperty = new HygienePropertyDefinition("nvc_ExclusionDataTag", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition ExclusionDataProperty = new HygienePropertyDefinition("nvc_ExclusionData", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition IsPersistentProperty = new HygienePropertyDefinition("f_IsPersistent", typeof(bool));

		public static readonly HygienePropertyDefinition IsListedProperty = new HygienePropertyDefinition("f_IsListed", typeof(bool));

		public static readonly HygienePropertyDefinition CreatedByProperty = new HygienePropertyDefinition("nvc_CreatedBy", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition ExpirationDateProperty = new HygienePropertyDefinition("dt_ExpirationDate", typeof(DateTime?));

		public static readonly HygienePropertyDefinition CommentProperty = new HygienePropertyDefinition("nvc_Comment", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
