using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	internal class TokenRenewQueryContext : QueryContext
	{
		internal List<TokenRenewRequestAsset> TokenRenewRequestAssets { get; set; }
	}
}
