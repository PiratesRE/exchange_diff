using System;
using Microsoft.Exchange.Cluster.Common;

namespace Microsoft.Exchange.Cluster.Replay.MountPoint
{
	internal class MountedFolderPath : IEquatable<MountedFolderPath>, IComparable<MountedFolderPath>
	{
		public static bool IsNullOrEmpty(MountedFolderPath mountedFolderPath)
		{
			return mountedFolderPath == null || string.IsNullOrEmpty(mountedFolderPath.Path);
		}

		public static bool IsEqual(MountedFolderPath src, MountedFolderPath dst)
		{
			return object.ReferenceEquals(src, dst) || (src != null && dst != null && StringUtil.IsEqualIgnoreCase(src.Path, dst.Path));
		}

		public MountedFolderPath(string mountedFolderPath) : this()
		{
			if (!string.IsNullOrEmpty(mountedFolderPath))
			{
				this.m_mountedFolderPathRaw = mountedFolderPath;
				this.m_mountedFolderPath = MountPointUtil.EnsurePathHasTrailingBackSlash(mountedFolderPath);
			}
		}

		private MountedFolderPath()
		{
			this.m_mountedFolderPathRaw = string.Empty;
			this.m_mountedFolderPath = string.Empty;
		}

		public string Path
		{
			get
			{
				return this.m_mountedFolderPath;
			}
		}

		public string RawString
		{
			get
			{
				return this.m_mountedFolderPathRaw;
			}
		}

		public int CompareTo(MountedFolderPath other)
		{
			if (other == null)
			{
				return 1;
			}
			return StringUtil.CompareIgnoreCase(this.Path, other.Path);
		}

		public bool Equals(MountedFolderPath other)
		{
			return MountedFolderPath.IsEqual(this, other);
		}

		public override bool Equals(object obj)
		{
			MountedFolderPath mountedFolderPath = obj as MountedFolderPath;
			return mountedFolderPath != null && MountedFolderPath.IsEqual(this, mountedFolderPath);
		}

		public override string ToString()
		{
			return this.Path;
		}

		public override int GetHashCode()
		{
			return StringUtil.GetStringIHashCode(this.Path);
		}

		public static readonly MountedFolderPath Empty = new MountedFolderPath();

		private readonly string m_mountedFolderPathRaw;

		private readonly string m_mountedFolderPath;
	}
}
