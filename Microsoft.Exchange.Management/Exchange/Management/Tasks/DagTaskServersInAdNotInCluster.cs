using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskServersInAdNotInCluster : LocalizedException
	{
		public DagTaskServersInAdNotInCluster(string serverNames) : base(Strings.DagTaskServersInAdNotInCluster(serverNames))
		{
			this.serverNames = serverNames;
		}

		public DagTaskServersInAdNotInCluster(string serverNames, Exception innerException) : base(Strings.DagTaskServersInAdNotInCluster(serverNames), innerException)
		{
			this.serverNames = serverNames;
		}

		protected DagTaskServersInAdNotInCluster(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serverNames = (string)info.GetValue("serverNames", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serverNames", this.serverNames);
		}

		public string ServerNames
		{
			get
			{
				return this.serverNames;
			}
		}

		private readonly string serverNames;
	}
}
