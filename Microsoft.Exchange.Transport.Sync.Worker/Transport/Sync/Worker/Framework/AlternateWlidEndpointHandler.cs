using System;
using System.IO;
using System.Security;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.LiveIDAuthentication;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Win32;

namespace Microsoft.Exchange.Transport.Sync.Worker.Framework
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class AlternateWlidEndpointHandler
	{
		internal AlternateWlidEndpointHandler(string registryKeyName, SyncLogSession syncLogSession, Trace tracer)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("registryKeyName", registryKeyName);
			SyncUtilities.ThrowIfArgumentNull("syncLogSession", syncLogSession);
			SyncUtilities.ThrowIfArgumentNull("tracer", tracer);
			this.registryKeyName = registryKeyName;
			this.syncLogSession = syncLogSession;
			this.tracer = tracer;
		}

		internal void SetWlidEndpoint(LiveIDAuthenticationClient authenticationClient)
		{
			Uri uri = null;
			Exception ex;
			this.alternateWlidLoaded = this.TryLoadAlternateWlidEndpoint(out uri, out ex);
			if (this.alternateWlidLoaded)
			{
				this.syncLogSession.LogDebugging((TSLID)664UL, this.tracer, "A valid alternate wlid endpoint found in registry: {0}", new object[]
				{
					uri
				});
				this.originalAuthServerUri = authenticationClient.AuthServerUri;
				this.syncLogSession.LogDebugging((TSLID)665UL, this.tracer, "Backing up originalAuthServerUri: {0}", new object[]
				{
					this.originalAuthServerUri
				});
				authenticationClient.AuthServerUri = uri;
				return;
			}
			this.syncLogSession.LogError((TSLID)666UL, this.tracer, "No valid alternate wlid endpoint found, error (if any): {0}", new object[]
			{
				ex
			});
		}

		internal void RestoreWlidEndpoint(LiveIDAuthenticationClient authenticationClient)
		{
			if (authenticationClient != null && this.alternateWlidLoaded)
			{
				authenticationClient.AuthServerUri = this.originalAuthServerUri;
				this.alternateWlidLoaded = false;
			}
		}

		private bool TryLoadAlternateWlidEndpoint(out Uri alternateWlidUri, out Exception exception)
		{
			alternateWlidUri = null;
			exception = null;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(AlternateWlidEndpointHandler.AlternateWlidEndpointLocation))
				{
					if (registryKey != null)
					{
						string text = registryKey.GetValue(this.registryKeyName) as string;
						if (!string.IsNullOrEmpty(text))
						{
							return Uri.TryCreate(text, UriKind.Absolute, out alternateWlidUri);
						}
					}
				}
			}
			catch (SecurityException ex)
			{
				exception = ex;
			}
			catch (IOException ex2)
			{
				exception = ex2;
			}
			catch (UnauthorizedAccessException ex3)
			{
				exception = ex3;
			}
			return false;
		}

		internal static readonly string AlternateWlidEndpointLocation = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\TransportSync\\Wlid\\";

		private readonly SyncLogSession syncLogSession;

		private readonly Trace tracer;

		private Uri originalAuthServerUri;

		private bool alternateWlidLoaded;

		private string registryKeyName;
	}
}
