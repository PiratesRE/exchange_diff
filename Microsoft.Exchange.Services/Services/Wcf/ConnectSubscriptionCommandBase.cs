using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Connect;

namespace Microsoft.Exchange.Services.Wcf
{
	internal abstract class ConnectSubscriptionCommandBase<TRequest, TResponse, TTask, TResult> : SingleCmdletCommandBase<TRequest, TResponse, TTask, TResult> where TResponse : OptionsResponseBase, new() where TTask : Task
	{
		protected ConnectSubscriptionCommandBase(CallContext callContext, TRequest request, string cmdletName, ScopeLocation rbacScope) : base(callContext, request, cmdletName, rbacScope)
		{
		}

		protected void PopulateTaskParametersFromConnectSubscription(NewConnectSubscriptionData connectSubscription)
		{
			TTask task = this.cmdletRunner.TaskWrapper.Task;
			this.cmdletRunner.SetTaskParameter("Mailbox", task, new MailboxIdParameter(base.CallContext.AccessingPrincipal.ObjectId));
			switch (connectSubscription.ConnectSubscriptionType)
			{
			case ConnectSubscriptionType.Facebook:
				this.PopulateFacebookParameters(task, connectSubscription);
				return;
			case ConnectSubscriptionType.LinkedIn:
				this.PopulateLinkedInParameters(task, connectSubscription);
				return;
			default:
				return;
			}
		}

		protected string CalculateParameterSet(ConnectSubscriptionType connectSubscriptionType)
		{
			switch (connectSubscriptionType)
			{
			case ConnectSubscriptionType.Facebook:
				return "FacebookParameterSet";
			case ConnectSubscriptionType.LinkedIn:
				return "LinkedInParameterSet";
			default:
				throw new InvalidOperationException("Invalid connect subscription type for this command:" + connectSubscriptionType);
			}
		}

		protected ConnectSubscription CreateConnectSubscriptionData(ConnectSubscriptionProxy connectSubscriptionProxy)
		{
			if (connectSubscriptionProxy == null)
			{
				return null;
			}
			return new ConnectSubscription
			{
				DetailedStatus = connectSubscriptionProxy.DetailedStatus,
				DisplayName = connectSubscriptionProxy.DisplayName,
				EmailAddress = connectSubscriptionProxy.EmailAddress.ToString(),
				AggregationType = connectSubscriptionProxy.AggregationType,
				AggregationSubscriptionType = connectSubscriptionProxy.SubscriptionType,
				ConnectState = connectSubscriptionProxy.ConnectState,
				Identity = new Identity(connectSubscriptionProxy.Identity.ToString(), connectSubscriptionProxy.DisplayName),
				IsErrorStatus = connectSubscriptionProxy.IsErrorStatus,
				IsValid = connectSubscriptionProxy.IsValid,
				LastSuccessfulSync = ((connectSubscriptionProxy.LastSuccessfulSync == null) ? null : ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime((ExDateTime)connectSubscriptionProxy.LastSuccessfulSync.Value)),
				Name = connectSubscriptionProxy.Name,
				SendAsState = connectSubscriptionProxy.SendAsState,
				Status = connectSubscriptionProxy.Status,
				StatusDescription = connectSubscriptionProxy.StatusDescription,
				SubscriptionType = connectSubscriptionProxy.SubscriptionType
			};
		}

		protected void PopulateFacebookParameters(TTask task, NewConnectSubscriptionData data)
		{
			this.cmdletRunner.SetTaskParameter("Facebook", task, new SwitchParameter(true));
			this.cmdletRunner.SetTaskParameter("AppAuthorizationCode", task, data.AppAuthorizationCode);
			this.cmdletRunner.SetTaskParameter("RedirectUri", task, data.RedirectUri);
		}

		protected void PopulateLinkedInParameters(TTask task, NewConnectSubscriptionData data)
		{
			this.cmdletRunner.SetTaskParameter("LinkedIn", task, new SwitchParameter(true));
			this.cmdletRunner.SetTaskParameter("RedirectUri", task, data.RedirectUri);
			if (base.CallContext.HttpContext.Request.Cookies[PeopleConstants.RequestSecretCookieName] != null)
			{
				this.cmdletRunner.SetTaskParameter("RequestSecret", task, base.CallContext.HttpContext.Request.Cookies[PeopleConstants.RequestSecretCookieName].Value);
				this.cmdletRunner.SetTaskParameter("RequestToken", task, data.RequestToken);
				this.cmdletRunner.SetTaskParameter("OAuthVerifier", task, data.OAuthVerifier);
				return;
			}
			throw new LinkedInAuthenticationException(NetServerException.LinkedInRequestCookiesMissingOAuthSecret);
		}
	}
}
