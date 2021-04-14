using System;
using System.Globalization;
using System.Net;
using System.Security.Principal;
using Microsoft.Exchange.Data.ApplicationLogic.Performance;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Diagnostics.Performance;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MailboxPhotoHandler : IPhotoHandler
	{
		public MailboxPhotoHandler(PhotosConfiguration configuration, string clientInfo, IMailboxPhotoReader reader, IRecipientSession recipientSession, ITracer upstreamTracer, IXSOFactory xsoFactory)
		{
			ArgumentValidator.ThrowIfNull("configuration", configuration);
			ArgumentValidator.ThrowIfNullOrEmpty("clientInfo", clientInfo);
			ArgumentValidator.ThrowIfNull("reader", reader);
			ArgumentValidator.ThrowIfNull("recipientSession", recipientSession);
			ArgumentValidator.ThrowIfNull("upstreamTracer", upstreamTracer);
			ArgumentValidator.ThrowIfNull("xsoFactory", xsoFactory);
			this.tracer = ExTraceGlobals.UserPhotosTracer.Compose(upstreamTracer);
			this.configuration = configuration;
			this.xsoFactory = xsoFactory;
			this.reader = reader;
			this.clientInfo = clientInfo;
			this.recipientSession = recipientSession;
		}

		public PhotoResponse Retrieve(PhotoRequest request, PhotoResponse response)
		{
			PhotoResponse result;
			using (new StopwatchPerformanceTracker("MailboxHandlerTotal", request.PerformanceLogger))
			{
				using (new StorePerformanceTracker("MailboxHandlerTotal", request.PerformanceLogger))
				{
					if (request.ShouldSkipHandlers(PhotoHandlers.Mailbox))
					{
						this.tracer.TraceDebug((long)this.GetHashCode(), "MAILBOX HANDLER: skipped by request.");
						result = response;
					}
					else if (response.Served)
					{
						this.tracer.TraceDebug((long)this.GetHashCode(), "MAILBOX HANDLER: skipped because photo has already been served by an upstream handler.");
						result = response;
					}
					else
					{
						response.MailboxHandlerProcessed = true;
						request.PerformanceLogger.Log("MailboxHandlerProcessed", string.Empty, 1U);
						try
						{
							this.ComputeTargetPrincipalAndStampOntoRequest(request);
							if (request.TargetPrincipal == null)
							{
								this.tracer.TraceDebug((long)this.GetHashCode(), "MAILBOX HANDLER: skipped because target principal has not been initialized and could not be computed.");
								result = response;
							}
							else
							{
								this.tracer.TraceDebug<ExchangePrincipal, SmtpAddress>((long)this.GetHashCode(), "MAILBOX HANDLER: target principal: {0};  PRIMARY SMTP address: {1}", request.TargetPrincipal, request.TargetPrincipal.MailboxInfo.PrimarySmtpAddress);
								using (DisposeGuard disposeGuard = default(DisposeGuard))
								{
									bool forceReloadThumbprint = true;
									IMailboxSession mailboxSession = request.HostOwnedTargetMailboxSessionGetter(request.TargetPrincipal);
									if (mailboxSession == null)
									{
										this.tracer.TraceDebug<ExchangePrincipal>((long)this.GetHashCode(), "MAILBOX HANDLER: opening session to mailbox of user {0}", request.TargetPrincipal);
										mailboxSession = this.CreateMailboxSession(request);
										disposeGuard.Add<IMailboxSession>(mailboxSession);
										forceReloadThumbprint = false;
									}
									int num = this.ReadThumbprintAndStampOntoResponse(request, response, mailboxSession, forceReloadThumbprint);
									this.tracer.TraceDebug<int>((long)this.GetHashCode(), "MAILBOX HANDLER: read photo thumbprint = {0:X8}", num);
									if (PhotoThumbprinter.Default.ThumbprintMatchesETag(num, request.ETag))
									{
										result = this.ServePhotoNotModified(request, response);
									}
									else
									{
										result = this.ReadPhotoFromMailboxOntoResponse(request, response, mailboxSession);
									}
								}
							}
						}
						catch (ObjectNotFoundException ex)
						{
							request.PerformanceLogger.Log("MailboxHandlerPhotoAvailable", string.Empty, 0U);
							if (this.reader.HasPhotoBeenDeleted(ex))
							{
								result = this.ServePhotoHasBeenDeleted(request, response);
							}
							else
							{
								this.tracer.TraceDebug<ObjectNotFoundException>((long)this.GetHashCode(), "MAILBOX HANDLER: photo not found.  Exception: {0}", ex);
								result = response;
							}
						}
						catch (StorageTransientException arg)
						{
							this.tracer.TraceError<StorageTransientException>((long)this.GetHashCode(), "MAILBOX HANDLER: transient exception at reading photo.  Exception: {0}", arg);
							request.PerformanceLogger.Log("MailboxHandlerError", string.Empty, 1U);
							throw;
						}
						catch (StoragePermanentException arg2)
						{
							this.tracer.TraceError<StoragePermanentException>((long)this.GetHashCode(), "MAILBOX HANDLER: permanent exception at reading photo.  Exception: {0}", arg2);
							request.PerformanceLogger.Log("MailboxHandlerError", string.Empty, 1U);
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

		private PhotoResponse ServePhotoNotModified(PhotoRequest request, PhotoResponse response)
		{
			this.tracer.TraceDebug<string>((long)this.GetHashCode(), "MAILBOX HANDLER: NOT MODIFIED.  Requestor already has photo cached.  ETag: {0}", request.ETag);
			response.Served = true;
			response.Status = HttpStatusCode.NotModified;
			response.HttpExpiresHeader = UserAgentPhotoExpiresHeader.Default.ComputeExpiresHeader(DateTime.UtcNow, HttpStatusCode.NotModified, this.configuration);
			request.PerformanceLogger.Log("MailboxHandlerPhotoAvailable", string.Empty, 1U);
			request.PerformanceLogger.Log("MailboxHandlerPhotoServed", string.Empty, 1U);
			return response;
		}

		private PhotoResponse ReadPhotoFromMailboxOntoResponse(PhotoRequest request, PhotoResponse response, IMailboxSession session)
		{
			this.tracer.TraceDebug((long)this.GetHashCode(), "MAILBOX HANDLER: reading photo from mailbox onto response.");
			using (new StopwatchPerformanceTracker("MailboxHandlerReadPhoto", request.PerformanceLogger))
			{
				using (new StorePerformanceTracker("MailboxHandlerReadPhoto", request.PerformanceLogger))
				{
					PhotoMetadata photoMetadata = this.reader.Read(session, request.Size, request.Preview, response.OutputPhotoStream, request.PerformanceLogger);
					this.tracer.TraceDebug((long)this.GetHashCode(), "MAILBOX HANDLER: photo was written to response.");
					response.Served = true;
					response.Status = HttpStatusCode.OK;
					response.HttpExpiresHeader = UserAgentPhotoExpiresHeader.Default.ComputeExpiresHeader(DateTime.UtcNow, HttpStatusCode.OK, this.configuration);
					response.ServerCacheHit = false;
					response.ContentLength = photoMetadata.Length;
					response.ContentType = photoMetadata.ContentType;
					request.PerformanceLogger.Log("MailboxHandlerPhotoAvailable", string.Empty, 1U);
					request.PerformanceLogger.Log("MailboxHandlerPhotoServed", string.Empty, 1U);
				}
			}
			return response;
		}

		private int ReadThumbprintAndStampOntoResponse(PhotoRequest request, PhotoResponse response, IMailboxSession session, bool forceReloadThumbprint)
		{
			int num = this.reader.ReadThumbprint(session, request.Preview, forceReloadThumbprint);
			response.Thumbprint = new int?(num);
			return num;
		}

		private IMailboxSession CreateMailboxSession(PhotoRequest request)
		{
			IMailboxSession result;
			using (new StopwatchPerformanceTracker("MailboxHandlerOpenMailbox", request.PerformanceLogger))
			{
				using (new StorePerformanceTracker("MailboxHandlerOpenMailbox", request.PerformanceLogger))
				{
					result = this.xsoFactory.ConfigurableOpenMailboxSession(request.TargetPrincipal, new MailboxAccessInfo(new WindowsPrincipal(WindowsIdentity.GetCurrent())), CultureInfo.InvariantCulture, this.clientInfo, LogonType.Admin, MailboxPhotoHandler.MailboxPropertyDefinitions, MailboxSession.InitializationFlags.DefaultFolders | MailboxSession.InitializationFlags.SuppressFolderIdPrefetch | MailboxSession.InitializationFlags.DeferDefaultFolderIdInitialization, MailboxPhotoHandler.MailboxDefaultFolderTypes);
				}
			}
			return result;
		}

		private void ComputeTargetPrincipalAndStampOntoRequest(PhotoRequest request)
		{
			if (request.TargetPrincipal != null)
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "MAILBOX HANDLER: target principal has already been initialized.");
				return;
			}
			if (request.Requestor == null || request.Requestor.OrganizationId == null)
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "MAILBOX HANDLER: cannot compute target principal because requestor's organization id has not been initialized.");
				return;
			}
			if (string.IsNullOrEmpty(request.TargetSmtpAddress))
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "MAILBOX HANDLER: cannot compute target principal because the target's SMTP address has not been initialized.");
				return;
			}
			using (new StopwatchPerformanceTracker("MailboxPhotoHandlerComputeTargetPrincipal", request.PerformanceLogger))
			{
				using (new ADPerformanceTracker("MailboxPhotoHandlerComputeTargetPrincipal", request.PerformanceLogger))
				{
					this.tracer.TraceDebug((long)this.GetHashCode(), "MAILBOX HANDLER: computing target principal.");
					request.TargetPrincipal = ExchangePrincipal.FromProxyAddress(this.recipientSession, request.TargetSmtpAddress, RemotingOptions.LocalConnectionsOnly);
					if (request.TargetAdObjectId == null)
					{
						request.TargetAdObjectId = request.TargetPrincipal.ObjectId;
						this.tracer.TraceDebug<ADObjectId>((long)this.GetHashCode(), "MAILBOX HANDLER: stamped AD object ID '{0}' onto request.", request.TargetAdObjectId);
					}
				}
			}
		}

		private PhotoResponse ServePhotoHasBeenDeleted(PhotoRequest request, PhotoResponse response)
		{
			this.tracer.TraceDebug((long)this.GetHashCode(), "MAILBOX HANDLER: photo has been deleted.");
			response.Served = true;
			response.Status = HttpStatusCode.NotFound;
			response.HttpExpiresHeader = UserAgentPhotoExpiresHeader.Default.ComputeExpiresHeader(DateTime.UtcNow, HttpStatusCode.NotFound, this.configuration);
			request.PerformanceLogger.Log("MailboxHandlerPhotoServed", string.Empty, 0U);
			return response;
		}

		private static readonly PropertyDefinition[] MailboxPropertyDefinitions = new PropertyDefinition[]
		{
			MailboxSchema.UserPhotoCacheId,
			MailboxSchema.UserPhotoPreviewCacheId,
			MailboxSchema.LocaleId,
			CoreItemSchema.Id,
			CoreItemSchema.ItemClass
		};

		private static readonly DefaultFolderType[] MailboxDefaultFolderTypes = new DefaultFolderType[]
		{
			DefaultFolderType.Configuration,
			DefaultFolderType.System
		};

		private readonly ITracer tracer = ExTraceGlobals.UserPhotosTracer;

		private readonly PhotosConfiguration configuration;

		private readonly IXSOFactory xsoFactory;

		private readonly string clientInfo;

		private readonly IMailboxPhotoReader reader;

		private readonly IRecipientSession recipientSession;
	}
}
