using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	internal class SpamRuleBlob : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(string.Format("{0}\\{1}", this.RuleId, this.ScopeId));
			}
		}

		public Guid Id
		{
			get
			{
				return (Guid)this[SpamRuleBlobSchema.IdProperty];
			}
			set
			{
				this[SpamRuleBlobSchema.IdProperty] = value;
			}
		}

		public long RuleId
		{
			get
			{
				return (long)this[SpamRuleBlobSchema.RuleIdProperty];
			}
			set
			{
				this[SpamRuleBlobSchema.RuleIdProperty] = value;
			}
		}

		public long GroupId
		{
			get
			{
				return (long)this[SpamRuleBlobSchema.GroupIdProperty];
			}
			set
			{
				this[SpamRuleBlobSchema.GroupIdProperty] = value;
			}
		}

		public byte ScopeId
		{
			get
			{
				return (byte)this[SpamRuleBlobSchema.ScopeIdProperty];
			}
			set
			{
				this[SpamRuleBlobSchema.ScopeIdProperty] = value;
			}
		}

		public byte PublishingState
		{
			get
			{
				return (byte)this[SpamRuleBlobSchema.PublishingStateProperty];
			}
			set
			{
				this[SpamRuleBlobSchema.PublishingStateProperty] = value;
			}
		}

		public byte Priority
		{
			get
			{
				return (byte)this[SpamRuleBlobSchema.PriorityProperty];
			}
			set
			{
				this[SpamRuleBlobSchema.PriorityProperty] = value;
			}
		}

		public string RuleData
		{
			get
			{
				return this[SpamRuleBlobSchema.RuleDataProperty] as string;
			}
			set
			{
				this[SpamRuleBlobSchema.RuleDataProperty] = value;
			}
		}

		public string RuleMetaData
		{
			get
			{
				return this[SpamRuleBlobSchema.RuleMetaDataProperty] as string;
			}
			set
			{
				this[SpamRuleBlobSchema.RuleMetaDataProperty] = value;
			}
		}

		public string ProcessorData
		{
			get
			{
				return this[SpamRuleBlobSchema.ProcessorDataProperty] as string;
			}
			set
			{
				this[SpamRuleBlobSchema.ProcessorDataProperty] = value;
			}
		}

		public DateTime? CreatedDatetime
		{
			get
			{
				return this[SpamRuleBlobSchema.CreatedDatetimeProperty] as DateTime?;
			}
			set
			{
				this[SpamRuleBlobSchema.CreatedDatetimeProperty] = value;
			}
		}

		public DateTime? ChangeDatetime
		{
			get
			{
				return this[SpamRuleBlobSchema.ChangedDatetimeProperty] as DateTime?;
			}
			set
			{
				this[SpamRuleBlobSchema.ChangedDatetimeProperty] = value;
			}
		}

		public DateTime? DeletedDatetime
		{
			get
			{
				return this[SpamRuleBlobSchema.DeletedDatetimeProperty] as DateTime?;
			}
			set
			{
				this[SpamRuleBlobSchema.DeletedDatetimeProperty] = value;
			}
		}

		public override Type GetSchemaType()
		{
			return typeof(SpamRuleBlobSchema);
		}
	}
}
