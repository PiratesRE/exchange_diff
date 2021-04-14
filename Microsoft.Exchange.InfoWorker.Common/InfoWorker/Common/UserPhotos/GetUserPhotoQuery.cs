using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Performance;
using Microsoft.Exchange.Data.ApplicationLogic.UserPhotos;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;
using Microsoft.Exchange.Diagnostics.Performance;
using Microsoft.Exchange.InfoWorker.Common.Availability;

namespace Microsoft.Exchange.InfoWorker.Common.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GetUserPhotoQuery : Query<byte[]>
	{
		public GetUserPhotoQuery(ClientContext clientContext, PhotoRequest photoRequest, HttpResponse httpResponse, bool isRequestFromExternalOrganization, PhotosConfiguration configuration, ITracer upstreamTracer) : base(clientContext, httpResponse, CasTraceEventType.Ews, UserPhotoApplication.UserPhotoIOCompletion, UserPhotosPerfCounters.UserPhotosCurrentRequests)
		{
			ArgumentValidator.ThrowIfNull("photoRequest", photoRequest);
			ArgumentValidator.ThrowIfNull("configuration", configuration);
			this.tracer = ExTraceGlobals.UserPhotosTracer.Compose(upstreamTracer);
			this.upstreamTracer = upstreamTracer;
			this.photoRequest = photoRequest;
			this.isRequestFromExternalOrganization = isRequestFromExternalOrganization;
			this.photosConfiguration = configuration;
		}

		public string ETag { get; private set; }

		public HttpStatusCode StatusCode { get; private set; }

		public string Expires { get; private set; }

		public string ContentType { get; private set; }

		protected override void ValidateSpecificInputData()
		{
		}

		protected override byte[] ExecuteInternal()
		{
			UserPhotoApplication userPhotoApplication = new UserPhotoApplication(this.photoRequest, this.photosConfiguration, this.photoRequest.Trace, this.upstreamTracer);
			byte[] result;
			using (RequestDispatcher requestDispatcher = new RequestDispatcher(base.RequestLogger))
			{
				IList<RecipientData> list = this.LookupTargetUserInDirectory();
				if (list.Count == 0)
				{
					this.tracer.TraceDebug((long)this.GetHashCode(), "Target user not found in directory.");
					this.StatusCode = HttpStatusCode.NotFound;
					this.Expires = this.ComputeExpiresHeader(string.Empty, HttpStatusCode.NotFound);
					result = null;
				}
				else
				{
					RecipientData recipientData = list[0];
					this.tracer.TraceDebug<SmtpAddress>((long)this.GetHashCode(), "Target user found with PRIMARY SMTP address: '{0}'", recipientData.PrimarySmtpAddress);
					if (string.IsNullOrEmpty(this.photoRequest.TargetPrimarySmtpAddress) && recipientData.PrimarySmtpAddress.IsValidAddress && recipientData.PrimarySmtpAddress != SmtpAddress.Empty)
					{
						this.photoRequest.TargetPrimarySmtpAddress = recipientData.PrimarySmtpAddress.ToString();
					}
					QueryGenerator queryGenerator = new QueryGenerator(userPhotoApplication, base.ClientContext, base.RequestLogger, requestDispatcher, this.queryPrepareDeadline, this.requestProcessingDeadline, list);
					try
					{
						UserPhotoQuery userPhotoQuery = (UserPhotoQuery)queryGenerator.GetQueries()[0];
						requestDispatcher.Execute(this.requestProcessingDeadline, base.HttpResponse);
						this.individualMailboxesProcessed = queryGenerator.UniqueQueriesCount;
						if (userPhotoQuery.Result == null)
						{
							this.tracer.TraceError<ClientContext, string>((long)this.GetHashCode(), "Query result is NULL.  Client context: {0}; target: {1}", base.ClientContext, this.photoRequest.TargetSmtpAddress);
							result = null;
						}
						else
						{
							byte[] array = userPhotoQuery.Result.UserPhotoBytes;
							this.ETag = userPhotoQuery.Result.CacheId;
							this.StatusCode = userPhotoQuery.Result.StatusCode;
							this.Expires = this.ComputeExpiresHeader(userPhotoQuery.Result.Expires, userPhotoQuery.Result.StatusCode);
							this.ContentType = userPhotoQuery.Result.ContentType;
							if (userPhotoQuery.Result.ExceptionInfo != null)
							{
								this.tracer.TraceError<LocalizedException>((long)this.GetHashCode(), "Query result has an exception: {0}", userPhotoQuery.Result.ExceptionInfo);
								base.RequestLogger.AppendToLog<string>("EXP", userPhotoQuery.Result.ExceptionInfo.ToString());
							}
							array = this.FallbackToThumbnailIfNecessary(userPhotoQuery);
							userPhotoApplication.LogThreadsUsage(base.RequestLogger);
							result = array;
						}
					}
					catch (Exception arg)
					{
						this.tracer.TraceError<Exception>((long)this.GetHashCode(), "Exception at query dispatch: {0}", arg);
						throw;
					}
					finally
					{
						requestDispatcher.LogStatistics(base.RequestLogger);
					}
				}
			}
			return result;
		}

		protected override void UpdateCountersAtExecuteEnd(Stopwatch responseTimer)
		{
		}

		protected override void AppendSpecificSpExecuteOperationData(StringBuilder spOperationData)
		{
		}

		private IList<RecipientData> LookupTargetUserInDirectory()
		{
			IList<RecipientData> result;
			using (new StopwatchPerformanceTracker("QueryResolveTargetInDirectory", this.photoRequest.PerformanceLogger))
			{
				using (new ADPerformanceTracker("QueryResolveTargetInDirectory", this.photoRequest.PerformanceLogger))
				{
					Tuple<OrganizationId, ADObjectId> directoryLookupScope = this.GetDirectoryLookupScope();
					OrganizationId item = directoryLookupScope.Item1;
					ADObjectId item2 = directoryLookupScope.Item2;
					if (item == null)
					{
						this.tracer.TraceDebug((long)this.GetHashCode(), "Photo query: organization scope cannot be determined.");
						result = Array<RecipientData>.Empty;
					}
					else
					{
						this.tracer.TraceDebug<OrganizationId, ADObjectId>((long)this.GetHashCode(), "Photo query: looking up target user in directory using organization: '{0}';  base DN: '{1}'", item, item2);
						result = new UserPhotoRecipientQuery(base.ClientContext, item2, item, this.queryPrepareDeadline, this.upstreamTracer).Query(this.photoRequest.TargetSmtpAddress, this.photoRequest.TargetAdObjectId);
					}
				}
			}
			return result;
		}

		private Tuple<OrganizationId, ADObjectId> GetDirectoryLookupScope()
		{
			if (this.isRequestFromExternalOrganization)
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "Photo query: request comes from external organization.  Using target's organization id and base DN: NULL.");
				return new Tuple<OrganizationId, ADObjectId>(this.GetTargetOrganizationIdFromCache(), null);
			}
			return new Tuple<OrganizationId, ADObjectId>(base.ClientContext.OrganizationId, base.ClientContext.QueryBaseDN);
		}

		private OrganizationId GetTargetOrganizationIdFromCache()
		{
			return DomainToOrganizationIdCache.Singleton.Get(new SmtpDomain(SmtpAddress.Parse(this.photoRequest.TargetSmtpAddress).Domain));
		}

		private string ComputeExpiresHeader(string currentExpiresHeader, HttpStatusCode photoStatus)
		{
			if (!string.IsNullOrEmpty(currentExpiresHeader))
			{
				return currentExpiresHeader;
			}
			return UserAgentPhotoExpiresHeader.Default.ComputeExpiresHeader(DateTime.UtcNow, photoStatus, this.photosConfiguration);
		}

		private byte[] FallbackToThumbnailIfNecessary(UserPhotoQuery query)
		{
			HttpStatusCode statusCode = query.Result.StatusCode;
			if (statusCode <= HttpStatusCode.NotModified)
			{
				if (statusCode != HttpStatusCode.OK && statusCode != HttpStatusCode.NotModified)
				{
					goto IL_78;
				}
			}
			else
			{
				switch (statusCode)
				{
				case HttpStatusCode.Forbidden:
					break;
				case HttpStatusCode.NotFound:
					goto IL_78;
				default:
					if (statusCode != HttpStatusCode.InternalServerError)
					{
						goto IL_78;
					}
					goto IL_78;
				}
			}
			if (query.Result.IsPhotoServedFromADFallback)
			{
				this.photoRequest.PerformanceLogger.Log("ADFallbackPhotoServed", string.Empty, 1U);
			}
			return query.Result.UserPhotoBytes;
			IL_78:
			if (query.RecipientData == null || query.RecipientData.ThumbnailPhoto == null || query.RecipientData.ThumbnailPhoto.Length == 0)
			{
				this.tracer.TraceDebug<HttpStatusCode>((long)this.GetHashCode(), "Photo query: fall-back to thumbnail photo (AD) not possible: no photo.  Returning status code: NOT FOUND.  Original status code: {0}", this.StatusCode);
				this.photoRequest.PerformanceLogger.Log("ADFallbackPhotoServed", string.Empty, 0U);
				this.StatusCode = HttpStatusCode.NotFound;
				return query.Result.UserPhotoBytes;
			}
			this.tracer.TraceDebug((long)this.GetHashCode(), "Photo query: fall-back to thumbnail photo (AD) successful.");
			this.photoRequest.PerformanceLogger.Log("ADFallbackPhotoServed", string.Empty, 1U);
			this.StatusCode = HttpStatusCode.OK;
			this.ETag = null;
			this.Expires = this.ComputeExpiresHeader(null, HttpStatusCode.OK);
			return query.RecipientData.ThumbnailPhoto;
		}

		private const string NoExpiresHeader = null;

		private readonly ITracer tracer;

		private readonly ITracer upstreamTracer;

		private readonly PhotoRequest photoRequest;

		private readonly bool isRequestFromExternalOrganization;

		private readonly PhotosConfiguration photosConfiguration;
	}
}
