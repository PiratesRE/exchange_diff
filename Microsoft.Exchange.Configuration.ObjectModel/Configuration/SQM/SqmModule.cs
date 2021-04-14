using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Configuration.SQM
{
	internal class SqmModule : ITaskModule, ICriticalFeature
	{
		public SqmModule(TaskContext context)
		{
			this.taskContext = context;
		}

		bool ICriticalFeature.IsCriticalException(Exception ex)
		{
			return false;
		}

		public void Init(ITaskEvent task)
		{
			task.Release += this.WriteSQMData;
			task.Error += this.AppendSqmErrorRecord;
			this.ResetSQMDataRecord();
		}

		public void Dispose()
		{
		}

		private void ResetSQMDataRecord()
		{
			this.sqmStartTimeStamp = ExDateTime.Now;
			this.sqmErrors.Clear();
		}

		private void AppendSqmErrorRecord(object sender, GenericEventArg<TaskErrorEventArg> e)
		{
			if (e.Data.ExceptionHandled)
			{
				return;
			}
			if (this.sqmErrors.Count < 6)
			{
				LocalizedException ex = e.Data.Exception as LocalizedException;
				this.sqmErrors.Add(new SqmErrorRecord(e.Data.GetType().Name, (ex == null) ? "Unknown" : ex.StringId));
			}
		}

		private void WriteSQMData(object sender, EventArgs e)
		{
			TaskInvocationInfo invocationInfo = this.taskContext.InvocationInfo;
			string commandName = invocationInfo.CommandName;
			string[] array = new string[invocationInfo.UserSpecifiedParameters.Keys.Count];
			invocationInfo.UserSpecifiedParameters.Keys.CopyTo(array, 0);
			CmdletSqmSession.Instance.WriteSQMSession(commandName, (array.Length == 0) ? new string[]
			{
				"No bound parameter"
			} : array, invocationInfo.ShellHostName, (uint)(this.taskContext.CurrentObjectIndex + 1), (uint)(ExDateTime.Now - this.sqmStartTimeStamp).Milliseconds, this.sqmErrors.ToArray());
		}

		private readonly TaskContext taskContext;

		private ExDateTime sqmStartTimeStamp;

		private List<SqmErrorRecord> sqmErrors = new List<SqmErrorRecord>(6);
	}
}
