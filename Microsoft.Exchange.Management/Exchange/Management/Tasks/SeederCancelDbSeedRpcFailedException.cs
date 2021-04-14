using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SeederCancelDbSeedRpcFailedException : LocalizedException
	{
		public SeederCancelDbSeedRpcFailedException(string dbName, string targetMachine, string errMessage) : base(Strings.SeederCancelDbSeedRpcFailedException(dbName, targetMachine, errMessage))
		{
			this.dbName = dbName;
			this.targetMachine = targetMachine;
			this.errMessage = errMessage;
		}

		public SeederCancelDbSeedRpcFailedException(string dbName, string targetMachine, string errMessage, Exception innerException) : base(Strings.SeederCancelDbSeedRpcFailedException(dbName, targetMachine, errMessage), innerException)
		{
			this.dbName = dbName;
			this.targetMachine = targetMachine;
			this.errMessage = errMessage;
		}

		protected SeederCancelDbSeedRpcFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbName = (string)info.GetValue("dbName", typeof(string));
			this.targetMachine = (string)info.GetValue("targetMachine", typeof(string));
			this.errMessage = (string)info.GetValue("errMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbName", this.dbName);
			info.AddValue("targetMachine", this.targetMachine);
			info.AddValue("errMessage", this.errMessage);
		}

		public string DbName
		{
			get
			{
				return this.dbName;
			}
		}

		public string TargetMachine
		{
			get
			{
				return this.targetMachine;
			}
		}

		public string ErrMessage
		{
			get
			{
				return this.errMessage;
			}
		}

		private readonly string dbName;

		private readonly string targetMachine;

		private readonly string errMessage;
	}
}
