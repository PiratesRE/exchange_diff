using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetUMMailboxPinConfiguration : UMBasePinSetConfiguration
	{
		public SetUMMailboxPinConfiguration(UMMailbox umMailbox) : base(umMailbox)
		{
			this.policy = umMailbox.GetPolicy();
		}

		[DataMember]
		public int MinPinLength
		{
			get
			{
				if (this.policy == null)
				{
					return 0;
				}
				return this.policy.MinPINLength;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		private UMMailboxPolicy policy;
	}
}
