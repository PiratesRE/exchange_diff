using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.Cache
{
	[KnownType(typeof(BaseDirectoryCacheRequest))]
	[DataContract]
	internal class RemoveDirectoryCacheRequest : BaseDirectoryCacheRequest, IExtensibleDataObject
	{
		public RemoveDirectoryCacheRequest(string forestFqdn, OrganizationId organizationId, Tuple<string, KeyType> key, ObjectType objectType) : base((organizationId != null) ? organizationId.PartitionId.ForestFQDN : null)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("forestFqdn", forestFqdn);
			ArgumentValidator.ThrowIfNull("organizationId", organizationId);
			ArgumentValidator.ThrowIfNull("key", key);
			if (objectType == ObjectType.Unknown)
			{
				throw new InvalidOperationException("Invalid object type");
			}
			this.Key = key;
			this.ObjectType = objectType;
			base.ForestOrPartitionFqdn = forestFqdn;
			base.InternalSetOrganizationId(organizationId);
		}

		[DataMember(IsRequired = true)]
		public ObjectType ObjectType { get; private set; }

		[DataMember(IsRequired = true)]
		public Tuple<string, KeyType> Key { get; private set; }
	}
}
