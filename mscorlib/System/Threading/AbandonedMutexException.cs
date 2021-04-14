using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Threading
{
	[ComVisible(false)]
	[__DynamicallyInvokable]
	[Serializable]
	public class AbandonedMutexException : SystemException
	{
		[__DynamicallyInvokable]
		public AbandonedMutexException() : base(Environment.GetResourceString("Threading.AbandonedMutexException"))
		{
			base.SetErrorCode(-2146233043);
		}

		[__DynamicallyInvokable]
		public AbandonedMutexException(string message) : base(message)
		{
			base.SetErrorCode(-2146233043);
		}

		[__DynamicallyInvokable]
		public AbandonedMutexException(string message, Exception inner) : base(message, inner)
		{
			base.SetErrorCode(-2146233043);
		}

		[__DynamicallyInvokable]
		public AbandonedMutexException(int location, WaitHandle handle) : base(Environment.GetResourceString("Threading.AbandonedMutexException"))
		{
			base.SetErrorCode(-2146233043);
			this.SetupException(location, handle);
		}

		[__DynamicallyInvokable]
		public AbandonedMutexException(string message, int location, WaitHandle handle) : base(message)
		{
			base.SetErrorCode(-2146233043);
			this.SetupException(location, handle);
		}

		[__DynamicallyInvokable]
		public AbandonedMutexException(string message, Exception inner, int location, WaitHandle handle) : base(message, inner)
		{
			base.SetErrorCode(-2146233043);
			this.SetupException(location, handle);
		}

		private void SetupException(int location, WaitHandle handle)
		{
			this.m_MutexIndex = location;
			if (handle != null)
			{
				this.m_Mutex = (handle as Mutex);
			}
		}

		protected AbandonedMutexException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		[__DynamicallyInvokable]
		public Mutex Mutex
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_Mutex;
			}
		}

		[__DynamicallyInvokable]
		public int MutexIndex
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_MutexIndex;
			}
		}

		private int m_MutexIndex = -1;

		private Mutex m_Mutex;
	}
}
