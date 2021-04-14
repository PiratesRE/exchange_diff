using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class ModerateMessageByUser : TransportAction
	{
		public ModerateMessageByUser(ShortList<Argument> arguments) : base(arguments)
		{
		}

		public override Version MinimumVersion
		{
			get
			{
				return TransportRuleConstants.VersionedContainerBaseVersion;
			}
		}

		public override string Name
		{
			get
			{
				return "ModerateMessageByUser";
			}
		}

		public override TransportActionType Type
		{
			get
			{
				return TransportActionType.BifurcationNeeded;
			}
		}

		public override Type[] ArgumentsType
		{
			get
			{
				return ModerateMessageByUser.argumentTypes;
			}
		}

		protected virtual string GetModeratorList(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext context = (TransportRulesEvaluationContext)baseContext;
			return (string)base.Arguments[0].GetValue(context);
		}

		protected override ExecutionControl OnExecute(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = (TransportRulesEvaluationContext)baseContext;
			MailItem mailItem = transportRulesEvaluationContext.MailItem;
			if (mailItem.Message.MapiMessageClass.StartsWith("IPM.Note.Microsoft.Approval", StringComparison.OrdinalIgnoreCase))
			{
				return ExecutionControl.Execute;
			}
			string moderatorList = this.GetModeratorList(transportRulesEvaluationContext);
			if (string.IsNullOrEmpty(moderatorList))
			{
				return ExecutionControl.Execute;
			}
			TransportRulesEvaluator.AddRuleToExecutionHistory(transportRulesEvaluationContext);
			ITransportMailItemFacade transportMailItemFacade = TransportUtils.GetTransportMailItemFacade(transportRulesEvaluationContext.MailItem);
			OrganizationId organizationId = (OrganizationId)transportMailItemFacade.OrganizationIdAsObject;
			try
			{
				SmtpResponse comparand = TransportFacades.CreateAndSubmitApprovalInitiationForTransportRules(transportMailItemFacade, transportRulesEvaluationContext.MailItem.FromAddress.ToString(), moderatorList, transportRulesEvaluationContext.RuleName);
				if (!SmtpResponse.Empty.Equals(comparand))
				{
					return RejectMessage.Reject(transportRulesEvaluationContext, comparand.StatusCode, comparand.EnhancedStatusCode, comparand.StatusText[0]);
				}
			}
			catch (ExchangeDataException arg)
			{
				ExTraceGlobals.TransportRulesEngineTracer.TraceError<ExchangeDataException>(0L, "Approval initiation not created due to ExchangeDataException {0}.  NDRing the message.", arg);
				return RejectMessage.Reject(transportRulesEvaluationContext, ModerateMessageByUser.InvalidContentForModeration.StatusCode, ModerateMessageByUser.InvalidContentForModeration.EnhancedStatusCode, ModerateMessageByUser.InvalidContentForModeration.StatusText[0]);
			}
			return DeleteMessage.Delete(transportRulesEvaluationContext, ModerateMessageByUser.TrackingLogResponse);
		}

		private static readonly SmtpResponse TrackingLogResponse = new SmtpResponse("550", "5.2.1", new string[]
		{
			"Message sent for moderation by the transport rules agent"
		});

		private static readonly SmtpResponse InvalidContentForModeration = new SmtpResponse(SmtpResponse.InvalidContent.StatusCode, SmtpResponse.InvalidContent.EnhancedStatusCode, new string[]
		{
			"Rules.MT.Content; invalid message content"
		});

		private static Type[] argumentTypes = new Type[]
		{
			typeof(string)
		};
	}
}
