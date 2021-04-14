using System;
using System.Threading;

namespace System
{
	[__DynamicallyInvokable]
	public class Progress<T> : IProgress<T>
	{
		[__DynamicallyInvokable]
		public Progress()
		{
			this.m_synchronizationContext = (SynchronizationContext.CurrentNoFlow ?? ProgressStatics.DefaultContext);
			this.m_invokeHandlers = new SendOrPostCallback(this.InvokeHandlers);
		}

		[__DynamicallyInvokable]
		public Progress(Action<T> handler) : this()
		{
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			this.m_handler = handler;
		}

		[__DynamicallyInvokable]
		[method: __DynamicallyInvokable]
		public event EventHandler<T> ProgressChanged;

		[__DynamicallyInvokable]
		protected virtual void OnReport(T value)
		{
			Action<T> handler = this.m_handler;
			EventHandler<T> progressChanged = this.ProgressChanged;
			if (handler != null || progressChanged != null)
			{
				this.m_synchronizationContext.Post(this.m_invokeHandlers, value);
			}
		}

		[__DynamicallyInvokable]
		void IProgress<!0>.Report(T value)
		{
			this.OnReport(value);
		}

		private void InvokeHandlers(object state)
		{
			T t = (T)((object)state);
			Action<T> handler = this.m_handler;
			EventHandler<T> progressChanged = this.ProgressChanged;
			if (handler != null)
			{
				handler(t);
			}
			if (progressChanged != null)
			{
				progressChanged(this, t);
			}
		}

		private readonly SynchronizationContext m_synchronizationContext;

		private readonly Action<T> m_handler;

		private readonly SendOrPostCallback m_invokeHandlers;
	}
}
