using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Microsoft.Exchange.Transport
{
	public sealed class AckDetails : IEquatable<AckDetails>
	{
		public AckDetails(IPEndPoint remoteEndPoint, string remoteHostName, string sessionId, string connectorId, IPAddress sourceIP) : this(remoteEndPoint, remoteHostName, sessionId, connectorId, sourceIP, null)
		{
		}

		public AckDetails(IPEndPoint remoteEndPoint, string remoteHostName, string sessionId, string connectorId, IPAddress sourceIP, DateTime? lastRetryTime)
		{
			this.remoteEndPoint = remoteEndPoint;
			this.remoteHostName = remoteHostName;
			this.sessionId = sessionId;
			this.connectorId = connectorId;
			this.sourceIPAddress = sourceIP;
			this.lastRetryTime = lastRetryTime;
		}

		public AckDetails(string remoteHostName)
		{
			this.remoteHostName = remoteHostName;
		}

		public IPEndPoint RemoteEndPoint
		{
			get
			{
				return this.remoteEndPoint;
			}
		}

		public string RemoteHostName
		{
			get
			{
				return this.remoteHostName;
			}
		}

		public string SessionId
		{
			get
			{
				return this.sessionId;
			}
		}

		public string ConnectorId
		{
			get
			{
				return this.connectorId;
			}
		}

		public IPAddress SourceIPAddress
		{
			get
			{
				return this.sourceIPAddress;
			}
		}

		public DateTime? LastRetryTime
		{
			get
			{
				return this.lastRetryTime;
			}
			set
			{
				this.lastRetryTime = value;
			}
		}

		public List<KeyValuePair<string, string>> ExtraEventData
		{
			get
			{
				return this.extraEventData;
			}
		}

		public void AddEventData(string name, string value)
		{
			if (this.extraEventData == null)
			{
				this.extraEventData = new List<KeyValuePair<string, string>>();
			}
			this.extraEventData.Add(new KeyValuePair<string, string>(name, value));
		}

		public override int GetHashCode()
		{
			int num = (this.remoteHostName != null) ? this.remoteHostName.GetHashCode() : 0;
			num = (num * 397 ^ ((this.sessionId != null) ? this.sessionId.GetHashCode() : 0));
			num = (num * 397 ^ ((this.connectorId != null) ? this.connectorId.GetHashCode() : 0));
			num = (num * 397 ^ ((this.remoteEndPoint != null) ? this.remoteEndPoint.GetHashCode() : 0));
			num = (num * 397 ^ ((this.sourceIPAddress != null) ? this.sourceIPAddress.GetHashCode() : 0));
			num = (num * 397 ^ this.lastRetryTime.GetHashCode());
			return num * 397 ^ ((this.extraEventData != null) ? this.extraEventData.GetHashCode() : 0);
		}

		public override bool Equals(object other)
		{
			return this.Equals(other as AckDetails);
		}

		public bool Equals(AckDetails other)
		{
			return !object.ReferenceEquals(other, null) && (object.ReferenceEquals(this, other) || (((this.remoteHostName == null && other.remoteHostName == null) || (this.remoteHostName != null && this.remoteHostName.Equals(other.remoteHostName))) && ((this.sessionId == null && other.sessionId == null) || (this.sessionId != null && this.sessionId.Equals(other.sessionId))) && ((this.connectorId == null && other.connectorId == null) || (this.connectorId != null && this.connectorId.Equals(other.connectorId))) && ((this.remoteEndPoint == null && other.remoteEndPoint == null) || (this.remoteEndPoint != null && this.remoteEndPoint.Equals(other.remoteEndPoint))) && ((this.sourceIPAddress == null && other.sourceIPAddress == null) || (this.sourceIPAddress != null && this.sourceIPAddress.Equals(other.sourceIPAddress))) && ((this.lastRetryTime == null && other.lastRetryTime == null) || (this.lastRetryTime != null && this.lastRetryTime.Equals(other.lastRetryTime))) && ((this.extraEventData == null && other.extraEventData == null) || (this.extraEventData != null && other.extraEventData != null && this.extraEventData.SequenceEqual(other.extraEventData)))));
		}

		private readonly string remoteHostName;

		private readonly string sessionId;

		private readonly string connectorId;

		private readonly IPEndPoint remoteEndPoint;

		private readonly IPAddress sourceIPAddress;

		private DateTime? lastRetryTime;

		private List<KeyValuePair<string, string>> extraEventData;
	}
}
