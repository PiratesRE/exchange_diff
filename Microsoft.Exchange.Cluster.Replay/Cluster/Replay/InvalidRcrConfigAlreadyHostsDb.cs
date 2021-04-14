using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidRcrConfigAlreadyHostsDb : TransientException
	{
		public InvalidRcrConfigAlreadyHostsDb(string nodeName, string dbName) : base(ReplayStrings.InvalidRcrConfigAlreadyHostsDb(nodeName, dbName))
		{
			this.nodeName = nodeName;
			this.dbName = dbName;
		}

		public InvalidRcrConfigAlreadyHostsDb(string nodeName, string dbName, Exception innerException) : base(ReplayStrings.InvalidRcrConfigAlreadyHostsDb(nodeName, dbName), innerException)
		{
			this.nodeName = nodeName;
			this.dbName = dbName;
		}

		protected InvalidRcrConfigAlreadyHostsDb(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.nodeName = (string)info.GetValue("nodeName", typeof(string));
			this.dbName = (string)info.GetValue("dbName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("nodeName", this.nodeName);
			info.AddValue("dbName", this.dbName);
		}

		public string NodeName
		{
			get
			{
				return this.nodeName;
			}
		}

		public string DbName
		{
			get
			{
				return this.dbName;
			}
		}

		private readonly string nodeName;

		private readonly string dbName;
	}
}
