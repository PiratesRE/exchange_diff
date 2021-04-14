using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public delegate IConfigurable TeamMailboxGetDataObject<TObject>(IIdentityParameter id, IConfigDataProvider session, ObjectId rootId, LocalizedString? notFoundError, LocalizedString? multipleFoundError, ExchangeErrorCategory errorCategory) where TObject : IConfigurable, new();
}
