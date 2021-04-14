using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class DiagnosticMethodDelegateCollection
	{
		private DiagnosticMethodDelegateCollection()
		{
		}

		public static DiagnosticMethodDelegateCollection Singleton
		{
			get
			{
				return DiagnosticMethodDelegateCollection.singleton;
			}
		}

		public DiagnosticMethodDelegate GetDelegate(string verb)
		{
			if (!this.delegateMapping.Member.ContainsKey(verb))
			{
				throw new InvalidRequestException();
			}
			DiagnosticMethodDelegate diagnosticMethodDelegate = this.delegateMapping.Member[verb];
			if (S2SRightsWrapper.AllowsTokenSerializationBy(CallContext.Current.EffectiveCaller.ClientSecurityContext))
			{
				return diagnosticMethodDelegate;
			}
			if (!this.IsMethodUsableInProduction(verb, diagnosticMethodDelegate))
			{
				throw new ServiceAccessDeniedException();
			}
			ExchangeRunspaceConfiguration exchangeRunspaceConfiguration = ExchangeRunspaceConfigurationCache.Singleton.Get(CallContext.Current.HttpContext.User.Identity, null, false);
			if (!exchangeRunspaceConfiguration.HasRoleOfType(RoleType.SupportDiagnostics))
			{
				throw new ServiceAccessDeniedException();
			}
			return diagnosticMethodDelegate;
		}

		private bool IsMethodUsableInProduction(string verb, DiagnosticMethodDelegate diagnosticDelegate)
		{
			bool result;
			lock (this.instanceLock)
			{
				if (this.verbAuthorizedForSupportDiagnosticRoleCache.ContainsKey(verb))
				{
					result = this.verbAuthorizedForSupportDiagnosticRoleCache[verb];
				}
				else
				{
					DiagnosticStrideJustificationAttribute diagnosticStrideJustificationAttribute = Attribute.GetCustomAttribute(diagnosticDelegate.Method, typeof(DiagnosticStrideJustificationAttribute)) as DiagnosticStrideJustificationAttribute;
					bool flag2 = diagnosticStrideJustificationAttribute != null && diagnosticStrideJustificationAttribute.IsAuthorizedForSupportDiagnosticRoleUse;
					this.verbAuthorizedForSupportDiagnosticRoleCache[verb] = flag2;
					result = flag2;
				}
			}
			return result;
		}

		private static DiagnosticMethodDelegateCollection singleton = new DiagnosticMethodDelegateCollection();

		private object instanceLock = new object();

		private LazyMember<Dictionary<string, DiagnosticMethodDelegate>> delegateMapping = new LazyMember<Dictionary<string, DiagnosticMethodDelegate>>(() => new Dictionary<string, DiagnosticMethodDelegate>
		{
			{
				"ClearSubscriptions",
				new DiagnosticMethodDelegate(SubscriptionDiagnosticMethods.ClearSubscriptions)
			},
			{
				"GetActiveSubscriptionIds",
				new DiagnosticMethodDelegate(SubscriptionDiagnosticMethods.GetActiveSubscriptionIds)
			},
			{
				"GetHangingSubscriptionConnections",
				new DiagnosticMethodDelegate(SubscriptionDiagnosticMethods.GetHangingSubscriptionConnections)
			},
			{
				"SetStreamingConnectionHeartbeatDefault",
				new DiagnosticMethodDelegate(SubscriptionDiagnosticMethods.SetStreamingConnectionHeartbeatDefault)
			},
			{
				"SetStreamingSubscriptionNewEventQueueSize",
				new DiagnosticMethodDelegate(SubscriptionDiagnosticMethods.SetStreamingSubscriptionNewEventQueueSize)
			},
			{
				"SetStreamingSubscriptionTimeToLiveDefault",
				new DiagnosticMethodDelegate(SubscriptionDiagnosticMethods.SetStreamingSubscriptionTimeToLiveDefault)
			},
			{
				"GetStreamingSubscriptionExpirationTime",
				new DiagnosticMethodDelegate(SubscriptionDiagnosticMethods.GetStreamingSubscriptionExpirationTime)
			},
			{
				"ClearExchangeRunspaceConfigurationCache",
				new DiagnosticMethodDelegate(CacheDiagnosticMethods.ClearExchangeRunspaceConfigurationCache)
			}
		});

		private Dictionary<string, bool> verbAuthorizedForSupportDiagnosticRoleCache = new Dictionary<string, bool>();
	}
}
