using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SqlReportingConnectionException : LocalizedException
	{
		public SqlReportingConnectionException(string errorMsg) : base(Strings.CheckReportingServerDatabaseParameters(errorMsg))
		{
			this.errorMsg = errorMsg;
		}

		public SqlReportingConnectionException(string errorMsg, Exception innerException) : base(Strings.CheckReportingServerDatabaseParameters(errorMsg), innerException)
		{
			this.errorMsg = errorMsg;
		}

		protected SqlReportingConnectionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errorMsg = (string)info.GetValue("errorMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errorMsg", this.errorMsg);
		}

		public string ErrorMsg
		{
			get
			{
				return this.errorMsg;
			}
		}

		private readonly string errorMsg;
	}
}
