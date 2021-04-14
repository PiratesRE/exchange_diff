using System;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	public class LiveIdAuthenticationResult
	{
		public LiveIdAuthenticationResult()
		{
		}

		public LiveIdAuthenticationResult(LiveIdAuthenticationResultEnum result)
		{
			this.result = result;
		}

		public LiveIdAuthenticationResultEnum Value
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
			case LiveIdAuthenticationResultEnum.Undefined:
				text = Strings.LiveIdAuthenticationResultUndefined;
				break;
			case LiveIdAuthenticationResultEnum.Success:
				text = Strings.LiveIdAuthenticationResultSuccess;
				break;
			case LiveIdAuthenticationResultEnum.Failure:
				text = Strings.LiveIdAuthenticationResultFailure;
				break;
			}
			return text;
		}

		private LiveIdAuthenticationResultEnum result;
	}
}
