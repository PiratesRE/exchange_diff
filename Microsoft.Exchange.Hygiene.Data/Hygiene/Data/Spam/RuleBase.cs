using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	internal class RuleBase : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.ID.ToString());
			}
		}

		public Guid ID
		{
			get
			{
				return (Guid)this[RuleBase.IDProperty];
			}
			set
			{
				this[RuleBase.IDProperty] = value;
			}
		}

		public long? RuleID
		{
			get
			{
				return (long?)this[RuleBase.RuleIDProperty];
			}
			set
			{
				this[RuleBase.RuleIDProperty] = value;
			}
		}

		public byte? ScopeID
		{
			get
			{
				return (byte?)this[RuleBase.ScopeIDProperty];
			}
			set
			{
				this[RuleBase.ScopeIDProperty] = value;
			}
		}

		public byte? RuleType
		{
			get
			{
				return (byte?)this[RuleBase.RuleTypeProperty];
			}
			set
			{
				this[RuleBase.RuleTypeProperty] = value;
			}
		}

		public long? GroupID
		{
			get
			{
				return (long?)this[RuleBase.GroupIDProperty];
			}
			set
			{
				this[RuleBase.GroupIDProperty] = value;
			}
		}

		public decimal? Sequence
		{
			get
			{
				return (decimal?)this[RuleBase.SequenceProperty];
			}
			set
			{
				this[RuleBase.SequenceProperty] = value;
			}
		}

		public DateTime? CreatedDatetime
		{
			get
			{
				return (DateTime?)this[RuleBase.CreatedDatetimeProperty];
			}
			set
			{
				this[RuleBase.CreatedDatetimeProperty] = value;
			}
		}

		public DateTime? ChangeDatetime
		{
			get
			{
				return (DateTime?)this[RuleBase.ChangedDatetimeProperty];
			}
			set
			{
				this[RuleBase.ChangedDatetimeProperty] = value;
			}
		}

		public DateTime? DeletedDatetime
		{
			get
			{
				return (DateTime?)this[RuleBase.DeletedDatetimeProperty];
			}
			set
			{
				this[RuleBase.DeletedDatetimeProperty] = value;
			}
		}

		public bool? IsPersistent
		{
			get
			{
				return (bool?)this[RuleBase.IsPersistentProperty];
			}
			set
			{
				this[RuleBase.IsPersistentProperty] = value;
			}
		}

		public bool? IsActive
		{
			get
			{
				return (bool?)this[RuleBase.IsActiveProperty];
			}
			set
			{
				this[RuleBase.IsActiveProperty] = value;
			}
		}

		public byte? State
		{
			get
			{
				return (byte?)this[RuleBase.StateProperty];
			}
			set
			{
				this[RuleBase.StateProperty] = value;
			}
		}

		public long? AddedVersion
		{
			get
			{
				return (long?)this[RuleBase.AddedVersionProperty];
			}
			set
			{
				this[RuleBase.AddedVersionProperty] = value;
			}
		}

		public long? RemovedVersion
		{
			get
			{
				return (long?)this[RuleBase.RemovedVersionProperty];
			}
			set
			{
				this[RuleBase.RemovedVersionProperty] = value;
			}
		}

		public static readonly HygienePropertyDefinition IDProperty = new HygienePropertyDefinition("id_RuleId", typeof(Guid));

		public static readonly HygienePropertyDefinition RuleIDProperty = new HygienePropertyDefinition("bi_RuleId", typeof(long?));

		public static readonly HygienePropertyDefinition RuleTypeProperty = new HygienePropertyDefinition("ti_RuleType", typeof(byte?));

		public static readonly HygienePropertyDefinition ScopeIDProperty = new HygienePropertyDefinition("ti_ScopeId", typeof(byte?));

		public static readonly HygienePropertyDefinition GroupIDProperty = new HygienePropertyDefinition("bi_GroupId", typeof(long?));

		public static readonly HygienePropertyDefinition SequenceProperty = new HygienePropertyDefinition("d_Sequence", typeof(decimal?));

		public static readonly HygienePropertyDefinition CreatedDatetimeProperty = new HygienePropertyDefinition("dt_CreatedDatetime", typeof(DateTime?));

		public static readonly HygienePropertyDefinition ChangedDatetimeProperty = new HygienePropertyDefinition("dt_ChangedDatetime", typeof(DateTime?));

		public static readonly HygienePropertyDefinition DeletedDatetimeProperty = new HygienePropertyDefinition("dt_DeletedDatetime", typeof(DateTime?));

		public static readonly HygienePropertyDefinition IsPersistentProperty = new HygienePropertyDefinition("f_IsPersistent", typeof(bool?));

		public static readonly HygienePropertyDefinition IsActiveProperty = new HygienePropertyDefinition("f_IsActive", typeof(bool?));

		public static readonly HygienePropertyDefinition StateProperty = new HygienePropertyDefinition("ti_State", typeof(byte?));

		public static readonly HygienePropertyDefinition AddedVersionProperty = new HygienePropertyDefinition("bi_AddedVersion", typeof(long?));

		public static readonly HygienePropertyDefinition RemovedVersionProperty = new HygienePropertyDefinition("bi_RemovedVersion", typeof(long?));
	}
}
