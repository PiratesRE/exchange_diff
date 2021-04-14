using System;

namespace Microsoft.Exchange.Diagnostics.WorkloadManagement
{
	internal struct OperationKey : IEquatable<OperationKey>
	{
		public OperationKey(ActivityOperationType activityOperationType, string instance)
		{
			this.ActivityOperationType = activityOperationType;
			this.Instance = instance;
		}

		public override int GetHashCode()
		{
			return ((this.Instance != null) ? this.Instance.GetHashCode() : 0) ^ (int)this.ActivityOperationType;
		}

		public override bool Equals(object obj)
		{
			return obj is OperationKey && this.Equals((OperationKey)obj);
		}

		public bool Equals(OperationKey other)
		{
			return this.ActivityOperationType == other.ActivityOperationType && 0 == string.Compare(this.Instance, other.Instance, StringComparison.OrdinalIgnoreCase);
		}

		public readonly ActivityOperationType ActivityOperationType;

		public readonly string Instance;
	}
}
