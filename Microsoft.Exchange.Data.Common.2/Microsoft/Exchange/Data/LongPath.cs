using System;
using System.Globalization;
using System.IO;
using System.Security;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class LongPath : IComparable, IComparable<LongPath>, IEquatable<LongPath>
	{
		public string PathName
		{
			get
			{
				return this.pathName;
			}
			protected set
			{
				this.pathName = value;
			}
		}

		protected bool IsValid
		{
			get
			{
				return this.isValid;
			}
			set
			{
				this.isValid = value;
			}
		}

		public bool IsLocalFull
		{
			get
			{
				return this.isLocalFull;
			}
		}

		public bool IsUnc
		{
			get
			{
				return this.isUnc;
			}
		}

		public string DriveName
		{
			get
			{
				if (!this.IsLocalFull)
				{
					throw new NotSupportedException("DriveName");
				}
				return this.driveName;
			}
		}

		public string ServerName
		{
			get
			{
				if (!this.IsUnc)
				{
					throw new NotSupportedException("ServerName");
				}
				return this.serverName;
			}
		}

		protected LongPath()
		{
		}

		public static LongPath Parse(string path)
		{
			return LongPath.ParseInternal(path, new LongPath());
		}

		public static explicit operator LongPath(FileInfo file)
		{
			if (file == null)
			{
				return null;
			}
			return LongPath.Parse(file.FullName);
		}

		public static explicit operator LongPath(DirectoryInfo dir)
		{
			if (dir == null)
			{
				return null;
			}
			return LongPath.Parse(dir.FullName);
		}

		public static bool TryParse(string path, out LongPath resultObject)
		{
			resultObject = LongPath.TryParseInternal(path, new LongPath());
			return null != resultObject;
		}

		protected static LongPath ParseInternal(string path, LongPath pathObject)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (!pathObject.ParseCore(path, false))
			{
				throw new ArgumentException(DataStrings.ErrorLongPathCannotConvert(path), "path");
			}
			return pathObject;
		}

		protected static LongPath TryParseInternal(string path, LongPath pathObject)
		{
			if (pathObject.ParseCore(path, true))
			{
				return pathObject;
			}
			return null;
		}

		protected virtual bool ParseCore(string path, bool nothrow)
		{
			try
			{
				if (!string.IsNullOrEmpty(path))
				{
					if (path.Length == 2 && path[1] == Path.VolumeSeparatorChar)
					{
						path += Path.DirectorySeparatorChar;
					}
					if (LongPath.IsLongPath(path) && ((this.isLocalFull = LongPath.IsLocalFullPath(path, out this.driveName)) || (this.isUnc = LongPath.IsUncPath(path, out this.serverName))))
					{
						string text = Path.GetFullPath(path);
						if (this.IsUnc || (this.IsLocalFull && !text.Equals(Path.GetPathRoot(text), StringComparison.OrdinalIgnoreCase)))
						{
							text = text.TrimEnd(new char[]
							{
								Path.DirectorySeparatorChar,
								Path.AltDirectorySeparatorChar
							});
						}
						this.PathName = text;
						this.IsValid = true;
					}
				}
			}
			catch (ArgumentException)
			{
			}
			catch (NotSupportedException)
			{
			}
			catch (PathTooLongException)
			{
			}
			catch (SecurityException ex)
			{
				if (!nothrow)
				{
					throw new ArgumentException(ex.Message, ex);
				}
			}
			return this.IsValid;
		}

		private static bool IsLongPath(string path)
		{
			return path != null && -1 == path.IndexOf('~');
		}

		private static bool IsLocalFullPath(string path, out string drive)
		{
			bool flag = false;
			drive = null;
			try
			{
				flag = !new Uri(path).IsUnc;
			}
			catch (UriFormatException ex)
			{
				throw new ArgumentException(ex.Message, ex);
			}
			if (flag)
			{
				if (!Path.IsPathRooted(path) || -1 == path.IndexOf(Path.VolumeSeparatorChar))
				{
					flag = false;
				}
				else
				{
					drive = path.Substring(0, path.IndexOf(Path.VolumeSeparatorChar) + 1);
				}
			}
			return flag;
		}

		private static bool IsUncPath(string path, out string server)
		{
			server = null;
			try
			{
				Uri uri = new Uri(path);
				if (uri.IsUnc)
				{
					server = uri.Host;
				}
			}
			catch (UriFormatException ex)
			{
				throw new ArgumentException(ex.Message, ex);
			}
			return null != server;
		}

		public override string ToString()
		{
			return this.PathName;
		}

		public override int GetHashCode()
		{
			return this.PathName.ToLower(CultureInfo.InvariantCulture).GetHashCode();
		}

		public int CompareTo(LongPath value)
		{
			if (null == value)
			{
				return 1;
			}
			if (object.ReferenceEquals(this, value))
			{
				return 0;
			}
			Type type = base.GetType();
			if (type != value.GetType())
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "object must be of type {0}", new object[]
				{
					type.Name
				}));
			}
			return string.Compare(this.PathName, value.PathName, StringComparison.OrdinalIgnoreCase);
		}

		public int CompareTo(object value)
		{
			LongPath longPath = value as LongPath;
			if (null == longPath && value != null)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "object must be of type {0}", new object[]
				{
					base.GetType().Name
				}));
			}
			return this.CompareTo(longPath);
		}

		public override bool Equals(object value)
		{
			return this.Equals(value as LongPath);
		}

		public bool Equals(LongPath value)
		{
			if (object.ReferenceEquals(this, value))
			{
				return true;
			}
			bool result = false;
			if (null != value && base.GetType() == value.GetType())
			{
				result = string.Equals(this.PathName, value.PathName, StringComparison.OrdinalIgnoreCase);
			}
			return result;
		}

		public static bool operator ==(LongPath a, LongPath b)
		{
			return object.Equals(a, b);
		}

		public static bool operator !=(LongPath a, LongPath b)
		{
			return !object.Equals(a, b);
		}

		private string pathName;

		private bool isValid;

		private bool isLocalFull;

		private bool isUnc;

		private string driveName;

		private string serverName;
	}
}
