using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices;
using System.Globalization;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Tracking;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.InfoWorker.Common.MessageTracking;
using Microsoft.Exchange.Net.WSTrust;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Transport.Logging.Search;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Services.Core
{
	internal abstract class BaseMessageTrackingReport<RequestType, SingleItemType> : SingleStepServiceCommand<RequestType, SingleItemType> where RequestType : BaseRequest
	{
		protected abstract string Domain { get; }

		protected abstract string DiagnosticsLevel { get; }

		protected virtual void PrepareRequest()
		{
		}

		public BaseMessageTrackingReport(CallContext callContext, RequestType request) : base(callContext, request)
		{
			this.PrepareRequest();
			if (string.IsNullOrEmpty(this.DiagnosticsLevel) || !EnumValidator<Microsoft.Exchange.Transport.Logging.Search.DiagnosticsLevel>.TryParse(this.DiagnosticsLevel, EnumParseOptions.Default, out this.diagnosticsLevelEnum))
			{
				this.diagnosticsLevelEnum = Microsoft.Exchange.Transport.Logging.Search.DiagnosticsLevel.None;
			}
			if (!ServerCache.Instance.InitializeIfNeeded(HostId.EWSApplicationPool))
			{
				ExTraceGlobals.WebServiceTracer.TraceError((long)this.GetHashCode(), "Failed to initialize MessageTracking ServerCache");
				throw new MessageTrackingFatalException();
			}
		}

		internal override bool SupportsExternalUsers
		{
			get
			{
				return true;
			}
		}

		internal override Offer ExpectedOffer
		{
			get
			{
				return Offer.SharingCalendarFreeBusy;
			}
		}

		protected void InitializeRequest()
		{
			this.LoadOrganization();
			this.CheckAccess();
			this.AfterCheckAccess();
		}

		protected void CleanupRequest()
		{
			if (this.diagnosticsLevelEnum == Microsoft.Exchange.Transport.Logging.Search.DiagnosticsLevel.Etw && this.perThreadTracingEnabled)
			{
				TraceWrapper.SearchLibraryTracer.Unregister();
			}
			if (this.directoryContext != null)
			{
				if (this.acquiredDirectoryContext)
				{
					this.directoryContext.Yield();
				}
				if (this.directoryContext.TrackingBudget != null)
				{
					this.directoryContext.TrackingBudget.Dispose();
				}
				if (this.directoryContext.ClientContext != null)
				{
					this.directoryContext.ClientContext.Dispose();
				}
			}
		}

		protected void AddError(TrackingError trackingError)
		{
			if (!TrackingErrorCollection.IsNullOrEmpty(this.errors))
			{
				this.errors.Errors.Add(trackingError);
			}
		}

		protected string[] GetDiagnosticsToTransmit()
		{
			string[] result = null;
			if (this.directoryContext != null && this.directoryContext.DiagnosticsContext != null)
			{
				result = this.directoryContext.DiagnosticsContext.Serialize();
			}
			return result;
		}

		protected void LogRequestStatus(bool succeeded)
		{
			List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>();
			list.Add(new KeyValuePair<string, object>("CallerIdentity", this.CallerIdentityForLogging()));
			list.Add(new KeyValuePair<string, object>("ResolvedDomain", this.Domain ?? string.Empty));
			list.Add(new KeyValuePair<string, object>("Succeeded", succeeded.ToString(CultureInfo.InvariantCulture)));
			if (base.Request != null)
			{
				((IMessageTrackingRequestLogInformation)((object)base.Request)).AddRequestDataForLogging(list);
			}
			CommonDiagnosticsLog.Instance.LogEvent(CommonDiagnosticsLog.Source.DeliveryReports, list);
		}

		private static void GetSecurityDescriptors(IEwsBudget callerBudget, out RawSecurityDescriptor trackingSecurityDescriptor, out RawSecurityDescriptor localServerSecurityDescriptor)
		{
			if (!BaseMessageTrackingReport<RequestType, SingleItemType>.securityDescriptorInitialized)
			{
				lock (BaseMessageTrackingReport<RequestType, SingleItemType>.initLock)
				{
					if (!BaseMessageTrackingReport<RequestType, SingleItemType>.securityDescriptorInitialized)
					{
						try
						{
							BaseMessageTrackingReport<RequestType, SingleItemType>.trackingSecurityDescriptor = BaseMessageTrackingReport<RequestType, SingleItemType>.CreateSecurityDescriptor(callerBudget);
							BaseMessageTrackingReport<RequestType, SingleItemType>.localServerSecurityDescriptor = BaseMessageTrackingReport<RequestType, SingleItemType>.GetLocalCasSecurityDescriptor(callerBudget);
							BaseMessageTrackingReport<RequestType, SingleItemType>.securityDescriptorInitialized = true;
						}
						catch (TransientException arg)
						{
							ExTraceGlobals.WebServiceTracer.TraceError<TransientException>(0L, "TransientException when initializing Security Descriptor: {0}", arg);
							throw new MessageTrackingTransientException();
						}
						catch (DataSourceOperationException arg2)
						{
							ExTraceGlobals.WebServiceTracer.TraceError<DataSourceOperationException>(0L, "DataSourceOperationException when initializing Security Descriptor: {0}", arg2);
							throw new MessageTrackingFatalException();
						}
						catch (DataValidationException arg3)
						{
							ExTraceGlobals.WebServiceTracer.TraceError<DataValidationException>(0L, "DataValidationException when initializing Security Descriptor: {0}", arg3);
							throw new MessageTrackingFatalException();
						}
					}
				}
			}
			trackingSecurityDescriptor = BaseMessageTrackingReport<RequestType, SingleItemType>.trackingSecurityDescriptor;
			localServerSecurityDescriptor = BaseMessageTrackingReport<RequestType, SingleItemType>.localServerSecurityDescriptor;
		}

		private static RawSecurityDescriptor GetLocalCasSecurityDescriptor(IEwsBudget callerBudget)
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 365, "GetLocalCasSecurityDescriptor", "f:\\15.00.1497\\sources\\dev\\services\\src\\Core\\servicecommands\\BaseMessageTrackingReport.cs");
			Server server = topologyConfigurationSession.ReadLocalServer();
			if (server == null)
			{
				ExTraceGlobals.WebServiceTracer.TraceError(0L, "Cannot read local server");
				throw new MessageTrackingFatalException();
			}
			RawSecurityDescriptor rawSecurityDescriptor = topologyConfigurationSession.ReadSecurityDescriptor(server.Id);
			if (rawSecurityDescriptor == null)
			{
				ExTraceGlobals.WebServiceTracer.TraceError<ADObjectId>(0L, "Cannot read security descriptor for server with id: {0}", server.Id);
				throw new MessageTrackingFatalException();
			}
			return rawSecurityDescriptor;
		}

		private static OrganizationId GetOrganizationId(string domain, IEwsBudget callerBudget)
		{
			ExTraceGlobals.WebServiceTracer.TraceDebug<string>(0L, "Trying to get OrgId for domain: {0}", domain);
			OrganizationId organizationId = null;
			if (!VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
			{
				return OrganizationId.ForestWideOrgId;
			}
			if (ServerCache.Instance.TryGetOrganizationId(domain, out organizationId))
			{
				return organizationId;
			}
			try
			{
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromTenantAcceptedDomain(domain), 424, "GetOrganizationId", "f:\\15.00.1497\\sources\\dev\\services\\src\\Core\\servicecommands\\BaseMessageTrackingReport.cs");
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, AcceptedDomainSchema.DomainName, domain);
				ADPagedReader<AcceptedDomain> adpagedReader = tenantOrTopologyConfigurationSession.FindPaged<AcceptedDomain>(null, QueryScope.SubTree, filter, null, 0);
				if (adpagedReader == null)
				{
					ExTraceGlobals.WebServiceTracer.TraceError(0L, "Domain not found in AD");
					throw new ServiceArgumentException(CoreResources.IDs.ErrorMessageTrackingNoSuchDomain);
				}
				int num = 0;
				foreach (AcceptedDomain acceptedDomain in adpagedReader)
				{
					num++;
					if (num > 1)
					{
						ExTraceGlobals.WebServiceTracer.TraceError(0L, "Multiple domains found in AD");
						throw new ServiceArgumentException(CoreResources.IDs.ErrorMessageTrackingPermanentError);
					}
					organizationId = acceptedDomain.OrganizationId;
				}
				if (organizationId == null)
				{
					ExTraceGlobals.WebServiceTracer.TraceError(0L, "Domain was not found in AD");
					throw new ServiceArgumentException(CoreResources.IDs.ErrorMessageTrackingNoSuchDomain);
				}
			}
			catch (CannotResolveTenantNameException)
			{
				ExTraceGlobals.WebServiceTracer.TraceError(0L, "Domain not found in AD");
				throw new ServiceArgumentException(CoreResources.IDs.ErrorMessageTrackingNoSuchDomain);
			}
			ExTraceGlobals.WebServiceTracer.TraceDebug<string, OrganizationId>(0L, "Domain {0} mapped to Org {1}", domain, organizationId);
			return organizationId;
		}

		private static RawSecurityDescriptor CreateSecurityDescriptor(IEwsBudget callerBudget)
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 487, "CreateSecurityDescriptor", "f:\\15.00.1497\\sources\\dev\\services\\src\\Core\\servicecommands\\BaseMessageTrackingReport.cs");
			ADGroup adgroup = tenantOrRootOrgRecipientSession.ResolveWellKnownGuid<ADGroup>(WellKnownGuid.ExSWkGuid, ADSession.GetConfigurationNamingContextForLocalForest());
			if (adgroup == null)
			{
				ExTraceGlobals.WebServiceTracer.TraceError(0L, "Could not get Exchange Servers Group");
				throw new MessageTrackingFatalException();
			}
			SecurityIdentifier sid = adgroup.Sid;
			ADGroup adgroup2 = tenantOrRootOrgRecipientSession.ResolveWellKnownGuid<ADGroup>(WellKnownGuid.EoaWkGuid, ADSession.GetConfigurationNamingContextForLocalForest());
			if (adgroup == null)
			{
				ExTraceGlobals.WebServiceTracer.TraceError(0L, "Could not get Exchange Org Admins Group");
				throw new MessageTrackingFatalException();
			}
			SecurityIdentifier sid2 = adgroup2.Sid;
			ActiveDirectorySecurity activeDirectorySecurity = new ActiveDirectorySecurity();
			SecurityIdentifier identity = new SecurityIdentifier(WellKnownSidType.NetworkServiceSid, null);
			ActiveDirectoryAccessRule rule = new ActiveDirectoryAccessRule(identity, ActiveDirectoryRights.ReadProperty, AccessControlType.Allow);
			activeDirectorySecurity.AddAccessRule(rule);
			rule = new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ReadProperty, AccessControlType.Allow, ActiveDirectorySecurityInheritance.All);
			activeDirectorySecurity.AddAccessRule(rule);
			rule = new ActiveDirectoryAccessRule(sid2, ActiveDirectoryRights.ReadProperty, AccessControlType.Allow, ActiveDirectorySecurityInheritance.All);
			activeDirectorySecurity.AddAccessRule(rule);
			return new RawSecurityDescriptor(activeDirectorySecurity.GetSecurityDescriptorBinaryForm(), 0)
			{
				Owner = new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null)
			};
		}

		private string CallerIdentityForLogging()
		{
			if (base.CallContext != null)
			{
				if (base.CallContext.IsExternalUser)
				{
					ExternalCallContext externalCallContext = (ExternalCallContext)base.CallContext;
					return "External:" + externalCallContext.EmailAddress;
				}
				if (base.CallContext.OriginalCallerContext.Sid != null)
				{
					return "CallerSid:" + base.CallContext.OriginalCallerContext.Sid.ToString();
				}
			}
			return "CallContextNull";
		}

		private void LoadOrganization()
		{
			OrganizationId organizationId = BaseMessageTrackingReport<RequestType, SingleItemType>.GetOrganizationId(this.Domain, base.CallerBudget);
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId), 596, "LoadOrganization", "f:\\15.00.1497\\sources\\dev\\services\\src\\Core\\servicecommands\\BaseMessageTrackingReport.cs");
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId), 600, "LoadOrganization", "f:\\15.00.1497\\sources\\dev\\services\\src\\Core\\servicecommands\\BaseMessageTrackingReport.cs");
			ITopologyConfigurationSession globalConfigSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 609, "LoadOrganization", "f:\\15.00.1497\\sources\\dev\\services\\src\\Core\\servicecommands\\BaseMessageTrackingReport.cs");
			ClientContext clientContext;
			try
			{
				ExternalCallContext externalCallContext = base.CallContext as ExternalCallContext;
				if (externalCallContext != null)
				{
					clientContext = ClientContext.Create(externalCallContext.EmailAddress, externalCallContext.ExternalId, externalCallContext.WSSecurityHeader, externalCallContext.SharingSecurityHeader, externalCallContext.Budget, null, EWSSettings.ClientCulture);
				}
				else
				{
					clientContext = ClientContext.Create(base.CallContext.EffectiveCaller.ClientSecurityContext, base.CallerBudget, null, EWSSettings.ClientCulture);
				}
			}
			catch (AuthzException arg)
			{
				ExTraceGlobals.WebServiceTracer.TraceError<AuthzException>((long)this.GetHashCode(), "Exception while creating client context: {0}", arg);
				throw new MessageTrackingFatalException();
			}
			this.errors = new TrackingErrorCollection();
			this.directoryContext = new DirectoryContext(clientContext, organizationId, globalConfigSession, tenantOrTopologyConfigurationSession, tenantOrRootOrgRecipientSession, null, this.diagnosticsLevelEnum, this.errors, true);
			this.directoryContext.Acquire();
			this.acquiredDirectoryContext = true;
		}

		private void CheckAccess()
		{
			if (!base.CallContext.IsExternalUser)
			{
				SecurityIdentifier sid = base.CallContext.OriginalCallerContext.Sid;
				if (sid != null)
				{
					RawSecurityDescriptor rawSecurityDescriptor;
					RawSecurityDescriptor rawSecurityDescriptor2;
					BaseMessageTrackingReport<RequestType, SingleItemType>.GetSecurityDescriptors(base.CallerBudget, out rawSecurityDescriptor, out rawSecurityDescriptor2);
					bool flag = false;
					try
					{
						flag = AuthzAuthorization.CheckSingleExtendedRight(sid, rawSecurityDescriptor2, WellKnownGuid.TokenSerializationRightGuid);
					}
					catch (Win32Exception arg)
					{
						ExTraceGlobals.WebServiceTracer.TraceError<SecurityIdentifier, Win32Exception>((long)this.GetHashCode(), "Failed call to CheckSingleExtendedRight for Caller Sid: {0} with Win32Exception: {1}", sid, arg);
						throw new MessageTrackingFatalException();
					}
					if (!flag)
					{
						ExTraceGlobals.WebServiceTracer.TraceError<SecurityIdentifier>((long)this.GetHashCode(), "Caller Sid: {0} was denied access based on server's Security Descriptor, trying local SD.", sid);
						if ((16 & AuthzAuthorization.CheckGenericPermission(sid, rawSecurityDescriptor, AccessMask.ReadProp)) != 16)
						{
							ExTraceGlobals.WebServiceTracer.TraceError<SecurityIdentifier>((long)this.GetHashCode(), "Caller Sid: {0} was denied access based on the tracking Security Descriptor", sid);
							throw new ServiceAccessDeniedException();
						}
					}
					ExTraceGlobals.WebServiceTracer.TraceDebug<SecurityIdentifier>((long)this.GetHashCode(), "Allowed access for {0} based on Security Descriptor", sid);
					return;
				}
				ExTraceGlobals.WebServiceTracer.TraceError((long)this.GetHashCode(), "Caller denied access, did not match security descriptor or organization relationship table");
				throw new ServiceAccessDeniedException();
			}
			else
			{
				ExternalCallContext externalCallContext = (ExternalCallContext)base.CallContext;
				SmtpAddress emailAddress = externalCallContext.EmailAddress;
				if (!emailAddress.IsValidAddress)
				{
					ExTraceGlobals.WebServiceTracer.TraceError<SmtpAddress>((long)this.GetHashCode(), "Caller SMTP address invalid: {0}", emailAddress);
					throw new ServiceAccessDeniedException();
				}
				if (!ServerCache.Instance.IsRemoteTrustedOrg(this.directoryContext.OrganizationId, emailAddress.Domain))
				{
					ExTraceGlobals.WebServiceTracer.TraceError<SmtpAddress>((long)this.GetHashCode(), "Caller {0} is denied access based on organization relationship", emailAddress);
					throw new ServiceAccessDeniedException();
				}
				ExTraceGlobals.WebServiceTracer.TraceError<SmtpAddress>((long)this.GetHashCode(), "Caller {0} allowed access based on organization relationship", emailAddress);
				return;
			}
		}

		private void AfterCheckAccess()
		{
			bool flag = this.AllowDebugMode();
			this.InitializeTracing(flag);
			TimeSpan timeout = this.GetTimeout(!flag);
			this.directoryContext.TrackingBudget = new TrackingEventBudget(this.errors, timeout);
		}

		private bool AllowDebugMode()
		{
			bool isExternalUser = base.CallContext.IsExternalUser;
			string text = string.Empty;
			if (isExternalUser)
			{
				text = ((ExternalCallContext)base.CallContext).EmailAddress.Domain;
			}
			return VariantConfiguration.InvariantNoFlightingSnapshot.MessageTracking.AllowDebugMode.Enabled || !isExternalUser || text.EndsWith(".microsoft.com", StringComparison.OrdinalIgnoreCase) || text.Equals("microsoft.com", StringComparison.OrdinalIgnoreCase);
		}

		private void InitializeTracing(bool allowDebugMode)
		{
			if (!allowDebugMode && this.diagnosticsLevelEnum >= Microsoft.Exchange.Transport.Logging.Search.DiagnosticsLevel.Etw)
			{
				this.diagnosticsLevelEnum = Microsoft.Exchange.Transport.Logging.Search.DiagnosticsLevel.Verbose;
			}
			if (this.diagnosticsLevelEnum == Microsoft.Exchange.Transport.Logging.Search.DiagnosticsLevel.Etw)
			{
				CommonDiagnosticsLogTracer traceWriter = new CommonDiagnosticsLogTracer();
				TraceWrapper.SearchLibraryTracer.Register(traceWriter);
				this.perThreadTracingEnabled = true;
			}
		}

		private TimeSpan GetTimeout(bool enforceMaximumLimit)
		{
			TimeSpan timeSpan = Constants.DefaultServerSideTimeout;
			string empty = string.Empty;
			if (this.extendedProperties == null || this.extendedProperties.Timeout == null)
			{
				ExTraceGlobals.WebServiceTracer.TraceDebug((long)this.GetHashCode(), "No timeout specified in request, using default");
				return Constants.DefaultServerSideTimeout;
			}
			timeSpan = this.extendedProperties.Timeout.Value;
			if (timeSpan > Constants.DefaultServerSideTimeout && enforceMaximumLimit)
			{
				ExTraceGlobals.WebServiceTracer.TraceError<double>((long)this.GetHashCode(), "Client requested timeout of {0} ms which is above the maximum limit they are authorized for", timeSpan.TotalMilliseconds);
				timeSpan = Constants.DefaultServerSideTimeout;
			}
			ExTraceGlobals.WebServiceTracer.TraceError<double>((long)this.GetHashCode(), "Using client requested timeout of {0} ms", timeSpan.TotalMilliseconds);
			return timeSpan;
		}

		private static object initLock = new object();

		private static RawSecurityDescriptor trackingSecurityDescriptor;

		private static RawSecurityDescriptor localServerSecurityDescriptor;

		private static bool securityDescriptorInitialized;

		protected DirectoryContext directoryContext;

		protected TrackingExtendedProperties extendedProperties;

		protected TrackingErrorCollection errors = TrackingErrorCollection.Empty;

		private DiagnosticsLevel diagnosticsLevelEnum;

		private bool perThreadTracingEnabled;

		private bool acquiredDirectoryContext;
	}
}
