using System;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop.Windows8
{
	public class JET_COMMIT_ID : IComparable<JET_COMMIT_ID>, IEquatable<JET_COMMIT_ID>
	{
		internal JET_COMMIT_ID(NATIVE_COMMIT_ID native)
		{
			this.signLog = new JET_SIGNATURE(native.signLog);
			this.commitId = native.commitId;
		}

		public static bool operator <(JET_COMMIT_ID lhs, JET_COMMIT_ID rhs)
		{
			return lhs.CompareTo(rhs) < 0;
		}

		public static bool operator >(JET_COMMIT_ID lhs, JET_COMMIT_ID rhs)
		{
			return lhs.CompareTo(rhs) > 0;
		}

		public static bool operator <=(JET_COMMIT_ID lhs, JET_COMMIT_ID rhs)
		{
			return lhs.CompareTo(rhs) <= 0;
		}

		public static bool operator >=(JET_COMMIT_ID lhs, JET_COMMIT_ID rhs)
		{
			return lhs.CompareTo(rhs) >= 0;
		}

		public static bool operator ==(JET_COMMIT_ID lhs, JET_COMMIT_ID rhs)
		{
			return lhs.CompareTo(rhs) == 0;
		}

		public static bool operator !=(JET_COMMIT_ID lhs, JET_COMMIT_ID rhs)
		{
			return lhs.CompareTo(rhs) != 0;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_COMMIT_ID({0}:{1}", new object[]
			{
				this.signLog,
				this.commitId
			});
		}

		public int CompareTo(JET_COMMIT_ID other)
		{
			if (other == null)
			{
				if (this.commitId <= 0L)
				{
					return 0;
				}
				return 1;
			}
			else
			{
				if (this.signLog != other.signLog)
				{
					throw new ArgumentException("The commit-ids belong to different log-streams");
				}
				return this.commitId.CompareTo(other.commitId);
			}
		}

		public bool Equals(JET_COMMIT_ID other)
		{
			return this.CompareTo(other) == 0;
		}

		public override bool Equals(object obj)
		{
			return obj != null && !(base.GetType() != obj.GetType()) && this.CompareTo((JET_COMMIT_ID)obj) == 0;
		}

		public override int GetHashCode()
		{
			return this.commitId.GetHashCode() ^ this.signLog.GetHashCode();
		}

		internal NATIVE_COMMIT_ID GetNativeCommitId()
		{
			return new NATIVE_COMMIT_ID
			{
				signLog = this.signLog.GetNativeSignature(),
				commitId = this.commitId
			};
		}

		private readonly JET_SIGNATURE signLog;

		private readonly long commitId;
	}
}
