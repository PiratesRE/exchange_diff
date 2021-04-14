using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class NetworkNodeEndPoints
	{
		internal List<NetworkEndPoint> EndPoints
		{
			get
			{
				return this.m_endPoints;
			}
		}

		private List<NetworkEndPoint> m_endPoints = new List<NetworkEndPoint>();
	}
}
