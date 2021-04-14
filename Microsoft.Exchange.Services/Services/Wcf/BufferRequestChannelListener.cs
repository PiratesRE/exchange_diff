using System;
using System.ServiceModel.Channels;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class BufferRequestChannelListener : ChannelListenerBase<IReplyChannel>
	{
		public BufferRequestChannelListener(IChannelListener<IReplyChannel> innerListener)
		{
			this.innerListener = innerListener;
		}

		public override T GetProperty<T>()
		{
			return this.innerListener.GetProperty<T>();
		}

		protected override IReplyChannel OnAcceptChannel(TimeSpan timeout)
		{
			return this.WrapChannel(this.innerListener.AcceptChannel(timeout));
		}

		protected override IAsyncResult OnBeginAcceptChannel(TimeSpan timeout, AsyncCallback callback, object state)
		{
			return this.innerListener.BeginAcceptChannel(timeout, callback, state);
		}

		protected override IReplyChannel OnEndAcceptChannel(IAsyncResult result)
		{
			return this.WrapChannel(this.innerListener.EndAcceptChannel(result));
		}

		protected override IAsyncResult OnBeginWaitForChannel(TimeSpan timeout, AsyncCallback callback, object state)
		{
			return this.innerListener.BeginWaitForChannel(timeout, callback, state);
		}

		protected override bool OnEndWaitForChannel(IAsyncResult result)
		{
			return this.innerListener.EndWaitForChannel(result);
		}

		protected override bool OnWaitForChannel(TimeSpan timeout)
		{
			return this.innerListener.WaitForChannel(timeout);
		}

		public override Uri Uri
		{
			get
			{
				return this.innerListener.Uri;
			}
		}

		protected override void OnAbort()
		{
			this.innerListener.Abort();
		}

		protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
		{
			return this.innerListener.BeginClose(timeout, callback, state);
		}

		protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
		{
			return this.innerListener.BeginOpen(timeout, callback, state);
		}

		protected override void OnClose(TimeSpan timeout)
		{
			this.innerListener.Close(timeout);
		}

		protected override void OnEndClose(IAsyncResult result)
		{
			this.innerListener.EndClose(result);
		}

		protected override void OnEndOpen(IAsyncResult result)
		{
			this.innerListener.EndOpen(result);
		}

		protected override void OnOpen(TimeSpan timeout)
		{
			this.innerListener.Open(timeout);
		}

		private IReplyChannel WrapChannel(IReplyChannel innerChannel)
		{
			if (innerChannel == null)
			{
				return null;
			}
			return new BufferRequestChannel(this, innerChannel);
		}

		private IChannelListener<IReplyChannel> innerListener;
	}
}
