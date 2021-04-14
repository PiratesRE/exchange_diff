using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskClusteringShouldBeDisabledException : LocalizedException
	{
		public DagTaskClusteringShouldBeDisabledException(string serverName) : base(Strings.DagTaskClusteringShouldBeDisabledException(serverName))
		{
			this.serverName = serverName;
		}

		public DagTaskClusteringShouldBeDisabledException(string serverName, Exception innerException) : base(Strings.DagTaskClusteringShouldBeDisabledException(serverName), innerException)
		{
			this.serverName = serverName;
		}

		protected DagTaskClusteringShouldBeDisabledException(SerializationInfo info, StreamingContext context) : base(info, context)
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
