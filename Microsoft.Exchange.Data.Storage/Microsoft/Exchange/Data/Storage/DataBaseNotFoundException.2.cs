using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Storage
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DataBaseNotFoundException : StorageTransientException
	{
		public DataBaseNotFoundException(Guid databaseGuid) : base(ServerStrings.DataBaseNotFoundError(databaseGuid))
		{
			this.databaseGuid = databaseGuid;
		}

		public DataBaseNotFoundException(Guid databaseGuid, Exception innerException) : base(ServerStrings.DataBaseNotFoundError(databaseGuid), innerException)
		{
			this.databaseGuid = databaseGuid;
		}

		protected DataBaseNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.databaseGuid = (Guid)info.GetValue("databaseGuid", typeof(Guid));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("databaseGuid", this.databaseGuid);
		}

		public Guid DatabaseGuid
		{
			get
			{
				return this.databaseGuid;
			}
		}

		private readonly Guid databaseGuid;
	}
}
