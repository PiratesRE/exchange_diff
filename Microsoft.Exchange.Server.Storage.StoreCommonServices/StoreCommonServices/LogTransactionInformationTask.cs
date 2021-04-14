using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	internal class LogTransactionInformationTask : ILogTransactionInformation
	{
		public LogTransactionInformationTask()
		{
		}

		public LogTransactionInformationTask(TaskTypeId taskTypeId)
		{
			this.taskTypeId = taskTypeId;
		}

		public TaskTypeId TaskTypeId
		{
			get
			{
				return this.taskTypeId;
			}
		}

		public override string ToString()
		{
			return string.Format("Task:\nIdentifier: {0}\n", this.taskTypeId);
		}

		public byte Type()
		{
			return 5;
		}

		public int Serialize(byte[] buffer, int offset)
		{
			int num = offset;
			if (buffer != null)
			{
				buffer[offset] = this.Type();
			}
			offset++;
			if (buffer != null)
			{
				buffer[offset] = (byte)this.taskTypeId;
			}
			offset++;
			return offset - num;
		}

		public void Parse(byte[] buffer, ref int offset)
		{
			byte b = buffer[offset++];
			this.taskTypeId = (TaskTypeId)buffer[offset++];
		}

		private TaskTypeId taskTypeId;
	}
}
