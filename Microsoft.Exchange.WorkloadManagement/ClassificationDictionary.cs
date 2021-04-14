using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal class ClassificationDictionary<TValue> : Dictionary<WorkloadClassification, TValue>
	{
		public ClassificationDictionary() : this(null)
		{
		}

		public ClassificationDictionary(Func<WorkloadClassification, TValue> initialize)
		{
			foreach (WorkloadClassification workloadClassification in this.Classifications)
			{
				base[workloadClassification] = ((initialize != null) ? initialize(workloadClassification) : default(TValue));
			}
		}

		public IEnumerable<WorkloadClassification> Classifications
		{
			get
			{
				return ClassificationDictionary<TValue>.classifications;
			}
		}

		private static WorkloadClassification[] GetClassifications()
		{
			return (from WorkloadClassification classification in Enum.GetValues(typeof(WorkloadClassification))
			where classification != WorkloadClassification.Unknown
			select classification).ToArray<WorkloadClassification>();
		}

		private static WorkloadClassification[] classifications = ClassificationDictionary<TValue>.GetClassifications();
	}
}
