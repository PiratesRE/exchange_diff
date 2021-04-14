using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Storage
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmDatabaseNotFoundException : AmServerException
	{
		public AmDatabaseNotFoundException(Guid dbGuid) : base(ServerStrings.AmDatabaseNotFoundException(dbGuid))
		{
			this.dbGuid = dbGuid;
		}

		public AmDatabaseNotFoundException(Guid dbGuid, Exception innerException) : base(ServerStrings.AmDatabaseNotFoundException(dbGuid), innerException)
		{
			this.dbGuid = dbGuid;
		}

		protected AmDatabaseNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbGuid = (Guid)info.GetValue("dbGuid", typeof(Guid));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbGuid", this.dbGuid);
		}

		public Guid DbGuid
		{
			get
			{
				return this.dbGuid;
			}
		}

		private readonly Guid dbGuid;
	}
}
