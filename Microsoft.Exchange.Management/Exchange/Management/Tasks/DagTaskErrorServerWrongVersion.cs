using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskErrorServerWrongVersion : LocalizedException
	{
		public DagTaskErrorServerWrongVersion(string serverName) : base(Strings.DagTaskErrorServerWrongVersion(serverName))
		{
			this.serverName = serverName;
		}

		public DagTaskErrorServerWrongVersion(string serverName, Exception innerException) : base(Strings.DagTaskErrorServerWrongVersion(serverName), innerException)
		{
			this.serverName = serverName;
		}

		protected DagTaskErrorServerWrongVersion(SerializationInfo info, StreamingContext context) : base(info, context)
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
