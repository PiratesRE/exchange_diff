using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class GetDagHealthInfoRequestException : LocalizedException
	{
		public GetDagHealthInfoRequestException(string serverFqdn, string errorMsg) : base(Strings.GetDagHealthInfoRequestException(serverFqdn, errorMsg))
		{
			this.serverFqdn = serverFqdn;
			this.errorMsg = errorMsg;
		}

		public GetDagHealthInfoRequestException(string serverFqdn, string errorMsg, Exception innerException) : base(Strings.GetDagHealthInfoRequestException(serverFqdn, errorMsg), innerException)
		{
			this.serverFqdn = serverFqdn;
			this.errorMsg = errorMsg;
		}

		protected GetDagHealthInfoRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serverFqdn = (string)info.GetValue("serverFqdn", typeof(string));
			this.errorMsg = (string)info.GetValue("errorMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serverFqdn", this.serverFqdn);
			info.AddValue("errorMsg", this.errorMsg);
		}

		public string ServerFqdn
		{
			get
			{
				return this.serverFqdn;
			}
		}

		public string ErrorMsg
		{
			get
			{
				return this.errorMsg;
			}
		}

		private readonly string serverFqdn;

		private readonly string errorMsg;
	}
}
