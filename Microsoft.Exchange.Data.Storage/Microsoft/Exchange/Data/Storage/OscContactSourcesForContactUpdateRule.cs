using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class OscContactSourcesForContactUpdateRule
	{
		public OscContactSourcesForContactUpdateRule() : this(OscContactSourcesForContactParser.Instance)
		{
		}

		public OscContactSourcesForContactUpdateRule(IOscContactSourcesForContactParser parser)
		{
			this.oscParser = parser;
		}

		public bool UpdatePartnerNetworkProperties(ICorePropertyBag propertyBag)
		{
			if (!propertyBag.IsPropertyDirty(ContactSchema.OscContactSourcesForContact))
			{
				return false;
			}
			byte[] valueOrDefault = propertyBag.GetValueOrDefault<byte[]>(ContactSchema.OscContactSourcesForContact, null);
			if (valueOrDefault == null || valueOrDefault.Length == 0)
			{
				return OscContactSourcesForContactUpdateRule.UpdateDependentProperties(propertyBag, null, null);
			}
			try
			{
				return OscContactSourcesForContactUpdateRule.UpdateDependentProperties(propertyBag, valueOrDefault, this.oscParser.ReadOscContactSource(valueOrDefault));
			}
			catch (OscContactSourcesForContactParseException arg)
			{
				OscContactSourcesForContactUpdateRule.Tracer.TraceError<OscContactSourcesForContactParseException>((long)this.GetHashCode(), "Encountered exception when parsing: {0}", arg);
			}
			return false;
		}

		private static bool UpdateDependentProperties(ICorePropertyBag propertyBag, byte[] oscContactSources, OscNetworkProperties networkProperties)
		{
			bool flag = false;
			flag |= OscContactSourcesForContactUpdateRule.UpdatePartnerNetworkIdAndUserId(propertyBag, oscContactSources, networkProperties);
			return flag | OscContactSourcesForContactUpdateRule.CopyCloudIdFromOscContactSourcesIfCurrentCloudIdIsBlank(propertyBag, networkProperties);
		}

		private static bool UpdatePartnerNetworkIdAndUserId(ICorePropertyBag propertyBag, byte[] oscContactSources, OscNetworkProperties networkProperties)
		{
			if (propertyBag.IsPropertyDirty(ContactSchema.PartnerNetworkId) || propertyBag.IsPropertyDirty(ContactSchema.PartnerNetworkUserId))
			{
				return false;
			}
			if (oscContactSources == null)
			{
				propertyBag.Delete(ContactSchema.PartnerNetworkId);
				propertyBag.Delete(ContactSchema.PartnerNetworkUserId);
				return true;
			}
			if (networkProperties != null)
			{
				propertyBag[ContactSchema.PartnerNetworkId] = networkProperties.NetworkId;
				propertyBag[ContactSchema.PartnerNetworkUserId] = networkProperties.NetworkUserId;
				return true;
			}
			return false;
		}

		private static bool CopyCloudIdFromOscContactSourcesIfCurrentCloudIdIsBlank(ICorePropertyBag propertyBag, OscNetworkProperties networkProperties)
		{
			if (propertyBag.IsPropertyDirty(ItemSchema.CloudId))
			{
				return false;
			}
			if (!string.IsNullOrEmpty(propertyBag.GetValueOrDefault<string>(ItemSchema.CloudId)))
			{
				return false;
			}
			if (networkProperties != null && !string.IsNullOrEmpty(networkProperties.NetworkUserId))
			{
				propertyBag[ItemSchema.CloudId] = networkProperties.NetworkUserId;
				return true;
			}
			return false;
		}

		private static readonly Trace Tracer = ExTraceGlobals.OutlookSocialConnectorInteropTracer;

		private readonly IOscContactSourcesForContactParser oscParser;

		public static readonly PropertyReference[] UpdateProperties = new PropertyReference[]
		{
			new PropertyReference(InternalSchema.OscContactSourcesForContact, PropertyAccess.Read),
			new PropertyReference(InternalSchema.PartnerNetworkId, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.PartnerNetworkUserId, PropertyAccess.ReadWrite),
			new PropertyReference(InternalSchema.CloudId, PropertyAccess.ReadWrite)
		};
	}
}
