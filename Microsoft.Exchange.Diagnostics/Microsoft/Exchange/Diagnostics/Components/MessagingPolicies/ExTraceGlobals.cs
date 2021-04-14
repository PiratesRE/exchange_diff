using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.MessagingPolicies
{
	public static class ExTraceGlobals
	{
		public static Trace TransportRulesEngineTracer
		{
			get
			{
				if (ExTraceGlobals.transportRulesEngineTracer == null)
				{
					ExTraceGlobals.transportRulesEngineTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.transportRulesEngineTracer;
			}
		}

		public static Trace CodeGeneratorTracer
		{
			get
			{
				if (ExTraceGlobals.codeGeneratorTracer == null)
				{
					ExTraceGlobals.codeGeneratorTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.codeGeneratorTracer;
			}
		}

		public static Trace JournalingTracer
		{
			get
			{
				if (ExTraceGlobals.journalingTracer == null)
				{
					ExTraceGlobals.journalingTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.journalingTracer;
			}
		}

		public static Trace AttachmentFilteringTracer
		{
			get
			{
				if (ExTraceGlobals.attachmentFilteringTracer == null)
				{
					ExTraceGlobals.attachmentFilteringTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.attachmentFilteringTracer;
			}
		}

		public static Trace AddressRewritingTracer
		{
			get
			{
				if (ExTraceGlobals.addressRewritingTracer == null)
				{
					ExTraceGlobals.addressRewritingTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.addressRewritingTracer;
			}
		}

		public static Trace RmSvcAgentTracer
		{
			get
			{
				if (ExTraceGlobals.rmSvcAgentTracer == null)
				{
					ExTraceGlobals.rmSvcAgentTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.rmSvcAgentTracer;
			}
		}

		public static Trace RulesEngineTracer
		{
			get
			{
				if (ExTraceGlobals.rulesEngineTracer == null)
				{
					ExTraceGlobals.rulesEngineTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.rulesEngineTracer;
			}
		}

		public static Trace ReconciliationTracer
		{
			get
			{
				if (ExTraceGlobals.reconciliationTracer == null)
				{
					ExTraceGlobals.reconciliationTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.reconciliationTracer;
			}
		}

		public static Trace CrossPremiseTracer
		{
			get
			{
				if (ExTraceGlobals.crossPremiseTracer == null)
				{
					ExTraceGlobals.crossPremiseTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.crossPremiseTracer;
			}
		}

		public static Trace OpenDomainRoutingAgentTracer
		{
			get
			{
				if (ExTraceGlobals.openDomainRoutingAgentTracer == null)
				{
					ExTraceGlobals.openDomainRoutingAgentTracer = new Trace(ExTraceGlobals.componentGuid, 9);
				}
				return ExTraceGlobals.openDomainRoutingAgentTracer;
			}
		}

		public static Trace JrdAgentTracer
		{
			get
			{
				if (ExTraceGlobals.jrdAgentTracer == null)
				{
					ExTraceGlobals.jrdAgentTracer = new Trace(ExTraceGlobals.componentGuid, 10);
				}
				return ExTraceGlobals.jrdAgentTracer;
			}
		}

		public static Trace PreLicensingAgentTracer
		{
			get
			{
				if (ExTraceGlobals.preLicensingAgentTracer == null)
				{
					ExTraceGlobals.preLicensingAgentTracer = new Trace(ExTraceGlobals.componentGuid, 11);
				}
				return ExTraceGlobals.preLicensingAgentTracer;
			}
		}

		public static Trace OutlookProtectionRulesTracer
		{
			get
			{
				if (ExTraceGlobals.outlookProtectionRulesTracer == null)
				{
					ExTraceGlobals.outlookProtectionRulesTracer = new Trace(ExTraceGlobals.componentGuid, 12);
				}
				return ExTraceGlobals.outlookProtectionRulesTracer;
			}
		}

		public static Trace EhfOutboundRoutingAgentTracer
		{
			get
			{
				if (ExTraceGlobals.ehfOutboundRoutingAgentTracer == null)
				{
					ExTraceGlobals.ehfOutboundRoutingAgentTracer = new Trace(ExTraceGlobals.componentGuid, 13);
				}
				return ExTraceGlobals.ehfOutboundRoutingAgentTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 14);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		public static Trace RedirectionAgentTracer
		{
			get
			{
				if (ExTraceGlobals.redirectionAgentTracer == null)
				{
					ExTraceGlobals.redirectionAgentTracer = new Trace(ExTraceGlobals.componentGuid, 15);
				}
				return ExTraceGlobals.redirectionAgentTracer;
			}
		}

		public static Trace InboundTrustAgentTracer
		{
			get
			{
				if (ExTraceGlobals.inboundTrustAgentTracer == null)
				{
					ExTraceGlobals.inboundTrustAgentTracer = new Trace(ExTraceGlobals.componentGuid, 16);
				}
				return ExTraceGlobals.inboundTrustAgentTracer;
			}
		}

		public static Trace OutboundTrustAgentTracer
		{
			get
			{
				if (ExTraceGlobals.outboundTrustAgentTracer == null)
				{
					ExTraceGlobals.outboundTrustAgentTracer = new Trace(ExTraceGlobals.componentGuid, 17);
				}
				return ExTraceGlobals.outboundTrustAgentTracer;
			}
		}

		public static Trace CentralizedMailFlowAgentTracer
		{
			get
			{
				if (ExTraceGlobals.centralizedMailFlowAgentTracer == null)
				{
					ExTraceGlobals.centralizedMailFlowAgentTracer = new Trace(ExTraceGlobals.componentGuid, 18);
				}
				return ExTraceGlobals.centralizedMailFlowAgentTracer;
			}
		}

		public static Trace InterceptorAgentTracer
		{
			get
			{
				if (ExTraceGlobals.interceptorAgentTracer == null)
				{
					ExTraceGlobals.interceptorAgentTracer = new Trace(ExTraceGlobals.componentGuid, 19);
				}
				return ExTraceGlobals.interceptorAgentTracer;
			}
		}

		public static Trace OutlookPolicyNudgeRulesTracer
		{
			get
			{
				if (ExTraceGlobals.outlookPolicyNudgeRulesTracer == null)
				{
					ExTraceGlobals.outlookPolicyNudgeRulesTracer = new Trace(ExTraceGlobals.componentGuid, 20);
				}
				return ExTraceGlobals.outlookPolicyNudgeRulesTracer;
			}
		}

		public static Trace ClassificationDefinitionsTracer
		{
			get
			{
				if (ExTraceGlobals.classificationDefinitionsTracer == null)
				{
					ExTraceGlobals.classificationDefinitionsTracer = new Trace(ExTraceGlobals.componentGuid, 21);
				}
				return ExTraceGlobals.classificationDefinitionsTracer;
			}
		}

		public static Trace FfoFrontendProxyAgentTracer
		{
			get
			{
				if (ExTraceGlobals.ffoFrontendProxyAgentTracer == null)
				{
					ExTraceGlobals.ffoFrontendProxyAgentTracer = new Trace(ExTraceGlobals.componentGuid, 22);
				}
				return ExTraceGlobals.ffoFrontendProxyAgentTracer;
			}
		}

		public static Trace FrontendProxyAgentTracer
		{
			get
			{
				if (ExTraceGlobals.frontendProxyAgentTracer == null)
				{
					ExTraceGlobals.frontendProxyAgentTracer = new Trace(ExTraceGlobals.componentGuid, 23);
				}
				return ExTraceGlobals.frontendProxyAgentTracer;
			}
		}

		public static Trace AddressBookPolicyRoutingAgentTracer
		{
			get
			{
				if (ExTraceGlobals.addressBookPolicyRoutingAgentTracer == null)
				{
					ExTraceGlobals.addressBookPolicyRoutingAgentTracer = new Trace(ExTraceGlobals.componentGuid, 24);
				}
				return ExTraceGlobals.addressBookPolicyRoutingAgentTracer;
			}
		}

		public static Trace OwaRulesEngineTracer
		{
			get
			{
				if (ExTraceGlobals.owaRulesEngineTracer == null)
				{
					ExTraceGlobals.owaRulesEngineTracer = new Trace(ExTraceGlobals.componentGuid, 25);
				}
				return ExTraceGlobals.owaRulesEngineTracer;
			}
		}

		private static Guid componentGuid = new Guid("D368698B-B84F-402e-A300-FA98CC39020F");

		private static Trace transportRulesEngineTracer = null;

		private static Trace codeGeneratorTracer = null;

		private static Trace journalingTracer = null;

		private static Trace attachmentFilteringTracer = null;

		private static Trace addressRewritingTracer = null;

		private static Trace rmSvcAgentTracer = null;

		private static Trace rulesEngineTracer = null;

		private static Trace reconciliationTracer = null;

		private static Trace crossPremiseTracer = null;

		private static Trace openDomainRoutingAgentTracer = null;

		private static Trace jrdAgentTracer = null;

		private static Trace preLicensingAgentTracer = null;

		private static Trace outlookProtectionRulesTracer = null;

		private static Trace ehfOutboundRoutingAgentTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace redirectionAgentTracer = null;

		private static Trace inboundTrustAgentTracer = null;

		private static Trace outboundTrustAgentTracer = null;

		private static Trace centralizedMailFlowAgentTracer = null;

		private static Trace interceptorAgentTracer = null;

		private static Trace outlookPolicyNudgeRulesTracer = null;

		private static Trace classificationDefinitionsTracer = null;

		private static Trace ffoFrontendProxyAgentTracer = null;

		private static Trace frontendProxyAgentTracer = null;

		private static Trace addressBookPolicyRoutingAgentTracer = null;

		private static Trace owaRulesEngineTracer = null;
	}
}
