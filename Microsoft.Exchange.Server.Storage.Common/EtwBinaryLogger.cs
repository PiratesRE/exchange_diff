using System;
using System.ComponentModel;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class EtwBinaryLogger : DisposableBase, IBinaryLogger, IDisposable
	{
		private EtwBinaryLogger(string name, Guid providerGuid)
		{
			this.name = name;
			this.providerGuid = providerGuid;
		}

		public bool IsLoggingEnabled
		{
			get
			{
				return this.sessionHandle != null && !this.sessionHandle.IsInvalid;
			}
		}

		public static EtwBinaryLogger Create(string name, Guid providerGuid)
		{
			return new EtwBinaryLogger(name, providerGuid);
		}

		public void Start()
		{
			try
			{
				this.registrationHandle = DiagnosticsNativeMethods.CriticalTraceRegistrationHandle.RegisterTrace(this.providerGuid, new DiagnosticsNativeMethods.ControlCallback(this.TraceControlCallback));
			}
			catch (Win32Exception ex)
			{
				NullExecutionDiagnostics.Instance.OnExceptionCatch(ex);
				throw new StoreException((LID)46200U, ErrorCodeValue.CallFailed, "RegisterTrace failed", ex);
			}
		}

		public void Stop()
		{
			if (this.registrationHandle != null)
			{
				this.registrationHandle.Dispose();
				this.registrationHandle = null;
			}
		}

		public bool TryWrite(TraceBuffer buffer, int retries, TimeSpan timeToWait)
		{
			for (int i = 0; i < retries; i++)
			{
				if (this.TryWrite(buffer))
				{
					return true;
				}
				Thread.Sleep(timeToWait);
			}
			return false;
		}

		public bool TryWrite(TraceBuffer buffer)
		{
			return this.TryWrite(buffer.RecordGuid, buffer.Data, buffer.Length);
		}

		internal bool TryWrite(Guid recordGuid, byte[] buffer, int retries, TimeSpan timeToWait)
		{
			for (int i = 0; i < retries; i++)
			{
				if (this.TryWrite(recordGuid, buffer, buffer.Length))
				{
					return true;
				}
				Thread.Sleep(timeToWait);
			}
			return false;
		}

		internal unsafe bool TryWrite(Guid recordGuid, byte[] buffer, int bytesToWrite)
		{
			if (buffer.Length < bytesToWrite)
			{
				return false;
			}
			if (!this.IsLoggingEnabled)
			{
				return false;
			}
			fixed (byte* ptr = buffer)
			{
				int num = 0;
				while (bytesToWrite > 0)
				{
					int num2 = Math.Min(bytesToWrite, 8064);
					uint num3 = DiagnosticsNativeMethods.TraceMessage(this.sessionHandle.DangerousGetHandle(), 43U, ref recordGuid, 0, ptr + num, num2, IntPtr.Zero, 0);
					bool result;
					if (num3 == 8U)
					{
						DiagnosticContext.TraceLocation((LID)49624U);
						result = false;
					}
					else if (num3 == 14U)
					{
						DiagnosticContext.TraceLocation((LID)52952U);
						result = false;
					}
					else if (num3 == 6U)
					{
						DiagnosticContext.TraceLocation((LID)47228U);
						result = false;
					}
					else
					{
						if (num3 == 0U)
						{
							num += num2;
							bytesToWrite -= num2;
							continue;
						}
						Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_TraceMessageFailed, new object[]
						{
							recordGuid,
							num3,
							buffer.Length,
							bytesToWrite,
							num2,
							num
						});
						result = false;
					}
					return result;
				}
			}
			return true;
		}

		private uint TraceControlCallback(int requestCode, IntPtr context, IntPtr reserved, IntPtr buffer)
		{
			DiagnosticsNativeMethods.CriticalTraceHandle criticalTraceHandle = null;
			int currentProcessId = DiagnosticsNativeMethods.GetCurrentProcessId();
			try
			{
				if (requestCode == 4)
				{
					try
					{
						criticalTraceHandle = DiagnosticsNativeMethods.CriticalTraceHandle.Attach(buffer);
					}
					catch (Win32Exception ex)
					{
						NullExecutionDiagnostics.Instance.OnExceptionCatch(ex);
						Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_TraceLoggerFailed, new object[]
						{
							this.name,
							this.providerGuid,
							currentProcessId,
							ex
						});
						return 0U;
					}
					DiagnosticsNativeMethods.CriticalTraceHandle criticalTraceHandle2 = this.sessionHandle;
					this.sessionHandle = criticalTraceHandle;
					criticalTraceHandle = criticalTraceHandle2;
					Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_TraceLoggerStarted, new object[]
					{
						this.name,
						this.providerGuid,
						currentProcessId
					});
				}
				else if (requestCode == 5)
				{
					criticalTraceHandle = this.sessionHandle;
					this.sessionHandle = null;
					Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_TraceLoggerStopped, new object[]
					{
						this.name,
						this.providerGuid,
						currentProcessId
					});
				}
			}
			finally
			{
				if (criticalTraceHandle != null)
				{
					criticalTraceHandle.Dispose();
					criticalTraceHandle = null;
				}
			}
			return 0U;
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				this.Stop();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<EtwBinaryLogger>(this);
		}

		internal const int TraceMessageMaxSize = 8064;

		private readonly string name;

		private readonly Guid providerGuid;

		private DiagnosticsNativeMethods.CriticalTraceHandle sessionHandle;

		private DiagnosticsNativeMethods.CriticalTraceRegistrationHandle registrationHandle;
	}
}
