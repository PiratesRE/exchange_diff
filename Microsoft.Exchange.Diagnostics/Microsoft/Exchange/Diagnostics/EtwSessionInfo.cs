using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Diagnostics
{
	internal class EtwSessionInfo
	{
		public event Action OnTraceStateChange;

		public EtwSessionInfo()
		{
			this.callback = new DiagnosticsNativeMethods.ControlCallback(this.TraceControlCallback);
		}

		public bool TracingEnabled
		{
			get
			{
				return this.tracingEnabled;
			}
		}

		public DiagnosticsNativeMethods.CriticalTraceHandle Session
		{
			get
			{
				return this.session;
			}
		}

		public DiagnosticsNativeMethods.ControlCallback ControlCallback
		{
			get
			{
				return this.callback;
			}
		}

		private static DiagnosticsNativeMethods.EventTraceProperties CreateEventTraceProperties()
		{
			DiagnosticsNativeMethods.EventTraceProperties eventTraceProperties = default(DiagnosticsNativeMethods.EventTraceProperties);
			eventTraceProperties.etp.wnode.bufferSize = (uint)Marshal.SizeOf(eventTraceProperties);
			eventTraceProperties.etp.wnode.flags = 131072U;
			eventTraceProperties.etp.logFileNameOffset = (uint)((int)Marshal.OffsetOf(typeof(DiagnosticsNativeMethods.EventTraceProperties), "logFileName"));
			eventTraceProperties.etp.loggerNameOffset = (uint)((int)Marshal.OffsetOf(typeof(DiagnosticsNativeMethods.EventTraceProperties), "loggerName"));
			eventTraceProperties.logFileName = null;
			eventTraceProperties.loggerName = null;
			return eventTraceProperties;
		}

		private uint TraceControlCallback(int requestCode, IntPtr context, IntPtr reserved, IntPtr buffer)
		{
			if (requestCode == 4)
			{
				try
				{
					this.session = DiagnosticsNativeMethods.CriticalTraceHandle.Attach(buffer);
				}
				catch (Win32Exception)
				{
					return 0U;
				}
				if (DiagnosticsNativeMethods.ControlTrace(this.session.DangerousGetHandle(), null, ref EtwSessionInfo.properties, 0U) == 0U)
				{
					this.tracingEnabled = true;
					this.InvokeOnTraceStateChange();
					return 0U;
				}
				return 0U;
			}
			if (requestCode == 5)
			{
				this.tracingEnabled = false;
				if (this.session != null)
				{
					this.session.Dispose();
				}
				this.InvokeOnTraceStateChange();
			}
			return 0U;
		}

		private void InvokeOnTraceStateChange()
		{
			Action onTraceStateChange = this.OnTraceStateChange;
			if (onTraceStateChange != null)
			{
				onTraceStateChange();
			}
		}

		private static DiagnosticsNativeMethods.EventTraceProperties properties = EtwSessionInfo.CreateEventTraceProperties();

		private DiagnosticsNativeMethods.CriticalTraceHandle session;

		private bool tracingEnabled;

		private DiagnosticsNativeMethods.ControlCallback callback;
	}
}
