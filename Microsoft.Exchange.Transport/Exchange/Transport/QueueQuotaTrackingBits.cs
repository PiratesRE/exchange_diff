using System;
using System.Threading;

namespace Microsoft.Exchange.Transport
{
	internal class QueueQuotaTrackingBits
	{
		public bool this[QueueQuotaEntity entity, QueueQuotaResources resources]
		{
			get
			{
				return this.data != null && (this.data[(int)entity] & resources) == resources;
			}
			set
			{
				if (value)
				{
					if (this.data == null)
					{
						Interlocked.CompareExchange<QueueQuotaResources[]>(ref this.data, new QueueQuotaResources[Enum.GetValues(typeof(QueueQuotaEntity)).Length], null);
					}
					QueueQuotaResources[] array = this.data;
					array[(int)entity] = (array[(int)entity] | resources);
					return;
				}
				if (this.data != null)
				{
					QueueQuotaResources[] array2 = this.data;
					array2[(int)entity] = (array2[(int)entity] & ~resources);
				}
			}
		}

		private QueueQuotaResources[] data;
	}
}
