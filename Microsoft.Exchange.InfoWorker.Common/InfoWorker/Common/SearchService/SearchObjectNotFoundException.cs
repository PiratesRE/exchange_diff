using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.InfoWorker.Common.SearchService
{
	[Serializable]
	internal class SearchObjectNotFoundException : ObjectNotFoundException
	{
		internal SearchObjectNotFoundException(LocalizedString message) : base(message)
		{
		}
	}
}
