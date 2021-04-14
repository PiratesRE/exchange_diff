using System;
using System.Threading;

namespace Microsoft.Exchange.LogUploader
{
	internal class CancellationContext
	{
		public CancellationContext(CancellationToken stopToken, ManualResetEvent stopWaitHandle)
		{
			this.stopToken = stopToken;
			this.stopWaitHandle = stopWaitHandle;
		}

		public CancellationToken StopToken
		{
			get
			{
				return this.stopToken;
			}
		}

		public ManualResetEvent StopWaitHandle
		{
			get
			{
				return this.stopWaitHandle;
			}
		}

		private CancellationToken stopToken;

		private ManualResetEvent stopWaitHandle;
	}
}
