using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	internal sealed class GetTokens
	{
		internal GetTokens(OmexWebServiceUrlsCache urlsCache, TokenRenewSubmitter tokenRenewSubmitter)
		{
			if (urlsCache == null)
			{
				throw new ArgumentNullException("urlsCache");
			}
			if (tokenRenewSubmitter == null)
			{
				throw new ArgumentNullException("tokenRenewSubmitter");
			}
			this.urlsCache = urlsCache;
			this.tokenRenewSubmitter = tokenRenewSubmitter;
		}

		internal void Execute(TokenRenewQueryContext queryContext)
		{
			if (queryContext == null)
			{
				throw new ArgumentNullException("queryContext");
			}
			if (queryContext.TokenRenewRequestAssets == null)
			{
				throw new ArgumentNullException("QueryContext.TokenRenewRequestAssets");
			}
			if (queryContext.TokenRenewRequestAssets.Count == 0)
			{
				throw new ArgumentException("QueryContext.TokenRenewRequestAssets must include 1 or more extensions");
			}
			this.queryContext = queryContext;
			this.downloadQueue = new Queue<TokenRenewRequestAsset>(queryContext.TokenRenewRequestAssets.Count);
			foreach (TokenRenewRequestAsset item in queryContext.TokenRenewRequestAssets)
			{
				this.downloadQueue.Enqueue(item);
			}
			this.downloadToken = new DownloadToken(this.urlsCache);
			this.ExecuteDownload(this.downloadToken);
		}

		internal void ExecuteDownload(DownloadToken downloadToken)
		{
			if (this.downloadQueue.Count > 0)
			{
				this.tokensInRequest.Clear();
				while (this.tokensInRequest.Count < 20 && this.downloadQueue.Count > 0)
				{
					TokenRenewRequestAsset item = this.downloadQueue.Dequeue();
					this.tokensInRequest.Add(item);
				}
				downloadToken.Execute(this.tokensInRequest, this.queryContext.DeploymentId, new BaseAsyncCommand.GetLoggedMailboxIdentifierCallback(this.GetLoggedMailboxIdentifier), new DownloadToken.SuccessCallback(this.DownloadTokenSuccessCallback), new BaseAsyncCommand.FailureCallback(this.DownloadTokenFailureCallback));
				return;
			}
			GetTokens.Tracer.TraceDebug(0L, "GetTokens.ExecuteDownload: Downloads complete.");
			this.ExecuteNextTokenRenewQuery();
		}

		private void DownloadTokenFailureCallback(Exception exception)
		{
			GetTokens.Tracer.TraceError<Exception>(0L, "GetTokens.DownloadTokenFailureCallback called with exception: {0}", exception);
			foreach (TokenRenewRequestAsset tokenRenewRequestAsset in this.tokensInRequest)
			{
				this.appStatuses[tokenRenewRequestAsset.ExtensionID] = "2.0";
			}
			this.ExecuteDownload(this.downloadToken);
		}

		private void DownloadTokenSuccessCallback(Dictionary<string, string> newTokens, Dictionary<string, string> appStatusCodes)
		{
			GetTokens.Tracer.TraceDebug<int>(0L, "GetTokens.DownloadTokenSuccessCallback called for {0} token", this.tokensInRequest.Count);
			foreach (KeyValuePair<string, string> keyValuePair in newTokens)
			{
				this.downloadedTokens.Add(keyValuePair.Key, keyValuePair.Value);
			}
			foreach (KeyValuePair<string, string> keyValuePair2 in appStatusCodes)
			{
				this.appStatuses.Add(keyValuePair2.Key, keyValuePair2.Value);
			}
			this.ExecuteDownload(this.downloadToken);
		}

		private string GetLoggedMailboxIdentifier()
		{
			return ExtensionDiagnostics.GetLoggedMailboxIdentifier(this.queryContext.ExchangePrincipal);
		}

		private void ExecuteNextTokenRenewQuery()
		{
			this.WriteTokensToMailbox();
			this.downloadedTokens.Clear();
			this.appStatuses.Clear();
			this.queryContext = null;
			this.downloadQueue = null;
			this.downloadToken = null;
			this.tokenRenewSubmitter.ExecuteTokenRenewQuery(this);
		}

		private void WriteTokensToMailbox()
		{
			if (this.downloadedTokens.Count == 0 && this.appStatuses.Count == 0)
			{
				return;
			}
			GetTokens.Tracer.TraceDebug<int, int>(0L, "GetTokens.WriteTokensToMailbox: Writing renewed tokens for {0} apps, failure error codes for {0} apps.", this.downloadedTokens.Count, this.appStatuses.Count);
			Exception ex = InstalledExtensionTable.RunClientExtensionAction(delegate
			{
				using (MailboxSession mailboxSession = MailboxSession.OpenAsSystemService(this.queryContext.ExchangePrincipal, this.queryContext.CultureInfo, this.queryContext.ClientInfoString))
				{
					using (InstalledExtensionTable installedExtensionTable = InstalledExtensionTable.CreateInstalledExtensionTable(this.queryContext.Domain, this.queryContext.IsUserScope, this.queryContext.OrgEmptyMasterTableCache, mailboxSession))
					{
						foreach (KeyValuePair<string, string> keyValuePair in this.downloadedTokens)
						{
							installedExtensionTable.ConfigureEtoken(keyValuePair.Key, keyValuePair.Value, true);
						}
						foreach (KeyValuePair<string, string> keyValuePair2 in this.appStatuses)
						{
							installedExtensionTable.ConfigureAppStatus(keyValuePair2.Key, keyValuePair2.Value);
						}
						installedExtensionTable.SaveXML();
					}
				}
			});
			if (ex != null)
			{
				GetTokens.Tracer.TraceError<Exception>(0L, "GetTokens.WriteTokensToMailbox: Writing renewed tokens failed. Exception: {0}", ex);
				ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_FailedToWritebackRenewedTokens, null, new object[]
				{
					"ProcessTokenRenew",
					ExtensionDiagnostics.GetLoggedMailboxIdentifier(this.queryContext.ExchangePrincipal),
					ExtensionDiagnostics.GetLoggedExceptionString(ex)
				});
				return;
			}
			ExtensionDiagnostics.LogToDatacenterOnly(ApplicationLogicEventLogConstants.Tuple_ProcessTokenRenewCompleted, null, new object[]
			{
				"ProcessTokenRenew",
				ExtensionDiagnostics.GetLoggedMailboxIdentifier(this.queryContext.ExchangePrincipal)
			});
		}

		private const string ScenarioProcessTokens = "ProcessTokenRenew";

		private const string ScenarioGetTokens = "GetTokens";

		private const int MaxTokensInOneRequest = 20;

		private static readonly Trace Tracer = ExTraceGlobals.ExtensionTracer;

		private OmexWebServiceUrlsCache urlsCache;

		private TokenRenewSubmitter tokenRenewSubmitter;

		private TokenRenewQueryContext queryContext;

		private Queue<TokenRenewRequestAsset> downloadQueue;

		private DownloadToken downloadToken;

		private Dictionary<string, string> downloadedTokens = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		private Dictionary<string, string> appStatuses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		private List<TokenRenewRequestAsset> tokensInRequest = new List<TokenRenewRequestAsset>(20);
	}
}
