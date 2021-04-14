using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Ceres.InteractionEngine.Services.Exchange;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Diagnostics;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol.EDiscovery;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Data.Search.KqlParser;
using Microsoft.Exchange.Search.Fast;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Compliance.TaskPlugins.EDiscovery
{
	internal class FastLocalSearchResultsProvider : ISearchResultsProvider
	{
		private static SearchConfig SearchConfig
		{
			get
			{
				return SearchConfig.Instance;
			}
		}

		private static PagingImsFlowExecutor FlowExecutor
		{
			get
			{
				if (FastLocalSearchResultsProvider.flowExecutor == null)
				{
					string hostName = FastLocalSearchResultsProvider.SearchConfig.HostName;
					int queryServicePort = FastLocalSearchResultsProvider.SearchConfig.QueryServicePort;
					int fastMmsImsChannelPoolSize = FastLocalSearchResultsProvider.SearchConfig.FastMmsImsChannelPoolSize;
					TimeSpan fastImsMmsSendTimeout = FastLocalSearchResultsProvider.SearchConfig.FastImsMmsSendTimeout;
					TimeSpan fastImsMmsReceiveTimeout = FastLocalSearchResultsProvider.SearchConfig.FastImsMmsReceiveTimeout;
					int fastMmsImsRetryCount = FastLocalSearchResultsProvider.SearchConfig.FastMmsImsRetryCount;
					long num = (long)FastLocalSearchResultsProvider.SearchConfig.FastIMSMaxReceivedMessageSize;
					int fastIMSMaxStringContentLength = FastLocalSearchResultsProvider.SearchConfig.FastIMSMaxStringContentLength;
					TimeSpan fastProxyCacheTimeout = FastLocalSearchResultsProvider.SearchConfig.FastProxyCacheTimeout;
					FastLocalSearchResultsProvider.flowExecutor = new PagingImsFlowExecutor(hostName, queryServicePort, fastMmsImsChannelPoolSize, fastImsMmsSendTimeout, fastImsMmsSendTimeout, fastImsMmsReceiveTimeout, fastProxyCacheTimeout, num, fastIMSMaxStringContentLength, fastMmsImsRetryCount);
				}
				return FastLocalSearchResultsProvider.flowExecutor;
			}
		}

		public SearchWorkDefinition ParseSearch(ComplianceMessage target, SearchWorkDefinition definition)
		{
			QueryFilter queryFilter = null;
			string query = definition.Query;
			CultureInfo culture;
			FaultDefinition faultDefinition;
			ExceptionHandler.Parser.TryRun(delegate
			{
				if (string.IsNullOrEmpty(target.Culture))
				{
					culture = CultureInfo.InvariantCulture;
				}
				else
				{
					culture = CultureInfo.GetCultureInfo(target.Culture);
				}
				Func<IRecipientResolver, IPolicyTagProvider, QueryFilter> func = null;
				switch (definition.Parser)
				{
				case SearchWorkDefinition.QueryParser.KQL:
					func = ((IRecipientResolver r, IPolicyTagProvider p) => KqlParser.ParseAndBuildQuery(query, KqlParser.ParseOption.EDiscoveryMode, culture, r, p));
					break;
				case SearchWorkDefinition.QueryParser.AQS:
					func = delegate(IRecipientResolver r, IPolicyTagProvider p)
					{
						AqsParser.ParseOption parseOption = AqsParser.ParseOption.UseCiKeywordOnly | AqsParser.ParseOption.DisablePrefixMatch | AqsParser.ParseOption.AllowShortWildcards;
						return AqsParser.ParseAndBuildQuery(query, parseOption, culture, r, p);
					};
					break;
				}
				if (func != null)
				{
					OrganizationId scopingOrganizationId = null;
					IPolicyTagProvider arg = null;
					IRecipientResolver arg2 = null;
					if (OrganizationId.TryCreateFromBytes(target.TenantId, Encoding.UTF8, out scopingOrganizationId))
					{
						ADSessionSettings adsessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(scopingOrganizationId);
						adsessionSettings.IncludeInactiveMailbox = true;
						IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.PartiallyConsistent, adsessionSettings, 146, "ParseSearch", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\ApplicationPlugins\\EDiscovery\\FastLocalSearchResultsProvider.cs");
						arg2 = new FastLocalSearchResultsProvider.RecipientIdentityResolver(tenantOrRootOrgRecipientSession);
						IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, adsessionSettings, 149, "ParseSearch", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\ApplicationPlugins\\EDiscovery\\FastLocalSearchResultsProvider.cs");
						arg = new PolicyTagAdProvider(tenantOrTopologyConfigurationSession);
					}
					queryFilter = func(arg2, arg);
				}
				if (queryFilter != null)
				{
					definition.Query = FqlQueryBuilder.ToFqlString(queryFilter, culture);
				}
			}, TimeSpan.FromMinutes(1.0), out faultDefinition, target, null, default(CancellationToken), null, "ParseSearch", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\ApplicationPlugins\\EDiscovery\\FastLocalSearchResultsProvider.cs", 102);
			if (faultDefinition != null)
			{
				ExceptionHandler.FaultMessage(target, faultDefinition, true);
			}
			return definition;
		}

		public SearchResult PerformSearch(ComplianceMessage target, SearchWorkDefinition definition)
		{
			string query = definition.Query;
			SearchResult result = new SearchResult();
			CultureInfo culture;
			FaultDefinition faultDefinition;
			ExceptionHandler.Parser.TryRun(delegate
			{
				if (string.IsNullOrEmpty(target.Culture))
				{
					culture = CultureInfo.InvariantCulture;
				}
				else
				{
					culture = CultureInfo.GetCultureInfo(target.Culture);
				}
				AdditionalParameters additionalParameters = new AdditionalParameters
				{
					Refiners = new string[]
					{
						FastIndexSystemSchema.Size.Name
					}
				};
				if (query != null)
				{
					Guid mailboxGuid = target.MessageTarget.Mailbox;
					Guid database = target.MessageTarget.Database;
					IEnumerable<KeyValuePair<PagingImsFlowExecutor.QueryExecutionContext, SearchResultItem[]>> pages = null;
					string flowName = FlowDescriptor.GetImsFlowDescriptor(FastLocalSearchResultsProvider.SearchConfig, FastIndexVersion.GetIndexSystemName(database)).DisplayName;
					if (ExceptionHandler.Proxy.TryRun(delegate
					{
						pages = FastLocalSearchResultsProvider.FlowExecutor.Execute(flowName, mailboxGuid, Guid.NewGuid(), query, 0L, culture, additionalParameters, Math.Min(FastLocalSearchResultsProvider.SearchConfig.FastQueryResultTrimHits, 1), null);
					}, TimeSpan.FromMinutes(1.0), out faultDefinition, target, new Action<ExceptionHandler.ExceptionData>(this.ProxyExceptionHandler), default(CancellationToken), null, "PerformSearch", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\ApplicationPlugins\\EDiscovery\\FastLocalSearchResultsProvider.cs", 213))
					{
						foreach (KeyValuePair<PagingImsFlowExecutor.QueryExecutionContext, SearchResultItem[]> keyValuePair in pages)
						{
							PagingImsFlowExecutor.QueryExecutionContext key = keyValuePair.Key;
							SearchResultItem[] value = keyValuePair.Value;
							ByteQuantifiedSize byteQuantifiedSize = default(ByteQuantifiedSize);
							long count = FastLocalSearchResultsProvider.FlowExecutor.ReadHitCount(key);
							IEnumerable<RefinerResult> source = FastLocalSearchResultsProvider.FlowExecutor.ReadRefiners(key);
							RefinerResult refinerResult = source.FirstOrDefault((RefinerResult t) => t.Name == FastIndexSystemSchema.Size.Name);
							if (refinerResult != null)
							{
								byteQuantifiedSize = new ByteQuantifiedSize((ulong)refinerResult.Sum);
							}
							result = new SearchResult();
							result.UpdateTotalSize((long)byteQuantifiedSize.ToMB());
							result.UpdateTotalCount(count);
							result.Results.Add(new SearchResult.TargetSearchResult
							{
								Target = target.MessageTarget,
								Size = result.TotalSize,
								Count = result.TotalCount
							});
						}
					}
				}
			}, TimeSpan.FromMinutes(1.0), out faultDefinition, target, null, default(CancellationToken), null, "PerformSearch", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\ApplicationPlugins\\EDiscovery\\FastLocalSearchResultsProvider.cs", 188);
			if (faultDefinition != null)
			{
				ExceptionHandler.FaultMessage(target, faultDefinition, true);
			}
			return result;
		}

		private void ProxyExceptionHandler(ExceptionHandler.ExceptionData args)
		{
			if (args.ShouldRetry)
			{
				FastLocalSearchResultsProvider.flowExecutor = null;
			}
		}

		private static PagingImsFlowExecutor flowExecutor;

		private class RecipientIdentityResolver : IRecipientResolver
		{
			internal RecipientIdentityResolver(IRecipientSession recipientSession)
			{
				this.recipientSession = recipientSession;
			}

			public string[] Resolve(string identity)
			{
				RecipientIdParameter recipientIdParameter = new RecipientIdParameter(identity);
				IEnumerable<ADRecipient> objects = recipientIdParameter.GetObjects<ADRecipient>(null, this.recipientSession);
				if (objects == null)
				{
					return null;
				}
				List<string> list = new List<string>();
				foreach (ADRecipient adrecipient in objects)
				{
					list.Add(adrecipient.DisplayName);
					list.Add(adrecipient.Alias);
					list.Add(adrecipient.LegacyExchangeDN);
					list.Add(adrecipient.PrimarySmtpAddress.ToString());
				}
				return list.ToArray();
			}

			private IRecipientSession recipientSession;
		}
	}
}
