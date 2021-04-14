using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public struct HeaderValue : IComparable, ISerializable
	{
		public HeaderValue(string input)
		{
			this.value = null;
			if (this.IsValid(input))
			{
				this.value = input;
				return;
			}
			throw new ArgumentOutOfRangeException(DataStrings.HeaderValue, DataStrings.InvalidInputErrorMsg);
		}

		public static HeaderValue Parse(string s)
		{
			return new HeaderValue(s);
		}

		private HeaderValue(SerializationInfo info, StreamingContext context)
		{
			this.value = (string)info.GetValue("value", typeof(string));
			if (!this.IsValid(this.value))
			{
				throw new ArgumentOutOfRangeException(DataStrings.HeaderValue, this.value.ToString());
			}
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("value", this.value);
		}

		private bool IsValid(string input)
		{
			return input == null || (input.Length <= 128 && HeaderValue.ValidatingExpression.IsMatch(input));
		}

		public static HeaderValue Empty
		{
			get
			{
				return default(HeaderValue);
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
			return obj is HeaderValue && this.Equals((HeaderValue)obj);
		}

		public bool Equals(HeaderValue obj)
		{
			return this.value == obj.Value;
		}

		public static bool operator ==(HeaderValue a, HeaderValue b)
		{
			return a.Value == b.Value;
		}

		public static bool operator !=(HeaderValue a, HeaderValue b)
		{
			return a.Value != b.Value;
		}

		public int CompareTo(object obj)
		{
			if (!(obj is HeaderValue))
			{
				throw new ArgumentException("Parameter is not of type HeaderValue.");
			}
			return string.Compare(this.value, ((HeaderValue)obj).Value, StringComparison.OrdinalIgnoreCase);
		}

		public const int MaxLength = 128;

		public const string AllowedCharacters = ".";

		public static readonly Regex ValidatingExpression = new Regex("^.+$", RegexOptions.Compiled);

		private string value;
	}
}
