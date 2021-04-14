using System;
using System.Collections.Generic;
using System.Data.Services.Providers;
using System.Linq;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	internal class ReportingQueryProvider : IDataServiceQueryProvider
	{
		public ReportingQueryProvider(IDataServiceMetadataProvider metadataProvider, ReportingSchema schema)
		{
			this.metadataProvider = metadataProvider;
			this.schema = schema;
			this.dataSource = DependencyFactory.CreateReportingDataSource(RbacPrincipal.Current);
		}

		object IDataServiceQueryProvider.CurrentDataSource
		{
			get
			{
				return this.dataSource;
			}
			set
			{
				this.dataSource = (IReportingDataSource)value;
			}
		}

		bool IDataServiceQueryProvider.IsNullPropagationRequired
		{
			get
			{
				return false;
			}
		}

		object IDataServiceQueryProvider.GetOpenPropertyValue(object target, string propertyName)
		{
			throw new NotImplementedException();
		}

		IEnumerable<KeyValuePair<string, object>> IDataServiceQueryProvider.GetOpenPropertyValues(object target)
		{
			throw new NotImplementedException();
		}

		object IDataServiceQueryProvider.GetPropertyValue(object target, ResourceProperty resourceProperty)
		{
			throw new NotImplementedException();
		}

		IQueryable IDataServiceQueryProvider.GetQueryRootForResourceSet(ResourceSet resourceSet)
		{
			IEntity entity = this.schema.Entities[resourceSet.Name];
			return (IQueryable)Activator.CreateInstance(typeof(ReportingDataQuery<>).MakeGenericType(new Type[]
			{
				entity.ClrType
			}), new object[]
			{
				this.dataSource,
				entity
			});
		}

		ResourceType IDataServiceQueryProvider.GetResourceType(object target)
		{
			Type type = target.GetType();
			ResourceType resourceType = ResourceType.GetPrimitiveResourceType(type);
			if (resourceType == null)
			{
				resourceType = this.metadataProvider.Types.Single((ResourceType t) => t.InstanceType == type);
			}
			return resourceType;
		}

		object IDataServiceQueryProvider.InvokeServiceOperation(ServiceOperation serviceOperation, object[] parameters)
		{
			throw new NotImplementedException();
		}

		private readonly IDataServiceMetadataProvider metadataProvider;

		private readonly ReportingSchema schema;

		private IReportingDataSource dataSource;
	}
}
