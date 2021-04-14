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
	internal sealed class FileSystemPhotoHandler : IPhotoHandler
	{
		public FileSystemPhotoHandler(PhotosConfiguration configuration, IFileSystemPhotoReader reader, ITracer upstreamTracer)
		{
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (upstreamTracer == null)
			{
				throw new ArgumentNullException("upstreamTracer");
			}
			this.tracer = ExTraceGlobals.UserPhotosTracer.Compose(upstreamTracer);
			this.upstreamTracer = upstreamTracer;
			this.photosRootDirectoryFullPath = configuration.PhotosRootDirectoryFullPath;
			this.photoFileTimeToLive = configuration.PhotoFileTimeToLive;
			this.sizesToCacheOnFileSystem = configuration.SizesToCacheOnFileSystem;
			this.reader = reader;
		}

		public PhotoResponse Retrieve(PhotoRequest request, PhotoResponse response)
		{
			PhotoResponse result;
			using (new StopwatchPerformanceTracker("FileSystemHandlerTotal", request.PerformanceLogger))
			{
				using (new CpuPerformanceTracker("FileSystemHandlerTotal", request.PerformanceLogger))
				{
					if (request.ShouldSkipHandlers(PhotoHandlers.FileSystem))
					{
						this.tracer.TraceDebug((long)this.GetHashCode(), "FILE SYSTEM HANDLER: skipped by request.");
						result = response;
					}
					else if (response.Served)
					{
						this.tracer.TraceDebug((long)this.GetHashCode(), "FILE SYSTEM HANDLER: skipped because photo has already been served by an upstream handler.");
						result = response;
					}
					else if (string.IsNullOrEmpty(request.TargetSmtpAddress))
					{
						this.tracer.TraceError((long)this.GetHashCode(), "FILE SYSTEM HANDLER: skipped because target SMTP address has not been initialized.");
						result = response;
					}
					else if (!this.IsSizeCacheable(request.Size))
					{
						this.tracer.TraceDebug((long)this.GetHashCode(), "FILE SYSTEM HANDLER: skipped because requested size is not cacheable on file system.");
						result = response;
					}
					else
					{
						response.FileSystemHandlerProcessed = true;
						request.PerformanceLogger.Log("FileSystemHandlerProcessed", string.Empty, 1U);
						try
						{
							string photoFullPath = this.ComputeAndStampPhotoFullPathOntoResponse(request, response);
							if (this.PhotoFileHasExpired(photoFullPath))
							{
								this.tracer.TraceDebug((long)this.GetHashCode(), "FILE SYSTEM HANDLER: photo file doesn't exist or has expired.");
								response.IsPhotoFileOnFileSystem = false;
								request.PerformanceLogger.Log("FileSystemHandlerPhotoAvailable", string.Empty, 0U);
								request.PerformanceLogger.Log("FileSystemHandlerPhotoServed", string.Empty, 0U);
								result = response;
							}
							else
							{
								int num = this.ReadThumbprintAndStampOntoResponse(request, response, photoFullPath);
								this.tracer.TraceDebug<int>((long)this.GetHashCode(), "FILE SYSTEM HANDLER: thumbprint = {0:X8}.", num);
								if (PhotoThumbprinter.Default.ThumbprintMatchesETag(num, request.ETag))
								{
									result = this.ServePhotoNotModified(request, response);
								}
								else
								{
									result = this.ReadPhotoFromFileOntoResponse(request, response, photoFullPath);
								}
							}
						}
						catch (CannotMapInvalidSmtpAddressToPhotoFileException arg)
						{
							this.tracer.TraceError<CannotMapInvalidSmtpAddressToPhotoFileException>((long)this.GetHashCode(), "FILE SYSTEM HANDLER: invalid SMTP address cannot be mapped to file system.  Exception: {0}", arg);
							request.PerformanceLogger.Log("FileSystemHandlerError", string.Empty, 1U);
							result = response;
						}
						catch (FileNotFoundException arg2)
						{
							this.tracer.TraceDebug<FileNotFoundException>((long)this.GetHashCode(), "FILE SYSTEM HANDLER: no photo.  Exception: {0}", arg2);
							response.IsPhotoFileOnFileSystem = false;
							request.PerformanceLogger.Log("FileSystemHandlerPhotoAvailable", string.Empty, 0U);
							result = response;
						}
						catch (DirectoryNotFoundException arg3)
						{
							this.tracer.TraceDebug<DirectoryNotFoundException>((long)this.GetHashCode(), "FILE SYSTEM HANDLER: no photo.  Exception: {0}", arg3);
							response.IsPhotoFileOnFileSystem = false;
							request.PerformanceLogger.Log("FileSystemHandlerPhotoAvailable", string.Empty, 0U);
							result = response;
						}
						catch (EndOfStreamException arg4)
						{
							this.tracer.TraceDebug<EndOfStreamException>((long)this.GetHashCode(), "FILE SYSTEM HANDLER: no photo.  Exception: {0}", arg4);
							request.PerformanceLogger.Log("FileSystemHandlerPhotoAvailable", string.Empty, 0U);
							result = response;
						}
						catch (IOException arg5)
						{
							this.tracer.TraceDebug<IOException>((long)this.GetHashCode(), "FILE SYSTEM HANDLER: I/O exception reading photo, possibly because it's opened with exclusive access by some other process.  Exception: {0}", arg5);
							response.IsPhotoFileOnFileSystem = true;
							request.PerformanceLogger.Log("FileSystemHandlerPhotoAvailable", string.Empty, 1U);
							request.PerformanceLogger.Log("FileSystemHandlerError", string.Empty, 1U);
							result = response;
						}
						catch (Win32Exception ex)
						{
							switch (ex.NativeErrorCode)
							{
							case 2:
							case 3:
								this.tracer.TraceDebug<Win32Exception>((long)this.GetHashCode(), "FILE SYSTEM HANDLER: no photo.  Exception: {0}", ex);
								response.IsPhotoFileOnFileSystem = false;
								request.PerformanceLogger.Log("FileSystemHandlerPhotoAvailable", string.Empty, 0U);
								return response;
							case 5:
								this.tracer.TraceDebug<Win32Exception>((long)this.GetHashCode(), "FILE SYSTEM HANDLER: cannot read photo because it's opened with exclusive access by some other process.  Exception: {0}", ex);
								response.IsPhotoFileOnFileSystem = true;
								request.PerformanceLogger.Log("FileSystemHandlerPhotoAvailable", string.Empty, 1U);
								request.PerformanceLogger.Log("FileSystemHandlerError", string.Empty, 1U);
								return response;
							}
							this.tracer.TraceDebug<Win32Exception>((long)this.GetHashCode(), "FILE SYSTEM HANDLER: Win32 exception reading photo.  Exception: {0}", ex);
							request.PerformanceLogger.Log("FileSystemHandlerError", string.Empty, 1U);
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

		private PhotoResponse ReadPhotoFromFileOntoResponse(PhotoRequest request, PhotoResponse response, string photoFullPath)
		{
			this.tracer.TraceDebug<string>((long)this.GetHashCode(), "FILE SYSTEM HANDLER: reading photo file {0}", photoFullPath);
			PhotoResponse result;
			using (new StopwatchPerformanceTracker("FileSystemHandlerReadPhoto", request.PerformanceLogger))
			{
				using (new CpuPerformanceTracker("FileSystemHandlerReadPhoto", request.PerformanceLogger))
				{
					PhotoMetadata photoMetadata = this.reader.Read(photoFullPath, response.OutputPhotoStream);
					if (this.IsNegativeCachingPhoto(photoMetadata.Length))
					{
						this.tracer.TraceDebug((long)this.GetHashCode(), "FILE SYSTEM HANDLER: photo file is empty.  NEGATIVE caching.");
						response.Served = true;
						response.Status = HttpStatusCode.NotFound;
						response.ServerCacheHit = true;
						response.IsPhotoFileOnFileSystem = true;
						request.PerformanceLogger.Log("FileSystemHandlerPhotoAvailable", string.Empty, 1U);
						request.PerformanceLogger.Log("FileSystemHandlerPhotoServed", string.Empty, 1U);
						result = response;
					}
					else
					{
						this.tracer.TraceDebug((long)this.GetHashCode(), "FILE SYSTEM HANDLER: photo was written into output stream.");
						response.Served = true;
						response.Status = HttpStatusCode.OK;
						response.ServerCacheHit = true;
						response.IsPhotoFileOnFileSystem = true;
						response.ContentLength = photoMetadata.Length;
						response.ContentType = photoMetadata.ContentType;
						request.PerformanceLogger.Log("FileSystemHandlerPhotoAvailable", string.Empty, 1U);
						request.PerformanceLogger.Log("FileSystemHandlerPhotoServed", string.Empty, 1U);
						result = response;
					}
				}
			}
			return result;
		}

		private PhotoResponse ServePhotoNotModified(PhotoRequest request, PhotoResponse response)
		{
			this.tracer.TraceDebug<string>((long)this.GetHashCode(), "FILE SYSTEM HANDLER: NOT MODIFIED.  Requestor already has photo cached.  ETag: {0}", request.ETag);
			response.ServerCacheHit = true;
			response.Served = true;
			response.Status = HttpStatusCode.NotModified;
			response.IsPhotoFileOnFileSystem = true;
			request.PerformanceLogger.Log("FileSystemHandlerPhotoAvailable", string.Empty, 1U);
			request.PerformanceLogger.Log("FileSystemHandlerPhotoServed", string.Empty, 1U);
			return response;
		}

		private string ComputeAndStampPhotoFullPathOntoResponse(PhotoRequest request, PhotoResponse response)
		{
			return response.PhotoFullPathOnFileSystem = new FileSystemPhotoMap(this.photosRootDirectoryFullPath, this.upstreamTracer).Map(request.TargetSmtpAddress, request.Size);
		}

		private int ReadThumbprintAndStampOntoResponse(PhotoRequest request, PhotoResponse response, string photoFullPath)
		{
			this.tracer.TraceDebug<string, string, UserPhotoSize>((long)this.GetHashCode(), "FILE SYSTEM HANDLER: reading thumbprint of photo file {0} for ({1}, {2})", photoFullPath, request.TargetSmtpAddress, request.Size);
			int result;
			using (new StopwatchPerformanceTracker("FileSystemHandlerReadThumbprint", request.PerformanceLogger))
			{
				using (new CpuPerformanceTracker("FileSystemHandlerReadThumbprint", request.PerformanceLogger))
				{
					int num = this.reader.ReadThumbprint(photoFullPath);
					response.Thumbprint = new int?(num);
					result = num;
				}
			}
			return result;
		}

		private bool IsNegativeCachingPhoto(long photoLength)
		{
			return photoLength == 0L;
		}

		private bool IsSizeCacheable(UserPhotoSize size)
		{
			return this.sizesToCacheOnFileSystem.Contains(size);
		}

		private bool PhotoFileHasExpired(string photoFullPath)
		{
			return DateTime.UtcNow.Subtract(this.reader.GetLastModificationTimeUtc(photoFullPath)) > this.photoFileTimeToLive;
		}

		private readonly ITracer tracer = ExTraceGlobals.UserPhotosTracer;

		private readonly ITracer upstreamTracer;

		private readonly string photosRootDirectoryFullPath;

		private readonly IFileSystemPhotoReader reader;

		private readonly TimeSpan photoFileTimeToLive;

		private readonly ICollection<UserPhotoSize> sizesToCacheOnFileSystem;
	}
}
