using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ActiveSyncMailboxPolicyRow : BaseRow
	{
		public ActiveSyncMailboxPolicyRow(ActiveSyncMailboxPolicy policy) : base(policy)
		{
			this.ActiveSyncMailboxPolicy = policy;
		}

		protected ActiveSyncMailboxPolicy ActiveSyncMailboxPolicy { get; set; }

		[DataMember]
		public string DisplayName
		{
			get
			{
				if (this.IsDefault)
				{
					return string.Format(Strings.DefaultEASPolicy, this.ActiveSyncMailboxPolicy.Name);
				}
				return this.ActiveSyncMailboxPolicy.Name;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool IsDefault
		{
			get
			{
				return this.ActiveSyncMailboxPolicy.IsDefault;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Modified
		{
			get
			{
				return this.ActiveSyncMailboxPolicy.WhenChangedUTC.UtcToUserDateTimeString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}
	}
}
