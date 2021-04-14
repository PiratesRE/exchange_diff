using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class PropTagsDataContext : DataContext
	{
		public PropTagsDataContext(PropTag[] ptags)
		{
			this.ptags = ptags;
		}

		public override string ToString()
		{
			return string.Format("PropTags: {0}", CommonUtils.ConcatEntries<PropTag>(this.ptags, new Func<PropTag, string>(TraceUtils.DumpPropTag)));
		}

		private PropTag[] ptags;
	}
}
