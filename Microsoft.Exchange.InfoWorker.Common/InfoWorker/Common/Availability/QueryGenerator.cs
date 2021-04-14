using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Authentication;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.RequestDispatch;
using Microsoft.Exchange.InfoWorker.EventLog;
using Microsoft.Exchange.Net.WSTrust;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class QueryGenerator
	{
		public QueryGenerator(Application application, ClientContext clientContext, RequestLogger requestLogger, RequestDispatcher requestDispatcher, DateTime queryPrepareDeadline, DateTime requestCompletionDeadline, IList<RecipientData> recipientQueryResults)
		{
			this.application = application;
			this.clientContext = clientContext;
			this.requestLogger = requestLogger;
			this.requestDispatcher = requestDispatcher;
			this.queryPrepareDeadline = queryPrepareDeadline;
			this.requestCompletionDeadline = requestCompletionDeadline;
			this.recipientQueryResults = recipientQueryResults;
		}

		public int UniqueQueriesCount
		{
			get
			{
				return this.individualMailboxesProcessed;
			}
		}

		public BaseQuery[] GetQueries()
		{
			this.requestLogger.CaptureRequestStage("PreGetQueries");
			BaseQuery[] queries = new BaseQuery[this.recipientQueryResults.Count];
			QueryGenerator.ProcessRecipientsInOrder(this.recipientQueryResults, delegate(int i)
			{
				queries[i] = this.GetQuery(this.recipientQueryResults[i]);
			});
			this.requestLogger.CaptureRequestStage("PostGetQueries");
			return queries;
		}

		public BaseQuery GetQuery(RecipientData recipientData)
		{
			QueryGenerator.RequestRoutingTracer.TraceDebug<object, RecipientData>((long)this.GetHashCode(), "{0}: Creating a query for attendee {1}.", TraceContext.Get(), recipientData);
			if (recipientData.Exception != null)
			{
				return this.application.CreateFromUnknown(recipientData, (LocalizedException)recipientData.Exception);
			}
			if (DateTime.UtcNow > this.queryPrepareDeadline)
			{
				return this.HandleTimeoutException(recipientData, "GetQueryBegin");
			}
			if (!recipientData.IsEmpty)
			{
				return this.GenerateRequestsForOneRecipient(recipientData);
			}
			QueryGenerator.RequestRoutingTracer.TraceDebug<object, EmailAddress, string>((long)this.GetHashCode(), "{0}: Cannot resolve email address: {1}, in the AD, RecipientData is empty, Requester: {2}.", TraceContext.Get(), recipientData.EmailAddress, (this.Requester == null) ? "<NULL>" : this.Requester.ToString());
			if (this.ExternalAuthentication.Enabled && this.application.OfferForExternalSharing != null && ProxyAddressPrefix.Smtp.PrimaryPrefix.Equals(recipientData.EmailAddress.RoutingType, StringComparison.OrdinalIgnoreCase) && SmtpAddress.IsValidSmtpAddress(recipientData.EmailAddress.Address) && this.Requester != null)
			{
				QueryGenerator.RequestRoutingTracer.TraceDebug<object, EmailAddress>((long)this.GetHashCode(), "{0}: Cannot resolve email address: {1}, checking if this is sharing has been setup in the mailbox.", TraceContext.Get(), recipientData.EmailAddress);
				return this.GenerateRequestForExternalUser(recipientData);
			}
			return this.application.CreateFromUnknown(recipientData, new MailRecipientNotFoundException(Strings.descCannotResolveEmailAddress(recipientData.EmailAddress.ToString()), 43324U));
		}

		private ADUser Requester
		{
			get
			{
				if (this.requester == null)
				{
					this.requester = this.GetRequester();
				}
				return this.requester;
			}
		}

		private SharingReader SharingReader
		{
			get
			{
				if (this.sharingReader == null)
				{
					this.sharingReader = new SharingReader(this.Requester, this.clientContext.Budget, this.queryPrepareDeadline, this.application.SupportsPersonalSharingRelationship);
				}
				return this.sharingReader;
			}
		}

		private static void ProcessRecipientsInOrder(IList<RecipientData> recipientQueryResults, QueryGenerator.ProcessRecipientDelegate processRecipientDelegate)
		{
			List<int>[] array = new List<int>[5];
			for (int i = 0; i < recipientQueryResults.Count; i++)
			{
				RecipientData recipientData = recipientQueryResults[i];
				int num;
				if (recipientData.IsEmpty)
				{
					num = 4;
				}
				else if (recipientData.Exception == null)
				{
					switch (recipientData.RecipientType)
					{
					case RecipientType.UserMailbox:
						num = 0;
						goto IL_5E;
					case RecipientType.MailUser:
						num = 1;
						goto IL_5E;
					case RecipientType.MailContact:
						num = 2;
						goto IL_5E;
					}
					num = 3;
				}
				else
				{
					num = 0;
				}
				IL_5E:
				if (array[num] == null)
				{
					array[num] = new List<int>();
				}
				array[num].Add(i);
			}
			foreach (List<int> list in array)
			{
				if (list != null)
				{
					foreach (int i2 in list)
					{
						processRecipientDelegate(i2);
					}
				}
			}
		}

		private static bool CheckSharingPartnerIdentities(ExchangePrincipal mailboxPrincipal, ExternalClientContext externalClientContext)
		{
			string domain = externalClientContext.EmailAddress.Domain;
			if (FreeBusyPermission.GetOrganizationRelationship(mailboxPrincipal.MailboxInfo.OrganizationId, domain) != null)
			{
				QueryGenerator.RequestRoutingTracer.TraceDebug<object, string>(0L, "{0}: Sharing relationship for domain {1} is found and enabled. Skip checking for SharingPartnerIdentities.", TraceContext.Get(), domain);
				return true;
			}
			PersonalClientContext personalClientContext = externalClientContext as PersonalClientContext;
			return personalClientContext != null && DirectoryHelper.HasSharingPartnership(mailboxPrincipal.MailboxInfo.MailboxGuid, mailboxPrincipal.MailboxInfo.IsArchive, personalClientContext.ExternalId.ToString(), mailboxPrincipal.MailboxInfo.OrganizationId.ToADSessionSettings().CreateRecipientSession(null));
		}

		private BaseQuery GenerateRequestsForOneRecipient(RecipientData recipientData)
		{
			if (this.individualMailboxesProcessed >= Configuration.MaximumIdentityArraySize)
			{
				QueryGenerator.RequestRoutingTracer.TraceError<object, string, int>((long)this.GetHashCode(), "{0}: The specified mailbox {1} could not be processed because the service already processed {2} individual mailboxes.", TraceContext.Get(), recipientData.EmailAddress.ToString(), this.individualMailboxesProcessed);
				return this.application.CreateFromUnknown(recipientData, new IndividualMailboxLimitReachedException(Strings.descIndividualMailboxLimitReached(recipientData.EmailAddress.ToString(), this.individualMailboxesProcessed), 59708U));
			}
			LocalizedException ex = null;
			try
			{
				if (recipientData.IsDistributionGroup)
				{
					if (this.application.ShouldProcessGroup)
					{
						return this.GenerateRequestsForGroup(recipientData);
					}
					QueryGenerator.RequestRoutingTracer.TraceError<object, string>((long)this.GetHashCode(), "{0}: Email address {1} is a group, but the application does not support groups", TraceContext.Get(), recipientData.EmailAddress.ToString());
					ex = new MailRecipientNotFoundException(Strings.descNotAContactOrUser(recipientData.EmailAddress.ToString()), 35132U);
				}
				else
				{
					if (recipientData.IsIndividual)
					{
						return this.GenerateRequestsForIndividual(recipientData);
					}
					QueryGenerator.RequestRoutingTracer.TraceError<object, string>((long)this.GetHashCode(), "{0}: Email address {1} does not represent an AD group, user or contact", TraceContext.Get(), recipientData.EmailAddress.ToString());
					ex = new MailRecipientNotFoundException(Strings.descNotAContactOrUser(recipientData.EmailAddress.ToString()), 51516U);
				}
			}
			catch (DataValidationException innerException)
			{
				ex = new MailRecipientNotFoundException(innerException, 45372U);
			}
			catch (ArgumentException innerException2)
			{
				ex = new MailRecipientNotFoundException(innerException2, 61756U);
			}
			catch (FormatException innerException3)
			{
				ex = new MailRecipientNotFoundException(innerException3, 37180U);
			}
			catch (LocalizedException ex2)
			{
				ex = ex2;
				ErrorHandler.SetErrorCodeIfNecessary(ex2, ErrorConstants.FreeBusyGenerationFailed);
			}
			QueryGenerator.RequestRoutingTracer.TraceError<object, string, LocalizedException>((long)this.GetHashCode(), "{0}: Failed to create query for recipient {1}. Exception {2}", TraceContext.Get(), recipientData.EmailAddress.ToString(), ex);
			return this.application.CreateFromUnknown(recipientData, ex);
		}

		private BaseQuery GenerateRequestsForGroup(RecipientData recipientData)
		{
			QueryGenerator.DistributionListHandlingTracer.TraceDebug((long)this.GetHashCode(), "{0}: Entering QueryGenerator.GenerateRequestsForGroup()", new object[]
			{
				TraceContext.Get()
			});
			RecipientQueryResults recipientQueryResults = (RecipientQueryResults)this.recipientQueryResults;
			FreeBusyRecipientQuery freeBusyRecipientQuery = (FreeBusyRecipientQuery)recipientQueryResults.RecipientQuery;
			bool groupCapped;
			List<RecipientData> list = freeBusyRecipientQuery.ExpandDistributionGroup(recipientData, this.queryPrepareDeadline, out groupCapped);
			QueryGenerator.DistributionListHandlingTracer.TraceDebug<object, int, EmailAddress>((long)this.GetHashCode(), "{0}: Found {1} members for group {2}.", TraceContext.Get(), list.Count, recipientData.EmailAddress);
			QueryGenerator.DistributionListHandlingTracer.TraceDebug<object, int, EmailAddress>((long)this.GetHashCode(), "{0}: Building requests for {1} recipients expanded from group {2}", TraceContext.Get(), list.Count, recipientData.EmailAddress);
			BaseQuery[] groupQuery = this.GetGroupQuery(list);
			QueryGenerator.DistributionListHandlingTracer.TraceDebug((long)this.GetHashCode(), "{0}: Total of {1} requests built for members of group {2}. {3} of {4} are duplicates.", new object[]
			{
				TraceContext.Get(),
				groupQuery.Length,
				recipientData.EmailAddress,
				this.individualMailboxesProcessed,
				list.Count
			});
			QueryGenerator.DistributionListHandlingTracer.TraceDebug((long)this.GetHashCode(), "{0}: Leaving QueryGenerator.GenerateRequestsForGroup()", new object[]
			{
				TraceContext.Get()
			});
			return this.application.CreateFromGroup(recipientData, groupQuery, groupCapped);
		}

		private BaseQuery[] GetGroupQuery(List<RecipientData> recipientList)
		{
			QueryList queryList = new QueryList(recipientList.Count);
			foreach (RecipientData recipientData in recipientList)
			{
				queryList.Add(this.GetGroupMemberQuery(recipientData));
			}
			return queryList.ToArray();
		}

		private BaseQuery GetGroupMemberQuery(RecipientData recipientData)
		{
			EmailAddress emailAddress = new EmailAddress(recipientData.DisplayName, recipientData.PrimarySmtpAddress.ToString(), ProxyAddressPrefix.Smtp.PrimaryPrefix);
			if (DateTime.UtcNow > this.queryPrepareDeadline)
			{
				return this.HandleTimeoutException(recipientData, "GetGroupMemberQuery:Begin");
			}
			if (this.individualMailboxesProcessed >= Configuration.MaximumIdentityArraySize)
			{
				QueryGenerator.DistributionListHandlingTracer.TraceError<object, EmailAddress, int>((long)this.GetHashCode(), "{0}: The specified mailbox {1} could not be processed because the service already processed {2} individual mailboxes.", TraceContext.Get(), emailAddress, this.individualMailboxesProcessed);
				return this.application.CreateFromIndividual(recipientData, new IndividualMailboxLimitReachedException(Strings.descIndividualMailboxLimitReached(emailAddress.ToString(), this.individualMailboxesProcessed), 53564U));
			}
			return this.GenerateRequestsForIndividual(recipientData);
		}

		private BaseQuery GenerateRequestsForIndividual(RecipientData recipientData)
		{
			string text = recipientData.PrimarySmtpAddress.ToString();
			BaseQuery baseQuery;
			if (this.uniqueQueries.TryGetValue(text, out baseQuery))
			{
				QueryGenerator.RequestRoutingTracer.TraceDebug<object, string>((long)this.GetHashCode(), "{0}: Query for recipient {1} already exists, using it instead.", TraceContext.Get(), text);
			}
			else
			{
				baseQuery = this.GenerateRequestsForUniqueIndividual(recipientData);
				this.uniqueQueries.Add(text, baseQuery);
			}
			return baseQuery;
		}

		private BaseQuery GenerateRequestsForUniqueIndividual(RecipientData recipientData)
		{
			recipientData.EmailAddress = new EmailAddress(recipientData.DisplayName, recipientData.PrimarySmtpAddress.ToString(), ProxyAddressPrefix.Smtp.PrimaryPrefix);
			QueryGenerator.RequestRoutingTracer.TraceDebug((long)this.GetHashCode(), "{0}: Email address {1} was successfully resolved. Primary SMTP Address = {2}, Legacy Exchange DN = {3}", new object[]
			{
				TraceContext.Get(),
				recipientData,
				recipientData.PrimarySmtpAddress,
				recipientData.LegacyExchangeDN
			});
			this.individualMailboxesProcessed++;
			switch (recipientData.RecipientType)
			{
			case RecipientType.UserMailbox:
				return this.HandleIntraForestMailbox(recipientData);
			case RecipientType.MailUser:
			case RecipientType.MailContact:
				return this.HandleRemoteMailbox(recipientData);
			}
			QueryGenerator.RequestRoutingTracer.TraceError<object, RecipientData>((long)this.GetHashCode(), "{0}: Email address {1} does not represent a mail-enabled contact, user or mailbox user.", TraceContext.Get(), recipientData);
			return this.application.CreateFromIndividual(recipientData, new MailRecipientNotFoundException(Strings.descNotAContactOrUser(recipientData.EmailAddress.ToString()), 41276U));
		}

		private ADUser GetRequester()
		{
			InternalClientContext internalClientContext = this.clientContext as InternalClientContext;
			if (internalClientContext == null)
			{
				QueryGenerator.RequestRoutingTracer.TraceError((long)this.GetHashCode(), "{0}: Cannot check for external recipients because caller is not internal identity.", new object[]
				{
					TraceContext.Get()
				});
				return null;
			}
			ADUser aduser = internalClientContext.ADUser;
			if (aduser == null)
			{
				QueryGenerator.RequestRoutingTracer.TraceError<object, string>((long)this.GetHashCode(), "{0}: Cannot get the ADUser for the requester {1}.", TraceContext.Get(), internalClientContext.ToString());
			}
			return aduser;
		}

		private BaseQuery GenerateRequestForExternalUser(RecipientData recipientData)
		{
			BaseQuery baseQuery = null;
			if (this.uniqueQueries.TryGetValue(recipientData.EmailAddress.Address, out baseQuery))
			{
				QueryGenerator.RequestRoutingTracer.TraceDebug<object, string>((long)this.GetHashCode(), "{0}: Query for external recipient {1} already exists, using it instead.", TraceContext.Get(), recipientData.EmailAddress.Address);
			}
			else
			{
				SharingInformation sharingInformation = this.SharingReader.Read(recipientData.EmailAddress, this.application);
				if (sharingInformation == null)
				{
					QueryGenerator.RequestRoutingTracer.TraceError((long)this.GetHashCode(), "{0}: No sharing information is available.", new object[]
					{
						TraceContext.Get()
					});
					return this.application.CreateFromUnknown(recipientData, new MailRecipientNotFoundException(Strings.descCannotResolveEmailAddress(recipientData.EmailAddress.ToString()), 57660U));
				}
				if (sharingInformation.Exception != null)
				{
					baseQuery = this.HandleException(recipientData, sharingInformation.Exception, 33084U);
				}
				else if (sharingInformation.TargetSharingEpr != null && !LocalizedString.Empty.Equals(sharingInformation.TargetSharingEpr.AutodiscoverFailedExceptionString))
				{
					QueryGenerator.RequestRoutingTracer.TraceDebug<object, string, RecipientData>((long)this.GetHashCode(), "{0}: Cached autodiscover exception with localized string ID {1} was found for recipient {2}, using it.", TraceContext.Get(), sharingInformation.TargetSharingEpr.AutodiscoverFailedExceptionString.StringId, recipientData);
					baseQuery = this.application.CreateFromIndividual(recipientData, new AutoDiscoverFailedException(sharingInformation.TargetSharingEpr.AutodiscoverFailedExceptionString, 49468U));
				}
				else
				{
					baseQuery = this.GenerateRequestForExternalUserInternal(recipientData, sharingInformation);
				}
				this.uniqueQueries.Add(recipientData.EmailAddress.Address, baseQuery);
			}
			return baseQuery;
		}

		private BaseQuery GenerateRequestForExternalUserInternal(RecipientData recipientData, SharingInformation sharingInformation)
		{
			QueryGenerator.RequestRoutingTracer.TraceDebug<object, bool>((long)this.GetHashCode(), "{0}: sharingInformation.IsFromIntraOrgConnector is set to {1}", TraceContext.Get(), sharingInformation.IsFromIntraOrgConnector);
			if (sharingInformation.TargetSharingEpr == null)
			{
				QueryGenerator.RequestRoutingTracer.TraceDebug<object, RecipientData>((long)this.GetHashCode(), "{0}: Found a organization relationship without TargetSharingEpr, generating a autodiscover request for the external user.", TraceContext.Get(), recipientData);
				if (!sharingInformation.IsFromIntraOrgConnector)
				{
					return this.PrepareProxyRequestForExternalRecipientWithAutodiscover(recipientData, sharingInformation);
				}
				return this.PrepareProxyRequestForExternalRecipientWithAutodiscoverByOAuth(recipientData, sharingInformation);
			}
			else
			{
				QueryGenerator.RequestRoutingTracer.TraceDebug<object, RecipientData>((long)this.GetHashCode(), "{0}: Found a organization relationship, generating a request for the external user.", TraceContext.Get(), recipientData);
				int serverVersion = sharingInformation.TargetSharingEpr.ServerVersion;
				if (!this.application.IsVersionSupportedForExternalUser(serverVersion))
				{
					QueryGenerator.RequestRoutingTracer.TraceError<object, int, Type>((long)this.GetHashCode(), "{0}: Remote server version {1} is considered a legacy server by {2} application for external user.", TraceContext.Get(), serverVersion, this.application.GetType());
					return this.application.CreateFromIndividual(recipientData, this.application.CreateExceptionForUnsupportedVersion(recipientData, serverVersion));
				}
				if (!sharingInformation.IsFromIntraOrgConnector)
				{
					return this.PrepareProxyRequestForExternalRecipient(recipientData, sharingInformation);
				}
				return this.PrepareProxyRequestForExternalRecipientByOAuth(recipientData, sharingInformation);
			}
		}

		private BaseQuery HandleIntraForestMailbox(RecipientData recipientData)
		{
			TimeSpan timeSpan = this.queryPrepareDeadline - DateTime.UtcNow;
			if (timeSpan <= TimeSpan.Zero)
			{
				return this.HandleTimeoutException(recipientData, "HandleIntraForestMailbox");
			}
			ExchangePrincipal exchangePrincipal = recipientData.ExchangePrincipal;
			if (exchangePrincipal == null)
			{
				QueryGenerator.RequestRoutingTracer.TraceError<object, RecipientData>((long)this.GetHashCode(), "{0}: Email address {1} could not be resolved to an ExchangePrincipal object.", TraceContext.Get(), recipientData);
				return this.application.CreateFromIndividual(recipientData, new MailRecipientNotFoundException(Strings.descNotAValidExchangePrincipal(recipientData.EmailAddress.ToString()), 48956U));
			}
			if (exchangePrincipal.MailboxInfo.MailboxDatabase.IsNullOrEmpty())
			{
				QueryGenerator.RequestRoutingTracer.TraceError<object, RecipientData>((long)this.GetHashCode(), "{0}: Could not find a database for the email address {1}.", TraceContext.Get(), recipientData);
				return this.application.CreateFromIndividual(recipientData, new MailRecipientNotFoundException(Strings.descNotAValidExchangePrincipal(recipientData.EmailAddress.ToString()), 65340U));
			}
			QueryGenerator.RequestRoutingTracer.TraceDebug((long)this.GetHashCode(), "{0}: Email address {1} was successfully resolved to an Exchange principal={2}. Server Legacy DN = {3}", new object[]
			{
				TraceContext.Get(),
				recipientData,
				exchangePrincipal,
				exchangePrincipal.MailboxInfo.Location.ServerLegacyDn
			});
			timeSpan = this.queryPrepareDeadline - DateTime.UtcNow;
			if (timeSpan <= TimeSpan.Zero)
			{
				return this.HandleTimeoutException(recipientData, "HandleIntraForestMailbox:EP");
			}
			ExternalClientContext externalClientContext = this.clientContext as ExternalClientContext;
			if (externalClientContext != null)
			{
				if (!QueryGenerator.CheckSharingPartnerIdentities(exchangePrincipal, externalClientContext))
				{
					QueryGenerator.RequestRoutingTracer.TraceDebug<object, RecipientData>((long)this.GetHashCode(), "{0}: No mailbox data will be returned for mailbox {1} since never granted access to caller.", TraceContext.Get(), recipientData);
					return this.application.CreateFromIndividual(recipientData, new NoFreeBusyAccessException(40764U));
				}
				timeSpan = this.queryPrepareDeadline - DateTime.UtcNow;
				if (timeSpan <= TimeSpan.Zero)
				{
					return this.HandleTimeoutException(recipientData, "HandleIntraForestMailbox:ECC");
				}
			}
			RequestType requestType = RequestType.CrossSite;
			BaseQuery result;
			try
			{
				ServerVersion serverVersion = new ServerVersion(exchangePrincipal.MailboxInfo.Location.ServerVersion);
				if (!this.application.IsVersionSupported(serverVersion.ToInt()))
				{
					QueryGenerator.RequestRoutingTracer.TraceError<object, ServerVersion, Type>((long)this.GetHashCode(), "{0}: Recipient server version {1} is not supported by {2} application.", TraceContext.Get(), serverVersion, this.application.GetType());
					result = this.application.CreateFromIndividual(recipientData, this.application.CreateExceptionForUnsupportedVersion(recipientData, serverVersion.ToInt()));
				}
				else
				{
					QueryGenerator.RequestRoutingTracer.TraceDebug<object, ServerVersion>((long)this.GetHashCode(), "{0}: Looking for CAS server that matches the recipient server version of {1}.", TraceContext.Get(), serverVersion);
					bool flag = false;
					Uri uri = null;
					int num = -1;
					Stopwatch stopwatch = Stopwatch.StartNew();
					if (exchangePrincipal.MailboxInfo.Location.ServerVersion < Globals.E15Version)
					{
						bool enabled = VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled;
						QueryGenerator.RequestRoutingTracer.TraceDebug<object, string>((long)this.GetHashCode(), "{0}: Using ServiceTopology.{1}", TraceContext.Get(), enabled ? "GetCurrentLegacyServiceTopology" : "GetCurrentServiceTopology");
						ServiceTopology serviceTopology = enabled ? ServiceTopology.GetCurrentLegacyServiceTopology(timeSpan, "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\RequestDispatch\\QueryGenerator.cs", "HandleIntraForestMailbox", 936) : ServiceTopology.GetCurrentServiceTopology(timeSpan, "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\RequestDispatch\\QueryGenerator.cs", "HandleIntraForestMailbox", 937);
						IList<WebServicesService> list = serviceTopology.FindAll<WebServicesService>(exchangePrincipal, ClientAccessType.InternalNLBBypass, "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\RequestDispatch\\QueryGenerator.cs", "HandleIntraForestMailbox", 938);
						using (IEnumerator<WebServicesService> enumerator = list.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								WebServicesService webServicesService = enumerator.Current;
								if (webServicesService.IsOutOfService)
								{
									QueryGenerator.RequestRoutingTracer.TraceDebug<object, string>((long)this.GetHashCode(), "{0}: Service {1} is out-of-service. Skipping this CAS.", TraceContext.Get(), webServicesService.ServerDistinguishedName);
								}
								else
								{
									ServerVersion serverVersion2 = new ServerVersion(webServicesService.ServerVersionNumber);
									if (serverVersion.Major == serverVersion2.Major)
									{
										uri = webServicesService.Url;
										num = webServicesService.ServerVersionNumber;
										QueryGenerator.RequestRoutingTracer.TraceDebug((long)this.GetHashCode(), "{0}: Found a CAS server {1} that matches the recipient {2} server version {3}.", new object[]
										{
											TraceContext.Get(),
											webServicesService.ServerFullyQualifiedDomainName,
											recipientData,
											serverVersion2.Major
										});
										break;
									}
								}
							}
							goto IL_44A;
						}
					}
					flag = StringComparer.OrdinalIgnoreCase.Equals(LocalServerCache.LocalServerFqdn, exchangePrincipal.MailboxInfo.Location.ServerFqdn);
					if (!flag)
					{
						uri = BackEndLocator.GetBackEndWebServicesUrl(exchangePrincipal.MailboxInfo);
						BackEndServer backEndServer = BackEndLocator.GetBackEndServer(exchangePrincipal.MailboxInfo);
						num = backEndServer.Version;
						QueryGenerator.RequestRoutingTracer.TraceDebug<object, RecipientData, Uri>((long)this.GetHashCode(), "{0}: Found a BE server that matches the recipient {1} - uri = {2}.", TraceContext.Get(), recipientData, uri);
					}
					IL_44A:
					stopwatch.Stop();
					if (flag)
					{
						QueryGenerator.RequestRoutingTracer.TraceDebug<object, RecipientData, string>((long)this.GetHashCode(), "{0}: Request for {1} is found to be a local request to {2}.", TraceContext.Get(), recipientData, exchangePrincipal.MailboxInfo.Location.ServerFqdn);
						BaseQuery baseQuery = this.PrepareIntraSiteCalendarRequest(recipientData);
						baseQuery.ServiceDiscoveryLatency = stopwatch.ElapsedMilliseconds;
						result = baseQuery;
					}
					else
					{
						BaseQuery baseQuery;
						if (uri == null)
						{
							QueryGenerator.RequestRoutingTracer.TraceDebug<object, RecipientData>((long)this.GetHashCode(), "{0}: Unable to find a target server to process the request for {1}.", TraceContext.Get(), recipientData);
							Globals.AvailabilityLogger.LogEvent(InfoWorkerEventLogConstants.Tuple_NoCASFoundForRequest, null, new object[]
							{
								Globals.ProcessId,
								recipientData.EmailAddress
							});
							baseQuery = this.HandleServiceDiscoveryException(recipientData, null);
						}
						else
						{
							WebServiceUri webServiceUri = new WebServiceUri(new UriBuilder(uri)
							{
								Scheme = (Configuration.UseSSLForCrossSiteRequests ? "https" : "http")
							}.Uri.OriginalString, "EXCH", UriSource.Directory, num);
							ServerVersion serverVersion3 = new ServerVersion(num);
							QueryGenerator.RequestRoutingTracer.TraceDebug<object, RecipientData, Uri>((long)this.GetHashCode(), "{0}: Request for {1} is being proxied to {2}", TraceContext.Get(), recipientData, webServiceUri.Uri);
							TargetServerVersion targetVersion = TargetServerVersion.Unknown;
							if (serverVersion3.Major >= QueryGenerator.e15Version.Major)
							{
								targetVersion = TargetServerVersion.E15;
							}
							else if (serverVersion3.Major == QueryGenerator.e14R3Version.Major)
							{
								targetVersion = TargetServerVersion.E14R3OrLater;
							}
							else if (serverVersion3.Major == QueryGenerator.e12Version.Major)
							{
								targetVersion = TargetServerVersion.E12;
							}
							if (recipientData.AssociatedFolderId != null)
							{
								baseQuery = this.PrepareGetFolderAndProxyRequest(recipientData, requestType, null, webServiceUri, true, targetVersion);
							}
							else
							{
								baseQuery = this.PrepareProxyRequest(recipientData, requestType, null, webServiceUri, true, UriSource.Directory, targetVersion);
							}
						}
						baseQuery.ServiceDiscoveryLatency = stopwatch.ElapsedMilliseconds;
						result = baseQuery;
					}
				}
			}
			catch (ServiceDiscoveryTransientException e)
			{
				result = this.HandleServiceDiscoveryException(recipientData, e);
			}
			catch (ServiceDiscoveryPermanentException e2)
			{
				result = this.HandleServiceDiscoveryException(recipientData, e2);
			}
			catch (ADTransientException e3)
			{
				result = this.HandleServiceDiscoveryException(recipientData, e3);
			}
			catch (DataSourceOperationException e4)
			{
				result = this.HandleServiceDiscoveryException(recipientData, e4);
			}
			catch (DataValidationException e5)
			{
				result = this.HandleServiceDiscoveryException(recipientData, e5);
			}
			return result;
		}

		private BaseQuery HandleRemoteMailbox(RecipientData recipientData)
		{
			if (this.application.OfferForExternalSharing != null)
			{
				QueryGenerator.RequestRoutingTracer.TraceDebug<object, RecipientData>((long)this.GetHashCode(), "{0}: External Authentication has been enabled, searching for a organization relationship for {1}.", TraceContext.Get(), recipientData);
				SharingInformation sharingInformation = null;
				if (recipientData.ExternalEmailAddress != null && recipientData.ExternalEmailAddress.GetType() != typeof(InvalidProxyAddress))
				{
					SmtpProxyAddress smtpProxyAddress = recipientData.ExternalEmailAddress as SmtpProxyAddress;
					if (smtpProxyAddress != null)
					{
						EmailAddress emailAddress = new EmailAddress(smtpProxyAddress.SmtpAddress, smtpProxyAddress.SmtpAddress);
						sharingInformation = this.SharingReader.Read(emailAddress, this.application);
						if (sharingInformation != null)
						{
							QueryGenerator.RequestRoutingTracer.TraceDebug<object, EmailAddress, RecipientData>((long)this.GetHashCode(), "{0}: using external email address {1} instead of {2} because organization relationship was for the former address.", TraceContext.Get(), emailAddress, recipientData);
							recipientData.EmailAddress = emailAddress;
						}
					}
					else
					{
						QueryGenerator.RequestRoutingTracer.TraceDebug<object, RecipientData>((long)this.GetHashCode(), "{0}: Not checking organization relationship for external email address for {1} because it is not SMTP address.", TraceContext.Get(), recipientData);
					}
				}
				else
				{
					QueryGenerator.RequestRoutingTracer.TraceDebug<object, RecipientData>((long)this.GetHashCode(), "{0}: Not checking organization relationship for external email address for {1} because it is not SMTP address or it is not present.", TraceContext.Get(), recipientData);
				}
				if (sharingInformation == null)
				{
					sharingInformation = this.SharingReader.Read(recipientData.EmailAddress, this.application);
				}
				if (sharingInformation != null)
				{
					if (sharingInformation.Exception != null)
					{
						QueryGenerator.RequestRoutingTracer.TraceError<object, RecipientData, Exception>((long)this.GetHashCode(), "{0}: Failed to get sharing information for recipient {1}, exception {2}.", TraceContext.Get(), recipientData, sharingInformation.Exception);
						return this.HandleException(recipientData, sharingInformation.Exception, 57148U);
					}
					if (sharingInformation.TargetSharingEpr != null && !LocalizedString.Empty.Equals(sharingInformation.TargetSharingEpr.AutodiscoverFailedExceptionString))
					{
						QueryGenerator.RequestRoutingTracer.TraceDebug<object, LocalizedString, RecipientData>((long)this.GetHashCode(), "{0}: Cached autodiscover failed exception with localized string ID {1} was found for recipient {2}, using it.", TraceContext.Get(), sharingInformation.TargetSharingEpr.AutodiscoverFailedExceptionString, recipientData);
						return this.application.CreateFromIndividual(recipientData, new AutoDiscoverFailedException(sharingInformation.TargetSharingEpr.AutodiscoverFailedExceptionString, 44860U));
					}
					QueryGenerator.RequestRoutingTracer.TraceDebug<object, RecipientData>((long)this.GetHashCode(), "{0}: Found a organization relationship, generating a request for the external user {1}.", TraceContext.Get(), recipientData);
					if (sharingInformation.IsFromIntraOrgConnector || this.ExternalAuthentication.Enabled)
					{
						return this.GenerateRequestForExternalUserInternal(recipientData, sharingInformation);
					}
				}
			}
			return this.HandleCrossForestMailbox(recipientData);
		}

		private BaseQuery HandleCrossForestMailbox(RecipientData recipientData)
		{
			TargetForestConfiguration targetForestConfiguration = null;
			string domain;
			if (recipientData.ExternalEmailAddress != null && recipientData.ExternalEmailAddress.GetType() != typeof(InvalidProxyAddress))
			{
				QueryGenerator.RequestRoutingTracer.TraceDebug<object, string>((long)this.GetHashCode(), "{0}: Attempting to get the address space for address {1}.", TraceContext.Get(), recipientData.ExternalEmailAddress.AddressString);
				recipientData.EmailAddress = new EmailAddress(recipientData.EmailAddress.Name, recipientData.ExternalEmailAddress.AddressString);
				domain = recipientData.EmailAddress.Domain;
				if (!string.IsNullOrEmpty(domain))
				{
					TimeSpan t = this.queryPrepareDeadline - DateTime.UtcNow;
					if (t <= TimeSpan.Zero)
					{
						return this.application.CreateFromIndividual(recipientData, new TimeoutExpiredException("Handling-Cross-Forest-Mailbox"));
					}
					targetForestConfiguration = this.GetTargetForestConfiguration(recipientData.EmailAddress);
				}
				else
				{
					QueryGenerator.RequestRoutingTracer.TraceError<object, RecipientData>((long)this.GetHashCode(), "{0}: Invalid target domain for mailbox {1}.", TraceContext.Get(), recipientData);
				}
			}
			else
			{
				QueryGenerator.RequestRoutingTracer.TraceDebug<object, RecipientData>((long)this.GetHashCode(), "{0}: Target address for user {1} is invalid.", TraceContext.Get(), recipientData);
			}
			if (targetForestConfiguration == null || targetForestConfiguration.Exception != null)
			{
				QueryGenerator.RequestRoutingTracer.TraceDebug<object, RecipientData>((long)this.GetHashCode(), "{0}: Unable to get forest configuration using target address trying the primary smtp address for user {1}.", TraceContext.Get(), recipientData);
				if (recipientData.PrimarySmtpAddress.IsValidAddress)
				{
					recipientData.EmailAddress = new EmailAddress(recipientData.EmailAddress.Name, recipientData.PrimarySmtpAddress.ToString());
					TimeSpan t2 = this.queryPrepareDeadline - DateTime.UtcNow;
					if (t2 <= TimeSpan.Zero)
					{
						return this.application.CreateFromIndividual(recipientData, new TimeoutExpiredException("Handling-Cross-Forest-Mailbox-Target-Configuration-Lookup"));
					}
					targetForestConfiguration = this.GetTargetForestConfiguration(recipientData.EmailAddress);
				}
				else
				{
					QueryGenerator.RequestRoutingTracer.TraceDebug<object, RecipientData>((long)this.GetHashCode(), "{0}: Primary SMTP address for user {1} is invalid.", TraceContext.Get(), recipientData);
				}
				if (targetForestConfiguration.Exception != null)
				{
					QueryGenerator.RequestRoutingTracer.TraceError<object, RecipientData>((long)this.GetHashCode(), "{0}: Failed to discover Availability Service for user {1} using both the target address and primary SMTP address.", TraceContext.Get(), recipientData);
					return this.application.CreateFromIndividual(recipientData, new ProxyWebRequestProcessingException(Strings.descInvalidConfigForCrossForestRequest(recipientData.EmailAddress.ToString()), targetForestConfiguration.Exception));
				}
			}
			domain = recipientData.EmailAddress.Domain;
			if (targetForestConfiguration.AccessMethod == AvailabilityAccessMethod.PublicFolder)
			{
				QueryGenerator.RequestRoutingTracer.TraceError<object, string>((long)this.GetHashCode(), "{0}: AccessMethod of PublicFolder is not supported in E15. Failing the request to {1}.", TraceContext.Get(), domain);
				return this.application.CreateFromIndividual(recipientData, new ProxyWebRequestProcessingException(Strings.descPFNotSupported(recipientData.EmailAddress.ToString())));
			}
			QueryGenerator.RequestRoutingTracer.TraceDebug<object, RecipientData, string>((long)this.GetHashCode(), "{0}: {1} is found to be a cross-forest mailbox. Discovering Availability service in forest {2}", TraceContext.Get(), recipientData, domain);
			int autodiscoverVersionBucket = this.application.GetAutodiscoverVersionBucket(AutodiscoverType.Internal);
			WebServiceUri webServiceUri = RemoteServiceUriCache.Get(recipientData.EmailAddress, autodiscoverVersionBucket);
			if (webServiceUri == null)
			{
				if (recipientData.AssociatedFolderId != null)
				{
					return this.PrepareGetFolderAndProxyRequestWithAutoDiscover(recipientData, targetForestConfiguration);
				}
				return this.PrepareProxyRequestWithAutoDiscover(recipientData, targetForestConfiguration);
			}
			else
			{
				if (!LocalizedString.Empty.Equals(webServiceUri.AutodiscoverFailedExceptionString))
				{
					QueryGenerator.RequestRoutingTracer.TraceDebug<object, RecipientData, string>((long)this.GetHashCode(), "{0}: Found bad URI cache entry for mailbox {1} with localized string ID {2}.", TraceContext.Get(), recipientData, webServiceUri.AutodiscoverFailedExceptionString.StringId);
					return this.application.CreateFromIndividual(recipientData, new AutoDiscoverFailedException(webServiceUri.AutodiscoverFailedExceptionString, 61244U));
				}
				if (!this.application.IsVersionSupported(webServiceUri.ServerVersion))
				{
					QueryGenerator.RequestRoutingTracer.TraceError<object, int, Type>((long)this.GetHashCode(), "{0}: Remote server version {1} is considered a legacy server by {2} application", TraceContext.Get(), webServiceUri.ServerVersion, this.application.GetType());
					return this.application.CreateFromIndividual(recipientData, this.application.CreateExceptionForUnsupportedVersion(recipientData, webServiceUri.ServerVersion));
				}
				if (webServiceUri.Uri == null)
				{
					QueryGenerator.RequestRoutingTracer.TraceError<object, RecipientData>((long)this.GetHashCode(), "{0}: Could not find a cross-forest availability service that can fill request for mailbox {1}.", TraceContext.Get(), recipientData);
					return this.application.CreateFromIndividual(recipientData, new AutoDiscoverFailedException(Strings.descCrossForestServiceMissing(recipientData.EmailAddress.ToString()), 36668U));
				}
				return this.PrepareCrossForestProxyRequest(recipientData, webServiceUri, targetForestConfiguration);
			}
		}

		private TargetForestConfiguration GetTargetForestConfiguration(EmailAddress emailAddress)
		{
			string domain = emailAddress.Domain;
			TargetForestConfiguration targetForestConfiguration = null;
			try
			{
				targetForestConfiguration = TargetForestConfigurationCache.FindByDomain(this.Requester.OrganizationId, domain);
			}
			catch (AddressSpaceNotFoundException exception)
			{
				QueryGenerator.RequestRoutingTracer.TraceError<object, EmailAddress, string>((long)this.GetHashCode(), "{0}: Failed to discover Availability service for mailbox {1} because address space could not be found matching forest {2}.", TraceContext.Get(), emailAddress, domain);
				return new TargetForestConfiguration("NotFound", domain, exception);
			}
			if (targetForestConfiguration.Exception != null)
			{
				QueryGenerator.RequestRoutingTracer.TraceError<object, EmailAddress, LocalizedException>((long)this.GetHashCode(), "{0}: Failed to discover Availability service for mailbox {1} because AvailabilityAddressSpace is misconfigured. Exception: {2}", TraceContext.Get(), emailAddress, targetForestConfiguration.Exception);
			}
			return targetForestConfiguration;
		}

		private BaseQuery PrepareCrossForestProxyRequest(RecipientData recipientData, WebServiceUri remoteUri, TargetForestConfiguration forestConfig)
		{
			if (remoteUri.EmailAddress != null)
			{
				recipientData.EmailAddress = remoteUri.EmailAddress;
			}
			if (recipientData.AssociatedFolderId != null)
			{
				return this.PrepareGetFolderAndProxyRequest(recipientData, RequestType.CrossForest, remoteUri.Credentials, remoteUri, forestConfig.IsPerUserAuthorizationSupported, TargetServerVersion.Unknown);
			}
			return this.PrepareProxyRequest(recipientData, RequestType.CrossForest, remoteUri.Credentials, remoteUri, forestConfig.IsPerUserAuthorizationSupported, remoteUri.Source, TargetServerVersion.Unknown);
		}

		private BaseQuery PrepareGetFolderAndProxyRequest(RecipientData recipientData, RequestType requestType, NetworkCredential credentials, WebServiceUri webServiceUri, bool sendRequesterToken, TargetServerVersion targetVersion)
		{
			string key = "GetFolderAndProxyRequest:" + requestType.ToString() + webServiceUri.Uri;
			BaseQuery baseQuery = this.application.CreateFromIndividual(recipientData);
			this.requestDispatcher.Add(key, baseQuery, requestType, delegate(QueryList queryList)
			{
				InternalClientContext internalClientContext = (InternalClientContext)this.clientContext;
				ProxyAuthenticator proxyAuthenticator = ProxyAuthenticator.Create(credentials, sendRequesterToken ? internalClientContext.SerializedSecurityContext : null, this.clientContext.MessageId);
				return new GetFolderAndProxyRequest(this.application, internalClientContext, requestType, this.requestLogger, queryList, targetVersion, proxyAuthenticator, webServiceUri);
			});
			return baseQuery;
		}

		private BaseQuery PrepareProxyRequest(RecipientData recipientData, RequestType requestType, NetworkCredential credentials, WebServiceUri webServiceUri, bool sendRequesterToken, UriSource source, TargetServerVersion targetVersion)
		{
			if (this.clientContext is PersonalClientContext && (targetVersion == TargetServerVersion.E12 || targetVersion == TargetServerVersion.Unknown))
			{
				QueryGenerator.RequestRoutingTracer.TraceError<object, RecipientData, Uri>((long)this.GetHashCode(), "{0}: Failing the proxy request because we can't proxy a p-p token to E12 user {1}, url {2}.", TraceContext.Get(), recipientData, webServiceUri.Uri);
				return this.application.CreateFromIndividual(recipientData, new ProxyNotAllowedForPersonalRelationship(recipientData.EmailAddress));
			}
			string key = "ProxyWebRequest:" + requestType.ToString() + webServiceUri.Uri;
			BaseQuery baseQuery = this.application.CreateFromIndividual(recipientData);
			this.requestDispatcher.Add(key, baseQuery, requestType, delegate(QueryList queryList)
			{
				InternalClientContext internalClientContext = this.clientContext as InternalClientContext;
				ProxyAuthenticator proxyAuthenticator;
				if (internalClientContext != null)
				{
					proxyAuthenticator = ProxyAuthenticator.Create(credentials, sendRequesterToken ? internalClientContext.SerializedSecurityContext : null, this.clientContext.MessageId);
				}
				else if (targetVersion == TargetServerVersion.E12 || targetVersion == TargetServerVersion.Unknown)
				{
					proxyAuthenticator = ProxyAuthenticator.Create(null, null, this.clientContext.MessageId);
				}
				else
				{
					ExternalClientContext externalClientContext = this.clientContext as ExternalClientContext;
					proxyAuthenticator = externalClientContext.CreateInternalProxyAuthenticator();
				}
				return new ProxyWebRequest(this.application, this.clientContext, requestType, this.requestLogger, queryList, targetVersion, proxyAuthenticator, webServiceUri, source);
			});
			return baseQuery;
		}

		private BaseQuery PrepareProxyRequestForExternalRecipient(RecipientData recipientData, SharingInformation sharingInformation)
		{
			string key = "ExternalProxyWebRequest:" + ((sharingInformation.SharingKey == SmtpAddress.Empty) ? sharingInformation.TargetSharingEpr.Uri.ToString() : sharingInformation.SharingKey.ToString());
			BaseQuery baseQuery = this.application.CreateFromIndividual(recipientData);
			this.requestDispatcher.Add(key, baseQuery, RequestType.FederatedCrossForest, delegate(QueryList queryList)
			{
				ExternalAuthenticationRequest externalAuthenticationRequest = this.CreateExternalAuthenticationRequest(sharingInformation, this.application.OfferForExternalSharing);
				return new ExternalProxyWebRequest(this.application, this.clientContext, this.requestLogger, queryList, externalAuthenticationRequest, sharingInformation.TargetSharingEpr, sharingInformation.SharingKey);
			});
			return baseQuery;
		}

		private BaseQuery PrepareProxyRequestForExternalRecipientByOAuth(RecipientData recipientData, SharingInformation sharingInformation)
		{
			string key = "ExternalByOAuthProxyWebRequest:" + sharingInformation.TargetSharingEpr.Uri.ToString();
			BaseQuery baseQuery = this.application.CreateFromIndividual(recipientData);
			this.requestDispatcher.Add(key, baseQuery, RequestType.FederatedCrossForest, (QueryList queryList) => new ExternalByOAuthProxyWebRequest(this.application, this.clientContext, this.requestLogger, queryList, sharingInformation.TargetSharingEpr));
			return baseQuery;
		}

		private BaseQuery PrepareGetFolderAndProxyRequestWithAutoDiscover(RecipientData recipientData, TargetForestConfiguration targetForestConfiguration)
		{
			string key = "GetFolderAndProxyRequestWithAutoDiscover:" + targetForestConfiguration.FullDnsDomainName;
			BaseQuery baseQuery = this.application.CreateFromIndividual(recipientData);
			this.requestDispatcher.Add(key, baseQuery, RequestType.CrossForest, (QueryList queryList) => new GetFolderAndProxyRequestWithAutoDiscover(this.application, (InternalClientContext)this.clientContext, this.requestLogger, queryList, targetForestConfiguration));
			return baseQuery;
		}

		private BaseQuery PrepareProxyRequestWithAutoDiscover(RecipientData recipientData, TargetForestConfiguration targetForestConfiguration)
		{
			bool remoteMailbox = !recipientData.IsEmpty && recipientData.IsRemoteMailbox;
			string key = "ProxyWebRequestWithAutoDiscover:" + targetForestConfiguration.FullDnsDomainName + (remoteMailbox ? "RemoteMailbox" : string.Empty);
			BaseQuery baseQuery = this.application.CreateFromIndividual(recipientData);
			this.requestDispatcher.Add(key, baseQuery, RequestType.CrossForest, delegate(QueryList queryList)
			{
				if (remoteMailbox)
				{
					QueryGenerator.RequestRoutingTracer.TraceDebug<object, RecipientData>((long)this.GetHashCode(), "{0}: Remote mailbox {1} detected, using user based cross forest autodiscover.", TraceContext.Get(), recipientData);
					return this.application.CreateCrossForestAsyncRequestWithAutoDiscoverForRemoteMailbox((InternalClientContext)this.clientContext, this.requestLogger, queryList, targetForestConfiguration);
				}
				QueryGenerator.RequestRoutingTracer.TraceDebug<object, RecipientData>((long)this.GetHashCode(), "{0}: Remote non-mailbox {1} detected, using domain based cross forest autodiscover.", TraceContext.Get(), recipientData);
				return this.application.CreateCrossForestAsyncRequestWithAutoDiscover((InternalClientContext)this.clientContext, this.requestLogger, queryList, targetForestConfiguration);
			});
			return baseQuery;
		}

		private BaseQuery PrepareProxyRequestForExternalRecipientWithAutodiscover(RecipientData recipientData, SharingInformation sharingInformation)
		{
			bool remoteMailbox = !recipientData.IsEmpty && recipientData.IsRemoteMailbox;
			string key = "ExternalProxyWebRequestWithAutoDiscover:" + ((sharingInformation.SharingKey == SmtpAddress.Empty) ? recipientData.EmailAddress.Domain : sharingInformation.SharingKey.ToString()) + (remoteMailbox ? "RemoteMailbox" : string.Empty);
			BaseQuery baseQuery = this.application.CreateFromIndividual(recipientData);
			this.requestDispatcher.Add(key, baseQuery, RequestType.FederatedCrossForest, delegate(QueryList queryList)
			{
				ExternalAuthenticationRequest autoDiscoverExternalAuthenticationRequest = this.CreateExternalAuthenticationRequest(sharingInformation, Offer.Autodiscover);
				ExternalAuthenticationRequest webProxyExternalAuthenticationRequest = this.CreateExternalAuthenticationRequest(sharingInformation, this.application.OfferForExternalSharing);
				if (remoteMailbox)
				{
					QueryGenerator.RequestRoutingTracer.TraceDebug<object, RecipientData>((long)this.GetHashCode(), "{0}: Remote mailbox {1} detected, using user based external autodiscover.", TraceContext.Get(), recipientData);
					return this.application.CreateExternalAsyncRequestWithAutoDiscoverForRemoteMailbox((InternalClientContext)this.clientContext, this.requestLogger, queryList, autoDiscoverExternalAuthenticationRequest, webProxyExternalAuthenticationRequest, sharingInformation.TargetAutodiscoverEpr, sharingInformation.SharingKey);
				}
				QueryGenerator.RequestRoutingTracer.TraceDebug<object, RecipientData>((long)this.GetHashCode(), "{0}: Remote non-mailbox {1} detected, using domain based external autodiscover.", TraceContext.Get(), recipientData);
				return this.application.CreateExternalAsyncRequestWithAutoDiscover((InternalClientContext)this.clientContext, this.requestLogger, queryList, autoDiscoverExternalAuthenticationRequest, webProxyExternalAuthenticationRequest, sharingInformation.TargetAutodiscoverEpr, sharingInformation.SharingKey);
			});
			return baseQuery;
		}

		private BaseQuery PrepareProxyRequestForExternalRecipientWithAutodiscoverByOAuth(RecipientData recipientData, SharingInformation sharingInformation)
		{
			bool remoteMailbox = !recipientData.IsEmpty && recipientData.IsRemoteMailbox;
			string key = "ExternalByOAuthProxyWebRequestWithAutoDiscover:" + recipientData.EmailAddress.Domain + (remoteMailbox ? "RemoteMailbox" : string.Empty);
			BaseQuery baseQuery = this.application.CreateFromIndividual(recipientData);
			this.requestDispatcher.Add(key, baseQuery, RequestType.FederatedCrossForest, delegate(QueryList queryList)
			{
				if (remoteMailbox)
				{
					QueryGenerator.RequestRoutingTracer.TraceDebug<object, RecipientData>((long)this.GetHashCode(), "{0}: Remote mailbox {1} detected, using user based external autodiscover by oauth.", TraceContext.Get(), recipientData);
					return this.application.CreateExternalByOAuthAsyncRequestWithAutoDiscoverForRemoteMailbox((InternalClientContext)this.clientContext, this.requestLogger, queryList, sharingInformation.TargetAutodiscoverEpr);
				}
				QueryGenerator.RequestRoutingTracer.TraceDebug<object, RecipientData>((long)this.GetHashCode(), "{0}: Remote non-mailbox {1} detected, using domain based external autodiscover by oauth.", TraceContext.Get(), recipientData);
				return this.application.CreateExternalByOAuthAsyncRequestWithAutoDiscover((InternalClientContext)this.clientContext, this.requestLogger, queryList, sharingInformation.TargetAutodiscoverEpr);
			});
			return baseQuery;
		}

		private BaseQuery PrepareIntraSiteCalendarRequest(RecipientData recipientData)
		{
			BaseQuery baseQuery = this.application.CreateFromIndividual(recipientData);
			this.requestDispatcher.Add("Local", baseQuery, RequestType.Local, (QueryList queryList) => new LocalRequest(this.application, this.clientContext, this.requestLogger, queryList, this.requestCompletionDeadline));
			return baseQuery;
		}

		private ExternalAuthenticationRequest CreateExternalAuthenticationRequest(SharingInformation sharingInformation, Offer offer)
		{
			return new ExternalAuthenticationRequest(this.requestLogger, this.ExternalAuthentication, this.Requester, sharingInformation.RequestorSmtpAddress, sharingInformation.TokenTarget, offer);
		}

		private BaseQuery HandleServiceDiscoveryException(RecipientData recipientData, Exception e)
		{
			QueryGenerator.RequestRoutingTracer.TraceDebug<object, EmailAddress>((long)this.GetHashCode(), "{0}: Could not find a CAS server that can serve request for intra-forest mailbox {1}.", TraceContext.Get(), recipientData.EmailAddress);
			Globals.AvailabilityLogger.LogEvent(InfoWorkerEventLogConstants.Tuple_CASDiscoveryExceptionHandled, null, new object[]
			{
				Globals.ProcessId,
				recipientData.EmailAddress,
				e
			});
			return this.application.CreateFromIndividual(recipientData, new ServiceDiscoveryFailedException(Strings.descMissingIntraforestCAS(recipientData.EmailAddress.ToString()), e));
		}

		private BaseQuery HandleException(RecipientData recipientData, Exception e, uint locationIdentifier)
		{
			QueryGenerator.RequestRoutingTracer.TraceError<object, EmailAddress, Exception>((long)this.GetHashCode(), "{0}: Unable to lookup email adddress {1} - exception {2}.", TraceContext.Get(), recipientData.EmailAddress, e);
			return this.application.CreateFromIndividual(recipientData, new MailRecipientNotFoundException(e, locationIdentifier));
		}

		private BaseQuery HandleTimeoutException(RecipientData recipientData, string location)
		{
			QueryGenerator.RequestRoutingTracer.TraceError<object, RecipientData, string>((long)this.GetHashCode(), "{0}: Query for recipient {1} timed out at {2}.", TraceContext.Get(), recipientData, location);
			return this.application.CreateFromIndividual(recipientData, new TimeoutExpiredException(location));
		}

		private ExternalAuthentication ExternalAuthentication
		{
			get
			{
				if (this.externalAuthentication == null)
				{
					this.externalAuthentication = ExternalAuthentication.GetCurrent();
				}
				return this.externalAuthentication;
			}
		}

		private const string UriSchemeHttps = "https";

		private const string UriSchemeHttp = "http";

		private Application application;

		private ClientContext clientContext;

		private RequestLogger requestLogger;

		private RequestDispatcher requestDispatcher;

		private DateTime queryPrepareDeadline;

		private DateTime requestCompletionDeadline;

		private ExternalAuthentication externalAuthentication;

		private IList<RecipientData> recipientQueryResults;

		private Dictionary<string, BaseQuery> uniqueQueries = new Dictionary<string, BaseQuery>();

		private SharingReader sharingReader;

		private int individualMailboxesProcessed;

		private static readonly ServerVersion e12Version = new ServerVersion(8, 0, 0, 0);

		private static readonly ServerVersion e14R3Version = new ServerVersion(14, 0, 449, 0);

		private static readonly ServerVersion e15Version = new ServerVersion(15, 0, 0, 0);

		private ADUser requester;

		private static readonly Microsoft.Exchange.Diagnostics.Trace RequestRoutingTracer = ExTraceGlobals.RequestRoutingTracer;

		private static readonly Microsoft.Exchange.Diagnostics.Trace DistributionListHandlingTracer = ExTraceGlobals.DistributionListHandlingTracer;

		private delegate void ProcessRecipientDelegate(int i);
	}
}
