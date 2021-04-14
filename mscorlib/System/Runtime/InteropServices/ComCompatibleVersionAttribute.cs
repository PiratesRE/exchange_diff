using System;

namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	[ComVisible(true)]
	public sealed class ComCompatibleVersionAttribute : Attribute
	{
		public ComCompatibleVersionAttribute(int major, int minor, int build, int revision)
		{
			this._major = major;
			this._minor = minor;
			this._build = build;
			this._revision = revision;
		}

		public int MajorVersion
		{
			get
			{
				return this._major;
			}
		}

		public int MinorVersion
		{
			get
			{
				return this._minor;
			}
		}

		public int BuildNumber
		{
			get
			{
				return this._build;
			}
		}

		public int RevisionNumber
		{
			get
			{
				return this._revision;
			}
		}

		internal int _major;

		internal int _minor;

		internal int _build;

		internal int _revision;
	}
}
