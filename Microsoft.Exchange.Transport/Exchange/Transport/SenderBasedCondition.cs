using System;

namespace Microsoft.Exchange.Transport
{
	internal class SenderBasedCondition : WaitCondition
	{
		public SenderBasedCondition(string sender)
		{
			if (string.IsNullOrEmpty(sender))
			{
				throw new ArgumentNullException("sender");
			}
			this.value = sender;
		}

		public override int CompareTo(object obj)
		{
			SenderBasedCondition senderBasedCondition = obj as SenderBasedCondition;
			if (senderBasedCondition == null)
			{
				throw new ArgumentException();
			}
			return this.value.CompareTo(senderBasedCondition.value);
		}

		public override bool Equals(object obj)
		{
			SenderBasedCondition senderBasedCondition = obj as SenderBasedCondition;
			return senderBasedCondition != null && this.Equals(senderBasedCondition);
		}

		public bool Equals(SenderBasedCondition condition)
		{
			return condition != null && this.value.Equals(condition.value, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			return this.value.GetHashCode();
		}

		public override string ToString()
		{
			return "SenderBasedCondition-" + this.value.ToString();
		}

		private readonly string value;
	}
}
