using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public delegate void WriteErrorDelegate(LocalizedException exception, ExchangeErrorCategory category);
}
