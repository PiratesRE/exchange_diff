using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Availability;
using Microsoft.Exchange.InfoWorker.EventLog;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal static class TargetForestConfigurationCache
	{
		public static TargetForestConfiguration FindByDomain(OrganizationId organizationId, string domainName)
		{
			if (!VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled)
			{
				TargetForestConfiguration result;
				if (!TargetForestConfigurationCache.cache.TryGetValue(domainName, out result))
				{
					TargetForestConfigurationCache.ConfigurationTracer.TraceError<object, string>(0L, "{0}: TargetForestConfiguration for domain {1} could not be found in cache", TraceContext.Get(), domainName);
					throw new AddressSpaceNotFoundException(Strings.descConfigurationInformationNotFound(domainName), 51004U);
				}
				return result;
			}
			else
			{
				if (organizationId == null)
				{
					OrganizationId forestWideOrgId = OrganizationId.ForestWideOrgId;
				}
				OrganizationIdCacheValue organizationIdCacheValue = OrganizationIdCache.Singleton.Get(organizationId);
				if (organizationIdCacheValue == null)
				{
					TargetForestConfigurationCache.ConfigurationTracer.TraceError<object, OrganizationId>(0L, "{0}: Unable to find organization {1} in the OrgId cache", TraceContext.Get(), organizationId);
					throw new AddressSpaceNotFoundException(Strings.descConfigurationInformationNotFound(domainName), 64316U);
				}
				AvailabilityAddressSpace availabilityAddressSpace = organizationIdCacheValue.GetAvailabilityAddressSpace(domainName);
				if (availabilityAddressSpace != null)
				{
					TargetForestConfiguration result = TargetForestConfigurationCache.ConstructTargetForestConfiguration(availabilityAddressSpace, null);
					return result;
				}
				TargetForestConfigurationCache.ConfigurationTracer.TraceError<object, string, OrganizationId>(0L, "{0}: TargetForestConfiguration for domain {1} could not be found in cache for organization {2}", TraceContext.Get(), domainName, organizationId);
				throw new AddressSpaceNotFoundException(Strings.descConfigurationInformationNotFound(domainName), 47932U);
			}
		}

		internal static void Populate(DateTime populateDeadline)
		{
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled)
			{
				return;
			}
			Dictionary<string, TargetForestConfiguration> dictionary = new Dictionary<string, TargetForestConfiguration>(StringComparer.InvariantCultureIgnoreCase);
			AvailabilityAddressSpace[] array = TargetForestConfigurationCache.SearchAddressSpaceForEnterprise();
			if (array != null && array.Length > 0)
			{
				ScpSearch localScpSearch = ScpSearch.FindLocal();
				foreach (AvailabilityAddressSpace availabilityAddressSpace in array)
				{
					TargetForestConfiguration targetForestConfiguration;
					if (dictionary.TryGetValue(availabilityAddressSpace.ForestName, out targetForestConfiguration))
					{
						TargetForestConfigurationCache.ConfigurationTracer.TraceError<string, string, ADObjectId>(0L, "There are two or more AvailabilityAddressSpace objects in AD with same ForestName {0}. Existing is {1}. Ignoring object {2}", availabilityAddressSpace.ForestName, targetForestConfiguration.Id, availabilityAddressSpace.Id);
						Globals.AvailabilityLogger.LogEvent(InfoWorkerEventLogConstants.Tuple_DuplicateAvailabilityAddressSpace, null, new object[]
						{
							Globals.ProcessId,
							availabilityAddressSpace.ForestName,
							targetForestConfiguration.Id,
							availabilityAddressSpace.Id
						});
					}
					else if (availabilityAddressSpace.AccessMethod == AvailabilityAccessMethod.InternalProxy)
					{
						TargetForestConfigurationCache.ConfigurationTracer.TraceDebug<string>(0L, "Ignoring AvailabilityAddressSpace with ForestName {0} because it is an internal proxy.", availabilityAddressSpace.ForestName);
					}
					else
					{
						dictionary.Add(availabilityAddressSpace.ForestName, TargetForestConfigurationCache.ConstructTargetForestConfiguration(availabilityAddressSpace, localScpSearch));
					}
					if (DateTime.UtcNow > populateDeadline)
					{
						TargetForestConfigurationCache.ConfigurationTracer.TraceError(0L, "Unable to continue populating the target forest cache. Deadline has been exceeded.");
						break;
					}
				}
			}
			if (dictionary.Count > 0)
			{
				TargetForestConfigurationCache.ConfigurationTracer.TraceDebug<int>(0L, "Updating to new TargetForestConfiguration cache with {0} entries", dictionary.Count);
				TargetForestConfigurationCache.cache = dictionary;
			}
		}

		private static TargetForestConfiguration ConstructTargetForestConfiguration(AvailabilityAddressSpace addressSpace, ScpSearch localScpSearch)
		{
			TargetForestConfigurationCache.ConfigurationTracer.TraceDebug<string>(0L, "Processing AvailabilityAddressSpace name: {0}", addressSpace.ForestName);
			NetworkCredential networkCredential = null;
			AutodiscoverUrlSource autodiscoverUrlSource = AutodiscoverUrlSource.Unknown;
			Uri uri = null;
			if (addressSpace.AccessMethod != AvailabilityAccessMethod.OrgWideFB && addressSpace.AccessMethod != AvailabilityAccessMethod.PerUserFB)
			{
				if (addressSpace.AccessMethod != AvailabilityAccessMethod.OrgWideFBBasic)
				{
					goto IL_1B2;
				}
			}
			try
			{
				networkCredential = TargetForestConfigurationCache.GetCredentials(addressSpace);
			}
			catch (InvalidCrossForestCredentialsException exception)
			{
				return new TargetForestConfiguration(addressSpace.Id.ToString(), addressSpace.ForestName, exception);
			}
			if (addressSpace.TargetAutodiscoverEpr != null)
			{
				uri = addressSpace.TargetAutodiscoverEpr;
				TargetForestConfigurationCache.ConfigurationTracer.TraceDebug<Uri>(0L, "Retrieved Autodiscover URL {0} from address space.", uri);
				goto IL_1B2;
			}
			TargetForestConfigurationCache.ConfigurationTracer.TraceDebug<string>(0L, "Searching for SCP objects for domain {0}", addressSpace.ForestName);
			uri = localScpSearch.FindRemote(addressSpace.ForestName, networkCredential);
			if (uri != null)
			{
				TargetForestConfigurationCache.ConfigurationTracer.TraceDebug<string, Uri>(0L, "Found autodiscover URL from SCP objects for domain {0}. Url is: {1}", addressSpace.ForestName, uri);
				autodiscoverUrlSource = AutodiscoverUrlSource.SCP;
				goto IL_1B2;
			}
			TargetForestConfigurationCache.ConfigurationTracer.TraceDebug<string>(0L, "Found no suitable autodiscover URL from SCP objects for domain {0}. Trying well-known endpoints.", addressSpace.ForestName);
			uri = TargetForestConfigurationCache.DiscoverFromWellKnown(addressSpace.ForestName, networkCredential);
			if (uri != null)
			{
				TargetForestConfigurationCache.ConfigurationTracer.TraceDebug<string, Uri>(0L, "Found autodiscover URL from well-known endpoints for domain {0}. Url is: {1}", addressSpace.ForestName, uri);
				autodiscoverUrlSource = AutodiscoverUrlSource.WellKnown;
				goto IL_1B2;
			}
			TargetForestConfigurationCache.ConfigurationTracer.TraceDebug<string>(0L, "Found no suitable autodiscover URL from well-known endpoints for domain {0}. Trying SRV records from DNS.", addressSpace.ForestName);
			uri = AutoDiscoverDnsReader.Query(addressSpace.ForestName);
			if (uri != null)
			{
				TargetForestConfigurationCache.ConfigurationTracer.TraceDebug<string, Uri>(0L, "Found autodiscover URL from SRV records from DNS for domain {0}. Url is: {1}", addressSpace.ForestName, uri);
				autodiscoverUrlSource = AutodiscoverUrlSource.SRV;
				goto IL_1B2;
			}
			TargetForestConfigurationCache.ConfigurationTracer.TraceDebug<string>(0L, "Found no suitable autodiscover URL from well-known endpoint lookup for domain {0}.", addressSpace.ForestName);
			return new TargetForestConfiguration(addressSpace.Id.ToString(), addressSpace.ForestName, new AutoDiscoverFailedException(Strings.descAvailabilityAddressSpaceFailed(addressSpace.Id.ToString())));
			IL_1B2:
			return new TargetForestConfiguration(addressSpace.Id.ToString(), addressSpace.ForestName, addressSpace.AccessMethod, networkCredential, uri, autodiscoverUrlSource);
		}

		internal static AvailabilityAddressSpace[] SearchAddressSpaceForEnterprise()
		{
			AvailabilityAddressSpace[] result;
			try
			{
				TargetForestConfigurationCache.ConfigurationTracer.TraceDebug(0L, "Searching for AvailabilityAddressSpace objects in the AD");
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 337, "SearchAddressSpaceForEnterprise", "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\RequestDispatch\\TargetForestConfigurationCache.cs");
				AvailabilityAddressSpace[] array = tenantOrTopologyConfigurationSession.Find<AvailabilityAddressSpace>(null, QueryScope.SubTree, null, null, 100);
				TargetForestConfigurationCache.ConfigurationTracer.TraceDebug<int>(0L, "Found {0} AvailabilityAddressSpace objects in AD.", (array != null) ? array.Length : 0);
				result = array;
			}
			catch (LocalizedException arg)
			{
				TargetForestConfigurationCache.ConfigurationTracer.TraceError<LocalizedException>(0L, "Failed to read AvailabilityAddressSpace objects from the AD because of exception: {0}", arg);
				result = null;
			}
			return result;
		}

		private static NetworkCredential GetCredentials(AvailabilityAddressSpace addressSpace)
		{
			if (addressSpace.UseServiceAccount)
			{
				TargetForestConfigurationCache.ConfigurationTracer.TraceDebug<ADObjectId>(0L, "AvailabilityAddressSpace {0} has been configured with UserServiceAccount. Returning default service account credentials.", addressSpace.Id);
				return null;
			}
			if (string.IsNullOrEmpty(addressSpace.UserName))
			{
				TargetForestConfigurationCache.ConfigurationTracer.TraceError<ADObjectId>(0L, "AvailabilityAddressSpace {0} has UseServiceAccount is false but username is null or empty.", addressSpace.Id);
				Globals.AvailabilityLogger.LogEvent(InfoWorkerEventLogConstants.Tuple_InvalidCredentialsForCrossForestProxying, null, new object[]
				{
					Globals.ProcessId,
					addressSpace.ForestName,
					Strings.descNullUserName
				});
				throw new InvalidCrossForestCredentialsException(Strings.descNullUserName);
			}
			if (SmtpAddress.IsValidSmtpAddress(addressSpace.UserName))
			{
				SmtpAddress smtpAddress = new SmtpAddress(addressSpace.UserName);
				return new NetworkCredential(addressSpace.UserName, addressSpace[AvailabilityAddressSpaceSchema.Password] as string, smtpAddress.Domain);
			}
			string[] array = addressSpace.UserName.Split(new char[]
			{
				'\\'
			});
			if (array == null || array.Length != 2)
			{
				TargetForestConfigurationCache.ConfigurationTracer.TraceError<ADObjectId>(0L, "AvailabilityAddressSpace {0} has invalid credentials for cross forest authentication.", addressSpace.Id);
				Globals.AvailabilityLogger.LogEvent(InfoWorkerEventLogConstants.Tuple_InvalidCredentialsForCrossForestProxying, null, new object[]
				{
					Globals.ProcessId,
					addressSpace.ForestName,
					Strings.descInvalidCredentials
				});
				throw new InvalidCrossForestCredentialsException(Strings.descInvalidCredentials);
			}
			return new NetworkCredential(array[1], addressSpace[AvailabilityAddressSpaceSchema.Password] as string, array[0]);
		}

		private static Uri DiscoverFromWellKnown(string domainName, NetworkCredential networkCredential)
		{
			RequestLogger requestLogger = new RequestLogger();
			EmailAddress emailAddress = new EmailAddress(null, "01B62C6D-4324-448f-9884-5FEC6D18A7E2@" + domainName);
			DateTime deadline = DateTime.UtcNow + Configuration.WebRequestTimeoutInSeconds;
			UriBuilder uriBuilder = new UriBuilder();
			uriBuilder.Path = "/autodiscover/autodiscover.xml";
			uriBuilder.Scheme = Uri.UriSchemeHttps;
			TargetForestConfigurationCache.PingAutoDiscover[] array = new TargetForestConfigurationCache.PingAutoDiscover[TargetForestConfigurationCache.UriHostFormatStrings.Length];
			for (int i = 0; i < TargetForestConfigurationCache.UriHostFormatStrings.Length; i++)
			{
				uriBuilder.Host = string.Format(TargetForestConfigurationCache.UriHostFormatStrings[i], domainName);
				array[i] = new TargetForestConfigurationCache.PingAutoDiscover(uriBuilder.Uri, emailAddress, networkCredential, deadline, requestLogger);
			}
			AsyncTaskParallel asyncTaskParallel = new AsyncTaskParallel(array);
			foreach (TargetForestConfigurationCache.PingAutoDiscover pingAutoDiscover in array)
			{
				pingAutoDiscover.Parent = asyncTaskParallel;
			}
			asyncTaskParallel.Invoke(deadline);
			foreach (TargetForestConfigurationCache.PingAutoDiscover pingAutoDiscover2 in array)
			{
				if (pingAutoDiscover2.Valid)
				{
					return pingAutoDiscover2.Url;
				}
			}
			return null;
		}

		private const int MaximumAllowedRedirections = 3;

		private const int MaximumAllowedRetries = 10;

		private const int MaximumAddressSpaceCount = 100;

		private const string UriPath = "/autodiscover/autodiscover.xml";

		private const string TestUserName = "01B62C6D-4324-448f-9884-5FEC6D18A7E2";

		private static Dictionary<string, TargetForestConfiguration> cache = new Dictionary<string, TargetForestConfiguration>(StringComparer.InvariantCultureIgnoreCase);

		private static Random randomIndexSelector = new Random();

		private static readonly string[] UriHostFormatStrings = new string[]
		{
			"autodiscover.{0}",
			"{0}"
		};

		private static readonly Trace ConfigurationTracer = ExTraceGlobals.ConfigurationTracer;

		private sealed class PingAutoDiscover : AsyncTask
		{
			public Uri Url
			{
				get
				{
					return this.url;
				}
			}

			public bool Valid
			{
				get
				{
					return this.valid;
				}
			}

			public AsyncTask Parent { get; set; }

			public PingAutoDiscover(Uri url, EmailAddress emailAddress, NetworkCredential networkCredential, DateTime deadline, RequestLogger requestLogger)
			{
				this.url = url;
				this.emailAddress = emailAddress;
				this.networkCredential = networkCredential;
				this.deadline = deadline;
				this.requestLogger = requestLogger;
			}

			public override void Abort()
			{
				base.Abort();
				if (this.request != null)
				{
					this.request.Abort();
				}
			}

			public override void BeginInvoke(TaskCompleteCallback callback)
			{
				base.BeginInvoke(callback);
				this.BeginInvokeRequest();
			}

			private void BeginInvokeRequest()
			{
				this.request = new AutoDiscoverRequest(DummyApplication.Instance, null, this.requestLogger, this.url, this.emailAddress, this.networkCredential, UriSource.EmailDomain);
				this.request.BeginInvoke(new TaskCompleteCallback(this.CompleteRequest));
			}

			private void CompleteRequest(AsyncTask task)
			{
				if (this.request.Result.AutoDiscoverRedirectUri != null)
				{
					if (this.redirectionDepth > 3)
					{
						TargetForestConfigurationCache.ConfigurationTracer.TraceError<Uri, int>((long)this.GetHashCode(), "Received a redirect to {0}, but number of redirections has exceeded {1}", this.request.Result.AutoDiscoverRedirectUri, 3);
						this.valid = false;
						base.Complete();
						return;
					}
					TargetForestConfigurationCache.ConfigurationTracer.TraceDebug<Uri>((long)this.GetHashCode(), "Received a redirect to {0}, attempting to follow the redirection", this.request.Result.AutoDiscoverRedirectUri);
					this.url = this.request.Result.AutoDiscoverRedirectUri;
					this.redirectionDepth++;
					this.BeginInvokeRequest();
					return;
				}
				else
				{
					if (this.request.Result.Exception == null || this.request.Result.Exception.InnerException == null)
					{
						this.valid = true;
						base.Complete();
						this.Parent.Abort();
						return;
					}
					if (this.request.Result.Exception.InnerException is WebException && this.retryAttempt < 10 && DateTime.UtcNow < this.deadline)
					{
						TargetForestConfigurationCache.ConfigurationTracer.TraceDebug<Uri>((long)this.GetHashCode(), "It appears a transient failure happened. Retrying request to {0}", this.url);
						this.retryAttempt++;
						this.BeginInvokeRequest();
						return;
					}
					TargetForestConfigurationCache.ConfigurationTracer.TraceDebug((long)this.GetHashCode(), "It appears a transient failure happened but we cannot retry because deadline has passed.");
					this.valid = false;
					base.Complete();
					return;
				}
			}

			public override string ToString()
			{
				return "PingAutoDiscover at " + this.url;
			}

			private AutoDiscoverRequest request;

			private Uri url;

			private EmailAddress emailAddress;

			private NetworkCredential networkCredential;

			private DateTime deadline;

			private bool valid;

			private int redirectionDepth;

			private int retryAttempt;

			private RequestLogger requestLogger;
		}
	}
}
