using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class SimpleValueDataContext : DataContext
	{
		public SimpleValueDataContext(string name, object value)
		{
			this.name = name;
			this.value = value;
		}

		public override string ToString()
		{
			return string.Format("{0}: {1}", this.name, (this.value == null) ? "(null)" : this.value.ToString());
		}

		private string name;

		private object value;
	}
}
