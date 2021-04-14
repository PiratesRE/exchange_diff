using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class MailboxTransportSmtpInStateMachine : TransportSmtpInStateMachine<MailboxTransportSmtpState>
	{
		public MailboxTransportSmtpInStateMachine(SmtpInSessionState sessionState) : base(sessionState, MailboxTransportSmtpState.WaitingForGreeting, MailboxTransportSmtpInStateMachine.StateTransitions)
		{
		}

		protected override ISmtpInCommandFactory<SmtpInStateMachineEvents> CreateCommandFactory()
		{
			return new MailboxTransportSmtpInCommandFactory(this.sessionState, new AwaitCompletedDelegate(this.OnAwaitCompleted));
		}

		protected override bool ReachedEndState
		{
			get
			{
				return base.CurrentState == MailboxTransportSmtpState.End;
			}
		}

		protected override SmtpInStateMachineEvents GetCompletedEventForCommand(SmtpInCommand commandType)
		{
			switch (commandType)
			{
			case SmtpInCommand.AUTH:
				return SmtpInStateMachineEvents.AuthProcessed;
			case SmtpInCommand.BDAT:
				return SmtpInStateMachineEvents.BdatProcessed;
			case SmtpInCommand.DATA:
				return SmtpInStateMachineEvents.DataProcessed;
			case SmtpInCommand.EHLO:
				return SmtpInStateMachineEvents.EhloProcessed;
			case SmtpInCommand.EXPN:
				return SmtpInStateMachineEvents.ExpnProcessed;
			case SmtpInCommand.HELO:
				return SmtpInStateMachineEvents.HeloProcessed;
			case SmtpInCommand.HELP:
				return SmtpInStateMachineEvents.HelpProcessed;
			case SmtpInCommand.MAIL:
				return SmtpInStateMachineEvents.MailProcessed;
			case SmtpInCommand.NOOP:
				return SmtpInStateMachineEvents.NoopProcessed;
			case SmtpInCommand.QUIT:
				return SmtpInStateMachineEvents.QuitProcessed;
			case SmtpInCommand.RCPT:
				return SmtpInStateMachineEvents.RcptProcessed;
			case SmtpInCommand.RSET:
				return SmtpInStateMachineEvents.RsetProcessed;
			case SmtpInCommand.STARTTLS:
				return SmtpInStateMachineEvents.StartTlsProcessed;
			case SmtpInCommand.VRFY:
				return SmtpInStateMachineEvents.VrfyProcessed;
			case SmtpInCommand.XANONYMOUSTLS:
				return SmtpInStateMachineEvents.XAnonymousTlsProcessed;
			case SmtpInCommand.XEXPS:
				return SmtpInStateMachineEvents.XExpsProcessed;
			case SmtpInCommand.XSESSIONPARAMS:
				return SmtpInStateMachineEvents.XSessionParamsProcessed;
			}
			return SmtpInStateMachineEvents.CommandFailed;
		}

		private static Dictionary<StateTransition<MailboxTransportSmtpState, SmtpInStateMachineEvents>, MailboxTransportSmtpState> CreateStateTransitions()
		{
			Dictionary<StateTransition<MailboxTransportSmtpState, SmtpInStateMachineEvents>, MailboxTransportSmtpState> dictionary = new Dictionary<StateTransition<MailboxTransportSmtpState, SmtpInStateMachineEvents>, MailboxTransportSmtpState>();
			StateMachineUtils<MailboxTransportSmtpState>.AddStateChangeTransition(MailboxTransportSmtpState.WaitingForGreeting, SmtpInStateMachineEvents.HeloProcessed, MailboxTransportSmtpState.GreetingReceived, dictionary);
			StateMachineUtils<MailboxTransportSmtpState>.AddStateChangeTransition(MailboxTransportSmtpState.WaitingForGreeting, SmtpInStateMachineEvents.EhloProcessed, MailboxTransportSmtpState.GreetingReceived, dictionary);
			MailboxTransportSmtpInStateMachine.AddDefaultTransitions(MailboxTransportSmtpState.WaitingForGreeting, dictionary);
			StateMachineUtils<MailboxTransportSmtpState>.AddNoStateChangeTransition(MailboxTransportSmtpState.GreetingReceived, SmtpInStateMachineEvents.HeloProcessed, dictionary);
			StateMachineUtils<MailboxTransportSmtpState>.AddNoStateChangeTransition(MailboxTransportSmtpState.GreetingReceived, SmtpInStateMachineEvents.EhloProcessed, dictionary);
			StateMachineUtils<MailboxTransportSmtpState>.AddNoStateChangeTransition(MailboxTransportSmtpState.GreetingReceived, SmtpInStateMachineEvents.AuthProcessed, dictionary);
			StateMachineUtils<MailboxTransportSmtpState>.AddNoStateChangeTransition(MailboxTransportSmtpState.GreetingReceived, SmtpInStateMachineEvents.XExpsProcessed, dictionary);
			StateMachineUtils<MailboxTransportSmtpState>.AddStateChangeTransition(MailboxTransportSmtpState.GreetingReceived, SmtpInStateMachineEvents.StartTlsProcessed, MailboxTransportSmtpState.WaitingForSecureGreeting, dictionary);
			StateMachineUtils<MailboxTransportSmtpState>.AddStateChangeTransition(MailboxTransportSmtpState.GreetingReceived, SmtpInStateMachineEvents.XAnonymousTlsProcessed, MailboxTransportSmtpState.WaitingForSecureGreeting, dictionary);
			StateMachineUtils<MailboxTransportSmtpState>.AddStateChangeTransition(MailboxTransportSmtpState.GreetingReceived, SmtpInStateMachineEvents.MailProcessed, MailboxTransportSmtpState.MailTransactionStarted, dictionary);
			MailboxTransportSmtpInStateMachine.AddDefaultTransitions(MailboxTransportSmtpState.GreetingReceived, dictionary);
			StateMachineUtils<MailboxTransportSmtpState>.AddStateChangeTransition(MailboxTransportSmtpState.WaitingForSecureGreeting, SmtpInStateMachineEvents.HeloProcessed, MailboxTransportSmtpState.SecureGreetingReceived, dictionary);
			StateMachineUtils<MailboxTransportSmtpState>.AddStateChangeTransition(MailboxTransportSmtpState.WaitingForSecureGreeting, SmtpInStateMachineEvents.EhloProcessed, MailboxTransportSmtpState.SecureGreetingReceived, dictionary);
			MailboxTransportSmtpInStateMachine.AddDefaultTransitions(MailboxTransportSmtpState.WaitingForSecureGreeting, dictionary);
			StateMachineUtils<MailboxTransportSmtpState>.AddNoStateChangeTransition(MailboxTransportSmtpState.SecureGreetingReceived, SmtpInStateMachineEvents.HeloProcessed, dictionary);
			StateMachineUtils<MailboxTransportSmtpState>.AddNoStateChangeTransition(MailboxTransportSmtpState.SecureGreetingReceived, SmtpInStateMachineEvents.EhloProcessed, dictionary);
			StateMachineUtils<MailboxTransportSmtpState>.AddNoStateChangeTransition(MailboxTransportSmtpState.SecureGreetingReceived, SmtpInStateMachineEvents.AuthProcessed, dictionary);
			StateMachineUtils<MailboxTransportSmtpState>.AddNoStateChangeTransition(MailboxTransportSmtpState.SecureGreetingReceived, SmtpInStateMachineEvents.XExpsProcessed, dictionary);
			StateMachineUtils<MailboxTransportSmtpState>.AddNoStateChangeTransition(MailboxTransportSmtpState.SecureGreetingReceived, SmtpInStateMachineEvents.XSessionParamsProcessed, dictionary);
			StateMachineUtils<MailboxTransportSmtpState>.AddStateChangeTransition(MailboxTransportSmtpState.SecureGreetingReceived, SmtpInStateMachineEvents.MailProcessed, MailboxTransportSmtpState.MailTransactionStarted, dictionary);
			MailboxTransportSmtpInStateMachine.AddDefaultTransitions(MailboxTransportSmtpState.SecureGreetingReceived, dictionary);
			StateMachineUtils<MailboxTransportSmtpState>.AddStateChangeTransition(MailboxTransportSmtpState.MailTransactionStarted, SmtpInStateMachineEvents.HeloProcessed, MailboxTransportSmtpState.GreetingReceived, dictionary);
			StateMachineUtils<MailboxTransportSmtpState>.AddStateChangeTransition(MailboxTransportSmtpState.MailTransactionStarted, SmtpInStateMachineEvents.EhloProcessed, MailboxTransportSmtpState.GreetingReceived, dictionary);
			StateMachineUtils<MailboxTransportSmtpState>.AddStateChangeTransition(MailboxTransportSmtpState.MailTransactionStarted, SmtpInStateMachineEvents.RcptProcessed, MailboxTransportSmtpState.WaitingForMoreRecipientsOrData, dictionary);
			MailboxTransportSmtpInStateMachine.AddDefaultTransitions(MailboxTransportSmtpState.MailTransactionStarted, dictionary);
			StateMachineUtils<MailboxTransportSmtpState>.AddStateChangeTransition(MailboxTransportSmtpState.WaitingForMoreRecipientsOrData, SmtpInStateMachineEvents.HeloProcessed, MailboxTransportSmtpState.GreetingReceived, dictionary);
			StateMachineUtils<MailboxTransportSmtpState>.AddStateChangeTransition(MailboxTransportSmtpState.WaitingForMoreRecipientsOrData, SmtpInStateMachineEvents.EhloProcessed, MailboxTransportSmtpState.GreetingReceived, dictionary);
			StateMachineUtils<MailboxTransportSmtpState>.AddNoStateChangeTransition(MailboxTransportSmtpState.WaitingForMoreRecipientsOrData, SmtpInStateMachineEvents.RcptProcessed, dictionary);
			StateMachineUtils<MailboxTransportSmtpState>.AddStateChangeTransition(MailboxTransportSmtpState.WaitingForMoreRecipientsOrData, SmtpInStateMachineEvents.DataProcessed, MailboxTransportSmtpState.GreetingReceived, dictionary);
			StateMachineUtils<MailboxTransportSmtpState>.AddStateChangeTransition(MailboxTransportSmtpState.WaitingForMoreRecipientsOrData, SmtpInStateMachineEvents.BdatLastProcessed, MailboxTransportSmtpState.GreetingReceived, dictionary);
			StateMachineUtils<MailboxTransportSmtpState>.AddStateChangeTransition(MailboxTransportSmtpState.WaitingForMoreRecipientsOrData, SmtpInStateMachineEvents.BdatProcessed, MailboxTransportSmtpState.ReceivingBdatChunks, dictionary);
			StateMachineUtils<MailboxTransportSmtpState>.AddStateChangeTransition(MailboxTransportSmtpState.WaitingForMoreRecipientsOrData, SmtpInStateMachineEvents.DataFailed, MailboxTransportSmtpState.GreetingReceived, dictionary);
			StateMachineUtils<MailboxTransportSmtpState>.AddStateChangeTransition(MailboxTransportSmtpState.WaitingForMoreRecipientsOrData, SmtpInStateMachineEvents.BdatFailed, MailboxTransportSmtpState.ReceivingBdatChunks, dictionary);
			MailboxTransportSmtpInStateMachine.AddDefaultTransitions(MailboxTransportSmtpState.WaitingForMoreRecipientsOrData, dictionary);
			StateMachineUtils<MailboxTransportSmtpState>.AddNoStateChangeTransition(MailboxTransportSmtpState.ReceivingBdatChunks, SmtpInStateMachineEvents.BdatProcessed, dictionary);
			StateMachineUtils<MailboxTransportSmtpState>.AddStateChangeTransition(MailboxTransportSmtpState.ReceivingBdatChunks, SmtpInStateMachineEvents.BdatLastProcessed, MailboxTransportSmtpState.GreetingReceived, dictionary);
			StateMachineUtils<MailboxTransportSmtpState>.AddNoStateChangeTransition(MailboxTransportSmtpState.ReceivingBdatChunks, SmtpInStateMachineEvents.BdatFailed, dictionary);
			MailboxTransportSmtpInStateMachine.AddDefaultTransitions(MailboxTransportSmtpState.ReceivingBdatChunks, dictionary);
			return dictionary;
		}

		public static void AddDefaultTransitions(MailboxTransportSmtpState fromState, ICollection<KeyValuePair<StateTransition<MailboxTransportSmtpState, SmtpInStateMachineEvents>, MailboxTransportSmtpState>> stateTransitions)
		{
			StateMachineUtils<MailboxTransportSmtpState>.AddStateChangeTransition(fromState, SmtpInStateMachineEvents.QuitProcessed, MailboxTransportSmtpState.End, stateTransitions);
			StateMachineUtils<MailboxTransportSmtpState>.AddStateChangeTransition(fromState, SmtpInStateMachineEvents.NetworkError, MailboxTransportSmtpState.End, stateTransitions);
			StateMachineUtils<MailboxTransportSmtpState>.AddStateChangeTransition(fromState, SmtpInStateMachineEvents.SendResponseAndDisconnectClient, MailboxTransportSmtpState.End, stateTransitions);
			StateMachineUtils<MailboxTransportSmtpState>.AddNoStateChangeTransition(fromState, SmtpInStateMachineEvents.CommandFailed, stateTransitions);
			StateMachineUtils<MailboxTransportSmtpState>.AddNoStateChangeTransition(fromState, SmtpInStateMachineEvents.HelpProcessed, stateTransitions);
			StateMachineUtils<MailboxTransportSmtpState>.AddNoStateChangeTransition(fromState, SmtpInStateMachineEvents.NoopProcessed, stateTransitions);
			if (fromState == MailboxTransportSmtpState.WaitingForGreeting)
			{
				StateMachineUtils<MailboxTransportSmtpState>.AddNoStateChangeTransition(fromState, SmtpInStateMachineEvents.RsetProcessed, stateTransitions);
			}
			else
			{
				StateMachineUtils<MailboxTransportSmtpState>.AddStateChangeTransition(fromState, SmtpInStateMachineEvents.RsetProcessed, MailboxTransportSmtpState.GreetingReceived, stateTransitions);
			}
			if (fromState != MailboxTransportSmtpState.WaitingForGreeting)
			{
				StateMachineUtils<MailboxTransportSmtpState>.AddNoStateChangeTransition(fromState, SmtpInStateMachineEvents.ExpnProcessed, stateTransitions);
				StateMachineUtils<MailboxTransportSmtpState>.AddNoStateChangeTransition(fromState, SmtpInStateMachineEvents.VrfyProcessed, stateTransitions);
			}
		}

		private static readonly Dictionary<StateTransition<MailboxTransportSmtpState, SmtpInStateMachineEvents>, MailboxTransportSmtpState> StateTransitions = MailboxTransportSmtpInStateMachine.CreateStateTransitions();
	}
}
