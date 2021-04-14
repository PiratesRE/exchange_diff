using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Assistants
{
	internal abstract class AITransientException : AIException
	{
		protected AITransientException(LocalizedString explain, Exception innerException, RetrySchedule retrySchedule) : base(explain, innerException)
		{
			this.retrySchedule = (retrySchedule ?? AITransientException.genericSchedule);
		}

		protected AITransientException(Exception innerException) : this(Strings.descTransientException, innerException, null)
		{
		}

		protected AITransientException(LocalizedString explain, Exception innerException) : this(explain, innerException, null)
		{
		}

		protected AITransientException(LocalizedString explain) : this(explain, null, null)
		{
		}

		public RetrySchedule RetrySchedule
		{
			get
			{
				return this.retrySchedule;
			}
			set
			{
				this.retrySchedule = value;
			}
		}

		private static RetrySchedule genericSchedule = new RetrySchedule(FinalAction.Skip, TimeSpan.FromDays(1.0), new TimeSpan[]
		{
			TimeSpan.Zero,
			TimeSpan.FromSeconds(5.0),
			TimeSpan.FromMinutes(1.0),
			TimeSpan.FromMinutes(5.0),
			TimeSpan.FromMinutes(15.0),
			TimeSpan.FromHours(1.0)
		});

		private RetrySchedule retrySchedule;
	}
}
