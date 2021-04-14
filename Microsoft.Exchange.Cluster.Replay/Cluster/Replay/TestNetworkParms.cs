using System;
using Microsoft.Exchange.Data.Serialization;

namespace Microsoft.Exchange.Cluster.Replay
{
	[Serializable]
	internal class TestNetworkParms
	{
		public static TestNetworkParms FromBytes(byte[] bytes)
		{
			return (TestNetworkParms)Serialization.BytesToObject(bytes);
		}

		public byte[] ToBytes()
		{
			return Serialization.ObjectToBytes(this);
		}

		public bool Compression { get; set; }

		public bool Encryption { get; set; }

		public int TcpWindowSize { get; set; }

		public long ReplyCount { get; set; }

		public int TransferSize { get; set; }

		public int TimeoutInSec { get; set; }

		public long TransferCount { get; set; }
	}
}
