using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.DefaultProvisioningAgent.Rus
{
	internal class PolicyContainer<T>
	{
		public List<T> Policies
		{
			get
			{
				return this.policies;
			}
			set
			{
				this.policies = value;
			}
		}

		public OrganizationId OrganizationId
		{
			get
			{
				return this.organizationId;
			}
			set
			{
				this.organizationId = value;
			}
		}

		private List<T> policies;

		private OrganizationId organizationId;
	}
}
