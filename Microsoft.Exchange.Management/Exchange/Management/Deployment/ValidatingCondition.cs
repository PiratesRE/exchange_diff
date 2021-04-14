using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ValidatingCondition
	{
		public ValidatingCondition(ValidationDelegate validate, LocalizedString description, bool abortValidationIfFailed)
		{
			this.validate = validate;
			this.description = description;
			this.abortValidationIfFailed = abortValidationIfFailed;
		}

		public ValidationDelegate Validate
		{
			get
			{
				return this.validate;
			}
		}

		public LocalizedString Description
		{
			get
			{
				return this.description;
			}
		}

		public bool AbortValidationIfFailed
		{
			get
			{
				return this.abortValidationIfFailed;
			}
		}

		private ValidationDelegate validate;

		private LocalizedString description;

		private readonly bool abortValidationIfFailed;
	}
}
