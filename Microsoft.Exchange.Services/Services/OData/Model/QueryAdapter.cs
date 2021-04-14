using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.OData.Web;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal abstract class QueryAdapter
	{
		protected QueryAdapter()
		{
		}

		public QueryAdapter(EntitySchema entitySchema, ODataQueryOptions odataQueryOptions)
		{
			ArgumentValidator.ThrowIfNull("entitySchema", entitySchema);
			ArgumentValidator.ThrowIfNull("odataQueryOptions", odataQueryOptions);
			this.EntitySchema = entitySchema;
			this.ODataQueryOptions = odataQueryOptions;
		}

		public EntitySchema EntitySchema { get; private set; }

		public ODataQueryOptions ODataQueryOptions { get; private set; }

		public IList<PropertyDefinition> RequestedProperties
		{
			get
			{
				if (this.requestedProperties == null)
				{
					this.PopulateRequestedProperties();
				}
				return this.requestedProperties;
			}
		}

		public int GetPageSize()
		{
			return QueryAdapter.GetPageSize(this.ODataQueryOptions.Top);
		}

		public static int GetPageSize(int? topParameter)
		{
			int result = 50;
			if (topParameter != null)
			{
				result = Math.Min(topParameter.Value, 500);
			}
			return result;
		}

		private void PopulateRequestedProperties()
		{
			this.requestedProperties = this.EntitySchema.DefaultProperties;
			if (this.ODataQueryOptions.Select != null)
			{
				this.requestedProperties = new List<PropertyDefinition>
				{
					EntitySchema.Id
				};
				foreach (string text in this.ODataQueryOptions.Select)
				{
					if (string.Equals(text, "*", StringComparison.OrdinalIgnoreCase))
					{
						this.requestedProperties = new List<PropertyDefinition>();
						using (IEnumerator<PropertyDefinition> enumerator = this.EntitySchema.AllProperties.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								PropertyDefinition propertyDefinition = enumerator.Current;
								if (propertyDefinition.Flags != PropertyDefinitionFlags.Navigation)
								{
									this.requestedProperties.Add(propertyDefinition);
								}
							}
							break;
						}
					}
					PropertyDefinition propertyDefinition2 = this.EntitySchema.ResolveProperty(text);
					if (!propertyDefinition2.IsNavigation)
					{
						this.requestedProperties.Add(propertyDefinition2);
					}
				}
			}
		}

		private IList<PropertyDefinition> requestedProperties;
	}
}
