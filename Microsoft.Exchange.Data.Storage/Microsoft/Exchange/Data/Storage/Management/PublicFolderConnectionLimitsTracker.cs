using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PublicFolderConnectionLimitsTracker
	{
		private PublicFolderConnectionLimitsTracker()
		{
			this.maxTokens = StoreSession.GetConfigFromRegistry("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\PublicFolder", "MaxCrossServerConnections", 20, null);
			this.minTokensPerServer = StoreSession.GetConfigFromRegistry("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\PublicFolder", "MinCrossServerConnections", 5, null);
			this.reservedTokensPerActiveServer = StoreSession.GetConfigFromRegistry("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\PublicFolder", "ReservedConnectionsPerActiveServer", 3, null);
			this.availableTokenCount = this.maxTokens;
			this.tokenTracker = new Dictionary<string, int>();
		}

		public static PublicFolderConnectionLimitsTracker Instance
		{
			get
			{
				return PublicFolderConnectionLimitsTracker.instance;
			}
		}

		public DisposableFrame GetToken(string server)
		{
			if (string.IsNullOrWhiteSpace(server))
			{
				throw new ArgumentNullException("server");
			}
			string key = server.ToLowerInvariant();
			DisposableFrame result;
			lock (this.tokenTracker)
			{
				this.ThrowIfOverLimit(key);
				if (!this.tokenTracker.ContainsKey(key))
				{
					this.tokenTracker[key] = 1;
				}
				else
				{
					Dictionary<string, int> dictionary;
					string key2;
					(dictionary = this.tokenTracker)[key2 = key] = dictionary[key2] + 1;
				}
				this.availableTokenCount--;
				result = new DisposableFrame(delegate()
				{
					this.ReturnToken(key);
				});
			}
			return result;
		}

		private void ReturnToken(string server)
		{
			lock (this.tokenTracker)
			{
				if (this.tokenTracker.ContainsKey(server))
				{
					this.availableTokenCount++;
					Dictionary<string, int> dictionary;
					if (((dictionary = this.tokenTracker)[server] = dictionary[server] - 1) == 0)
					{
						this.tokenTracker.Remove(server);
					}
				}
			}
		}

		private void ThrowIfOverLimit(string server)
		{
			if (this.availableTokenCount == 0)
			{
				throw new LimitExceededException(ServerStrings.PublicFolderConnectionThreadLimitExceeded(this.maxTokens));
			}
			int num;
			this.tokenTracker.TryGetValue(server, out num);
			int count = this.tokenTracker.Count;
			int num2 = Math.Max(this.minTokensPerServer, this.maxTokens - this.reservedTokensPerActiveServer * count);
			if (num >= num2)
			{
				throw new LimitExceededException(ServerStrings.PublicFolderPerServerThreadLimitExceeded(server, num2, count));
			}
		}

		private static readonly PublicFolderConnectionLimitsTracker instance = new PublicFolderConnectionLimitsTracker();

		private readonly int maxTokens;

		private readonly int minTokensPerServer;

		private readonly int reservedTokensPerActiveServer;

		private Dictionary<string, int> tokenTracker;

		private int availableTokenCount;
	}
}
