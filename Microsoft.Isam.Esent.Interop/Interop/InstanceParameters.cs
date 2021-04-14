using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Isam.Esent.Interop.Unpublished;
using Microsoft.Isam.Esent.Interop.Windows8;
using Microsoft.Isam.Esent.Interop.Windows81;

namespace Microsoft.Isam.Esent.Interop
{
	public class InstanceParameters
	{
		public JET_INDEXCHECKING EnableIndexCheckingEx
		{
			get
			{
				return (JET_INDEXCHECKING)this.GetIntegerParameter(JET_param.EnableIndexChecking);
			}
			set
			{
				this.SetIntegerParameter(JET_param.EnableIndexChecking, (int)value);
			}
		}

		public bool EnableExternalAutoHealing
		{
			get
			{
				return this.GetBoolParameter((JET_param)175);
			}
			set
			{
				this.SetBoolParameter((JET_param)175, value);
			}
		}

		public bool AggressiveLogRollover
		{
			get
			{
				return this.GetBoolParameter((JET_param)157);
			}
			set
			{
				this.SetBoolParameter((JET_param)157, value);
			}
		}

		public bool EnableHaPublish
		{
			get
			{
				return this.GetBoolParameter((JET_param)168);
			}
			set
			{
				this.SetBoolParameter((JET_param)168, value);
			}
		}

		public int CheckpointTooDeep
		{
			get
			{
				return this.GetIntegerParameter((JET_param)154);
			}
			set
			{
				this.SetIntegerParameter((JET_param)154, value);
			}
		}

		public int AutomaticShrinkDatabaseFreeSpaceThreshold
		{
			get
			{
				return this.GetIntegerParameter((JET_param)185);
			}
			set
			{
				this.SetIntegerParameter((JET_param)185, value);
			}
		}

		public bool ZeroDatabaseUnusedSpace
		{
			get
			{
				return this.GetBoolParameter((JET_param)191);
			}
			set
			{
				this.SetBoolParameter((JET_param)191, value);
			}
		}

		internal IntPtr EmitLogDataCallbackCtx
		{
			get
			{
				return this.GetIntPtrParameter((JET_param)174);
			}
			set
			{
				this.SetIntPtrParameter((JET_param)174, value);
			}
		}

		internal NATIVE_JET_PFNEMITLOGDATA GetEmitLogDataCallback()
		{
			NATIVE_JET_PFNEMITLOGDATA result = null;
			IntPtr intPtrParameter = this.GetIntPtrParameter((JET_param)173);
			if (intPtrParameter != IntPtr.Zero)
			{
				result = (NATIVE_JET_PFNEMITLOGDATA)Marshal.GetDelegateForFunctionPointer(intPtrParameter, typeof(NATIVE_JET_PFNEMITLOGDATA));
			}
			return result;
		}

		internal void SetEmitLogDataCallback(NATIVE_JET_PFNEMITLOGDATA callback)
		{
			IntPtr value;
			if (callback != null)
			{
				value = Marshal.GetFunctionPointerForDelegate(callback);
			}
			else
			{
				value = IntPtr.Zero;
			}
			this.SetIntPtrParameter((JET_param)173, value);
		}

		public InstanceParameters(JET_INSTANCE instance)
		{
			this.instance = instance;
			this.sesid = JET_SESID.Nil;
		}

		public string SystemDirectory
		{
			get
			{
				return Util.AddTrailingDirectorySeparator(this.GetStringParameter(JET_param.SystemPath));
			}
			set
			{
				this.SetStringParameter(JET_param.SystemPath, Util.AddTrailingDirectorySeparator(value));
			}
		}

		public string TempDirectory
		{
			get
			{
				string stringParameter = this.GetStringParameter(JET_param.TempPath);
				string directoryName = Path.GetDirectoryName(stringParameter);
				return Util.AddTrailingDirectorySeparator(directoryName);
			}
			set
			{
				this.SetStringParameter(JET_param.TempPath, Util.AddTrailingDirectorySeparator(value));
			}
		}

