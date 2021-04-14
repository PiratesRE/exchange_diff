using System;
using System.Management.Automation;
using Microsoft.Exchange.Collections;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class EdgeSyncConnector : ADConfigurationObject
	{
		[Parameter(Mandatory = false)]
		public bool Enabled
		{
			get
			{
				return (bool)this[EdgeSyncConnectorSchema.Enabled];
			}
			set
			{
				this[EdgeSyncConnectorSchema.Enabled] = value;
			}
		}

		public string SynchronizationProvider
		{
			get
			{
				return (string)this[EdgeSyncConnectorSchema.SynchronizationProvider];
			}
			internal set
			{
				this[EdgeSyncConnectorSchema.SynchronizationProvider] = value;
			}
		}

		public string AssemblyPath
		{
			get
			{
				return (string)this[EdgeSyncConnectorSchema.AssemblyPath];
			}
			internal set
			{
				this[EdgeSyncConnectorSchema.AssemblyPath] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ObjectSchema.GetInstance<EdgeSyncConnector.AllEdgeSyncConnectorProperties>();
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchEdgeSyncConnector";
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new OrFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, this.MostDerivedObjectClass),
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, "msExchEdgeSyncMservConnector"),
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, "msExchEdgeSyncEhfConnector")
				});
			}
		}

		private const string MostDerivedClass = "msExchEdgeSyncConnector";

		private static readonly EdgeSyncConnectorSchema schema = ObjectSchema.GetInstance<EdgeSyncConnectorSchema>();

		private class AllEdgeSyncConnectorProperties : ADPropertyUnionSchema
		{
			public override ReadOnlyCollection<ADObjectSchema> ObjectSchemas
			{
				get
				{
					return EdgeSyncConnector.AllEdgeSyncConnectorProperties.edgeSyncConnectorSchema;
				}
			}

			private static ReadOnlyCollection<ADObjectSchema> edgeSyncConnectorSchema = new ReadOnlyCollection<ADObjectSchema>(new ADObjectSchema[]
			{
				ObjectSchema.GetInstance<EdgeSyncEhfConnector.EdgeSyncEhfConnectorSchema>(),
				ObjectSchema.GetInstance<EdgeSyncMservConnectorSchema>()
			});
		}
	}
}
