using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToFindDatabaseException : TransientException
	{
		public FailedToFindDatabaseException(string databaseName) : base(ReplayStrings.FailedToFindDatabaseException(databaseName))
		{
			this.databaseName = databaseName;
		}

		public FailedToFindDatabaseException(string databaseName, Exception innerException) : base(ReplayStrings.FailedToFindDatabaseException(databaseName), innerException)
		{
			this.databaseName = databaseName;
		}

		protected FailedToFindDatabaseException(SerializationInfo info, StreamingContext context) : base(info, context)
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
