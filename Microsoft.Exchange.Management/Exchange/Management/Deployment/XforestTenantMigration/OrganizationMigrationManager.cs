using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Text;

namespace Microsoft.Exchange.Management.Deployment.XforestTenantMigration
{
	internal sealed class OrganizationMigrationManager
	{
		public static void UpdateDirectoryObjectProperties(DirectoryObjectCollection exportedItems, DirectoryBindingInfo targetDirectoryBindingInfo)
		{
			DirectoryObjectCollection directoryObjectCollection;
			using (SearchResultCollection schema = OrganizationMigrationManager.GetSchema(null, "(|(objectClass=attributeSchema)(objectClass=classSchema))", targetDirectoryBindingInfo))
			{
				directoryObjectCollection = new DirectoryObjectCollection("Schema", schema);
			}
			foreach (DirectoryObject directoryObject in exportedItems)
			{
				using (ActiveDirectorySchemaClass activeDirectorySchemaClass = ActiveDirectorySchemaClass.FindByName(targetDirectoryBindingInfo.GetDirectoryContext(DirectoryContextType.Forest), directoryObject.Properties["objectclass"][directoryObject.Properties["objectclass"].Values.Count - 1].ToString()))
				{
					foreach (object obj in activeDirectorySchemaClass.MandatoryProperties)
					{
						ActiveDirectorySchemaProperty activeDirectorySchemaProperty = (ActiveDirectorySchemaProperty)obj;
						if (directoryObject.Properties.Contains(activeDirectorySchemaProperty.Name))
						{
							directoryObject.Properties[activeDirectorySchemaProperty.Name].IsRequired = true;
						}
					}
					foreach (object obj2 in activeDirectorySchemaClass.GetAllProperties())
					{
						ActiveDirectorySchemaProperty activeDirectorySchemaProperty2 = (ActiveDirectorySchemaProperty)obj2;
						if (directoryObject.Properties.Contains(activeDirectorySchemaProperty2.Name))
						{
							directoryObject.Properties[activeDirectorySchemaProperty2.Name].Syntax = activeDirectorySchemaProperty2.Syntax;
						}
					}
				}
				foreach (DirectoryProperty directoryProperty in directoryObject.Properties)
				{
					DirectoryObject directoryObjectByLdapDisplayName = directoryObjectCollection.GetDirectoryObjectByLdapDisplayName(directoryProperty.Name);
					if (directoryObjectByLdapDisplayName != null)
					{
						if (directoryObjectByLdapDisplayName.Properties.Contains("linkID") && directoryObjectByLdapDisplayName.Properties["linkID"].Values != null && int.Parse(directoryObjectByLdapDisplayName.Properties["linkID"][0].ToString()) % 2 != 0)
						{
							directoryProperty.IsBackLink = true;
						}
						if (directoryObjectByLdapDisplayName.Properties.Contains("attributeSyntax") && directoryObjectByLdapDisplayName.Properties["attributeSyntax"].Values != null && directoryObjectByLdapDisplayName.Properties["attributeSyntax"][0].ToString().CompareTo("2.5.5.1") == 0)
						{
							directoryProperty.IsLink = true;
						}
						if (directoryObjectByLdapDisplayName.Properties.Contains("systemOnly") && directoryObjectByLdapDisplayName.Properties["systemOnly"].Values != null && directoryObjectByLdapDisplayName.Properties["systemOnly"][0].ToString().ToLower() == "true")
						{
							directoryProperty.IsSystemOnly = true;
						}
					}
				}
			}
		}

		public static OrganizationConfigurationTree CalculateImportOrder(DirectoryObjectCollection exportedItems)
		{
			new List<string>();
			foreach (DirectoryObject directoryObject in exportedItems)
			{
				foreach (DirectoryProperty directoryProperty in directoryObject.Properties)
				{
					if (directoryProperty.IsLink && !directoryProperty.IsBackLink && !directoryProperty.IsRequired)
					{
						directoryObject.DelayedProperties.Add(directoryProperty);
					}
				}
			}
			return new OrganizationConfigurationTree(exportedItems);
		}

		public static void Import(DirectoryBindingInfo directoryInfo, DirectoryObjectCollection exportedItems, OrganizationConfigurationTree tree, string sourcedomain, string targetdomain, string sourceOrg, string targetOrg, string sourceSiteName, string targetSiteName)
		{
			string[] array = sourcedomain.Split(new char[]
			{
				'.'
			});
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append("DC=");
				stringBuilder.Append(array[i]);
				if (i != array.Length - 1)
				{
					stringBuilder.Append(',');
				}
			}
			array = targetdomain.Split(new char[]
			{
				'.'
			});
			StringBuilder stringBuilder2 = new StringBuilder();
			for (int j = 0; j < array.Length; j++)
			{
				stringBuilder2.Append("DC=");
				stringBuilder2.Append(array[j]);
				if (j != array.Length - 1)
				{
					stringBuilder2.Append(',');
				}
			}
			string text = stringBuilder.ToString();
			string text2 = stringBuilder2.ToString();
			string oldValue = "CN=" + sourceOrg + ",CN=Microsoft Exchange,CN=Services,CN=Configuration," + text;
			string newValue = "CN=" + targetOrg + ",CN=Microsoft Exchange,CN=Services,CN=Configuration," + text2;
			if (string.Compare(sourceOrg, targetOrg) != 0)
			{
				tree.Replace(null, oldValue, newValue);
			}
			string text3 = "CN=" + sourceSiteName + ",CN=Sites,CN=Configuration," + text;
			string text4 = "CN=" + targetSiteName + ",CN=Sites,CN=Configuration," + text2;
			if (string.Compare(text3, text4) != 0)
			{
				tree.Replace(null, text3, text4);
			}
			tree.Replace(null, text, text2);
			tree.Replace(null, sourcedomain, targetdomain);
			tree.Import(null, directoryInfo);
			tree.UpdateDelayedProperties(null, directoryInfo);
		}

		private static SearchResultCollection GetSchema(string[] propertiesToLoad, string filter, DirectoryBindingInfo directoryBindingInformation)
		{
			new List<SearchResult>();
			DirectoryEntry directoryEntry = directoryBindingInformation.GetDirectoryEntry(Path.Combine(directoryBindingInformation.LdapBasePath, directoryBindingInformation.SchemaNamingContextDN));
			DirectorySearcher directorySearcher = new DirectorySearcher(directoryEntry);
			directorySearcher.SearchScope = SearchScope.Subtree;
			directorySearcher.PageSize = int.MaxValue;
			if (!string.IsNullOrEmpty(filter))
			{
				directorySearcher.Filter = filter;
			}
			if (propertiesToLoad != null && propertiesToLoad.Length > 0)
			{
				directorySearcher.PropertiesToLoad.AddRange(propertiesToLoad);
			}
			return directorySearcher.FindAll();
		}
	}
}
