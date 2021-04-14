using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Hybrid
{
	[Serializable]
	internal class TaskCheckPrereqsException : TaskExceptionBase
	{
		public TaskCheckPrereqsException(string taskName, Exception innerException, IEnumerable<LocalizedString> errors) : base("CheckPrereqs", taskName, innerException, errors)
		{
		}
	}
}
