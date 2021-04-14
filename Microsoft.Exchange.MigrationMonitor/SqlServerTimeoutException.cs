using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class SqlServerTimeoutException : LocalizedException
	{
		public SqlServerTimeoutException(string sprocName) : base(MigrationMonitorStrings.ErrorSqlServerTimeout(sprocName))
		{
			this.sprocName = sprocName;
		}

		public SqlServerTimeoutException(string sprocName, Exception innerException) : base(MigrationMonitorStrings.ErrorSqlServerTimeout(sprocName), innerException)
		{
			this.sprocName = sprocName;
		}

		protected SqlServerTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.sprocName = (string)info.GetValue("sprocName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("sprocName", this.sprocName);
		}

		public string SprocName
		{
			get
			{
				return this.sprocName;
			}
		}

		private readonly string sprocName;
	}
}
