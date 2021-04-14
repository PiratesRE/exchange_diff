using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public struct EumAddress : IEquatable<EumAddress>, IComparable<EumAddress>
	{
		public EumAddress(string address)
		{
			if (address != null && address.Length != 0)
			{
				this.address = address;
				return;
			}
			this.address = null;
		}

		public EumAddress(string extension, string phoneContext)
		{
			if (string.IsNullOrEmpty(extension))
			{
				throw new ArgumentNullException("extension");
			}
			if (string.IsNullOrEmpty(phoneContext))
			{
				throw new ArgumentNullException("phoneContext");
			}
			this.address = EumAddress.BuildAddressString(extension, phoneContext);
		}

		internal static string BuildAddressString(string extension, string phoneContext)
		{
			return extension + ";" + "phone-context=" + phoneContext;
		}

		public int Length
		{
			get
			{
				if (this.address != null)
				{
					return this.address.Length;
				}
				return 0;
			}
		}

		public string Extension
		{
			get
			{
				if (this.address == null)
				{
					return null;
				}
				int num = this.address.LastIndexOf(";phone-context=");
				if (num != -1)
				{
					return this.address.Substring(0, num);
				}
				return null;
			}
		}

		public string PhoneContext
		{
			get
			{
				if (this.address == null)
				{
					return null;
				}
				int num = this.address.LastIndexOf(";phone-context=");
				if (num != -1)
				{
					return this.address.Substring(num + "phone-context=".Length + ";".Length);
				}
				return null;
			}
		}

		public bool IsValid
		{
			get
			{
				return this.address != null;
			}
		}

		public bool IsSipExtension
		{
			get
			{
				return !string.IsNullOrEmpty(this.Extension) && this.Extension.IndexOf("@", StringComparison.OrdinalIgnoreCase) > 0;
			}
		}

		public static bool IsValidEumAddress(string address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (address.IndexOf(";phone-context=") == -1)
			{
				return false;
			}
			string[] separator = new string[]
			{
				";phone-context="
			};
			string[] array = address.Split(separator, StringSplitOptions.None);
			if (array.Length != 2)
			{
				return false;
			}
			EumAddress eumAddress = EumAddress.Parse(address);
			return !string.IsNullOrEmpty(eumAddress.Extension) && !string.IsNullOrEmpty(eumAddress.PhoneContext);
		}

		public static EumAddress Parse(string proxyAddress)
		{
			EumAddress result = new EumAddress(proxyAddress);
			if (!result.IsValid)
			{
				throw new FormatException(DataStrings.InvalidEumAddress(proxyAddress));
			}
			return result;
		}

		public static bool operator ==(EumAddress value1, EumAddress value2)
		{
			return value1.Equals(value2);
		}

		public static bool operator !=(EumAddress value1, EumAddress value2)
		{
			return !(value1 == value2);
		}

		public static explicit operator string(EumAddress address)
		{
			if (address.address == null)
			{
				return string.Empty;
			}
			return address.address;
		}

		public static explicit operator EumAddress(string address)
		{
			return new EumAddress(address);
		}

		public override int GetHashCode()
		{
			if (this.address == null)
			{
				return 0;
			}
			return StringComparer.OrdinalIgnoreCase.GetHashCode(this.address);
		}

		public int CompareTo(EumAddress address)
		{
			return StringComparer.OrdinalIgnoreCase.Compare(this.address, address.address);
		}

		public int CompareTo(object address)
		{
			if (address is EumAddress)
			{
				return this.CompareTo((EumAddress)address);
			}
			string text = address as string;
			if (text != null)
			{
				return string.Compare(this.address, text, StringComparison.OrdinalIgnoreCase);
			}
			if (this.address != null)
			{
				return 1;
			}
			return 0;
		}

		public byte[] GetBytes()
		{
			if (this.address == null)
			{
				return null;
			}
			return CTSGlobals.AsciiEncoding.GetBytes(this.address);
		}

		public override bool Equals(object address)
		{
			if (address is EumAddress)
			{
				return this.Equals((EumAddress)address);
			}
			if (address is ProxyAddress)
			{
				return this.address.Equals(address.ToString(), StringComparison.OrdinalIgnoreCase);
			}
			if (address is string)
			{
				string text = address as string;
				if (text != null)
				{
					return string.Equals(this.address, text, StringComparison.OrdinalIgnoreCase);
				}
			}
			return false;
		}

		public bool Equals(EumAddress address)
		{
			return string.Equals(this.address, address.address, StringComparison.OrdinalIgnoreCase);
		}

		public override string ToString()
		{
			if (this.address == null)
			{
				return string.Empty;
			}
			return this.address;
		}

		public const string UMDialPlanString = "phone-context=";

		public const string UMExtensionDelimiter = ";";

		public static readonly EumAddress Empty = default(EumAddress);

		private string address;
	}
}
