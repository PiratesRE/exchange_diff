using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal abstract class TransportAction : Action
	{
		public TransportAction(ShortList<Argument> arguments) : base(arguments)
		{
		}

		public virtual TransportActionType Type
		{
			get
			{
				return TransportActionType.NonRecipientRelated;
			}
		}

		internal static ExEventLog Logger
		{
			get
			{
				return TransportAction.logger.Value;
			}
		}

		private static Lazy<ExEventLog> logger = new Lazy<ExEventLog>(() => new ExEventLog(new Guid("7D2A0005-2C75-42ac-B495-8FE62F3B4FCF"), "MSExchange Messaging Policies"));
	}
}
