using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using Microsoft.Exchange.Data.ApplicationLogic.Performance;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Diagnostics.Performance;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CachingPhotoHandler : IPhotoHandler
	{
		public CachingPhotoHandler(IFileSystemPhotoWriter writer, PhotosConfiguration configuration, ITracer upstreamTracer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}
			if (upstreamTracer == null)
			{
				throw new ArgumentNullException("upstreamTracer");
			}
			this.tracer = ExTraceGlobals.UserPhotosTracer.Compose(upstreamTracer);
			this.writer = writer;
			this.sizesToCacheOnFileSystem = configuration.SizesToCacheOnFileSystem;
			this.photosRootDirectoryFullPath = configuration.PhotosRootDirectoryFullPath;
		}

		public PhotoResponse Retrieve(PhotoRequest request, PhotoResponse response)
		{
			PhotoResponse result;
			using (new StopwatchPerformanceTracker("CachingHandlerTotal", request.PerformanceLogger))
			{
				using (new CpuPerformanceTracker("CachingHandlerTotal", request.PerformanceLogger))
				{
					if (request.ShouldSkipHandlers(PhotoHandlers.Caching))
					{
						this.tracer.TraceDebug((long)this.GetHashCode(), "CACHING HANDLER: skipped by request.");
						result = response;
					}
					else if (response.ServerCacheHit)
					{
						this.tracer.TraceDebug((long)this.GetHashCode(), "CACHING HANDLER: skipped because photo is already cached on the server.");
						result = response;
					}
					else if (response.IsPhotoFileOnFileSystem)
					{
						this.tracer.TraceDebug((long)this.GetHashCode(), "CACHING HANDLER: skipped because photo file is already on the file system.");
						result = response;
					}
					else if (response.Status == HttpStatusCode.NotModified)
					{
						this.tracer.TraceDebug((long)this.GetHashCode(), "CACHING HANDLER: NOT MODIFIED.  Skipped because requestor (client) already has the photo cached.");
						result = response;
					}
					else if (!this.IsSizeCacheable(request.Size))
					{
						this.tracer.TraceDebug((long)this.GetHashCode(), "CACHING HANDLER: skipped because size is not cacheable.");
						result = response;
					}
					else if (CachingPhotoHandler.UserNotFound(request))
					{
						this.tracer.TraceDebug((long)this.GetHashCode(), "CACHING HANDLER: skipped because target user has not been found.");
						result = response;
					}
					else
					{
						try
						{
							this.ComputeAndStampPhotoFullPathOntoResponse(request, response);
							if (string.IsNullOrEmpty(response.PhotoFullPathOnFileSystem))
							{
								this.tracer.TraceDebug((long)this.GetHashCode(), "CACHING HANDLER: cannot cache because path on filesystem has NOT been initialized and could NOT be computed.");
								result = response;
							}
							else if (this.NoPhoto(response))
							{
								result = this.NegativeCacheToFileSystem(request, response);
							}
							else
							{
								result = this.CacheToFileSystem(request, response);
							}
						}
						catch (CannotMapInvalidSmtpAddressToPhotoFileException arg)
						{
							this.tracer.TraceError<CannotMapInvalidSmtpAddressToPhotoFileException>((long)this.GetHashCode(), "CACHING HANDLER: invalid SMTP address cannot be mapped to file system.  Exception: {0}", arg);
							result = response;
						}
						catch (IOException arg2)
						{
							this.tracer.TraceError<string, IOException>((long)this.GetHashCode(), "CACHING HANDLER: I/O failure writing photo at {0}.  Exception: {1}", response.PhotoFullPathOnFileSystem, arg2);
							result = response;
						}
						catch (UnauthorizedAccessException arg3)
						{
							this.tracer.TraceError<string, UnauthorizedAccessException>((long)this.GetHashCode(), "CACHING HANDLER: authorization failure writing photo at {0}.  Exception: {1}", response.PhotoFullPathOnFileSystem, arg3);
							throw;
						}
						catch (Win32Exception arg4)
						{
							this.tracer.TraceDebug<Win32Exception>((long)this.GetHashCode(), "CACHING HANDLER: Win32 exception writing photo.  Exception: {0}", arg4);
							result = response;
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

		internal bool IsSizeCacheable(UserPhotoSize size)
		{
			return this.sizesToCacheOnFileSystem.Contains(size);
		}

		private static bool UserNotFound(PhotoRequest request)
		{
			return request.TargetPrincipal == null;
		}

		private bool NoPhoto(PhotoResponse response)
		{
			return response.Status == HttpStatusCode.NotFound;
		}

		private PhotoResponse CacheToFileSystem(PhotoRequest request, PhotoResponse response)
		{
			PhotoResponse result;
			using (new StopwatchPerformanceTracker("CachingHandlerCachePhoto", request.PerformanceLogger))
			{
				using (new CpuPerformanceTracker("CachingHandlerCachePhoto", request.PerformanceLogger))
				{
					if (!response.OutputPhotoStream.CanSeek || !response.OutputPhotoStream.CanRead)
					{
						this.tracer.TraceError((long)this.GetHashCode(), "CACHING HANDLER: skipped because photo stream is unreadable and/or not seekable.");
						result = response;
					}
					else
					{
						response.CachingHandlerProcessed = true;
						request.PerformanceLogger.Log("CachingHandlerProcessed", string.Empty, 1U);
						int num = this.ComputeAndStampThumbprintOntoResponse(response);
						response.OutputPhotoStream.Seek(0L, SeekOrigin.Begin);
						this.tracer.TraceDebug<string, int, long>((long)this.GetHashCode(), "CACHING HANDLER: caching photo at {0} with thumbprint {1:X8} and size {2} bytes", response.PhotoFullPathOnFileSystem, num, response.OutputPhotoStream.Length);
						this.writer.Write(response.PhotoFullPathOnFileSystem, num, response.OutputPhotoStream);
						response.PhotoWrittenToFileSystem = true;
						this.tracer.TraceDebug((long)this.GetHashCode(), "CACHING HANDLER: photo has been cached successfully");
						result = response;
					}
				}
			}
			return result;
		}

		private PhotoResponse NegativeCacheToFileSystem(PhotoRequest request, PhotoResponse response)
		{
			using (new StopwatchPerformanceTracker("CachingHandlerCacheNegativePhoto", request.PerformanceLogger))
			{
				using (new CpuPerformanceTracker("CachingHandlerCacheNegativePhoto", request.PerformanceLogger))
				{
					response.CachingHandlerProcessed = true;
					request.PerformanceLogger.Log("CachingHandlerProcessed", string.Empty, 1U);
					int num = PhotoThumbprinter.Default.GenerateThumbprintForNegativeCache();
					this.tracer.TraceDebug<string, int>((long)this.GetHashCode(), "CACHING HANDLER: caching NEGATIVE photo at {0} with thumbprint = {1:X8}", response.PhotoFullPathOnFileSystem, num);
					this.writer.Write(response.PhotoFullPathOnFileSystem, num, Stream.Null);
					response.PhotoWrittenToFileSystem = true;
					this.tracer.TraceDebug((long)this.GetHashCode(), "CACHING HANDLER: wrote NEGATIVE cache photo file.");
					response.Thumbprint = new int?(num);
				}
			}
			return response;
		}

		private int ComputeAndStampThumbprintOntoResponse(PhotoResponse response)
		{
			if (response.Thumbprint != null)
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "CACHING HANDLER: thumbprint has already been computed by upstream handler.");
				return response.Thumbprint.Value;
			}
			this.tracer.TraceDebug((long)this.GetHashCode(), "CACHING HANDLER: thumbprint has NOT been computed by upstream handler.  Computing now.");
			response.OutputPhotoStream.Seek(0L, SeekOrigin.Begin);
			int num = PhotoThumbprinter.Default.Compute(response.OutputPhotoStream);
			response.Thumbprint = new int?(num);
			return num;
		}

		private void ComputeAndStampPhotoFullPathOntoResponse(PhotoRequest request, PhotoResponse response)
		{
			if (!string.IsNullOrEmpty(response.PhotoFullPathOnFileSystem))
			{
				return;
			}
			string smtpAddressForCacheFilename = this.GetSmtpAddressForCacheFilename(request);
			if (string.IsNullOrEmpty(smtpAddressForCacheFilename))
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "CACHING HANDLER: cannot compute SMTP address for cache filename.");
				return;
			}
			response.PhotoFullPathOnFileSystem = new FileSystemPhotoMap(this.photosRootDirectoryFullPath, this.tracer).Map(smtpAddressForCacheFilename, request.Size);
			this.tracer.TraceDebug<string>((long)this.GetHashCode(), "CACHING HANDLER: computed photo full path: {0}", response.PhotoFullPathOnFileSystem);
		}

		private string GetSmtpAddressForCacheFilename(PhotoRequest request)
		{
			if (!string.IsNullOrEmpty(request.TargetPrimarySmtpAddress))
			{
				return request.TargetPrimarySmtpAddress;
			}
			if (!string.IsNullOrEmpty(request.TargetSmtpAddress))
			{
				return request.TargetSmtpAddress;
			}
			if (request.TargetRecipient != null)
			{
				SmtpAddress primarySmtpAddress = request.TargetRecipient.PrimarySmtpAddress;
				return request.TargetRecipient.PrimarySmtpAddress.ToString();
			}
			return string.Empty;
		}

		private readonly ICollection<UserPhotoSize> sizesToCacheOnFileSystem;

		private readonly ITracer tracer = ExTraceGlobals.UserPhotosTracer;

		private readonly IFileSystemPhotoWriter writer;

		private readonly string photosRootDirectoryFullPath;
	}
}
