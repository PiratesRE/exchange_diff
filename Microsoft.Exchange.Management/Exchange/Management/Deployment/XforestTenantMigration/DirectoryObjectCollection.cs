using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;

namespace Microsoft.Exchange.Management.Deployment.XforestTenantMigration
{
	[Serializable]
	public sealed class DirectoryObjectCollection : List<DirectoryObject>
	{
		public DirectoryObjectCollection()
		{
		}

		public DirectoryObjectCollection(string searchRoot, SearchResultCollection searchResults)
		{
			if (searchRoot == null)
			{
				throw new ArgumentNullException("searchRoot");
			}
			if (searchResults == null)
			{
				throw new ArgumentNullException("searchResults");
			}
			foreach (object obj in searchResults)
			{
				SearchResult searchResult = (SearchResult)obj;
				base.Add(new DirectoryObject(searchRoot, searchResult));
			}
		}

		public DirectoryObject this[string name]
		{
			get
			{
				return base.Find((DirectoryObject p) => p.DistinguishedName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
			}
		}

		internal DirectoryObject GetDirectoryObjectByLdapDisplayName(string ldapDisplayName)
		{
			return this.FirstOrDefault((DirectoryObject p) => p.Properties["lDAPDisplayName"][0].ToString().Equals(ldapDisplayName, StringComparison.CurrentCultureIgnoreCase));
		}
	}
}
