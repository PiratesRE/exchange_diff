using System;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	public class TopologyServiceResult
	{
		public TopologyServiceResult()
		{
		}

		public TopologyServiceResult(TopologyServiceResultEnum result)
		{
			this.result = result;
		}

		public TopologyServiceResultEnum Value
		{
			get
			{
				return this.result;
			}
		}

		public override string ToString()
		{
			string text = string.Empty;
			switch (this.result)
			{
			case TopologyServiceResultEnum.Undefined:
				text = Strings.TopologyServiceResultUndefined;
				break;
			case TopologyServiceResultEnum.Success:
				text = Strings.TopologyServiceResultSuccess;
				break;
			case TopologyServiceResultEnum.Failure:
				text = Strings.TopologyServiceResultFailure;
				break;
			}
			return text;
		}

		private TopologyServiceResultEnum result;
	}
}
