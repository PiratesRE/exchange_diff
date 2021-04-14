using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.ClientAccessRules;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.OAB;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.OAB;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.Assistants;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.OABRequestHandler
{
	internal sealed class OABRequestHandler
	{
		static OABRequestHandler()
		{
			OABRequestHandler.outstandingRequests = new SimpleTimeoutCache<Guid>(OABRequestHandler.TimeBeforeRequestingToGeneratedAgain.Value, OABRequestHandler.OutstandingRequestsCacheCleanupInterval.Value);
			OABRequestHandler.outstandingRequests.CountChanged += delegate(int count)
			{
				OABRequestHandlerPerformanceCounters.CurrentNumberRequestsInCache.RawValue = (long)count;
			};
		}

		public void HandleRequest(HttpContext context)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			try
			{
				ExWatson.SendReportOnUnhandledException(delegate()
				{
					this.logger = RequestDetailsLoggerBase<RequestDetailsLogger>.InitializeRequestLogger();
					OABRequestHandler.outstandingRequests.CountChanged += this.LogOutstandingRequests;
					this.HandleRequestInternal(context);
				}, (object e) => !(e is ADOperationException) && !(e is ADTransientException) && !(e is ThreadAbortException) && !(e is OutOfMemoryException) && !(e is StackOverflowException), ReportOptions.None);
			}
			catch (ADOperationException ex)
			{
				OABRequestHandler.Tracer.TraceError<ADOperationException>((long)this.GetHashCode(), "Request failed with directory operation exception: {0}", ex);
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericError(this.logger, "ADOperationException", ex.ToString());
				RequestDetailsLogger.TryAppendToIISLog(context.Response, "&{0}={1}", new object[]
				{
					"ADOperationException",
					ex.ToString()
				});
				this.TerminateRequest(context, OABRequestHandler.Failure.Directory);
			}
			catch (ADTransientException ex2)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericError(this.logger, "ADTransientException", ex2.ToString());
				RequestDetailsLogger.TryAppendToIISLog(context.Response, "&{0}={1}", new object[]
				{
					"ADTransientException",
					ex2.ToString()
				});
				this.TerminateRequest(context, OABRequestHandler.Failure.Directory);
			}
			catch (Exception ex3)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericError(this.logger, "Exception", ex3.ToString());
				RequestDetailsLogger.TryAppendToIISLog(context.Response, "&{0}={1}", new object[]
				{
					"Exception",
					ex3.ToString()
				});
				this.TerminateRequest(context, OABRequestHandler.Failure.Unknown);
			}
			finally
			{
				stopwatch.Stop();
				OABRequestHandler.outstandingRequests.CountChanged -= this.LogOutstandingRequests;
				OABRequestHandlerPerformanceCounters.RequestHandlingAverageTime.IncrementBy(stopwatch.ElapsedTicks);
				OABRequestHandlerPerformanceCounters.RequestHandlingAverageTimeBase.Increment();
				if (this.logger != null && !this.logger.IsDisposed)
				{
					this.logger.Commit();
				}
			}
		}

		private static ADRawEntry FetchUserADRawEntry(OrganizationId organizationId, SecurityIdentifier sid)
		{
			ADRawEntry adrawEntry = null;
			if (OABRequestHandler.adRawEntryCache.TryGetValue(sid, out adrawEntry))
			{
				return adrawEntry;
			}
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId), 290, "FetchUserADRawEntry", "f:\\15.00.1497\\sources\\dev\\services\\src\\oab\\OABRequestHandler.cs");
			tenantOrRootOrgRecipientSession.UseGlobalCatalog = true;
			adrawEntry = tenantOrRootOrgRecipientSession.FindADRawEntryBySid(sid, ObjectSchema.GetInstance<ClientAccessRulesRecipientFilterSchema>().AllProperties);
			OABRequestHandler.adRawEntryCache.TryInsertAbsolute(sid, adrawEntry, OABRequestHandler.cacheTimeout);
			return adrawEntry;
		}

		private void HandleRequestInternal(HttpContext context)
		{
			ADSessionSettings sessionSettings = this.GetSessionSettings(context);
			RequestDetailsLoggerBase<RequestDetailsLogger>.SetCurrent(context, this.logger);
			this.logger.ActivityScope.UpdateFromMessage(context.Request);
			this.logger.ActivityScope.SerializeTo(context.Response);
			if (sessionSettings == null)
			{
				this.TerminateRequest(context, OABRequestHandler.Failure.AccessDenied);
				return;
			}
			Guid oabGuidFromRequest = OABRequestUrl.GetOabGuidFromRequest(context.Request);
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.logger, OABDownloadRequestMetadata.OfflineAddressBookGuid, oabGuidFromRequest);
			if (oabGuidFromRequest == Guid.Empty)
			{
				this.TerminateRequest(context, OABRequestHandler.Failure.InvalidRequest);
				return;
			}
			ConfigurableObject configurationObject = this.GetConfigurationObject(sessionSettings, oabGuidFromRequest);
			if (configurationObject == null)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericError(this.logger, "ConfigObj", "AccessDenied");
				this.TerminateRequest(context, OABRequestHandler.Failure.AccessDenied);
				return;
			}
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.logger, OABDownloadRequestMetadata.IsAddressListDeleted, configurationObject[OfflineAddressBookSchema.AddressLists] == null || this.IsAddressListDeleted((MultiValuedProperty<ADObjectId>)configurationObject[OfflineAddressBookSchema.AddressLists]));
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.logger, ActivityStandardMetadata.TenantStatus, Enum.GetName(typeof(OrganizationStatus), configurationObject[TenantRelocationRequestSchema.OrganizationStatus]));
			if (OABVariantConfigurationSettings.OabHttpClientAccessRulesEnabled)
			{
				OrganizationId organizationId = (OrganizationId)configurationObject[ADObjectSchema.OrganizationId];
				ClientAccessRuleCollection collection = ClientAccessRulesCache.Instance.GetCollection(organizationId ?? OrganizationId.ForestWideOrgId);
				if (collection.Count > 0)
				{
					SecurityIdentifier securityIdentifier = context.User.Identity.GetSecurityIdentifier();
					ADRawEntry adrawEntry = OABRequestHandler.FetchUserADRawEntry(organizationId, securityIdentifier);
					bool flag;
					if (configurationObject[ADObjectSchema.OrganizationId] != null)
					{
						flag = ClientAccessRulesUtils.ShouldBlockConnection(organizationId, (adrawEntry == null) ? string.Empty : ClientAccessRulesUtils.GetUsernameFromADRawEntry(adrawEntry), ClientAccessProtocol.OfflineAddressBook, ClientAccessRulesUtils.GetRemoteEndPointFromContext(context), ClientAccessAuthenticationMethod.BasicAuthentication, adrawEntry, delegate(ClientAccessRulesEvaluationContext httpContext)
						{
							RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(this.logger, ClientAccessRulesConstants.ClientAccessRuleName, httpContext.CurrentRule.Name);
						}, delegate(double latency)
						{
							if (latency > 50.0)
							{
								RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(this.logger, ClientAccessRulesConstants.ClientAccessRulesLatency, latency.ToString());
							}
						});
					}
					else
					{
						flag = false;
					}
					bool flag2 = flag;
					if (flag2)
					{
						RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericError(this.logger, "ConfigObj", "Request blocked by client access rule");
						this.TerminateRequest(context, OABRequestHandler.Failure.InvalidRequest);
						return;
					}
				}
			}
			this.UpdateLastRequestedTimeIfNeeded(sessionSettings, configurationObject);
			bool flag3 = false;
			bool flag4 = false;
			this.IsLocalOABFileCurrent(context.Request, configurationObject, out flag3, out flag4);
			if (flag4)
			{
				OrganizationId orgId = (OrganizationId)configurationObject[ADObjectSchema.OrganizationId];
				this.RequestGenerationOrDownload(context, orgId, sessionSettings, configurationObject, oabGuidFromRequest);
			}
			if (!flag3)
			{
				this.TerminateRequest(context, OABRequestHandler.Failure.FileNotAvailable);
			}
		}

		private ConfigurableObject GetConfigurationObject(ADSessionSettings sessionSettings, Guid offlineAddressBook)
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 454, "GetConfigurationObject", "f:\\15.00.1497\\sources\\dev\\services\\src\\oab\\OABRequestHandler.cs");
			ADObjectId entryId = new ADObjectId(offlineAddressBook);
			return tenantOrTopologyConfigurationSession.ReadADRawEntry(entryId, OABRequestHandler.ADProperties);
		}

		private void UpdateLastRequestedTimeIfNeeded(ADSessionSettings sessionSettings, ConfigurableObject configurationObject)
		{
			if (OABVariantConfigurationSettings.IsGenerateRequestedOABsOnlyEnabled)
			{
				DateTime? dateTime = (DateTime?)configurationObject[OfflineAddressBookSchema.LastRequestedTime];
				DateTime? dateTime2 = (DateTime?)configurationObject[OfflineAddressBookSchema.LastTouchedTime];
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.logger, OABDownloadRequestMetadata.LastRequestedTime, (dateTime != null) ? dateTime.ToString() : "(null)");
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.logger, OABDownloadRequestMetadata.LastTouchedTime, (dateTime2 != null) ? dateTime2.ToString() : "(null)");
				if (dateTime == null || DateTime.UtcNow - dateTime.Value > OABRequestHandler.LastRequestedTimeUpdateInterval.Value)
				{
					IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(false, ConsistencyMode.IgnoreInvalid, sessionSettings, 496, "UpdateLastRequestedTimeIfNeeded", "f:\\15.00.1497\\sources\\dev\\services\\src\\oab\\OABRequestHandler.cs");
					OfflineAddressBook offlineAddressBook = (OfflineAddressBook)tenantOrTopologyConfigurationSession.Read<OfflineAddressBook>(configurationObject.Identity);
					DateTime utcNow = DateTime.UtcNow;
					offlineAddressBook.LastRequestedTime = new DateTime?(utcNow);
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.logger, OABDownloadRequestMetadata.LastRequestedTime, offlineAddressBook.LastRequestedTime);
					tenantOrTopologyConfigurationSession.Save(offlineAddressBook);
				}
			}
		}

		private void IsLocalOABFileCurrent(HttpRequest request, ConfigurableObject configurationObject, out bool shouldServeFiles, out bool shouldRequestOABGeneration)
		{
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.logger, OABDownloadRequestMetadata.FileRequested, request.PhysicalPath);
			if (request.PhysicalPath.EndsWith("\\oab.xml", StringComparison.OrdinalIgnoreCase))
			{
				FileInfo fileInfo = new FileInfo(request.PhysicalPath);
				if (!fileInfo.Exists)
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericError(this.logger, "IsLocalOABFileCurrent:NotFound", request.PhysicalPath);
					shouldServeFiles = false;
					shouldRequestOABGeneration = true;
					return;
				}
				if (!OABVariantConfigurationSettings.IsEnforceManifestVersionMatchEnabled && !OABRequestHandler.EnforceManifestVersionMatch.Value)
				{
					DateTime t = DateTime.UtcNow - OABRequestHandler.FallbackOldestFileAllowed;
					if (fileInfo.LastWriteTimeUtc < t)
					{
						RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericError(this.logger, "IsLocalOABFileCurrent:ManifestEnforcementDisabledAndLocalFileTooOld", string.Format("File {0} was last modified (UTC) {1}", request.PhysicalPath, fileInfo.LastWriteTimeUtc.ToString()));
						shouldServeFiles = false;
						shouldRequestOABGeneration = true;
						return;
					}
					shouldServeFiles = true;
					shouldRequestOABGeneration = false;
					return;
				}
				else
				{
					OABManifest oabmanifest = OABManifest.LoadFromFile(request.PhysicalPath);
					if (oabmanifest == null)
					{
						RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericError(this.logger, "IsLocalOABFileCurrent:DiskManifestUnreadable", request.PhysicalPath);
						shouldServeFiles = false;
						shouldRequestOABGeneration = false;
						return;
					}
					OfflineAddressBookManifestVersion version = oabmanifest.GetVersion();
					if (version == null)
					{
						RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericError(this.logger, "IsLocalOABFileCurrent:DiskManifestVersionUnreadable", request.PhysicalPath);
						shouldServeFiles = false;
						shouldRequestOABGeneration = true;
						return;
					}
					OfflineAddressBookManifestVersion offlineAddressBookManifestVersion = (OfflineAddressBookManifestVersion)configurationObject[OfflineAddressBookSchema.ManifestVersion];
					if (offlineAddressBookManifestVersion == null)
					{
						DateTime t2 = DateTime.UtcNow - OABRequestHandler.FallbackOldestFileAllowed;
						if (fileInfo.LastWriteTimeUtc < t2)
						{
							RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericError(this.logger, "IsLocalOABFileCurrent:NoVersionStampedInADAndLocalFileTooOld", string.Format("File {0} was last modified (UTC) {1}", request.PhysicalPath, fileInfo.LastWriteTimeUtc.ToString()));
							shouldServeFiles = false;
							shouldRequestOABGeneration = true;
							return;
						}
						shouldServeFiles = true;
						shouldRequestOABGeneration = false;
						return;
					}
					else if (!version.Equals(offlineAddressBookManifestVersion))
					{
						RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericError(this.logger, "IsLocalOABFileCurrent:NotCurrent", string.Format("File {0} is not a current manifest for this OAB; file version is {1} while current version is {2}", request.PhysicalPath, version, offlineAddressBookManifestVersion));
						shouldServeFiles = false;
						shouldRequestOABGeneration = true;
						return;
					}
				}
			}
			shouldServeFiles = true;
			shouldRequestOABGeneration = false;
		}

		private void RequestGenerationOrDownload(HttpContext context, OrganizationId orgId, ADSessionSettings sessionSettings, ConfigurableObject configurationObject, Guid offlineAddressBook)
		{
			if (OABRequestHandler.outstandingRequests.Contains(offlineAddressBook))
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(this.logger, "OABGenRequest", "ignoring request to generator OAB files because we have made this request recently");
				return;
			}
			Server localServer = LocalServerCache.LocalServer;
			if (localServer == null)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericError(this.logger, "OABGenRequest", "unable to get local server for OABGEN");
				return;
			}
			ADUser aduser = null;
			BackEndServer backEndServer = null;
			if (OABVariantConfigurationSettings.IsLinkedOABGenMailboxesEnabled)
			{
				ADObjectId adobjectId = (ADObjectId)configurationObject[OfflineAddressBookSchema.GeneratingMailbox];
				if (adobjectId != null)
				{
					IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 744, "RequestGenerationOrDownload", "f:\\15.00.1497\\sources\\dev\\services\\src\\oab\\OABRequestHandler.cs");
					aduser = (tenantOrRootOrgRecipientSession.Read(adobjectId) as ADUser);
				}
			}
			if (aduser == null)
			{
				aduser = OrganizationMailbox.GetLocalOrganizationMailboxByCapability(orgId, OrganizationCapability.OABGen, true);
				backEndServer = new BackEndServer(localServer.Fqdn, localServer.VersionNumber);
			}
			if (aduser == null)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericError(this.logger, "OABGenRequest", "unable to locate organization mailbox for OAB");
				return;
			}
			if (backEndServer == null)
			{
				ADObjectId database = aduser.Database;
				if (database != null && database.ObjectGuid != Guid.Empty)
				{
					using (MailboxServerLocator mailboxServerLocator = MailboxServerLocator.CreateWithResourceForestFqdn(database.ObjectGuid, null))
					{
						backEndServer = mailboxServerLocator.GetServer();
					}
				}
			}
			if (backEndServer == null)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericError(this.logger, "OABGenRequest", "unable to locate organization mailbox server using MailboxServerLocator");
				return;
			}
			if (localServer.Fqdn.Equals(backEndServer.Fqdn, StringComparison.OrdinalIgnoreCase))
			{
				try
				{
					OABRequestHandler.outstandingRequests.Add(offlineAddressBook);
					AssistantsRpcClient assistantsRpcClient = new AssistantsRpcClient(localServer.Fqdn);
					OABGeneratorAssistantRunNowParameters oabgeneratorAssistantRunNowParameters = new OABGeneratorAssistantRunNowParameters
					{
						PartitionId = aduser.Id.GetPartitionId(),
						ObjectGuid = offlineAddressBook,
						Description = "from OABRequestHandler on Server:" + localServer.Fqdn
					};
					assistantsRpcClient.StartWithParams("OABGeneratorAssistant", aduser.ExchangeGuid, aduser.Database.ObjectGuid, oabgeneratorAssistantRunNowParameters.ToString());
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(this.logger, "OABGenRequest", string.Format("successful request for OAB {0} to org mailbox {1}", offlineAddressBook, aduser.Id));
					return;
				}
				catch (RpcException arg)
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericError(this.logger, "OABGenRequest", string.Format("unable to make RunNow request to the assistant for OAB {0} due exception: {1}", offlineAddressBook, arg));
					return;
				}
			}
			if ((bool)configurationObject[OfflineAddressBookSchema.ShadowMailboxDistributionEnabled])
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(this.logger, "OABGenDownload", string.Format("starting download request (shadow distribution enabled) for OAB {0} from {1}", offlineAddressBook, backEndServer.Fqdn));
				BITSDownloadManager.Instance.StartOABDownloadFromRemoteServer(context.Request.PhysicalApplicationPath, backEndServer.Fqdn, offlineAddressBook);
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(this.logger, "OABGenDownload", string.Format("finished download request (shadow distribution enabled) for OAB {0} from {1}", offlineAddressBook, backEndServer.Fqdn));
				return;
			}
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericError(this.logger, "OABGenDownload", "OAB is generated on a remote server and shadow distribution is not enabled");
		}

		private void TerminateRequest(HttpContext context, OABRequestHandler.Failure failure)
		{
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.logger, OABDownloadRequestMetadata.FailureCode, failure);
			switch (failure)
			{
			case OABRequestHandler.Failure.AccessDenied:
				context.Response.StatusCode = 403;
				context.Response.SubStatusCode = 5;
				OABRequestHandlerPerformanceCounters.AccessDeniedFailures.Increment();
				break;
			case OABRequestHandler.Failure.InvalidRequest:
				context.Response.StatusCode = 403;
				OABRequestHandlerPerformanceCounters.InvalidRequestFailures.Increment();
				break;
			case OABRequestHandler.Failure.FileNotAvailable:
				context.Response.StatusCode = 404;
				OABRequestHandlerPerformanceCounters.FileNotAvailableFailures.Increment();
				break;
			case OABRequestHandler.Failure.Directory:
				context.Response.StatusCode = 500;
				OABRequestHandlerPerformanceCounters.DirectoryFailures.Increment();
				break;
			case OABRequestHandler.Failure.Unknown:
				context.Response.StatusCode = 500;
				OABRequestHandlerPerformanceCounters.UnknownFailures.Increment();
				break;
			}
			context.ApplicationInstance.CompleteRequest();
		}

		private ADSessionSettings GetSessionSettings(HttpContext context)
		{
			if (!OABVariantConfigurationSettings.IsMultitenancyEnabled)
			{
				return ADSessionSettings.FromRootOrgScopeSet();
			}
			string text = context.Request.Headers["X-WLID-MemberName"];
			if (string.IsNullOrEmpty(text))
			{
				SidBasedIdentity sidBasedIdentity = context.User.Identity as SidBasedIdentity;
				if (sidBasedIdentity != null)
				{
					return ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(sidBasedIdentity.UserOrganizationId);
				}
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericError(this.logger, "OAB.GetSessionSettings", "unable to retrieve member name from HttpContext");
				return null;
			}
			else
			{
				string[] array = text.Split(new char[]
				{
					'@'
				});
				if (array.Length != 2 || string.IsNullOrWhiteSpace(array[0]) || string.IsNullOrWhiteSpace(array[1]))
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericError(this.logger, "OAB.GetSessionSettings", string.Format("member name has invalid format: {0}", text));
					return null;
				}
				string text2 = array[1];
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.logger, OABDownloadRequestMetadata.Domain, text2);
				RequestDetailsLogger.TryAppendToIISLog(context.Response, "&{0}={1}", new object[]
				{
					OABDownloadRequestMetadata.Domain,
					text2
				});
				return ADSessionSettings.FromTenantAcceptedDomain(text2);
			}
		}

		private void LogOutstandingRequests(int count)
		{
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.logger, OABDownloadRequestMetadata.NoOfRequestsOutStanding, count);
		}

		private bool IsAddressListDeleted(IEnumerable<ADObjectId> addressLists)
		{
			return addressLists.Any((ADObjectId addressList) => addressList.IsDeleted);
		}

		private static readonly Microsoft.Exchange.Diagnostics.Trace Tracer = ExTraceGlobals.HttpHandlerTracer;

		private static readonly TimeSpan FallbackOldestFileAllowed = TimeSpan.FromHours(24.0);

		private static readonly TimeSpanAppSettingsEntry TimeBeforeRequestingToGeneratedAgain = new TimeSpanAppSettingsEntry("TimeBeforeRequestingToGeneratedAgain", TimeSpanUnit.Minutes, TimeSpan.FromMinutes(480.0), OABRequestHandler.Tracer);

		private static readonly TimeSpanAppSettingsEntry OutstandingRequestsCacheCleanupInterval = new TimeSpanAppSettingsEntry("OutstandingRequestsCacheCleanupInterval", TimeSpanUnit.Minutes, TimeSpan.FromMinutes(15.0), OABRequestHandler.Tracer);

		private static readonly TimeSpanAppSettingsEntry LastRequestedTimeUpdateInterval = new TimeSpanAppSettingsEntry("LastRequestedTimeUpdateInterval", TimeSpanUnit.Days, TimeSpan.FromDays(7.0), OABRequestHandler.Tracer);

		private static readonly BoolAppSettingsEntry EnforceManifestVersionMatch = new BoolAppSettingsEntry("EnforceManifestVersionMatch", false, OABRequestHandler.Tracer);

		private static readonly ADPropertyDefinition[] ADProperties = new ADPropertyDefinition[]
		{
			ADObjectSchema.OrganizationId,
			OfflineAddressBookSchema.LastTouchedTime,
			OfflineAddressBookSchema.LastRequestedTime,
			OfflineAddressBookSchema.ManifestVersion,
			OfflineAddressBookSchema.GeneratingMailbox,
			OfflineAddressBookSchema.ShadowMailboxDistributionEnabled,
			OfflineAddressBookSchema.AddressLists
		};

		private static readonly SimpleTimeoutCache<Guid> outstandingRequests;

		private static IntAppSettingsEntry oabADRawEntryCacheBucketSize = new IntAppSettingsEntry("OabADRawEntryCacheBucketSize", 1000, null);

		private static ExactTimeoutCache<SecurityIdentifier, ADRawEntry> adRawEntryCache = new ExactTimeoutCache<SecurityIdentifier, ADRawEntry>(null, null, null, OABRequestHandler.oabADRawEntryCacheBucketSize.Value, false);

		private static IntAppSettingsEntry oabADRawEntryCacheTimeout = new IntAppSettingsEntry("OabADRawEntryCacheTimeout", 5, null);

		private static TimeSpan cacheTimeout = TimeSpan.FromMinutes((double)OABRequestHandler.oabADRawEntryCacheTimeout.Value);

		private RequestDetailsLogger logger;

		private enum Failure
		{
			AccessDenied,
			InvalidRequest,
			FileNotAvailable,
			Directory,
			Unknown
		}
	}
}
