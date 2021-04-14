using System;
using System.ComponentModel;
using System.IO;
using Microsoft.Exchange.Data.ApplicationLogic.Performance;
using Microsoft.Exchange.Data.ApplicationLogic.UserPhotos;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;
using Microsoft.Exchange.Diagnostics.Performance;
using Microsoft.Exchange.InfoWorker.Common.Availability;

namespace Microsoft.Exchange.InfoWorker.Common.UserPhotos
{
	internal class UserPhotoLocalQuery : LocalQuery
	{
		internal UserPhotoLocalQuery(ClientContext clientContext, DateTime deadline, PhotoRequest photoRequest, PhotosConfiguration configuration, ITracer upstreamTracer) : base(clientContext, deadline)
		{
			this.photoRequest = photoRequest;
			this.tracer = ExTraceGlobals.UserPhotosTracer.Compose(upstreamTracer);
			this.upstreamTracer = upstreamTracer;
			this.photosConfiguration = configuration;
		}

		internal override BaseQueryResult GetData(BaseQuery query)
		{
			this.tracer.TraceDebug((long)this.GetHashCode(), "Executing local photo query.");
			UserPhotoQuery userPhotoQuery = (UserPhotoQuery)query;
			BaseQueryResult result;
			try
			{
				using (new StopwatchPerformanceTracker("LocalAuthorization", this.photoRequest.PerformanceLogger))
				{
					using (new ADPerformanceTracker("LocalAuthorization", this.photoRequest.PerformanceLogger))
					{
						new PhotoAuthorization(OrganizationIdCache.Singleton, this.upstreamTracer).Authorize(this.photoRequest.Requestor, new PhotoPrincipal
						{
							EmailAddresses = new string[]
							{
								userPhotoQuery.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString()
							},
							OrganizationId = userPhotoQuery.ExchangePrincipal.MailboxInfo.OrganizationId
						});
					}
				}
				using (MemoryStream memoryStream = new MemoryStream())
				{
					this.CheckDeadline("Retrieving-UserPhoto");
					LocalServerPhotoRetrievalPipeline localServerPhotoRetrievalPipeline = new LocalServerPhotoRetrievalPipeline(this.photosConfiguration, "Client=WebServices;Action=UserPhotoTask", this.CreateRecipientSession(userPhotoQuery), XSOFactory.Default, this.upstreamTracer);
					this.photoRequest.TargetSmtpAddress = userPhotoQuery.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString();
					this.photoRequest.TargetPrincipal = userPhotoQuery.ExchangePrincipal;
					PhotoResponse photoResponse = localServerPhotoRetrievalPipeline.Retrieve(this.photoRequest, memoryStream);
					this.CheckDeadline("Reading-Image-Bytes");
					byte[] array = memoryStream.ToArray();
					this.CheckDeadline("Returning-Result");
					this.tracer.TraceDebug((long)this.GetHashCode(), "Local query completed.  Returned photo is blank? {0};  Status: {1};  ETag: {2};  Content-type: '{3}'", new object[]
					{
						array == null || array.Length == 0,
						photoResponse.Status,
						photoResponse.ETag,
						photoResponse.ContentType
					});
					result = new UserPhotoQueryResult(array, photoResponse.ETag, photoResponse.Status, photoResponse.HttpExpiresHeader, photoResponse.ContentType, this.upstreamTracer);
				}
			}
			catch (IOException ex)
			{
				result = this.TraceAndReturnQueryResult(ex, new PhotoRetrievalFailedIOException(Strings.PhotoRetrievalFailedIOError(ex.Message), ex));
			}
			catch (Win32Exception ex2)
			{
				result = this.TraceAndReturnQueryResult(ex2, new PhotoRetrievalFailedWin32Exception(Strings.PhotoRetrievalFailedWin32Error(ex2.Message), ex2));
			}
			catch (UnauthorizedAccessException ex3)
			{
				result = this.TraceAndReturnQueryResult(ex3, new PhotoRetrievalFailedUnauthorizedAccessException(Strings.PhotoRetrievalFailedUnauthorizedAccessError(ex3.Message), ex3));
			}
			catch (AccessDeniedException ex4)
			{
				result = this.TraceAndReturnQueryResult(ex4, ex4);
			}
			catch (LocalizedException ex5)
			{
				result = this.TraceAndReturnQueryResult(ex5, ex5);
			}
			return result;
		}

		private void CheckDeadline(string requestState)
		{
			DateTime utcNow = DateTime.UtcNow;
			if (utcNow > this.deadline)
			{
				throw new TimeoutExpiredException(requestState);
			}
		}

		private UserPhotoQueryResult TraceAndReturnQueryResult(Exception caughtException, LocalizedException wrappingException)
		{
			this.tracer.TraceError<Exception>((long)this.GetHashCode(), "Local photo query failed with exception: {0}", caughtException);
			return new UserPhotoQueryResult(wrappingException, this.upstreamTracer);
		}

		private IRecipientSession CreateRecipientSession(UserPhotoQuery query)
		{
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, query.ExchangePrincipal.MailboxInfo.OrganizationId.ToADSessionSettings(), 179, "CreateRecipientSession", "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\UserPhotos\\UserPhotoLocalQuery.cs");
		}

		private readonly ITracer tracer;

		private readonly ITracer upstreamTracer;

		private readonly PhotoRequest photoRequest;

		private readonly PhotosConfiguration photosConfiguration;
	}
}
