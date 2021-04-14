using System;
using System.IO;
using System.Net;
using Microsoft.Exchange.Data.ApplicationLogic.Performance;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Diagnostics.Performance;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ADPhotoHandler : IPhotoHandler
	{
		public ADPhotoHandler(IADPhotoReader reader, IRecipientSession recipientSession, ITracer upstreamTracer)
		{
			ArgumentValidator.ThrowIfNull("reader", reader);
			ArgumentValidator.ThrowIfNull("recipientSession", recipientSession);
			ArgumentValidator.ThrowIfNull("upstreamTracer", upstreamTracer);
			this.tracer = ExTraceGlobals.UserPhotosTracer.Compose(upstreamTracer);
			this.reader = reader;
			this.recipientSession = recipientSession;
		}

		public PhotoResponse Retrieve(PhotoRequest request, PhotoResponse response)
		{
			PhotoResponse result;
			using (new StopwatchPerformanceTracker("ADHandlerTotal", request.PerformanceLogger))
			{
				using (new ADPerformanceTracker("ADHandlerTotal", request.PerformanceLogger))
				{
					if (request.ShouldSkipHandlers(PhotoHandlers.ActiveDirectory))
					{
						this.tracer.TraceDebug((long)this.GetHashCode(), "AD HANDLER: skipped by request.");
						result = response;
					}
					else if (response.Served)
					{
						this.tracer.TraceDebug((long)this.GetHashCode(), "AD HANDLER: skipped because photo has already been served by an upstream handler.");
						result = response;
					}
					else
					{
						response.ADHandlerProcessed = true;
						request.PerformanceLogger.Log("ADHandlerProcessed", string.Empty, 1U);
						try
						{
							result = this.FindTargetAndReadPhoto(request, response);
						}
						catch (ADNoSuchObjectException arg)
						{
							this.tracer.TraceDebug<ADNoSuchObjectException>((long)this.GetHashCode(), "AD HANDLER: no photo.  Exception: {0}", arg);
							request.PerformanceLogger.Log("ADHandlerPhotoAvailable", string.Empty, 0U);
							result = response;
						}
						catch (TransientException arg2)
						{
							this.tracer.TraceError<TransientException>((long)this.GetHashCode(), "AD HANDLER: transient exception at reading photo.  Exception: {0}", arg2);
							request.PerformanceLogger.Log("ADHandlerError", string.Empty, 1U);
							throw;
						}
						catch (ADOperationException arg3)
						{
							this.tracer.TraceError<ADOperationException>((long)this.GetHashCode(), "AD HANDLER: AD exception at reading photo.  Exception: {0}", arg3);
							request.PerformanceLogger.Log("ADHandlerError", string.Empty, 1U);
							throw;
						}
						catch (IOException arg4)
						{
							this.tracer.TraceError<IOException>((long)this.GetHashCode(), "AD HANDLER: I/O exception at reading photo.  Exception: {0}", arg4);
							request.PerformanceLogger.Log("ADHandlerError", string.Empty, 1U);
							throw;
						}
					}
				}
			}
			return result;
		}

		public IPhotoHandler Then(IPhotoHandler next)
		{
			return new CompositePhotoHandler(this, next);
		}

		private PhotoResponse FindTargetAndReadPhoto(PhotoRequest request, PhotoResponse response)
		{
			this.ComputeTargetADObjectIdAndStampOntoRequest(request);
			if (request.TargetAdObjectId == null)
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "AD HANDLER: cannot serve photo because target AD object ID has not been initialized.");
				return response;
			}
			return this.ReadPhotoOntoResponse(request, response);
		}

		private void ComputeTargetADObjectIdAndStampOntoRequest(PhotoRequest request)
		{
			if (request.TargetAdObjectId != null)
			{
				return;
			}
			if (request.TargetPrincipal != null && request.TargetPrincipal.ObjectId != null)
			{
				request.TargetAdObjectId = request.TargetPrincipal.ObjectId;
				return;
			}
			if (string.IsNullOrEmpty(request.TargetSmtpAddress) || !SmtpAddress.IsValidSmtpAddress(request.TargetSmtpAddress))
			{
				return;
			}
			ADRecipient adrecipient = this.recipientSession.FindByProxyAddress(ProxyAddress.Parse(request.TargetSmtpAddress));
			if (adrecipient == null)
			{
				return;
			}
			request.TargetAdObjectId = adrecipient.Id;
		}

		private PhotoResponse ReadPhotoOntoResponse(PhotoRequest request, PhotoResponse response)
		{
			using (new StopwatchPerformanceTracker("ADHandlerReadPhoto", request.PerformanceLogger))
			{
				using (new ADPerformanceTracker("ADHandlerReadPhoto", request.PerformanceLogger))
				{
					PhotoMetadata photoMetadata = this.reader.Read(this.recipientSession, request.TargetAdObjectId, response.OutputPhotoStream);
					response.Served = true;
					response.Status = HttpStatusCode.OK;
					response.ContentLength = photoMetadata.Length;
					response.ContentType = photoMetadata.ContentType;
					response.Thumbprint = null;
					request.PerformanceLogger.Log("ADHandlerPhotoAvailable", string.Empty, 1U);
					request.PerformanceLogger.Log("ADHandlerPhotoServed", string.Empty, 1U);
				}
			}
			return response;
		}

		private readonly ITracer tracer = ExTraceGlobals.UserPhotosTracer;

		private readonly IADPhotoReader reader;

		private readonly IRecipientSession recipientSession;
	}
}
