using System;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Linq;
using System.Management.Automation;
using Interop.ActiveDS;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.Deployment.XforestTenantMigration
{
	internal sealed class OrganizationConfigurationTree
	{
		internal Task.TaskVerboseLoggingDelegate WriteVerboseDelegate { get; set; }

		internal Task.TaskWarningLoggingDelegate WriteWarningDelegate { get; set; }

		internal Task.TaskErrorLoggingDelegate WriteErrorDelegate { get; set; }

		public Node Root { get; set; }

		public OrganizationConfigurationTree(DirectoryObjectCollection data)
		{
			if (data == null || data.Count == 0)
			{
				throw new ArgumentNullException("data");
			}
			ADObjectId adobjectId = new ADObjectId(data.ElementAt(0).Properties["distinguishedname"][0].ToString());
			this.Root = new Node(adobjectId.AncestorDN(adobjectId.Depth).ToDNString());
			foreach (DirectoryObject adObject in data)
			{
				this.AddLeaf(this.Root, adObject);
			}
		}

		private void AddLeaf(Node node, DirectoryObject adObject)
		{
			string[] array = DNConvertor.SplitDistinguishedName(adObject.Properties["distinguishedname"][0].ToString(), ',').Reverse<string>().ToArray<string>();
			for (int i = 0; i < array.Length; i++)
			{
				if (string.Compare(node.Name, array[i]) != 0)
				{
					if (!node.Children.ContainsKey(array[i]))
					{
						Node value = new Node(array[i]);
						node.Children.Add(array[i], value);
					}
					node = node.Children[array[i]];
				}
				if (i == array.Length - 1)
				{
					node.Value = adObject;
				}
			}
		}

		public void Replace(Node node, string oldValue, string newValue)
		{
			if (node == null)
			{
				node = this.Root;
			}
			if (node.Value != null)
			{
				node.Value.Replace(oldValue, newValue);
			}
			foreach (Node node2 in node.Children.Values)
			{
				this.Replace(node2, oldValue, newValue);
			}
		}

		public void Import(Node node, DirectoryBindingInfo directoryInformation)
		{
			if (node == null)
			{
				node = this.Root;
			}
			if (string.IsNullOrEmpty(OrganizationConfigurationTree.homemta))
			{
				using (DirectoryEntry directoryEntry = directoryInformation.GetDirectoryEntry(Path.Combine(directoryInformation.LdapBasePath, directoryInformation.ConfigurationNamingContextDN)))
				{
					SearchResult searchResult = new DirectorySearcher(directoryEntry)
					{
						Filter = "(CN=Microsoft MTA)"
					}.FindOne();
					if (searchResult != null)
					{
						OrganizationConfigurationTree.homemta = searchResult.Properties["distinguishedName"][0].ToString();
					}
				}
			}
			if (string.IsNullOrEmpty(OrganizationConfigurationTree.homemdb))
			{
				using (DirectoryEntry directoryEntry2 = directoryInformation.GetDirectoryEntry(Path.Combine(directoryInformation.LdapBasePath, directoryInformation.ConfigurationNamingContextDN)))
				{
					SearchResult searchResult2 = new DirectorySearcher(directoryEntry2)
					{
						Filter = "(objectclass=msExchPrivateMDB)"
					}.FindOne();
					if (searchResult2 != null)
					{
						OrganizationConfigurationTree.homemdb = searchResult2.Properties["distinguishedName"][0].ToString();
					}
				}
			}
			if (string.IsNullOrEmpty(OrganizationConfigurationTree.msexchowningserver))
			{
				using (DirectoryEntry directoryEntry3 = directoryInformation.GetDirectoryEntry(Path.Combine(directoryInformation.LdapBasePath, directoryInformation.ConfigurationNamingContextDN)))
				{
					SearchResult searchResult3 = new DirectorySearcher(directoryEntry3)
					{
						Filter = "(msexchowningserver=*)"
					}.FindOne();
					if (searchResult3 != null)
					{
						OrganizationConfigurationTree.msexchowningserver = searchResult3.Properties["msexchowningserver"][0].ToString();
						OrganizationConfigurationTree.msexchmasterserveroravailabilitygroup = searchResult3.Properties["msexchmasterserveroravailabilitygroup"][0].ToString();
					}
				}
			}
			foreach (Node node2 in node.Children.Values)
			{
				if (node2.Value != null && !this.IsNodeExist(node2.Value.DistinguishedName, directoryInformation))
				{
					if (this.IsNodeExist(node2.Value.ParentDistinguishedName, directoryInformation))
					{
						this.WriteVerbose(string.Format("Trying to import node {0}", node2.Value.DistinguishedName));
						using (DirectoryEntry directoryEntry4 = directoryInformation.GetDirectoryEntry(Path.Combine(directoryInformation.LdapBasePath, node2.Value.ParentDistinguishedName.ToString())))
						{
							using (DirectoryEntry directoryEntry5 = directoryEntry4.Children.Add(node2.Name, node2.Value.ObjectClass))
							{
								if (node2.Value.Properties.Contains("samaccountname"))
								{
									node2.Value.Properties["samaccountname"][0] = this.GetUniqueSamAccount(node2.Value.Properties["samaccountname"][0].ToString(), directoryInformation);
								}
								foreach (DirectoryProperty directoryProperty in node2.Value.Properties)
								{
									if (directoryProperty.Name.ToLower() == "homemdb")
									{
										directoryProperty.Values[0] = OrganizationConfigurationTree.homemdb;
									}
									if (directoryProperty.Name.ToLower() == "homemta")
									{
										directoryProperty.Values[0] = OrganizationConfigurationTree.homemta;
									}
									if (directoryProperty.Name.ToLower() == "msexchowningserver" || directoryProperty.Name.ToLower() == "offlineabserver")
									{
										directoryProperty.Values[0] = OrganizationConfigurationTree.msexchowningserver;
									}
									if (directoryProperty.Name.ToLower() == "msexchmasterserveroravailabilitygroup")
									{
										directoryProperty.Values[0] = OrganizationConfigurationTree.msexchmasterserveroravailabilitygroup;
									}
									this.AddObjectProperties(directoryProperty, directoryEntry5, node2, true, directoryInformation);
								}
								try
								{
									directoryEntry5.CommitChanges();
								}
								catch (Exception ex)
								{
									this.WriteError(new Exception(string.Format("Error committing changes to object {0}.  Inner exception was {1}.", node2.Value.DistinguishedName, ex.Message)));
								}
								if (node2.Value.ObjectClass.ToLower() == "user" && node2.Value.Properties["useraccountcontrol"][0].ToString() == "512")
								{
									directoryEntry5.Invoke("SetPassword", new object[]
									{
										Guid.NewGuid().ToString()
									});
									directoryEntry5.Properties["useraccountcontrol"].Value = 512;
									try
									{
										directoryEntry5.CommitChanges();
									}
									catch (Exception ex2)
									{
										this.WriteError(new Exception(string.Format("Error setting temp password on object {0}.  Inner exception was {1}.", node2.Value.DistinguishedName, ex2.Message)));
									}
								}
								foreach (DirectoryProperty property in node2.Value.Properties)
								{
									this.AddObjectProperties(property, directoryEntry5, node2, false, directoryInformation);
									try
									{
										directoryEntry5.CommitChanges();
									}
									catch (Exception ex3)
									{
										this.WriteError(new Exception(string.Format("Error setting non mandatory properties on object {0}.  Inner exception was {1}.", node2.Value.DistinguishedName, ex3.Message)));
									}
								}
							}
							continue;
						}
					}
					this.WriteError(new Exception("Can't import object " + node2.Value.DistinguishedName + " because its parent object does not exist."));
				}
			}
			foreach (Node node3 in node.Children.Values)
			{
				this.Import(node3, directoryInformation);
			}
		}

		public void UpdateDelayedProperties(Node node, DirectoryBindingInfo directoryInformation)
		{
			if (node == null)
			{
				node = this.Root;
			}
			foreach (Node node2 in node.Children.Values)
			{
				if (node2.Value != null && node2.Value.DelayedProperties.Count<DirectoryProperty>() > 0)
				{
					this.WriteVerbose(string.Format("Updating object {0} with properties that were delayed.", node2.Value.DistinguishedName));
					using (DirectoryEntry directoryEntry = directoryInformation.GetDirectoryEntry(Path.Combine(directoryInformation.LdapBasePath, node2.Value.DistinguishedName.ToString())))
					{
						foreach (DirectoryProperty directoryProperty in node2.Value.DelayedProperties)
						{
							object value = directoryEntry.Properties[directoryProperty.Name].Value;
							directoryEntry.Properties[directoryProperty.Name].Value = null;
							for (int i = 0; i < directoryProperty.Values.Count; i++)
							{
								if (this.IsNodeExist(node2.Value.DelayedProperties[directoryProperty.Name][i].ToString(), directoryInformation))
								{
									if (directoryProperty.Syntax == ActiveDirectorySyntax.Int64)
									{
										this.WriteVerbose(string.Format("Updating Property {0}: {1} ; {2}", directoryProperty.Name, directoryProperty.Syntax, directoryProperty.Values[i]));
										directoryEntry.Properties[directoryProperty.Name].Add(this.GetLargeInteger((long)directoryProperty.Values[i]));
									}
									else
									{
										this.WriteVerbose(string.Format("Updating Property {0}: {1} ; {2}", directoryProperty.Name, directoryProperty.Syntax, directoryProperty.Values[i]));
										directoryEntry.Properties[directoryProperty.Name].Add(directoryProperty.Values[i]);
									}
								}
								else
								{
									this.WriteWarning(string.Format("Could not update property {0} on object {1}.  Distinguished Name {2} was not found on target directory.", directoryProperty.Name, node2.Value.DistinguishedName, directoryProperty.Values[i]));
								}
							}
							if (directoryEntry.Properties[directoryProperty.Name].Value == null)
							{
								directoryEntry.Properties[directoryProperty.Name].Value = value;
							}
						}
						try
						{
							directoryEntry.CommitChanges();
						}
						catch (Exception ex)
						{
							this.WriteError(new Exception(string.Format("Error updating delayed properties on object {0}.  Inner exception was {1}.", node2.Value.DistinguishedName, ex.Message)));
						}
					}
				}
			}
			foreach (Node node3 in node.Children.Values)
			{
				this.UpdateDelayedProperties(node3, directoryInformation);
			}
		}

		private string GetUniqueSamAccount(string p, DirectoryBindingInfo d)
		{
			DirectoryEntry directoryEntry = d.GetDirectoryEntry(d.LdapBasePath + d.DefaultNamingContextDN);
			using (SearchResultCollection searchResultCollection = new DirectorySearcher(directoryEntry)
			{
				Filter = "(samaccountname=" + p + ")"
			}.FindAll())
			{
				if (searchResultCollection != null && searchResultCollection.Count > 0)
				{
					Random random = new Random();
					p = random.Next(0, int.MaxValue).ToString();
				}
			}
			return p;
		}

		private void AddObjectProperties(DirectoryProperty property, DirectoryEntry newObject, Node childNode, bool importOnlyRequiredProperties, DirectoryBindingInfo directoryInformation)
		{
			if (!property.IsSystemOnly && !property.IsBackLink && property.Values.Count > 0 && !this.excludedProperties.Contains(property.Name.ToLower()) && property.IsRequired == importOnlyRequiredProperties)
			{
				if (property.IsLink)
				{
					bool flag = false;
					foreach (object obj in property.Values)
					{
						string distinguishedName = (string)obj;
						if (!this.IsNodeExist(distinguishedName, directoryInformation))
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						if (property.IsRequired)
						{
							newObject.Properties[property.Name].Value = directoryInformation.SchemaNamingContextDN;
						}
						childNode.Value.DelayedProperties.Add(property);
						return;
					}
					for (int i = 0; i < property.Values.Count; i++)
					{
						this.WriteVerbose(string.Format("Adding Property {0}: {1} ; {2}", property.Name, property.Syntax, property.Values[i]));
						newObject.Properties[property.Name].Add(property.Values[i]);
					}
					return;
				}
				else
				{
					for (int j = 0; j < property.Values.Count; j++)
					{
						if (property.Syntax == ActiveDirectorySyntax.Int64)
						{
							this.WriteVerbose(string.Format("Adding Property {0}: {1} ; 2", property.Name, property.Syntax, property.Values[j]));
							newObject.Properties[property.Name].Add(this.GetLargeInteger((long)property.Values[j]));
						}
						else
						{
							this.WriteVerbose(string.Format("Adding Property {0}: {1}; {2}", property.Name, property.Syntax, property.Values[j]));
							newObject.Properties[property.Name].Add(property.Values[j]);
						}
					}
				}
			}
		}

		private bool IsNodeExist(string distinguishedName, DirectoryBindingInfo directoryInformation)
		{
			bool result = false;
			if (distinguishedName == null || string.IsNullOrEmpty(distinguishedName.ToString()))
			{
				return result;
			}
			using (DirectoryEntry directoryEntry = directoryInformation.GetDirectoryEntry(Path.Combine(directoryInformation.LdapBasePath, distinguishedName.ToString())))
			{
				using (DirectorySearcher directorySearcher = new DirectorySearcher(directoryEntry))
				{
					try
					{
						directorySearcher.SearchScope = SearchScope.Base;
						using (SearchResultCollection searchResultCollection = directorySearcher.FindAll())
						{
							if (searchResultCollection != null && searchResultCollection.Count > 0)
							{
								result = true;
							}
						}
					}
					catch (InvalidOperationException)
					{
					}
					catch (DirectoryServicesCOMException)
					{
					}
				}
			}
			return result;
		}

		private object GetLargeInteger(long value)
		{
			return new LargeIntegerClass
			{
				HighPart = (int)(value >> 32),
				LowPart = (int)(value & (long)((ulong)-1))
			};
		}

		private void WriteVerbose(string message)
		{
			if (this.WriteVerboseDelegate != null)
			{
				this.WriteVerboseDelegate(new LocalizedString(message));
			}
		}

		private void WriteWarning(string message)
		{
			if (this.WriteWarningDelegate != null)
			{
				this.WriteWarningDelegate(new LocalizedString(message));
			}
		}

		private void WriteError(Exception ex)
		{
			if (this.WriteErrorDelegate != null)
			{
				this.WriteErrorDelegate(ex, ErrorCategory.InvalidOperation, null);
			}
		}

		private static string homemta;

		private static string homemdb;

		private static string msexchmasterserveroravailabilitygroup;

		private static string msexchowningserver;

		private string[] excludedProperties = new string[]
		{
			"objectcategory",
			"cn",
			"adspath",
			"primarygroupid",
			"pwdlastset",
			"lastlogon",
			"lastlogontimestamp",
			"lastlogoff",
			"logoncount",
			"badpwdcount",
			"badpasswordtime",
			"samaccounttype",
			"revision",
			"objectsid",
			"domainreplica",
			"creationtime",
			"modifiedcount",
			"modifiedcountatlastpromotion",
			"nextrid",
			"serverstate",
			"iscriticalsystemobject",
			"dbcspwd",
			"ntpwdhistory",
			"lmpwdhistory",
			"badpasswordtime",
			"supplementalcredentials",
			"useraccountcontrol"
		};
	}
}
