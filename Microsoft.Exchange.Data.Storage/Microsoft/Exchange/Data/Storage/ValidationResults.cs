using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ValidationResults
	{
		public ValidationResults(ValidationResult result, string reason)
		{
			EnumValidator.ThrowIfInvalid<ValidationResult>(result, "result");
			this.result = result;
			this.reason = reason;
		}

		public ValidationResult Result
		{
			get
			{
				return this.result;
			}
		}

		public string Reason
		{
			get
			{
				return this.reason;
			}
		}

		public override string ToString()
		{
			return "Validation result=" + this.result.ToString() + ", reason=" + this.reason;
		}

		private ValidationResults(ValidationResult result)
		{
			this.result = result;
			this.reason = null;
		}

		internal static readonly ValidationResults Success = new ValidationResults(ValidationResult.Success);

		private ValidationResult result;

		private string reason;
	}
}
