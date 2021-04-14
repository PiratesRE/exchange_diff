using System;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PhoneNumber : IEquatable<PhoneNumber>
	{
		public string Number
		{
			get
			{
				return this.number;
			}
			set
			{
				this.number = value;
			}
		}

		public PersonPhoneNumberType Type
		{
			get
			{
				return this.type;
			}
			set
			{
				EnumValidator.ThrowIfInvalid<PersonPhoneNumberType>(value, "value");
				this.type = value;
			}
		}

		public bool Equals(PhoneNumber other)
		{
			if (other == null)
			{
				return false;
			}
			string numericPartFromText = this.GetNumericPartFromText(this.Number);
			string numericPartFromText2 = this.GetNumericPartFromText(other.Number);
			return string.Equals(numericPartFromText, numericPartFromText2, StringComparison.OrdinalIgnoreCase);
		}

		public override bool Equals(object other)
		{
			return this.Equals(other as PhoneNumber);
		}

		public override int GetHashCode()
		{
			if (string.IsNullOrEmpty(this.number))
			{
				return 0;
			}
			return this.number.GetHashCode();
		}

		private string GetNumericPartFromText(string text)
		{
			if (text == null)
			{
				return string.Empty;
			}
			return string.Join(null, Regex.Split(text, "[^\\d]"));
		}

		private const string NumericRegEx = "[^\\d]";

		private string number;

		private PersonPhoneNumberType type;
	}
}
