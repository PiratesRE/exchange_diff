using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmDbNotMountedMultipleServersException : AmBcsSelectionException
	{
		public AmDbNotMountedMultipleServersException(string dbName, string detailedMsg) : base(ReplayStrings.AmDbNotMountedMultipleServersException(dbName, detailedMsg))
		{
			this.dbName = dbName;
			this.detailedMsg = detailedMsg;
		}

		public AmDbNotMountedMultipleServersException(string dbName, string detailedMsg, Exception innerException) : base(ReplayStrings.AmDbNotMountedMultipleServersException(dbName, detailedMsg), innerException)
		{
			this.dbName = dbName;
			this.detailedMsg = detailedMsg;
		}

		protected AmDbNotMountedMultipleServersException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbName = (string)info.GetValue("dbName", typeof(string));
			this.detailedMsg = (string)info.GetValue("detailedMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbName", this.dbName);
			info.AddValue("detailedMsg", this.detailedMsg);
		}

		public string DbName
		{
			get
			{
				return this.dbName;
			}
		}

		public string DetailedMsg
		{
			get
			{
				return this.detailedMsg;
			}
		}

		private readonly string dbName;

		private readonly string detailedMsg;
	}
}
