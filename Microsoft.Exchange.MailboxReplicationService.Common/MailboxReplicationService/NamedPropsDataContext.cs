using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class NamedPropsDataContext : DataContext
	{
		public NamedPropsDataContext(NamedPropData[] npda)
		{
			this.npda = npda;
		}

		public override string ToString()
		{
			return string.Format("NamedProps: {0}", CommonUtils.ConcatEntries<NamedPropData>(this.npda, null));
		}

		private NamedPropData[] npda;
	}
}
