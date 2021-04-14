using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public struct DsnText : IComparable, ISerializable
	{
		public DsnText(string input)
		{
			this.value = null;
			if (this.IsValid(input))
			{
				this.value = input;
				return;
			}
			throw new ArgumentOutOfRangeException(DataStrings.DsnText, DataStrings.InvalidInputErrorMsg);
		}

		public static DsnText Parse(string s)
		{
			return new DsnText(s);
		}

		private DsnText(SerializationInfo info, StreamingContext context)
		{
			this.value = (string)info.GetValue("value", typeof(string));
			if (!this.IsValid(this.value))
			{
				throw new ArgumentOutOfRangeException(DataStrings.DsnText, this.value.ToString());
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
			return input == null || (input.Length <= 256 && DsnText.ValidatingExpression.IsMatch(input));
		}

		public static DsnText Empty
		{
			get
			{
				return default(DsnText);
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
			return obj is DsnText && this.Equals((DsnText)obj);
		}

		public bool Equals(DsnText obj)
		{
			return this.value == obj.Value;
		}

		public static bool operator ==(DsnText a, DsnText b)
		{
			return a.Value == b.Value;
		}

		public static bool operator !=(DsnText a, DsnText b)
		{
			return a.Value != b.Value;
		}

		public int CompareTo(object obj)
		{
			if (!(obj is DsnText))
			{
				throw new ArgumentException("Parameter is not of type DsnText.");
			}
			return string.Compare(this.value, ((DsnText)obj).Value, StringComparison.OrdinalIgnoreCase);
		}

		public const int MaxLength = 256;

		public const string AllowedCharacters = ".";

		public static readonly Regex ValidatingExpression = new Regex("^.+$", RegexOptions.Compiled);

		private string value;
	}
}
