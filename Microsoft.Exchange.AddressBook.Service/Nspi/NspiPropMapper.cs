using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Exchange.AddressBook.Service;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.UserPhotos;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AddressBook.Service;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.InfoWorker.Common.Availability.Proxy;
using Microsoft.Exchange.InfoWorker.Common.UserPhotos;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Mapi;

namespace Microsoft.Exchange.AddressBook.Nspi
{
	internal class NspiPropMapper
	{
		internal NspiPropMapper(NspiContext context, IList<PropTag> requestedProperties, int codePage) : this(context, requestedProperties, codePage, NspiPropMapperFlags.None)
		{
		}

		internal NspiPropMapper(NspiContext context, NspiGetTemplateInfoFlags templateFlags, int codePage) : this(context, NspiPropMapper.GetTemplateProperties(templateFlags), codePage, NspiPropMapperFlags.SkipMissingProperties | NspiPropMapperFlags.GetTemplateProps)
		{
		}

		internal NspiPropMapper(NspiContext context, IList<PropTag> requestedProperties, int codePage, NspiPropMapperFlags flags)
		{
			this.context = context;
			this.flags = flags;
			this.codePage = codePage;
			this.isHttp = this.context.IsUsingHttp;
			this.photosDataLogger = new PhotoRequestAddressbookLogger(this.context.ProtocolLogSession);
			this.substitutionEncoding = this.GetEncoding(false);
			if (requestedProperties != null)
			{
				this.requestedProperties = requestedProperties;
			}
			else
			{
				this.requestedProperties = NspiPropMapper.EmptyPropTagArray;
			}
			HashSet<ADPropertyDefinition> hashSet = new HashSet<ADPropertyDefinition>();
			hashSet.Add(ADObjectSchema.Guid);
			if ((flags & NspiPropMapperFlags.IncludeDisplayName) != NspiPropMapperFlags.None)
			{
				hashSet.Add(ADRecipientSchema.DisplayName);
			}
			if ((flags & NspiPropMapperFlags.IncludeHiddenFromAddressListsEnabled) != NspiPropMapperFlags.None)
			{
				hashSet.Add(ADRecipientSchema.HiddenFromAddressListsEnabled);
				hashSet.Add(SharedPropertyDefinitions.LegacyExchangeDN);
				hashSet.Add(ADRecipientSchema.RecipientTypeDetails);
			}
			NspiPropMapper.PropertyMapperTracer.TraceDebug<int>((long)context.ContextHandle, "Properties to map: {0}", this.requestedProperties.Count);
			for (int i = 0; i < this.requestedProperties.Count; i++)
			{
				NspiPropMapper.PropertyDefinitionEntry propertyDefinitionEntry = NspiPropMapper.GetPropertyDefinitionEntry(this.requestedProperties[i], (this.flags & NspiPropMapperFlags.GetTemplateProps) == NspiPropMapperFlags.GetTemplateProps);
				if (propertyDefinitionEntry != null)
				{
					if (this.requestedProperties[i].ValueType() == PropType.Unspecified)
					{
						this.requestedProperties[i] = propertyDefinitionEntry.PropTag;
					}
					if (propertyDefinitionEntry.ADPropertyDefinition != NspiPropMapper.NoLdapMapping)
					{
						hashSet.Add(propertyDefinitionEntry.ADPropertyDefinition);
						if (propertyDefinitionEntry.ADPropertyDefinition.IsSoftLinkAttribute && propertyDefinitionEntry.ADPropertyDefinition.Type == typeof(ADObjectId))
						{
							hashSet.Add(propertyDefinitionEntry.ADPropertyDefinition.SoftLinkShadowProperty);
						}
						NspiPropMapper.MapMethod mapMethod = propertyDefinitionEntry.MapMethod;
						switch (mapMethod)
						{
						case NspiPropMapper.MapMethod.GetEntryId:
						case NspiPropMapper.MapMethod.GetPermanentEntryId:
							hashSet.Add(ADRecipientSchema.RecipientType);
							break;
						default:
							switch (mapMethod)
							{
							case NspiPropMapper.MapMethod.GetDisplayName:
								hashSet.Add(SharedPropertyDefinitions.SimpleDisplayName);
								hashSet.Add(ADRecipientSchema.Alias);
								break;
							case NspiPropMapper.MapMethod.GetDisplayNamePrintable:
								hashSet.Add(ADRecipientSchema.Alias);
								break;
							case NspiPropMapper.MapMethod.GetMembers:
								hashSet.Add(ADGroupSchema.HiddenGroupMembershipEnabled);
								break;
							case NspiPropMapper.MapMethod.GetHomeMdb:
								hashSet.Add(SharedPropertyDefinitions.LegacyExchangeDN);
								hashSet.Add(ADRecipientSchema.PrimarySmtpAddress);
								hashSet.Add(ADMailboxRecipientSchema.ExchangeGuid);
								break;
							case NspiPropMapper.MapMethod.GetThumbnailPhoto:
								hashSet.Add(ADRecipientSchema.PrimarySmtpAddress);
								break;
							}
							break;
						}
						if (propertyDefinitionEntry.ADPropertyDefinition.Type == typeof(ADObjectId))
						{
							if (this.linkProperties == null)
							{
								this.linkProperties = new Dictionary<int, ADPropertyDefinition>(4);
							}
							this.linkProperties[this.requestedProperties[i].Id()] = propertyDefinitionEntry.ADPropertyDefinition;
						}
					}
				}
				NspiPropMapper.PropertyMapperTracer.TraceDebug<int, int, PropTag>((long)context.ContextHandle, "Property {0}: 0x{1:X8} {2}", i, (int)this.requestedProperties[i], this.requestedProperties[i]);
			}
			this.requestedPropDefs = new ADPropertyDefinition[hashSet.Count];
			int num = 0;
			foreach (ADPropertyDefinition adpropertyDefinition in hashSet)
			{
				this.requestedPropDefs[num++] = adpropertyDefinition;
			}
		}

		internal static ReadOnlyCollection<PropTag> SupportedPropTagsAnsi
		{
			get
			{
				return NspiPropMapper.supportedTagsAnsi;
			}
		}

		internal static ReadOnlyCollection<PropTag> SupportedPropTagsUnicode
		{
			get
			{
				return NspiPropMapper.supportedTagsUnicode;
			}
		}

		internal ADPropertyDefinition[] PropertyDefinitions
		{
			get
			{
				return this.requestedPropDefs;
			}
		}

		internal IList<PropTag> RequestedPropTags
		{
			get
			{
				return this.requestedProperties;
			}
		}

		internal Encoding NonsubstitutionEncoding
		{
			get
			{
				if (this.nonsubstitutionEncoding == null)
				{
					this.nonsubstitutionEncoding = this.GetEncoding(true);
				}
				return this.nonsubstitutionEncoding;
			}
		}

		internal static void Initialize()
		{
			NspiPropMapper.LoadSchema();
			NspiPropMapper.emptyTemplateScript = new byte[8];
			ExBitConverter.Write(1, NspiPropMapper.emptyTemplateScript, 0);
			ExBitConverter.Write(0, NspiPropMapper.emptyTemplateScript, 4);
		}

		internal static PropertyDefinition GetPropertyDefinition(PropTag propTag)
		{
			NspiPropMapper.PropertyDefinitionEntry propertyDefinitionEntry = NspiPropMapper.GetPropertyDefinitionEntry(propTag, false);
			if (propertyDefinitionEntry != null)
			{
				return propertyDefinitionEntry.ADPropertyDefinition;
			}
			return null;
		}

		internal static PropertyDefinition GetPropertyDefinitionForLinkedAttribute(PropTag propTag)
		{
			NspiPropMapper.PropertyDefinitionEntry propertyDefinitionEntry = NspiPropMapper.GetPropertyDefinitionEntry(propTag, false);
			if (propertyDefinitionEntry != null)
			{
				return propertyDefinitionEntry.LinkedPropertyDefinition;
			}
			return null;
		}

		internal PropRow GetProps(ADRawEntry adobject)
		{
			List<PropValue> list = new List<PropValue>(this.requestedProperties.Count);
			for (int i = 0; i < this.requestedProperties.Count; i++)
			{
				if ((this.flags & NspiPropMapperFlags.SkipObjects) == NspiPropMapperFlags.SkipObjects && this.requestedProperties[i].ValueType() == PropType.Object)
				{
					NspiPropMapper.PropertyMapperTracer.TraceDebug<int, int>((long)this.context.ContextHandle, "Property {0}: 0x{1:X8} -- SKIPPED (object)", i, (int)this.requestedProperties[i]);
				}
				else
				{
					PropValue prop = this.GetProp(adobject, this.requestedProperties[i]);
					if (prop.IsError() && (this.flags & NspiPropMapperFlags.SkipMissingProperties) == NspiPropMapperFlags.SkipMissingProperties)
					{
						NspiPropMapper.PropertyMapperTracer.TraceDebug<int, int>((long)this.context.ContextHandle, "Property {0}: 0x{1:X8} -- SKIPPED (missing)", i, (int)this.requestedProperties[i]);
					}
					else
					{
						list.Add(prop);
						NspiPropMapper.PropertyMapperTracer.TraceDebug<int, int, object>((long)this.context.ContextHandle, "Property {0}: 0x{1:X8} {2}", i, (int)this.requestedProperties[i], prop.RawValue);
					}
				}
			}
			return new PropRow(list);
		}

