using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data
{
	public class ProvisioningValidationError : ValidationError
	{
		public ProvisioningValidationError(LocalizedString description, ExchangeErrorCategory errorCategory, Exception exception) : base(description)
		{
			this.errorCategory = errorCategory;
			this.exception = exception;
		}

		public ProvisioningValidationError(LocalizedString description, ExchangeErrorCategory errorCategory) : this(description, errorCategory, null)
		{
		}

		public ProvisioningValidationError(LocalizedString description, Exception exception) : this(description, ExchangeErrorCategory.Client, exception)
		{
		}

		public ProvisioningValidationError(LocalizedString description) : this(description, null)
		{
		}

		internal string AgentName
		{
			get
			{
				return this.agentName;
			}
			set
			{
				this.agentName = value;
			}
		}

		internal ExchangeErrorCategory ErrorCategory
		{
			get
			{
				return this.errorCategory;
			}
		}

		internal Exception Exception
		{
			get
			{
				return this.exception;
			}
		}

		private string agentName;

		private ExchangeErrorCategory errorCategory;

		private Exception exception;
	}
}
