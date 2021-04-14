using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Isam.Esent.Interop.Unpublished;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class LogChecksummer : IDisposeTrackable, IDisposable
	{
		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.LogCopyTracer;
			}
		}

		public LogChecksummer(string basename)
		{
			bool flag = false;
			EseHelper.GlobalInit();
			this.m_disposeTracker = this.GetDisposeTracker();
			this.m_instanceName = string.Format("LogChecksummer {0} {1}", basename, this.GetHashCode());
			Api.JetCreateInstance(out this.m_instance, this.m_instanceName);
			this.m_basename = basename;
			bool jettermNeeded = true;
			try
			{
				InstanceParameters instanceParameters = new InstanceParameters(this.Instance);
				instanceParameters.Recovery = false;
				instanceParameters.MaxTemporaryTables = 0;
				instanceParameters.NoInformationEvent = true;
				instanceParameters.BaseName = basename;
				jettermNeeded = false;
				Api.JetInit(ref this.m_instance);
				jettermNeeded = true;
				Api.JetBeginSession(this.Instance, out this.m_sesid, null, null);
				this.AssertNotTerminated();
				flag = true;
				LogChecksummer.Tracer.TraceDebug<string, int>((long)this.GetHashCode(), "LogChecksummer created: {0}. DispTracker=0x{1:X}", this.m_instanceName, this.m_disposeTracker.GetHashCode());
			}
			finally
			{
				if (!flag)
				{
					this.Term(jettermNeeded);
				}
			}
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<LogChecksummer>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.m_disposeTracker != null)
			{
				this.m_disposeTracker.Suppress();
			}
		}

		public void Dispose()
		{
			if (!this.m_isDisposed)
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}
		}

		public void Dispose(bool disposing)
		{
			lock (this)
			{
				if (!this.m_isDisposed)
				{
					if (disposing && this.m_disposeTracker != null)
					{
						this.m_disposeTracker.Dispose();
					}
					this.Term(true);
					this.m_isDisposed = true;
				}
			}
		}

		private void Term(bool jettermNeeded)
		{
			lock (this)
			{
				if (!this.m_terminated)
				{
					LogChecksummer.Tracer.TraceDebug<string>((long)this.GetHashCode(), "LogChecksummer terminating: {0}", this.m_instanceName);
					if (jettermNeeded)
					{
						Api.JetTerm(this.Instance);
					}
					this.m_terminated = true;
				}
			}
		}

		public EsentErrorException Verify(string logfile, byte[] logInMemory)
		{
			EsentErrorException ex = null;
			lock (this)
			{
				this.AssertNotTerminated();
				try
				{
					UnpublishedApi.ChecksumLogFromMemory(this.Sesid, logfile, this.m_basename, logInMemory);
					LogChecksummer.Tracer.TraceDebug<string>((long)this.GetHashCode(), "LogChecksummer({0}) verified.", logfile);
				}
				catch (EsentLogFileCorruptException ex2)
				{
					ex = ex2;
				}
				catch (EsentErrorException ex3)
				{
					ex = ex3;
				}
				if (ex != null)
				{
					LogChecksummer.Tracer.TraceError<string, EsentErrorException>((long)this.GetHashCode(), "LogChecksummer({0}) failed:{1}", logfile, ex);
				}
			}
			return ex;
		}

		private void AssertNotTerminated()
		{
		}

		private JET_INSTANCE Instance
		{
			get
			{
				this.AssertNotTerminated();
				return this.m_instance;
			}
		}

		private JET_SESID Sesid
		{
			get
			{
				this.AssertNotTerminated();
				return this.m_sesid;
			}
		}

		private DisposeTracker m_disposeTracker;

		private bool m_isDisposed;

		private readonly JET_INSTANCE m_instance;

		private readonly JET_SESID m_sesid;

		private readonly string m_basename;

		private bool m_terminated;

		private readonly string m_instanceName;
	}
}
