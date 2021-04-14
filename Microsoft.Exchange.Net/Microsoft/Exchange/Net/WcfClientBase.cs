using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Common;

namespace Microsoft.Exchange.Net
{
	public abstract class WcfClientBase<T> : ClientBase<T>, IDisposeTrackable, IDisposable where T : class
	{
		public WcfClientBase()
		{
			this.disposeTracker = this.GetDisposeTracker();
		}

		public WcfClientBase(string endpointConfigurationName) : base(endpointConfigurationName)
		{
			this.disposeTracker = this.GetDisposeTracker();
		}

		public WcfClientBase(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
		{
			this.disposeTracker = this.GetDisposeTracker();
		}

		public bool IsDisposed
		{
			get
			{
				return this.disposed;
			}
		}

		public DisposeTracker DisposeTracker
		{
			get
			{
				return this.disposeTracker;
			}
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void ForceLeakReport()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.AddExtraDataWithStackTrace("Force leak was called");
			}
			this.disposeTracker = null;
		}

		public virtual void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public DisposeTracker GetDisposeTracker()
		{
			return this.InternalGetDisposeTracker();
		}

		protected void CheckDisposed()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		protected void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing && this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
					this.disposeTracker = null;
				}
				this.InternalDispose(disposing);
				this.disposed = true;
			}
		}

		protected virtual void InternalDispose(bool disposing)
		{
			WcfUtils.DisposeWcfClientGracefully(this, true);
			try
			{
				typeof(ClientBase<T>).GetInterfaceMap(typeof(IDisposable)).TargetMethods[0].Invoke(this, null);
			}
			catch (Exception arg)
			{
				ExTraceGlobals.CommonTracer.TraceInformation<Exception>(0, (long)this.GetHashCode(), "WcfClientBase.Dispose: base.Dispose() failed with: {0}", arg);
			}
		}

		protected virtual DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<WcfClientBase<T>>(this);
		}

		protected virtual void CallService(Action serviceCall, string context)
		{
			try
			{
				serviceCall();
			}
			catch (TimeoutException ex)
			{
				throw new TimeoutErrorTransientException(context, WcfUtils.FullExceptionMessage(ex), ex);
			}
			catch (EndpointNotFoundException ex2)
			{
				throw new EndpointNotFoundTransientException(context, WcfUtils.FullExceptionMessage(ex2), ex2);
			}
			catch (CommunicationException ex3)
			{
				if (ex3 is FaultException)
				{
					FaultException ex4 = (FaultException)ex3;
					if (ex4.Code != null && ex4.Code.SubCode != null && ex4.Code.IsSenderFault && ex4.Code.SubCode.Name == "DeserializationFailed")
					{
						throw new CommunicationErrorPermanentException(context, WcfUtils.FullExceptionMessage(ex3), ex3);
					}
				}
				throw new CommunicationErrorTransientException(context, WcfUtils.FullExceptionMessage(ex3), ex3);
			}
			catch (QuotaExceededException ex5)
			{
				throw new QuotaExceededPermanentException(context, WcfUtils.FullExceptionMessage(ex5), ex5);
			}
			catch (InvalidOperationException ex6)
			{
				throw new InvalidOperationTransientException(context, WcfUtils.FullExceptionMessage(ex6), ex6);
			}
			catch (InvalidDataException ex7)
			{
				throw new InvalidDataTransientException(context, WcfUtils.FullExceptionMessage(ex7), ex7);
			}
		}

		private bool disposed;

		private DisposeTracker disposeTracker;
	}
}
