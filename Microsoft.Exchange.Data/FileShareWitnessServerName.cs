using System;
using System.Globalization;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class FileShareWitnessServerName : IComparable, IComparable<FileShareWitnessServerName>, IEquatable<FileShareWitnessServerName>, ISerializable
	{
		public bool isHostName
		{
			get
			{
				return this.m_isHostName;
			}
		}

		public bool isFqdn
		{
			get
			{
				return !this.m_isHostName;
			}
		}

		public string Fqdn
		{
			get
			{
				return this.m_fqdn;
			}
		}

		public string HostName
		{
			get
			{
				return this.m_hostName;
			}
		}

		public string DomainName
		{
			get
			{
				return this.m_domainName;
			}
		}

		public string RawData
		{
			get
			{
				if (this.isFqdn)
				{
					return this.Fqdn;
				}
				return this.HostName;
			}
		}

		protected FileShareWitnessServerName()
		{
		}

		public static FileShareWitnessServerName Parse(string FileShareWitnessServerName)
		{
			return FileShareWitnessServerName.ParseInternal(FileShareWitnessServerName, new FileShareWitnessServerName());
		}

		public static bool TryParse(string FileShareWitnessServerName, out FileShareWitnessServerName FileShareWitnessServerNameObject)
		{
			FileShareWitnessServerNameObject = FileShareWitnessServerName.TryParseInternal(FileShareWitnessServerName, new FileShareWitnessServerName());
			return null != FileShareWitnessServerNameObject;
		}

		protected static FileShareWitnessServerName ParseInternal(string FileShareWitnessServerName, FileShareWitnessServerName FileShareWitnessServerNameObject)
		{
			if (FileShareWitnessServerName == null)
			{
				throw new ArgumentNullException("FileShareWitnessServerName");
			}
			if (!FileShareWitnessServerNameObject.ParseCore(FileShareWitnessServerName, false))
			{
				throw new ArgumentException(DataStrings.ErrorFileShareWitnessServerNameCannotConvert(FileShareWitnessServerName), "FileShareWitnessServerName");
			}
			return FileShareWitnessServerNameObject;
		}

		protected static FileShareWitnessServerName TryParseInternal(string FileShareWitnessServerName, FileShareWitnessServerName FileShareWitnessServerNameObject)
		{
			if (FileShareWitnessServerNameObject.ParseCore(FileShareWitnessServerName, true))
			{
				return FileShareWitnessServerNameObject;
			}
			return null;
		}

		private void AssignFromHostName(string hostName, bool nothrow)
		{
			this.m_fqdn = null;
			this.m_hostName = hostName;
			this.m_domainName = null;
			this.m_isValid = true;
			this.m_isHostName = true;
		}

		private void AssignFromFqdn(string fqdn, bool nothrow)
		{
			int num = fqdn.IndexOf('.');
			if (num > -1)
			{
				this.m_fqdn = fqdn;
				this.m_hostName = fqdn.Substring(0, num);
				this.m_domainName = fqdn.Substring(num + 1);
				this.m_isValid = true;
				this.m_isHostName = false;
				return;
			}
			if (!nothrow)
			{
				throw new ArgumentException(DataStrings.ErrorFileShareWitnessServerNameIsNotValidHostNameorFqdn(fqdn), "FileShareWitnessServerName");
			}
		}

		protected virtual bool ParseCore(string FileShareWitnessServerName, bool nothrow)
		{
			IPAddress ipaddress;
			if (string.IsNullOrEmpty(FileShareWitnessServerName))
			{
				if (!nothrow)
				{
					throw new ArgumentException(DataStrings.ErrorFileShareWitnessServerNameMustNotBeEmpty, "FileShareWitnessServerName");
				}
				return false;
			}
			else if (IPAddress.TryParse(FileShareWitnessServerName, out ipaddress))
			{
				if (!nothrow)
				{
					throw new ArgumentException(DataStrings.ErrorFileShareWitnessServerNameMustNotBeIP(FileShareWitnessServerName), "FileShareWitnessServerName");
				}
				return false;
			}
			else if (string.Equals(FileShareWitnessServerName, FileShareWitnessServerName.s_localHostName, StringComparison.OrdinalIgnoreCase) || string.Equals(FileShareWitnessServerName, FileShareWitnessServerName.s_localHostFqdn, StringComparison.OrdinalIgnoreCase))
			{
				if (!nothrow)
				{
					throw new ArgumentException(DataStrings.ErrorFileShareWitnessServerNameIsLocalhost(FileShareWitnessServerName), "FileShareWitnessServerName");
				}
				return false;
			}
			else
			{
				if (Microsoft.Exchange.Data.Fqdn.IsValidFqdn(FileShareWitnessServerName))
				{
					if (FileShareWitnessServerName.IndexOf('.') == -1)
					{
						this.AssignFromHostName(FileShareWitnessServerName, nothrow);
					}
					else
					{
						this.AssignFromFqdn(FileShareWitnessServerName, nothrow);
					}
					return this.m_isValid;
				}
				if (nothrow)
				{
					return false;
				}
				if (FileShareWitnessServerName.Contains(FileShareWitnessServerName.s_wildcard))
				{
					throw new ArgumentException(DataStrings.ErrorFileShareWitnessServerNameIsNotValidHostNameorFqdnWildcard(FileShareWitnessServerName), "FileShareWitnessServerName");
				}
				throw new ArgumentException(DataStrings.ErrorFileShareWitnessServerNameIsNotValidHostNameorFqdn(FileShareWitnessServerName), "FileShareWitnessServerName");
			}
		}

		public override string ToString()
		{
			return this.RawData;
		}

		public override int GetHashCode()
		{
			return this.RawData.ToLowerInvariant().GetHashCode();
		}

		public override bool Equals(object value)
		{
			return this.Equals(value as FileShareWitnessServerName);
		}

		public static bool operator ==(FileShareWitnessServerName a, FileShareWitnessServerName b)
		{
			return FileShareWitnessServerName.Equals(a, b);
		}

		public static bool operator !=(FileShareWitnessServerName a, FileShareWitnessServerName b)
		{
			return !FileShareWitnessServerName.Equals(a, b);
		}

		public int CompareTo(object value)
		{
			FileShareWitnessServerName fileShareWitnessServerName = value as FileShareWitnessServerName;
			if (null == fileShareWitnessServerName && value != null)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "object must be of type {0}", new object[]
				{
					base.GetType().Name
				}));
			}
			return this.CompareTo(fileShareWitnessServerName);
		}

		public int CompareTo(FileShareWitnessServerName value)
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
			return string.Compare(this.RawData, value.RawData, StringComparison.OrdinalIgnoreCase);
		}

		public bool Equals(FileShareWitnessServerName value)
		{
			if (object.ReferenceEquals(this, value))
			{
				return true;
			}
			bool result = false;
			if (null != value && base.GetType() == value.GetType())
			{
				result = string.Equals(this.RawData, value.RawData, StringComparison.OrdinalIgnoreCase);
			}
			return result;
		}

		public static bool Equals(FileShareWitnessServerName a, FileShareWitnessServerName b)
		{
			return object.ReferenceEquals(a, b) || (!object.ReferenceEquals(a, null) && !object.ReferenceEquals(b, null) && string.Equals(a.RawData, b.RawData, StringComparison.OrdinalIgnoreCase));
		}

		private FileShareWitnessServerName(SerializationInfo info, StreamingContext context)
		{
			this.m_fqdn = (string)info.GetValue("fqdn", typeof(string));
			this.m_hostName = (string)info.GetValue("hostName", typeof(string));
			this.m_domainName = (string)info.GetValue("domainName", typeof(string));
			this.m_isValid = (bool)info.GetValue("isValid", typeof(bool));
			this.m_isHostName = (bool)info.GetValue("isHostName", typeof(bool));
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("fqdn", this.m_fqdn);
			info.AddValue("hostName", this.m_hostName);
			info.AddValue("domainName", this.m_domainName);
			info.AddValue("isValid", this.m_isValid);
			info.AddValue("isHostName", this.m_isHostName);
		}

		private string m_fqdn;

		private string m_hostName;

		private string m_domainName;

		private bool m_isValid;

		private bool m_isHostName;

		private static string s_localHostName = "localhost";

		private static string s_localHostFqdn = "localhost.localdomain";

		private static string s_wildcard = "*";
	}
}
