using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Auditing
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CompletedAsyncResult : IAsyncResult
	{
		public object AsyncState
		{
			get
			{
				return null;
			}
		}

		public WaitHandle AsyncWaitHandle
		{
			get
			{
				if (this.waitHandle == null)
				{
					this.waitHandle = new ManualResetEvent(true);
				}
				return this.waitHandle;
			}
		}

		public bool CompletedSynchronously
		{
			get
			{
				return true;
			}
		}

		public bool IsCompleted
		{
			get
			{
				return true;
			}
		}

		private WaitHandle waitHandle;
	}
}
