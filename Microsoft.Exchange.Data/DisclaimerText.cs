using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public struct DisclaimerText : IComparable, ISerializable
	{
		public DisclaimerText(string input)
		{
			this.value = null;
			if (this.IsValid(input))
			{
				this.value = input;
				return;
			}
			throw new ArgumentOutOfRangeException(DataStrings.DisclaimerText, DataStrings.InvalidInputErrorMsg);
		}

		public static DisclaimerText Parse(string s)
		{
			return new DisclaimerText(s);
		}

		private DisclaimerText(SerializationInfo info, StreamingContext context)
		{
			this.value = (string)info.GetValue("value", typeof(string));
			if (!this.IsValid(this.value))
			{
				throw new ArgumentOutOfRangeException(DataStrings.DisclaimerText, this.value.ToString());
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
			return input == null || (input.Length <= 5120 && DisclaimerText.ValidatingExpression.IsMatch(input));
		}

		public static DisclaimerText Empty
		{
			get
			{
				return default(DisclaimerText);
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
			return obj is DisclaimerText && this.Equals((DisclaimerText)obj);
		}

		public bool Equals(DisclaimerText obj)
		{
			return this.value == obj.Value;
		}

		public static bool operator ==(DisclaimerText a, DisclaimerText b)
		{
			return a.Value == b.Value;
		}

		public static bool operator !=(DisclaimerText a, DisclaimerText b)
		{
			return a.Value != b.Value;
		}

		public int CompareTo(object obj)
		{
			if (!(obj is DisclaimerText))
			{
				throw new ArgumentException("Parameter is not of type DisclaimerText.");
			}
			return string.Compare(this.value, ((DisclaimerText)obj).Value, StringComparison.OrdinalIgnoreCase);
		}

		public const int MaxLength = 5120;

		public const string AllowedCharacters = "(.|[^.])";

		public static readonly Regex ValidatingExpression = new Regex("^(.|[^.])+$", RegexOptions.Compiled);

		private string value;
	}
}
