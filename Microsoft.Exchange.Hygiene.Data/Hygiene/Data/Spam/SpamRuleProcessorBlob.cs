using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	internal class SpamRuleProcessorBlob : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(string.Format("{0}", this.ProcessorId));
			}
		}

		public Guid Id
		{
			get
			{
				return (Guid)this[SpamRuleProcessorBlobSchema.IdProperty];
			}
			set
			{
				this[SpamRuleProcessorBlobSchema.IdProperty] = value;
			}
		}

		public string ProcessorId
		{
			get
			{
				return this[SpamRuleProcessorBlobSchema.ProcessorIdProperty] as string;
			}
			set
			{
				this[SpamRuleProcessorBlobSchema.ProcessorIdProperty] = value;
			}
		}

		public string Data
		{
			get
			{
				return this[SpamRuleProcessorBlobSchema.DataProperty] as string;
			}
			set
			{
				this[SpamRuleProcessorBlobSchema.DataProperty] = value;
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
			return typeof(SpamRuleProcessorBlobSchema);
		}
	}
}
