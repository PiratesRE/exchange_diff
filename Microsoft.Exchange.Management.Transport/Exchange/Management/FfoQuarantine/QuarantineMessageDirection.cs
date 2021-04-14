using System;
using Microsoft.Exchange.Core;

namespace Microsoft.Exchange.Management.FfoQuarantine
{
	[Serializable]
	public class QuarantineMessageDirection
	{
		public QuarantineMessageDirection()
		{
		}

		public QuarantineMessageDirection(QuarantineMessageDirectionEnum value)
		{
			this.value = value;
		}

		public QuarantineMessageDirectionEnum Value
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
			case QuarantineMessageDirectionEnum.Inbound:
				result = CoreStrings.QuarantineInbound;
				break;
			case QuarantineMessageDirectionEnum.Outbound:
				result = CoreStrings.QuarantineOutbound;
				break;
			}
			return result;
		}

		private QuarantineMessageDirectionEnum value;
	}
}
