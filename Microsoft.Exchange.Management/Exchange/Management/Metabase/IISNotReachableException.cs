using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Metabase
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IISNotReachableException : DataSourceTransientException
	{
		public IISNotReachableException(string serverName, string errorMsg) : base(Strings.IISNotReachableException(serverName, errorMsg))
		{
			this.serverName = serverName;
			this.errorMsg = errorMsg;
		}

		public IISNotReachableException(string serverName, string errorMsg, Exception innerException) : base(Strings.IISNotReachableException(serverName, errorMsg), innerException)
		{
			this.serverName = serverName;
			this.errorMsg = errorMsg;
		}

		protected IISNotReachableException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serverName = (string)info.GetValue("serverName", typeof(string));
			this.errorMsg = (string)info.GetValue("errorMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serverName", this.serverName);
			info.AddValue("errorMsg", this.errorMsg);
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		public string ErrorMsg
		{
			get
			{
				return this.errorMsg;
			}
		}

		private readonly string serverName;

		private readonly string errorMsg;
	}
}
