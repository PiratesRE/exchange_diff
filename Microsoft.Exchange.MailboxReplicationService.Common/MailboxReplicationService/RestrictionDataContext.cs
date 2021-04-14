using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class RestrictionDataContext : DataContext
	{
		public RestrictionDataContext(RestrictionData restriction)
		{
			this.restriction = restriction;
		}

		public override string ToString()
		{
			return string.Format("Restriction: {0}", (this.restriction != null) ? this.restriction.ToString() : "(null)");
		}

		private RestrictionData restriction;
	}
}
