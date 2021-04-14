using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[DataContract]
	public sealed class IncrementalCollection<T> : IncrementalAttributeBase
	{
		public IncrementalCollection()
		{
			this.Changed = false;
		}

		public IncrementalCollection(IEnumerable<T> changedValues, IEnumerable<T> removedValues)
		{
			this.Changed = true;
			this.ChangedValues = changedValues;
			this.RemovedValues = removedValues;
		}

		[DataMember]
		public IEnumerable<T> ChangedValues { get; set; }

		[DataMember]
		public IEnumerable<T> RemovedValues { get; set; }

		[DataMember]
		public override bool Changed { get; protected set; }

		public override object GetObjectValue()
		{
			if (!this.Changed)
			{
				throw new InvalidOperationException("Incremental value not present");
			}
			return this.ChangedValues;
		}
	}
}