		public string LogFileDirectory
		{
			get
			{
				return Util.AddTrailingDirectorySeparator(this.GetStringParameter(JET_param.LogFilePath));
			}
			set
			{
				this.SetStringParameter(JET_param.LogFilePath, Util.AddTrailingDirectorySeparator(value));
			}
		}

		public string AlternateDatabaseRecoveryDirectory
		{
			get
			{
				if (EsentVersion.SupportsServer2003Features)
				{
					return Util.AddTrailingDirectorySeparator(this.GetStringParameter((JET_param)113));
				}
				return null;
			}
			set
			{
				if (EsentVersion.SupportsServer2003Features)
				{
					this.SetStringParameter((JET_param)113, Util.AddTrailingDirectorySeparator(value));
				}
			}
		}

		public string BaseName
		{
			get
			{
				return this.GetStringParameter(JET_param.BaseName);
			}
			set
			{
				this.SetStringParameter(JET_param.BaseName, value);
			}
		}

		public string EventSource
		{
			get
			{
				return this.GetStringParameter(JET_param.EventSource);
			}
			set
			{
				this.SetStringParameter(JET_param.EventSource, value);
			}
		}

		public int MaxSessions
		{
			get
			{
				return this.GetIntegerParameter(JET_param.MaxSessions);
			}
			set
			{
				this.SetIntegerParameter(JET_param.MaxSessions, value);
			}
		}

		public int MaxOpenTables
		{
			get
			{
				return this.GetIntegerParameter(JET_param.MaxOpenTables);
			}
			set
			{
				this.SetIntegerParameter(JET_param.MaxOpenTables, value);
			}
		}

		public int MaxCursors
		{
			get
			{
				return this.GetIntegerParameter(JET_param.MaxCursors);
			}
			set
			{
				this.SetIntegerParameter(JET_param.MaxCursors, value);
			}
		}

		public int MaxVerPages
		{
			get
			{
				return this.GetIntegerParameter(JET_param.MaxVerPages);
			}
			set
			{
				this.SetIntegerParameter(JET_param.MaxVerPages, value);
			}
		}

		public int PreferredVerPages
		{
			get
			{
				return this.GetIntegerParameter(JET_param.PreferredVerPages);
			}
			set
			{
				this.SetIntegerParameter(JET_param.PreferredVerPages, value);
			}
		}

		public int VersionStoreTaskQueueMax
		{
			get
			{
				return this.GetIntegerParameter(JET_param.VersionStoreTaskQueueMax);
			}
			set
			{
				this.SetIntegerParameter(JET_param.VersionStoreTaskQueueMax, value);
			}
		}

		public int MaxTemporaryTables
		{
			get
			{
				return this.GetIntegerParameter(JET_param.MaxTemporaryTables);
			}
			set
			{
				this.SetIntegerParameter(JET_param.MaxTemporaryTables, value);
			}
		}

		public int LogFileSize
		{
			get
			{
				return this.GetIntegerParameter(JET_param.LogFileSize);
			}
			set
			{
				this.SetIntegerParameter(JET_param.LogFileSize, value);
			}
		}

		public int LogBuffers
		{
			get
			{
				return this.GetIntegerParameter(JET_param.LogBuffers);
			}
			set
			{
				this.SetIntegerParameter(JET_param.LogBuffers, value);
			}
		}

		public bool CircularLog
		{
			get
			{
				return this.GetBoolParameter(JET_param.CircularLog);
			}
			set
			{
				this.SetBoolParameter(JET_param.CircularLog, value);
			}
		}

		public bool CleanupMismatchedLogFiles
		{
			get
			{
				return this.GetBoolParameter(JET_param.CleanupMismatchedLogFiles);
			}
			set
			{
				this.SetBoolParameter(JET_param.CleanupMismatchedLogFiles, value);
			}
		}

		public int PageTempDBMin
		{
			get
			{
				return this.GetIntegerParameter(JET_param.PageTempDBMin);
			}
			set
			{
				this.SetIntegerParameter(JET_param.PageTempDBMin, value);
			}
		}

