using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public struct SubjectPrefix : IComparable, ISerializable
	{
		public SubjectPrefix(string input)
		{
			this.value = null;
			if (this.IsValid(input))
			{
				this.value = input;
				return;
			}
			throw new ArgumentOutOfRangeException(DataStrings.SubjectPrefix, DataStrings.InvalidInputErrorMsg);
		}

		public static SubjectPrefix Parse(string s)
		{
			return new SubjectPrefix(s);
		}

		private SubjectPrefix(SerializationInfo info, StreamingContext context)
		{
			this.value = (string)info.GetValue("value", typeof(string));
			if (!this.IsValid(this.value))
			{
				throw new ArgumentOutOfRangeException(DataStrings.SubjectPrefix, this.value.ToString());
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
			return input == null || (input.Length <= 32 && SubjectPrefix.ValidatingExpression.IsMatch(input));
		}

		public static SubjectPrefix Empty
		{
			get
			{
				return default(SubjectPrefix);
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
			return obj is SubjectPrefix && this.Equals((SubjectPrefix)obj);
		}

		public bool Equals(SubjectPrefix obj)
		{
			return this.value == obj.Value;
		}

		public static bool operator ==(SubjectPrefix a, SubjectPrefix b)
		{
			return a.Value == b.Value;
		}

		public static bool operator !=(SubjectPrefix a, SubjectPrefix b)
		{
			return a.Value != b.Value;
		}

		public int CompareTo(object obj)
		{
			if (!(obj is SubjectPrefix))
			{
				throw new ArgumentException("Parameter is not of type SubjectPrefix.");
			}
			return string.Compare(this.value, ((SubjectPrefix)obj).Value, StringComparison.OrdinalIgnoreCase);
		}

		public const int MaxLength = 32;

		public const string AllowedCharacters = ".";

		public static readonly Regex ValidatingExpression = new Regex("^.+$", RegexOptions.Compiled);

		private string value;
	}
}
