using System;
using System.Collections.Generic;
using System.Data.Services.Providers;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	internal class ReportingMetadataProvider : IDataServiceMetadataProvider
	{
		public ReportingMetadataProvider(ReportingSchema schema)
		{
			this.metadata = new Metadata(RbacPrincipal.Current, schema.Entities.Values, schema.ComplexTypeResourceTypes);
		}

		string IDataServiceMetadataProvider.ContainerName
		{
			get
			{
				return "TenantReportingWebService";
			}
		}

		string IDataServiceMetadataProvider.ContainerNamespace
		{
			get
			{
				return "TenantReporting";
			}
		}

		IEnumerable<ResourceSet> IDataServiceMetadataProvider.ResourceSets
		{
			get
			{
				return this.metadata.ResourceSets.Values;
			}
		}

		IEnumerable<ServiceOperation> IDataServiceMetadataProvider.ServiceOperations
		{
			get
			{
				yield break;
			}
		}

		IEnumerable<ResourceType> IDataServiceMetadataProvider.Types
		{
			get
			{
				return this.metadata.ResourceTypes.Values;
			}
		}

		IEnumerable<ResourceType> IDataServiceMetadataProvider.GetDerivedTypes(ResourceType resourceType)
		{
			yield break;
		}

		ResourceAssociationSet IDataServiceMetadataProvider.GetResourceAssociationSet(ResourceSet resourceSet, ResourceType resourceType, ResourceProperty resourceProperty)
		{
			throw new NotImplementedException("No relationships.");
		}

		bool IDataServiceMetadataProvider.HasDerivedTypes(ResourceType resourceType)
		{
			return false;
		}

		bool IDataServiceMetadataProvider.TryResolveResourceSet(string name, out ResourceSet resourceSet)
		{
			return this.metadata.ResourceSets.TryGetValue(name, out resourceSet);
		}

		bool IDataServiceMetadataProvider.TryResolveResourceType(string name, out ResourceType resourceType)
		{
			return this.metadata.ResourceTypes.TryGetValue(name, out resourceType);
		}

		bool IDataServiceMetadataProvider.TryResolveServiceOperation(string name, out ServiceOperation serviceOperation)
		{
			serviceOperation = null;
			return false;
		}

		public const string Container = "TenantReportingWebService";

		private readonly Metadata metadata;
	}
}
