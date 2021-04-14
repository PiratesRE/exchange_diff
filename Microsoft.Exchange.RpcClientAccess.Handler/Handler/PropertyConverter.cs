using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Handler.StorageObjects;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	internal class PropertyConverter
	{
		internal PropertyConverter(IEnumerable<PropertyConverter.PropertyDefinitionMapping> propertyDefinitionMapping, params PropertyConversion[] conversions)
		{
			this.propertyConversionToClientDictionary = PropertyConverter.CreatePropertyConversionToClientDictionary(conversions);
			this.propertyConversions = conversions;
			if (propertyDefinitionMapping != null)
			{
				foreach (PropertyConverter.PropertyDefinitionMapping propertyDefinitionMapping2 in propertyDefinitionMapping)
				{
					if (this.propertyTagToPropertyDefinitionMapping.ContainsKey(propertyDefinitionMapping2.PropertyTag))
					{
						throw new ArgumentException(string.Format("Mapping for property tag [{0}] already exists", propertyDefinitionMapping2.PropertyTag), "propertyDefinitionMapping");
					}
					if (this.propertyDefinitionToPropertyTagMapping.ContainsKey(propertyDefinitionMapping2.PropertyDefinition))
					{
						throw new ArgumentException(string.Format("Mapping for property definition [{0}] already exists", propertyDefinitionMapping2.PropertyDefinition), "propertyDefinitionMapping");
					}
					this.propertyTagToPropertyDefinitionMapping.Add(propertyDefinitionMapping2.PropertyTag, propertyDefinitionMapping2.PropertyDefinition);
					this.propertyDefinitionToPropertyTagMapping.Add(propertyDefinitionMapping2.PropertyDefinition, propertyDefinitionMapping2.PropertyTag);
				}
			}
		}

		internal static PropertyConverter Message
		{
			get
			{
				return PropertyConverter.genericConverter;
			}
		}

		internal static PropertyConverter Folder
		{
			get
			{
				return PropertyConverter.folderConverter;
			}
		}

		internal static PropertyConverter HierarchyView
		{
			get
			{
				return PropertyConverter.hierarchyViewConverter;
			}
		}

		internal static PropertyConverter Attachment
		{
			get
			{
				return PropertyConverter.genericConverter;
			}
		}

		internal static PropertyConverter Recipient
		{
			get
			{
				return PropertyConverter.genericConverter;
			}
		}

		internal static PropertyConverter Rule
		{
			get
			{
				return PropertyConverter.genericConverter;
			}
		}

		internal static PropertyConverter Logon
		{
			get
			{
				return PropertyConverter.logonConverter;
			}
		}

		internal static PropertyConverter Permission
		{
			get
			{
				return PropertyConverter.genericConverter;
			}
		}

		internal IEnumerable<PropertyTag> ClientPropertyTagsThatRequireConversion
		{
			get
			{
				return from conversion in this.propertyConversions
				select conversion.ClientPropertyTag;
			}
		}

		internal IEnumerable<PropertyTag> ServerPropertyTags
		{
			get
			{
				return from conversion in this.propertyConversions
				select conversion.ServerPropertyTag;
			}
		}

		internal static void ConvertFromExportToOurServerId(StoreSession session, ref PropertyValue propertyValue)
		{
			if (propertyValue.PropertyTag.PropertyType == PropertyType.ServerId)
			{
				byte[] valueAssert = propertyValue.GetValueAssert<byte[]>();
				ServerIdType serverIdType = ServerIdConverter.GetServerIdType(new ArraySegment<byte>(valueAssert));
				if (serverIdType == ServerIdType.Export)
				{
					propertyValue = new PropertyValue(propertyValue.PropertyTag, ServerIdConverter.MakeOurServerIdFromExportServerId(session, valueAssert));
				}
			}
		}

		internal static void ConvertFromOurToExportServerId(StoreSession session, ref PropertyValue propertyValue)
		{
			if (propertyValue.PropertyTag.PropertyType == PropertyType.ServerId)
			{
				byte[] valueAssert = propertyValue.GetValueAssert<byte[]>();
				ServerIdType serverIdType = ServerIdConverter.GetServerIdType(new ArraySegment<byte>(valueAssert));
				if (serverIdType == ServerIdType.Ours)
				{
					propertyValue = new PropertyValue(propertyValue.PropertyTag, ServerIdConverter.MakeExportServerIdFromOurServerId(session, valueAssert));
				}
			}
		}

		internal PropertyTag ConvertPropertyTagFromClient(PropertyTag propertyTag)
		{
			for (int i = 0; i < this.propertyConversions.Length; i++)
			{
				PropertyConversion propertyConversion = this.propertyConversions[i];
				PropertyTag result;
				if (propertyConversion.TryConvertPropertyTagFromClient(propertyTag, out result))
				{
					return result;
				}
			}
			return propertyTag;
		}

		internal PropertyTag[] ConvertPropertyTagsFromClient(PropertyTag[] propertyTags)
		{
			if (propertyTags == null)
			{
				return null;
			}
			PropertyTag[] array = null;
			for (int i = 0; i < propertyTags.Length; i++)
			{
				PropertyTag propertyTag = this.ConvertPropertyTagFromClient(propertyTags[i]);
				if (propertyTag != propertyTags[i])
				{
					if (array == null)
					{
						array = new PropertyTag[propertyTags.Length];
						Array.Copy(propertyTags, array, propertyTags.Length);
					}
					array[i] = propertyTag;
				}
			}
			return array ?? propertyTags;
		}

		internal void ConvertPropertyValueFromClient(StoreSession session, IStorageObjectProperties storageObjectProperties, ref PropertyValue propertyValue)
		{
			for (int i = 0; i < this.propertyConversions.Length; i++)
			{
				PropertyConversion propertyConversion = this.propertyConversions[i];
				if (propertyConversion.TryConvertPropertyValueFromClient(session, storageObjectProperties, ref propertyValue))
				{
					return;
				}
			}
		}

		internal void ConvertPropertyValuesFromClient(StoreSession session, IStorageObjectProperties storageObjectProperties, PropertyValue[] propertyValues)
		{
			if (propertyValues == null)
			{
				return;
			}
			for (int i = 0; i < propertyValues.Length; i++)
			{
				this.ConvertPropertyValueFromClient(session, storageObjectProperties, ref propertyValues[i]);
			}
		}

		internal PropertyTag ConvertPropertyTagToClient(PropertyTag propertyTag, PropertyTag? originalTag)
		{
			PropertyConversion propertyConversion;
			PropertyTag result;
			if (this.propertyConversionToClientDictionary.TryGetValue(propertyTag.PropertyId, out propertyConversion) && propertyConversion.TryConvertPropertyTagToClient(propertyTag, originalTag, out result))
			{
				return result;
			}
			return propertyTag;
		}

		internal PropertyTag ConvertPropertyTagToClient(PropertyTag propertyTag)
		{
			return this.ConvertPropertyTagToClient(propertyTag, null);
		}

		internal void ConvertPropertyValueToClient(StoreSession session, IStorageObjectProperties storageObjectProperties, ref PropertyValue propertyValue, PropertyTag? originalTag)
		{
			PropertyConversion propertyConversion;
			if (this.propertyConversionToClientDictionary.TryGetValue(propertyValue.PropertyTag.PropertyId, out propertyConversion))
			{
				propertyConversion.TryConvertPropertyValueToClient(session, storageObjectProperties, originalTag, ref propertyValue);
			}
		}

		internal void ConvertPropertyValuesToClientAndSuppressClientSide(StoreSession session, IStorageObjectProperties storageObjectProperties, PropertyValue[] propertyValues, PropertyTag[] originalTags, ClientSideProperties clientSideProperties)
		{
			if (propertyValues == null)
			{
				return;
			}
			if (originalTags != null && propertyValues.Length != originalTags.Length)
			{
				throw new ArgumentException("PropertyValue[] length isn't the same as the PropertyTag[] length.");
			}
			for (int i = 0; i < propertyValues.Length; i++)
			{
				this.ConvertPropertyValueToClient(session, storageObjectProperties, ref propertyValues[i], (originalTags != null) ? new PropertyTag?(originalTags[i]) : null);
				if (!clientSideProperties.ShouldBeReturnedIfRequested(propertyValues[i].PropertyTag.PropertyId))
				{
					propertyValues[i] = new PropertyValue(new PropertyTag(propertyValues[i].PropertyTag.PropertyId, PropertyType.Error), (ErrorCode)2147746063U);
				}
			}
		}

		internal PropertyTag[] GetValidClientSideProperties(PropertyServerObject propertyServerObject, ICollection<PropertyDefinition> propertyDefinitions, bool useUnicodeType, PropertyTag[] additionalTags)
		{
			ICollection<PropertyDefinition> smartProperties = propertyServerObject.Schema.SmartProperties;
			List<PropertyTag> list = new List<PropertyTag>(propertyDefinitions.Count + ((additionalTags != null) ? additionalTags.Length : 0));
			list.AddRange(this.GetMappedPropertyTags(smartProperties));
			ICollection<PropertyTag> collection = MEDSPropertyTranslator.PropertyTagsFromPropertyDefinitions<PropertyDefinition>(propertyServerObject.Session, propertyDefinitions, useUnicodeType);
			foreach (PropertyTag propertyTag in collection)
			{
				if (!propertyServerObject.ClientSideProperties.ExcludeFromGetPropertyList(propertyTag.PropertyId))
				{
					list.Add(this.ConvertPropertyTagToClient(propertyTag));
				}
			}
			if (additionalTags != null)
			{
				foreach (PropertyTag propertyTag2 in additionalTags)
				{
					if (!propertyServerObject.ClientSideProperties.ExcludeFromGetPropertyList(propertyTag2.PropertyId))
					{
						PropertyTag propertyTag3 = this.ConvertPropertyTagToClient(propertyTag2);
						if (!list.Contains(propertyTag3, PropertyTag.PropertyIdComparer))
						{
							list.Add(propertyTag3);
						}
					}
				}
			}
			return list.ToArray();
		}

		internal bool TryGetMappedPropertyTag(PropertyDefinition propertyDefinition, out PropertyTag propertyTag)
		{
			return this.propertyDefinitionToPropertyTagMapping.TryGetValue(propertyDefinition, out propertyTag);
		}

		internal bool TryGetMappedPropertyDefinition(PropertyTag propertyTag, out PropertyDefinition propertyDefinition)
		{
			return this.propertyTagToPropertyDefinitionMapping.TryGetValue(propertyTag, out propertyDefinition);
		}

		internal IEnumerable<PropertyTag> GetMappedPropertyTags(IEnumerable<PropertyDefinition> propertyDefinitions)
		{
			return from propertyDefinition in propertyDefinitions
			where this.propertyDefinitionToPropertyTagMapping.ContainsKey(propertyDefinition)
			select this.propertyDefinitionToPropertyTagMapping[propertyDefinition];
		}

		internal IEnumerable<PropertyDefinition> GetMappedPropertyDefinitions(IEnumerable<PropertyTag> propertyTags)
		{
			return from propTag in propertyTags
			where this.propertyTagToPropertyDefinitionMapping.ContainsKey(propTag)
			select this.propertyTagToPropertyDefinitionMapping[propTag];
		}

		private static Dictionary<PropertyId, PropertyConversion> CreatePropertyConversionToClientDictionary(PropertyConversion[] conversions)
		{
			Dictionary<PropertyId, PropertyConversion> dictionary = new Dictionary<PropertyId, PropertyConversion>(conversions.Length, PropertyIdComparer.Instance);
			foreach (PropertyConversion propertyConversion in conversions)
			{
				dictionary.Add(propertyConversion.ServerPropertyTag.PropertyId, propertyConversion);
			}
			return dictionary;
		}

		private static PropertyConverter genericConverter = new PropertyConverter(Array<PropertyConverter.PropertyDefinitionMapping>.Empty, new PropertyConversion[]
		{
			new SentMailConversion(),
			new DamOrgMsgConversion(),
			new ParentIdConversion(),
			new ConflictMsgKeyConversion(),
			new RuleFolderIdConversion(),
			new MessageSubmissionIdConversion(),
			new ConversationMvItemIdsConversion(),
			new ConversationMvItemIdsMailboxWideConversion(),
			new LocalDirectoryEntryIdConversion()
		});

		private static PropertyConverter folderConverter = new PropertyConverter(Array<PropertyConverter.PropertyDefinitionMapping>.Empty, new PropertyConversion[]
		{
			new SentMailConversion(),
			new DamOrgMsgConversion(),
			new ParentIdConversion(),
			new ConflictMsgKeyConversion(),
			new RuleFolderIdConversion(),
			new MessageSubmissionIdConversion(),
			new ConversationMvItemIdsConversion(),
			new ConversationMvItemIdsMailboxWideConversion(),
			new LocalDirectoryEntryIdConversion(),
			new FolderSecurityDescriptorConversion()
		});

		private static PropertyConverter hierarchyViewConverter = new PropertyConverter(Array<PropertyConverter.PropertyDefinitionMapping>.Empty, new PropertyConversion[]
		{
			new SentMailConversion(),
			new DamOrgMsgConversion(),
			new ParentIdConversion(),
			new ConflictMsgKeyConversion(),
			new RuleFolderIdConversion(),
			new MessageSubmissionIdConversion(),
			new ConversationMvItemIdsConversion(),
			new ConversationMvItemIdsMailboxWideConversion(),
			new LocalDirectoryEntryIdConversion()
		});

		private static PropertyConverter logonConverter = new PropertyConverter(new PropertyConverter.PropertyDefinitionMapping[]
		{
			PropertyConverter.PropertyDefinitionMapping.InferenceOLKUserActivityLoggingEnabled
		}, new PropertyConversion[]
		{
			new SentMailConversion(),
			new DamOrgMsgConversion(),
			new ParentIdConversion(),
			new ConflictMsgKeyConversion(),
			new RuleFolderIdConversion(),
			new MessageSubmissionIdConversion(),
			new ConversationMvItemIdsConversion(),
			new ConversationMvItemIdsMailboxWideConversion(),
			new LocalDirectoryEntryIdConversion()
		});

		private readonly Dictionary<PropertyId, PropertyConversion> propertyConversionToClientDictionary;

		private readonly PropertyConversion[] propertyConversions;

		private readonly Dictionary<PropertyDefinition, PropertyTag> propertyDefinitionToPropertyTagMapping = new Dictionary<PropertyDefinition, PropertyTag>();

		private readonly Dictionary<PropertyTag, PropertyDefinition> propertyTagToPropertyDefinitionMapping = new Dictionary<PropertyTag, PropertyDefinition>();

		internal class PropertyDefinitionMapping
		{
			internal PropertyTag PropertyTag { get; private set; }

			internal PropertyDefinition PropertyDefinition { get; private set; }

			public PropertyDefinitionMapping(PropertyTag propertyTag, PropertyDefinition propertyDefinition)
			{
				Type type = NativeStorePropertyDefinition.ClrTypeFromPropertyTag(propertyTag);
				if (type != propertyDefinition.Type)
				{
					throw new ArgumentException(string.Format("Property type [propertyTag ({0})] must match with [propertyDefinition ({1})]", type, propertyDefinition.Type));
				}
				this.PropertyTag = propertyTag;
				this.PropertyDefinition = propertyDefinition;
			}

			internal static readonly PropertyConverter.PropertyDefinitionMapping InferenceOLKUserActivityLoggingEnabled = new PropertyConverter.PropertyDefinitionMapping(new PropertyTag(1748369419U), MailboxSchema.InferenceOLKUserActivityLoggingEnabled);
		}
	}
}
