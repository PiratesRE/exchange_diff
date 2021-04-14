using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Hybrid
{
	[Serializable]
	internal class TaskConfigureException : TaskExceptionBase
	{
		public TaskConfigureException(string taskName, Exception innerException, IEnumerable<LocalizedString> errors) : base("Configure", taskName, innerException, errors)
		{
		}
	}
}
