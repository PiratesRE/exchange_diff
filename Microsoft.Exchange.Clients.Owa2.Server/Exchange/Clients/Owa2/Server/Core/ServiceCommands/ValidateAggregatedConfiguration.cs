using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.ServiceCommands
{
	internal class ValidateAggregatedConfiguration : ServiceCommand<ValidateAggregatedConfigurationResponse>
	{
		public ValidateAggregatedConfiguration(CallContext callContext) : base(callContext)
		{
		}

		internal static object RemoveFromValidationCache(MailboxSession session)
		{
			return HttpRuntime.Cache.Remove(ValidateAggregatedConfiguration.CacheKeyFromSession(session));
		}

		internal static void AddToValidationCache(MailboxSession session, object objectToAdd)
		{
			HttpRuntime.Cache.Insert(ValidateAggregatedConfiguration.CacheKeyFromSession(session), objectToAdd, null, DateTime.UtcNow.Add(TimeSpan.FromMinutes(5.0)), Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, new CacheItemRemovedCallback(ValidateAggregatedConfiguration.AggregationContextRemoved));
		}

		protected override ValidateAggregatedConfigurationResponse InternalExecute()
		{
			ValidateAggregatedConfigurationResponse report = new ValidateAggregatedConfigurationResponse();
			bool flag = false;
			object obj = ValidateAggregatedConfiguration.RemoveFromValidationCache(base.MailboxIdentityMailboxSession);
			bool flag2 = obj is bool && (bool)obj;
			UserConfigurationManager.IAggregationContext aggregationContext = obj as UserConfigurationManager.IAggregationContext;
			try
			{
				if (flag2)
				{
					flag = true;
					ExTraceGlobals.SessionDataHandlerTracer.TraceDebug((long)this.GetHashCode(), "ValidateAggregatedConfiguration was invoked and the aggregator was recently validated.");
				}
				else if (aggregationContext != null)
				{
					aggregationContext.Validate(base.MailboxIdentityMailboxSession, delegate(IEnumerable<UserConfigurationDescriptor.MementoClass> faisRebuilt, IEnumerable<string> typesRebuilt)
					{
						foreach (UserConfigurationDescriptor.MementoClass mementoClass in faisRebuilt)
						{
							report.FaiUpdates.Add(mementoClass.ConfigurationName);
							OwaSingleCounters.AggregatedUserConfigurationPartsRebuilt.Increment();
							ExTraceGlobals.SessionDataHandlerTracer.TraceError<string>((long)this.GetHashCode(), "error found in configuration type {0}", mementoClass.ConfigurationName);
						}
						foreach (string item in typesRebuilt)
						{
							report.TypeUpdates.Add(item);
						}
					});
					flag = true;
				}
				if (flag)
				{
					ValidateAggregatedConfiguration.AddToValidationCache(base.MailboxIdentityMailboxSession, true);
					report.IsValidated = true;
				}
			}
			finally
			{
				DisposeGuard.DisposeIfPresent(aggregationContext);
			}
			return report;
		}

		private static void AggregationContextRemoved(string key, object value, CacheItemRemovedReason reason)
		{
			if (reason == CacheItemRemovedReason.Expired)
			{
				ExTraceGlobals.CoreTracer.TraceWarning(0L, "an aggregation context expired before being revalidated!");
				DisposeGuard.DisposeIfPresent(value as IDisposable);
			}
		}

		private static string CacheKeyFromSession(MailboxSession session)
		{
			string arg = session.UserLegacyDN ?? string.Empty;
			string arg2 = session.MailboxGuid.ToString();
			return string.Format("{0}_{1}_{2}", arg, arg2, "Owa.Aggregated");
		}
	}
}
