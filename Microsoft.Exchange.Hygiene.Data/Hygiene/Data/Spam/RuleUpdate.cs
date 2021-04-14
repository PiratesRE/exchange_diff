using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	internal class RuleUpdate : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.ID.ToString());
			}
		}

		public Guid? ID
		{
			get
			{
				return (Guid?)this[RuleUpdate.IDProperty];
			}
			set
			{
				this[RuleUpdate.IDProperty] = value;
			}
		}

		public long? RuleID
		{
			get
			{
				return (long?)this[RuleUpdate.RuleIDProperty];
			}
			set
			{
				this[RuleUpdate.RuleIDProperty] = value;
			}
		}

		public byte? RuleType
		{
			get
			{
				return (byte?)this[RuleUpdate.RuleTypeProperty];
			}
			set
			{
				this[RuleUpdate.RuleTypeProperty] = value;
			}
		}

		public bool? IsPersistent
		{
			get
			{
				return (bool?)this[RuleUpdate.IsPersistentProperty];
			}
			set
			{
				this[RuleUpdate.IsPersistentProperty] = value;
			}
		}

		public bool? IsActive
		{
			get
			{
				return (bool?)this[RuleUpdate.IsActiveProperty];
			}
			set
			{
				this[RuleUpdate.IsActiveProperty] = value;
			}
		}

		public byte? State
		{
			get
			{
				return (byte?)this[RuleUpdate.StateProperty];
			}
			set
			{
				this[RuleUpdate.StateProperty] = value;
			}
		}

		public long? AddedVersion
		{
			get
			{
				return (long?)this[RuleUpdate.AddedVersionProperty];
			}
			set
			{
				this[RuleUpdate.AddedVersionProperty] = value;
			}
		}

		public long? RemovedVersion
		{
			get
			{
				return (long?)this[RuleUpdate.RemovedVersionProperty];
			}
			set
			{
				this[RuleUpdate.RemovedVersionProperty] = value;
			}
		}

		public static readonly HygienePropertyDefinition IDProperty = new HygienePropertyDefinition("id_RuleId", typeof(Guid?));

		public static readonly HygienePropertyDefinition RuleIDProperty = new HygienePropertyDefinition("bi_RuleId", typeof(long?));

		public static readonly HygienePropertyDefinition RuleTypeProperty = new HygienePropertyDefinition("ti_RuleType", typeof(byte?));

		public static readonly HygienePropertyDefinition IsPersistentProperty = new HygienePropertyDefinition("f_IsPersistent", typeof(bool?));

		public static readonly HygienePropertyDefinition IsActiveProperty = new HygienePropertyDefinition("f_IsActive", typeof(bool?));

		public static readonly HygienePropertyDefinition StateProperty = new HygienePropertyDefinition("ti_State", typeof(byte?));

		public static readonly HygienePropertyDefinition AddedVersionProperty = new HygienePropertyDefinition("bi_AddedVersion", typeof(long?));

		public static readonly HygienePropertyDefinition RemovedVersionProperty = new HygienePropertyDefinition("bi_RemovedVersion", typeof(long?));
	}
}
