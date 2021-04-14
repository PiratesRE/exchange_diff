using System;
using System.ComponentModel;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class MobileRecoAsyncCompletedArgs : AsyncCompletedEventArgs
	{
		public string Result { get; private set; }

		public int ResultCount { get; private set; }

		public TimeSpan RequestElapsedTime { get; set; }

		public MobileRecoAsyncCompletedArgs(string result, int resultCount, Exception error) : base(error, false, null)
		{
			this.Result = result;
			this.ResultCount = resultCount;
			this.RequestElapsedTime = TimeSpan.Zero;
		}
	}
}
