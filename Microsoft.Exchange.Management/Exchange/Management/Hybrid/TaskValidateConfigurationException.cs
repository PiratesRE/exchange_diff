using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Hybrid
{
	[Serializable]
	internal class TaskValidateConfigurationException : TaskExceptionBase
	{
		public TaskValidateConfigurationException(string taskName, Exception innerException, IEnumerable<LocalizedString> errors) : base("ValidateConfiguration", taskName, innerException, errors)
		{
		}
	}
}
