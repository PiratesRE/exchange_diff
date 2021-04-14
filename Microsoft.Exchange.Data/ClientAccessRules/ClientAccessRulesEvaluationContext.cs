using System;
using System.Net;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Data.ClientAccessRules
{
	internal class ClientAccessRulesEvaluationContext : RulesEvaluationContext
	{
		public ClientAccessRulesEvaluationContext(RuleCollection rules, string username, IPEndPoint remoteEndpoint, ClientAccessProtocol protocol, ClientAccessAuthenticationMethod authenticationType, IReadOnlyPropertyBag userPropertyBag, ObjectSchema userSchema, Action<ClientAccessRulesEvaluationContext> denyAccessDelegate, Action<Rule, ClientAccessRulesAction> whatIfActionDelegate, long traceId) : base(rules)
		{
			this.AuthenticationType = authenticationType;
			this.UserName = username;
			this.RemoteEndpoint = remoteEndpoint;
			this.Protocol = protocol;
			this.User = userPropertyBag;
			this.UserSchema = userSchema;
			this.DenyAccessDelegate = denyAccessDelegate;
			this.WhatIfActionDelegate = whatIfActionDelegate;
			this.WhatIf = (whatIfActionDelegate != null);
			base.Tracer = new ClientAccessRulesTracer(traceId);
		}

		public string UserName { get; private set; }

		public IPEndPoint RemoteEndpoint { get; private set; }

		public ClientAccessAuthenticationMethod AuthenticationMethod { get; private set; }

		public ClientAccessProtocol Protocol { get; private set; }

		public ClientAccessAuthenticationMethod AuthenticationType { get; private set; }

		public IReadOnlyPropertyBag User { get; private set; }

		public ObjectSchema UserSchema { get; private set; }

		public Action<ClientAccessRulesEvaluationContext> DenyAccessDelegate { get; private set; }

		internal bool WhatIf { get; private set; }

		internal Action<Rule, ClientAccessRulesAction> WhatIfActionDelegate { get; private set; }
	}
}
