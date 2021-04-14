using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetPasswordExpirationDate : SingleStepServiceCommand<GetPasswordExpirationDateRequest, DateTime>
	{
		public GetPasswordExpirationDate(CallContext callContext, GetPasswordExpirationDateRequest request) : base(callContext, request)
		{
			this.mailboxSmtpAddress = request.MailboxSmtpAddress;
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new GetPasswordExpirationDateResponse(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<DateTime> Execute()
		{
			ExchangePrincipal principal = GetPasswordExpirationDate.GetPrincipal(base.CallContext, this.mailboxSmtpAddress);
			DateTime value = DateTime.MaxValue;
			if (VariantConfiguration.InvariantNoFlightingSnapshot.Ews.UserPasswordExpirationDate.Enabled)
			{
				if (principal != base.CallContext.AccessingPrincipal)
				{
					throw new ServiceArgumentException((CoreResources.IDs)3784063568U);
				}
				CommonAccessToken commonAccessToken = base.CallContext.HttpContext.Items["Item-CommonAccessToken"] as CommonAccessToken;
				if (commonAccessToken == null || commonAccessToken.TokenType != AccessTokenType.LiveIdBasic.ToString())
				{
					throw new ServiceArgumentException((CoreResources.IDs)3784063568U);
				}
				string text = null;
				if (commonAccessToken.ExtensionData.ContainsKey("PasswordExpiry"))
				{
					text = commonAccessToken.ExtensionData["PasswordExpiry"];
				}
				uint num;
				if (!string.IsNullOrEmpty(text) && uint.TryParse(text, out num))
				{
					value = DateTime.UtcNow.AddDays(num);
				}
			}
			else if (principal.MasterAccountSid == null)
			{
				value = DirectoryHelper.GetPasswordExpirationDate(principal.ObjectId, base.MailboxIdentityMailboxSession.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid)).UniversalTime;
			}
			return new ServiceResult<DateTime>(value);
		}

		private static ExchangePrincipal GetPrincipal(CallContext callContext, string smtpAddress)
		{
			ExchangePrincipal exchangePrincipal;
			if (!string.IsNullOrEmpty(smtpAddress))
			{
				exchangePrincipal = ExchangePrincipalCache.GetFromCache(smtpAddress, callContext.ADRecipientSessionContext);
			}
			else
			{
				exchangePrincipal = callContext.AccessingPrincipal;
				if (exchangePrincipal == null)
				{
					throw new NonExistentMailboxException((CoreResources.IDs)4088802584U, string.Empty);
				}
			}
			return exchangePrincipal;
		}

		private string mailboxSmtpAddress;
	}
}
