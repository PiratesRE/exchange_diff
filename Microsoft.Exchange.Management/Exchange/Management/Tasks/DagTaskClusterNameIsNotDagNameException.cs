using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskClusterNameIsNotDagNameException : LocalizedException
	{
		public DagTaskClusterNameIsNotDagNameException(string mailboxServer, string clusterName, string dagName) : base(Strings.DagTaskClusterNameIsNotDagNameException(mailboxServer, clusterName, dagName))
		{
			this.mailboxServer = mailboxServer;
			this.clusterName = clusterName;
			this.dagName = dagName;
		}

		public DagTaskClusterNameIsNotDagNameException(string mailboxServer, string clusterName, string dagName, Exception innerException) : base(Strings.DagTaskClusterNameIsNotDagNameException(mailboxServer, clusterName, dagName), innerException)
		{
			this.mailboxServer = mailboxServer;
			this.clusterName = clusterName;
			this.dagName = dagName;
		}

		protected DagTaskClusterNameIsNotDagNameException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mailboxServer = (string)info.GetValue("mailboxServer", typeof(string));
			this.clusterName = (string)info.GetValue("clusterName", typeof(string));
			this.dagName = (string)info.GetValue("dagName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mailboxServer", this.mailboxServer);
			info.AddValue("clusterName", this.clusterName);
			info.AddValue("dagName", this.dagName);
		}

		public string MailboxServer
		{
			get
			{
				return this.mailboxServer;
			}
		}

		public string ClusterName
		{
			get
			{
				return this.clusterName;
			}
		}

		public string DagName
		{
			get
			{
				return this.dagName;
			}
		}

		private readonly string mailboxServer;

		private readonly string clusterName;

		private readonly string dagName;
	}
}
