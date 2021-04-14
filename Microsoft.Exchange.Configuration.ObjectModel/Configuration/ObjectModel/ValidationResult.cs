using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;

namespace Microsoft.Exchange.Configuration.ObjectModel
{
	internal class ValidationResult : Dictionary<string, string>
	{
		public ValidationResult()
		{
		}

		public ValidationResult(string mess)
		{
			this.errorMessage = mess;
		}

		public string ErrorMessage
		{
			get
			{
				string text = this.errorMessage;
				if (!this.Valid && string.IsNullOrEmpty(text))
				{
					text = Strings.InvalidProperties;
				}
				return text;
			}
			set
			{
				this.errorMessage = value;
				this.valid &= string.IsNullOrEmpty(this.errorMessage);
			}
		}

		public bool Valid
		{
			get
			{
				return this.valid;
			}
			set
			{
				this.valid = value;
			}
		}

		public new string this[string PropertyID]
		{
			get
			{
				string result = "";
				base.TryGetValue(PropertyID, out result);
				return result;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					base.Add(PropertyID, value);
					this.valid = false;
				}
			}
		}

		private string errorMessage = "";

		private bool valid = true;
	}
}
