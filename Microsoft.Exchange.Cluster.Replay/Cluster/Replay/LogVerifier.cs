using System;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Isam.Esent.Interop.Unpublished;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class LogVerifier
	{
		private LogVerifier(string basename, bool ignored)
		{
			bool flag = false;
			EseHelper.GlobalInit();
			this.m_instanceName = string.Concat(new object[]
			{
				"Log Verifier ",
				basename,
				" ",
				this.GetHashCode()
			});
			Api.JetCreateInstance(out this.m_instance, this.m_instanceName);
			this.m_basename = basename;
			bool jettermNeeded = true;
			try
			{
				InstanceParameters instanceParameters = new InstanceParameters(this.Instance);
				instanceParameters.Recovery = false;
				instanceParameters.MaxTemporaryTables = 0;
				instanceParameters.NoInformationEvent = true;
				instanceParameters.EventLoggingLevel = EventLoggingLevels.Min;
				instanceParameters.BaseName = basename;
				jettermNeeded = false;
				Api.JetInit(ref this.m_instance);
				jettermNeeded = true;
				Api.JetBeginSession(this.Instance, out this.m_sesid, null, null);
				this.m_dbutil = new JET_DBUTIL();
				this.m_dbutil.sesid = this.Sesid;
				this.m_dbutil.op = DBUTIL_OP.DumpLogfile;
				this.AssertNotTerminated();
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					this.Term(jettermNeeded);
				}
			}
		}

		public LogVerifier(string basename, string csvFile) : this(basename, true)
		{
			this.m_dbutil.grbitOptions = (DbutilGrbit.OptionDumpLogInfoCSV | DbutilGrbit.OptionDumpLogPermitPatching);
			this.m_dbutil.szIntegPrefix = csvFile;
		}

		public LogVerifier(string basename) : this(basename, true)
		{
			this.m_dbutil.grbitOptions = (DbutilGrbit.OptionVerify | DbutilGrbit.OptionDumpLogPermitPatching);
		}

		public static Exception Verify(string fileName, string logFilePrefix, long expectedGen, JET_SIGNATURE? expectedLogSignature)
		{
			LogVerifier logVerifier = new LogVerifier(logFilePrefix);
			EsentErrorException ex = logVerifier.Verify(fileName);
			if (ex != null)
			{
				return ex;
			}
			JET_LOGINFOMISC jet_LOGINFOMISC;
			UnpublishedApi.JetGetLogFileInfo(fileName, out jet_LOGINFOMISC, JET_LogInfo.Misc2);
			if ((long)jet_LOGINFOMISC.ulGeneration != expectedGen)
			{
				return new FileCheckLogfileGenerationException(fileName, (long)jet_LOGINFOMISC.ulGeneration, expectedGen);
			}
			if (expectedLogSignature != null && !jet_LOGINFOMISC.signLog.Equals(expectedLogSignature))
			{
				return new FileCheckLogfileSignatureException(fileName, jet_LOGINFOMISC.signLog.ToString(), expectedLogSignature.ToString());
			}
			return null;
		}

		public void Term()
		{
			this.Term(true);
		}

		private void Term(bool jettermNeeded)
		{
			lock (this)
			{
				this.AssertNotTerminated();
				if (jettermNeeded)
				{
					Api.JetTerm(this.Instance);
				}
				this.m_terminated = true;
			}
		}

		public void Dump(string logfile)
		{
			this.AssertNotTerminated();
			this.m_dbutil.szDatabase = logfile;
			ExTraceGlobals.EseutilWrapperTracer.TraceDebug<string>((long)this.GetHashCode(), "Dumping {0}", logfile);
			UnpublishedApi.JetDBUtilities(this.Dbutil);
			ExTraceGlobals.EseutilWrapperTracer.TraceDebug<string>((long)this.GetHashCode(), "Dumping of logfile {0} detected no errors", logfile);
		}

		public EsentErrorException Verify(string logfile)
		{
			lock (this)
			{
				this.AssertNotTerminated();
				this.m_dbutil.szDatabase = logfile;
				try
				{
					ExTraceGlobals.EseutilWrapperTracer.TraceDebug<string>((long)this.GetHashCode(), "Verifying {0}", logfile);
					UnpublishedApi.JetDBUtilities(this.Dbutil);
					ExTraceGlobals.EseutilWrapperTracer.TraceDebug<string>((long)this.GetHashCode(), "Verification of logfile {0} detected no errors", logfile);
					ExTraceGlobals.PFDTracer.TracePfd<int, string>((long)this.GetHashCode(), "PFD CRS {0} Verification of logfile {1} detected no errors", 31211, logfile);
				}
				catch (EsentErrorException ex)
				{
					ExTraceGlobals.EseutilWrapperTracer.TraceError<string, EsentErrorException>((long)this.GetHashCode(), "Verification of logfile {0} detected corruption: {1}", logfile, ex);
					return ex;
				}
			}
			return null;
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

		private JET_DBUTIL Dbutil
		{
			get
			{
				this.AssertNotTerminated();
				return this.m_dbutil;
			}
		}

		private readonly JET_INSTANCE m_instance;

		private readonly JET_SESID m_sesid;

		private readonly string m_basename;

		private JET_DBUTIL m_dbutil;

		private bool m_terminated;

		private readonly string m_instanceName;
	}
}
