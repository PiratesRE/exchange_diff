using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	internal class ConfigurableBatch : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(Guid.NewGuid().ToString());
			}
		}

		public BatchPropertyTable Batch
		{
			get
			{
				return (BatchPropertyTable)this[ConfigurableBatch.BatchProp];
			}
			set
			{
				this[ConfigurableBatch.BatchProp] = value;
			}
		}

		public static readonly HygienePropertyDefinition BatchProp = new HygienePropertyDefinition("batch", typeof(BatchPropertyTable));
	}
}
