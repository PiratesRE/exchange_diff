using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Hygiene.Data.AsyncQueue
{
	internal class AsyncQueueLogProperty : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return new ADObjectId(this.identity);
			}
		}

		public DateTime LogTime
		{
			get
			{
				return (DateTime)this[AsyncQueueLogPropertySchema.LogTimeProperty];
			}
			set
			{
				this[AsyncQueueLogPropertySchema.LogTimeProperty] = value;
			}
		}

		public string LogType
		{
			get
			{
				return (string)this[AsyncQueueLogPropertySchema.LogTypeProperty];
			}
			set
			{
				this[AsyncQueueLogPropertySchema.LogTypeProperty] = value;
			}
		}

		public int LogIndex
		{
			get
			{
				return (int)this[AsyncQueueLogPropertySchema.LogIndexProperty];
			}
			set
			{
				this[AsyncQueueLogPropertySchema.LogIndexProperty] = value;
			}
		}

		public string LogData
		{
			get
			{
				return (string)this[AsyncQueueLogPropertySchema.LogDataProperty];
			}
			set
			{
				this[AsyncQueueLogPropertySchema.LogDataProperty] = value;
			}
		}

		public override Type GetSchemaType()
		{
			return typeof(AsyncQueueLogPropertySchema);
		}

		private readonly Guid identity = CombGuidGenerator.NewGuid();
	}
}