		internal PropRow GetErrorRow()
		{
			PropValue[] array = new PropValue[this.requestedProperties.Count];
			for (int i = 0; i < this.requestedProperties.Count; i++)
			{
				array[i] = new PropValue(PropTagHelper.ConvertToError(this.requestedProperties[i]), -2147221233);
			}
			return new PropRow(array);
		}

		internal void ResolveLinks(ICollection<ADRawEntry> entries)
		{
			this.ResolveLinks(entries, true);
		}

		internal void ResolveLinks(Result<ADRawEntry>[] results)
		{
			ADRawEntry[] entries = Array.ConvertAll<Result<ADRawEntry>, ADRawEntry>(results, (Result<ADRawEntry> x) => x.Data);
			this.ResolveLinks(entries, true);
		}

		internal void ResolveLinks(ICollection<ADRawEntry> entries, bool resetLinkDictionaries)
		{
			if (entries == null || entries.Count == 0)
			{
				return;
			}
			if (this.linkProperties == null || this.linkProperties.Count == 0)
			{
				return;
			}
			this.homeMdbLinkCollection = new HashSet<ADObjectId>();
			if (resetLinkDictionaries || this.linkDictionary == null)
			{
				this.linkDictionary = new Dictionary<ADObjectId, string>(entries.Count * this.linkProperties.Count);
			}
			foreach (ADRawEntry adrawEntry in entries)
			{
				if (adrawEntry != null)
				{
					this.GetLinkedADObjectIDs(adrawEntry);
				}
			}
			this.LookupLinks();
		}

		internal object ConvertStringWithoutSubstitions(PropTag tag, string input)
		{
			PropType propType = tag.ValueType();
			switch (propType)
			{
			case PropType.AnsiString:
				goto IL_30;
			case PropType.String:
				break;
			default:
				switch (propType)
				{
				case PropType.AnsiStringArray:
					goto IL_30;
				case PropType.StringArray:
					break;
				default:
					goto IL_44;
				}
				break;
			}
			return input;
			try
			{
				IL_30:
				return this.NonsubstitutionEncoding.GetBytes(input);
			}
			catch (EncoderFallbackException)
			{
				return null;
			}
			IL_44:
			throw new ArgumentException("tag");
		}

		internal object ConvertStringWithSubstitions(PropTag tag, string input)
		{
			PropType propType = tag.ValueType();
			switch (propType)
			{
			case PropType.AnsiString:
				goto IL_30;
			case PropType.String:
				break;
			default:
				switch (propType)
				{
				case PropType.AnsiStringArray:
					goto IL_30;
				case PropType.StringArray:
					break;
				default:
					throw new ArgumentException("tag");
				}
				break;
			}
			return input;
			IL_30:
			return this.substitutionEncoding.GetBytes(input);
		}

		internal void RewritePassThruProperties(PropRowSet rowset)
		{
			if (rowset == null)
			{
				return;
			}
			for (int i = 0; i < rowset.Rows.Count; i++)
			{
				for (int j = 0; j < rowset.Rows[i].Properties.Count; j++)
				{
					PropValue propValue = rowset.Rows[i].Properties[j];
					PropTag propTag = propValue.PropTag;
					EntryId entryId;
					if (propTag == PropTag.EntryId && EntryId.TryParse((byte[])propValue.Value, out entryId) && entryId.IsEphemeral)
					{
						entryId.ProviderGuid = this.context.Guid;
						rowset.Rows[i].Properties[j] = new PropValue(propValue.PropTag, entryId.ToByteArray());
					}
				}
			}
		}

