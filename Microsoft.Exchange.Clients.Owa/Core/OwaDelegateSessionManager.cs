using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class OwaDelegateSessionManager
	{
		internal OwaDelegateSessionManager(UserContext userContext)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			this.userContext = userContext;
			this.exchangePrincipals = new Dictionary<string, ExchangePrincipal>();
		}

		internal bool TryGetExchangePrincipal(string legacyDN, out ExchangePrincipal exchangePrincipal)
		{
			bool result = false;
			exchangePrincipal = null;
			if (string.IsNullOrEmpty(legacyDN) || !Utilities.IsValidLegacyDN(legacyDN))
			{
				return result;
			}
			if (this.exchangePrincipals.TryGetValue(legacyDN.ToUpperInvariant(), out exchangePrincipal))
			{
				result = true;
			}
			else
			{
				try
				{
					ADSessionSettings adSettings = Utilities.CreateScopedADSessionSettings(this.userContext.LogonIdentity.DomainName);
					exchangePrincipal = ExchangePrincipal.FromLegacyDN(adSettings, legacyDN, RemotingOptions.AllowCrossSite);
					result = true;
					if (!this.exchangePrincipals.ContainsKey(exchangePrincipal.LegacyDn.ToUpperInvariant()))
					{
						this.exchangePrincipals.Add(exchangePrincipal.LegacyDn.ToUpperInvariant(), exchangePrincipal);
					}
				}
				catch (StoragePermanentException ex)
				{
					ExTraceGlobals.CoreCallTracer.TraceDebug<string, string>((long)this.GetHashCode(), "OwaDelegateSessionManager.TryGetExchangePrincipal. Unable to get ExchangePrincipal from legacy DN {0}. Exception: {1}.", legacyDN, ex.Message);
				}
				catch (StorageTransientException ex2)
				{
					ExTraceGlobals.CoreCallTracer.TraceDebug<string, string>((long)this.GetHashCode(), "OwaDelegateSessionManager.TryGetExchangePrincipal. Unable to get ExchangePrincipal from legacy DN {0}. Exception: {1}.", legacyDN, ex2.Message);
				}
			}
			return result;
		}

		public void RemoveInvalidExchangePrincipal(string legacyDN)
		{
			if (this.exchangePrincipals != null && this.exchangePrincipals.ContainsKey(legacyDN.ToUpperInvariant()))
			{
				this.exchangePrincipals.Remove(legacyDN.ToUpperInvariant());
			}
		}

		public void ClearAllExchangePrincipals()
		{
			if (this.exchangePrincipals != null)
			{
				this.exchangePrincipals.Clear();
				this.exchangePrincipals = null;
			}
		}

		private Dictionary<string, ExchangePrincipal> exchangePrincipals;

		private UserContext userContext;
	}
}
