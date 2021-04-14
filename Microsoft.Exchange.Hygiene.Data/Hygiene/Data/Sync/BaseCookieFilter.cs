using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Sync
{
	internal abstract class BaseCookieFilter : ConfigurablePropertyBag
	{
		public string Version
		{
			get
			{
				return this[BaseCookieSchema.VersionProp] as string;
			}
			set
			{
				this[BaseCookieSchema.VersionProp] = value;
			}
		}

		public string ServiceInstance
		{
			get
			{
				return this[BaseCookieSchema.ServiceInstanceProp] as string;
			}
			set
			{
				this[BaseCookieSchema.ServiceInstanceProp] = value;
			}
		}

		public DateTime? LastUpdatedCutoffThreshold
		{
			get
			{
				return (DateTime?)this[BaseCookieSchema.LastUpdatedCutoffThresholdQueryProp];
			}
			set
			{
				this[BaseCookieSchema.LastUpdatedCutoffThresholdQueryProp] = value;
			}
		}

		public DateTime? LastCompletedCutoffThreshold
		{
			get
			{
				return (DateTime?)this[BaseCookieSchema.LastCompletedCutoffThresholdQueryProp];
			}
			set
			{
				this[BaseCookieSchema.LastCompletedCutoffThresholdQueryProp] = value;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public List<QueryFilter> GetQueryFilters()
		{
			List<QueryFilter> list = new List<QueryFilter>();
			foreach (PropertyDefinition propertyDefinition in this.GetPropertyDefinitions(true))
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, propertyDefinition, this[propertyDefinition]));
			}
			return list;
		}
	}
}
