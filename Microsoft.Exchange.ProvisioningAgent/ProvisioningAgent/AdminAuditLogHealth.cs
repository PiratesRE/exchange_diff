using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.ProvisioningAgent
{
	public class AdminAuditLogHealth
	{
		public ExceptionDetails[] Exceptions
		{
			get
			{
				ExceptionDetails[] result;
				lock (this.syncRoot)
				{
					ExceptionDetails[] array = new ExceptionDetails[this.exceptions.Count];
					int num = 0;
					foreach (Exception exception in this.exceptions)
					{
						array[num++] = ExceptionDetails.FromException(exception);
					}
					result = array;
				}
				return result;
			}
			set
			{
				throw new NotSupportedException("AdminAuditLogHealth.Exceptions is read-only");
			}
		}

		public void AddException(Exception exception)
		{
			lock (this.syncRoot)
			{
				if (this.exceptions.Count >= 25)
				{
					this.exceptions.Dequeue();
				}
				this.exceptions.Enqueue(exception);
			}
		}

		public void Clear()
		{
			lock (this.syncRoot)
			{
				this.exceptions.Clear();
			}
		}

		internal const int MaxExceptions = 25;

		private readonly object syncRoot = new object();

		private readonly Queue<Exception> exceptions = new Queue<Exception>();
	}
}
