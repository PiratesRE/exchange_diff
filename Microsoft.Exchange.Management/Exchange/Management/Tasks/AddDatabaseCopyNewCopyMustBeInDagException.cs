using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AddDatabaseCopyNewCopyMustBeInDagException : LocalizedException
	{
		public AddDatabaseCopyNewCopyMustBeInDagException(string serverName, string databaseName) : base(Strings.AddDatabaseCopyNewCopyMustBeInDagException(serverName, databaseName))
		{
			this.serverName = serverName;
			this.databaseName = databaseName;
		}

		public AddDatabaseCopyNewCopyMustBeInDagException(string serverName, string databaseName, Exception innerException) : base(Strings.AddDatabaseCopyNewCopyMustBeInDagException(serverName, databaseName), innerException)
		{
			this.serverName = serverName;
			this.databaseName = databaseName;
		}

		protected AddDatabaseCopyNewCopyMustBeInDagException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serverName = (string)info.GetValue("serverName", typeof(string));
			this.databaseName = (string)info.GetValue("databaseName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serverName", this.serverName);
			info.AddValue("databaseName", this.databaseName);
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		public string DatabaseName
		{
			get
			{
				return this.databaseName;
			}
		}

		private readonly string serverName;

		private readonly string databaseName;
	}
}
