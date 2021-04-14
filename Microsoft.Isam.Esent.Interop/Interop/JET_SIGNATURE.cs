using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	[StructLayout(LayoutKind.Auto)]
	public struct JET_SIGNATURE : IEquatable<JET_SIGNATURE>
	{
		internal JET_SIGNATURE(int random, DateTime? time, string computerName)
		{
			this.ulRandom = (uint)random;
			this.logtimeCreate = ((time != null) ? new JET_LOGTIME(time.Value) : default(JET_LOGTIME));
			this.szComputerName = computerName;
		}

		internal JET_SIGNATURE(NATIVE_SIGNATURE native)
		{
			this.ulRandom = native.ulRandom;
			this.logtimeCreate = native.logtimeCreate;
			this.szComputerName = native.szComputerName;
		}

		public static bool operator ==(JET_SIGNATURE lhs, JET_SIGNATURE rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(JET_SIGNATURE lhs, JET_SIGNATURE rhs)
		{
			return !(lhs == rhs);
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_SIGNATURE({0}:{1}:{2})", new object[]
			{
				this.ulRandom,
				this.logtimeCreate.ToDateTime(),
				this.szComputerName
			});
		}

		public override bool Equals(object obj)
		{
			return obj != null && !(base.GetType() != obj.GetType()) && this.Equals((JET_SIGNATURE)obj);
		}

		public override int GetHashCode()
		{
			return this.ulRandom.GetHashCode() ^ this.logtimeCreate.GetHashCode() ^ ((this.szComputerName == null) ? -1 : this.szComputerName.GetHashCode());
		}

		public bool Equals(JET_SIGNATURE other)
		{
			bool flag = (string.IsNullOrEmpty(this.szComputerName) && string.IsNullOrEmpty(other.szComputerName)) || (!string.IsNullOrEmpty(this.szComputerName) && !string.IsNullOrEmpty(other.szComputerName) && this.szComputerName == other.szComputerName);
			return flag && this.ulRandom == other.ulRandom && this.logtimeCreate == other.logtimeCreate;
		}

		internal NATIVE_SIGNATURE GetNativeSignature()
		{
			return new NATIVE_SIGNATURE
			{
				ulRandom = this.ulRandom,
				szComputerName = this.szComputerName,
				logtimeCreate = this.logtimeCreate
			};
		}

		internal readonly uint ulRandom;

		internal readonly JET_LOGTIME logtimeCreate;

		private readonly string szComputerName;
	}
}
