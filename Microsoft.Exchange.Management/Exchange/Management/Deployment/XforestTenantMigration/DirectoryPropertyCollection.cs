using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;

namespace Microsoft.Exchange.Management.Deployment.XforestTenantMigration
{
	[Serializable]
	public sealed class DirectoryPropertyCollection : List<DirectoryProperty>
	{
		public DirectoryPropertyCollection()
		{
		}

		public DirectoryPropertyCollection(string searchRoot, ResultPropertyCollection properties)
		{
			if (properties == null)
			{
				throw new ArgumentNullException("properties");
			}
			foreach (object obj in properties)
			{
				DictionaryEntry value = (DictionaryEntry)obj;
				base.Add(new DirectoryProperty(searchRoot, value));
			}
		}

		public DirectoryProperty this[string name]
		{
			get
			{
				if (string.IsNullOrEmpty(name))
				{
					throw new ArgumentNullException("name");
				}
				return this.FirstOrDefault((DirectoryProperty p) => p.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
			}
		}

		public bool Contains(string propertyName)
		{
			return this[propertyName] != null;
		}
	}
}
