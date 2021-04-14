using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskServersInClusterNotInAd : LocalizedException
	{
		public DagTaskServersInClusterNotInAd(string serverNames) : base(Strings.DagTaskServersInClusterNotInAd(serverNames))
		{
			this.serverNames = serverNames;
		}

		public DagTaskServersInClusterNotInAd(string serverNames, Exception innerException) : base(Strings.DagTaskServersInClusterNotInAd(serverNames), innerException)
		{
			this.serverNames = serverNames;
		}

		protected DagTaskServersInClusterNotInAd(SerializationInfo info, StreamingContext context) : base(info, context)
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
