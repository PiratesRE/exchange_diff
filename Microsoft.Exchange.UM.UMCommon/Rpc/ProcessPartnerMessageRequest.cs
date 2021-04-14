using System;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.Rpc
{
	[Serializable]
	public class ProcessPartnerMessageRequest : UMVersionedRpcRequest
	{
		public Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
			set
			{
				this.mailboxGuid = value;
			}
		}

		public Guid TenantGuid
		{
			get
			{
				return this.tenantGuid;
			}
			set
			{
				this.tenantGuid = value;
			}
		}

		public string ItemId
		{
			get
			{
				return this.itemId;
			}
			set
			{
				this.itemId = value;
			}
		}

		internal ProcessPartnerMessageDelegate ProcessPartnerMessage { get; set; }

		internal override string GetFriendlyName()
		{
			return Strings.ProcessPartnerMessageRequest;
		}

		internal override UMRpcResponse Execute()
		{
			this.ProcessPartnerMessage(this);
			return null;
		}

		protected override void LogErrorEvent(Exception exception)
		{
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_UMPartnerMessageRpcRequestError, null, new object[]
			{
				this.GetFriendlyName(),
				CommonUtil.ToEventLogString(exception)
			});
		}

		private Guid mailboxGuid;

		private Guid tenantGuid;

		private string itemId;
	}
}
