using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.UpdatableHelp
{
	[Serializable]
	internal class UpdatableExchangeHelpSystemException : LocalizedException
	{
		internal UpdatableExchangeHelpSystemException(LocalizedString errorId, LocalizedString message, ErrorCategory cat, object targetObject, Exception innerException) : base(message, innerException)
		{
			this.FullyQualifiedErrorId = errorId;
			this.ErrorCategory = cat;
			this.TargetObject = targetObject;
		}

		internal string FullyQualifiedErrorId { get; private set; }

		internal ErrorCategory ErrorCategory { get; private set; }

		internal object TargetObject { get; private set; }

		internal ErrorRecord CreateErrorRecord()
		{
			return new ErrorRecord(new Exception(this.Message), this.FullyQualifiedErrorId, this.ErrorCategory, this.TargetObject);
		}
	}
}
