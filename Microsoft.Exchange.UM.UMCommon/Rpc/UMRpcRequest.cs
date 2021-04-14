using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.Rpc
{
	[Serializable]
	public abstract class UMRpcRequest : UMVersionedRpcRequest
	{
		public UMRpcRequest()
		{
		}

		internal UMRpcRequest(ADUser user) : this()
		{
			this.user = (ADUser)user.Clone();
			this.userId = user.Id;
			this.policyId = user.UMMailboxPolicy;
			this.dialPlanId = user.UMRecipientDialPlanId;
			this.addressList = user.EmailAddresses.ToStringArray();
			this.domainController = user.OriginatingServer;
			using (UMRecipient umrecipient = UMRecipient.Factory.FromADRecipient<UMRecipient>(this.user))
			{
				this.mailboxSiteId = umrecipient.MailboxServerSite;
			}
		}

		public ADObjectId UserId
		{
			get
			{
				return this.userId;
			}
			set
			{
				this.userId = value;
			}
		}

		public ADObjectId PolicyId
		{
			get
			{
				return this.policyId;
			}
			set
			{
				this.policyId = value;
			}
		}

		public ADObjectId DialPlanId
		{
			get
			{
				return this.dialPlanId;
			}
			set
			{
				this.dialPlanId = value;
			}
		}

		public ADObjectId MailboxSiteId
		{
			get
			{
				return this.mailboxSiteId;
			}
			set
			{
				this.mailboxSiteId = value;
			}
		}

		public string[] AddressList
		{
			get
			{
				return this.addressList;
			}
			set
			{
				this.addressList = value;
			}
		}

		public string DomainController
		{
			get
			{
				return this.domainController;
			}
			set
			{
				this.domainController = value;
			}
		}

		public ADUser User
		{
			get
			{
				throw new InvalidOperationException();
			}
		}

		protected override void LogErrorEvent(Exception ex)
		{
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_RPCRequestError, null, new object[]
			{
				this.GetFriendlyName(),
				(this.user != null) ? this.user.Name : string.Empty,
				CommonUtil.ToEventLogString(ex)
			});
		}

		protected virtual void PopulateUserFields(ADUser adUser)
		{
		}

		private ADObjectId userId;

		private ADObjectId policyId;

		private ADObjectId dialPlanId;

		private string[] addressList;

		private string domainController;

		[NonSerialized]
		private ADUser user;

		[NonSerialized]
		private ADObjectId mailboxSiteId;
	}
}
