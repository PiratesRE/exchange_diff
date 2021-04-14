using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FileCheckDatabaseLogfileSignatureException : FileCheckException
	{
		public FileCheckDatabaseLogfileSignatureException(string database, string logfileSignature, string expectedSignature) : base(ReplayStrings.FileCheckDatabaseLogfileSignature(database, logfileSignature, expectedSignature))
		{
			this.database = database;
			this.logfileSignature = logfileSignature;
			this.expectedSignature = expectedSignature;
		}

		public FileCheckDatabaseLogfileSignatureException(string database, string logfileSignature, string expectedSignature, Exception innerException) : base(ReplayStrings.FileCheckDatabaseLogfileSignature(database, logfileSignature, expectedSignature), innerException)
		{
			this.database = database;
			this.logfileSignature = logfileSignature;
			this.expectedSignature = expectedSignature;
		}

		protected FileCheckDatabaseLogfileSignatureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.database = (string)info.GetValue("database", typeof(string));
			this.logfileSignature = (string)info.GetValue("logfileSignature", typeof(string));
			this.expectedSignature = (string)info.GetValue("expectedSignature", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("database", this.database);
			info.AddValue("logfileSignature", this.logfileSignature);
			info.AddValue("expectedSignature", this.expectedSignature);
		}

		public string Database
		{
			get
			{
				return this.database;
			}
		}

		public string LogfileSignature
		{
			get
			{
				return this.logfileSignature;
			}
		}

		public string ExpectedSignature
		{
			get
			{
				return this.expectedSignature;
			}
		}

		private readonly string database;

		private readonly string logfileSignature;

		private readonly string expectedSignature;
	}
}
