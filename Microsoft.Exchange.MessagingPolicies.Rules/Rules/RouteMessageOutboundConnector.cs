using System;
using System.Web;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Configuration;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class RouteMessageOutboundConnector : TransportAction
	{
		public RouteMessageOutboundConnector(ShortList<Argument> arguments) : base(arguments)
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
				return "RouteMessageOutboundConnector";
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
				return RouteMessageOutboundConnector.ArgumentTypes;
			}
		}

		protected override ExecutionControl OnExecute(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext context = (TransportRulesEvaluationContext)baseContext;
			string connectorName = (string)base.Arguments[0].GetValue(context);
			return RouteMessageOutboundConnector.RouteByConnectorName(context, connectorName);
		}

		internal static ExecutionControl RouteByConnectorName(TransportRulesEvaluationContext context, string connectorName)
		{
			if (context.EventType == EventType.EndOfData || context.EventType == EventType.OnRoutedMessage)
			{
				return ExecutionControl.Execute;
			}
			if (context.OnResolvedSource == null)
			{
				throw new RuleInvalidOperationException("Routing actions can only be called at OnResolvedMessage");
			}
			ExTraceGlobals.TransportRulesEngineTracer.TraceDebug(0L, "Message is conditional routed through specified connector at OnResolvedMessage");
			TenantOutboundConnector enabledOutboundConnector;
			try
			{
				enabledOutboundConnector = ConnectorConfigurationSession.GetEnabledOutboundConnector(TransportUtils.GetTransportMailItem(context.MailItem).OrganizationId, connectorName);
			}
			catch (OutboundConnectorNotFoundException)
			{
				context.ResetRulesCache();
				throw;
			}
			Header header = Header.Create("X-MS-Exchange-Organization-OutboundConnector");
			header.Value = string.Format("{0};{1}", enabledOutboundConnector.Guid, HttpUtility.UrlEncode(connectorName));
			context.MailItem.Message.MimeDocument.RootPart.Headers.RemoveAll("X-MS-Exchange-Organization-OutboundConnector");
			context.MailItem.Message.MimeDocument.RootPart.Headers.AppendChild(header);
			Header header2 = Header.Create("X-MS-Exchange-Forest-OutboundConnector");
			header2.Value = header.Value;
			context.MailItem.Message.MimeDocument.RootPart.Headers.RemoveAll("X-MS-Exchange-Forest-OutboundConnector");
			context.MailItem.Message.MimeDocument.RootPart.Headers.AppendChild(header2);
			return ExecutionControl.Execute;
		}

		internal static readonly Version ConditionBasedRoutingBaseVersion = new Version("15.00.0002.00");

		private static readonly Type[] ArgumentTypes = new Type[]
		{
			typeof(string)
		};
	}
}
