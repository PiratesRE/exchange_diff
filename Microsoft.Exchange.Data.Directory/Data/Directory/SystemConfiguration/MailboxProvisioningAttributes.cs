using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Exchange.Collections;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[XmlType(TypeName = "MailboxProvisioningAttributes")]
	[Serializable]
	public sealed class MailboxProvisioningAttributes : XMLSerializableBase
	{
		public MailboxProvisioningAttributes()
		{
			this.propertyBag = new SimpleProviderPropertyBag();
			this.propertyBag.SetObjectVersion(ExchangeObjectVersion.Exchange2010);
		}

		public MailboxProvisioningAttributes(MailboxProvisioningAttribute[] attributes)
		{
			this.propertyBag = new SimpleProviderPropertyBag();
			this.propertyBag.SetObjectVersion(ExchangeObjectVersion.Exchange2010);
			this.Attributes = attributes;
		}

		public static IEnumerable<string> PermanentAttributeNames
		{
			get
			{
				return MailboxProvisioningAttributes.permanentReadOnlyNames;
			}
		}

		[XmlArray("Attributes")]
		[XmlArrayItem("MailboxProvisioningAttribute")]
		public MailboxProvisioningAttribute[] Attributes
		{
			get
			{
				List<MailboxProvisioningAttribute> list = new List<MailboxProvisioningAttribute>();
				foreach (PropertyDefinition propertyDefinition in ObjectSchema.GetInstance<MailboxProvisioningAttributesSchema>().AllFilterableProperties)
				{
					object obj = this.propertyBag[propertyDefinition];
					if (obj != null)
					{
						list.Add(new MailboxProvisioningAttribute
						{
							Key = propertyDefinition.Name,
							Value = (string)obj
						});
					}
				}
				return list.ToArray();
			}
			set
			{
				this.propertyBag.Clear();
				if (value != null)
				{
					for (int i = 0; i < value.Length; i++)
					{
						MailboxProvisioningAttribute attribute = value[i];
						PropertyDefinition key = ObjectSchema.GetInstance<MailboxProvisioningAttributesSchema>().AllFilterableProperties.First((PropertyDefinition x) => x.Name.Equals(attribute.Key));
						this.propertyBag[key] = attribute.Value;
					}
				}
			}
		}

		internal IReadOnlyPropertyBag PropertyBag
		{
			get
			{
				return this.propertyBag;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (MailboxProvisioningAttribute mailboxProvisioningAttribute in this.Attributes)
			{
				stringBuilder.AppendFormat("{0};", mailboxProvisioningAttribute.ToString());
			}
			return stringBuilder.ToString();
		}

		public static MailboxProvisioningAttributes Parse(string attributes)
		{
			if (string.IsNullOrEmpty(attributes) || string.IsNullOrWhiteSpace(attributes))
			{
				return null;
			}
			string[] array = attributes.Split(MailboxProvisioningAttributes.provisoningAttributeDelimiter, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length == 0 || array.Length > 30)
			{
				throw new InvalidMailboxProvisioningAttributeException(DirectoryStrings.ErrorInvalidMailboxProvisioningAttributes(30));
			}
			HashSet<string> hashSet = new HashSet<string>();
			List<MailboxProvisioningAttribute> list = new List<MailboxProvisioningAttribute>();
			ReadOnlyCollection<PropertyDefinition> allFilterableProperties = ObjectSchema.GetInstance<MailboxProvisioningAttributesSchema>().AllFilterableProperties;
			for (int i = 0; i < array.Length; i++)
			{
				MailboxProvisioningAttribute attribute = MailboxProvisioningAttribute.Parse(array[i]);
				if (hashSet.Contains(attribute.Key))
				{
					throw new InvalidMailboxProvisioningAttributeException(DirectoryStrings.ErrorDuplicateKeyInMailboxProvisioningAttributes(attribute.Key));
				}
				if (!allFilterableProperties.Any((PropertyDefinition x) => x.Name.Equals(attribute.Key)))
				{
					string validKeys = string.Join(",", from x in allFilterableProperties
					select x.Name);
					throw new ProvisioningAttributeDoesNotMatchSchemaException(attribute.Key, validKeys);
				}
				if (MailboxProvisioningAttributes.PermanentAttributeNames.Contains(attribute.Key))
				{
					throw new CannotSetPermanentAttributesException(string.Join(",", MailboxProvisioningAttributes.PermanentAttributeNames));
				}
				list.Add(attribute);
			}
			return new MailboxProvisioningAttributes(list.ToArray());
		}

		private const int MaxAllowedAttributes = 30;

		private static char[] provisoningAttributeDelimiter = new char[]
		{
			';'
		};

		private SimpleProviderPropertyBag propertyBag;

		private static readonly HashSet<string> permanentReadOnlyNames = new HashSet<string>(new string[]
		{
			MailboxProvisioningAttributesSchema.DagName.Name,
			MailboxProvisioningAttributesSchema.ServerName.Name,
			MailboxProvisioningAttributesSchema.DatabaseName.Name
		});
	}
}
