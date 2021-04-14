using System;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Contexts;
using System.Security;
using System.Threading;

namespace System.Runtime.Remoting.Messaging
{
	internal class ClientContextTerminatorSink : InternalSink, IMessageSink
	{
		internal static IMessageSink MessageSink
		{
			get
			{
				if (ClientContextTerminatorSink.messageSink == null)
				{
					ClientContextTerminatorSink clientContextTerminatorSink = new ClientContextTerminatorSink();
					object obj = ClientContextTerminatorSink.staticSyncObject;
					lock (obj)
					{
						if (ClientContextTerminatorSink.messageSink == null)
						{
							ClientContextTerminatorSink.messageSink = clientContextTerminatorSink;
						}
					}
				}
				return ClientContextTerminatorSink.messageSink;
			}
		}

		[SecurityCritical]
		internal static object SyncProcessMessageCallback(object[] args)
		{
			IMessage msg = (IMessage)args[0];
			IMessageSink messageSink = (IMessageSink)args[1];
			return messageSink.SyncProcessMessage(msg);
		}

		[SecurityCritical]
		public virtual IMessage SyncProcessMessage(IMessage reqMsg)
		{
			IMessage message = InternalSink.ValidateMessage(reqMsg);
			if (message != null)
			{
				return message;
			}
			Context currentContext = Thread.CurrentContext;
			bool flag = currentContext.NotifyDynamicSinks(reqMsg, true, true, false, true);
			IMessage message2;
			if (reqMsg is IConstructionCallMessage)
			{
				message = currentContext.NotifyActivatorProperties(reqMsg, false);
				if (message != null)
				{
					return message;
				}
				message2 = ((IConstructionCallMessage)reqMsg).Activator.Activate((IConstructionCallMessage)reqMsg);
				message = currentContext.NotifyActivatorProperties(message2, false);
				if (message != null)
				{
					return message;
				}
			}
			else
			{
				ChannelServices.NotifyProfiler(reqMsg, RemotingProfilerEvent.ClientSend);
				object[] array = new object[2];
				IMessageSink channelSink = this.GetChannelSink(reqMsg);
				array[0] = reqMsg;
				array[1] = channelSink;
				InternalCrossContextDelegate internalCrossContextDelegate = new InternalCrossContextDelegate(ClientContextTerminatorSink.SyncProcessMessageCallback);
				if (channelSink != CrossContextChannel.MessageSink)
				{
					message2 = (IMessage)Thread.CurrentThread.InternalCrossContextCallback(Context.DefaultContext, internalCrossContextDelegate, array);
				}
				else
				{
					message2 = (IMessage)internalCrossContextDelegate(array);
				}
				ChannelServices.NotifyProfiler(message2, RemotingProfilerEvent.ClientReceive);
			}
			if (flag)
			{
				currentContext.NotifyDynamicSinks(reqMsg, true, false, false, true);
			}
			return message2;
		}

		[SecurityCritical]
		internal static object AsyncProcessMessageCallback(object[] args)
		{
			IMessage msg = (IMessage)args[0];
			IMessageSink replySink = (IMessageSink)args[1];
			IMessageSink messageSink = (IMessageSink)args[2];
			return messageSink.AsyncProcessMessage(msg, replySink);
		}

		[SecurityCritical]
		public virtual IMessageCtrl AsyncProcessMessage(IMessage reqMsg, IMessageSink replySink)
		{
			IMessage message = InternalSink.ValidateMessage(reqMsg);
			IMessageCtrl result = null;
			if (message == null)
			{
				message = InternalSink.DisallowAsyncActivation(reqMsg);
			}
			if (message != null)
			{
				if (replySink != null)
				{
					replySink.SyncProcessMessage(message);
				}
			}
			else
			{
				if (RemotingServices.CORProfilerTrackRemotingAsync())
				{
					Guid guid;
					RemotingServices.CORProfilerRemotingClientSendingMessage(out guid, true);
					if (RemotingServices.CORProfilerTrackRemotingCookie())
					{
						reqMsg.Properties["CORProfilerCookie"] = guid;
					}
					if (replySink != null)
					{
						IMessageSink messageSink = new ClientAsyncReplyTerminatorSink(replySink);
						replySink = messageSink;
					}
				}
				Context currentContext = Thread.CurrentContext;
				currentContext.NotifyDynamicSinks(reqMsg, true, true, true, true);
				if (replySink != null)
				{
					replySink = new AsyncReplySink(replySink, currentContext);
				}
				object[] array = new object[3];
				InternalCrossContextDelegate internalCrossContextDelegate = new InternalCrossContextDelegate(ClientContextTerminatorSink.AsyncProcessMessageCallback);
				IMessageSink channelSink = this.GetChannelSink(reqMsg);
				array[0] = reqMsg;
				array[1] = replySink;
				array[2] = channelSink;
				if (channelSink != CrossContextChannel.MessageSink)
				{
					result = (IMessageCtrl)Thread.CurrentThread.InternalCrossContextCallback(Context.DefaultContext, internalCrossContextDelegate, array);
				}
				else
				{
					result = (IMessageCtrl)internalCrossContextDelegate(array);
				}
			}
			return result;
		}

		public IMessageSink NextSink
		{
			[SecurityCritical]
			get
			{
				return null;
			}
		}

		[SecurityCritical]
		private IMessageSink GetChannelSink(IMessage reqMsg)
		{
			Identity identity = InternalSink.GetIdentity(reqMsg);
			return identity.ChannelSink;
		}

		private static volatile ClientContextTerminatorSink messageSink;

		private static object staticSyncObject = new object();
	}
}
