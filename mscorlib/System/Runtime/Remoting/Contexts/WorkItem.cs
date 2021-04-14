using System;
using System.Runtime.Remoting.Messaging;
using System.Security;
using System.Threading;

namespace System.Runtime.Remoting.Contexts
{
	internal class WorkItem
	{
		[SecurityCritical]
		internal WorkItem(IMessage reqMsg, IMessageSink nextSink, IMessageSink replySink)
		{
			this._reqMsg = reqMsg;
			this._replyMsg = null;
			this._nextSink = nextSink;
			this._replySink = replySink;
			this._ctx = Thread.CurrentContext;
			this._callCtx = Thread.CurrentThread.GetMutableExecutionContext().LogicalCallContext;
		}

		internal virtual void SetWaiting()
		{
			this._flags |= 1;
		}

		internal virtual bool IsWaiting()
		{
			return (this._flags & 1) == 1;
		}

		internal virtual void SetSignaled()
		{
			this._flags |= 2;
		}

		internal virtual bool IsSignaled()
		{
			return (this._flags & 2) == 2;
		}

		internal virtual void SetAsync()
		{
			this._flags |= 4;
		}

		internal virtual bool IsAsync()
		{
			return (this._flags & 4) == 4;
		}

		internal virtual void SetDummy()
		{
			this._flags |= 8;
		}

		internal virtual bool IsDummy()
		{
			return (this._flags & 8) == 8;
		}

		[SecurityCritical]
		internal static object ExecuteCallback(object[] args)
		{
			WorkItem workItem = (WorkItem)args[0];
			if (workItem.IsAsync())
			{
				workItem._nextSink.AsyncProcessMessage(workItem._reqMsg, workItem._replySink);
			}
			else if (workItem._nextSink != null)
			{
				workItem._replyMsg = workItem._nextSink.SyncProcessMessage(workItem._reqMsg);
			}
			return null;
		}

		[SecurityCritical]
		internal virtual void Execute()
		{
			Thread.CurrentThread.InternalCrossContextCallback(this._ctx, WorkItem._xctxDel, new object[]
			{
				this
			});
		}

		internal virtual IMessage ReplyMessage
		{
			get
			{
				return this._replyMsg;
			}
		}

		private const int FLG_WAITING = 1;

		private const int FLG_SIGNALED = 2;

		private const int FLG_ASYNC = 4;

		private const int FLG_DUMMY = 8;

		internal int _flags;

		internal IMessage _reqMsg;

		internal IMessageSink _nextSink;

		internal IMessageSink _replySink;

		internal IMessage _replyMsg;

		internal Context _ctx;

		[SecurityCritical]
		internal LogicalCallContext _callCtx;

		internal static InternalCrossContextDelegate _xctxDel = new InternalCrossContextDelegate(WorkItem.ExecuteCallback);
	}
}
