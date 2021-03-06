using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DatabaseValidationNoCopiesException : DatabaseValidationException
	{
		public DatabaseValidationNoCopiesException(string databaseName) : base(ReplayStrings.DatabaseValidationNoCopiesException(databaseName))
		{
			this.databaseName = databaseName;
		}

		public DatabaseValidationNoCopiesException(string databaseName, Exception innerException) : base(ReplayStrings.DatabaseValidationNoCopiesException(databaseName), innerException)
		{
			this.databaseName = databaseName;
		}

		protected DatabaseValidationNoCopiesException(SerializationInfo info, StreamingContext context) : base(info, context)
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
