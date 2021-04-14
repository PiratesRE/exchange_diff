using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.AsyncQueue
{
	internal class AsyncQueueLogBatch : ConfigurablePropertyBag
	{
		public AsyncQueueLogBatch(Guid tenantId)
		{
			this.OrganizationalUnitRoot = tenantId;
			this.Logs = new MultiValuedProperty<AsyncQueueLog>();
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.OrganizationalUnitRoot.ToString());
			}
		}

		public Guid OrganizationalUnitRoot
		{
			get
			{
				return (Guid)this[AsyncQueueLogBatchSchema.OrganizationalUnitRootProperty];
			}
			set
			{
				this[AsyncQueueLogBatchSchema.OrganizationalUnitRootProperty] = value;
			}
		}

		public object PersistentStoreCopyId
		{
			get
			{
				return this[AsyncQueueLogBatchSchema.FssCopyIdProp];
			}
			set
			{
				this[AsyncQueueLogBatchSchema.FssCopyIdProp] = value;
			}
		}

		public MultiValuedProperty<AsyncQueueLog> Logs
		{
			get
			{
				return (MultiValuedProperty<AsyncQueueLog>)this[AsyncQueueLogBatchSchema.LogProperty];
			}
			set
			{
				this[AsyncQueueLogBatchSchema.LogProperty] = value;
			}
		}

		public void Add(AsyncQueueLog log)
		{
			if (log == null)
			{
				throw new ArgumentNullException("log object is NULL");
			}
			this.Logs.Add(log);
		}

		public override Type GetSchemaType()
		{
			return typeof(AsyncQueueLogBatchSchema);
		}
	}
}
