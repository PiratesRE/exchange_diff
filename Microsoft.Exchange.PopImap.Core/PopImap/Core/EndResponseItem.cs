using System;

namespace Microsoft.Exchange.PopImap.Core
{
	internal class EndResponseItem : IResponseItem
	{
		public EndResponseItem(BaseSession.SendCompleteDelegate sendCompleteDelegate)
		{
			this.endResponseDelegate = sendCompleteDelegate;
		}

		public BaseSession.SendCompleteDelegate SendCompleteDelegate
		{
			get
			{
				return this.endResponseDelegate;
			}
		}

		public int GetNextChunk(BaseSession session, out byte[] buffer, out int offset)
		{
			buffer = null;
			offset = 0;
			return 0;
		}

		private BaseSession.SendCompleteDelegate endResponseDelegate;
	}
}
