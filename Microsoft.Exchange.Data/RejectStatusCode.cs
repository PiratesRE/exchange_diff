using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public struct RejectStatusCode : IComparable, ISerializable
	{
		public RejectStatusCode(string input)
		{
			this.value = null;
			if (this.IsValid(input))
			{
				this.value = input;
				return;
			}
			throw new ArgumentOutOfRangeException(DataStrings.RejectStatusCode, DataStrings.InvalidInputErrorMsg);
		}

		public static RejectStatusCode Parse(string s)
		{
			return new RejectStatusCode(s);
		}

		private RejectStatusCode(SerializationInfo info, StreamingContext context)
		{
			this.value = (string)info.GetValue("value", typeof(string));
			if (!this.IsValid(this.value))
			{
				throw new ArgumentOutOfRangeException(DataStrings.RejectStatusCode, this.value.ToString());
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
			return input == null || (input.Length <= 3 && RejectStatusCode.ValidatingExpression.IsMatch(input));
		}

		public static RejectStatusCode Empty
		{
			get
			{
				return default(RejectStatusCode);
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
			return obj is RejectStatusCode && this.Equals((RejectStatusCode)obj);
		}

		public bool Equals(RejectStatusCode obj)
		{
			return this.value == obj.Value;
		}

		public static bool operator ==(RejectStatusCode a, RejectStatusCode b)
		{
			return a.Value == b.Value;
		}

		public static bool operator !=(RejectStatusCode a, RejectStatusCode b)
		{
			return a.Value != b.Value;
		}

		public int CompareTo(object obj)
		{
			if (!(obj is RejectStatusCode))
			{
				throw new ArgumentException("Parameter is not of type RejectStatusCode.");
			}
			return string.Compare(this.value, ((RejectStatusCode)obj).Value, StringComparison.OrdinalIgnoreCase);
		}

		public const int MaxLength = 3;

		public const string AllowedCharacters = "[0-9]";

		public static readonly Regex ValidatingExpression = new Regex("[4-5][0-9][0-9]", RegexOptions.Compiled);

		private string value;
	}
}
