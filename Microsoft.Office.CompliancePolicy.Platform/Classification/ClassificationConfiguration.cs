using System;
using System.Collections.Generic;

namespace Microsoft.Office.CompliancePolicy.Classification
{
	public sealed class ClassificationConfiguration
	{
		public ClassificationConfiguration()
		{
			this.properties = new Dictionary<string, object>();
			this.propertyBag = new ClassificationConfiguration.ReadOnlyClassificationPropertyBag(this.properties);
		}

		public int? MaxRulePackageCacheCount
		{
			get
			{
				return (int?)this.RetrievePropertyHelper("MaxRulePackageCacheCount");
			}
			set
			{
				this.SetPropertyHelper("MaxRulePackageCacheCount", value);
			}
		}

		public int? MaxRulePackageCacheStorage
		{
			get
			{
				return (int?)this.RetrievePropertyHelper("MaxRulePackageCacheStorage");
			}
			set
			{
				this.SetPropertyHelper("MaxRulePackageCacheStorage", value);
			}
		}

		public int? PerformanceMonitorPollPeriod
		{
			get
			{
				return (int?)this.RetrievePropertyHelper("PerformanceMonitorPollPeriod");
			}
			set
			{
				this.SetPropertyHelper("PerformanceMonitorPollPeriod", value);
			}
		}

		public int? RulePackageCachePollPeriod
		{
			get
			{
				return (int?)this.RetrievePropertyHelper("RulePackageCachePollPeriod");
			}
			set
			{
				this.SetPropertyHelper("RulePackageCachePollPeriod", value);
			}
		}

		public int? RulePackageCacheRetrievalWaitTime
		{
			get
			{
				return (int?)this.RetrievePropertyHelper("RulePackageCacheRetrievalWaitTime");
			}
			set
			{
				this.SetPropertyHelper("RulePackageCacheRetrievalWaitTime", value);
			}
		}

		public string TraceSessionId
		{
			get
			{
				return (string)this.RetrievePropertyHelper("TraceSessionId");
			}
			set
			{
				this.SetPropertyHelper("TraceSessionId", value);
			}
		}

		public int? TraceSessionLevel
		{
			get
			{
				return (int?)this.RetrievePropertyHelper("TraceSessionLevel");
			}
			set
			{
				this.SetPropertyHelper("TraceSessionLevel", value);
			}
		}

		public bool? UseLazyRegexCompilation
		{
			get
			{
				return (bool?)this.RetrievePropertyHelper("UseLazyRegexCompilation");
			}
			set
			{
				this.SetPropertyHelper("UseLazyRegexCompilation", value);
			}
		}

		public bool? UseMemoryToImprovePerformance
		{
			get
			{
				return (bool?)this.RetrievePropertyHelper("UseMemoryToImprovePerformance");
			}
			set
			{
				this.SetPropertyHelper("UseMemoryToImprovePerformance", value);
			}
		}

		internal IPropertyBag PropertyBag
		{
			get
			{
				return this.propertyBag;
			}
		}

		private object RetrievePropertyHelper(string propertyName)
		{
			if (this.properties.ContainsKey(propertyName))
			{
				return this.properties[propertyName];
			}
			return null;
		}

		private void SetPropertyHelper(string propertyName, object value)
		{
			if (value == null && this.properties.ContainsKey(propertyName))
			{
				this.properties.Remove(propertyName);
				return;
			}
			this.properties[propertyName] = value;
		}

		private Dictionary<string, object> properties;

		private IPropertyBag propertyBag;

		private class ReadOnlyClassificationPropertyBag : IPropertyBag
		{
			internal ReadOnlyClassificationPropertyBag(Dictionary<string, object> properties)
			{
				if (properties == null)
				{
					throw new ArgumentNullException("properties");
				}
				this.properties = properties;
			}

			public int Read(string propertyName, ref object propertyValue, IErrorLog errorLog)
			{
				if (this.properties.ContainsKey(propertyName))
				{
					propertyValue = this.properties[propertyName];
					return 0;
				}
				return -2147467259;
			}

			public int Write(string propertyName, ref object propertyValue)
			{
				throw new NotSupportedException();
			}

			private const int ComOK = 0;

			private const int ComFAIL = -2147467259;

			private Dictionary<string, object> properties;
		}
	}
}
