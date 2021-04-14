using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[DataContract]
	public sealed class IncrementalAttribute<T> : IncrementalAttributeBase
	{
		public IncrementalAttribute()
		{
			this.Changed = false;
		}

		public IncrementalAttribute(T value)
		{
			this.Changed = true;
			this.Value = value;
		}

		[DataMember]
		public T Value { get; set; }

		[DataMember]
		public override bool Changed { get; protected set; }

		public override object GetObjectValue()
		{
			if (!this.Changed)
			{
				throw new InvalidOperationException("Incremental value not present");
			}
			return this.Value;
		}
	}
}
