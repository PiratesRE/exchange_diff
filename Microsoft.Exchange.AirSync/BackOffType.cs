using System;
using System.ComponentModel;

namespace Microsoft.Exchange.AirSync
{
	public enum BackOffType
	{
		[Description("L")]
		Low,
		[Description("M")]
		Medium,
		[Description("H")]
		High
	}
}
