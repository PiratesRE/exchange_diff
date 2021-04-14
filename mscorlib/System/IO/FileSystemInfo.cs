using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using Microsoft.Win32;

namespace System.IO
{
	[ComVisible(true)]
	[FileIOPermission(SecurityAction.InheritanceDemand, Unrestricted = true)]
	[Serializable]
	public abstract class FileSystemInfo : MarshalByRefObject, ISerializable
	{
		protected FileSystemInfo()
		{
		}

		protected FileSystemInfo(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			this.FullPath = Path.GetFullPathInternal(info.GetString("FullPath"));
			this.OriginalPath = info.GetString("OriginalPath");
			this._dataInitialised = -1;
		}

		[SecurityCritical]
		internal void InitializeFrom(ref Win32Native.WIN32_FIND_DATA findData)
		{
			this._data = default(Win32Native.WIN32_FILE_ATTRIBUTE_DATA);
			this._data.PopulateFrom(ref findData);
			this._dataInitialised = 0;
		}

		public virtual string FullName
		{
			[SecuritySafeCritical]
			get
			{
				FileIOPermission.QuickDemand(FileIOPermissionAccess.PathDiscovery, this.FullPath, false, true);
				return this.FullPath;
			}
		}

		internal virtual string UnsafeGetFullName
		{
			[SecurityCritical]
			get
			{
				FileIOPermission.QuickDemand(FileIOPermissionAccess.PathDiscovery, this.FullPath, false, true);
				return this.FullPath;
			}
		}

		public string Extension
		{
			get
			{
				int length = this.FullPath.Length;
				int num = length;
				while (--num >= 0)
				{
					char c = this.FullPath[num];
					if (c == '.')
					{
						return this.FullPath.Substring(num, length - num);
					}
					if (c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar || c == Path.VolumeSeparatorChar)
					{
						break;
					}
				}
				return string.Empty;
			}
		}

		public abstract string Name { get; }

		public abstract bool Exists { get; }

		public abstract void Delete();

		public DateTime CreationTime
		{
			get
			{
				return this.CreationTimeUtc.ToLocalTime();
			}
			set
			{
				this.CreationTimeUtc = value.ToUniversalTime();
			}
		}

		[ComVisible(false)]
		public DateTime CreationTimeUtc
		{
			[SecuritySafeCritical]
			get
			{
				if (this._dataInitialised == -1)
				{
					this._data = default(Win32Native.WIN32_FILE_ATTRIBUTE_DATA);
					this.Refresh();
				}
				if (this._dataInitialised != 0)
				{
					__Error.WinIOError(this._dataInitialised, this.DisplayPath);
				}
				return DateTime.FromFileTimeUtc(this._data.ftCreationTime.ToTicks());
			}
			set
			{
				if (this is DirectoryInfo)
				{
					Directory.SetCreationTimeUtc(this.FullPath, value);
				}
				else
				{
					File.SetCreationTimeUtc(this.FullPath, value);
				}
				this._dataInitialised = -1;
			}
		}

		public DateTime LastAccessTime
		{
			get
			{
				return this.LastAccessTimeUtc.ToLocalTime();
			}
			set
			{
				this.LastAccessTimeUtc = value.ToUniversalTime();
			}
		}

		[ComVisible(false)]
		public DateTime LastAccessTimeUtc
		{
			[SecuritySafeCritical]
			get
			{
				if (this._dataInitialised == -1)
				{
					this._data = default(Win32Native.WIN32_FILE_ATTRIBUTE_DATA);
					this.Refresh();
				}
				if (this._dataInitialised != 0)
				{
					__Error.WinIOError(this._dataInitialised, this.DisplayPath);
				}
				return DateTime.FromFileTimeUtc(this._data.ftLastAccessTime.ToTicks());
			}
			set
			{
				if (this is DirectoryInfo)
				{
					Directory.SetLastAccessTimeUtc(this.FullPath, value);
				}
				else
				{
					File.SetLastAccessTimeUtc(this.FullPath, value);
				}
				this._dataInitialised = -1;
			}
		}

		public DateTime LastWriteTime
		{
			get
			{
				return this.LastWriteTimeUtc.ToLocalTime();
			}
			set
			{
				this.LastWriteTimeUtc = value.ToUniversalTime();
			}
		}

		[ComVisible(false)]
		public DateTime LastWriteTimeUtc
		{
			[SecuritySafeCritical]
			get
			{
				if (this._dataInitialised == -1)
				{
					this._data = default(Win32Native.WIN32_FILE_ATTRIBUTE_DATA);
					this.Refresh();
				}
				if (this._dataInitialised != 0)
				{
					__Error.WinIOError(this._dataInitialised, this.DisplayPath);
				}
				return DateTime.FromFileTimeUtc(this._data.ftLastWriteTime.ToTicks());
			}
			set
			{
				if (this is DirectoryInfo)
				{
					Directory.SetLastWriteTimeUtc(this.FullPath, value);
				}
				else
				{
					File.SetLastWriteTimeUtc(this.FullPath, value);
				}
				this._dataInitialised = -1;
			}
		}

		[SecuritySafeCritical]
		public void Refresh()
		{
			this._dataInitialised = File.FillAttributeInfo(this.FullPath, ref this._data, false, false);
		}

		public FileAttributes Attributes
		{
			[SecuritySafeCritical]
			get
			{
				if (this._dataInitialised == -1)
				{
					this._data = default(Win32Native.WIN32_FILE_ATTRIBUTE_DATA);
					this.Refresh();
				}
				if (this._dataInitialised != 0)
				{
					__Error.WinIOError(this._dataInitialised, this.DisplayPath);
				}
				return (FileAttributes)this._data.fileAttributes;
			}
			[SecuritySafeCritical]
			set
			{
				FileIOPermission.QuickDemand(FileIOPermissionAccess.Write, this.FullPath, false, true);
				if (!Win32Native.SetFileAttributes(this.FullPath, (int)value))
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					if (lastWin32Error == 87)
					{
						throw new ArgumentException(Environment.GetResourceString("Arg_InvalidFileAttrs"));
					}
					if (lastWin32Error == 5)
					{
						throw new ArgumentException(Environment.GetResourceString("UnauthorizedAccess_IODenied_NoPathName"));
					}
					__Error.WinIOError(lastWin32Error, this.DisplayPath);
				}
				this._dataInitialised = -1;
			}
		}

		[SecurityCritical]
		[ComVisible(false)]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			FileIOPermission.QuickDemand(FileIOPermissionAccess.PathDiscovery, this.FullPath, false, true);
			info.AddValue("OriginalPath", this.OriginalPath, typeof(string));
			info.AddValue("FullPath", this.FullPath, typeof(string));
		}

		internal string DisplayPath
		{
			get
			{
				return this._displayPath;
			}
			set
			{
				this._displayPath = value;
			}
		}

		[SecurityCritical]
		internal Win32Native.WIN32_FILE_ATTRIBUTE_DATA _data;

		internal int _dataInitialised = -1;

		private const int ERROR_INVALID_PARAMETER = 87;

		internal const int ERROR_ACCESS_DENIED = 5;

		protected string FullPath;

		protected string OriginalPath;

		private string _displayPath = "";
	}
}
