using System;

namespace Microsoft.Exchange.Rpc.QueueViewer
{
	internal class QueueViewerException : Exception
	{
		public QueueViewerException(int result)
		{
			this.errorCode = result;
		}

		public int ErrorCode
		{
			get
			{
				return this.errorCode;
			}
		}

		private int errorCode;
	}
}
