using System;
using System.Collections.Generic;
using System.DirectoryServices;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.Deployment.XforestTenantMigration
{
	[Serializable]
	public sealed class DirectoryObject
	{
		public string ObjectClass
		{
			get
			{
				List<object> values = this.Properties["objectClass"].Values;
				return (string)values[values.Count - 1];
			}
		}

		public string DistinguishedName
		{
			get
			{
				return (string)this.Properties["distinguishedName"].Value;
			}
			set
			{
				if (this.Properties["distinguishedName"] != null)
				{
					this.Properties["distinguishedName"].Value = value;
				}
			}
		}

		internal List<string> DependentObjects { get; set; }

		internal List<string> DependedOnObjects { get; set; }

		public DirectoryPropertyCollection Properties { get; set; }

		internal DirectoryPropertyCollection DelayedProperties { get; set; }

		public string ParentDistinguishedName
		{
			get
			{
				ADObjectId adobjectId = new ADObjectId(this.DistinguishedName);
				return adobjectId.AncestorDN(1).ToDNString();
			}
		}

		public DirectoryObject()
		{
			this.Properties = new DirectoryPropertyCollection();
			this.DependentObjects = new List<string>();
			this.DependedOnObjects = new List<string>();
			this.DelayedProperties = new DirectoryPropertyCollection();
		}

		public DirectoryObject(string searchRoot, SearchResult searchResult) : this()
		{
			if (searchRoot == null)
			{
				throw new ArgumentNullException("searchRoot");
			}
			if (searchResult == null)
			{
				throw new ArgumentNullException("searchResult");
			}
			this.Properties = new DirectoryPropertyCollection(searchRoot, searchResult.Properties);
		}

		internal void Replace(string oldValue, string newValue)
		{
			this.DistinguishedName = this.DistinguishedName.ToString().Replace(oldValue, newValue);
			foreach (DirectoryProperty directoryProperty in this.Properties)
			{
				for (int i = 0; i < directoryProperty.Values.Count; i++)
				{
					if (directoryProperty[i].ToString().Contains(oldValue))
					{
						directoryProperty[i] = directoryProperty[i].ToString().Replace(oldValue, newValue);
					}
				}
			}
		}
	}
}
