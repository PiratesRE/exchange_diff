using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class TeamMailboxADUserResolver : LazyLookupTimeoutCache<ADObjectId, ADUser>
	{
		private TeamMailboxADUserResolver() : base(10, 1000, false, TimeSpan.FromHours(1.0), TimeSpan.FromHours(1.0))
		{
		}

		public static ADUser ResolveBypassCache(IRecipientSession dataSession, ADObjectId id, out Exception ex)
		{
			TeamMailboxADUserResolver.RemoveIdIfExists(id);
			return TeamMailboxADUserResolver.Resolve(dataSession, id, out ex);
		}

		public static ADUser Resolve(IRecipientSession dataSession, ADObjectId id, out Exception ex)
		{
			if (dataSession == null)
			{
				throw new ArgumentNullException("dataSession");
			}
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			TeamMailboxADUserResolver.instance.dataSession = dataSession;
			ADUser result = null;
			ex = null;
			try
			{
				result = TeamMailboxADUserResolver.instance.Get(id);
			}
			catch (ADTransientException ex2)
			{
				ex = ex2;
			}
			catch (ADExternalException ex3)
			{
				ex = ex3;
			}
			catch (ADOperationException ex4)
			{
				ex = ex4;
			}
			return result;
		}

		public static void RemoveIdIfExists(ADObjectId id)
		{
			if (TeamMailboxADUserResolver.instance.Contains(id))
			{
				TeamMailboxADUserResolver.instance.Remove(id);
			}
		}

		protected override ADUser CreateOnCacheMiss(ADObjectId key, ref bool shouldAdd)
		{
			ADUser aduser = this.dataSession.FindADUserByObjectId(key);
			shouldAdd = (aduser != null);
			return aduser;
		}

		private static TeamMailboxADUserResolver instance = new TeamMailboxADUserResolver();

		private IRecipientSession dataSession;
	}
}
