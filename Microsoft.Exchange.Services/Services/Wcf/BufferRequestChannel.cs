using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;

namespace Microsoft.Exchange.Services.Wcf
{
	public class BufferRequestChannel : ChannelBase, IReplyChannel, IChannel, ICommunicationObject
	{
		protected IReplyChannel InnerChannel
		{
			get
			{
				return this.innerChannel;
			}
		}

		public BufferRequestChannel(ChannelManagerBase channelManager, IReplyChannel innerChannel) : base(channelManager)
		{
			if (innerChannel == null)
			{
				throw new ArgumentNullException("innerChannel");
			}
			this.innerChannel = innerChannel;
		}

		public IAsyncResult BeginReceiveRequest(TimeSpan timeout, AsyncCallback callback, object state)
		{
			return this.innerChannel.BeginReceiveRequest(timeout, callback, state);
		}

		public IAsyncResult BeginReceiveRequest(AsyncCallback callback, object state)
		{
			return this.BeginReceiveRequest(base.DefaultReceiveTimeout, callback, state);
		}

		public IAsyncResult BeginTryReceiveRequest(TimeSpan timeout, AsyncCallback callback, object state)
		{
			return new BufferRequestChannel.OnTryReceiveRequestAsyncResult(this, timeout, callback, state);
		}

		public bool EndTryReceiveRequest(IAsyncResult result, out RequestContext context)
		{
			return BufferRequestChannel.OnTryReceiveRequestAsyncResult.End(result, out context);
		}

		public IAsyncResult BeginWaitForRequest(TimeSpan timeout, AsyncCallback callback, object state)
		{
			return this.innerChannel.BeginWaitForRequest(timeout, callback, state);
		}

		public RequestContext EndReceiveRequest(IAsyncResult result)
		{
			return this.innerChannel.EndReceiveRequest(result);
		}

		public bool EndWaitForRequest(IAsyncResult result)
		{
			return this.innerChannel.EndWaitForRequest(result);
		}

		public RequestContext ReceiveRequest(TimeSpan timeout)
		{
			return this.innerChannel.ReceiveRequest(timeout);
		}

		public RequestContext ReceiveRequest()
		{
			return this.innerChannel.ReceiveRequest();
		}

		public bool TryReceiveRequest(TimeSpan timeout, out RequestContext context)
		{
			return this.innerChannel.TryReceiveRequest(timeout, out context);
		}

		public bool WaitForRequest(TimeSpan timeout)
		{
			return this.innerChannel.WaitForRequest(timeout);
		}

		public override T GetProperty<T>()
		{
			return this.innerChannel.GetProperty<T>();
		}

		protected override void OnAbort()
		{
			this.innerChannel.Abort();
		}

		protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
		{
			return this.innerChannel.BeginClose(timeout, callback, state);
		}

		protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
		{
			return this.innerChannel.BeginOpen(timeout, callback, state);
		}

		protected override void OnClose(TimeSpan timeout)
		{
			this.innerChannel.Close(timeout);
		}

		protected override void OnEndClose(IAsyncResult result)
		{
			this.innerChannel.EndClose(result);
		}

		protected override void OnEndOpen(IAsyncResult result)
		{
			this.innerChannel.EndOpen(result);
		}

		protected override void OnOpen(TimeSpan timeout)
		{
			this.innerChannel.Open(timeout);
		}

		public EndpointAddress LocalAddress
		{
			get
			{
				return this.innerChannel.LocalAddress;
			}
		}

		private RequestContext BufferMessageAndWrapContext(RequestContext requestContext)
		{
			BufferRequestChannel.WrappingRequestContext wrappingRequestContext = new BufferRequestChannel.WrappingRequestContext(requestContext, this);
			wrappingRequestContext.BufferMessage();
			return wrappingRequestContext;
		}

		private IReplyChannel innerChannel;

		private class OnTryReceiveRequestAsyncResult : AsyncResultBase
		{
			public OnTryReceiveRequestAsyncResult(BufferRequestChannel channel, TimeSpan timeout, AsyncCallback callback, object state) : base(callback, state)
			{
				if (channel == null)
				{
					throw new ArgumentException("channel");
				}
				this.channel = channel;
				IAsyncResult asyncResult = this.channel.InnerChannel.BeginTryReceiveRequest(timeout, BufferRequestChannel.OnTryReceiveRequestAsyncResult.onTryReceiveRequest, this);
				if (asyncResult.CompletedSynchronously)
				{
					this.tryReceiveRequestSuccess = this.channel.InnerChannel.EndTryReceiveRequest(asyncResult, out this.requestContext);
					if (this.tryReceiveRequestSuccess && this.requestContext != null)
					{
						ThreadPool.QueueUserWorkItem(BufferRequestChannel.OnTryReceiveRequestAsyncResult.enqueueBufferMessageAndWrapContext, this);
						return;
					}
					base.Complete(true);
				}
			}

