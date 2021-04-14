using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.ProvisioningMonitoring;

namespace Microsoft.Exchange.Servicelets.Provisioning
{
	internal sealed class Error
	{
		public Error(Exception exception) : this(exception, null)
		{
		}

		public Error(Exception exception, string errorMessage) : this(exception, null, null)
		{
			this.errorMessage = errorMessage;
			this.errorRecord = new ErrorRecord(exception, string.Empty, ErrorCategory.NotSpecified, null);
		}

		public Error(Exception exception, string errorMessage, string cmdletName)
		{
			this.errorMessage = errorMessage;
			this.errorRecord = new ErrorRecord(exception, string.Empty, ErrorCategory.NotSpecified, null);
			this.cmdletName = cmdletName;
		}

		public Error(ErrorRecord errorRecord, string cmdletName)
		{
			this.errorRecord = errorRecord;
			this.cmdletName = cmdletName;
		}

		public bool IsUserInputError
		{
			get
			{
				return (string.IsNullOrEmpty(this.cmdletName) ? (this.errorRecord.Exception is RecipientTaskException) : ProvisioningMonitoringConfig.IsExceptionWhiteListedForCmdlet(this.errorRecord, this.cmdletName)) || this.IsLegacyUserInputError();
			}
		}

		public Exception Exception
		{
			get
			{
				return this.errorRecord.Exception;
			}
		}

		public string Message
		{
			get
			{
				return this.errorMessage ?? this.errorRecord.Exception.Message;
			}
			set
			{
				this.errorMessage = value;
			}
		}

		public override string ToString()
		{
			return this.errorRecord.Exception.ToString();
		}

		private bool IsLegacyUserInputError()
		{
			bool flag = false;
			if (this.errorRecord != null)
			{
				flag = (this.errorRecord.Exception is InvalidOperationException || this.errorRecord.Exception is ProcessingException || this.errorRecord.Exception is ManagementObjectNotFoundException || this.errorRecord.Exception is DataValidationException || this.errorRecord.Exception is ParameterBindingException || (this.errorRecord.Exception.InnerException != null && this.errorRecord.Exception.InnerException.InnerException != null && this.errorRecord.Exception.InnerException.InnerException is PSInvalidCastException));
				if (!flag && !string.IsNullOrEmpty(this.cmdletName))
				{
					if (string.Equals("New-DistributionGroup", this.cmdletName, StringComparison.OrdinalIgnoreCase) || string.Equals("New-MailContact", this.cmdletName, StringComparison.OrdinalIgnoreCase) || string.Equals("New-MailUser", this.cmdletName, StringComparison.OrdinalIgnoreCase))
					{
						flag = (this.errorRecord.Exception is ManagementObjectAmbiguousException || this.errorRecord.Exception is ADObjectAlreadyExistsException || this.errorRecord.Exception is RecipientTaskException);
					}
					else if (string.Equals("Set-Contact", this.cmdletName, StringComparison.OrdinalIgnoreCase) || string.Equals("set-user", this.cmdletName, StringComparison.OrdinalIgnoreCase) || string.Equals("Set-MailContact", this.cmdletName, StringComparison.OrdinalIgnoreCase) || string.Equals("Set-MailUser", this.cmdletName, StringComparison.OrdinalIgnoreCase) || string.Equals("Set-DistributionGroup", this.cmdletName, StringComparison.OrdinalIgnoreCase))
					{
						flag = (this.errorRecord.Exception is ManagementObjectAmbiguousException || this.errorRecord.Exception is ADObjectAlreadyExistsException || this.errorRecord.Exception is UserWithMatchingWindowsLiveIdExistsException || this.errorRecord.Exception is RecipientTaskException);
					}
					else if (string.Equals("Update-DistributionGroupMember", this.cmdletName, StringComparison.OrdinalIgnoreCase))
					{
						flag = (this.errorRecord.Exception is ManagementObjectAmbiguousException || this.errorRecord.Exception is RecipientTaskException);
					}
				}
			}
			return flag;
		}

		private string errorMessage;

		private string cmdletName;

		private ErrorRecord errorRecord;
	}
}
