using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Microsoft.Isam.Esent.Interop
{
	internal sealed class JetCallbackWrapper
	{
		static JetCallbackWrapper()
		{
			RuntimeHelpers.PrepareMethod(typeof(StatusCallbackWrapper).GetMethod("CallbackImpl", BindingFlags.Instance | BindingFlags.NonPublic).MethodHandle);
		}

		public JetCallbackWrapper(JET_CALLBACK callback)
		{
			this.wrappedCallback = new WeakReference(callback);
			this.nativeCallback = new NATIVE_CALLBACK(this.CallbackImpl);
		}

		public bool IsAlive
		{
			get
			{
				return this.wrappedCallback.IsAlive;
			}
		}

		public NATIVE_CALLBACK NativeCallback
		{
			get
			{
				return this.nativeCallback;
			}
		}

		public bool IsWrapping(JET_CALLBACK callback)
		{
			return callback.Equals(this.wrappedCallback.Target);
		}

		private JET_err CallbackImpl(IntPtr nativeSesid, uint nativeDbid, IntPtr nativeTableid, uint nativeCbtyp, IntPtr arg1, IntPtr arg2, IntPtr nativeContext, IntPtr unused)
		{
			RuntimeHelpers.PrepareConstrainedRegions();
			JET_err result;
			try
			{
				JET_SESID sesid = new JET_SESID
				{
					Value = nativeSesid
				};
				JET_DBID dbid = new JET_DBID
				{
					Value = nativeDbid
				};
				JET_TABLEID tableid = new JET_TABLEID
				{
					Value = nativeTableid
				};
				JET_CALLBACK jet_CALLBACK = (JET_CALLBACK)this.wrappedCallback.Target;
				result = jet_CALLBACK(sesid, dbid, tableid, (JET_cbtyp)nativeCbtyp, null, null, nativeContext, IntPtr.Zero);
			}
			catch (Exception)
			{
				result = JET_err.CallbackFailed;
			}
			return result;
		}

		private static readonly TraceSwitch TraceSwitch = new TraceSwitch("ESENT JetCallbackWrapper", "Wrapper around unmanaged ESENT callback");

		private readonly WeakReference wrappedCallback;

		private readonly NATIVE_CALLBACK nativeCallback;
	}
}
