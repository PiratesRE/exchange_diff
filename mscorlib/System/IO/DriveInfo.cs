using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Text;
using Microsoft.Win32;

namespace System.IO
{
	[ComVisible(true)]
	[Serializable]
	public sealed class DriveInfo : ISerializable
	{
		[SecuritySafeCritical]
		public DriveInfo(string driveName)
		{
			if (driveName == null)
			{
				throw new ArgumentNullException("driveName");
			}
			if (driveName.Length == 1)
			{
				this._name = driveName + ":\\";
			}
			else
			{
				Path.CheckInvalidPathChars(driveName, false);
				this._name = Path.GetPathRoot(driveName);
				if (this._name == null || this._name.Length == 0 || this._name.StartsWith("\\\\", StringComparison.Ordinal))
				{
					throw new ArgumentException(Environment.GetResourceString("Arg_MustBeDriveLetterOrRootDir"));
				}
			}
			if (this._name.Length == 2 && this._name[1] == ':')
			{
				this._name += "\\";
			}
			char c = driveName[0];
			if ((c < 'A' || c > 'Z') && (c < 'a' || c > 'z'))
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_MustBeDriveLetterOrRootDir"));
			}
			string path = this._name + ".";
			new FileIOPermission(FileIOPermissionAccess.PathDiscovery, path).Demand();
		}

		[SecurityCritical]
		private DriveInfo(SerializationInfo info, StreamingContext context)
		{
			this._name = (string)info.GetValue("_name", typeof(string));
			string path = this._name + ".";
			new FileIOPermission(FileIOPermissionAccess.PathDiscovery, path).Demand();
		}

		public string Name
		{
			get
			{
				return this._name;
			}
		}

		public DriveType DriveType
		{
			[SecuritySafeCritical]
			get
			{
				return (DriveType)Win32Native.GetDriveType(this.Name);
			}
		}

		public string DriveFormat
		{
			[SecuritySafeCritical]
			get
			{
				StringBuilder volumeName = new StringBuilder(50);
				StringBuilder stringBuilder = new StringBuilder(50);
				int errorMode = Win32Native.SetErrorMode(1);
				try
				{
					int num;
					int num2;
					int num3;
					if (!Win32Native.GetVolumeInformation(this.Name, volumeName, 50, out num, out num2, out num3, stringBuilder, 50))
					{
						int lastWin32Error = Marshal.GetLastWin32Error();
						__Error.WinIODriveError(this.Name, lastWin32Error);
					}
				}
				finally
				{
					Win32Native.SetErrorMode(errorMode);
				}
				return stringBuilder.ToString();
			}
		}

		public bool IsReady
		{
			[SecuritySafeCritical]
			get
			{
				return Directory.InternalExists(this.Name);
			}
		}

		public long AvailableFreeSpace
		{
			[SecuritySafeCritical]
			get
			{
				int errorMode = Win32Native.SetErrorMode(1);
				long result;
				try
				{
					long num;
					long num2;
					if (!Win32Native.GetDiskFreeSpaceEx(this.Name, out result, out num, out num2))
					{
						__Error.WinIODriveError(this.Name);
					}
				}
				finally
				{
					Win32Native.SetErrorMode(errorMode);
				}
				return result;
			}
		}

		public long TotalFreeSpace
		{
			[SecuritySafeCritical]
			get
			{
				int errorMode = Win32Native.SetErrorMode(1);
				long result;
				try
				{
					long num;
					long num2;
					if (!Win32Native.GetDiskFreeSpaceEx(this.Name, out num, out num2, out result))
					{
						__Error.WinIODriveError(this.Name);
					}
				}
				finally
				{
					Win32Native.SetErrorMode(errorMode);
				}
				return result;
			}
		}

		public long TotalSize
		{
			[SecuritySafeCritical]
			get
			{
				int errorMode = Win32Native.SetErrorMode(1);
				long result;
				try
				{
					long num;
					long num2;
					if (!Win32Native.GetDiskFreeSpaceEx(this.Name, out num, out result, out num2))
					{
						__Error.WinIODriveError(this.Name);
					}
				}
				finally
				{
					Win32Native.SetErrorMode(errorMode);
				}
				return result;
			}
		}

		public static DriveInfo[] GetDrives()
		{
			string[] logicalDrives = Directory.GetLogicalDrives();
			DriveInfo[] array = new DriveInfo[logicalDrives.Length];
			for (int i = 0; i < logicalDrives.Length; i++)
			{
				array[i] = new DriveInfo(logicalDrives[i]);
			}
			return array;
		}

		public DirectoryInfo RootDirectory
		{
			get
			{
				return new DirectoryInfo(this.Name);
			}
		}

		public string VolumeLabel
		{
			[SecuritySafeCritical]
			get
			{
				StringBuilder stringBuilder = new StringBuilder(50);
				StringBuilder fileSystemName = new StringBuilder(50);
				int errorMode = Win32Native.SetErrorMode(1);
				try
				{
					int num;
					int num2;
					int num3;
					if (!Win32Native.GetVolumeInformation(this.Name, stringBuilder, 50, out num, out num2, out num3, fileSystemName, 50))
					{
						int num4 = Marshal.GetLastWin32Error();
						if (num4 == 13)
						{
							num4 = 15;
						}
						__Error.WinIODriveError(this.Name, num4);
					}
				}
				finally
				{
					Win32Native.SetErrorMode(errorMode);
				}
				return stringBuilder.ToString();
			}
			[SecuritySafeCritical]
			set
			{
				string path = this._name + ".";
				new FileIOPermission(FileIOPermissionAccess.Write, path).Demand();
				int errorMode = Win32Native.SetErrorMode(1);
				try
				{
					if (!Win32Native.SetVolumeLabel(this.Name, value))
					{
						int lastWin32Error = Marshal.GetLastWin32Error();
						if (lastWin32Error == 5)
						{
							throw new UnauthorizedAccessException(Environment.GetResourceString("InvalidOperation_SetVolumeLabelFailed"));
						}
						__Error.WinIODriveError(this.Name, lastWin32Error);
					}
				}
				finally
				{
					Win32Native.SetErrorMode(errorMode);
				}
			}
		}

		public override string ToString()
		{
			return this.Name;
		}

		[SecurityCritical]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("_name", this._name, typeof(string));
		}

		private string _name;

		private const string NameField = "_name";
	}
}
