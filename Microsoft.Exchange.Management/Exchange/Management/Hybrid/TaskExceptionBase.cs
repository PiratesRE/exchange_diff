using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Hybrid
{
	[Serializable]
	internal abstract class TaskExceptionBase : LocalizedException
	{
		public TaskExceptionBase(string exceptionSubtask, string taskName, Exception innerException, IEnumerable<LocalizedString> exceptionErrors) : base(new LocalizedString(taskName), innerException)
		{
			this.Subtask = exceptionSubtask;
			this.Errors = exceptionErrors;
		}

		public IEnumerable<LocalizedString> Errors { get; private set; }

		public string Subtask { get; private set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(HybridStrings.ErrorTaskExceptionTemplate(this.Subtask, this.Message));
			if (this.Errors != null && this.Errors.Count<LocalizedString>() > 0)
			{
				stringBuilder.AppendLine();
				foreach (LocalizedString value in this.Errors)
				{
					stringBuilder.AppendLine(value);
				}
			}
			for (Exception innerException = base.InnerException; innerException != null; innerException = innerException.InnerException)
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine(innerException.Message);
				if (innerException.InnerException == null)
				{
					stringBuilder.AppendLine(innerException.StackTrace);
				}
			}
			return stringBuilder.ToString();
		}
	}
}
