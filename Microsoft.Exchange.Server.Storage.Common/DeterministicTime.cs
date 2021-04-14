using System;
using System.Threading;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class DeterministicTime
	{
		public DateTime UtcNow
		{
			get
			{
				long num = DateTime.UtcNow.Ticks;
				long num2;
				do
				{
					num2 = this.previousTimeTicks;
					if (num <= num2)
					{
						num = num2 + 1L;
					}
				}
				while (num2 != Interlocked.CompareExchange(ref this.previousTimeTicks, num, num2));
				return new DateTime(num, DateTimeKind.Utc);
			}
		}

		private long previousTimeTicks = DateTime.MinValue.Ticks;
	}
}
