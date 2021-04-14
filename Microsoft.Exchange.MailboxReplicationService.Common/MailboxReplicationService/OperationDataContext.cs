using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class OperationDataContext : DataContext
	{
		public OperationDataContext(string operationName, OperationType operationType = OperationType.None)
		{
			this.OperationName = operationName;
			this.OperationType = operationType;
		}

		public string OperationName { get; private set; }

		public OperationType OperationType { get; private set; }

		public override string ToString()
		{
			if (this.OperationType != OperationType.None)
			{
				return string.Format("Operation: [{0}] {1}", this.OperationType, this.OperationName);
			}
			return string.Format("Operation: {0}", this.OperationName);
		}
	}
}
