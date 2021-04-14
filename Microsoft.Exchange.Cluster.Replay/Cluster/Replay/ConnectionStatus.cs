using System;
using System.Text;

namespace Microsoft.Exchange.Cluster.Replay
{
	[Serializable]
	public class ConnectionStatus
	{
		internal ConnectionStatus(string mailboxServer, string network, string lastFailure, ConnectionDirection direction, bool isSeeding)
		{
			this.m_partner = mailboxServer;
			this.m_network = network;
			this.m_lastFailure = lastFailure;
			this.m_isSeeding = isSeeding;
			this.m_direction = direction;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("{");
			stringBuilder.AppendFormat("{0},{1}", this.m_partner, this.m_network);
			if (this.m_isSeeding)
			{
				stringBuilder.AppendFormat(",{0}", this.m_direction);
			}
			if (this.m_lastFailure != null)
			{
				stringBuilder.AppendFormat(",{0}", this.m_lastFailure);
			}
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}

		public string Status
		{
			get
			{
				if (this.m_externalStatusString == null)
				{
					this.m_externalStatusString = this.ToString();
				}
				return this.m_externalStatusString;
			}
		}

		private string m_partner;

		private string m_network;

		private string m_lastFailure;

		private bool m_isSeeding;

		private ConnectionDirection m_direction;

		private string m_externalStatusString;
	}
}
