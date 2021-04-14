using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public sealed class OperatingSystem : ICloneable, ISerializable
	{
		private OperatingSystem()
		{
		}

		public OperatingSystem(PlatformID platform, Version version) : this(platform, version, null)
		{
		}

		internal OperatingSystem(PlatformID platform, Version version, string servicePack)
		{
			if (platform < PlatformID.Win32S || platform > PlatformID.MacOSX)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_EnumIllegalVal", new object[]
				{
					(int)platform
				}), "platform");
			}
			if (version == null)
			{
				throw new ArgumentNullException("version");
			}
			this._platform = platform;
			this._version = (Version)version.Clone();
			this._servicePack = servicePack;
		}

		private OperatingSystem(SerializationInfo info, StreamingContext context)
		{
			SerializationInfoEnumerator enumerator = info.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string name = enumerator.Name;
				if (!(name == "_version"))
				{
					if (!(name == "_platform"))
					{
						if (name == "_servicePack")
						{
							this._servicePack = info.GetString("_servicePack");
						}
					}
					else
					{
						this._platform = (PlatformID)info.GetValue("_platform", typeof(PlatformID));
					}
				}
				else
				{
					this._version = (Version)info.GetValue("_version", typeof(Version));
				}
			}
			if (this._version == null)
			{
				throw new SerializationException(Environment.GetResourceString("Serialization_MissField", new object[]
				{
					"_version"
				}));
			}
		}

		[SecurityCritical]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("_version", this._version);
			info.AddValue("_platform", this._platform);
			info.AddValue("_servicePack", this._servicePack);
		}

		public PlatformID Platform
		{
			get
			{
				return this._platform;
			}
		}

		public string ServicePack
		{
			get
			{
				if (this._servicePack == null)
				{
					return string.Empty;
				}
				return this._servicePack;
			}
		}

		public Version Version
		{
			get
			{
				return this._version;
			}
		}

		public object Clone()
		{
			return new OperatingSystem(this._platform, this._version, this._servicePack);
		}

		public override string ToString()
		{
			return this.VersionString;
		}

		public string VersionString
		{
			get
			{
				if (this._versionString != null)
				{
					return this._versionString;
				}
				string str;
				switch (this._platform)
				{
				case PlatformID.Win32S:
					str = "Microsoft Win32S ";
					goto IL_9A;
				case PlatformID.Win32Windows:
					if (this._version.Major > 4 || (this._version.Major == 4 && this._version.Minor > 0))
					{
						str = "Microsoft Windows 98 ";
						goto IL_9A;
					}
					str = "Microsoft Windows 95 ";
					goto IL_9A;
				case PlatformID.Win32NT:
					str = "Microsoft Windows NT ";
					goto IL_9A;
				case PlatformID.WinCE:
					str = "Microsoft Windows CE ";
					goto IL_9A;
				case PlatformID.MacOSX:
					str = "Mac OS X ";
					goto IL_9A;
				}
				str = "<unknown> ";
				IL_9A:
				if (string.IsNullOrEmpty(this._servicePack))
				{
					this._versionString = str + this._version.ToString();
				}
				else
				{
					this._versionString = str + this._version.ToString(3) + " " + this._servicePack;
				}
				return this._versionString;
			}
		}

		private Version _version;

		private PlatformID _platform;

		private string _servicePack;

		private string _versionString;
	}
}
