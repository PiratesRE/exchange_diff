using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public abstract class SendConnector : ADLegacyVersionableObject
	{
		public SendConnector()
		{
		}

		public ADObjectId SourceRoutingGroup
		{
			get
			{
				return (ADObjectId)this[SendConnectorSchema.SourceRoutingGroup];
			}
		}

		public MultiValuedProperty<ADObjectId> SourceTransportServers
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[SendConnectorSchema.SourceTransportServers];
			}
			set
			{
				this[SendConnectorSchema.SourceTransportServers] = value;
			}
		}

		public ADObjectId HomeMTA
		{
			get
			{
				return (ADObjectId)this[SendConnectorSchema.HomeMTA];
			}
			internal set
			{
				this[SendConnectorSchema.HomeMTA] = value;
			}
		}

		public ADObjectId HomeMtaServerId
		{
			get
			{
				return (ADObjectId)this[SendConnectorSchema.HomeMtaServerId];
			}
		}

		[Parameter]
		public Unlimited<ByteQuantifiedSize> MaxMessageSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[SendConnectorSchema.MaxMessageSize];
			}
			set
			{
				this[SendConnectorSchema.MaxMessageSize] = value;
			}
		}

		internal ulong AbsoluteMaxMessageSize
		{
			get
			{
				Unlimited<ByteQuantifiedSize> unlimited = (Unlimited<ByteQuantifiedSize>)this[SendConnectorSchema.MaxMessageSize];
				if (!unlimited.IsUnlimited)
				{
					return unlimited.Value.ToBytes();
				}
				return 2147483647UL;
			}
		}

		internal static MultiValuedProperty<ADObjectId> VsiIdsToServerIds(IPropertyBag propertyBag, ADPropertyDefinition vsisProperty, ADPropertyDefinition serversProperty)
		{
			MultiValuedProperty<ADObjectId> multiValuedProperty = (MultiValuedProperty<ADObjectId>)propertyBag[vsisProperty];
			if (multiValuedProperty.Count == 0)
			{
				return new MultiValuedProperty<ADObjectId>(false, serversProperty, new ADObjectId[0]);
			}
			List<ADObjectId> list = new List<ADObjectId>(multiValuedProperty.Count);
			foreach (ADObjectId adobjectId in multiValuedProperty)
			{
				ADObjectId adobjectId2;
				try
				{
					adobjectId2 = adobjectId.DescendantDN(8);
				}
				catch (InvalidOperationException ex)
				{
					throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty(serversProperty.Name, ex.Message), serversProperty, propertyBag[ADObjectSchema.Id]), ex);
				}
				if (adobjectId2 != null)
				{
					bool flag = false;
					foreach (ADObjectId adobjectId3 in list)
					{
						if (adobjectId3.DistinguishedName.Equals(adobjectId2.DistinguishedName, StringComparison.OrdinalIgnoreCase))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						list.Add(adobjectId2);
					}
				}
			}
			return new MultiValuedProperty<ADObjectId>(false, serversProperty, list);
		}

		internal static void ServerIdsToVsiIds(IPropertyBag propertyBag, object serverIdsObject, ADPropertyDefinition vsisProperty, ADPropertyDefinition serversProperty)
		{
			MultiValuedProperty<ADObjectId> serverIds;
			if (serverIdsObject == null)
			{
				serverIds = new MultiValuedProperty<ADObjectId>(false, serversProperty, new ADObjectId[0]);
			}
			else
			{
				serverIds = (MultiValuedProperty<ADObjectId>)serverIdsObject;
			}
			SendConnector.ServerIdsToVsiIds(propertyBag, serverIds, vsisProperty);
		}

		internal static void ServerIdsToVsiIds(IPropertyBag propertyBag, MultiValuedProperty<ADObjectId> serverIds, ADPropertyDefinition vsisProperty)
		{
			if (serverIds == null || serverIds.Count == 0)
			{
				propertyBag[vsisProperty] = new MultiValuedProperty<ADObjectId>(false, vsisProperty, new ADObjectId[0]);
				return;
			}
			List<ADObjectId> list = new List<ADObjectId>(serverIds.Count);
			foreach (ADObjectId adobjectId in serverIds)
			{
				ADObjectId childId = adobjectId.GetChildId("Protocols").GetChildId("SMTP").GetChildId("1");
				list.Add(childId);
			}
			propertyBag[vsisProperty] = new MultiValuedProperty<ADObjectId>(false, vsisProperty, list);
		}

		internal static object SourceRoutingGroupGetter(IPropertyBag propertyBag)
		{
			object result;
			try
			{
				ADObjectId adobjectId = (ADObjectId)propertyBag[ADObjectSchema.Id];
				if (adobjectId == null)
				{
					throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("SourceRoutingGroup", string.Empty), SendConnectorSchema.SourceRoutingGroup, propertyBag[ADObjectSchema.Id]), null);
				}
				result = adobjectId.DescendantDN(8);
			}
			catch (InvalidOperationException ex)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("SourceRoutingGroup", ex.Message), SendConnectorSchema.SourceRoutingGroup, propertyBag[ADObjectSchema.Id]), ex);
			}
			return result;
		}

		internal static object SourceTransportServersGetter(IPropertyBag propertyBag)
		{
			return SendConnector.VsiIdsToServerIds(propertyBag, SendConnectorSchema.SourceTransportServerVsis, SendConnectorSchema.SourceTransportServers);
		}

		internal static void SourceTransportServersSetter(object value, IPropertyBag propertyBag)
		{
			SendConnector.ServerIdsToVsiIds(propertyBag, value, SendConnectorSchema.SourceTransportServerVsis, SendConnectorSchema.SourceTransportServers);
		}

		internal const ulong UnlimitedMaxMessageSize = 2147483647UL;
	}
}
