using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Servicelets.AuditLogSearch
{
	internal class AuditLogSearchRetryPolicy
	{
		public AuditLogSearchRetryPolicy()
		{
			this.RetryTenants = new ConcurrentQueue<ADUser>();
		}

		public static int RetryLimit
		{
			get
			{
				return AuditLogSearchRetryPolicy.retryLimit;
			}
		}

		public ConcurrentQueue<ADUser> RetryTenants { get; private set; }

		public bool IsRetrying
		{
			get
			{
				return this.RetryIteration > 0;
			}
		}

		public int RetryIteration
		{
			get
			{
				return this.retryIteration;
			}
		}

		public void Reset()
		{
			this.retryIteration = 0;
			this.RetryTenants = new ConcurrentQueue<ADUser>();
			AuditLogSearchHealth auditLogSearchHealth = AuditLogSearchHealthHandler.GetInstance().AuditLogSearchHealth;
			auditLogSearchHealth.RetryIteration = this.RetryIteration;
			auditLogSearchHealth.NextSearchTime = new DateTime?(DateTime.UtcNow);
		}

		public void ClearRetryTenants()
		{
			this.RetryTenants = new ConcurrentQueue<ADUser>();
		}

		public int ProceedToNextIteration(int defaultDelay)
		{
			AuditLogSearchHealth auditLogSearchHealth = AuditLogSearchHealthHandler.GetInstance().AuditLogSearchHealth;
			bool flag = !this.RetryTenants.IsEmpty && this.RetryIteration < AuditLogSearchRetryPolicy.retryLimit;
			int num;
			if (flag)
			{
				num = Math.Min(AuditLogSearchRetryPolicy.retryDelays[this.RetryIteration], defaultDelay);
				this.retryIteration = this.RetryIteration + 1;
				auditLogSearchHealth.ClearRetry();
				using (IEnumerator<ADUser> enumerator = this.RetryTenants.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ADUser tenant = enumerator.Current;
						auditLogSearchHealth.AddRetryTenant(tenant);
					}
					goto IL_90;
				}
			}
			num = defaultDelay;
			this.Reset();
			IL_90:
			Random random = new Random();
			int num2 = (int)TimeSpan.FromMinutes(2.0).TotalMilliseconds;
			num += random.Next(-num2, num2);
			auditLogSearchHealth.RetryIteration = this.RetryIteration;
			auditLogSearchHealth.NextSearchTime = new DateTime?(DateTime.UtcNow + TimeSpan.FromMilliseconds((double)num));
			return num;
		}

		private static readonly int[] retryDelays = new int[]
		{
			(int)TimeSpan.FromMinutes(15.0).TotalMilliseconds,
			(int)TimeSpan.FromMinutes(90.0).TotalMilliseconds,
			(int)TimeSpan.FromMinutes(180.0).TotalMilliseconds
		};

		private static readonly int retryLimit = AuditLogSearchRetryPolicy.retryDelays.Length;

		private int retryIteration;
	}
}
