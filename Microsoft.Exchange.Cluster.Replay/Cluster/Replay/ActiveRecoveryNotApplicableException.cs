﻿using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ActiveRecoveryNotApplicableException : LocalizedException
	{
		public ActiveRecoveryNotApplicableException(string dbName) : base(ReplayStrings.ActiveRecoveryNotApplicableException(dbName))
		{
			this.dbName = dbName;
		}

		public ActiveRecoveryNotApplicableException(string dbName, Exception innerException) : base(ReplayStrings.ActiveRecoveryNotApplicableException(dbName), innerException)
		{
			this.dbName = dbName;
		}

		protected ActiveRecoveryNotApplicableException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbName = (string)info.GetValue("dbName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbName", this.dbName);
		}

		public string DbName
		{
			get
			{
				return this.dbName;
			}
		}

		private readonly string dbName;
	}
}
