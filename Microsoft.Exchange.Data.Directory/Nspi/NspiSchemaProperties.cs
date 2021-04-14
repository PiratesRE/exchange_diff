using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Nspi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class NspiSchemaProperties
	{
		public static NspiPropertyDefinition[] All
		{
			get
			{
				NspiSchemaProperties.Initialize();
				return NspiSchemaProperties.all;
			}
		}

		public static PropTag Lookup(ADPropertyDefinition adProperty)
		{
			NspiSchemaProperties.Initialize();
			PropTag result;
			if (NspiSchemaProperties.ldapNameToPropTagMap.TryGetValue(adProperty.LdapDisplayName, out result))
			{
				return result;
			}
			return PropTag.Null;
		}

		private static void Initialize()
		{
			if (NspiSchemaProperties.isInitialized)
			{
				return;
			}
			IConfigurationSession configurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.SessionSettingsFactory.Default.FromRootOrgScopeSet(), 98, "Initialize", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\nspi\\NspiSchemaProperties.cs");
			ADPagedReader<ADSchemaAttributeObject> adpagedReader = configurationSession.FindPaged<ADSchemaAttributeObject>(configurationSession.SchemaNamingContext, QueryScope.SubTree, NspiSchemaProperties.AllMapiAttributesFilter, null, 0);
			Dictionary<int, ADSchemaAttributeObject> dictionary = new Dictionary<int, ADSchemaAttributeObject>(500);
			foreach (ADSchemaAttributeObject adschemaAttributeObject in adpagedReader)
			{
				NspiSchemaProperties.Tracer.TraceDebug(0L, "Property 0x{0:X4}: {1} {2} {3} {4} {5}", new object[]
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
					NspiSchemaProperties.Tracer.TraceDebug(0L, "-- Duplicate property, skipped");
					dictionary[mapiID] = null;
				}
				else
				{
					dictionary[mapiID] = adschemaAttributeObject;
				}
			}
			List<NspiPropertyDefinition> list = new List<NspiPropertyDefinition>(dictionary.Count);
			Dictionary<string, PropTag> dictionary2 = new Dictionary<string, PropTag>(dictionary.Count, StringComparer.OrdinalIgnoreCase);
			foreach (ADSchemaAttributeObject adschemaAttributeObject2 in dictionary.Values)
			{
				if (adschemaAttributeObject2 != null)
				{
					NspiPropertyDefinition nspiPropertyDefinition = NspiSchemaProperties.CreatePropertyDefinition(adschemaAttributeObject2);
					if (nspiPropertyDefinition != null)
					{
						list.Add(nspiPropertyDefinition);
						dictionary2.Add(nspiPropertyDefinition.LdapDisplayName, nspiPropertyDefinition.PropTag);
					}
				}
			}
			NspiSchemaProperties.all = list.ToArray();
			NspiSchemaProperties.ldapNameToPropTagMap = dictionary2;
			NspiSchemaProperties.isInitialized = true;
		}

		private static NspiPropertyDefinition CreatePropertyDefinition(ADSchemaAttributeObject mapiAttribute)
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
			NspiSchemaProperties.Tracer.TraceDebug<DataSyntax>(0L, "Unsupported DataSyntax {0} -- Skipped", mapiAttribute.DataSyntax);
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
				NspiSchemaProperties.Tracer.TraceDebug<PropType>(0L, "Unsupported multivalue property type {0} -- Skipped", propType);
				return null;
				IL_19C:
				throw new InvalidOperationException("Invalid PropType");
				IL_1A7:
				defaultValue = null;
				adpropertyDefinitionFlags |= ADPropertyDefinitionFlags.MultiValued;
				adpropertyDefinitionFlags &= ~ADPropertyDefinitionFlags.PersistDefaultValue;
			}
			return new NspiPropertyDefinition(PropTagHelper.PropTagFromIdAndType(mapiAttribute.MapiID, propType), typeFromHandle, mapiAttribute.LdapDisplayName, adpropertyDefinitionFlags, defaultValue, mapiAttribute.IsMemberOfPartialAttributeSet);
		}

		private static readonly Trace Tracer = ExTraceGlobals.NspiRpcClientConnectionTracer;

		private static readonly QueryFilter AllMapiAttributesFilter = new AndFilter(new QueryFilter[]
		{
			new ComparisonFilter(ComparisonOperator.GreaterThan, ADSchemaAttributeSchema.MapiID, 0),
			new ComparisonFilter(ComparisonOperator.LessThanOrEqual, ADSchemaAttributeSchema.MapiID, 65535)
		});

		private static bool isInitialized;

		private static NspiPropertyDefinition[] all;

		private static Dictionary<string, PropTag> ldapNameToPropTagMap;
	}
}
