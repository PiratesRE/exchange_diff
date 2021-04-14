using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class DatabaseNotLocalException : MailboxLoadBalancePermanentException
	{
		public DatabaseNotLocalException(string databaseName, string edbPath) : base(MigrationWorkflowServiceStrings.ErrorDatabaseNotLocal(databaseName, edbPath))
		{
			this.databaseName = databaseName;
			this.edbPath = edbPath;
		}

		public DatabaseNotLocalException(string databaseName, string edbPath, Exception innerException) : base(MigrationWorkflowServiceStrings.ErrorDatabaseNotLocal(databaseName, edbPath), innerException)
		{
			this.databaseName = databaseName;
			this.edbPath = edbPath;
		}

		protected DatabaseNotLocalException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.databaseName = (string)info.GetValue("databaseName", typeof(string));
			this.edbPath = (string)info.GetValue("edbPath", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("databaseName", this.databaseName);
			info.AddValue("edbPath", this.edbPath);
		}

		public string DatabaseName
		{
			get
			{
				return this.databaseName;
			}
		}

		public string EdbPath
		{
			get
			{
				return this.edbPath;
			}
		}

		private readonly string databaseName;

		private readonly string edbPath;
	}
}
