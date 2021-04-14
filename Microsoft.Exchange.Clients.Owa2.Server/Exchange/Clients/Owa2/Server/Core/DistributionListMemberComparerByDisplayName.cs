using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class DistributionListMemberComparerByDisplayName : IComparer<IDistributionListMember>
	{
		public DistributionListMemberComparerByDisplayName(CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			this.culture = culture;
		}

		public int Compare(IDistributionListMember member1, IDistributionListMember member2)
		{
			if (member1 == null)
			{
				throw new ArgumentNullException("member1");
			}
			if (member2 == null)
			{
				throw new ArgumentNullException("member2");
			}
			if (member1.Participant == null)
			{
				if (!(member2.Participant == null))
				{
					return -1;
				}
				return 0;
			}
			else
			{
				if (member2.Participant == null)
				{
					return 1;
				}
				return string.Compare(member1.Participant.DisplayName, member2.Participant.DisplayName, this.culture, CompareOptions.None);
			}
		}

		private readonly CultureInfo culture;
	}
}
