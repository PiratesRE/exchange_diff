using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class TaskDiagnosticInformation
	{
		public TaskDiagnosticInformation(TaskTypeId taskTypeId, ClientType clientType, Guid databaseGuid) : this(taskTypeId, clientType, databaseGuid, Guid.Empty, Guid.Empty, null, null, null)
		{
		}

		public TaskDiagnosticInformation(TaskTypeId taskTypeId, ClientType clientType, Guid databaseGuid, Guid mailboxGuid, Guid clientActivityId, string clientComponentName, string clientProtocolName, string clientActionString)
		{
			this.taskTypeId = taskTypeId;
			this.clientType = clientType;
			this.databaseGuid = databaseGuid;
			this.mailboxGuid = mailboxGuid;
			this.clientActivityId = clientActivityId;
			this.clientComponentName = clientComponentName;
			this.clientProtocolName = clientProtocolName;
			this.clientActionString = clientActionString;
		}

		public TaskTypeId TaskTypeId
		{
			get
			{
				return this.taskTypeId;
			}
		}

		public ClientType ClientType
		{
			get
			{
				return this.clientType;
			}
		}

		public Guid DatabaseGuid
		{
			get
			{
				return this.databaseGuid;
			}
		}

		public Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		public Guid ClientActivityId
		{
			get
			{
				return this.clientActivityId;
			}
		}

		public string ClientComponentName
		{
			get
			{
				return this.clientComponentName;
			}
		}

		public string ClientProtocolName
		{
			get
			{
				return this.clientProtocolName;
			}
		}

		public string ClientActionString
		{
			get
			{
				return this.clientActionString;
			}
		}

		private readonly TaskTypeId taskTypeId;

		private readonly ClientType clientType;

		private readonly Guid databaseGuid;

		private readonly Guid mailboxGuid;

		private readonly Guid clientActivityId;

		private readonly string clientComponentName;

		private readonly string clientProtocolName;

		private readonly string clientActionString;
	}
}
