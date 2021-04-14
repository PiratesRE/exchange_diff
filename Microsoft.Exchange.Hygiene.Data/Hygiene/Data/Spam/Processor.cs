using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	internal class Processor : ConfigurablePropertyTable
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
				return (long?)this[Processor.ProcessorIDProperty];
			}
			set
			{
				this[Processor.ProcessorIDProperty] = value;
			}
		}

		public static readonly HygienePropertyDefinition ProcessorIDProperty = new HygienePropertyDefinition("bi_ProcessorId", typeof(long?));
	}
}
