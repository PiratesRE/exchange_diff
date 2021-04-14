using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PopImap.Core
{
	internal class StringResponseItem : IResponseItem, IDisposeTrackable, IDisposable
	{
		public StringResponseItem() : this(null, null)
		{
		}

		public StringResponseItem(string s) : this(s, null)
		{
		}

		public StringResponseItem(string s, BaseSession.SendCompleteDelegate sendCompleteDelegate)
		{
			this.disposeTracker = this.GetDisposeTracker();
			this.stringResponse = s;
			this.sendCompleteDelegate = sendCompleteDelegate;
		}

		public BaseSession.SendCompleteDelegate SendCompleteDelegate
		{
			get
			{
				return this.sendCompleteDelegate;
			}
		}

		public string StringResponse
		{
			get
			{
				return this.stringResponse;
			}
		}

		public void BindData(string s, BaseSession.SendCompleteDelegate sendCompleteDelegate)
		{
			this.stringResponse = s;
			this.sendCompleteDelegate = sendCompleteDelegate;
		}

		public int GetNextChunk(BaseSession session, out byte[] buffer, out int offset)
		{
			if (!session.StringResponseItemProcessor.DataBound)
			{
				lock (session.StringResponseItemProcessor)
				{
					if (!session.StringResponseItemProcessor.DataBound)
					{
						this.processor = session.StringResponseItemProcessor;
						session.StringResponseItemProcessor.BindData(this);
					}
				}
			}
			int num = 0;
			bool flag2 = false;
			try
			{
				num = session.StringResponseItemProcessor.GetNextChunk(this, session, out buffer, out offset);
				flag2 = true;
			}
			finally
			{
				if (num == 0 || !flag2)
				{
					session.StringResponseItemProcessor.UnbindData();
					this.processor = null;
				}
			}
			return num;
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<StringResponseItem>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
				this.disposeTracker = null;
			}
			if (this.processor != null)
			{
				this.processor.UnbindData();
				this.processor = null;
			}
		}

		private string stringResponse;

		private BaseSession.SendCompleteDelegate sendCompleteDelegate;

		private DisposeTracker disposeTracker;

		private StringResponseItemProcessor processor;
	}
}