			private static void OnTryReceiveRequest(IAsyncResult result)
			{
				if (result.CompletedSynchronously)
				{
					return;
				}
				BufferRequestChannel.OnTryReceiveRequestAsyncResult onTryReceiveRequestAsyncResult = (BufferRequestChannel.OnTryReceiveRequestAsyncResult)result.AsyncState;
				try
				{
					onTryReceiveRequestAsyncResult.HandleTryReceiveRequest(result);
					onTryReceiveRequestAsyncResult.Complete(false);
				}
				catch (Exception ex)
				{
					if (onTryReceiveRequestAsyncResult.requestContext != null && onTryReceiveRequestAsyncResult.requestContext.RequestMessage != null)
					{
						onTryReceiveRequestAsyncResult.requestContext.RequestMessage.Properties["WS_WcfDelayedExceptionKey"] = ex;
						onTryReceiveRequestAsyncResult.Complete(false);
					}
					else
					{
						onTryReceiveRequestAsyncResult.Complete(false, ex);
					}
				}
			}

			private void HandleTryReceiveRequest(IAsyncResult result)
			{
				this.tryReceiveRequestSuccess = this.channel.InnerChannel.EndTryReceiveRequest(result, out this.requestContext);
				if (this.tryReceiveRequestSuccess && this.requestContext != null)
				{
					this.requestContext = this.channel.BufferMessageAndWrapContext(this.requestContext);
				}
			}

			private static void EnqueueBufferMessageAndWrapContext(object state)
			{
				BufferRequestChannel.OnTryReceiveRequestAsyncResult onTryReceiveRequestAsyncResult = (BufferRequestChannel.OnTryReceiveRequestAsyncResult)state;
				bool flag = true;
				try
				{
					onTryReceiveRequestAsyncResult.requestContext = onTryReceiveRequestAsyncResult.channel.BufferMessageAndWrapContext(onTryReceiveRequestAsyncResult.requestContext);
					flag = false;
					onTryReceiveRequestAsyncResult.Complete(false);
				}
				catch (Exception ex)
				{
					if (onTryReceiveRequestAsyncResult.requestContext != null && onTryReceiveRequestAsyncResult.requestContext.RequestMessage != null)
					{
						onTryReceiveRequestAsyncResult.requestContext.RequestMessage.Properties["WS_WcfDelayedExceptionKey"] = ex;
						if (flag)
						{
							onTryReceiveRequestAsyncResult.Complete(false);
						}
					}
					else if (flag)
					{
						onTryReceiveRequestAsyncResult.Complete(false, ex);
					}
				}
			}

			public static bool End(IAsyncResult result, out RequestContext requestContext)
			{
				BufferRequestChannel.OnTryReceiveRequestAsyncResult onTryReceiveRequestAsyncResult = AsyncResultBase.End<BufferRequestChannel.OnTryReceiveRequestAsyncResult>(result);
				requestContext = onTryReceiveRequestAsyncResult.requestContext;
				return onTryReceiveRequestAsyncResult.tryReceiveRequestSuccess;
			}

			private BufferRequestChannel channel;

			private bool tryReceiveRequestSuccess;

			private RequestContext requestContext;

			private static AsyncCallback onTryReceiveRequest = new AsyncCallback(BufferRequestChannel.OnTryReceiveRequestAsyncResult.OnTryReceiveRequest);

			private static WaitCallback enqueueBufferMessageAndWrapContext = new WaitCallback(BufferRequestChannel.OnTryReceiveRequestAsyncResult.EnqueueBufferMessageAndWrapContext);
		}

		private class WrappingRequestContext : RequestContext
		{
			public WrappingRequestContext(RequestContext innerContext, BufferRequestChannel channel)
			{
				this.innerContext = innerContext;
				this.channel = channel;
				this.message = this.innerContext.RequestMessage;
			}

			internal void BufferMessage()
			{
				if (this.innerContext.RequestMessage.State == MessageState.Created)
				{
					this.message = this.innerContext.RequestMessage.CreateBufferedCopy(int.MaxValue).CreateMessage();
				}
			}

			public override void Abort()
			{
				this.innerContext.Abort();
			}

			public override IAsyncResult BeginReply(Message message, TimeSpan timeout, AsyncCallback callback, object state)
			{
				return this.innerContext.BeginReply(message, timeout, callback, state);
			}

			public override IAsyncResult BeginReply(Message message, AsyncCallback callback, object state)
			{
				return this.BeginReply(message, this.channel.DefaultSendTimeout, callback, state);
			}

			public override void Close(TimeSpan timeout)
			{
				this.innerContext.Close(timeout);
			}

			public override void Close()
			{
				this.innerContext.Close();
			}

			public override void EndReply(IAsyncResult result)
			{
				this.innerContext.EndReply(result);
			}

			public override void Reply(Message message, TimeSpan timeout)
			{
				this.innerContext.Reply(message, timeout);
			}

			public override void Reply(Message message)
			{
				this.Reply(message, this.channel.DefaultSendTimeout);
			}

			public override Message RequestMessage
			{
				get
				{
					return this.message;
				}
			}

			private RequestContext innerContext;

			private BufferRequestChannel channel;

			private Message message;
		}
	}
}
