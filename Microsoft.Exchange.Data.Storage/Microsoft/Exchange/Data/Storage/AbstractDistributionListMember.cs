using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class AbstractDistributionListMember : IDistributionListMember, IRecipientBase
	{
		public virtual RecipientId Id
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool? IsDistributionList()
		{
			throw new NotImplementedException();
		}

		public virtual Participant Participant
		{
			get
			{
				throw new NotImplementedException();
			}
		}
	}
}
