using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	internal class MaxProcessorID : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(Guid.NewGuid().ToString());
			}
		}

		public long? ProcessorID
		{
			get
			{
				return (long?)this[MaxProcessorID.ProcessorIDProperty];
			}
			set
			{
				this[MaxProcessorID.ProcessorIDProperty] = value;
			}
		}

		public static readonly HygienePropertyDefinition ProcessorIDProperty = new HygienePropertyDefinition("bi_ProcessorId", typeof(long?));
	}
}
