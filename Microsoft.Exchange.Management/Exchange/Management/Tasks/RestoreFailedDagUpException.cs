using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RestoreFailedDagUpException : LocalizedException
	{
		public RestoreFailedDagUpException(string serverName) : base(Strings.RestoreFailedDagUp(serverName))
		{
			this.serverName = serverName;
		}

		public RestoreFailedDagUpException(string serverName, Exception innerException) : base(Strings.RestoreFailedDagUp(serverName), innerException)
		{
			this.serverName = serverName;
		}

		protected RestoreFailedDagUpException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serverName = (string)info.GetValue("serverName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serverName", this.serverName);
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		private readonly string serverName;
	}
}
