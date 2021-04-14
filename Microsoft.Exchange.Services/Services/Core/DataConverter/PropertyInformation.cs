using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class PropertyInformation : XmlElementInformation
	{
		static PropertyInformation()
		{
			PropertyInformation.commandInterfaceAttributes.Add(typeof(IAppendUpdateCommand), PropertyInformationAttributes.ImplementsAppendUpdateCommand);
			PropertyInformation.commandInterfaceAttributes.Add(typeof(IDeleteUpdateCommand), PropertyInformationAttributes.ImplementsDeleteUpdateCommand);
			PropertyInformation.commandInterfaceAttributes.Add(typeof(ISetCommand), PropertyInformationAttributes.ImplementsSetCommand);
			PropertyInformation.commandInterfaceAttributes.Add(typeof(ISetUpdateCommand), PropertyInformationAttributes.ImplementsSetUpdateCommand);
			PropertyInformation.commandInterfaceAttributes.Add(typeof(IToXmlCommand), PropertyInformationAttributes.ImplementsToXmlCommand);
			PropertyInformation.commandInterfaceAttributes.Add(typeof(IToXmlForPropertyBagCommand), PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand);
			PropertyInformation.commandInterfaceAttributes.Add(typeof(IToServiceObjectCommand), PropertyInformationAttributes.ImplementsToServiceObjectCommand);
			PropertyInformation.commandInterfaceAttributes.Add(typeof(IToServiceObjectForPropertyBagCommand), PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);
		}

		public PropertyInformation(string localName, string xmlPath, string namespaceUri, ExchangeVersion effectiveVersion, PropertyDefinition[] propertyDefinitions, PropertyPath propertyPath, PropertyCommand.CreatePropertyCommand createPropertyCommand, bool supportsMultipleInstancesForToXml, PropertyInformationAttributes propertyInformationAttributes) : base(localName, xmlPath, namespaceUri, effectiveVersion)
		{
			if (propertyDefinitions == null)
			{
				this.propertyDefinitions = new PropertyDefinition[0];
			}
			else
			{
				this.propertyDefinitions = propertyDefinitions;
			}
			this.supportsMultipleInstancesForToXml = supportsMultipleInstancesForToXml;
			this.propertyPath = propertyPath;
			this.createPropertyCommand = createPropertyCommand;
			this.propertyInformationAttributes = this.InternalGetPropertyInformationAttributes(propertyInformationAttributes);
		}

		private static PropertyInformationAttributes GetAttributesFromCommandInterfaces(Type propertyCommandType)
		{
			PropertyInformationAttributes propertyInformationAttributes = PropertyInformationAttributes.None;
			foreach (Type key in propertyCommandType.GetTypeInfo().ImplementedInterfaces)
			{
				PropertyInformationAttributes propertyInformationAttributes2;
				if (PropertyInformation.commandInterfaceAttributes.TryGetValue(key, out propertyInformationAttributes2))
				{
					propertyInformationAttributes |= propertyInformationAttributes2;
				}
			}
			return propertyInformationAttributes;
		}

		protected PropertyInformationAttributes PreparePropertyInformationAttributes(Type propertyCommandType, PropertyInformationAttributes propertyInformationAttributesToSupport)
		{
			PropertyInformationAttributes attributesFromCommandInterfaces = PropertyInformation.GetAttributesFromCommandInterfaces(propertyCommandType);
			PropertyInformationAttributes result;
			if (propertyInformationAttributesToSupport == PropertyInformationAttributes.None)
			{
				result = attributesFromCommandInterfaces;
			}
			else
			{
				result = propertyInformationAttributesToSupport;
			}
			if (this.propertyDefinitions.Length == 1)
			{
				result = this.ModifyAttributesForStoreReadOnlyFlag(result);
			}
			return result;
		}

		private PropertyInformationAttributes ModifyAttributesForStoreReadOnlyFlag(PropertyInformationAttributes propertyInformationAttributes)
		{
			StorePropertyDefinition storePropertyDefinition = this.propertyDefinitions[0] as StorePropertyDefinition;
			if (storePropertyDefinition != null && (storePropertyDefinition.PropertyFlags & PropertyFlags.ReadOnly) == PropertyFlags.ReadOnly)
			{
				propertyInformationAttributes &= ~(PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsAppendUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand);
			}
			return propertyInformationAttributes;
		}

		public PropertyInformation(string localName, string xmlPath, string namespaceUri, ExchangeVersion effectiveVersion, PropertyDefinition propertyDefinition, PropertyPath propertyPath, PropertyCommand.CreatePropertyCommand createPropertyCommand, bool supportsMultipleInstancesForToXml) : this(localName, xmlPath, namespaceUri, effectiveVersion, (propertyDefinition == null) ? null : new PropertyDefinition[]
		{
			propertyDefinition
		}, propertyPath, createPropertyCommand, supportsMultipleInstancesForToXml, PropertyInformationAttributes.None)
		{
		}

		public PropertyInformation(string localName, string xmlPath, string namespaceUri, ExchangeVersion effectiveVersion, PropertyDefinition[] propertyDefinitions, PropertyPath propertyPath, PropertyCommand.CreatePropertyCommand createPropertyCommand) : this(localName, xmlPath, namespaceUri, effectiveVersion, propertyDefinitions, propertyPath, createPropertyCommand, false, PropertyInformationAttributes.None)
		{
		}

		public PropertyInformation(string localName, string xmlPath, string namespaceUri, ExchangeVersion effectiveVersion, PropertyDefinition propertyDefinition, PropertyPath propertyPath, PropertyCommand.CreatePropertyCommand createPropertyCommand) : this(localName, xmlPath, namespaceUri, effectiveVersion, propertyDefinition, propertyPath, createPropertyCommand, false)
		{
		}

		public PropertyInformation(string localName, ExchangeVersion effectiveVersion, PropertyDefinition propertyDefinition, PropertyPath propertyPath, PropertyCommand.CreatePropertyCommand createPropertyCommand) : this(localName, ServiceXml.GetFullyQualifiedName(localName.ToString()), ServiceXml.DefaultNamespaceUri, effectiveVersion, propertyDefinition, propertyPath, createPropertyCommand)
		{
		}

		public PropertyInformation(PropertyUriEnum propertyUriEnum, ExchangeVersion effectiveVersion, PropertyDefinition propertyDefinition, PropertyCommand.CreatePropertyCommand createPropertyCommand) : this(propertyUriEnum.ToString(), effectiveVersion, propertyDefinition, new PropertyUri(propertyUriEnum), createPropertyCommand)
		{
		}

		public PropertyInformation(string localName, string xmlPath, string namespaceUri, ExchangeVersion effectiveVersion, PropertyDefinition[] propertyDefinitions, PropertyPath propertyPath, PropertyCommand.CreatePropertyCommand createPropertyCommand, PropertyInformationAttributes propertyInformationAttributes) : this(localName, xmlPath, namespaceUri, effectiveVersion, propertyDefinitions, propertyPath, createPropertyCommand, false, propertyInformationAttributes)
		{
		}

		public PropertyInformation(string localName, ExchangeVersion effectiveVersion, PropertyDefinition propertyDefinition, PropertyPath propertyPath, PropertyCommand.CreatePropertyCommand createPropertyCommand, PropertyInformationAttributes propertyInformationAttributes) : this(localName, ServiceXml.GetFullyQualifiedName(localName.ToString()), ServiceXml.DefaultNamespaceUri, effectiveVersion, (propertyDefinition == null) ? null : new PropertyDefinition[]
		{
			propertyDefinition
		}, propertyPath, createPropertyCommand, propertyInformationAttributes)
		{
		}

		public PropertyInformation(PropertyUriEnum propertyUriEnum, ExchangeVersion effectiveVersion, PropertyDefinition propertyDefinition, PropertyCommand.CreatePropertyCommand createPropertyCommand, PropertyInformationAttributes propertyInformationAttributes) : this(propertyUriEnum.ToString(), effectiveVersion, propertyDefinition, new PropertyUri(propertyUriEnum), createPropertyCommand, propertyInformationAttributes)
		{
		}

		public virtual PropertyDefinition[] GetPropertyDefinitions(CommandSettings commandSettings)
		{
			return this.propertyDefinitions;
		}

		public PropertyPath PropertyPath
		{
			get
			{
				return this.propertyPath;
			}
		}

		public virtual PropertyCommand.CreatePropertyCommand CreatePropertyCommand
		{
			get
			{
				return this.createPropertyCommand;
			}
		}

		public virtual PropertyInformationAttributes PropertyInformationAttributes
		{
			get
			{
				return this.propertyInformationAttributes;
			}
		}

		private bool IsAttributeSet(PropertyInformationAttributes checkPropertyInformationAttributes)
		{
			return (this.PropertyInformationAttributes & checkPropertyInformationAttributes) == checkPropertyInformationAttributes;
		}

		public bool SupportsMultipleInstancesForToXml
		{
			get
			{
				return this.supportsMultipleInstancesForToXml;
			}
		}

		public bool ImplementsSetCommand
		{
			get
			{
				return this.IsAttributeSet(PropertyInformationAttributes.ImplementsSetCommand);
			}
		}

		public bool ImplementsToXmlCommand
		{
			get
			{
				return this.IsAttributeSet(PropertyInformationAttributes.ImplementsToXmlCommand);
			}
		}

		public bool ImplementsAppendUpdateCommand
		{
			get
			{
				return this.IsAttributeSet(PropertyInformationAttributes.ImplementsAppendUpdateCommand);
			}
		}

		public bool ImplementsDeleteUpdateCommand
		{
			get
			{
				return this.IsAttributeSet(PropertyInformationAttributes.ImplementsDeleteUpdateCommand);
			}
		}

		public bool ImplementsSetUpdateCommand
		{
			get
			{
				return this.IsAttributeSet(PropertyInformationAttributes.ImplementsSetUpdateCommand);
			}
		}

		public bool ImplementsToXmlForPropertyBagCommand
		{
			get
			{
				return this.IsAttributeSet(PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand);
			}
		}

		public bool ImplementsToServiceObjectCommand
		{
			get
			{
				return this.IsAttributeSet(PropertyInformationAttributes.ImplementsToServiceObjectCommand);
			}
		}

		public bool ImplementsToServiceObjectForPropertyBagCommand
		{
			get
			{
				return this.IsAttributeSet(PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);
			}
		}

		public static PropertyInformation CreateMessageFlagsPropertyInformation(string localName, ExchangeVersion effectiveVersion, PropertyUriEnum propertyUriEnum, PropertyCommand.CreatePropertyCommand creationDelegate)
		{
			return new PropertyInformation(localName, effectiveVersion, MessageItemSchema.Flags, new PropertyUri(propertyUriEnum), creationDelegate, PropertyInformationAttributes.ImplementsReadOnlyCommands);
		}

		private PropertyInformationAttributes InternalGetPropertyInformationAttributes(PropertyInformationAttributes propertyInformationAttributes)
		{
			return this.PreparePropertyInformationAttributes(this.createPropertyCommand.Method.ReturnType, propertyInformationAttributes);
		}

		private const bool SupportsMultipleInstancesForToXmlDefault = false;

		private readonly PropertyDefinition[] propertyDefinitions;

		private readonly PropertyPath propertyPath;

		private readonly PropertyCommand.CreatePropertyCommand createPropertyCommand;

		private readonly bool supportsMultipleInstancesForToXml;

		private readonly PropertyInformationAttributes propertyInformationAttributes;

		private static Dictionary<Type, PropertyInformationAttributes> commandInterfaceAttributes = new Dictionary<Type, PropertyInformationAttributes>();
	}
}
