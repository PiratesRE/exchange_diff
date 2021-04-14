using System;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal abstract class DialGroupEntryField
	{
		protected DialGroupEntryField(string data, string fieldName)
		{
			this.data = data;
			this.fieldName = fieldName;
			this.Validate();
		}

		protected string Data
		{
			get
			{
				return this.data;
			}
		}

		public override string ToString()
		{
			return this.Data;
		}

		protected abstract void Validate();

		protected void ValidateNullOrEmpty()
		{
			if (string.IsNullOrEmpty(this.Data))
			{
				throw new ArgumentNullException(this.fieldName);
			}
		}

		protected void ValidateMaxLength(int maxLength)
		{
			if (this.Data.Length > maxLength)
			{
				throw new FormatException(DataStrings.InvalidDialGroupEntryElementLength(this.fieldName, this.Data, maxLength));
			}
		}

		protected void ValidateRegex(Regex regex)
		{
			if (!regex.IsMatch(this.Data))
			{
				throw new FormatException(DataStrings.InvalidDialGroupEntryElementFormat(this.fieldName));
			}
		}

		private string data;

		private string fieldName;
	}
}
