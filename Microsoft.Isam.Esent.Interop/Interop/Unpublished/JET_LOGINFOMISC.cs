using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	[Serializable]
	public sealed class JET_LOGINFOMISC : IContentEquatable<JET_LOGINFOMISC>, IDeepCloneable<JET_LOGINFOMISC>
	{
		public int ulGeneration
		{
			[DebuggerStepThrough]
			get
			{
				return this._ulGeneration;
			}
			internal set
			{
				this._ulGeneration = value;
			}
		}

		public JET_SIGNATURE signLog
		{
			[DebuggerStepThrough]
			get
			{
				return this._signLog;
			}
			internal set
			{
				this._signLog = value;
			}
		}

		public JET_LOGTIME logtimeCreate
		{
			[DebuggerStepThrough]
			get
			{
				return this._logtimeCreate;
			}
			internal set
			{
				this._logtimeCreate = value;
			}
		}

		public JET_LOGTIME logtimePreviousGeneration
		{
			[DebuggerStepThrough]
			get
			{
				return this._logtimePreviousGeneration;
			}
			internal set
			{
				this._logtimePreviousGeneration = value;
			}
		}

		public JET_LogInfoFlag ulFlags
		{
			[DebuggerStepThrough]
			get
			{
				return this._ulFlags;
			}
			internal set
			{
				this._ulFlags = value;
			}
		}

		public int ulVersionMajor
		{
			[DebuggerStepThrough]
			get
			{
				return this._ulVersionMajor;
			}
			internal set
			{
				this._ulVersionMajor = value;
			}
		}

		public int ulVersionMinor
		{
			[DebuggerStepThrough]
			get
			{
				return this._ulVersionMinor;
			}
			internal set
			{
				this._ulVersionMinor = value;
			}
		}

		public int ulVersionUpdate
		{
			[DebuggerStepThrough]
			get
			{
				return this._ulVersionUpdate;
			}
			internal set
			{
				this._ulVersionUpdate = value;
			}
		}

		public int cbSectorSize
		{
			[DebuggerStepThrough]
			get
			{
				return this._cbSectorSize;
			}
			internal set
			{
				this._cbSectorSize = value;
			}
		}

		public int cbHeader
		{
			[DebuggerStepThrough]
			get
			{
				return this._cbHeader;
			}
			internal set
			{
				this._cbHeader = value;
			}
		}

		public int cbFile
		{
			[DebuggerStepThrough]
			get
			{
				return this._cbFile;
			}
			internal set
			{
				this._cbFile = value;
			}
		}

		public int cbDatabasePageSize
		{
			[DebuggerStepThrough]
			get
			{
				return this._cbDatabasePageSize;
			}
			internal set
			{
				this._cbDatabasePageSize = value;
			}
		}

		public JET_LGPOS lgposCheckpoint
		{
			[DebuggerStepThrough]
			get
			{
				return this._lgposCheckpoint;
			}
			internal set
			{
				this._lgposCheckpoint = value;
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_LOGINFOMISC({0})", new object[]
			{
				this._signLog
			});
		}

		public bool ContentEquals(JET_LOGINFOMISC other)
		{
			return other != null && (this._ulGeneration == other._ulGeneration && this._signLog == other._signLog && this._logtimeCreate == other._logtimeCreate && this._logtimePreviousGeneration == other._logtimePreviousGeneration && this._ulFlags == other._ulFlags && this._ulVersionMajor == other._ulVersionMajor && this._ulVersionMinor == other._ulVersionMinor && this._ulVersionUpdate == other._ulVersionUpdate && this._cbSectorSize == other._cbSectorSize && this._cbHeader == other._cbHeader && this._cbFile == other._cbFile && this._cbDatabasePageSize == other._cbDatabasePageSize) && this._lgposCheckpoint == other._lgposCheckpoint;
		}

		public JET_LOGINFOMISC DeepClone()
		{
			return (JET_LOGINFOMISC)base.MemberwiseClone();
		}

		internal void SetFromNativeLoginfoMisc(ref NATIVE_LOGINFOMISC native)
		{
			this._ulGeneration = (int)native.ulGeneration;
			this._signLog = new JET_SIGNATURE(native.signLog);
			this._logtimeCreate = native.logtimeCreate;
			this._logtimePreviousGeneration = native.logtimePreviousGeneration;
			this._ulFlags = native.ulFlags;
			this._ulVersionMajor = (int)native.ulVersionMajor;
			this._ulVersionMinor = (int)native.ulVersionMinor;
			this._ulVersionUpdate = (int)native.ulVersionUpdate;
			this._cbSectorSize = (int)native.cbSectorSize;
			this._cbHeader = (int)native.cbHeader;
			this._cbFile = (int)native.cbFile;
			this._cbDatabasePageSize = (int)native.cbDatabasePageSize;
		}

		internal void SetFromNativeLoginfoMisc(ref NATIVE_LOGINFOMISC2 native)
		{
			this.SetFromNativeLoginfoMisc(ref native.loginfo);
			this._lgposCheckpoint = native.lgposCheckpoint;
		}

		internal NATIVE_LOGINFOMISC GetNativeLoginfomisc()
		{
			return new NATIVE_LOGINFOMISC
			{
				ulGeneration = (uint)this._ulGeneration,
				signLog = this._signLog.GetNativeSignature(),
				logtimeCreate = this._logtimeCreate,
				logtimePreviousGeneration = this._logtimePreviousGeneration,
				ulFlags = this._ulFlags,
				ulVersionMajor = (uint)this._ulVersionMajor,
				ulVersionMinor = (uint)this._ulVersionMinor,
				ulVersionUpdate = (uint)this._ulVersionUpdate,
				cbSectorSize = (uint)this._cbSectorSize,
				cbHeader = (uint)this._cbHeader,
				cbFile = (uint)this._cbFile,
				cbDatabasePageSize = (uint)this._cbDatabasePageSize
			};
		}

		internal NATIVE_LOGINFOMISC2 GetNativeLoginfomisc2()
		{
			return new NATIVE_LOGINFOMISC2
			{
				loginfo = this.GetNativeLoginfomisc(),
				lgposCheckpoint = this._lgposCheckpoint
			};
		}

		private int _ulGeneration;

		private JET_SIGNATURE _signLog;

		private JET_LOGTIME _logtimeCreate;

		private JET_LOGTIME _logtimePreviousGeneration;

		private JET_LogInfoFlag _ulFlags;

		private int _ulVersionMajor;

		private int _ulVersionMinor;

		private int _ulVersionUpdate;

		private int _cbSectorSize;

		private int _cbHeader;

		private int _cbFile;

		private int _cbDatabasePageSize;

		private JET_LGPOS _lgposCheckpoint;
	}
}
