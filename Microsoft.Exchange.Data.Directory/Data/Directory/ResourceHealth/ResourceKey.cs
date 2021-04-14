using System;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Data.Directory.ResourceHealth
{
	internal abstract class ResourceKey
	{
		public ResourceKey(ResourceMetricType metric, string id)
		{
			this.MetricType = metric;
			this.Id = id;
		}

		public ResourceMetricType MetricType { get; private set; }

		public string Id { get; private set; }

		public override bool Equals(object obj)
		{
			ResourceKey resourceKey = obj as ResourceKey;
			return resourceKey != null && this.MetricType == resourceKey.MetricType && string.Equals(this.Id, resourceKey.Id, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			return (int)(this.MetricType ^ (ResourceMetricType)((this.Id != null) ? this.Id.GetHashCode() : 0));
		}

		public override string ToString()
		{
			if (this.Id != null)
			{
				return string.Format("{0}({1})", this.MetricType, this.Id);
			}
			return this.MetricType.ToString();
		}

		protected internal abstract CacheableResourceHealthMonitor CreateMonitor();
	}
}
