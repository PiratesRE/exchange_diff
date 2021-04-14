using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal abstract class AddRecipientAction : TransportAction
	{
		protected AddRecipientAction(ShortList<Argument> arguments, bool isToRecipient) : base(arguments)
		{
			this.isToRecipient = isToRecipient;
		}

		public override Type[] ArgumentsType
		{
			get
			{
				return AddRecipientAction.addressType;
			}
		}

		public override TransportActionType Type
		{
			get
			{
				return TransportActionType.BifurcationNeeded;
			}
		}

		public virtual string GetDisplayName(RulesEvaluationContext baseContext)
		{
			return (string)base.Arguments[0].GetValue(baseContext);
		}

		public abstract RecipientP2Type RecipientP2Type { get; }

		protected override ExecutionControl OnExecute(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = (TransportRulesEvaluationContext)baseContext;
			string text = (string)base.Arguments[0].GetValue(transportRulesEvaluationContext);
			string displayName = this.GetDisplayName(transportRulesEvaluationContext);
			RecipientP2Type recipientP2Type;
			EmailRecipientCollection collection;
			if (this.isToRecipient)
			{
				ExTraceGlobals.TransportRulesEngineTracer.TraceDebug<string, string>(0L, "AddToRecipient, name: {0} address : {1}", displayName, text);
				recipientP2Type = RecipientP2Type.To;
				collection = transportRulesEvaluationContext.MailItem.Message.To;
			}
			else
			{
				ExTraceGlobals.TransportRulesEngineTracer.TraceDebug<string, string>(0L, "AddCcRecipient, name: {0} address : {1}", displayName, text);
				recipientP2Type = RecipientP2Type.Cc;
				collection = transportRulesEvaluationContext.MailItem.Message.Cc;
			}
			if (!AddRecipientAction.RecipientCollectionContainsAddress(transportRulesEvaluationContext, collection, text))
			{
				transportRulesEvaluationContext.RecipientsToAdd.Add(new TransportRulesEvaluationContext.AddedRecipient(text, displayName, recipientP2Type));
			}
			return ExecutionControl.Execute;
		}

		internal static bool RecipientCollectionContainsAddress(TransportRulesEvaluationContext context, EmailRecipientCollection collection, string address)
		{
			AddressBook addressBook = context.Server.AddressBook;
			foreach (EmailRecipient emailRecipient in collection)
			{
				string smtpAddress = emailRecipient.SmtpAddress;
				if (!string.IsNullOrEmpty(smtpAddress) && addressBook.IsSameRecipient((RoutingAddress)smtpAddress, (RoutingAddress)address))
				{
					return true;
				}
			}
			return false;
		}

		private static Type[] addressType = new Type[]
		{
			typeof(string)
		};

		private bool isToRecipient;
	}
}
