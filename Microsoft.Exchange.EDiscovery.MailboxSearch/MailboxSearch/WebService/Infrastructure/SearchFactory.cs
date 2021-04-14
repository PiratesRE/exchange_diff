using System;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.External;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model;
using Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Infrastructure
{
	internal class SearchFactory
	{
		public static SearchFactory Current
		{
			get
			{
				return SearchFactory.current;
			}
			protected set
			{
				SearchFactory.current = value;
			}
		}

		public virtual ISearchPolicy GetSearchPolicy(IRecipientSession recipientSession, CallerInfo callerInfo, ExchangeRunspaceConfiguration runspaceConfiguration, IBudget budget = null)
		{
			Recorder.Trace(2L, TraceType.InfoTrace, "SearchFactory.GetSearchPolicy");
			return new SearchPolicy(recipientSession, callerInfo, runspaceConfiguration, budget);
		}

		public virtual IThrottlingPolicy GetThrottlingPolicy(ISearchPolicy policy)
		{
			Recorder.Trace(2L, TraceType.InfoTrace, "SearchFactory.GetThrottlingPolicy");
			DiscoveryTenantBudgetKey discoveryTenantBudgetKey = new DiscoveryTenantBudgetKey(policy.RecipientSession.SessionSettings.CurrentOrganizationId, BudgetType.PowerShell);
			return discoveryTenantBudgetKey.Lookup();
		}

		public virtual VariantConfigurationSnapshot GetVariantConfigurationSnapshot(ISearchPolicy policy)
		{
			Recorder.Trace(2L, TraceType.InfoTrace, "SearchFactory.GetVariantConfigurationSnapshot");
			VariantConfigurationSnapshot variantConfigurationSnapshot = null;
			if (policy.RunspaceConfiguration != null && policy.RunspaceConfiguration.ExecutingUser != null)
			{
				Recorder.Trace(2L, TraceType.InfoTrace, "SearchFactory.GetVariantConfigurationSnapshot Loading User Snapshpt");
				ADUser user = new ADUser(policy.RecipientSession, policy.RunspaceConfiguration.ExecutingUser.propertyBag);
				variantConfigurationSnapshot = VariantConfiguration.GetSnapshot(user.GetContext(null), null, null);
			}
			if (variantConfigurationSnapshot == null)
			{
				Recorder.Trace(2L, TraceType.InfoTrace, "SearchFactory.GetVariantConfigurationSnapshot User Snapshot Failed, Loading Global Snapshot");
				variantConfigurationSnapshot = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null);
			}
			return variantConfigurationSnapshot;
		}

		public virtual IDirectoryProvider GetDirectoryProvider(ISearchPolicy policy)
		{
			Recorder.Trace(2L, TraceType.InfoTrace, "SearchFactory.GetDirectoryProvider");
			return new ActiveDirectoryProvider();
		}

		public virtual IServerProvider GetServerProvider(ISearchPolicy policy)
		{
			Recorder.Trace(2L, TraceType.InfoTrace, "SearchFactory.GetServerProvider");
			return new AutoDiscoveryServerProvider();
		}

		public virtual ISearchConfigurationProvider GetSearchConfigurationProvider(ISearchPolicy policy)
		{
			Recorder.Trace(2L, TraceType.InfoTrace, "SearchFactory.GetSearchConfigurationProvider");
			return new ArbitrationSearchConfigurationProvider();
		}

		public virtual IExchangeProxy GetProxy(ISearchPolicy policy, FanoutParameters parameter)
		{
			Recorder.Trace(2L, TraceType.InfoTrace, "SearchFactory.GetProxy");
			return new ExchangeProxy(policy, parameter);
		}

		public virtual ISourceConverter GetSourceConverter(ISearchPolicy policy, SourceType sourceFrom)
		{
			Recorder.Trace(2L, TraceType.InfoTrace, "SearchFactory.GetSourceConverter SourceType:", sourceFrom);
			if (sourceFrom == SourceType.PublicFolder || sourceFrom == SourceType.AllPublicFolders)
			{
				return new PublicFolderSourceConverter();
			}
			Recorder.Trace(2L, TraceType.InfoTrace, "SearchFactory.GetSourceConverter No Converter SourceType:", sourceFrom);
			return null;
		}

		public virtual ISearchResultProvider GetSearchResultProvider(ISearchPolicy policy, SearchType searchType)
		{
			Recorder.Trace(2L, TraceType.InfoTrace, "SearchFactory.GetSearchResultProvider");
			if (searchType == SearchType.NonIndexedItemPreview || searchType == SearchType.NonIndexedItemStatistics)
			{
				return new LocalNonIndexedResultProvider();
			}
			if (policy.ExecutionSettings.DiscoveryUseFastSearch && searchType == SearchType.Preview)
			{
				return new FastLocalSearchResultsProvider();
			}
			return new LocalSearchResultsProvider();
		}

		public virtual IConfigurationSession GetConfigurationSession(ISearchPolicy policy)
		{
			Recorder.Trace(2L, TraceType.InfoTrace, "SearchFactory.GetConfigurationSession");
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, policy.RecipientSession.SessionSettings, 222, "GetConfigurationSession", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\MailboxSearch\\WebService\\Infrastructure\\SearchFactory.cs");
		}

		private static SearchFactory current = new SearchFactory();
	}
}
