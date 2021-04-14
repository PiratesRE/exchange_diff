using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	internal class UpdateQueryContext : QueryContext
	{
		internal Dictionary<string, UpdateRequestAsset> UpdateRequestAssets { get; set; }
	}
}