		private static void LoadSchema()
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 709, "LoadSchema", "f:\\15.00.1497\\sources\\dev\\DoMT\\src\\Service\\NspiPropMapper.cs");
			tenantOrTopologyConfigurationSession.ServerTimeout = Configuration.ADTimeout;
			ADPagedReader<ADSchemaAttributeObject> adpagedReader = tenantOrTopologyConfigurationSession.FindPaged<ADSchemaAttributeObject>(tenantOrTopologyConfigurationSession.SchemaNamingContext, QueryScope.SubTree, new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.GreaterThan, ADSchemaAttributeSchema.MapiID, 0),
				new ComparisonFilter(ComparisonOperator.LessThanOrEqual, ADSchemaAttributeSchema.MapiID, 65535)
			}), null, 0);
			Dictionary<int, NspiPropMapper.PropertyDefinitionEntry> dictionary = new Dictionary<int, NspiPropMapper.PropertyDefinitionEntry>(500);
			Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
			foreach (ADSchemaAttributeObject adschemaAttributeObject in adpagedReader)
			{
				NspiPropMapper.PropertyMapperTracer.TraceDebug(0L, "Property 0x{0:X4}: {1} {2} {3} {4} {5}", new object[]
				{
					adschemaAttributeObject.MapiID,
					adschemaAttributeObject.LdapDisplayName,
					adschemaAttributeObject.DataSyntax,
					adschemaAttributeObject.IsSingleValued ? "Singlevalue" : "Multivalue",
					(adschemaAttributeObject.LinkID == 0) ? "!Link" : adschemaAttributeObject.LinkID.ToString(),
					adschemaAttributeObject.IsMemberOfPartialAttributeSet ? "GC" : "!GC"
				});
				int mapiID = adschemaAttributeObject.MapiID;
				if (dictionary.ContainsKey(mapiID))
				{
					NspiPropMapper.PropertyMapperTracer.TraceDebug(0L, "-- Duplicate property, skipped");
					dictionary[mapiID] = null;
				}
				else
				{
					NspiPropMapper.PropertyDefinitionEntry propertyDefinitionEntry = NspiPropMapper.CreatePropertyDefinitionEntry(adschemaAttributeObject);
					if (propertyDefinitionEntry != null)
					{
						dictionary.Add(mapiID, propertyDefinitionEntry);
						if (adschemaAttributeObject.LinkID != 0 && mapiID != ((PropTag)2147876877U).Id())
						{
							dictionary2.Add(adschemaAttributeObject.LinkID, mapiID);
						}
					}
				}
			}
			NspiPropMapper.UpdateMapping(dictionary, PropTag.InstanceKey, ADObjectSchema.Guid, NspiPropMapper.MapMethod.GetInstanceKey);
			NspiPropMapper.UpdateMapping(dictionary, PropTag.MappingSignature, NspiPropMapper.NoLdapMapping, NspiPropMapper.MapMethod.GetMappingSignature);
			NspiPropMapper.UpdateMapping(dictionary, PropTag.RecordKey, SharedPropertyDefinitions.LegacyExchangeDN, NspiPropMapper.MapMethod.GetPermanentEntryId);
			NspiPropMapper.UpdateMapping(dictionary, PropTag.ObjectType, ADRecipientSchema.RecipientType, NspiPropMapper.MapMethod.GetObjectType);
			NspiPropMapper.UpdateMapping(dictionary, PropTag.EntryId, SharedPropertyDefinitions.LegacyExchangeDN, NspiPropMapper.MapMethod.GetEntryId);
			NspiPropMapper.UpdateMapping(dictionary, PropTag.DisplayName, ADRecipientSchema.DisplayName, NspiPropMapper.MapMethod.GetDisplayName);
			NspiPropMapper.UpdateMapping(dictionary, PropTag.AddrType, NspiPropMapper.NoLdapMapping, NspiPropMapper.MapMethod.GetAddressType);
			NspiPropMapper.UpdateMapping(dictionary, PropTag.EmailAddress, SharedPropertyDefinitions.LegacyExchangeDN, NspiPropMapper.MapMethod.GetValue);
			NspiPropMapper.UpdateMapping(dictionary, PropTag.SearchKey, SharedPropertyDefinitions.LegacyExchangeDN, NspiPropMapper.MapMethod.GetSearchKey);
			NspiPropMapper.UpdateMapping(dictionary, PropTag.DisplayType, ADRecipientSchema.RecipientType, NspiPropMapper.MapMethod.GetDisplayType);
			NspiPropMapper.UpdateMapping(dictionary, PropTag.TemplateId, SharedPropertyDefinitions.LegacyExchangeDN, NspiPropMapper.MapMethod.GetPermanentEntryId);
			NspiPropMapper.UpdateMapping(dictionary, PropTag.DisplayNamePrintable, SharedPropertyDefinitions.SimpleDisplayName, NspiPropMapper.MapMethod.GetDisplayNamePrintable);
			NspiPropMapper.UpdateMapping(dictionary, PropTag.TransmitableDisplayName, ADRecipientSchema.DisplayName, NspiPropMapper.MapMethod.GetValue);
			NspiPropMapper.ChangeMapMethod(dictionary, (PropTag)2148073485U, NspiPropMapper.MapMethod.GetMembers);
			NspiPropMapper.UpdateMapping(dictionary, (PropTag)2148470815U, ADRecipientSchema.EmailAddresses, NspiPropMapper.MapMethod.GetProxyAddresses);
			NspiPropMapper.ChangeMapMethod(dictionary, (PropTag)2148991234U, NspiPropMapper.MapMethod.GetTemplate);
			NspiPropMapper.ChangeMapMethod(dictionary, (PropTag)2149056770U, NspiPropMapper.MapMethod.GetTemplateScript);
			NspiPropMapper.ChangeMapMethod(dictionary, (PropTag)2151157791U, NspiPropMapper.MapMethod.GetPfContacts);
			NspiPropMapper.UpdateMapping(dictionary, (PropTag)2151415839U, SharedPropertyDefinitions.LegacyExchangeDN, NspiPropMapper.MapMethod.GetValue);
			NspiPropMapper.ChangeMapMethod(dictionary, (PropTag)2152202270U, NspiPropMapper.MapMethod.GetTemplateAddressType);
			NspiPropMapper.ChangeMapMethod(dictionary, (PropTag)2358644766U, NspiPropMapper.MapMethod.GetAllRoomsLists);
			NspiPropMapper.UpdateMapping(dictionary, (PropTag)2147876877U, IADMailStorageSchema.Database, NspiPropMapper.MapMethod.GetHomeMdb);
			NspiPropMapper.UpdateMapping(dictionary, (PropTag)2147942413U, ADRecipientSchema.HomeMTA, NspiPropMapper.MapMethod.GetValue);
			NspiPropMapper.ChangeMapMethod(dictionary, (PropTag)2359165186U, NspiPropMapper.MapMethod.GetThumbnailPhoto);
			NspiPropMapper.UpdateMapping(dictionary, PropTag.ExchangeObjectId, ADObjectSchema.ExchangeObjectId, NspiPropMapper.MapMethod.GetGuidAsBinary);
			foreach (KeyValuePair<int, int> keyValuePair in dictionary2)
			{
				int key = keyValuePair.Key;
				int value = keyValuePair.Value;
				bool flag = key % 2 == 1;
				int num;
				if (flag)
				{
					dictionary2.TryGetValue(key - 1, out num);
				}
				else
				{
					dictionary2.TryGetValue(key + 1, out num);
				}
				NspiPropMapper.PropertyDefinitionEntry propertyDefinitionEntry;
				dictionary.TryGetValue(value, out propertyDefinitionEntry);
				NspiPropMapper.PropertyDefinitionEntry propertyDefinitionEntry2;
				dictionary.TryGetValue(num, out propertyDefinitionEntry2);
				bool flag2 = propertyDefinitionEntry != null && propertyDefinitionEntry2 != null;
				if (flag2)
				{
					propertyDefinitionEntry.LinkedPropertyDefinition = propertyDefinitionEntry2.ADPropertyDefinition;
					if (flag && propertyDefinitionEntry2.MemberOfGlobalCatalog)
					{
						propertyDefinitionEntry.MemberOfGlobalCatalog = true;
					}
				}
				NspiPropMapper.PropertyMapperTracer.TraceDebug(0L, "LinkId: {0}, property: 0x{1:X4} {2}, Linked property 0x{3:X4} {4}{5}", new object[]
				{
					key,
					value,
					(propertyDefinitionEntry == null) ? "(unknown)" : propertyDefinitionEntry.ADPropertyDefinition.LdapDisplayName,
					num,
					(propertyDefinitionEntry2 == null) ? "(unknown)" : propertyDefinitionEntry2.ADPropertyDefinition.LdapDisplayName,
					flag2 ? string.Empty : " -- skipped"
				});
			}
			List<PropTag> list = new List<PropTag>(dictionary.Count);
			List<PropTag> list2 = new List<PropTag>(dictionary.Count);
			foreach (KeyValuePair<int, NspiPropMapper.PropertyDefinitionEntry> keyValuePair2 in dictionary)
			{
				if (keyValuePair2.Value != null && keyValuePair2.Value.MemberOfGlobalCatalog)
				{
					list2.Add(keyValuePair2.Value.PropTag);
					PropType propType = keyValuePair2.Value.PropTag.ValueType();
					if (propType != PropType.String)
					{
						if (propType != PropType.StringArray)
						{
							list.Add(keyValuePair2.Value.PropTag);
						}
						else
						{
							list.Add(PropTagHelper.PropTagFromIdAndType(keyValuePair2.Key, PropType.AnsiStringArray));
						}
					}
					else
					{
						list.Add(PropTagHelper.PropTagFromIdAndType(keyValuePair2.Key, PropType.AnsiString));
					}
				}
			}
			list.Sort();
			list2.Sort();
			NspiPropMapper.staticMap = dictionary;
			NspiPropMapper.supportedTagsAnsi = new ReadOnlyCollection<PropTag>(list);
			NspiPropMapper.supportedTagsUnicode = new ReadOnlyCollection<PropTag>(list2);
		}

		private static void ChangeMapMethod(Dictionary<int, NspiPropMapper.PropertyDefinitionEntry> mapiDictionary, PropTag tag, NspiPropMapper.MapMethod mapMethod)
		{
			NspiPropMapper.PropertyDefinitionEntry propertyDefinitionEntry;
			if (mapiDictionary.TryGetValue(tag.Id(), out propertyDefinitionEntry))
			{
				propertyDefinitionEntry.MapMethod = mapMethod;
			}
		}

		private static void UpdateMapping(Dictionary<int, NspiPropMapper.PropertyDefinitionEntry> mapiDictionary, PropTag propTag, ADPropertyDefinition propDefinition, NspiPropMapper.MapMethod mapMethod)
		{
			NspiPropMapper.PropertyDefinitionEntry propertyDefinitionEntry;
			if (!mapiDictionary.TryGetValue(propTag.Id(), out propertyDefinitionEntry))
			{
				propertyDefinitionEntry = new NspiPropMapper.PropertyDefinitionEntry();
				mapiDictionary.Add(propTag.Id(), propertyDefinitionEntry);
			}
			propertyDefinitionEntry.PropTag = propTag;
			propertyDefinitionEntry.MapMethod = mapMethod;
			propertyDefinitionEntry.ADPropertyDefinition = propDefinition;
			propertyDefinitionEntry.MemberOfGlobalCatalog = true;
		}

		private static NspiPropMapper.PropertyDefinitionEntry CreatePropertyDefinitionEntry(ADSchemaAttributeObject mapiAttribute)
		{
			ADPropertyDefinitionFlags adpropertyDefinitionFlags = ADPropertyDefinitionFlags.None;
			Type typeFromHandle;
			PropType propType;
			object defaultValue;
			switch (mapiAttribute.DataSyntax)
			{
			case DataSyntax.Boolean:
				typeFromHandle = typeof(bool);
				propType = PropType.Boolean;
				defaultValue = false;
				goto IL_13B;
			case DataSyntax.Integer:
			case DataSyntax.Enumeration:
			case DataSyntax.LargeInteger:
				typeFromHandle = typeof(int);
				propType = PropType.Int;
				defaultValue = 0;
				adpropertyDefinitionFlags |= ADPropertyDefinitionFlags.PersistDefaultValue;
				goto IL_13B;
			case DataSyntax.Sid:
			case DataSyntax.Octet:
				typeFromHandle = typeof(byte[]);
				propType = PropType.Binary;
				defaultValue = null;
				adpropertyDefinitionFlags |= ADPropertyDefinitionFlags.Binary;
				goto IL_13B;
			case DataSyntax.Numeric:
			case DataSyntax.Printable:
			case DataSyntax.Teletex:
			case DataSyntax.IA5:
			case DataSyntax.CaseSensitive:
			case DataSyntax.Unicode:
				typeFromHandle = typeof(string);
				propType = PropType.String;
				defaultValue = string.Empty;
				goto IL_13B;
			case DataSyntax.UTCTime:
			case DataSyntax.GeneralizedTime:
				typeFromHandle = typeof(DateTime?);
				propType = PropType.SysTime;
				defaultValue = null;
				goto IL_13B;
			case DataSyntax.DNBinary:
			case DataSyntax.DNString:
			case DataSyntax.DSDN:
			case DataSyntax.ORName:
				typeFromHandle = typeof(ADObjectId);
				defaultValue = null;
				if (mapiAttribute.LinkID == 0)
				{
					propType = PropType.String;
					goto IL_13B;
				}
				propType = PropType.Object;
				if (mapiAttribute.LinkID % 2 == 1)
				{
					adpropertyDefinitionFlags |= ADPropertyDefinitionFlags.BackLink;
					goto IL_13B;
				}
				goto IL_13B;
			}
			NspiPropMapper.PropertyMapperTracer.TraceDebug<DataSyntax>(0L, "Unsupported DataSyntax {0} -- Skipped", mapiAttribute.DataSyntax);
			return null;
			IL_13B:
			if (!mapiAttribute.IsSingleValued)
			{
				PropType propType2 = propType;
				if (propType2 > PropType.Object)
				{
					if (propType2 != PropType.String)
					{
						if (propType2 == PropType.SysTime)
						{
							goto IL_188;
						}
						if (propType2 != PropType.Binary)
						{
							goto IL_19C;
						}
					}
					propType |= PropType.MultiValueFlag;
					goto IL_1A7;
				}
				if (propType2 != PropType.Int)
				{
					switch (propType2)
					{
					case PropType.Boolean:
						break;
					case (PropType)12:
						goto IL_19C;
					case PropType.Object:
						goto IL_1A7;
					default:
						goto IL_19C;
					}
				}
				IL_188:
				NspiPropMapper.PropertyMapperTracer.TraceDebug<PropType>(0L, "Unsupported multivalue property type {0} -- Skipped", propType);
				return null;
				IL_19C:
				throw new InvalidOperationException("Invalid PropType");
				IL_1A7:
				defaultValue = null;
				adpropertyDefinitionFlags |= ADPropertyDefinitionFlags.MultiValued;
				adpropertyDefinitionFlags &= ~ADPropertyDefinitionFlags.PersistDefaultValue;
			}
			ADPropertyDefinition adpropertyDefinition = new ADPropertyDefinition(mapiAttribute.LdapDisplayName, ExchangeObjectVersion.Exchange2003, typeFromHandle, mapiAttribute.LdapDisplayName, adpropertyDefinitionFlags, defaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
			return new NspiPropMapper.PropertyDefinitionEntry
			{
				PropTag = PropTagHelper.PropTagFromIdAndType(mapiAttribute.MapiID, propType),
				ADPropertyDefinition = adpropertyDefinition,
				MapMethod = NspiPropMapper.MapMethod.GetValue,
				MemberOfGlobalCatalog = mapiAttribute.IsMemberOfPartialAttributeSet
			};
		}

		private static IList<PropTag> GetTemplateProperties(NspiGetTemplateInfoFlags templateFlags)
		{
			List<PropTag> list = new List<PropTag>(5);
			if ((templateFlags & NspiGetTemplateInfoFlags.Template) != NspiGetTemplateInfoFlags.None)
			{
				list.Add((PropTag)2148991234U);
			}
			if ((templateFlags & NspiGetTemplateInfoFlags.Script) != NspiGetTemplateInfoFlags.None)
			{
				list.Add((PropTag)2149056770U);
			}
			if ((templateFlags & NspiGetTemplateInfoFlags.EmailType) != NspiGetTemplateInfoFlags.None)
			{
				list.Add((PropTag)2152202270U);
			}
			if ((templateFlags & NspiGetTemplateInfoFlags.HelpFileName) != NspiGetTemplateInfoFlags.None)
			{
				list.Add((PropTag)2151350302U);
			}
			if ((templateFlags & NspiGetTemplateInfoFlags.HelpFile) != NspiGetTemplateInfoFlags.None)
			{
				list.Add((PropTag)2148532482U);
			}
			return list;
		}

		private static PropTag ConvertPropTagToTemplatePropTag(PropTag tag)
		{
			int num = tag.Id();
			NspiGetTemplateInfoFlags propId;
			if (num == ((PropTag)2148991234U).Id())
			{
				propId = NspiGetTemplateInfoFlags.Template;
			}
			else if (num == ((PropTag)2149056770U).Id())
			{
				propId = NspiGetTemplateInfoFlags.Script;
			}
			else if (num == ((PropTag)2152202270U).Id())
			{
				propId = NspiGetTemplateInfoFlags.EmailType;
			}
			else if (num == ((PropTag)2151350302U).Id())
			{
				propId = NspiGetTemplateInfoFlags.HelpFileName;
			}
			else
			{
				if (num != ((PropTag)2148532482U).Id())
				{
					return tag;
				}
				propId = NspiGetTemplateInfoFlags.HelpFile;
			}
			return PropTagHelper.PropTagFromIdAndType((int)propId, tag.ValueType());
		}

		private static NspiPropMapper.PropertyDefinitionEntry GetPropertyDefinitionEntry(PropTag propTag, bool ignoreMemberOfGC)
		{
			NspiPropMapper.PropertyDefinitionEntry propertyDefinitionEntry;
			NspiPropMapper.staticMap.TryGetValue(propTag.Id(), out propertyDefinitionEntry);
			if (propertyDefinitionEntry != null && (ignoreMemberOfGC || propertyDefinitionEntry.MemberOfGlobalCatalog))
			{
				return propertyDefinitionEntry;
			}
			return null;
		}

		private static IMailboxSession GetCachedMailboxSessionForPhotoRequest(ExchangePrincipal user)
		{
			return null;
		}

		private void GetLinkedADObjectIDs(ADRawEntry entry)
		{
			foreach (KeyValuePair<int, ADPropertyDefinition> keyValuePair in this.linkProperties)
			{
				int key = keyValuePair.Key;
				ADPropertyDefinition value = keyValuePair.Value;
				bool flag = key == ((PropTag)2147876877U).Id();
				if (value.IsMultivalued)
				{
					MultiValuedPropertyBase multiValuedPropertyBase = entry[value] as MultiValuedPropertyBase;
					using (IEnumerator enumerator2 = ((IEnumerable)multiValuedPropertyBase).GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							object obj = enumerator2.Current;
							ADObjectId adobjectId = obj as ADObjectId;
							if (adobjectId != null && !this.linkDictionary.ContainsKey(adobjectId))
							{
								if (flag)
								{
									this.homeMdbLinkCollection.Add(adobjectId);
								}
								else
								{
									this.linkDictionary[adobjectId] = null;
								}
							}
						}
						continue;
					}
				}
				ADObjectId adobjectId2 = entry[value] as ADObjectId;
				if (adobjectId2 != null && !this.linkDictionary.ContainsKey(adobjectId2))
				{
					if (flag)
					{
						this.homeMdbLinkCollection.Add(adobjectId2);
					}
					else
					{
						this.linkDictionary[adobjectId2] = null;
					}
				}
			}
		}

		private void LookupLinks()
		{
			foreach (ADObjectId adobjectId in this.homeMdbLinkCollection)
			{
				LegacyDN legacyDN;
				try
				{
					ADObjectId adobjectId2;
					ActiveManager.GetCachingActiveManagerInstance().CalculatePreferredHomeServer(adobjectId.ObjectGuid, out legacyDN, out adobjectId2);
				}
				catch (DatabaseNotFoundException)
				{
					legacyDN = null;
				}
				if (legacyDN == null)
				{
					this.linkDictionary[adobjectId] = null;
				}
				else
				{
					this.linkDictionary[adobjectId] = Database.GetDatabaseLegacyDNFromRcaLegacyDN(legacyDN, false).ToString();
				}
			}
			for (int i = 0; i < 2; i++)
			{
				List<ADObjectId> list = new List<ADObjectId>();
				List<ADObjectId> list2 = new List<ADObjectId>();
				foreach (KeyValuePair<ADObjectId, string> keyValuePair in this.linkDictionary)
				{
					if (string.IsNullOrEmpty(keyValuePair.Value))
					{
						if (ADSession.IsTenantIdentity(keyValuePair.Key, (keyValuePair.Key.DomainId != null) ? keyValuePair.Key.GetPartitionId().ForestFQDN : PartitionId.LocalForest.ForestFQDN))
						{
							list2.Add(keyValuePair.Key);
						}
						else
						{
							list.Add(keyValuePair.Key);
						}
					}
				}
				if (list2.Count != 0)
				{
					IDirectorySession directorySession;
					if (i != 0)
					{
						IDirectorySession recipientSession = this.context.GetRecipientSession(null);
						directorySession = recipientSession;
					}
					else
					{
						directorySession = this.context.GetTenantSystemConfigurationSession();
					}
					IDirectorySession directorySession2 = directorySession;
					Result<ADRawEntry>[] array = directorySession2.FindByADObjectIds(list2.ToArray(), new PropertyDefinition[]
					{
						SharedPropertyDefinitions.LegacyExchangeDN
					});
					foreach (Result<ADRawEntry> result in array)
					{
						ADRawEntry data = result.Data;
						object obj;
						if (data != null && data.TryGetValueWithoutDefault(SharedPropertyDefinitions.LegacyExchangeDN, out obj))
						{
							this.linkDictionary[data.Id] = (string)obj;
						}
					}
				}
				if (list.Count != 0)
				{
					IDirectorySession directorySession3;
					if (i != 0)
					{
						IDirectorySession recipientSession2 = this.context.GetRecipientSession(null);
						directorySession3 = recipientSession2;
					}
					else
					{
						directorySession3 = this.context.GetRootOrgSystemConfigurationSession();
					}
					IDirectorySession directorySession4 = directorySession3;
					Result<ADRawEntry>[] array3 = directorySession4.FindByADObjectIds(list.ToArray(), new PropertyDefinition[]
					{
						SharedPropertyDefinitions.LegacyExchangeDN
					});
					foreach (Result<ADRawEntry> result2 in array3)
					{
						ADRawEntry data2 = result2.Data;
						object obj2;
						if (data2 != null && data2.TryGetValueWithoutDefault(SharedPropertyDefinitions.LegacyExchangeDN, out obj2))
						{
							this.linkDictionary[data2.Id] = (string)obj2;
						}
					}
				}
			}
		}

		private PropValue GetProp(ADRawEntry adRawEntry, PropTag tag)
		{
			NspiPropMapper.PropertyDefinitionEntry propertyDefinitionEntry;
			NspiPropMapper.staticMap.TryGetValue(tag.Id(), out propertyDefinitionEntry);
			if (propertyDefinitionEntry == null)
			{
				return new PropValue(PropTagHelper.ConvertToError(tag), -2147221233);
			}
			if (!propertyDefinitionEntry.TypesAreCompatible(tag))
			{
				return new PropValue(PropTagHelper.ConvertToError(tag), -2147221233);
			}
			if ((this.flags & NspiPropMapperFlags.GetTemplateProps) == NspiPropMapperFlags.GetTemplateProps)
			{
				tag = NspiPropMapper.ConvertPropTagToTemplatePropTag(tag);
			}
			else if (!propertyDefinitionEntry.MemberOfGlobalCatalog)
			{
				return new PropValue(PropTagHelper.ConvertToError(tag), -2147221233);
			}
			switch (propertyDefinitionEntry.MapMethod)
			{
			case NspiPropMapper.MapMethod.GetValue:
				return this.GetValue(adRawEntry, tag, propertyDefinitionEntry);
			case NspiPropMapper.MapMethod.GetAddressType:
				return new PropValue(tag, "EX");
			case NspiPropMapper.MapMethod.GetMappingSignature:
				return new PropValue(tag, EntryId.ExchangeProviderGuidByteArray);
			case NspiPropMapper.MapMethod.GetInstanceKey:
			{
				EphemeralIdTable.NamingContext namingContext = EphemeralIdTable.GetNamingContext(adRawEntry);
				Guid guid = (Guid)adRawEntry[propertyDefinitionEntry.ADPropertyDefinition];
				int value = this.context.EphemeralIdTable.CreateEphemeralId(guid, namingContext);
				byte[] array = new byte[4];
				ExBitConverter.Write(value, array, 0);
				return new PropValue(tag, array);
			}
			case NspiPropMapper.MapMethod.GetDisplayType:
				return new PropValue(tag, this.GetDisplayTypeEnum(adRawEntry));
			case NspiPropMapper.MapMethod.GetObjectType:
				return this.GetObjectType(adRawEntry, tag);
			case NspiPropMapper.MapMethod.GetEntryId:
				return this.GetEntryId(adRawEntry, tag);
			case NspiPropMapper.MapMethod.GetPermanentEntryId:
				return this.GetPermanentEntryId(adRawEntry, tag);
			case NspiPropMapper.MapMethod.GetSearchKey:
			{
				string text = "EX:" + (string)adRawEntry[propertyDefinitionEntry.ADPropertyDefinition];
				text = text.ToUpper(CultureInfo.InvariantCulture);
				byte[] bytes = Encoding.ASCII.GetBytes(text);
				Array.Resize<byte>(ref bytes, bytes.Length + 1);
				return new PropValue(tag, bytes);
			}
			case NspiPropMapper.MapMethod.GetTemplate:
				return this.GetTemplate(adRawEntry, tag, propertyDefinitionEntry);
			case NspiPropMapper.MapMethod.GetTemplateScript:
			{
				PropValue value2 = this.GetValue(adRawEntry, tag, propertyDefinitionEntry);
				if (value2.IsError())
				{
					value2 = new PropValue(tag, NspiPropMapper.emptyTemplateScript);
				}
				else if (value2.PropType == PropType.Binary)
				{
					byte[] bytes2 = value2.GetBytes();
					if (bytes2 != null)
					{
						int num = (bytes2.Length + 4 - 1) / 4;
						byte[] array2 = new byte[bytes2.Length + 4];
						int num2 = 255;
						for (int i = 0; i < 4; i++)
						{
							array2[i] = (byte)(num & num2);
							num >>= 8;
						}
						bytes2.CopyTo(array2, 4);
						value2 = new PropValue(tag, array2);
					}
				}
				return value2;
			}
			case NspiPropMapper.MapMethod.GetTemplateAddressType:
			{
				PropValue value2 = this.GetValue(adRawEntry, tag, propertyDefinitionEntry);
				if (value2.IsError())
				{
					value2 = new PropValue(tag, "EX");
				}
				return value2;
			}
			case NspiPropMapper.MapMethod.GetDisplayName:
				return this.GetDisplayName(adRawEntry, tag, propertyDefinitionEntry);
			case NspiPropMapper.MapMethod.GetDisplayNamePrintable:
				return this.GetDisplayNamePrintable(adRawEntry, tag, propertyDefinitionEntry);
			case NspiPropMapper.MapMethod.GetProxyAddresses:
				return this.GetProxyAddresses(adRawEntry, tag, propertyDefinitionEntry);
			case NspiPropMapper.MapMethod.GetMembers:
				return this.GetMembers(adRawEntry, tag, propertyDefinitionEntry);
			case NspiPropMapper.MapMethod.GetAllRoomsLists:
				return this.GetAllRoomsLists(adRawEntry, tag, propertyDefinitionEntry);
			case NspiPropMapper.MapMethod.GetPfContacts:
				return this.GetPfContacts(adRawEntry, tag, propertyDefinitionEntry);
			case NspiPropMapper.MapMethod.GetHomeMdb:
				if (this.isHttp && !adRawEntry.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2012))
				{
					return this.GetHomeMdb(adRawEntry, tag, propertyDefinitionEntry);
				}
				return this.GetValue(adRawEntry, tag, propertyDefinitionEntry);
			case NspiPropMapper.MapMethod.GetThumbnailPhoto:
				return this.GetThumbnailPhoto(adRawEntry, tag, propertyDefinitionEntry);
			case NspiPropMapper.MapMethod.GetGuidAsBinary:
				return this.GetGuidAsBinary(adRawEntry, tag, propertyDefinitionEntry);
			default:
				throw new InvalidOperationException();
			}
		}

		private PropValue GetValue(ADRawEntry adRawEntry, PropTag tag, NspiPropMapper.PropertyDefinitionEntry entry)
		{
			if (entry.ADPropertyDefinition == null)
			{
				return new PropValue(PropTagHelper.ConvertToError(tag), -2147221233);
			}
			object obj = null;
			bool flag = false;
			if (entry.ADPropertyDefinition.IsSoftLinkAttribute && entry.ADPropertyDefinition.Type == typeof(ADObjectId))
			{
				flag = adRawEntry.TryGetValueWithoutDefault(entry.ADPropertyDefinition.SoftLinkShadowProperty, out obj);
				if (flag)
				{
					obj = ADObjectId.FromSoftLinkValue((byte[])obj, null, OrganizationId.ForestWideOrgId);
				}
			}
			if (!flag && !adRawEntry.TryGetValueWithoutDefault(entry.ADPropertyDefinition, out obj))
			{
				if (entry.PropTag.Id() != PropTag.EmailAddress.Id() || !adRawEntry.TryGetValueWithoutDefault(ADObjectSchema.Id, out obj))
				{
					return new PropValue(PropTagHelper.ConvertToError(tag), -2147221233);
				}
				obj = this.GetLegacyDnFromObjectGuid(adRawEntry, obj);
			}
			MultiValuedPropertyBase multiValuedPropertyBase = null;
			if (tag.IsMultiValued())
			{
				multiValuedPropertyBase = (adRawEntry[entry.ADPropertyDefinition] as MultiValuedPropertyBase);
				if (multiValuedPropertyBase == null || multiValuedPropertyBase.Count == 0)
				{
					return new PropValue(PropTagHelper.ConvertToError(tag), -2147221233);
				}
			}
			PropType propType = tag.ValueType();
			if (propType <= PropType.String)
			{
				if (propType != PropType.Int)
				{
					switch (propType)
					{
					case PropType.Boolean:
						break;
					case (PropType)12:
						goto IL_2D6;
					case PropType.Object:
						return new PropValue(tag, null);
					default:
						switch (propType)
						{
						case PropType.AnsiString:
							return new PropValue(tag, this.GetAnsiStringValue(adRawEntry, tag, obj, entry));
						case PropType.String:
							return new PropValue(tag, this.GetUnicodeStringValue(adRawEntry, tag, obj, entry));
						default:
							goto IL_2D6;
						}
						break;
					}
				}
			}
			else if (propType <= PropType.Binary)
			{
				if (propType != PropType.SysTime && propType != PropType.Binary)
				{
					goto IL_2D6;
				}
			}
			else
			{
				switch (propType)
				{
				case PropType.AnsiStringArray:
				{
					byte[][] array = new byte[multiValuedPropertyBase.Count][];
					int num = 0;
					foreach (object value in ((IEnumerable)multiValuedPropertyBase))
					{
						array[num++] = this.GetAnsiStringValue(adRawEntry, tag, value, entry);
					}
					return new PropValue(tag, array);
				}
				case PropType.StringArray:
				{
					string[] array2 = new string[multiValuedPropertyBase.Count];
					int num = 0;
					foreach (object value2 in ((IEnumerable)multiValuedPropertyBase))
					{
						array2[num++] = this.GetUnicodeStringValue(adRawEntry, tag, value2, entry);
					}
					return new PropValue(tag, array2);
				}
				default:
				{
					if (propType != PropType.BinaryArray)
					{
						goto IL_2D6;
					}
					byte[][] array = new byte[multiValuedPropertyBase.Count][];
					int num = 0;
					foreach (object obj2 in ((IEnumerable)multiValuedPropertyBase))
					{
						array[num++] = (byte[])obj2;
					}
					return new PropValue(tag, array);
				}
				}
			}
			return new PropValue(tag, obj);
			IL_2D6:
			throw new InvalidOperationException("Property type is not supported");
		}

		private string GetUnicodeStringValue(ADRawEntry adRawEntry, PropTag tag, object value, NspiPropMapper.PropertyDefinitionEntry entry)
		{
			if (!(entry.ADPropertyDefinition.Type == typeof(ADObjectId)))
			{
				return (string)value;
			}
			if (this.linkDictionary == null)
			{
				throw new InvalidOperationException("linkDictionary must not be null");
			}
			string text = this.linkDictionary[(ADObjectId)value];
			if (text == null)
			{
				text = this.GetLegacyDnFromObjectGuid(adRawEntry, value);
				this.linkDictionary[(ADObjectId)value] = text;
			}
			return text;
		}

		private byte[] GetAnsiStringValue(ADRawEntry adRawEntry, PropTag tag, object value, NspiPropMapper.PropertyDefinitionEntry entry)
		{
			string unicodeStringValue = this.GetUnicodeStringValue(adRawEntry, tag, value, entry);
			NspiPropMapper.PropertyMapperTracer.TraceDebug<int, string>((long)this.context.ContextHandle, "Returning Ansi value for property 0x{0:X8} {1}", (int)tag, unicodeStringValue);
			return (byte[])this.ConvertStringWithSubstitions(tag, unicodeStringValue);
		}

		private string GetLegacyDnFromObjectGuid(ADRawEntry adRawEntry, object value)
		{
			EphemeralIdTable.NamingContext namingContext = EphemeralIdTable.GetNamingContext(adRawEntry);
			Guid namingContext2;
			if (namingContext == EphemeralIdTable.NamingContext.Config)
			{
				namingContext2 = this.context.ConfigNamingContext.ObjectGuid;
			}
			else if (namingContext == EphemeralIdTable.NamingContext.TenantConfig)
			{
				namingContext2 = (adRawEntry.Id.IsDescendantOf(ADSession.GetConfigurationNamingContextForLocalForest()) ? this.context.ConfigNamingContext.ObjectGuid : this.context.DomainNamingContext.ObjectGuid);
			}
			else
			{
				namingContext2 = this.context.DomainNamingContext.ObjectGuid;
			}
			this.context.EphemeralIdTable.CreateEphemeralId(((ADObjectId)value).ObjectGuid, namingContext);
			return LegacyDN.FormatLegacyDnFromGuid(namingContext2, ((ADObjectId)value).ObjectGuid);
		}

		private PropValue GetEntryId(ADRawEntry adRawEntry, PropTag tag)
		{
			if ((this.flags & NspiPropMapperFlags.UseEphemeralId) == NspiPropMapperFlags.UseEphemeralId)
			{
				Guid guid = (Guid)adRawEntry[ADObjectSchema.Guid];
				int ephemeralId = this.context.EphemeralIdTable.CreateEphemeralId(guid, EphemeralIdTable.GetNamingContext(adRawEntry));
				EntryId entryId = new EntryId(this.GetDisplayTypeEnum(adRawEntry), this.context.Guid, ephemeralId);
				return new PropValue(tag, entryId.ToByteArray());
			}
			return this.GetPermanentEntryId(adRawEntry, tag);
		}

		private PropValue GetPermanentEntryId(ADRawEntry adRawEntry, PropTag tag)
		{
			EntryId.DisplayType displayTypeEnum = this.GetDisplayTypeEnum(adRawEntry);
			string text = (string)adRawEntry[SharedPropertyDefinitions.LegacyExchangeDN];
			if (string.IsNullOrEmpty(text))
			{
				text = this.GetLegacyDnFromObjectGuid(adRawEntry, (ADObjectId)adRawEntry[ADObjectSchema.Id]);
			}
			EntryId entryId = new EntryId(displayTypeEnum, text);
			return new PropValue(tag, entryId.ToByteArray());
		}

		private PropValue GetObjectType(ADRawEntry adRawEntry, PropTag tag)
		{
			ObjectType objectType;
			switch ((RecipientType)adRawEntry[ADRecipientSchema.RecipientType])
			{
			case RecipientType.MailUser:
				objectType = ObjectType.MAPI_MAILUSER;
				goto IL_4F;
			case RecipientType.MailUniversalDistributionGroup:
			case RecipientType.MailUniversalSecurityGroup:
			case RecipientType.MailNonUniversalGroup:
				objectType = ObjectType.MAPI_DISTLIST;
				goto IL_4F;
			case RecipientType.PublicFolder:
				objectType = ObjectType.MAPI_FOLDER;
				goto IL_4F;
			}
			objectType = ObjectType.MAPI_MAILUSER;
			IL_4F:
			return new PropValue(tag, objectType);
		}

		private EntryId.DisplayType GetDisplayTypeEnum(ADRawEntry entry)
		{
			switch ((RecipientType)entry[ADRecipientSchema.RecipientType])
			{
			case RecipientType.User:
			case RecipientType.MailUser:
			case RecipientType.Contact:
			case RecipientType.MailContact:
				return EntryId.DisplayType.RemoteMailUser;
			case RecipientType.UserMailbox:
			case RecipientType.SystemMailbox:
				return EntryId.DisplayType.MailUser;
			case RecipientType.MailUniversalDistributionGroup:
			case RecipientType.MailUniversalSecurityGroup:
			case RecipientType.MailNonUniversalGroup:
				return EntryId.DisplayType.DistList;
			case RecipientType.DynamicDistributionGroup:
				return EntryId.DisplayType.Agent;
			case RecipientType.PublicFolder:
				return EntryId.DisplayType.Forum;
			}
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)entry[ADObjectSchema.ObjectClass];
			if (multiValuedProperty.Contains("organizationalUnit"))
			{
				return EntryId.DisplayType.Organization;
			}
			return EntryId.DisplayType.Agent;
		}

		private PropValue GetDisplayName(ADRawEntry adRawEntry, PropTag tag, NspiPropMapper.PropertyDefinitionEntry entry)
		{
			object obj;
			if (adRawEntry.TryGetValueWithoutDefault(ADRecipientSchema.DisplayName, out obj))
			{
				object obj2 = this.ConvertStringWithoutSubstitions(tag, (string)obj);
				if (obj2 != null)
				{
					return new PropValue(tag, obj2);
				}
			}
			return this.GetDisplayNamePrintable(adRawEntry, tag, entry);
		}

		private PropValue GetDisplayNamePrintable(ADRawEntry adRawEntry, PropTag tag, NspiPropMapper.PropertyDefinitionEntry entry)
		{
			object obj;
			if (adRawEntry.TryGetValueWithoutDefault(SharedPropertyDefinitions.SimpleDisplayName, out obj))
			{
				object obj2 = this.ConvertStringWithoutSubstitions(tag, (string)obj);
				if (obj2 != null)
				{
					return new PropValue(tag, obj2);
				}
			}
			if (adRawEntry.TryGetValueWithoutDefault(ADRecipientSchema.Alias, out obj))
			{
				object obj2 = this.ConvertStringWithoutSubstitions(tag, (string)obj);
				if (obj2 != null)
				{
					return new PropValue(tag, obj2);
				}
			}
			return new PropValue(PropTagHelper.ConvertToError(tag), -2147221233);
		}

		private PropValue GetTemplate(ADRawEntry adRawEntry, PropTag tag, NspiPropMapper.PropertyDefinitionEntry entry)
		{
			object obj;
			if (entry.ADPropertyDefinition == null || !adRawEntry.TryGetValueWithoutDefault(entry.ADPropertyDefinition, out obj))
			{
				return new PropValue(PropTagHelper.ConvertToError(tag), -2147221233);
			}
			byte[] array = (byte[])obj;
			BufferBuilder bufferBuilder = new BufferBuilder(array.Length);
			GCHandle gchandle = GCHandle.Alloc(array, GCHandleType.Pinned);
			try
			{
				IntPtr ptr = gchandle.AddrOfPinnedObject();
				int num = Marshal.ReadInt32(ptr, 0);
				int num2 = Marshal.ReadInt32(ptr, 4);
				if (num != 1)
				{
					return new PropValue(PropTagHelper.ConvertToError(tag), -2147221233);
				}
				int count = 8 + num2 * 36;
				bufferBuilder.Append(array, 0, count);
				for (int i = 0; i < num2; i++)
				{
					int num3 = 8 + i * 36 + 32;
					int index = Marshal.ReadInt32(ptr, num3);
					IntPtr ptr2 = Marshal.UnsafeAddrOfPinnedArrayElement(array, index);
					string s = Marshal.PtrToStringUni(ptr2);
					int length = bufferBuilder.Length;
					ExBitConverter.Write(length, bufferBuilder.GetBuffer(), num3);
					byte[] bytes = this.substitutionEncoding.GetBytes(s);
					bufferBuilder.Append(bytes);
					bufferBuilder.Append(0);
				}
			}
			finally
			{
				gchandle.Free();
			}
			bufferBuilder.RemoveUnusedBufferSpace();
			return new PropValue(tag, bufferBuilder.GetBuffer());
		}

		private PropValue GetProxyAddresses(ADRawEntry adRawEntry, PropTag tag, NspiPropMapper.PropertyDefinitionEntry entry)
		{
			object obj;
			if (!adRawEntry.TryGetValueWithoutDefault(entry.ADPropertyDefinition, out obj))
			{
				return new PropValue(PropTagHelper.ConvertToError(tag), -2147221233);
			}
			ProxyAddressCollection proxyAddressCollection = (ProxyAddressCollection)obj;
			string[] array = new string[proxyAddressCollection.Count];
			int num = -1;
			int num2 = -1;
			for (int i = 0; i < proxyAddressCollection.Count; i++)
			{
				ProxyAddress proxyAddress = proxyAddressCollection[i];
				array[i] = proxyAddress.ToString();
				if (proxyAddress.IsPrimaryAddress)
				{
					if (num == -1 && proxyAddress.Prefix == ProxyAddressPrefix.Smtp)
					{
						num = i;
					}
					else if (num2 == -1 && proxyAddress.Prefix == NspiPropMapper.AutodiscoverPrefix)
					{
						num2 = i;
					}
				}
			}
			if (num2 != -1 && num != -1)
			{
				try
				{
					string text = proxyAddressCollection[num2].AddressString;
					text = new SmtpProxyAddress(text, true).ToString();
					string text2 = proxyAddressCollection[num].AddressString;
					text2 = new SmtpProxyAddress(text2, false).ToString();
					array[num2] = text;
					array[num] = text2;
				}
				catch (ArgumentOutOfRangeException ex)
				{
					NspiPropMapper.PropertyMapperTracer.TraceError<ProxyAddress, ProxyAddress, string>((long)this.context.ContextHandle, "Could not convert {0} or {1}: {2}", proxyAddressCollection[num], proxyAddressCollection[num2], ex.Message);
				}
			}
			return new PropValue(tag, array);
		}

		private PropValue GetMembers(ADRawEntry adRawEntry, PropTag tag, NspiPropMapper.PropertyDefinitionEntry entry)
		{
			if ((bool)adRawEntry[ADGroupSchema.HiddenGroupMembershipEnabled])
			{
				return new PropValue(PropTagHelper.ConvertToError(tag), -2147221233);
			}
			return this.GetValue(adRawEntry, tag, entry);
		}

		private PropValue GetPfContacts(ADRawEntry adRawEntry, PropTag tag, NspiPropMapper.PropertyDefinitionEntry entry)
		{
			MultiValuedPropertyBase multiValuedPropertyBase = adRawEntry[entry.ADPropertyDefinition] as MultiValuedPropertyBase;
			if (multiValuedPropertyBase != null)
			{
				List<string> list = new List<string>();
				foreach (object obj in ((IEnumerable)multiValuedPropertyBase))
				{
					ADObjectId adobjectId = obj as ADObjectId;
					if (adobjectId != null && !adobjectId.IsDeleted)
					{
						list.Add(adobjectId.DistinguishedName);
					}
				}
				if (list.Count > 0)
				{
					return new PropValue(tag, list.ToArray());
				}
			}
			return new PropValue(PropTagHelper.ConvertToError(tag), -2147221233);
		}

		private PropValue GetAllRoomsLists(ADRawEntry adRawEntry, PropTag tag, NspiPropMapper.PropertyDefinitionEntry entry)
		{
			if (this.context.NspiPrincipal.AddressBookPolicy == null)
			{
				return this.GetValue(adRawEntry, tag, entry);
			}
			if (this.context.NspiPrincipal.AllRoomsListFromAddressBookPolicy != null)
			{
				string text = LegacyDN.FormatLegacyDnFromGuid(this.context.ConfigNamingContext.ObjectGuid, this.context.NspiPrincipal.AllRoomsListFromAddressBookPolicy.ObjectGuid);
				string[] value = new string[]
				{
					text
				};
				return new PropValue(tag, value);
			}
			return new PropValue(PropTagHelper.ConvertToError(tag), -2147221233);
		}

		private PropValue GetHomeMdb(ADRawEntry adRawEntry, PropTag tag, NspiPropMapper.PropertyDefinitionEntry entry)
		{
			object obj;
			object obj2;
			object obj3;
			if (adRawEntry.TryGetValueWithoutDefault(SharedPropertyDefinitions.LegacyExchangeDN, out obj) && adRawEntry.TryGetValueWithoutDefault(ADRecipientSchema.PrimarySmtpAddress, out obj2) && adRawEntry.TryGetValueWithoutDefault(ADMailboxRecipientSchema.ExchangeGuid, out obj3))
			{
				string text = (string)obj;
				SmtpAddress smtpAddress = (SmtpAddress)obj2;
				Guid mailboxGuid = (Guid)obj3;
				int num = text.IndexOf("/cn=", StringComparison.OrdinalIgnoreCase);
				if (num != -1)
				{
					string str = text.Substring(0, num);
					string value = str + "/cn=Configuration/cn=Servers/cn=" + ExchangeRpcClientAccess.CreatePersonalizedServer(mailboxGuid, smtpAddress.Domain) + "/cn=Microsoft Private MDB";
					return new PropValue(tag, value);
				}
			}
			return new PropValue(PropTagHelper.ConvertToError(tag), -2147221233);
		}

		private PropValue GetThumbnailPhoto(ADRawEntry adRawEntry, PropTag tag, NspiPropMapper.PropertyDefinitionEntry entry)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			PropValue result;
			try
			{
				result = this.GetThumbnailPhotoFromMailbox(adRawEntry, tag, entry);
			}
			finally
			{
				stopwatch.Stop();
				AddressBookPerformanceCountersWrapper.AddressBookPerformanceCounters.ThumbnailPhotoAverageTime.IncrementBy(stopwatch.ElapsedTicks);
				AddressBookPerformanceCountersWrapper.AddressBookPerformanceCounters.ThumbnailPhotoAverageTimeBase.Increment();
			}
			if (!result.IsError())
			{
				AddressBookPerformanceCountersWrapper.AddressBookPerformanceCounters.ThumbnailPhotoFromMailboxCount.Increment();
				return result;
			}
			result = this.GetValue(adRawEntry, tag, entry);
			if (result.IsError())
			{
				AddressBookPerformanceCountersWrapper.AddressBookPerformanceCounters.ThumbnailPhotoNotPresentCount.Increment();
			}
			else
			{
				AddressBookPerformanceCountersWrapper.AddressBookPerformanceCounters.ThumbnailPhotoFromDirectoryCount.Increment();
			}
			return result;
		}

		private PropValue GetThumbnailPhotoFromMailbox(ADRawEntry adRawEntry, PropTag tag, NspiPropMapper.PropertyDefinitionEntry entry)
		{
			if (!NspiPropMapper.HDPhotoEnabled.Value)
			{
				return NspiPropMapper.ThumbnailPhotoError;
			}
			ClientContext clientContext = ClientContext.Create(this.context.ClientSecurityContext, this.context.NspiPrincipal.OrganizationId, null, null, CultureInfo.InvariantCulture, Guid.NewGuid().ToString());
			clientContext.RequestSchemaVersion = ExchangeVersionType.Exchange2012;
			string text = adRawEntry[ADRecipientSchema.PrimarySmtpAddress].ToString();
			if (!SmtpAddress.IsValidSmtpAddress(text))
			{
				NspiPropMapper.PropertyMapperTracer.TraceError<string>((long)this.context.ContextHandle, "Target SMTP address is not valid: {0}", text);
				return NspiPropMapper.ThumbnailPhotoError;
			}
			PropValue result;
			try
			{
				GetUserPhotoQuery getUserPhotoQuery = new GetUserPhotoQuery(clientContext, new PhotoRequest
				{
					Requestor = new PhotoPrincipal
					{
						EmailAddresses = new string[]
						{
							this.context.NspiPrincipal.PrimarySmtpAddress.ToString()
						},
						OrganizationId = this.context.NspiPrincipal.OrganizationId
					},
					Size = NspiPropMapper.HDPhotoSize.Value,
					TargetSmtpAddress = text,
					PerformanceLogger = this.photosDataLogger,
					HostOwnedTargetMailboxSessionGetter = new Func<ExchangePrincipal, IMailboxSession>(NspiPropMapper.GetCachedMailboxSessionForPhotoRequest),
					Trace = NspiPropMapper.PropertyMapperTracer.IsTraceEnabled(TraceType.DebugTrace)
				}, null, false, NspiPropMapper.PhotosConfiguration, NspiPropMapper.PropertyMapperTracer);
				byte[] array = getUserPhotoQuery.Execute();
				if (array == null || array.Length == 0)
				{
					NspiPropMapper.PropertyMapperTracer.TraceError<string>((long)this.context.ContextHandle, "Unable to retrieve thumbnailPhoto via GetUserPhoto for {0}", text);
					result = NspiPropMapper.ThumbnailPhotoError;
				}
				else
				{
					this.LogPhotoRequestData(getUserPhotoQuery, text);
					result = new PropValue(tag, array);
				}
			}
			catch (FileNotFoundException arg)
			{
				NspiPropMapper.PropertyMapperTracer.TraceError<string, FileNotFoundException>((long)this.context.ContextHandle, "File not found retrieving thumbnailPhoto via GetUserPhoto for {0}.  Exception: {1}", text, arg);
				result = NspiPropMapper.ThumbnailPhotoError;
			}
			catch (AccessDeniedException arg2)
			{
				NspiPropMapper.PropertyMapperTracer.TraceError<string, AccessDeniedException>((long)this.context.ContextHandle, "Access denied retrieving thumbnailPhoto via GetUserPhoto for {0}.  Exception: {1}", text, arg2);
				result = NspiPropMapper.ThumbnailPhotoError;
			}
			return result;
		}

		private PropValue GetGuidAsBinary(ADRawEntry adRawEntry, PropTag tag, NspiPropMapper.PropertyDefinitionEntry entry)
		{
			object obj;
			if (entry.ADPropertyDefinition == null || !adRawEntry.TryGetValueWithoutDefault(entry.ADPropertyDefinition, out obj) || obj == null || !(obj is Guid))
			{
				return new PropValue(PropTagHelper.ConvertToError(tag), -2147221233);
			}
			return new PropValue(tag, ((Guid)obj).ToByteArray());
		}

		private Encoding GetEncoding(bool raiseException)
		{
			Encoding encoding;
			try
			{
				if (raiseException)
				{
					encoding = Encoding.GetEncoding(this.codePage, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
				}
				else
				{
					encoding = Encoding.GetEncoding(this.codePage);
				}
			}
			catch (ArgumentOutOfRangeException inner)
			{
				throw new NspiException(NspiStatus.InvalidCodePage, "codePage", inner);
			}
			catch (ArgumentException inner2)
			{
				throw new NspiException(NspiStatus.InvalidCodePage, "codePage", inner2);
			}
			catch (NotSupportedException inner3)
			{
				throw new NspiException(NspiStatus.InvalidCodePage, "codePage", inner3);
			}
			return encoding;
		}

		private void LogPhotoRequestData(GetUserPhotoQuery query, string targetPrimarySmtpAddress)
		{
			if (query != null && query.RequestLogger != null && query.RequestLogger.LogData != null)
			{
				string log = query.RequestLogger.LogData.ToString();
				this.photosDataLogger.AppendToLog(this.GetRequestTypeFromRoutingAndDiscoveryLog(log));
			}
			if (query != null && query.StatusCode != HttpStatusCode.OK && query.StatusCode != HttpStatusCode.NotModified && query.StatusCode != HttpStatusCode.Forbidden && query.StatusCode != HttpStatusCode.NotFound)
			{
				this.photosDataLogger.AppendToLog(string.Format(CultureInfo.InvariantCulture, "Status={0}", new object[]
				{
					query.StatusCode
				}));
				this.photosDataLogger.AppendToLog(string.Format(CultureInfo.InvariantCulture, "TgEm={0}", new object[]
				{
					targetPrimarySmtpAddress
				}));
			}
		}

		private string GetRequestTypeFromRoutingAndDiscoveryLog(string log)
		{
			char c = ';';
			char c2 = '=';
			string[] array = log.Split(new char[]
			{
				c
			});
			foreach (string text in array)
			{
				string text2 = text.Split(new char[]
				{
					c2
				})[0];
				foreach (string value in NspiPropMapper.PhotoRequestType)
				{
					if (text2.Equals(value, StringComparison.InvariantCultureIgnoreCase))
					{
						return text;
					}
				}
			}
			return null;
		}

		private static readonly ADPropertyDefinition NoLdapMapping = null;

		private static readonly PropTag[] EmptyPropTagArray = new PropTag[0];

		private static readonly Microsoft.Exchange.Diagnostics.Trace PropertyMapperTracer = ExTraceGlobals.PropertyMapperTracer;

		private static readonly ProxyAddressPrefix AutodiscoverPrefix = new CustomProxyAddressPrefix("AUTOD");

		private static readonly BoolAppSettingsEntry HDPhotoEnabled = new BoolAppSettingsEntry("HDPhotoEnabled", false, NspiPropMapper.PropertyMapperTracer);

		private static readonly EnumAppSettingsEntry<UserPhotoSize> HDPhotoSize = new EnumAppSettingsEntry<UserPhotoSize>("HDPhotoSize", UserPhotoSize.HR96x96, NspiPropMapper.PropertyMapperTracer);

		private static readonly PropValue ThumbnailPhotoError = new PropValue(PropTagHelper.ConvertToError((PropTag)2359165186U), -2147221233);

		private static readonly PhotosConfiguration PhotosConfiguration = new PhotosConfiguration(ExchangeSetupContext.InstallPath);

		private static readonly string[] PhotoRequestType = new string[]
		{
			"local",
			"intrasiteproxy",
			"x-site",
			"x-forest",
			"federatedxforest",
			"PF"
		};

		private static byte[] emptyTemplateScript;

		private static Dictionary<int, NspiPropMapper.PropertyDefinitionEntry> staticMap = new Dictionary<int, NspiPropMapper.PropertyDefinitionEntry>(500);

		private static ReadOnlyCollection<PropTag> supportedTagsAnsi;

		private static ReadOnlyCollection<PropTag> supportedTagsUnicode;

		private readonly NspiPropMapperFlags flags;

		private readonly NspiContext context;

		private readonly ADPropertyDefinition[] requestedPropDefs;

		private readonly Dictionary<int, ADPropertyDefinition> linkProperties;

		private readonly int codePage;

		private readonly Encoding substitutionEncoding;

		private readonly bool isHttp;

		private Dictionary<ADObjectId, string> linkDictionary;

		private HashSet<ADObjectId> homeMdbLinkCollection;

		private Encoding nonsubstitutionEncoding;

		private IList<PropTag> requestedProperties;

		private PhotoRequestAddressbookLogger photosDataLogger;

		internal enum MapMethod
		{
			GetValue,
			GetAddressType,
			GetMappingSignature,
			GetInstanceKey,
			GetDisplayType,
			GetObjectType,
			GetEntryId,
			GetPermanentEntryId,
			GetSearchKey,
			GetTemplate,
			GetTemplateScript,
			GetTemplateAddressType,
			GetDisplayName,
			GetDisplayNamePrintable,
			GetProxyAddresses,
			GetMembers,
			GetAllRoomsLists,
			GetPfContacts,
			GetHomeMdb,
			GetThumbnailPhoto,
			GetGuidAsBinary
		}

		internal class PropertyDefinitionEntry
		{
			internal PropertyDefinitionEntry()
			{
			}

			internal PropTag PropTag
			{
				get
				{
					return this.propTag;
				}
				set
				{
					this.propTag = NspiPropMapper.PropertyDefinitionEntry.NormalizePropTag(value);
				}
			}

			internal ADPropertyDefinition ADPropertyDefinition { get; set; }

			internal ADPropertyDefinition LinkedPropertyDefinition { get; set; }

			internal NspiPropMapper.MapMethod MapMethod { get; set; }

			internal bool MemberOfGlobalCatalog { get; set; }

			internal bool TypesAreCompatible(PropType type)
			{
				type = NspiPropMapper.PropertyDefinitionEntry.NormalizePropType(type);
				return this.propTag.ValueType() == type || (this.propTag.ValueType() == PropType.Object && this.ADPropertyDefinition != null && ((type == PropType.String && !this.ADPropertyDefinition.IsMultivalued) || (type == PropType.StringArray && this.ADPropertyDefinition.IsMultivalued)));
			}

			internal bool TypesAreCompatible(PropTag tag)
			{
				return this.TypesAreCompatible(tag.ValueType());
			}

			private static PropType NormalizePropType(PropType type)
			{
				if (type == PropType.AnsiString)
				{
					return PropType.String;
				}
				if (type == PropType.AnsiStringArray)
				{
					return PropType.StringArray;
				}
				return type;
			}

			private static PropTag NormalizePropTag(PropTag tag)
			{
				return PropTagHelper.PropTagFromIdAndType(tag.Id(), NspiPropMapper.PropertyDefinitionEntry.NormalizePropType(tag.ValueType()));
			}

			private PropTag propTag;
		}
	}
}
