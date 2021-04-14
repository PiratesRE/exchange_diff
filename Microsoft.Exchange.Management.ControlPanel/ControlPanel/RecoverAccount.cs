using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class RecoverAccount : BaseRow
	{
		public RecoverAccount(RemovedMailbox removedMailbox) : base(removedMailbox)
		{
			if (Util.IsMicrosoftHostedOnly)
			{
				this.UserName = removedMailbox.WindowsLiveID.Local;
				this.Domain = removedMailbox.WindowsLiveID.Domain;
				this.OriginalLiveID = removedMailbox.WindowsLiveID.ToString();
				this.IsPasswordRequired = removedMailbox.IsPasswordResetRequired;
			}
			else
			{
				this.UserName = removedMailbox.Name;
			}
			this.RemovedMailbox = removedMailbox.Guid.ToString();
		}

		public RecoverAccount(Mailbox mailbox) : base(mailbox)
		{
			if (Util.IsMicrosoftHostedOnly)
			{
				this.UserName = mailbox.WindowsLiveID.Local;
				this.Domain = mailbox.WindowsLiveID.Domain;
				this.OriginalLiveID = mailbox.WindowsLiveID.ToString();
				this.IsPasswordRequired = false;
				this.NeedCredential = true;
				using (MultiValuedProperty<ProxyAddress>.Enumerator enumerator = mailbox.EmailAddresses.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ProxyAddress proxyAddress = enumerator.Current;
						if (proxyAddress.Prefix.ToString().Equals("DLTDNETID", StringComparison.OrdinalIgnoreCase))
						{
							this.NeedCredential = false;
							break;
						}
					}
					goto IL_BE;
				}
			}
			this.UserName = mailbox.Name;
			IL_BE:
			this.SoftDeletedMailbox = mailbox.Guid.ToString();
		}

		[DataMember]
		public string UserName { get; private set; }

		[DataMember]
		public string Domain { get; private set; }

		[DataMember]
		public string OriginalLiveID { get; private set; }

		[DataMember]
		public string RemovedMailbox { get; private set; }

		[DataMember]
		public bool IsPasswordRequired { get; private set; }

		[DataMember]
		public string SoftDeletedMailbox { get; private set; }

		[DataMember]
		public bool NeedCredential { get; private set; }

		private const string DELETED_NETID_PREFIX = "DLTDNETID";
	}
}
