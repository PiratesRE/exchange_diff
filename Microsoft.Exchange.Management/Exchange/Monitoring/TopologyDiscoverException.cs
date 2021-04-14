using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TopologyDiscoverException : LocalizedException
	{
		public TopologyDiscoverException(string topologyDiscoverMode) : base(Strings.messageTopologyDiscoverException(topologyDiscoverMode))
		{
			this.topologyDiscoverMode = topologyDiscoverMode;
		}

		public TopologyDiscoverException(string topologyDiscoverMode, Exception innerException) : base(Strings.messageTopologyDiscoverException(topologyDiscoverMode), innerException)
		{
			this.topologyDiscoverMode = topologyDiscoverMode;
		}

		protected TopologyDiscoverException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.topologyDiscoverMode = (string)info.GetValue("topologyDiscoverMode", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("topologyDiscoverMode", this.topologyDiscoverMode);
		}

		public string TopologyDiscoverMode
		{
			get
			{
				return this.topologyDiscoverMode;
			}
		}

		private readonly string topologyDiscoverMode;
	}
}
