using System;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.ObjectModel
{
	internal enum ComplianceBindingErrorType
	{
		NoError,
		BindingNotFound,
		AmbiguousBinding,
		InvalidRecipientType
	}
}
