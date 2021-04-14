using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public struct RejectText : IComparable, ISerializable
	{
		public RejectText(string input)
		{
			this.value = null;
			if (this.IsValid(input))
			{
				this.value = input;
				return;
			}
			throw new ArgumentOutOfRangeException(DataStrings.RejectText, DataStrings.InvalidInputErrorMsg);
		}

		public static RejectText Parse(string s)
		{
			return new RejectText(s);
		}

		private RejectText(SerializationInfo info, StreamingContext context)
		{
			this.value = (string)info.GetValue("value", typeof(string));
			if (!this.IsValid(this.value))
			{
				throw new ArgumentOutOfRangeException(DataStrings.RejectText, this.value.ToString());
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
			return input == null || (input.Length <= 128 && RejectText.ValidatingExpression.IsMatch(input));
		}

		public static RejectText Empty
		{
			get
			{
				return default(RejectText);
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
			return obj is RejectText && this.Equals((RejectText)obj);
		}

		public bool Equals(RejectText obj)
		{
			return this.value == obj.Value;
		}

		public static bool operator ==(RejectText a, RejectText b)
		{
			return a.Value == b.Value;
		}

		public static bool operator !=(RejectText a, RejectText b)
		{
			return a.Value != b.Value;
		}

		public int CompareTo(object obj)
		{
			if (!(obj is RejectText))
			{
				throw new ArgumentException("Parameter is not of type RejectText.");
			}
			return string.Compare(this.value, ((RejectText)obj).Value, StringComparison.OrdinalIgnoreCase);
		}

		public const int MaxLength = 128;

		public const string AllowedCharacters = "[\\x20-\\x7e]";

		public static readonly Regex ValidatingExpression = new Regex("^[\\x20-\\x7e]+$", RegexOptions.Compiled);

		private string value;
	}
}
