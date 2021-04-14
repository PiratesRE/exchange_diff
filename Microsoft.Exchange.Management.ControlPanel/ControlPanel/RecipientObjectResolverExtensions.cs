using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class RecipientObjectResolverExtensions
	{
		public static IEnumerable<RecipientObjectResolverRow> ResolveRecipients(this MultiValuedProperty<ADObjectId> identities)
		{
			return RecipientObjectResolver.Instance.ResolveObjects(identities);
		}

		public static IEnumerable<PeopleRecipientObject> ResolveRecipientsForPeoplePicker(this MultiValuedProperty<ADObjectId> identities)
		{
			return RecipientObjectResolver.Instance.ResolvePeople(identities);
		}

		public static IEnumerable<string> ResolveRecipientsForSDO(this MultiValuedProperty<ADObjectId> identities, int maxNumber, Func<RecipientObjectResolverRow, string> convert)
		{
			List<string> list = new List<string>();
			if (identities != null)
			{
				IEnumerable<ADObjectId> identities2 = identities.Take(maxNumber);
				IEnumerable<RecipientObjectResolverRow> source = RecipientObjectResolver.Instance.ResolveObjects(identities2);
				list.AddRange(from resolvedRecipient in source
				select convert(resolvedRecipient));
				if (identities.Count > maxNumber)
				{
					list.Add(Strings.EllipsisText);
				}
			}
			return new MultiValuedProperty<string>(list);
		}
	}
}
