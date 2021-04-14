using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.Rpc
{
	[Serializable]
	public abstract class UpdateUMMailboxRequest : UMRpcRequest
	{
		public UpdateUMMailboxRequest()
		{
		}

		internal UpdateUMMailboxRequest(ADUser user) : base(user)
		{
		}

		protected override void PopulateUserFields(ADUser adUser)
		{
			base.PopulateUserFields(adUser);
			adUser.UMRecipientDialPlanId = base.DialPlanId;
			adUser.UMMailboxPolicy = base.PolicyId;
			adUser.EmailAddresses = new ProxyAddressCollection();
			foreach (string text in base.AddressList)
			{
				ProxyAddress proxyAddress = ProxyAddress.Parse(text);
				InvalidProxyAddress invalidProxyAddress = proxyAddress as InvalidProxyAddress;
				if (invalidProxyAddress != null)
				{
					CallIdTracer.TraceWarning(ExTraceGlobals.UtilTracer, 0, "{0}.PopulateUserFields: failed to parse proxy address '{1}'. Skipping it. Error: {2}.", new object[]
					{
						this.GetFriendlyName(),
						text,
						invalidProxyAddress.ParseException
					});
				}
				else
				{
					adUser.EmailAddresses.Add(proxyAddress);
				}
			}
		}
	}
}
