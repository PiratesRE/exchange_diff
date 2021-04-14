using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.Cache
{
	[DebuggerDisplay("{RequestId}")]
	[DataContract]
	internal abstract class BaseDirectoryCacheRequest : IExtensibleDataObject
	{
		protected BaseDirectoryCacheRequest()
		{
			this.RequestId = string.Concat(new object[]
			{
				Globals.ProcessId,
				":",
				Globals.ProcessName,
				":",
				Thread.CurrentThread.ManagedThreadId,
				":",
				Guid.NewGuid().ToString()
			});
		}

		protected BaseDirectoryCacheRequest(string forestOrPartitionFqdn) : this()
		{
			ArgumentValidator.ThrowIfNullOrEmpty("forestOrPartitionFqdn", forestOrPartitionFqdn);
			this.ForestOrPartitionFqdn = forestOrPartitionFqdn;
			this.OrganizationId = null;
		}

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public string RequestId { get; private set; }

		[DataMember(IsRequired = true, EmitDefaultValue = false)]
		public string ForestOrPartitionFqdn { get; protected set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public string OrganizationId { get; private set; }

		protected void InternalSetOrganizationId(OrganizationId organizationId)
		{
			ArgumentValidator.ThrowIfNull("organizationId", organizationId);
			if (Microsoft.Exchange.Data.Directory.OrganizationId.ForestWideOrgId.Equals(organizationId))
			{
				return;
			}
			this.OrganizationId = organizationId.ConfigurationUnit.DistinguishedName;
		}

		public ExtensionDataObject ExtensionData { get; set; }
	}
}
