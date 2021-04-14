using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Data.Globalization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class DetailsTemplate : ADConfigurationObject
	{
		internal static ICollection<string> GetTemplateNames()
		{
			Collection<string> collection = new Collection<string>();
			foreach (KeyValuePair<string, string> keyValuePair in DetailsTemplate.templateTypes)
			{
				collection.Add(keyValuePair.Key);
			}
			return collection;
		}

		internal static string TranslateTemplateIDToName(string id)
		{
			foreach (KeyValuePair<string, string> keyValuePair in DetailsTemplate.templateTypes)
			{
				if (string.Compare(id, keyValuePair.Value, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return keyValuePair.Key;
				}
			}
			return null;
		}

		internal static string TranslateTemplateNameToID(string name)
		{
			foreach (KeyValuePair<string, string> keyValuePair in DetailsTemplate.templateTypes)
			{
				if (string.Compare(name, keyValuePair.Key, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return keyValuePair.Value;
				}
			}
			return null;
		}

		public new string Name
		{
			get
			{
				return (string)this[ADObjectSchema.Name];
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return DetailsTemplate.schemaObject;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "displayTemplate";
			}
		}

		public string TemplateType
		{
			get
			{
				if (this.templateType == null)
				{
					string name = this.Name;
					this.templateType = DetailsTemplate.TranslateTemplateIDToName(name);
					if (this.templateType == null)
					{
						this.templateType = "Unrecognized";
					}
				}
				return this.templateType;
			}
		}

		internal MAPIPropertiesDictionary MAPIPropertiesDictionary { get; set; }

		public string Language
		{
			get
			{
				if (this.language == null)
				{
					string distinguishedName = base.DistinguishedName;
					string s = distinguishedName.Split(new char[]
					{
						','
					})[1].Substring(3);
					Culture.TryGetCulture(int.Parse(s, NumberStyles.HexNumber), out this.language);
				}
				if (this.language == null)
				{
					return null;
				}
				CultureInfo cultureInfo = this.language.GetCultureInfo();
				if (this.language.LCID.Equals(cultureInfo.LCID))
				{
					return cultureInfo.DisplayName;
				}
				return this.language.Description;
			}
		}

		public byte[] TemplateBlob
		{
			get
			{
				return (byte[])this[DetailsTemplateSchema.TemplateBlob];
			}
		}

		public string ExchangeLegacyDN
		{
			get
			{
				return (string)this[DetailsTemplateSchema.ExchangeLegacyDN];
			}
		}

		[Parameter]
		public MultiValuedProperty<Page> Pages
		{
			get
			{
				return (MultiValuedProperty<Page>)this[DetailsTemplateSchema.Pages];
			}
			set
			{
				this[DetailsTemplateSchema.Pages] = value;
				this.validationErrors.Clear();
			}
		}

		internal void BlobToPages()
		{
			MultiValuedProperty<Page> multiValuedProperty = this.Pages;
			if (multiValuedProperty.IsReadOnly)
			{
				multiValuedProperty = new MultiValuedProperty<Page>();
			}
			multiValuedProperty = DetailsTemplateAdapter.BlobToPageCollection((byte[])this.propertyBag[DetailsTemplateSchema.TemplateBlob], this.MAPIPropertiesDictionary);
			multiValuedProperty.ResetChangeTracking();
			this.propertyBag.SetField(DetailsTemplateSchema.Pages, multiValuedProperty);
			this.ValidatePages();
		}

		internal void PagesToBlob()
		{
			this[DetailsTemplateSchema.TemplateBlob] = DetailsTemplateAdapter.PageCollectionToBlob(this.Pages, this.MAPIPropertiesDictionary);
			this.Pages.ResetChangeTracking();
			this.ValidatePages();
		}

		private void ValidatePages()
		{
			this.validationErrors.Clear();
			if (this.Pages != null && this.Pages.Count != 0)
			{
				string text = this.TemplateType;
				if (text.Equals("Mailbox Agent"))
				{
					return;
				}
				using (MultiValuedProperty<Page>.Enumerator enumerator = this.Pages.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Page page = enumerator.Current;
						int num = -1;
						foreach (DetailsTemplateControl detailsTemplateControl in page.Controls)
						{
							num++;
							string attributeName = detailsTemplateControl.m_AttributeName;
							if (!detailsTemplateControl.ValidateAttribute(this.MAPIPropertiesDictionary))
							{
								this.validationErrors.Add(new PropertyValidationError(DirectoryStrings.InvalidControlAttributeName(detailsTemplateControl.GetType().Name, page.Text, num, attributeName), DetailsTemplateSchema.Pages, this));
							}
							else if (!attributeName.Equals(string.Empty) && !this.MAPIPropertiesDictionary[attributeName][text])
							{
								this.validationErrors.Add(new PropertyValidationError(DirectoryStrings.InvalidControlAttributeForTemplateType(detailsTemplateControl.GetType().Name, page.Text, num, attributeName, text), DetailsTemplateSchema.Pages, this));
							}
						}
					}
					return;
				}
			}
			this.validationErrors.Add(new PropertyValidationError(DirectoryStrings.NoPagesSpecified, DetailsTemplateSchema.Pages, this));
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			errors.AddRange(this.validationErrors);
		}

		private const string LdapName = "displayTemplate";

		public const string ContactTemplate = "Contact";

		public const string GroupTemplate = "Group";

		public const string PublicFolderTemplate = "Public Folder";

		public const string UserTemplate = "User";

		public const string SearchTemplate = "Search Dialog";

		public const string MailboxAgentTemplate = "Mailbox Agent";

		internal static readonly ADObjectId ContainerId = new ADObjectId("CN=Display-Templates,CN=Addressing");

		private static readonly DetailsTemplateSchema schemaObject = ObjectSchema.GetInstance<DetailsTemplateSchema>();

		private static KeyValuePair<string, string>[] templateTypes = new KeyValuePair<string, string>[]
		{
			new KeyValuePair<string, string>("User", "0"),
			new KeyValuePair<string, string>("Group", "1"),
			new KeyValuePair<string, string>("Public Folder", "2"),
			new KeyValuePair<string, string>("Search Dialog", "200"),
			new KeyValuePair<string, string>("Mailbox Agent", "3"),
			new KeyValuePair<string, string>("Contact", "6")
		};

		private string templateType;

		private Culture language;

		private List<ValidationError> validationErrors = new List<ValidationError>();
	}
}
