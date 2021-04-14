using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Hybrid
{
	[Serializable]
	internal class TaskNeedsConfigurationException : TaskExceptionBase
	{
		public TaskNeedsConfigurationException(string taskName, Exception innerException, IEnumerable<LocalizedString> errors) : base("NeedsConfiguration", taskName, innerException, errors)
		{
		}
	}
}
