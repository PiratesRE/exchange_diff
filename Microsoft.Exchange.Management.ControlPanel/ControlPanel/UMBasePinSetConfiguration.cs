using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Management.Tasks.UM;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[KnownType(typeof(SetUMMailboxConfiguration))]
	[KnownType(typeof(SetUMMailboxPinConfiguration))]
	[DataContract]
	public abstract class UMBasePinSetConfiguration : UMMailboxRow
	{
		public UMBasePinSetConfiguration(UMMailbox umMailbox) : base(umMailbox)
		{
		}

		public UMMailboxPin UMMailboxPin { get; set; }

		[DataMember]
		public bool AccountLockedOut
		{
			get
			{
				return this.UMMailboxPin != null && this.UMMailboxPin.LockedOut;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string AccountLockedOutStatus
		{
			get
			{
				return this.AccountLockedOut ? Strings.EditUMMailboxAccountLockedOutStatus : Strings.EditUMMailboxAccountActiveStatus;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string AccountLockedOutMessage
		{
			get
			{
				if (!this.AccountLockedOut)
				{
					return string.Empty;
				}
				return Strings.EditUMMailboxLockedoutAction;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}
	}
}
