using System;

namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = true)]
	[ComVisible(true)]
	public sealed class PrimaryInteropAssemblyAttribute : Attribute
	{
		public PrimaryInteropAssemblyAttribute(int major, int minor)
		{
			this._major = major;
			this._minor = minor;
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

		internal int _major;

		internal int _minor;
	}
}
