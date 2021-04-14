using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Caching;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.DirectoryCache;

namespace Microsoft.Exchange.Data.Directory.Cache
{
	[KnownType(typeof(BaseDirectoryCacheRequest))]
	[KnownType(typeof(List<Tuple<string, KeyType>>))]
	[DataContract]
	[DebuggerDisplay("{RequestId}-{ObjectType}")]
	internal class AddDirectoryCacheRequest : BaseDirectoryCacheRequest, IExtensibleDataObject
	{
		public AddDirectoryCacheRequest(List<Tuple<string, KeyType>> keys, ADRawEntry objectToCache, string forestFqdn, OrganizationId organizationId, IEnumerable<PropertyDefinition> properties, int secondsTimeout = 2147483646, CacheItemPriority priority = CacheItemPriority.Default)
		{
			ArgumentValidator.ThrowIfNull("keys", keys);
			ArgumentValidator.ThrowIfNull("objectToCache", objectToCache);
			ArgumentValidator.ThrowIfOutOfRange<int>("secondsTimeout", secondsTimeout, 1, 2147483646);
			ArgumentValidator.ThrowIfNull("organizationId", organizationId);
			if (keys.Count == 0)
			{
				throw new InvalidOperationException("Keys should not be empty");
			}
			this.Keys = keys;
			ADObject adobject = objectToCache as ADObject;
			if (adobject != null)
			{
				this.Object = SimpleADObject.CreateFrom(adobject, properties);
			}
			else
			{
				this.Object = SimpleADObject.CreateFromRawEntry(objectToCache, properties, true);
			}
			this.ObjectType = CacheUtils.GetObjectTypeFor(objectToCache.GetType(), true);
			this.SecondsTimeout = secondsTimeout;
			this.Priority = priority;
			base.ForestOrPartitionFqdn = forestFqdn;
			base.InternalSetOrganizationId(organizationId);
		}

		[DataMember(IsRequired = true, EmitDefaultValue = false)]
		public List<Tuple<string, KeyType>> Keys { get; private set; }

		[DataMember(IsRequired = true, EmitDefaultValue = false)]
		public int SecondsTimeout { get; private set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public CacheItemPriority Priority { get; private set; }

		[DataMember(IsRequired = true, EmitDefaultValue = false)]
		public SimpleADObject Object { get; private set; }

		[DataMember(IsRequired = true, EmitDefaultValue = false)]
		public ObjectType ObjectType { get; private set; }

		public override string ToString()
		{
			if (ExTraceGlobals.WCFServiceEndpointTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				return string.Format("{0}-{1}-{2}-{3}-[{4}]", new object[]
				{
					base.RequestId,
					this.ObjectType,
					this.SecondsTimeout,
					this.Priority,
					string.Join<Tuple<string, KeyType>>("|", this.Keys)
				});
			}
			return base.RequestId + this.ObjectType.ToString();
		}
	}
}
