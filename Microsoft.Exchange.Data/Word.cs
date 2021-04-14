using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public struct Word : IComparable, ISerializable
	{
		public Word(string input)
		{
			this.value = null;
			if (this.IsValid(input))
			{
				this.value = input;
				return;
			}
			throw new ArgumentOutOfRangeException(DataStrings.Word, DataStrings.InvalidInputErrorMsg);
		}

		public static Word Parse(string s)
		{
			return new Word(s);
		}

		private Word(SerializationInfo info, StreamingContext context)
		{
			this.value = (string)info.GetValue("value", typeof(string));
			if (!this.IsValid(this.value))
			{
				throw new ArgumentOutOfRangeException(DataStrings.Word, this.value.ToString());
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
			return input == null || (input.Length <= 128 && Word.ValidatingExpression.IsMatch(input));
		}

		public static Word Empty
		{
			get
			{
				return default(Word);
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
			return obj is Word && this.Equals((Word)obj);
		}

		public bool Equals(Word obj)
		{
			return this.value == obj.Value;
		}

		public static bool operator ==(Word a, Word b)
		{
			return a.Value == b.Value;
		}

		public static bool operator !=(Word a, Word b)
		{
			return a.Value != b.Value;
		}

		public int CompareTo(object obj)
		{
			if (!(obj is Word))
			{
				throw new ArgumentException("Parameter is not of type Word.");
			}
			return string.Compare(this.value, ((Word)obj).Value, StringComparison.OrdinalIgnoreCase);
		}

		public const int MaxLength = 128;

		public const string AllowedCharacters = ".";

		public static readonly Regex ValidatingExpression = new Regex("^.+$", RegexOptions.Compiled);

		private string value;
	}
}
