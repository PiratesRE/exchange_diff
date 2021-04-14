using System;

namespace Microsoft.Exchange.Transport
{
	internal class ResubmitterBasedCondition : WaitCondition
	{
		public ResubmitterBasedCondition(string resubmitter)
		{
			if (string.IsNullOrEmpty(resubmitter))
			{
				throw new ArgumentNullException("resubmitter");
			}
			this.value = resubmitter;
		}

		public override int CompareTo(object obj)
		{
			ResubmitterBasedCondition resubmitterBasedCondition = obj as ResubmitterBasedCondition;
			if (resubmitterBasedCondition == null)
			{
				throw new ArgumentException();
			}
			return this.value.CompareTo(resubmitterBasedCondition.value);
		}

		public override bool Equals(object obj)
		{
			ResubmitterBasedCondition resubmitterBasedCondition = obj as ResubmitterBasedCondition;
			return resubmitterBasedCondition != null && this.Equals(resubmitterBasedCondition);
		}

		public bool Equals(ResubmitterBasedCondition condition)
		{
			return condition != null && this.value.Equals(condition.value, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			return this.value.GetHashCode();
		}

		public override string ToString()
		{
			return "ResubmitterBasedCondition-" + this.value.ToString();
		}

		private readonly string value;
	}
}
