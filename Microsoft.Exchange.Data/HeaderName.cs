using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public struct HeaderName : IComparable, ISerializable
	{
		public HeaderName(string input)
		{
			this.value = null;
			if (this.IsValid(input))
			{
				this.value = input;
				return;
			}
			throw new ArgumentOutOfRangeException(DataStrings.HeaderName, DataStrings.InvalidInputErrorMsg);
		}

		public static HeaderName Parse(string s)
		{
			return new HeaderName(s);
		}

		private HeaderName(SerializationInfo info, StreamingContext context)
		{
			this.value = (string)info.GetValue("value", typeof(string));
			if (!this.IsValid(this.value))
			{
				throw new ArgumentOutOfRangeException(DataStrings.HeaderName, this.value.ToString());
			}
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("value", this.value);
		}

		private bool IsValid(string input)
		{
			return input == null || (input.Length <= 64 && HeaderName.ValidatingExpression.IsMatch(input));
		}

		public static HeaderName Empty
		{
			get
			{
				return default(HeaderName);
			}
		}

		public string Value
		{
			get
			{
				if (this.IsValid(this.value))
				{
					return this.value;
				}
				throw new ArgumentOutOfRangeException("Value", this.value.ToString());
			}
		}

		public override string ToString()
		{
			if (this.value == null)
			{
				return string.Empty;
			}
			return this.value.ToString();
		}

		public override int GetHashCode()
		{
			if (this.value == null)
			{
				return string.Empty.GetHashCode();
			}
			return this.value.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is HeaderName && this.Equals((HeaderName)obj);
		}

		public bool Equals(HeaderName obj)
		{
			return this.value == obj.Value;
		}

		public static bool operator ==(HeaderName a, HeaderName b)
		{
			return a.Value == b.Value;
		}

		public static bool operator !=(HeaderName a, HeaderName b)
		{
			return a.Value != b.Value;
		}

		public int CompareTo(object obj)
		{
			if (!(obj is HeaderName))
			{
				throw new ArgumentException("Parameter is not of type HeaderName.");
			}
			return string.Compare(this.value, ((HeaderName)obj).Value, StringComparison.OrdinalIgnoreCase);
		}

		public const int MaxLength = 64;

		public const string AllowedCharacters = "[\\x21-\\x39\\x3b-\\x7e]";

		public static readonly Regex ValidatingExpression = new Regex("^[\\x21-\\x39\\x3b-\\x7e]+$", RegexOptions.Compiled);

		private string value;
	}
}
