using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	internal class RulePredicate : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.PredicateID.ToString());
			}
		}

		public Guid ID
		{
			get
			{
				return (Guid)this[RulePredicate.IDProperty];
			}
			set
			{
				this[RulePredicate.IDProperty] = value;
			}
		}

		public Guid PredicateID
		{
			get
			{
				return (Guid)this[RulePredicate.PredicateIDProperty];
			}
			set
			{
				this[RulePredicate.PredicateIDProperty] = value;
			}
		}

		public decimal? Sequence
		{
			get
			{
				return (decimal?)this[RulePredicate.SequenceProperty];
			}
			set
			{
				this[RulePredicate.SequenceProperty] = value;
			}
		}

		public Guid? ParentID
		{
			get
			{
				return (Guid?)this[RulePredicate.ParentIDProperty];
			}
			set
			{
				this[RulePredicate.ParentIDProperty] = value;
			}
		}

		public byte? PredicateType
		{
			get
			{
				return (byte?)this[RulePredicate.PredicateTypeProperty];
			}
			set
			{
				this[RulePredicate.PredicateTypeProperty] = value;
			}
		}

		public long? ProcessorID
		{
			get
			{
				return (long?)this[RulePredicate.ProcessorIdProperty];
			}
			set
			{
				this[RulePredicate.ProcessorIdProperty] = value;
			}
		}

		public static readonly HygienePropertyDefinition IDProperty = new HygienePropertyDefinition("id_RuleId", typeof(Guid));

		public static readonly HygienePropertyDefinition PredicateIDProperty = new HygienePropertyDefinition("id_PredicateId", typeof(Guid));

		public static readonly HygienePropertyDefinition ParentIDProperty = new HygienePropertyDefinition("id_ParentId", typeof(Guid?));

		public static readonly HygienePropertyDefinition ProcessorIdProperty = new HygienePropertyDefinition("bi_ProcessorId", typeof(long?));

		public static readonly HygienePropertyDefinition PredicateTypeProperty = new HygienePropertyDefinition("ti_PredicateType", typeof(byte?));

		public static readonly HygienePropertyDefinition SequenceProperty = new HygienePropertyDefinition("d_Sequence", typeof(decimal?));
	}
}
