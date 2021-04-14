using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging
{
	public static class ExTraceGlobals
	{
		public static Trace ServiceStartTracer
		{
			get
			{
				if (ExTraceGlobals.serviceStartTracer == null)
				{
					ExTraceGlobals.serviceStartTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.serviceStartTracer;
			}
		}

		public static Trace ServiceTracer
		{
			get
			{
				if (ExTraceGlobals.serviceTracer == null)
				{
					ExTraceGlobals.serviceTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.serviceTracer;
			}
		}

		public static Trace ServiceStopTracer
		{
			get
			{
				if (ExTraceGlobals.serviceStopTracer == null)
				{
					ExTraceGlobals.serviceStopTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.serviceStopTracer;
			}
		}

		public static Trace VoipPlatformTracer
		{
			get
			{
				if (ExTraceGlobals.voipPlatformTracer == null)
				{
					ExTraceGlobals.voipPlatformTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.voipPlatformTracer;
			}
		}

		public static Trace CallSessionTracer
		{
			get
			{
				if (ExTraceGlobals.callSessionTracer == null)
				{
					ExTraceGlobals.callSessionTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.callSessionTracer;
			}
		}

		public static Trace StackIfTracer
		{
			get
			{
				if (ExTraceGlobals.stackIfTracer == null)
				{
					ExTraceGlobals.stackIfTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.stackIfTracer;
			}
		}

		public static Trace StateMachineTracer
		{
			get
			{
				if (ExTraceGlobals.stateMachineTracer == null)
				{
					ExTraceGlobals.stateMachineTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.stateMachineTracer;
			}
		}

		public static Trace GreetingsTracer
		{
			get
			{
				if (ExTraceGlobals.greetingsTracer == null)
				{
					ExTraceGlobals.greetingsTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.greetingsTracer;
			}
		}

		public static Trace AuthenticationTracer
		{
			get
			{
				if (ExTraceGlobals.authenticationTracer == null)
				{
					ExTraceGlobals.authenticationTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.authenticationTracer;
			}
		}

		public static Trace VoiceMailTracer
		{
			get
			{
				if (ExTraceGlobals.voiceMailTracer == null)
				{
					ExTraceGlobals.voiceMailTracer = new Trace(ExTraceGlobals.componentGuid, 9);
				}
				return ExTraceGlobals.voiceMailTracer;
			}
		}

		public static Trace CalendarTracer
		{
			get
			{
				if (ExTraceGlobals.calendarTracer == null)
				{
					ExTraceGlobals.calendarTracer = new Trace(ExTraceGlobals.componentGuid, 10);
				}
				return ExTraceGlobals.calendarTracer;
			}
		}

		public static Trace EmailTracer
		{
			get
			{
				if (ExTraceGlobals.emailTracer == null)
				{
					ExTraceGlobals.emailTracer = new Trace(ExTraceGlobals.componentGuid, 11);
				}
				return ExTraceGlobals.emailTracer;
			}
		}

		public static Trace XsoTracer
		{
			get
			{
				if (ExTraceGlobals.xsoTracer == null)
				{
					ExTraceGlobals.xsoTracer = new Trace(ExTraceGlobals.componentGuid, 12);
				}
				return ExTraceGlobals.xsoTracer;
			}
		}

		public static Trace FaxTracer
		{
			get
			{
				if (ExTraceGlobals.faxTracer == null)
				{
					ExTraceGlobals.faxTracer = new Trace(ExTraceGlobals.componentGuid, 13);
				}
				return ExTraceGlobals.faxTracer;
			}
		}

		public static Trace AutoAttendantTracer
		{
			get
			{
				if (ExTraceGlobals.autoAttendantTracer == null)
				{
					ExTraceGlobals.autoAttendantTracer = new Trace(ExTraceGlobals.componentGuid, 14);
				}
				return ExTraceGlobals.autoAttendantTracer;
			}
		}

		public static Trace DirectorySearchTracer
		{
			get
			{
				if (ExTraceGlobals.directorySearchTracer == null)
				{
					ExTraceGlobals.directorySearchTracer = new Trace(ExTraceGlobals.componentGuid, 15);
				}
				return ExTraceGlobals.directorySearchTracer;
			}
		}

		public static Trace UtilTracer
		{
			get
			{
				if (ExTraceGlobals.utilTracer == null)
				{
					ExTraceGlobals.utilTracer = new Trace(ExTraceGlobals.componentGuid, 16);
				}
				return ExTraceGlobals.utilTracer;
			}
		}

		public static Trace ClientAccessTracer
		{
			get
			{
				if (ExTraceGlobals.clientAccessTracer == null)
				{
					ExTraceGlobals.clientAccessTracer = new Trace(ExTraceGlobals.componentGuid, 17);
				}
				return ExTraceGlobals.clientAccessTracer;
			}
		}

		public static Trace DiagnosticTracer
		{
			get
			{
				if (ExTraceGlobals.diagnosticTracer == null)
				{
					ExTraceGlobals.diagnosticTracer = new Trace(ExTraceGlobals.componentGuid, 18);
				}
				return ExTraceGlobals.diagnosticTracer;
			}
		}

		public static Trace OutdialingTracer
		{
			get
			{
				if (ExTraceGlobals.outdialingTracer == null)
				{
					ExTraceGlobals.outdialingTracer = new Trace(ExTraceGlobals.componentGuid, 19);
				}
				return ExTraceGlobals.outdialingTracer;
			}
		}

		public static Trace SpeechAutoAttendantTracer
		{
			get
			{
				if (ExTraceGlobals.speechAutoAttendantTracer == null)
				{
					ExTraceGlobals.speechAutoAttendantTracer = new Trace(ExTraceGlobals.componentGuid, 20);
				}
				return ExTraceGlobals.speechAutoAttendantTracer;
			}
		}

		public static Trace AsrContactsTracer
		{
			get
			{
				if (ExTraceGlobals.asrContactsTracer == null)
				{
					ExTraceGlobals.asrContactsTracer = new Trace(ExTraceGlobals.componentGuid, 21);
				}
				return ExTraceGlobals.asrContactsTracer;
			}
		}

		public static Trace AsrSearchTracer
		{
			get
			{
				if (ExTraceGlobals.asrSearchTracer == null)
				{
					ExTraceGlobals.asrSearchTracer = new Trace(ExTraceGlobals.componentGuid, 22);
				}
				return ExTraceGlobals.asrSearchTracer;
			}
		}

		public static Trace PromptProvisioningTracer
		{
			get
			{
				if (ExTraceGlobals.promptProvisioningTracer == null)
				{
					ExTraceGlobals.promptProvisioningTracer = new Trace(ExTraceGlobals.componentGuid, 23);
				}
				return ExTraceGlobals.promptProvisioningTracer;
			}
		}

		public static Trace PFDUMCallAcceptanceTracer
		{
			get
			{
				if (ExTraceGlobals.pFDUMCallAcceptanceTracer == null)
				{
					ExTraceGlobals.pFDUMCallAcceptanceTracer = new Trace(ExTraceGlobals.componentGuid, 24);
				}
				return ExTraceGlobals.pFDUMCallAcceptanceTracer;
			}
		}

		public static Trace UMCertificateTracer
		{
			get
			{
				if (ExTraceGlobals.uMCertificateTracer == null)
				{
					ExTraceGlobals.uMCertificateTracer = new Trace(ExTraceGlobals.componentGuid, 25);
				}
				return ExTraceGlobals.uMCertificateTracer;
			}
		}

		public static Trace OCSNotifEventsTracer
		{
			get
			{
				if (ExTraceGlobals.oCSNotifEventsTracer == null)
				{
					ExTraceGlobals.oCSNotifEventsTracer = new Trace(ExTraceGlobals.componentGuid, 26);
				}
				return ExTraceGlobals.oCSNotifEventsTracer;
			}
		}

		public static Trace PersonalAutoAttendantTracer
		{
			get
			{
				if (ExTraceGlobals.personalAutoAttendantTracer == null)
				{
					ExTraceGlobals.personalAutoAttendantTracer = new Trace(ExTraceGlobals.componentGuid, 27);
				}
				return ExTraceGlobals.personalAutoAttendantTracer;
			}
		}

		public static Trace FindMeTracer
		{
			get
			{
				if (ExTraceGlobals.findMeTracer == null)
				{
					ExTraceGlobals.findMeTracer = new Trace(ExTraceGlobals.componentGuid, 28);
				}
				return ExTraceGlobals.findMeTracer;
			}
		}

		public static Trace MWITracer
		{
			get
			{
				if (ExTraceGlobals.mWITracer == null)
				{
					ExTraceGlobals.mWITracer = new Trace(ExTraceGlobals.componentGuid, 29);
				}
				return ExTraceGlobals.mWITracer;
			}
		}

		public static Trace UMPartnerMessageTracer
		{
			get
			{
				if (ExTraceGlobals.uMPartnerMessageTracer == null)
				{
					ExTraceGlobals.uMPartnerMessageTracer = new Trace(ExTraceGlobals.componentGuid, 30);
				}
				return ExTraceGlobals.uMPartnerMessageTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 31);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		public static Trace SipPeerManagerTracer
		{
			get
			{
				if (ExTraceGlobals.sipPeerManagerTracer == null)
				{
					ExTraceGlobals.sipPeerManagerTracer = new Trace(ExTraceGlobals.componentGuid, 32);
				}
				return ExTraceGlobals.sipPeerManagerTracer;
			}
		}

		public static Trace RpcTracer
		{
			get
			{
				if (ExTraceGlobals.rpcTracer == null)
				{
					ExTraceGlobals.rpcTracer = new Trace(ExTraceGlobals.componentGuid, 33);
				}
				return ExTraceGlobals.rpcTracer;
			}
		}

		public static Trace UCMATracer
		{
			get
			{
				if (ExTraceGlobals.uCMATracer == null)
				{
					ExTraceGlobals.uCMATracer = new Trace(ExTraceGlobals.componentGuid, 34);
				}
				return ExTraceGlobals.uCMATracer;
			}
		}

		public static Trace UMReportsTracer
		{
			get
			{
				if (ExTraceGlobals.uMReportsTracer == null)
				{
					ExTraceGlobals.uMReportsTracer = new Trace(ExTraceGlobals.componentGuid, 35);
				}
				return ExTraceGlobals.uMReportsTracer;
			}
		}

		public static Trace MobileSpeechRecoTracer
		{
			get
			{
				if (ExTraceGlobals.mobileSpeechRecoTracer == null)
				{
					ExTraceGlobals.mobileSpeechRecoTracer = new Trace(ExTraceGlobals.componentGuid, 36);
				}
				return ExTraceGlobals.mobileSpeechRecoTracer;
			}
		}

		public static Trace UMGrammarGeneratorTracer
		{
			get
			{
				if (ExTraceGlobals.uMGrammarGeneratorTracer == null)
				{
					ExTraceGlobals.uMGrammarGeneratorTracer = new Trace(ExTraceGlobals.componentGuid, 37);
				}
				return ExTraceGlobals.uMGrammarGeneratorTracer;
			}
		}

		public static Trace UMCallRouterTracer
		{
			get
			{
				if (ExTraceGlobals.uMCallRouterTracer == null)
				{
					ExTraceGlobals.uMCallRouterTracer = new Trace(ExTraceGlobals.componentGuid, 38);
				}
				return ExTraceGlobals.uMCallRouterTracer;
			}
		}

		private static Guid componentGuid = new Guid("321b4079-df13-45c3-bbc9-2c610013dff4");

		private static Trace serviceStartTracer = null;

		private static Trace serviceTracer = null;

		private static Trace serviceStopTracer = null;

		private static Trace voipPlatformTracer = null;

		private static Trace callSessionTracer = null;

		private static Trace stackIfTracer = null;

		private static Trace stateMachineTracer = null;

		private static Trace greetingsTracer = null;

		private static Trace authenticationTracer = null;

		private static Trace voiceMailTracer = null;

		private static Trace calendarTracer = null;

		private static Trace emailTracer = null;

		private static Trace xsoTracer = null;

		private static Trace faxTracer = null;

		private static Trace autoAttendantTracer = null;

		private static Trace directorySearchTracer = null;

		private static Trace utilTracer = null;

		private static Trace clientAccessTracer = null;

		private static Trace diagnosticTracer = null;

		private static Trace outdialingTracer = null;

		private static Trace speechAutoAttendantTracer = null;

		private static Trace asrContactsTracer = null;

		private static Trace asrSearchTracer = null;

		private static Trace promptProvisioningTracer = null;

		private static Trace pFDUMCallAcceptanceTracer = null;

		private static Trace uMCertificateTracer = null;

		private static Trace oCSNotifEventsTracer = null;

		private static Trace personalAutoAttendantTracer = null;

		private static Trace findMeTracer = null;

		private static Trace mWITracer = null;

		private static Trace uMPartnerMessageTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace sipPeerManagerTracer = null;

		private static Trace rpcTracer = null;

		private static Trace uCMATracer = null;

		private static Trace uMReportsTracer = null;

		private static Trace mobileSpeechRecoTracer = null;

		private static Trace uMGrammarGeneratorTracer = null;

		private static Trace uMCallRouterTracer = null;
	}
}