		public int CheckpointDepthMax
		{
			get
			{
				return this.GetIntegerParameter(JET_param.CheckpointDepthMax);
			}
			set
			{
				this.SetIntegerParameter(JET_param.CheckpointDepthMax, value);
			}
		}

		public int DbExtensionSize
		{
			get
			{
				return this.GetIntegerParameter(JET_param.DbExtensionSize);
			}
			set
			{
				this.SetIntegerParameter(JET_param.DbExtensionSize, value);
			}
		}

		public bool Recovery
		{
			get
			{
				return 0 == string.Compare(this.GetStringParameter(JET_param.Recovery), "on", StringComparison.OrdinalIgnoreCase);
			}
			set
			{
				if (value)
				{
					this.SetStringParameter(JET_param.Recovery, "on");
					return;
				}
				this.SetStringParameter(JET_param.Recovery, "off");
			}
		}

		public bool EnableOnlineDefrag
		{
			get
			{
				return this.GetBoolParameter(JET_param.EnableOnlineDefrag);
			}
			set
			{
				this.SetBoolParameter(JET_param.EnableOnlineDefrag, value);
			}
		}

		public bool EnableIndexChecking
		{
			get
			{
				return this.GetBoolParameter(JET_param.EnableIndexChecking);
			}
			set
			{
				this.SetBoolParameter(JET_param.EnableIndexChecking, value);
			}
		}

		public string EventSourceKey
		{
			get
			{
				return this.GetStringParameter(JET_param.EventSourceKey);
			}
			set
			{
				this.SetStringParameter(JET_param.EventSourceKey, value);
			}
		}

		public bool NoInformationEvent
		{
			get
			{
				return this.GetBoolParameter(JET_param.NoInformationEvent);
			}
			set
			{
				this.SetBoolParameter(JET_param.NoInformationEvent, value);
			}
		}

		public EventLoggingLevels EventLoggingLevel
		{
			get
			{
				return (EventLoggingLevels)this.GetIntegerParameter(JET_param.EventLoggingLevel);
			}
			set
			{
				this.SetIntegerParameter(JET_param.EventLoggingLevel, (int)value);
			}
		}

		public bool OneDatabasePerSession
		{
			get
			{
				return this.GetBoolParameter(JET_param.OneDatabasePerSession);
			}
			set
			{
				this.SetBoolParameter(JET_param.OneDatabasePerSession, value);
			}
		}

		public bool CreatePathIfNotExist
		{
			get
			{
				return this.GetBoolParameter(JET_param.CreatePathIfNotExist);
			}
			set
			{
				this.SetBoolParameter(JET_param.CreatePathIfNotExist, value);
			}
		}

		public int CachedClosedTables
		{
			get
			{
				if (EsentVersion.SupportsVistaFeatures)
				{
					return this.GetIntegerParameter((JET_param)125);
				}
				return 0;
			}
			set
			{
				if (EsentVersion.SupportsVistaFeatures)
				{
					this.SetIntegerParameter((JET_param)125, value);
				}
			}
		}

		public int WaypointLatency
		{
			get
			{
				if (EsentVersion.SupportsWindows7Features)
				{
					return this.GetIntegerParameter((JET_param)153);
				}
				return 0;
			}
			set
			{
				if (EsentVersion.SupportsWindows7Features)
				{
					this.SetIntegerParameter((JET_param)153, value);
				}
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "InstanceParameters (0x{0:x})", new object[]
			{
				this.instance.Value
			});
		}

		private void SetStringParameter(JET_param param, string value)
		{
			Api.JetSetSystemParameter(this.instance, this.sesid, param, 0, value);
		}

		private string GetStringParameter(JET_param param)
		{
			int num = 0;
			string result;
			Api.JetGetSystemParameter(this.instance, this.sesid, param, ref num, out result, 1024);
			return result;
		}

		private void SetIntegerParameter(JET_param param, int value)
		{
			Api.JetSetSystemParameter(this.instance, this.sesid, param, value, null);
		}

		private int GetIntegerParameter(JET_param param)
		{
			int result = 0;
			string text;
			Api.JetGetSystemParameter(this.instance, this.sesid, param, ref result, out text, 0);
			return result;
		}

