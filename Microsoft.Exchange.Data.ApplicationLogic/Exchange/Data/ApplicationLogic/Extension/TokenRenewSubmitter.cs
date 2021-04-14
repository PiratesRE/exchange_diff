using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	internal class TokenRenewSubmitter
	{
		internal int GetTokensCount { get; set; }

		internal int QueryQueueCount
		{
			get
			{
				return this.queryQueue.Count;
			}
		}

		internal TokenRenewSubmitter(OmexWebServiceUrlsCache urlsCache)
		{
			if (urlsCache == null)
			{
				throw new ArgumentNullException("urlsCache");
			}
			this.urlsCache = urlsCache;
		}

		internal void SubmitRenewQuery(ICollection<ExtensionData> extensions, TokenRenewQueryContext queryContext)
		{
			if (extensions == null)
			{
				throw new ArgumentNullException("extensions");
			}
			if (extensions.Count == 0)
			{
				throw new ArgumentException("extensions must contain one or more extensions");
			}
			List<TokenRenewRequestAsset> list = new List<TokenRenewRequestAsset>(extensions.Count);
			foreach (ExtensionData extensionData in extensions)
			{
				list.Add(new TokenRenewRequestAsset
				{
					MarketplaceContentMarket = extensionData.MarketplaceContentMarket,
					ExtensionID = extensionData.ExtensionId,
					MarketplaceAssetID = extensionData.MarketplaceAssetID,
					Scope = extensionData.Scope.Value,
					Etoken = extensionData.Etoken
				});
			}
			if (list.Count == 0)
			{
				TokenRenewSubmitter.Tracer.TraceDebug(0L, "ExtensionsCache.SubmitRenewQuery: TokenRenewRequestAssets count is 0. Token renew query will not be started.");
				return;
			}
			queryContext.TokenRenewRequestAssets = list;
			queryContext.DeploymentId = ExtensionDataHelper.GetDeploymentId(queryContext.Domain);
			this.QueueQueryItem(queryContext);
		}

		internal void QueueQueryItem(TokenRenewQueryContext queryContext)
		{
			GetTokens getTokens = null;
			lock (this.queryQueueLockObject)
			{
				if (this.queryQueue.Count > 500)
				{
					TokenRenewSubmitter.Tracer.TraceError<IExchangePrincipal>(0L, "Query for {0} not added to the query queue because queue is full.", queryContext.ExchangePrincipal);
					ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_ExtensionTokenQueryMaxExceeded, null, new object[]
					{
						"ProcessTokenRenew",
						ExtensionDiagnostics.GetLoggedMailboxIdentifier(queryContext.ExchangePrincipal)
					});
					return;
				}
				TokenRenewSubmitter.Tracer.TraceDebug<IExchangePrincipal>(0L, "Adding query for {0} to the query queue.", queryContext.ExchangePrincipal);
				this.queryQueue.Enqueue(queryContext);
				if (this.GetTokensCount < 50)
				{
					getTokens = new GetTokens(this.urlsCache, this);
					this.GetTokensCount++;
					TokenRenewSubmitter.Tracer.TraceDebug<int>(0L, "Creating a new instance of GetTokens. GetTokens Count {0}", this.GetTokensCount);
				}
				else
				{
					TokenRenewSubmitter.Tracer.TraceDebug<int>(0L, "Too many GetTokens commands. Query will be handled from pool. GetTokens Count {0}", this.GetTokensCount);
				}
			}
			if (getTokens != null)
			{
				this.ExecuteTokenRenewQuery(getTokens);
			}
		}

		internal void ExecuteTokenRenewQuery(GetTokens getTokens)
		{
			TokenRenewQueryContext tokenRenewQueryContext = null;
			lock (this.queryQueueLockObject)
			{
				if (this.queryQueue.Count > 0)
				{
					tokenRenewQueryContext = this.queryQueue.Dequeue();
				}
				else
				{
					this.GetTokensCount--;
					if (this.GetTokensCount < 0)
					{
						throw new InvalidOperationException("GetTokensCount can't be less than 0.");
					}
					TokenRenewSubmitter.Tracer.TraceDebug<int>(0L, "Query queue is empty. GetTokens Count {0}", this.GetTokensCount);
				}
			}
			if (tokenRenewQueryContext != null)
			{
				TokenRenewSubmitter.Tracer.TraceDebug<IExchangePrincipal>(0L, "Starting query for {0}.", tokenRenewQueryContext.ExchangePrincipal);
				getTokens.Execute(tokenRenewQueryContext);
			}
		}

		private const string ScenarioProcessTokenRenew = "ProcessTokenRenew";

		private const int QueryQueueMaxCount = 500;

		internal const int MaxGetTokensCount = 50;

		private static readonly Trace Tracer = ExTraceGlobals.ExtensionTracer;

		private OmexWebServiceUrlsCache urlsCache;

		private object queryQueueLockObject = new object();

		private Queue<TokenRenewQueryContext> queryQueue = new Queue<TokenRenewQueryContext>(500);
	}
}
