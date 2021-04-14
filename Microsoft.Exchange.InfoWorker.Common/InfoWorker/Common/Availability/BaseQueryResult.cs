using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal abstract class BaseQueryResult
	{
		protected BaseQueryResult(LocalizedException exception)
		{
			this.exception = exception;
		}

		protected BaseQueryResult()
		{
		}

		public LocalizedException ExceptionInfo
		{
			get
			{
				return this.exception;
			}
		}

		private LocalizedException exception;
	}
}
