using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Isam.Esent.Interop.Implementation;

namespace Microsoft.Isam.Esent.Interop.Windows8
{
	public class DurableCommitCallback : EsentResource
	{
		public DurableCommitCallback(JET_INSTANCE instance, JET_PFNDURABLECOMMITCALLBACK wrappedCallback)
		{
			this.instance = instance;
			this.wrappedCallback = wrappedCallback;
			this.wrapperCallback = new NATIVE_JET_PFNDURABLECOMMITCALLBACK(this.NativeDurableCommitCallback);
			if (this.wrappedCallback != null)
			{
				RuntimeHelpers.PrepareMethod(this.wrappedCallback.Method.MethodHandle);
			}
			RuntimeHelpers.PrepareMethod(typeof(DurableCommitCallback).GetMethod("NativeDurableCommitCallback", BindingFlags.Instance | BindingFlags.NonPublic).MethodHandle);
			InstanceParameters instanceParameters = new InstanceParameters(this.instance);
			instanceParameters.SetDurableCommitCallback(this.wrapperCallback);
			base.ResourceWasAllocated();
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "DurableCommitCallback({0})", new object[]
			{
				this.instance.ToString()
			});
		}

		public void End()
		{
			base.CheckObjectIsNotDisposed();
			this.ReleaseResource();
		}

		protected override void ReleaseResource()
		{
			this.instance = JET_INSTANCE.Nil;
			this.wrappedCallback = null;
			this.wrapperCallback = null;
			base.ResourceWasReleased();
		}

		private JET_err NativeDurableCommitCallback(IntPtr instance, ref NATIVE_COMMIT_ID commitIdSeen, uint grbit)
		{
			RuntimeHelpers.PrepareConstrainedRegions();
			JET_err result;
			try
			{
				JET_INSTANCE rhs = new JET_INSTANCE
				{
					Value = instance
				};
				if (this.instance != rhs)
				{
					result = JET_err.CallbackFailed;
				}
				else
				{
					JET_COMMIT_ID pCommitIdSeen = new JET_COMMIT_ID(commitIdSeen);
					result = this.wrappedCallback(rhs, pCommitIdSeen, (DurableCommitCallbackGrbit)grbit);
				}
			}
			catch (Exception exception)
			{
				JetApi.ReportUnhandledException(exception, "Unhandled exception during NativeDurableCommitCallback");
				result = JET_err.CallbackFailed;
			}
			return result;
		}

		private static readonly TraceSwitch TraceSwitch = new TraceSwitch("ESENT DurableCommitCallback", "Wrapper around unmanaged ESENT durable commit callback");

		private JET_INSTANCE instance;

		private JET_PFNDURABLECOMMITCALLBACK wrappedCallback;

		private NATIVE_JET_PFNDURABLECOMMITCALLBACK wrapperCallback;
	}
}
