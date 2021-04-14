using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Nspi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NspiPropertyMap
	{
		private NspiPropertyMap(ADPropertyDefinition[] adProperties, int[] nspiProperties, Encoding encoding)
		{
			this.adProperties = adProperties;
			this.nspiProperties = nspiProperties;
			this.encoding = encoding;
		}

		public static NspiPropertyMap Create(PropertyDefinition[] properties, Encoding encoding)
		{
			NspiPropertyMap.Initialize();
			HashSet<ADPropertyDefinition> hashSet = new HashSet<ADPropertyDefinition>();
			Queue<PropertyDefinition> queue = new Queue<PropertyDefinition>(properties);
			List<int> list = new List<int>(queue.Count);
			List<ADPropertyDefinition> list2 = new List<ADPropertyDefinition>(queue.Count);
			while (queue.Count > 0)
			{
				ADPropertyDefinition adpropertyDefinition = (ADPropertyDefinition)queue.Dequeue();
				hashSet.Add(adpropertyDefinition);
				if (adpropertyDefinition.Equals(ADObjectSchema.Id))
				{
					throw new ArgumentException(string.Format("Link-DN property {0} cannot be handled", adpropertyDefinition.Name));
				}
				if (!(adpropertyDefinition.Type == typeof(ADObjectId)))
				{
					if (adpropertyDefinition.IsCalculated)
					{
						using (ReadOnlyCollection<ProviderPropertyDefinition>.Enumerator enumerator = adpropertyDefinition.SupportingProperties.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								ProviderPropertyDefinition providerPropertyDefinition = enumerator.Current;
								ADPropertyDefinition item = (ADPropertyDefinition)providerPropertyDefinition;
								if (!hashSet.Contains(item))
								{
									queue.Enqueue(item);
								}
							}
							continue;
						}
					}
					PropTag propTag = NspiPropertyMap.Lookup(adpropertyDefinition);
					if (propTag != PropTag.Null)
					{
						list.Add((int)propTag);
						list2.Add(adpropertyDefinition);
					}
					else
					{
						NspiPropertyMap.Tracer.TraceWarning<string>(0L, "Requested property {0} is not available thru NSPI", adpropertyDefinition.Name);
					}
				}
			}
			list.Add((int)NspiPropertyMap.objectGuidPropertyDefinition.PropTag);
			list2.Add(NspiPropertyMap.objectGuidPropertyDefinition);
			return new NspiPropertyMap(list2.ToArray(), list.ToArray(), encoding);
		}

		public int[] NspiProperties
		{
			get
			{
				return this.nspiProperties;
			}
		}

		public ADRawEntry[] Convert(PropRowSet rowset)
		{
			ADRawEntry[] array = new ADRawEntry[rowset.Rows.Count];
			for (int i = 0; i < rowset.Rows.Count; i++)
			{
				array[i] = this.Convert(rowset.Rows[i].Properties);
			}
			return array;
		}

		public ADRawEntry Convert(IList<PropValue> properties)
		{
			ADPropertyBag adpropertyBag = new ADPropertyBag();
			Guid guid = Guid.Empty;
			for (int i = 0; i < properties.Count; i++)
			{
				ADPropertyDefinition adpropertyDefinition = this.adProperties[i];
				object obj = this.ConvertValue(adpropertyDefinition, properties[i]);
				if (adpropertyDefinition == NspiPropertyMap.objectGuidPropertyDefinition)
				{
					guid = (Guid)obj;
				}
				else
				{
					adpropertyBag.SetField(adpropertyDefinition, obj);
				}
			}
			adpropertyBag.SetField(ADObjectSchema.Id, new ADObjectId(guid));
			return new ADRawEntry(adpropertyBag);
		}

		private static void Initialize()
		{
			if (NspiPropertyMap.isInitialized)
			{
				return;
			}
			foreach (NspiPropertyDefinition nspiPropertyDefinition in NspiSchemaProperties.All)
			{
				if (StringComparer.OrdinalIgnoreCase.Equals(ADObjectSchema.Guid.LdapDisplayName, nspiPropertyDefinition.LdapDisplayName))
				{
					NspiPropertyMap.objectGuidPropertyDefinition = new NspiPropertyDefinition(nspiPropertyDefinition.PropTag, typeof(Guid), nspiPropertyDefinition.LdapDisplayName, ADPropertyDefinitionFlags.None, Guid.Empty, nspiPropertyDefinition.MemberOfGlobalCatalog);
				}
			}
			NspiPropertyMap.isInitialized = true;
		}

		private static PropTag Lookup(ADPropertyDefinition adProperty)
		{
			NspiPropertyDefinition nspiPropertyDefinition = adProperty as NspiPropertyDefinition;
			if (nspiPropertyDefinition != null)
			{
				return nspiPropertyDefinition.PropTag;
			}
			if (adProperty.Equals(ADRecipientSchema.DisplayName))
			{
				return PropTag.DisplayName;
			}
			return NspiSchemaProperties.Lookup(adProperty);
		}

		private object ConvertValue(ADPropertyDefinition propertyDefinition, PropValue propValue)
		{
			if (propValue.IsError() || propValue.IsNull())
			{
				return null;
			}
			PropType propType = propValue.PropType;
			object[] values;
			if (propType <= PropType.SysTime)
			{
				if (propType <= PropType.Boolean)
				{
					if (propType != PropType.Int && propType != PropType.Boolean)
					{
						goto IL_174;
					}
				}
				else
				{
					switch (propType)
					{
					case PropType.AnsiString:
						values = new object[]
						{
							this.encoding.GetString(propValue.GetBytes())
						};
						goto IL_18F;
					case PropType.String:
						break;
					default:
						if (propType != PropType.SysTime)
						{
							goto IL_174;
						}
						break;
					}
				}
			}
			else if (propType <= PropType.Binary)
			{
				if (propType != PropType.Guid)
				{
					if (propType != PropType.Binary)
					{
						goto IL_174;
					}
					values = new object[]
					{
						propValue.RawValue
					};
					goto IL_18F;
				}
			}
			else
			{
				switch (propType)
				{
				case PropType.AnsiStringArray:
					values = Array.ConvertAll<byte[], object>(propValue.GetBytesArray(), (byte[] bytesValue) => this.encoding.GetString(bytesValue));
					goto IL_18F;
				case PropType.StringArray:
					values = Array.ConvertAll<string, object>(propValue.GetStringArray(), (string stringValue) => stringValue);
					goto IL_18F;
				default:
					if (propType != PropType.BinaryArray)
					{
						goto IL_174;
					}
					values = Array.ConvertAll<byte[], object>(propValue.GetBytesArray(), (byte[] bytesValue) => bytesValue);
					goto IL_18F;
				}
			}
			values = new object[]
			{
				propValue.RawValue.ToString()
			};
			goto IL_18F;
			IL_174:
			throw new InvalidOperationException(string.Format("Property type {0} is not supported", propValue.GetType()));
			object result;
			try
			{
				IL_18F:
				result = ADValueConvertor.GetValueFromDirectoryAttributeValues(propertyDefinition, values);
			}
			catch (DataValidationException arg)
			{
				NspiPropertyMap.Tracer.TraceWarning<string, DataValidationException>((long)this.GetHashCode(), "Unable to handle property {0} because it has invalid value, exception: {1}", propertyDefinition.Name, arg);
				result = null;
			}
			return result;
		}

		private static readonly Trace Tracer = ExTraceGlobals.NspiRpcClientConnectionTracer;

		private readonly int[] nspiProperties;

		private readonly ADPropertyDefinition[] adProperties;

		private readonly Encoding encoding;

		private static bool isInitialized;

		private static NspiPropertyDefinition objectGuidPropertyDefinition;
	}
}
