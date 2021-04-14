using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Transport.Internal.MExRuntime
{
	internal sealed class MExAsyncResult : IAsyncResult
	{
		internal MExAsyncResult(AsyncCallback completionCallback, object state)
		{
			this.asyncState = state;
			this.asyncWaitHandle = null;
			this.completedSynchronously = true;
			this.isCompleted = false;
			this.completionCallback = completionCallback;
			this.asyncException = null;
		}

		public object AsyncState
		{
			get
			{
				return this.asyncState;
			}
		}

		public WaitHandle AsyncWaitHandle
		{
			get
			{
				lock (this.syncRoot)
				{
					if (this.asyncWaitHandle == null)
					{
						this.asyncWaitHandle = new ManualResetEvent(false);
					}
				}
				return this.asyncWaitHandle;
			}
		}

		public bool CompletedSynchronously
		{
			get
			{
				return this.completedSynchronously;
			}
		}

		public bool IsCompleted
		{
			get
			{
				return this.isCompleted;
			}
		}

		internal Exception AsyncException
		{
			get
			{
				return this.asyncException;
			}
			set
			{
				this.asyncException = value;
			}
		}

		internal string FaultyAgentName
		{
			get
			{
				return this.faultyAgentName;
			}
			set
			{
				this.faultyAgentName = value;
			}
		}

		internal string EventTopic
		{
			set
			{
				this.eventTopic = value;
			}
		}

		internal static void WrapAndRethrowException(Exception e, LocalizedString message)
		{
			Exception ex = null;
			Type type = e.GetType();
			Type[] types = new Type[]
			{
				typeof(string),
				typeof(Exception)
			};
			object[] array = new object[]
			{
				message,
				e
			};
			Type[] types2 = new Type[]
			{
				typeof(LocalizedString),
				typeof(Exception)
			};
			object[] array2 = new object[]
			{
				message,
				e
			};
			do
			{
				try
				{
					bool flag = true;
					ConstructorInfo constructor = type.GetConstructor(types2);
					if (constructor == null)
					{
						flag = false;
						constructor = type.GetConstructor(types);
					}
					if (constructor != null)
					{
						ex = (Exception)Activator.CreateInstance(type, flag ? array2 : array);
					}
				}
				catch (TargetInvocationException)
				{
					ex = null;
				}
				catch (MemberAccessException)
				{
					ex = null;
				}
				catch (InvalidComObjectException)
				{
					ex = null;
				}
				catch (COMException)
				{
					ex = null;
				}
				catch (TypeLoadException)
				{
					ex = null;
				}
				if (ex == null)
				{
					type = type.BaseType;
				}
			}
			while (ex == null && type != typeof(object));
			throw ex;
		}

		internal void SetAsync()
		{
			this.completedSynchronously = false;
		}

		internal void InvokeCompleted()
		{
			lock (this.syncRoot)
			{
				this.isCompleted = true;
				if (this.asyncWaitHandle != null)
				{
					this.asyncWaitHandle.Set();
				}
			}
			if (this.completionCallback != null)
			{
				this.completionCallback(this);
			}
		}

		internal void EndInvoke()
		{
			if (this.asyncException != null)
			{
				MExAsyncResult.WrapAndRethrowException(this.asyncException, new LocalizedString(MExRuntimeStrings.AgentFault(this.faultyAgentName, this.eventTopic)));
			}
			WaitHandle waitHandle = null;
			lock (this.syncRoot)
			{
				if (!this.isCompleted)
				{
					waitHandle = this.AsyncWaitHandle;
				}
			}
			if (waitHandle != null)
			{
				waitHandle.WaitOne();
			}
			if (this.asyncException != null)
			{
				MExAsyncResult.WrapAndRethrowException(this.asyncException, new LocalizedString(MExRuntimeStrings.AgentFault(this.faultyAgentName, this.eventTopic)));
			}
		}

		private object syncRoot = new object();

		private object asyncState;

		private ManualResetEvent asyncWaitHandle;

		private bool completedSynchronously;

		private bool isCompleted;

		private AsyncCallback completionCallback;

		private Exception asyncException;

		private string faultyAgentName;

		private string eventTopic;
	}
}
