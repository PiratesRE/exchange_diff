using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToDeserializeDumpsterRequestStrException : DumpsterRedeliveryException
	{
		public FailedToDeserializeDumpsterRequestStrException(string dbName, string stringToDeserialize, string typeName, string serializationError) : base(ReplayStrings.FailedToDeserializeDumpsterRequestStrException(dbName, stringToDeserialize, typeName, serializationError))
		{
			this.dbName = dbName;
			this.stringToDeserialize = stringToDeserialize;
			this.typeName = typeName;
			this.serializationError = serializationError;
		}

		public FailedToDeserializeDumpsterRequestStrException(string dbName, string stringToDeserialize, string typeName, string serializationError, Exception innerException) : base(ReplayStrings.FailedToDeserializeDumpsterRequestStrException(dbName, stringToDeserialize, typeName, serializationError), innerException)
		{
			this.dbName = dbName;
			this.stringToDeserialize = stringToDeserialize;
			this.typeName = typeName;
			this.serializationError = serializationError;
		}

		protected FailedToDeserializeDumpsterRequestStrException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbName = (string)info.GetValue("dbName", typeof(string));
			this.stringToDeserialize = (string)info.GetValue("stringToDeserialize", typeof(string));
			this.typeName = (string)info.GetValue("typeName", typeof(string));
			this.serializationError = (string)info.GetValue("serializationError", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbName", this.dbName);
			info.AddValue("stringToDeserialize", this.stringToDeserialize);
			info.AddValue("typeName", this.typeName);
			info.AddValue("serializationError", this.serializationError);
		}

		public string DbName
		{
			get
			{
				return this.dbName;
			}
		}

		public string StringToDeserialize
		{
			get
			{
				return this.stringToDeserialize;
			}
		}

		public string TypeName
		{
			get
			{
				return this.typeName;
			}
		}

		public string SerializationError
		{
			get
			{
				return this.serializationError;
			}
		}

		private readonly string dbName;

		private readonly string stringToDeserialize;

		private readonly string typeName;

		private readonly string serializationError;
	}
}
