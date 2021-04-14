using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToReadDatabaseSchemaVersionInformationForServerException : LocalizedException
	{
		public FailedToReadDatabaseSchemaVersionInformationForServerException(string serverName) : base(Strings.FailedToReadDatabaseSchemaVersionInformationForServer(serverName))
		{
			this.serverName = serverName;
		}

		public FailedToReadDatabaseSchemaVersionInformationForServerException(string serverName, Exception innerException) : base(Strings.FailedToReadDatabaseSchemaVersionInformationForServer(serverName), innerException)
		{
			this.serverName = serverName;
		}

		protected FailedToReadDatabaseSchemaVersionInformationForServerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serverName = (string)info.GetValue("serverName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serverName", this.serverName);
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		private readonly string serverName;
	}
}
