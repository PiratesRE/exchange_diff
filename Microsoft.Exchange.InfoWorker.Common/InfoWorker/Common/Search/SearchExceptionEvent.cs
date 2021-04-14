using System;

namespace Microsoft.Exchange.InfoWorker.Common.Search
{
	internal class SearchExceptionEvent : EventArgs
	{
		internal SearchExceptionEvent(int? sourceIndex, Exception exception)
		{
			this.sourceIndex = sourceIndex;
			this.exception = exception;
		}

		internal int? SourceIndex
		{
			get
			{
				return this.sourceIndex;
			}
		}

		internal Exception Exception
		{
			get
			{
				return this.exception;
			}
		}

		private int? sourceIndex;

		private Exception exception;
	}
}
