using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	internal class MAPIPropertiesDictionary
	{
		internal ICollection<string> GetControlMAPIAttributes(string templateType, DetailsTemplateControl.AttributeControlTypes controlType)
		{
			Dictionary<DetailsTemplateControl.AttributeControlTypes, ICollection<string>> dictionary;
			ICollection<string> result;
			if (this.winformDictionaries.TryGetValue(templateType, out dictionary) && dictionary.TryGetValue(controlType, out result))
			{
				return result;
			}
			return null;
		}

		internal string this[int mapiID]
		{
			get
			{
				string result;
				if (this.idToAttributeName.TryGetValue(mapiID, out result))
				{
					return result;
				}
				return null;
			}
		}

		internal AttributeInfo this[string attributeName]
		{
			get
			{
				if (attributeName == null)
				{
					throw new ArgumentNullException(attributeName);
				}
				AttributeInfo result;
				if (this.attributeNameToInfo.TryGetValue(attributeName, out result))
				{
					return result;
				}
				return null;
			}
		}

		private void ProcessAttributes(MultiValuedProperty<string> attributes, string[] templateTypes, Dictionary<string, string> ldapDisplayNameMapping)
		{
			for (int i = 0; i < attributes.Count; i++)
			{
				string key;
				if (ldapDisplayNameMapping.TryGetValue(attributes[i], out key))
				{
					for (int j = 0; j < templateTypes.Length; j++)
					{
						this.attributeNameToInfo[key][templateTypes[j]] = true;
					}
				}
			}
		}

		private void AssociateAttributesWithTemplates(ITopologyConfigurationSession configSession, Dictionary<string, string> ldapDisplayNameMapping)
		{
			Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();
			string text = "Contact";
			string text2 = "Group";
			string text3 = "Public Folder";
			string text4 = "User";
			string text5 = "Search Dialog";
			dictionary["contact"] = new string[]
			{
				text
			};
			dictionary["group"] = new string[]
			{
				text2
			};
			dictionary["publicFolder"] = new string[]
			{
				text3
			};
			dictionary["user"] = new string[]
			{
				text4,
				text5
			};
			dictionary["msExchCustomAttributes"] = new string[]
			{
				text,
				text2,
				text3,
				text5,
				text4
			};
			dictionary["organizationalPerson"] = new string[]
			{
				text4,
				text5,
				text
			};
			dictionary["msExchMultiMediaUser"] = new string[]
			{
				text,
				text4,
				text5
			};
			dictionary["msExchCertificateInformation"] = new string[]
			{
				text,
				text4,
				text5
			};
			dictionary["msExchBaseClass"] = new string[]
			{
				text,
				text2,
				text3,
				text4,
				text5
			};
			dictionary["person"] = new string[]
			{
				text,
				text4,
				text5
			};
			dictionary["mailRecipient"] = new string[]
			{
				text,
				text2,
				text3,
				text4,
				text5
			};
			dictionary["msExchIMRecipient"] = new string[]
			{
				text2,
				text4,
				text5
			};
			dictionary["securityPrincipal"] = new string[]
			{
				text2,
				text4,
				text5
			};
			dictionary["msExchMailStorage"] = new string[]
			{
				text3,
				text4,
				text5
			};
			dictionary["msExchOmaUser"] = new string[]
			{
				text4,
				text5
			};
			dictionary["classSchema"] = new string[]
			{
				text,
				text3,
				text4,
				text5
			};
			dictionary["top"] = new string[]
			{
				text,
				text2,
				text3,
				text4,
				text5
			};
			QueryFilter[] array = new QueryFilter[dictionary.Count];
			int num = 0;
			foreach (string propertyValue in dictionary.Keys)
			{
				array[num++] = new ComparisonFilter(ComparisonOperator.Equal, ADSchemaObjectSchema.DisplayName, propertyValue);
			}
			QueryFilter filter = new OrFilter(array);
			ADSchemaClassObject[] array2 = configSession.Find<ADSchemaClassObject>(configSession.SchemaNamingContext, QueryScope.SubTree, filter, null, int.MaxValue);
			for (int i = 0; i < array2.Length; i++)
			{
				string displayName = array2[i].DisplayName;
				string[] templateTypes = dictionary[displayName];
				MultiValuedProperty<string>[] array3 = new MultiValuedProperty<string>[]
				{
					array2[i].MayContain,
					array2[i].MustContain,
					array2[i].SystemMayContain,
					array2[i].SystemMustContain
				};
				for (int j = 0; j < array3.Length; j++)
				{
					this.ProcessAttributes(array3[j], templateTypes, ldapDisplayNameMapping);
				}
			}
		}

		private void RetrieveMapiAttributes(ITopologyConfigurationSession configSession, Dictionary<string, string> ldapDisplayNameMapping)
		{
			PropertyDefinition mapiID = ADSchemaAttributeSchema.MapiID;
			foreach (ADSchemaAttributeObject adschemaAttributeObject in configSession.Find<ADSchemaAttributeObject>(configSession.SchemaNamingContext, QueryScope.SubTree, new ExistsFilter(mapiID), null, int.MaxValue))
			{
				int num = (int)adschemaAttributeObject[mapiID];
				string name = adschemaAttributeObject.Name;
				string ldapDisplayName = adschemaAttributeObject.LdapDisplayName;
				this.idToAttributeName[num] = name;
				if (!this.attributeNameToInfo.ContainsKey(name))
				{
					AttributeSyntax omsyntax = adschemaAttributeObject.OMSyntax;
					DetailsTemplateControl.AttributeControlTypes attributeControlTypes = DetailsTemplateControl.AttributeControlTypes.None;
					AttributeSyntax attributeSyntax = omsyntax;
					if (attributeSyntax <= AttributeSyntax.Printable)
					{
						switch (attributeSyntax)
						{
						case AttributeSyntax.Boolean:
							attributeControlTypes = DetailsTemplateControl.AttributeControlTypes.Checkbox;
							break;
						case AttributeSyntax.Integer:
							goto IL_D0;
						default:
							switch (attributeSyntax)
							{
							case AttributeSyntax.Numeric:
							case AttributeSyntax.Printable:
								goto IL_D0;
							}
							break;
						}
					}
					else
					{
						if (attributeSyntax == AttributeSyntax.GeneralizedTime || attributeSyntax == AttributeSyntax.Unicode)
						{
							goto IL_D0;
						}
						if (attributeSyntax == AttributeSyntax.AccessPoint)
						{
							attributeControlTypes = DetailsTemplateControl.AttributeControlTypes.Listbox;
							if (!adschemaAttributeObject.IsSingleValued)
							{
								attributeControlTypes |= DetailsTemplateControl.AttributeControlTypes.MultiValued;
							}
						}
					}
					IL_E1:
					this.attributeNameToInfo.Add(name, new AttributeInfo(num, attributeControlTypes));
					goto IL_F7;
					IL_D0:
					attributeControlTypes = DetailsTemplateControl.AttributeControlTypes.Edit;
					if (!adschemaAttributeObject.IsSingleValued)
					{
						attributeControlTypes |= DetailsTemplateControl.AttributeControlTypes.MultiValued;
						goto IL_E1;
					}
					goto IL_E1;
				}
				IL_F7:
				ldapDisplayNameMapping[ldapDisplayName] = name;
			}
			int num2 = 12289;
			string text = "Display-Name";
			string key = "displayName";
			this.idToAttributeName.Add(num2, text);
			this.attributeNameToInfo.Add(text, new AttributeInfo(num2, DetailsTemplateControl.AttributeControlTypes.Edit));
			ldapDisplayNameMapping[key] = text;
			string key2 = "lDAPDisplayName";
			if (ldapDisplayNameMapping.ContainsKey(key2))
			{
				ldapDisplayNameMapping.Remove(key2);
			}
		}

		private void PrepareWinformDictionaries()
		{
			this.winformDictionaries = new Dictionary<string, Dictionary<DetailsTemplateControl.AttributeControlTypes, ICollection<string>>>();
			ICollection<string> templateNames = DetailsTemplate.GetTemplateNames();
			DetailsTemplateControl.AttributeControlTypes[] array = (DetailsTemplateControl.AttributeControlTypes[])Enum.GetValues(typeof(DetailsTemplateControl.AttributeControlTypes));
			foreach (string key in templateNames)
			{
				this.winformDictionaries[key] = new Dictionary<DetailsTemplateControl.AttributeControlTypes, ICollection<string>>();
				foreach (DetailsTemplateControl.AttributeControlTypes key2 in array)
				{
					this.winformDictionaries[key][key2] = new Collection<string>();
				}
			}
			foreach (AttributeInfo attributeInfo in this.attributeNameToInfo.Values)
			{
				string item = this.idToAttributeName[attributeInfo.MapiID];
				foreach (string text in templateNames)
				{
					if (attributeInfo[text])
					{
						foreach (DetailsTemplateControl.AttributeControlTypes attributeControlTypes in array)
						{
							if ((attributeInfo.ControlType & attributeControlTypes) != DetailsTemplateControl.AttributeControlTypes.None)
							{
								this.winformDictionaries[text][attributeControlTypes].Add(item);
							}
						}
					}
				}
			}
		}

		internal MAPIPropertiesDictionary()
		{
			this.idToAttributeName = new Dictionary<int, string>();
			this.attributeNameToInfo = new Dictionary<string, AttributeInfo>();
			Dictionary<string, string> ldapDisplayNameMapping = new Dictionary<string, string>();
			ITopologyConfigurationSession configSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 502, ".ctor", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\MAPIPropertiesDictionary.cs");
			this.RetrieveMapiAttributes(configSession, ldapDisplayNameMapping);
			this.AssociateAttributesWithTemplates(configSession, ldapDisplayNameMapping);
			this.PrepareWinformDictionaries();
		}

		private Dictionary<int, string> idToAttributeName;

		private Dictionary<string, AttributeInfo> attributeNameToInfo;

		private Dictionary<string, Dictionary<DetailsTemplateControl.AttributeControlTypes, ICollection<string>>> winformDictionaries;
	}
}