		private void SetBoolParameter(JET_param param, bool value)
		{
			if (value)
			{
				Api.JetSetSystemParameter(this.instance, this.sesid, param, 1, null);
				return;
			}
			Api.JetSetSystemParameter(this.instance, this.sesid, param, 0, null);
		}

		private bool GetBoolParameter(JET_param param)
		{
			int num = 0;
			string text;
			Api.JetGetSystemParameter(this.instance, this.sesid, param, ref num, out text, 0);
			return num != 0;
		}

		public int MaxTransactionSize
		{
			get
			{
				return this.GetIntegerParameter((JET_param)178);
			}
			set
			{
				this.SetIntegerParameter((JET_param)178, value);
			}
		}

		public int EnableDbScanInRecovery
		{
			get
			{
				return this.GetIntegerParameter((JET_param)169);
			}
			set
			{
				this.SetIntegerParameter((JET_param)169, value);
			}
		}

		public bool EnableDBScanSerialization
		{
			get
			{
				return this.GetBoolParameter((JET_param)180);
			}
			set
			{
				this.SetBoolParameter((JET_param)180, value);
			}
		}

		public int DbScanThrottle
		{
			get
			{
				return this.GetIntegerParameter((JET_param)170);
			}
			set
			{
				this.SetIntegerParameter((JET_param)170, value);
			}
		}

		public int DbScanIntervalMinSec
		{
			get
			{
				return this.GetIntegerParameter((JET_param)171);
			}
			set
			{
				this.SetIntegerParameter((JET_param)171, value);
			}
		}

		public int DbScanIntervalMaxSec
		{
			get
			{
				return this.GetIntegerParameter((JET_param)172);
			}
			set
			{
				this.SetIntegerParameter((JET_param)172, value);
			}
		}

		public int CachePriority
		{
			get
			{
				return this.GetIntegerParameter((JET_param)177);
			}
			set
			{
				this.SetIntegerParameter((JET_param)177, value);
			}
		}

		public int PrereadIOMax
		{
			get
			{
				return this.GetIntegerParameter((JET_param)179);
			}
			set
			{
				this.SetIntegerParameter((JET_param)179, value);
			}
		}

		internal NATIVE_JET_PFNDURABLECOMMITCALLBACK GetDurableCommitCallback()
		{
			NATIVE_JET_PFNDURABLECOMMITCALLBACK result = null;
			IntPtr intPtrParameter = this.GetIntPtrParameter((JET_param)187);
			if (intPtrParameter != IntPtr.Zero)
			{
				result = (NATIVE_JET_PFNDURABLECOMMITCALLBACK)Marshal.GetDelegateForFunctionPointer(intPtrParameter, typeof(NATIVE_JET_PFNDURABLECOMMITCALLBACK));
			}
			return result;
		}

		internal void SetDurableCommitCallback(NATIVE_JET_PFNDURABLECOMMITCALLBACK callback)
		{
			IntPtr value;
			if (callback != null)
			{
				value = Marshal.GetFunctionPointerForDelegate(callback);
			}
			else
			{
				value = IntPtr.Zero;
			}
			this.SetIntPtrParameter((JET_param)187, value);
		}

		private IntPtr GetIntPtrParameter(JET_param param)
		{
			IntPtr zero = IntPtr.Zero;
			string text;
			Api.JetGetSystemParameter(this.instance, this.sesid, param, ref zero, out text, 0);
			return zero;
		}

		private void SetIntPtrParameter(JET_param param, IntPtr value)
		{
			Api.JetSetSystemParameter(this.instance, this.sesid, param, value, null);
		}

		public ShrinkDatabaseGrbit EnableShrinkDatabase
		{
			get
			{
				return (ShrinkDatabaseGrbit)this.GetIntegerParameter((JET_param)184);
			}
			set
			{
				this.SetIntegerParameter((JET_param)184, (int)value);
			}
		}

		private readonly JET_INSTANCE instance;

		private readonly JET_SESID sesid;
	}
}
