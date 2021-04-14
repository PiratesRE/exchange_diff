using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Win32;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.ProcessManager
{
	internal class WorkerInstance
	{
		internal WorkerInstance(bool passive, WorkerInstance.WorkerContacted workerContactedDelegate, WorkerInstance.WorkerExited workerExitedDelegate, int thrashCrashCount, Microsoft.Exchange.Diagnostics.Trace tracer)
		{
			this.diag = tracer;
			this.workerContactedDelegate = workerContactedDelegate;
			this.workerExitedDelegate = workerExitedDelegate;
			this.workerIsActive = !passive;
			this.thrashCrashCount = thrashCrashCount;
		}

		internal int Pid
		{
			get
			{
				return this.pid;
			}
		}

		internal bool IsActive
		{
			get
			{
				return this.workerIsActive;
			}
			set
			{
				this.workerIsActive = value;
			}
		}

		internal bool Exited
		{
			get
			{
				return this.exited;
			}
		}

		internal bool IsConnected
		{
			get
			{
				return this.pipeConnected;
			}
		}

		internal bool SignaledReady
		{
			get
			{
				return this.signaledReady;
			}
		}

		internal Process Process
		{
			get
			{
				return this.process;
			}
		}

		internal bool ResetRequested
		{
			get
			{
				return this.resetRequested;
			}
		}

		internal int ThrashCrashCount
		{
			get
			{
				return this.thrashCrashCount;
			}
			set
			{
				this.thrashCrashCount = value;
			}
		}

		internal DateTime StartTime
		{
			get
			{
				return this.startTime;
			}
			set
			{
				this.startTime = value;
			}
		}

		internal string StdOutText
		{
			get
			{
				if (this.stdOutStream == null)
				{
					return string.Empty;
				}
				this.stdOutStream.Seek(0L, SeekOrigin.Begin);
				StreamReader streamReader = new StreamReader(this.stdOutStream);
				return streamReader.ReadToEnd();
			}
		}

		internal string StdErrText
		{
			get
			{
				if (this.stdErrStream == null)
				{
					return string.Empty;
				}
				this.stdErrStream.Seek(0L, SeekOrigin.Begin);
				StreamReader streamReader = new StreamReader(this.stdErrStream);
				return streamReader.ReadToEnd();
			}
		}

		internal bool Start(string pathName, bool paused, bool serviceListening, SafeJobHandle jobObject)
		{
			string text = null;
			this.stopHandle = this.CreateNamedSemaphore("Global\\ExchangeStopKey-", out text);
			if (this.stopHandle == null)
			{
				this.diag.TraceError(0L, "Failed to create a named semaphore: Stop Key");
				return false;
			}
			string text2 = null;
			this.hangHandle = this.CreateNamedSemaphore("Global\\ExchangeHangKey-", out text2);
			if (this.hangHandle == null)
			{
				this.diag.TraceError(0L, "Failed to create a named semaphore: Hang Key");
				return false;
			}
			string text3 = null;
			this.resetHandle = this.CreateNamedSemaphore("Global\\ExchangeResetKey-", out text3);
			if (this.resetHandle == null)
			{
				this.diag.TraceError(0L, "Failed to create a named semaphore: Reset Key");
				return false;
			}
			string text4 = null;
			this.readyHandle = this.CreateNamedSemaphore("Global\\ExchangeReadyKey-", out text4);
			if (this.readyHandle == null)
			{
				this.diag.TraceError(0L, "Failed to create a named semaphore: Ready Key");
				return false;
			}
			SafeFileHandle handle;
			SafeFileHandle safeFileHandle;
			if (!PipeStream.TryCreatePipeHandles(out handle, out safeFileHandle, this.diag))
			{
				throw new InvalidOperationException("Pipe for service-worker communication cannot be created.");
			}
			this.controlPipeStream = new PipeStream(handle, FileAccess.Write, false);
			ProcessStartInfo processStartInfo = new ProcessStartInfo();
			processStartInfo.FileName = pathName;
			processStartInfo.CreateNoWindow = true;
			processStartInfo.UseShellExecute = false;
			processStartInfo.RedirectStandardError = true;
			processStartInfo.RedirectStandardOutput = true;
			processStartInfo.Arguments = string.Format("-pipe:{0} -stopkey:{1} -resetkey:{2} -readykey:{3} -hangkey:{4}", new object[]
			{
				safeFileHandle.DangerousGetHandle().ToInt64(),
				text,
				text3,
				text4,
				text2
			});
			if (!this.workerIsActive)
			{
				ProcessStartInfo processStartInfo2 = processStartInfo;
				processStartInfo2.Arguments += " -passive";
			}
			if (paused)
			{
				ProcessStartInfo processStartInfo3 = processStartInfo;
				processStartInfo3.Arguments += " -paused";
			}
			if (!serviceListening)
			{
				ProcessStartInfo processStartInfo4 = processStartInfo;
				processStartInfo4.Arguments += " -workerListening";
			}
			this.diag.TraceDebug<string>(0L, "Worker commandline: {0}", processStartInfo.Arguments);
			lock (this)
			{
				this.process = new Process();
				this.process.StartInfo = processStartInfo;
				this.process.EnableRaisingEvents = true;
				this.process.Exited += this.OnExited;
				this.startTime = DateTime.UtcNow;
				this.monitorResetThread = new Thread(new ThreadStart(this.MonitorResetHandle));
				this.monitorResetThread.Start();
				Thread thread = new Thread(new ThreadStart(this.WaitForProcessReady));
				thread.Start();
				this.diag.TraceDebug(0L, "About to start the worker process");
				bool flag2 = false;
				try
				{
					flag2 = this.process.Start();
					Thread.Sleep(1000);
				}
				catch (Win32Exception ex)
				{
					this.diag.TraceError<int, int>(0L, "Win32Exception while trying to start the worker process (error code={0}, native error code={1})", ex.ErrorCode, ex.NativeErrorCode);
				}
				if (!flag2)
				{
					this.diag.TraceError(0L, "Failed to start the worker process");
					return false;
				}
				this.pid = this.process.Id;
				this.diag.TraceDebug<int>(0L, "Started the worker process (pid={0})", this.pid);
				if (!jobObject.IsInvalid && !jobObject.Add(this.process))
				{
					this.diag.TraceError(0L, "AssignProcessToJobObject() failed");
					return false;
				}
				this.stdErrBuffer = new byte[4096];
				this.stdErrStream = new MemoryStream(4096);
				this.stdOutBuffer = new byte[4096];
				this.stdOutStream = new MemoryStream(4096);
				this.PendStdXxxRead(this.process.StandardError);
				this.PendStdXxxRead(this.process.StandardOutput);
				safeFileHandle.Close();
			}
			return true;
		}

		internal void Stop()
		{
			if (!this.stopping)
			{
				this.stopping = true;
				this.diag.TraceDebug<int>(0L, "Stop worker instance (pid={0})", this.Pid);
				PipeStream pipeStream = this.controlPipeStream;
				if (pipeStream != null)
				{
					try
					{
						pipeStream.Flush();
						pipeStream.Dispose();
						this.controlPipeStream = null;
					}
					catch (IOException)
					{
					}
					catch (ObjectDisposedException)
					{
					}
				}
				this.SignalStop();
			}
		}

		internal void CloseProcess(out string hasExited)
		{
			hasExited = "No value";
			lock (this)
			{
				if (this.process != null)
				{
					this.diag.TraceDebug<int>(0L, "Closing process (pid={0})", this.Pid);
					try
					{
						hasExited = this.process.HasExited.ToString();
					}
					catch (Exception ex)
					{
						hasExited = ex.ToString();
					}
					this.process.Close();
					this.process = null;
				}
			}
		}

		internal void SignalStop()
		{
			if (this.stopHandle != null)
			{
				try
				{
					this.stopHandle.Release();
					this.diag.TraceDebug<int>(0L, "Signaled stop to worker instance (pid={0})", this.Pid);
					return;
				}
				catch (SemaphoreFullException)
				{
					this.diag.TraceDebug<int>(0L, "Worker instance (pid={0}) stopHandle already signaled", this.Pid);
					return;
				}
			}
			this.diag.TraceDebug<int>(0L, "Ignored to signal stop to worker instance (pid={0}) as the stop handle is null", this.Pid);
		}

		internal void SignalHang()
		{
			Semaphore semaphore = Interlocked.Exchange<Semaphore>(ref this.hangHandle, null);
			if (semaphore != null)
			{
				semaphore.Release();
				this.diag.TraceDebug<int>(0L, "Signaled hang to worker instance (pid={0})", this.Pid);
			}
		}

		internal bool SendMessage(byte[] buffer, int offset, int count)
		{
			bool flag = false;
			PipeStream pipeStream = this.controlPipeStream;
			if (this.pipeConnected && pipeStream != null)
			{
				try
				{
					pipeStream.Write(buffer, offset, count);
					flag = true;
					this.diag.TraceDebug(0L, "Message sent to worker instance");
				}
				catch (IOException)
				{
				}
				catch (ObjectDisposedException)
				{
				}
				if (!flag)
				{
					this.pipeConnected = false;
					try
					{
						pipeStream.Flush();
						pipeStream.Close();
						this.controlPipeStream = null;
					}
					catch (IOException)
					{
					}
					catch (ObjectDisposedException)
					{
					}
				}
			}
			return flag;
		}

		private void WaitForProcessReady()
		{
			this.diag.TraceDebug<int>(0L, "Wait for process (pid={0}) to signal ready", this.pid);
			this.readyHandle.WaitOne();
			this.signaledReady = true;
			Semaphore semaphore = Interlocked.Exchange<Semaphore>(ref this.readyHandle, null);
			if (semaphore != null)
			{
				semaphore.Close();
			}
			if (this.stopping)
			{
				return;
			}
			this.diag.TracePfd<int, int>(0L, "PFD ETS {0} Process (pid={1}) signaled ready", 25111, this.pid);
			this.pipeConnected = true;
			if (this.workerContactedDelegate != null)
			{
				this.workerContactedDelegate(this);
			}
		}

		private Semaphore CreateNamedSemaphore(string prefix, out string name)
		{
			bool flag = false;
			Semaphore semaphore = null;
			name = null;
			for (int i = 0; i < 10; i++)
			{
				name = prefix + Guid.NewGuid();
				semaphore = new Semaphore(0, 1, name, ref flag);
				if (flag)
				{
					break;
				}
				if (semaphore != null)
				{
					semaphore.Close();
					semaphore = null;
				}
			}
			return semaphore;
		}

		private void CallWorkerExitedDelegate(bool resetRequested)
		{
			PipeStream pipeStream = this.controlPipeStream;
			if (pipeStream != null)
			{
				try
				{
					pipeStream.Flush();
					pipeStream.Dispose();
				}
				catch (IOException)
				{
				}
				catch (ObjectDisposedException)
				{
				}
			}
			this.workerExitedDelegate(this, resetRequested);
		}

		private void OnExited(object sender, EventArgs e)
		{
			this.diag.TraceDebug<int>(0L, "Process {0} exited", this.pid);
			this.exited = true;
			this.stopping = true;
			this.CallWorkerExitedDelegate(false);
			Semaphore semaphore = Interlocked.Exchange<Semaphore>(ref this.readyHandle, null);
			if (semaphore != null)
			{
				semaphore.Release();
			}
			semaphore = Interlocked.Exchange<Semaphore>(ref this.resetHandle, null);
			if (semaphore != null)
			{
				semaphore.Release();
			}
		}

		private void MonitorResetHandle()
		{
			this.resetHandle.WaitOne();
			if (this.stopping)
			{
				return;
			}
			this.diag.TraceDebug<int>(0L, "Process {0} requested reset", this.pid);
			this.resetRequested = true;
			this.stopping = true;
			Semaphore semaphore = Interlocked.Exchange<Semaphore>(ref this.resetHandle, null);
			if (semaphore != null)
			{
				semaphore.Close();
			}
			this.CallWorkerExitedDelegate(true);
		}

		private void PendStdXxxRead(StreamReader streamReader)
		{
			try
			{
				if (streamReader == this.process.StandardError)
				{
					streamReader.BaseStream.BeginRead(this.stdErrBuffer, 0, this.stdErrBuffer.Length, new AsyncCallback(this.OnAsyncReadComplete), streamReader);
				}
				else if (streamReader == this.process.StandardOutput)
				{
					streamReader.BaseStream.BeginRead(this.stdOutBuffer, 0, this.stdOutBuffer.Length, new AsyncCallback(this.OnAsyncReadComplete), streamReader);
				}
			}
			catch (IOException)
			{
				this.diag.TraceDebug(0L, "PendStdXxxRead failed: System.IO.IOException");
			}
			catch (InvalidOperationException)
			{
				this.diag.TraceDebug(0L, "PendStdXxxRead failed: InvalidOperationException");
			}
			catch (OperationCanceledException)
			{
				this.diag.TraceDebug(0L, "PendStdXxxRead failed: OperationCanceledException");
			}
		}

		private void OnAsyncReadComplete(IAsyncResult res)
		{
			int num = 0;
			bool flag = false;
			StreamReader streamReader = res.AsyncState as StreamReader;
			try
			{
				num = streamReader.BaseStream.EndRead(res);
			}
			catch (IOException)
			{
				flag = true;
			}
			catch (InvalidOperationException)
			{
				flag = true;
			}
			catch (OperationCanceledException)
			{
				flag = true;
			}
			if (flag || num == 0)
			{
				return;
			}
			lock (this)
			{
				if (this.process != null)
				{
					bool flag3 = false;
					bool flag4 = false;
					try
					{
						flag3 = (streamReader == this.process.StandardError);
						flag4 = (streamReader == this.process.StandardOutput);
					}
					catch (InvalidOperationException)
					{
						return;
					}
					if (flag3)
					{
						if (this.stdErrStream.Position > (long)(16 * this.stdErrBuffer.Length))
						{
							this.stdErrStream.Seek(0L, SeekOrigin.Begin);
						}
						this.stdErrStream.Write(this.stdErrBuffer, 0, num);
						this.stdErrStream.SetLength((long)num);
						string stdErrText = this.StdErrText;
						this.diag.TraceDebug<string>(0L, "StdErr: {0}", stdErrText);
						this.stdErrStream.Seek(0L, SeekOrigin.Begin);
						this.PendStdXxxRead(streamReader);
					}
					else if (flag4)
					{
						if (this.stdOutStream.Position > (long)(16 * this.stdOutBuffer.Length))
						{
							this.stdOutStream.Seek(0L, SeekOrigin.Begin);
						}
						this.stdOutStream.Write(this.stdOutBuffer, 0, num);
						this.stdOutStream.SetLength((long)num);
						string stdOutText = this.StdOutText;
						this.diag.TraceDebug<string>(0L, "StdOut: {0}", stdOutText);
						this.stdOutStream.Seek(0L, SeekOrigin.Begin);
						this.PendStdXxxRead(streamReader);
					}
				}
			}
		}

		private const long TraceId = 0L;

		private const int MaxConnectAttempts = 5;

		private const int ConnectAttemptInterval = 1000;

		private Process process;

		private int pid;

		private DateTime startTime;

		private int thrashCrashCount;

		private Thread monitorResetThread;

		private Semaphore stopHandle;

		private Semaphore hangHandle;

		private Semaphore resetHandle;

		private Semaphore readyHandle;

		private PipeStream controlPipeStream;

		private bool workerIsActive;

		private bool exited;

		private bool resetRequested;

		private bool signaledReady;

		private bool pipeConnected;

		private MemoryStream stdErrStream;

		private MemoryStream stdOutStream;

		private byte[] stdErrBuffer;

		private byte[] stdOutBuffer;

		private Microsoft.Exchange.Diagnostics.Trace diag;

		private bool stopping;

		private WorkerInstance.WorkerContacted workerContactedDelegate;

		private WorkerInstance.WorkerExited workerExitedDelegate;

		internal delegate void WorkerContacted(WorkerInstance workerInstance);

		internal delegate void WorkerExited(WorkerInstance workerInstance, bool resetRequested);
	}
}
