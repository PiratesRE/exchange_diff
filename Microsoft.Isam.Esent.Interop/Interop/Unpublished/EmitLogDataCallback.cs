using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Isam.Esent.Interop.Implementation;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	public class EmitLogDataCallback : EsentResource
	{
		public EmitLogDataCallback(JET_INSTANCE instance, JET_PFNEMITLOGDATA wrappedCallback, IntPtr emitContext)
		{
			this.instance = instance;
			this.wrappedCallback = wrappedCallback;
			this.wrapperCallback = new NATIVE_JET_PFNEMITLOGDATA(this.NativeEmitLogDataCallback);
			this.bytesGranule = new byte[1024];
			if (this.wrappedCallback != null)
			{
				RuntimeHelpers.PrepareMethod(this.wrappedCallback.Method.MethodHandle);
			}
			RuntimeHelpers.PrepareMethod(typeof(EmitLogDataCallback).GetMethod("NativeEmitLogDataCallback", BindingFlags.Instance | BindingFlags.NonPublic).MethodHandle);
			InstanceParameters instanceParameters = new InstanceParameters(this.instance);
			instanceParameters.SetEmitLogDataCallback(this.wrapperCallback);
			instanceParameters.EmitLogDataCallbackCtx = emitContext;
			base.ResourceWasAllocated();
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "EmitLogDataCallback({0})", new object[]
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
			base.ResourceWasReleased();
		}

		private JET_err NativeEmitLogDataCallback(IntPtr instance, ref NATIVE_EMITDATACTX emitLogDataCtx, IntPtr logData, uint logDataSize, IntPtr callbackCtx)
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
					JET_EMITDATACTX jet_EMITDATACTX = new JET_EMITDATACTX();
					jet_EMITDATACTX.SetFromNative(ref emitLogDataCtx);
					if (logDataSize > 0U)
					{
						if (this.bytesGranule.Length < (int)logDataSize)
						{
							this.bytesGranule = new byte[logDataSize];
						}
						Marshal.Copy(logData, this.bytesGranule, 0, (int)logDataSize);
					}
					result = this.wrappedCallback(rhs, jet_EMITDATACTX, this.bytesGranule, (int)logDataSize, callbackCtx);
				}
			}
			catch (Exception exception)
			{
				JetApi.ReportUnhandledException(exception, "Unhandled exception during NativeEmitLogDataCallback");
				result = JET_err.CallbackFailed;
			}
			return result;
		}

		private static readonly TraceSwitch TraceSwitch = new TraceSwitch("ESENT EmitLogDataCallback", "Wrapper around unmanaged ESENT granule callback");

		private JET_INSTANCE instance;

		private JET_PFNEMITLOGDATA wrappedCallback;

		private NATIVE_JET_PFNEMITLOGDATA wrapperCallback;

		private byte[] bytesGranule;
	}
}
