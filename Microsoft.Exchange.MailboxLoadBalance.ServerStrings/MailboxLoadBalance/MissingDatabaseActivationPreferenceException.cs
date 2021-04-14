using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MissingDatabaseActivationPreferenceException : MailboxLoadBalancePermanentException
	{
		public MissingDatabaseActivationPreferenceException(string databaseName) : base(MigrationWorkflowServiceStrings.ErrorMissingDatabaseActivationPreference(databaseName))
		{
			this.databaseName = databaseName;
		}

		public MissingDatabaseActivationPreferenceException(string databaseName, Exception innerException) : base(MigrationWorkflowServiceStrings.ErrorMissingDatabaseActivationPreference(databaseName), innerException)
		{
			this.databaseName = databaseName;
		}

		protected MissingDatabaseActivationPreferenceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.databaseName = (string)info.GetValue("databaseName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("databaseName", this.databaseName);
		}

		public string DatabaseName
		{
			get
			{
				return this.databaseName;
			}
		}

		private readonly string databaseName;
	}
}
