using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class UMMailboxRow : BaseRow
	{
		public UMMailboxRow(UMMailbox umMailbox) : base(umMailbox)
		{
			this.UMMailbox = umMailbox;
		}

		internal UMMailbox UMMailbox { get; private set; }
	}
}
