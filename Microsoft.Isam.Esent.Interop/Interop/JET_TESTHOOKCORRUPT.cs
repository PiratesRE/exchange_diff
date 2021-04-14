using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.Isam.Esent.Interop.Implementation;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public class JET_TESTHOOKCORRUPT : IContentEquatable<JET_TESTHOOKCORRUPT>, IDeepCloneable<JET_TESTHOOKCORRUPT>
	{
		public int grbit
		{
			[DebuggerStepThrough]
			get
			{
				return this.grbitOptions;
			}
			set
			{
				this.grbitOptions = value;
			}
		}

		public long pgnoTarget
		{
			[DebuggerStepThrough]
			get
			{
				return this.pgnoTargetPage;
			}
			set
			{
				this.pgnoTargetPage = value;
			}
		}

		public long iSubTarget
		{
			[DebuggerStepThrough]
			get
			{
				return this.iSubTargetTarget;
			}
			set
			{
				this.iSubTargetTarget = value;
			}
		}

		public string szDatabaseFilePath
		{
			[DebuggerStepThrough]
			get
			{
				return this.databaseFilePath;
			}
			set
			{
				this.databaseFilePath = value;
			}
		}

		public bool ContentEquals(JET_TESTHOOKCORRUPT other)
		{
			return other != null && (this.grbitOptions == other.grbitOptions && this.pgnoTargetPage == other.pgnoTargetPage && this.iSubTargetTarget == other.iSubTargetTarget) && string.Equals(this.databaseFilePath, other.databaseFilePath);
		}

		public JET_TESTHOOKCORRUPT DeepClone()
		{
			return (JET_TESTHOOKCORRUPT)base.MemberwiseClone();
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_TESTHOOKCORRUPT({0}:{1})", new object[]
			{
				this.grbit,
				this.databaseFilePath,
				this.pgnoTargetPage,
				this.iSubTargetTarget
			});
		}

		internal NATIVE_TESTHOOKCORRUPT_DATABASEFILE GetNativeCorruptDatabaseFile(ref GCHandleCollection handles)
		{
			NATIVE_TESTHOOKCORRUPT_DATABASEFILE native_TESTHOOKCORRUPT_DATABASEFILE = default(NATIVE_TESTHOOKCORRUPT_DATABASEFILE);
			native_TESTHOOKCORRUPT_DATABASEFILE.cbStruct = checked((uint)Marshal.SizeOf(native_TESTHOOKCORRUPT_DATABASEFILE));
			native_TESTHOOKCORRUPT_DATABASEFILE.grbit = (uint)this.grbitOptions;
			native_TESTHOOKCORRUPT_DATABASEFILE.szDatabaseFilePath = handles.Add(Util.ConvertToNullTerminatedUnicodeByteArray(this.szDatabaseFilePath));
			native_TESTHOOKCORRUPT_DATABASEFILE.pgnoTarget = this.pgnoTarget;
			native_TESTHOOKCORRUPT_DATABASEFILE.iSubTarget = this.iSubTarget;
			return native_TESTHOOKCORRUPT_DATABASEFILE;
		}

		private int grbitOptions;

		private string databaseFilePath;

		private long pgnoTargetPage;

		private long iSubTargetTarget;
	}
}
