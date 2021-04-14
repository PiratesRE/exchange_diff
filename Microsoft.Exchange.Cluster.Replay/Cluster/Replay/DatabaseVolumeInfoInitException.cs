using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DatabaseVolumeInfoInitException : DatabaseVolumeInfoException
	{
		public DatabaseVolumeInfoInitException(string databaseCopy, string errMsg) : base(ReplayStrings.DatabaseVolumeInfoInitException(databaseCopy, errMsg))
		{
			this.databaseCopy = databaseCopy;
			this.errMsg = errMsg;
		}

		public DatabaseVolumeInfoInitException(string databaseCopy, string errMsg, Exception innerException) : base(ReplayStrings.DatabaseVolumeInfoInitException(databaseCopy, errMsg), innerException)
		{
			this.databaseCopy = databaseCopy;
			this.errMsg = errMsg;
		}

		protected DatabaseVolumeInfoInitException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.databaseCopy = (string)info.GetValue("databaseCopy", typeof(string));
			this.errMsg = (string)info.GetValue("errMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("databaseCopy", this.databaseCopy);
			info.AddValue("errMsg", this.errMsg);
		}

		public string DatabaseCopy
		{
			get
			{
				return this.databaseCopy;
			}
		}

		public string ErrMsg
		{
			get
			{
				return this.errMsg;
			}
		}

		private readonly string databaseCopy;

		private readonly string errMsg;
	}
}
