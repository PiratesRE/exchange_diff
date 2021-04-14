using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmMountCallbackFailedWithDBFolderNotUnderMountPointException : AmServerException
	{
		public AmMountCallbackFailedWithDBFolderNotUnderMountPointException(string dbName, string error) : base(ReplayStrings.AmMountCallbackFailedWithDBFolderNotUnderMountPointException(dbName, error))
		{
			this.dbName = dbName;
			this.error = error;
		}

		public AmMountCallbackFailedWithDBFolderNotUnderMountPointException(string dbName, string error, Exception innerException) : base(ReplayStrings.AmMountCallbackFailedWithDBFolderNotUnderMountPointException(dbName, error), innerException)
		{
			this.dbName = dbName;
			this.error = error;
		}

		protected AmMountCallbackFailedWithDBFolderNotUnderMountPointException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbName = (string)info.GetValue("dbName", typeof(string));
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbName", this.dbName);
			info.AddValue("error", this.error);
		}

		public string DbName
		{
			get
			{
				return this.dbName;
			}
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string dbName;

		private readonly string error;
	}
}
