using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics.Components.ContentFilter;

namespace Microsoft.Exchange.Data.Transport.Interop
{
	[ComVisible(false)]
	internal sealed class ComProxy : IDisposable
	{
		public ComProxy(Guid guidClass)
		{
			this.comObject = ComProxy.CreateComObject(guidClass);
			if (this.comObject == null)
			{
				throw new InvalidOperationException("CreateComObject() returned a null proxy.");
			}
		}

		~ComProxy()
		{
			ExTraceGlobals.ComInteropTracer.TraceDebug(0L, "Finalizer called on ComProxy object that wasn't disposed");
			this.Dispose();
		}

		private static IComInvoke CreateComObject(Guid guidClass)
		{
			Guid riid = new Guid("786E6730-5D95-3D9D-951B-5C9ABD1E158D");
			return (IComInvoke)UnsafeNativeMethods.CoCreateInstance(guidClass, null, 4U, riid);
		}

		private static void Release(IComInvoke comObject)
		{
			Marshal.ReleaseComObject(comObject);
		}

		public void Invoke(ComProxy.AsyncCompletionCallback asyncComplete, ComArguments propertyBag, MailItem mailItem)
		{
			if (this.comObject == null)
			{
				throw new InvalidOperationException("comObject cannot be null when invoking ComProxy object");
			}
			if (this.disposed)
			{
				throw new ObjectDisposedException("ComProxy");
			}
			InvocationContext callback = new InvocationContext(asyncComplete, propertyBag, mailItem);
			this.comObject.ComAsyncInvoke(callback);
		}

		public void Dispose()
		{
			if (!this.disposed)
			{
				if (this.comObject != null)
				{
					ComProxy.Release(this.comObject);
					this.comObject = null;
				}
				this.disposed = true;
				GC.SuppressFinalize(this);
			}
		}

		private bool disposed;

		private IComInvoke comObject;

		[ComVisible(false)]
		public delegate void AsyncCompletionCallback(ComArguments propertyBag);
	}
}
