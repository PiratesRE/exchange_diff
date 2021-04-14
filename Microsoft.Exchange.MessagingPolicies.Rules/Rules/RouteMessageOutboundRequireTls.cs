using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal sealed class RouteMessageOutboundRequireTls : TransportAction
	{
		public RouteMessageOutboundRequireTls(ShortList<Argument> arguments) : base(arguments)
		{
		}

		public override Version MinimumVersion
		{
			get
			{
				return RouteMessageOutboundConnector.ConditionBasedRoutingBaseVersion;
			}
		}

		public override string Name
		{
			get
			{
				return "RouteMessageOutboundRequireTls";
			}
		}

		public override TransportActionType Type
		{
			get
			{
				return TransportActionType.RecipientRelated;
			}
		}

		protected override ExecutionControl OnExecute(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext context = (TransportRulesEvaluationContext)baseContext;
			return RouteMessageOutboundRequireTls.RequireOutboundTls(context);
		}

		internal static ExecutionControl RequireOutboundTls(TransportRulesEvaluationContext context)
		{
			if (context.EventType == EventType.EndOfData || context.EventType == EventType.OnRoutedMessage)
			{
				return ExecutionControl.Execute;
			}
			if (context.OnResolvedSource == null)
			{
				throw new RuleInvalidOperationException("Routing actions can only be called at OnResolvedMessage");
			}
			ExTraceGlobals.TransportRulesEngineTracer.TraceDebug(0L, "Message is forced outbound TLS at OnResolvedMessage");
			RoutingActionUtils.ProcessRecipients(context, delegate(EnvelopeRecipient recipient)
			{
				context.OnResolvedSource.SetTlsAuthLevel(recipient, new RequiredTlsAuthLevel?(RequiredTlsAuthLevel.EncryptionOnly));
			});
			return ExecutionControl.Execute;
		}
	}
}
