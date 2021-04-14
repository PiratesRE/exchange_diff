using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class MailboxPlanResolverExtensions
	{
		public static MailboxPlanResolverRow ResolveMailboxPlan(ADObjectId mailboxPlan)
		{
			List<ADObjectId> list = new List<ADObjectId>();
			list.Add(mailboxPlan);
			return MailboxPlanResolver.Instance.ResolveObjects(list).FirstOrDefault<MailboxPlanResolverRow>();
		}
	}
}
