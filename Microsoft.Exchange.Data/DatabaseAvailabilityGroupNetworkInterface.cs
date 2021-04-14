using System;
using System.Net;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class DatabaseAvailabilityGroupNetworkInterface
	{
		public string NodeName
		{
			get
			{
				return this.m_nodeName;
			}
			set
			{
				this.m_nodeName = value;
			}
		}

		public DatabaseAvailabilityGroupNetworkInterface.InterfaceState State
		{
			get
			{
				return this.m_state;
			}
			set
			{
				this.m_state = value;
			}
		}

		public IPAddress IPAddress
		{
			get
			{
				return this.m_ipAddr;
			}
			set
			{
				this.m_ipAddr = value;
			}
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"{",
				this.NodeName,
				",",
				this.State.ToString(),
				",",
				this.IPAddress.ToString(),
				"}"
			});
		}

		private string m_nodeName;

		private DatabaseAvailabilityGroupNetworkInterface.InterfaceState m_state;

		private IPAddress m_ipAddr;

		public enum InterfaceState
		{
			[LocDescription(DataStrings.IDs.Unknown)]
			Unknown,
			[LocDescription(DataStrings.IDs.Up)]
			Up,
			[LocDescription(DataStrings.IDs.Failed)]
			Failed,
			[LocDescription(DataStrings.IDs.Unreachable)]
			Unreachable,
			[LocDescription(DataStrings.IDs.Unavailable)]
			Unavailable
		}
	}
}
