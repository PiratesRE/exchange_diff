using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.DirectoryCache;

namespace Microsoft.Exchange.Data.Directory.Cache
{
	[KnownType(typeof(BaseDirectoryCacheRequest))]
	[DebuggerDisplay("{RequestId}-{ObjectType}")]
	[DataContract]
	internal class DirectoryCacheRequest : BaseDirectoryCacheRequest, IExtensibleDataObject
	{
		public DirectoryCacheRequest(string forestOrPartitionFqdn, List<Tuple<string, KeyType>> keys, ObjectType objectType, IEnumerable<PropertyDefinition> properties = null) : base(forestOrPartitionFqdn)
		{
			ArgumentValidator.ThrowIfNull("keys", keys);
			if (keys.Count == 0)
			{
				throw new InvalidOperationException("Keys should not be empty");
			}
			this.Keys = keys;
			this.ObjectType = objectType;
			if (this.ObjectType == ObjectType.ADRawEntry && properties == null)
			{
				ArgumentValidator.ThrowIfNull("properties", properties);
			}
			if (properties != null)
			{
				this.Properties = this.Convert(properties).ToArray<string>();
				this.ADPropertiesRequested = properties;
			}
		}

		public DirectoryCacheRequest(string forestOrPartitionFqdn, Tuple<string, KeyType> key, ObjectType objectType, IEnumerable<PropertyDefinition> properties = null) : this(forestOrPartitionFqdn, new List<Tuple<string, KeyType>>(1)
		{
			key
		}, objectType, properties)
		{
			ArgumentValidator.ThrowIfNull("key", key);
		}

		public DirectoryCacheRequest(string forestOrPartitionFqdn, string key, KeyType keyType, ObjectType objectType, IEnumerable<PropertyDefinition> properties = null) : this(forestOrPartitionFqdn, new Tuple<string, KeyType>(key, keyType), objectType, properties)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("key", key);
		}

		[DataMember(IsRequired = true)]
		public ObjectType ObjectType { get; set; }

		[DataMember(IsRequired = true)]
		public List<Tuple<string, KeyType>> Keys { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public string[] Properties { get; private set; }

		public IEnumerable<PropertyDefinition> ADPropertiesRequested { get; private set; }

		[Conditional("DEBUG")]
		internal void Dbg_Validate()
		{
			ObjectType objectType = this.ObjectType;
			if (objectType <= ObjectType.OWAMiniRecipient)
			{
				if (objectType <= ObjectType.FederatedOrganizationId)
				{
					switch (objectType)
					{
					case ObjectType.ExchangeConfigurationUnit:
					case ObjectType.Recipient:
					case ObjectType.AcceptedDomain:
						goto IL_7C;
					case ObjectType.ExchangeConfigurationUnit | ObjectType.Recipient:
						break;
					default:
						if (objectType == ObjectType.FederatedOrganizationId)
						{
							goto IL_7C;
						}
						break;
					}
				}
				else if (objectType == ObjectType.MiniRecipient || objectType == ObjectType.TransportMiniRecipient || objectType == ObjectType.OWAMiniRecipient)
				{
					goto IL_7C;
				}
			}
			else if (objectType <= ObjectType.ADRawEntry)
			{
				if (objectType == ObjectType.ActiveSyncMiniRecipient || objectType == ObjectType.ADRawEntry)
				{
					goto IL_7C;
				}
			}
			else if (objectType == ObjectType.StorageMiniRecipient || objectType == ObjectType.MiniRecipientWithTokenGroups || objectType == ObjectType.FrontEndMiniRecipient)
			{
				goto IL_7C;
			}
			throw new NotSupportedException("ObjectType should be single value");
			IL_7C:
			if (string.IsNullOrEmpty(base.ForestOrPartitionFqdn))
			{
				throw new InvalidOperationException("ForestOrPartitionFqdn should not be null");
			}
			if (this.Keys == null || this.Keys.Count == 0)
			{
				throw new InvalidOperationException("Keys should not be null or empty");
			}
		}

		internal void SetOrganizationId(OrganizationId organizationId)
		{
			base.InternalSetOrganizationId(organizationId);
		}

		private string[] Convert(IEnumerable<PropertyDefinition> properties)
		{
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (PropertyDefinition propertyDefinition in properties)
			{
				ADPropertyDefinition adpropertyDefinition = (ADPropertyDefinition)propertyDefinition;
				if (adpropertyDefinition.LdapDisplayName != null && !adpropertyDefinition.IsCalculated && !hashSet.Contains(adpropertyDefinition.LdapDisplayName))
				{
					hashSet.Add(adpropertyDefinition.LdapDisplayName);
				}
			}
			return hashSet.ToArray<string>();
		}

		public override string ToString()
		{
			if (ExTraceGlobals.CacheSessionTracer.IsTraceEnabled(TraceType.DebugTrace) || ExTraceGlobals.WCFServiceEndpointTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				return string.Format("{0}-{1}-{2}-[{3}]", new object[]
				{
					base.RequestId,
					base.ForestOrPartitionFqdn,
					this.ObjectType,
					string.Join<Tuple<string, KeyType>>("|", this.Keys)
				});
			}
			return base.ForestOrPartitionFqdn + base.RequestId + this.ObjectType.ToString();
		}
	}
}
