using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Microsoft.Isam.Esent.Interop
{
	internal sealed class StatusCallbackWrapper
	{
		static StatusCallbackWrapper()
		{
			RuntimeHelpers.PrepareMethod(typeof(StatusCallbackWrapper).GetMethod("CallbackImpl", BindingFlags.Instance | BindingFlags.NonPublic).MethodHandle);
		}

		public StatusCallbackWrapper(JET_PFNSTATUS wrappedCallback)
		{
			this.wrappedCallback = wrappedCallback;
			this.nativeCallback = ((wrappedCallback != null) ? new NATIVE_PFNSTATUS(this.CallbackImpl) : null);
		}

		public NATIVE_PFNSTATUS NativeCallback
		{
			get
			{
				return this.nativeCallback;
			}
		}

		private Exception SavedException { get; set; }

		private bool ThreadWasAborted { get; set; }

		public void ThrowSavedException()
		{
			if (this.ThreadWasAborted)
			{
				Thread.CurrentThread.Abort();
			}
			if (this.SavedException != null)
			{
				throw this.SavedException;
			}
		}

		private JET_err CallbackImpl(IntPtr nativeSesid, uint nativeSnp, uint nativeSnt, IntPtr nativeData)
		{
			RuntimeHelpers.PrepareConstrainedRegions();
			JET_err result;
			try
			{
				JET_SESID sesid = new JET_SESID
				{
					Value = nativeSesid
				};
				object managedData = CallbackDataConverter.GetManagedData(nativeData, (JET_SNP)nativeSnp, (JET_SNT)nativeSnt);
				result = this.wrappedCallback(sesid, (JET_SNP)nativeSnp, (JET_SNT)nativeSnt, managedData);
			}
			catch (ThreadAbortException)
			{
				this.ThreadWasAborted = true;
				LibraryHelpers.ThreadResetAbort();
				result = JET_err.CallbackFailed;
			}
			catch (Exception savedException)
			{
				this.SavedException = savedException;
				result = JET_err.CallbackFailed;
			}
			return result;
		}

		private static readonly TraceSwitch TraceSwitch = new TraceSwitch("ESENT StatusCallbackWrapper", "Wrapper around unmanaged ESENT status callback");

		private readonly JET_PFNSTATUS wrappedCallback;

		private readonly NATIVE_PFNSTATUS nativeCallback;
	}
}
