using System;
using System.Runtime.Remoting.Messaging;
using System.Security;

namespace System.Runtime.Remoting.Contexts
{
	internal class SynchronizedServerContextSink : InternalSink, IMessageSink
	{
		[SecurityCritical]
		internal SynchronizedServerContextSink(SynchronizationAttribute prop, IMessageSink nextSink)
		{
			this._property = prop;
			this._nextSink = nextSink;
		}

		[SecuritySafeCritical]
		~SynchronizedServerContextSink()
		{
			this._property.Dispose();
		}

		[SecurityCritical]
		public virtual IMessage SyncProcessMessage(IMessage reqMsg)
		{
			WorkItem workItem = new WorkItem(reqMsg, this._nextSink, null);
			this._property.HandleWorkRequest(workItem);
			return workItem.ReplyMessage;
		}

		[SecurityCritical]
		public virtual IMessageCtrl AsyncProcessMessage(IMessage reqMsg, IMessageSink replySink)
		{
			WorkItem workItem = new WorkItem(reqMsg, this._nextSink, replySink);
			workItem.SetAsync();
			this._property.HandleWorkRequest(workItem);
			return null;
		}

		public IMessageSink NextSink
		{
			[SecurityCritical]
			get
			{
				return this._nextSink;
			}
		}

		internal IMessageSink _nextSink;

		[SecurityCritical]
		internal SynchronizationAttribute _property;
	}
}
