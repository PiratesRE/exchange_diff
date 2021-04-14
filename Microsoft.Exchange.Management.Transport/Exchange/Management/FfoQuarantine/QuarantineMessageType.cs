using System;
using Microsoft.Exchange.Core;

namespace Microsoft.Exchange.Management.FfoQuarantine
{
	[Serializable]
	public class QuarantineMessageType
	{
		public QuarantineMessageType()
		{
		}

		public QuarantineMessageType(QuarantineMessageTypeEnum value)
		{
			this.value = value;
		}

		public QuarantineMessageTypeEnum Value
		{
			get
			{
				return this.value;
			}
		}

		public override string ToString()
		{
			string result = string.Empty;
			switch (this.value)
			{
			case QuarantineMessageTypeEnum.Spam:
				result = CoreStrings.QuarantineSpam;
				break;
			case QuarantineMessageTypeEnum.TransportRule:
				result = CoreStrings.QuarantineTransportRule;
				break;
			}
			return result;
		}

		private QuarantineMessageTypeEnum value;
	}
}
