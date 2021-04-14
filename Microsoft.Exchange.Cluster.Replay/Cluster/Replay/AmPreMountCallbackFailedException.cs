using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmPreMountCallbackFailedException : TransientException
	{
		public AmPreMountCallbackFailedException(string dbName, string error) : base(ReplayStrings.AmPreMountCallbackFailedException(dbName, error))
		{
			this.dbName = dbName;
			this.error = error;
		}

		public AmPreMountCallbackFailedException(string dbName, string error, Exception innerException) : base(ReplayStrings.AmPreMountCallbackFailedException(dbName, error), innerException)
		{
			this.dbName = dbName;
			this.error = error;
		}

		protected AmPreMountCallbackFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
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
