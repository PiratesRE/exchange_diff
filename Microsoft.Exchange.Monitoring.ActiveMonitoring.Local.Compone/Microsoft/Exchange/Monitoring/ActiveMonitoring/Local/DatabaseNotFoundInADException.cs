using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DatabaseNotFoundInADException : RecoveryActionExceptionCommon
	{
		public DatabaseNotFoundInADException(string databaseGuid) : base(Strings.DatabaseNotFoundInADException(databaseGuid))
		{
			this.databaseGuid = databaseGuid;
		}

		public DatabaseNotFoundInADException(string databaseGuid, Exception innerException) : base(Strings.DatabaseNotFoundInADException(databaseGuid), innerException)
		{
			this.databaseGuid = databaseGuid;
		}

		protected DatabaseNotFoundInADException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.databaseGuid = (string)info.GetValue("databaseGuid", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("databaseGuid", this.databaseGuid);
		}

		public string DatabaseGuid
		{
			get
			{
				return this.databaseGuid;
			}
		}

		private readonly string databaseGuid;
	}
}
