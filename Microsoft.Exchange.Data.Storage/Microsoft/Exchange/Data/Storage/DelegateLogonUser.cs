using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class DelegateLogonUser
	{
		internal DelegateLogonUser(IADOrgPerson adOrgPerson)
		{
			this.adOrgPerson = adOrgPerson;
		}

		internal DelegateLogonUser(string legacyDn)
		{
			this.legacyDn = legacyDn;
		}

		internal string LegacyDn
		{
			get
			{
				if (this.adOrgPerson != null)
				{
					return this.adOrgPerson.LegacyExchangeDN;
				}
				return this.legacyDn;
			}
		}

		internal IADOrgPerson ADOrgPerson
		{
			get
			{
				return this.adOrgPerson;
			}
		}

		public override string ToString()
		{
			return this.LegacyDn;
		}

		private readonly string legacyDn;

		private readonly IADOrgPerson adOrgPerson;
	}
}
