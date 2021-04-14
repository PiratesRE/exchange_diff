using System;
using System.Security;

namespace System.Threading
{
	public class HostExecutionContext : IDisposable
	{
		protected internal object State
		{
			get
			{
				return this.state;
			}
			set
			{
				this.state = value;
			}
		}

		public HostExecutionContext()
		{
		}

		public HostExecutionContext(object state)
		{
			this.state = state;
		}

		[SecuritySafeCritical]
		public virtual HostExecutionContext CreateCopy()
		{
			object obj = this.state;
			if (this.state is IUnknownSafeHandle)
			{
				obj = ((IUnknownSafeHandle)this.state).Clone();
			}
			return new HostExecutionContext(this.state);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public virtual void Dispose(bool disposing)
		{
		}

		private object state;
	}
}
