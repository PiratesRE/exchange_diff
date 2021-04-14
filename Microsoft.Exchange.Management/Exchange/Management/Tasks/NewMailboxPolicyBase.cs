using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class NewMailboxPolicyBase<T> : NewMultitenancySystemConfigurationObjectTask<T> where T : MailboxPolicy, new()
	{
		protected override IConfigurable PrepareDataObject()
		{
			T t = (T)((object)base.PrepareDataObject());
			t.SetId((IConfigurationSession)base.DataSession, base.Name);
			return t;
		}

		protected IList<T> existingDefaultPolicies;

		protected bool updateExistingDefaultPolicies;
	}
}
