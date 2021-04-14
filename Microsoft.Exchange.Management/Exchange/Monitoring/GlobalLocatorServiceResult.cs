using System;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	public class GlobalLocatorServiceResult
	{
		public GlobalLocatorServiceResult()
		{
		}

		public GlobalLocatorServiceResult(GlobalLocatorServiceResultEnum result)
		{
			this.result = result;
		}

		public GlobalLocatorServiceResultEnum Value
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
			case GlobalLocatorServiceResultEnum.Undefined:
				text = Strings.GlobalLocatorServiceResultUndefined;
				break;
			case GlobalLocatorServiceResultEnum.Success:
				text = Strings.GlobalLocatorServiceResultSuccess;
				break;
			case GlobalLocatorServiceResultEnum.Failure:
				text = Strings.GlobalLocatorServiceResultFailure;
				break;
			}
			return text;
		}

		private GlobalLocatorServiceResultEnum result;
	}
}
