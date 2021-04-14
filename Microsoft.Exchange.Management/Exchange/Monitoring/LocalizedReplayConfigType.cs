using System;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	public class LocalizedReplayConfigType
	{
		internal LocalizedReplayConfigType(ReplayConfigType configtype)
		{
			this.configType = configtype;
		}

		internal ReplayConfigType ConfigType
		{
			get
			{
				return this.configType;
			}
		}

		public override string ToString()
		{
			string result = string.Empty;
			switch (this.configType)
			{
			case ReplayConfigType.RemoteCopyTarget:
			case ReplayConfigType.RemoteCopySource:
				result = Strings.RemoteContinuousReplication;
				break;
			case ReplayConfigType.SingleCopySource:
				result = Strings.SingleCopyDatabase;
				break;
			}
			return result;
		}

		private ReplayConfigType configType;
	}
}
