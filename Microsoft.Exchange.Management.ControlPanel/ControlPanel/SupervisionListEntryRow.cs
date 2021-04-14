using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Management.RecipientTasks;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SupervisionListEntryRow : BaseRow
	{
		public SupervisionListEntryRow(SupervisionListEntry supervisionListEntry) : base(supervisionListEntry.ToIdentity(), supervisionListEntry)
		{
			this.SupervisionListEntry = supervisionListEntry;
		}

		public SupervisionListEntry SupervisionListEntry { get; private set; }

		[DataMember]
		public string EntryName
		{
			get
			{
				return this.SupervisionListEntry.EntryName;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		public string Tag
		{
			get
			{
				return this.SupervisionListEntry.Tag;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}
	}
}
