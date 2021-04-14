using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskServerIPAddressSameAsStaticIPAddressException : LocalizedException
	{
		public DagTaskServerIPAddressSameAsStaticIPAddressException(string serverName, string conflict, string dag) : base(Strings.DagTaskServerIPAddressSameAsStaticIPAddress(serverName, conflict, dag))
		{
			this.serverName = serverName;
			this.conflict = conflict;
			this.dag = dag;
		}

		public DagTaskServerIPAddressSameAsStaticIPAddressException(string serverName, string conflict, string dag, Exception innerException) : base(Strings.DagTaskServerIPAddressSameAsStaticIPAddress(serverName, conflict, dag), innerException)
		{
			this.serverName = serverName;
			this.conflict = conflict;
			this.dag = dag;
		}

		protected DagTaskServerIPAddressSameAsStaticIPAddressException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serverName = (string)info.GetValue("serverName", typeof(string));
			this.conflict = (string)info.GetValue("conflict", typeof(string));
			this.dag = (string)info.GetValue("dag", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serverName", this.serverName);
			info.AddValue("conflict", this.conflict);
			info.AddValue("dag", this.dag);
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		public string Conflict
		{
			get
			{
				return this.conflict;
			}
		}

		public string Dag
		{
			get
			{
				return this.dag;
			}
		}

		private readonly string serverName;

		private readonly string conflict;

		private readonly string dag;
	}
}
