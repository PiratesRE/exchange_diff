using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	public class JET_DBUTIL : IContentEquatable<JET_DBUTIL>, IDeepCloneable<JET_DBUTIL>
	{
		public JET_SESID sesid { [DebuggerStepThrough] get; set; }

		public JET_DBID dbid { [DebuggerStepThrough] get; set; }

		public JET_TABLEID tableid { [DebuggerStepThrough] get; set; }

		public DBUTIL_OP op { [DebuggerStepThrough] get; set; }

		public EDBDUMP_OP edbdump { [DebuggerStepThrough] get; set; }

		public DbutilGrbit grbitOptions { [DebuggerStepThrough] get; set; }

		public string szDatabase { [DebuggerStepThrough] get; set; }

		public string szSLV { [DebuggerStepThrough] get; internal set; }

		public string szBackup { [DebuggerStepThrough] get; set; }

		public string szTable { [DebuggerStepThrough] get; set; }

		public string szIndex { [DebuggerStepThrough] get; set; }

		public string szIntegPrefix { [DebuggerStepThrough] get; set; }

		public int pgno { [DebuggerStepThrough] get; set; }

		public int iline { [DebuggerStepThrough] get; set; }

		public int lGeneration { [DebuggerStepThrough] get; set; }

		public int isec { [DebuggerStepThrough] get; set; }

		public int ib { [DebuggerStepThrough] get; set; }

		public int cRetry { [DebuggerStepThrough] get; set; }

		public IntPtr pfnCallback { [DebuggerStepThrough] get; set; }

		public IntPtr pvCallback { [DebuggerStepThrough] get; set; }

		public string szLog { [DebuggerStepThrough] get; set; }

		public string szBase { [DebuggerStepThrough] get; set; }

		public byte[] pvBuffer { [DebuggerStepThrough] get; set; }

		public int cbBuffer { [DebuggerStepThrough] get; set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_DBUTIL({0})", new object[]
			{
				this.op
			});
		}

		public bool ContentEquals(JET_DBUTIL other)
		{
			if (other == null)
			{
				return false;
			}
			this.CheckMembersAreValid();
			other.CheckMembersAreValid();
			return this.sesid == other.sesid && this.dbid == other.dbid && this.tableid == other.tableid && this.op == other.op && this.edbdump == other.edbdump && this.grbitOptions == other.grbitOptions && this.szDatabase == other.szDatabase && this.szSLV == other.szSLV && this.szBackup == other.szBackup && this.szTable == other.szTable && this.szIndex == other.szIndex && this.szIntegPrefix == other.szIntegPrefix && this.pgno == other.pgno && this.iline == other.iline && this.lGeneration == other.lGeneration && this.isec == other.isec && this.ib == other.ib && this.cRetry == other.cRetry && this.pfnCallback == other.pfnCallback && this.pvCallback == other.pvCallback && this.szLog == other.szLog && this.szBase == other.szBase && this.cbBuffer == other.cbBuffer && Util.ArrayEqual(this.pvBuffer, other.pvBuffer, 0, this.cbBuffer);
		}

		public JET_DBUTIL DeepClone()
		{
			JET_DBUTIL jet_DBUTIL = (JET_DBUTIL)base.MemberwiseClone();
			if (this.pvBuffer != null)
			{
				jet_DBUTIL.pvBuffer = new byte[this.pvBuffer.Length];
				Array.Copy(this.pvBuffer, jet_DBUTIL.pvBuffer, this.pvBuffer.Length);
			}
			return jet_DBUTIL;
		}

		internal void SetFromNative(ref NATIVE_DBUTIL_LEGACY native)
		{
			this.sesid = new JET_SESID
			{
				Value = native.sesid
			};
			this.dbid = new JET_DBID
			{
				Value = native.dbid
			};
			this.tableid = new JET_TABLEID
			{
				Value = native.tableid
			};
			this.op = native.op;
			this.edbdump = native.edbdump;
			this.grbitOptions = native.grbitOptions;
			this.szDatabase = native.szDatabase;
			this.szSLV = native.szSLV;
			this.szBackup = native.szBackup;
			this.szTable = native.szTable;
			this.szIndex = native.szIndex;
			this.szIntegPrefix = native.szIntegPrefix;
			this.pgno = native.pgno;
			this.iline = native.iline;
			this.lGeneration = native.lGeneration;
			this.isec = native.isec;
			this.ib = native.ib;
			this.cRetry = native.cRetry;
			this.pfnCallback = native.pfnCallback;
			this.pvCallback = native.pvCallback;
			this.szLog = null;
			this.szBase = null;
			this.pvBuffer = null;
			this.cbBuffer = 0;
		}

		internal void SetFromNative(ref NATIVE_DBUTIL_CHECKSUMLOG native)
		{
			this.sesid = new JET_SESID
			{
				Value = native.sesid
			};
			this.dbid = new JET_DBID
			{
				Value = native.dbid
			};
			this.tableid = new JET_TABLEID
			{
				Value = native.tableid
			};
			this.op = native.op;
			this.edbdump = native.edbdump;
			this.grbitOptions = native.grbitOptions;
			this.szDatabase = null;
			this.szSLV = null;
			this.szBackup = null;
			this.szTable = null;
			this.szIndex = null;
			this.szIntegPrefix = null;
			this.pgno = 0;
			this.iline = 0;
			this.lGeneration = 0;
			this.isec = 0;
			this.ib = 0;
			this.cRetry = 0;
			this.pfnCallback = IntPtr.Zero;
			this.pvCallback = IntPtr.Zero;
			this.szLog = native.szLog;
			this.szBase = native.szBase;
			this.pvBuffer = null;
			this.cbBuffer = native.cbBuffer;
		}

		internal NATIVE_DBUTIL_LEGACY GetNativeDbutilLegacy()
		{
			return new NATIVE_DBUTIL_LEGACY
			{
				cbStruct = checked((uint)Marshal.SizeOf(typeof(NATIVE_DBUTIL_LEGACY))),
				sesid = this.sesid.Value,
				dbid = this.dbid.Value,
				tableid = this.tableid.Value,
				op = this.op,
				edbdump = this.edbdump,
				grbitOptions = this.grbitOptions,
				szDatabase = this.szDatabase,
				szSLV = this.szSLV,
				szBackup = this.szBackup,
				szTable = this.szTable,
				szIndex = this.szIndex,
				szIntegPrefix = this.szIntegPrefix,
				pgno = this.pgno,
				iline = this.iline,
				lGeneration = this.lGeneration,
				isec = this.isec,
				ib = this.ib,
				cRetry = this.cRetry,
				pfnCallback = this.pfnCallback,
				pvCallback = this.pvCallback
			};
		}

		internal NATIVE_DBUTIL_CHECKSUMLOG GetNativeDbutilChecksumLog()
		{
			return new NATIVE_DBUTIL_CHECKSUMLOG
			{
				cbStruct = checked((uint)Marshal.SizeOf(typeof(NATIVE_DBUTIL_CHECKSUMLOG))),
				sesid = this.sesid.Value,
				dbid = this.dbid.Value,
				tableid = this.tableid.Value,
				op = this.op,
				edbdump = this.edbdump,
				grbitOptions = this.grbitOptions,
				szLog = this.szLog,
				szBase = this.szBase,
				pvBuffer = IntPtr.Zero,
				cbBuffer = this.cbBuffer
			};
		}

		internal void CheckMembersAreValid()
		{
			if (this.cbBuffer < 0)
			{
				throw new ArgumentOutOfRangeException("cbBuffer", this.cbBuffer, "cannot be negative");
			}
			if (this.pvBuffer == null && this.cbBuffer != 0)
			{
				throw new ArgumentOutOfRangeException("cbBuffer", this.cbBuffer, "must be 0");
			}
			if (this.pvBuffer != null && this.cbBuffer > this.pvBuffer.Length)
			{
				throw new ArgumentOutOfRangeException("cbBuffer", this.cbBuffer, "can't be greater than pvBuffer.Length");
			}
		}
	}
}
