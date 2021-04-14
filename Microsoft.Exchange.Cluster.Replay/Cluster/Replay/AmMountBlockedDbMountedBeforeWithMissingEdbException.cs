using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmMountBlockedDbMountedBeforeWithMissingEdbException : AmServerException
	{
		public AmMountBlockedDbMountedBeforeWithMissingEdbException(string dbName, string edbFilePath) : base(ReplayStrings.AmMountBlockedDbMountedBeforeWithMissingEdbException(dbName, edbFilePath))
		{
			this.dbName = dbName;
			this.edbFilePath = edbFilePath;
		}

		public AmMountBlockedDbMountedBeforeWithMissingEdbException(string dbName, string edbFilePath, Exception innerException) : base(ReplayStrings.AmMountBlockedDbMountedBeforeWithMissingEdbException(dbName, edbFilePath), innerException)
		{
			this.dbName = dbName;
			this.edbFilePath = edbFilePath;
		}

		protected AmMountBlockedDbMountedBeforeWithMissingEdbException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbName = (string)info.GetValue("dbName", typeof(string));
			this.edbFilePath = (string)info.GetValue("edbFilePath", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbName", this.dbName);
			info.AddValue("edbFilePath", this.edbFilePath);
		}

		public string DbName
		{
			get
			{
				return this.dbName;
			}
		}

		public string EdbFilePath
		{
			get
			{
				return this.edbFilePath;
			}
		}

		private readonly string dbName;

		private readonly string edbFilePath;
	}
}
