using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.UserPhotos;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.InfoWorker.Common.Availability.Proxy;
using Microsoft.Exchange.InfoWorker.Common.UserPhotos;

namespace Microsoft.Exchange.AirSync
{
	internal sealed class AirSyncPhotoRetriever : DisposeTrackableBase
	{
		public IAirSyncContext Context { get; set; }

		public List<StatusCode> StatusCodes { get; set; }

		public Dictionary<string, byte[]> RetrievedPhotos { get; set; }

		public int NumberOfPhotosRequested { get; set; }

		public int NumberOfPhotosSuccess { get; set; }

		public int NumberOfPhotosFromCache { get; set; }

		internal AirSyncPhotoRetriever(IAirSyncContext context)
		{
			this.Context = context;
		}

		internal byte[] EndGetThumbnailPhotoFromMailbox(string targetPrimarySmtpAddress, TimeSpan waitTime, UserPhotoSize photoSize)
		{
			AirSyncPhotoRetriever.UserPhotoWithSize userPhotoWithSize = null;
			if ((!AirSyncPhotoRetriever.userPhotosCache.ContainsKey(targetPrimarySmtpAddress) || (from s in AirSyncPhotoRetriever.userPhotosCache[targetPrimarySmtpAddress]
			where s.PhotoSize == photoSize
			select s).Count<AirSyncPhotoRetriever.UserPhotoWithSize>() == 0) && waitTime.TotalMilliseconds > 0.0)
			{
				AirSyncDiagnostics.TraceDebug<string, double>(ExTraceGlobals.ProtocolTracer, "AirSyncPhotoRetriever::EndGetThumbnailPhotoFromMailbox - user photo not available in cache. TargetUser: {0},  PhotoSize: {1}, waittime:{2}", targetPrimarySmtpAddress, photoSize.ToString(), waitTime.TotalMilliseconds);
				IAsyncResult asyncResult;
				if (this.delegatesCollection.TryRemove(targetPrimarySmtpAddress, out asyncResult) && asyncResult.AsyncWaitHandle.WaitOne(waitTime))
				{
					AirSyncPhotoRetriever.GetThumbnailPhotoCompleted(asyncResult);
					this.NumberOfPhotosSuccess++;
					AirSyncDiagnostics.TraceDebug<int>(ExTraceGlobals.ProtocolTracer, "AirSyncPhotoRetriever::EndGetThumbnailPhotoFromMailbox - user photo successfully retrieved. user:{0}, numberofPhotosSuccess:{1}", targetPrimarySmtpAddress, this.NumberOfPhotosSuccess);
				}
				else
				{
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.ProtocolTracer, null, "AirSyncPhotoRetriever::EndGetThumbnailPhotoFromMailbox - user photo failed to retrieve.");
				}
			}
			else
			{
				AirSyncDiagnostics.TraceDebug<int>(ExTraceGlobals.ProtocolTracer, null, "AirSyncPhotoRetriever::EndGetThumbnailPhotoFromMailbox - photo retrieved from cache. number of photos from cache:{0}", this.NumberOfPhotosFromCache);
			}
			List<AirSyncPhotoRetriever.UserPhotoWithSize> source;
			if (this.VerifyUserPermissions(this.Context.User.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString(), this.Context.User.OrganizationId, targetPrimarySmtpAddress) && AirSyncPhotoRetriever.userPhotosCache.TryGetValue(targetPrimarySmtpAddress, out source))
			{
				userPhotoWithSize = (from s in source
				where s.PhotoSize == photoSize
				select s).FirstOrDefault<AirSyncPhotoRetriever.UserPhotoWithSize>();
			}
			if (userPhotoWithSize == null)
			{
				return null;
			}
			return userPhotoWithSize.UserPhotoBytes;
		}

		internal void BeginGetThumbnailPhotoFromMailbox(List<string> targetPrimarySmtpAddresses, UserPhotoSize photoSize)
		{
			foreach (string text in targetPrimarySmtpAddresses)
			{
				if (AirSyncPhotoRetriever.userPhotosCache.ContainsKey(text))
				{
					if ((from s in AirSyncPhotoRetriever.userPhotosCache[text]
					where s.PhotoSize == photoSize
					select s).Count<AirSyncPhotoRetriever.UserPhotoWithSize>() != 0)
					{
						goto IL_111;
					}
				}
				if (!this.delegatesCollection.ContainsKey(text))
				{
					AirSyncDiagnostics.TraceDebug<string, string, UserPhotoSize>(ExTraceGlobals.ProtocolTracer, null, "AirSyncPhotoRetriever::BeginGetThumbnailPhotoFromMailbox - {0} requesting photo for {1} , with photosize {2}", this.Context.User.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString(), text, photoSize);
					AirSyncPhotoRetriever.GetThumbnailPhotoFromMailboxDelegate getThumbnailPhotoFromMailboxDelegate = new AirSyncPhotoRetriever.GetThumbnailPhotoFromMailboxDelegate(AirSyncPhotoRetriever.GetThumbnailPhotoFromMailbox);
					IAsyncResult value = getThumbnailPhotoFromMailboxDelegate.BeginInvoke(this.Context, text, photoSize, null, new AirSyncPhotoRetriever.UserPhotoWithSize
					{
						UserEmail = text,
						PhotoSize = photoSize
					});
					this.delegatesCollection.TryAdd(text, value);
					this.NumberOfPhotosRequested++;
					continue;
				}
				IL_111:
				this.NumberOfPhotosFromCache++;
			}
		}

		private static void GetThumbnailPhotoCompleted(IAsyncResult result)
		{
			AirSyncPhotoRetriever.GetThumbnailPhotoFromMailboxDelegate getThumbnailPhotoFromMailboxDelegate = ((AsyncResult)result).AsyncDelegate as AirSyncPhotoRetriever.GetThumbnailPhotoFromMailboxDelegate;
			AirSyncPhotoRetriever.UserPhotoWithSize userPhotoWithSize = result.AsyncState as AirSyncPhotoRetriever.UserPhotoWithSize;
			try
			{
				byte[] userPhotoBytes = getThumbnailPhotoFromMailboxDelegate.EndInvoke(result);
				lock (AirSyncPhotoRetriever.lockObject)
				{
					List<AirSyncPhotoRetriever.UserPhotoWithSize> list;
					if (!AirSyncPhotoRetriever.userPhotosCache.ContainsKey(userPhotoWithSize.UserEmail))
					{
						list = new List<AirSyncPhotoRetriever.UserPhotoWithSize>();
					}
					else
					{
						list = AirSyncPhotoRetriever.userPhotosCache[userPhotoWithSize.UserEmail];
					}
					list.Add(new AirSyncPhotoRetriever.UserPhotoWithSize
					{
						UserEmail = userPhotoWithSize.UserEmail,
						PhotoSize = userPhotoWithSize.PhotoSize,
						UserPhotoBytes = userPhotoBytes
					});
					AirSyncPhotoRetriever.userPhotosCache.Add(userPhotoWithSize.UserEmail, list);
				}
			}
			catch (AccessDeniedException arg)
			{
				AirSyncDiagnostics.TraceError<AccessDeniedException>(ExTraceGlobals.ProtocolTracer, "AirSyncPhotoRetriever::GetThumbnailPhotoCompleted- Access denied retrieving thumbnailPhoto via GetUserPhoto. TargetUser: {0}.  Exception: {1}", userPhotoWithSize.UserEmail, arg);
			}
		}

		private static IMailboxSession GetCachedMailboxSessionForPhotoRequest(ExchangePrincipal user)
		{
			return null;
		}

		private static byte[] GetThumbnailPhotoFromMailbox(IAirSyncContext context, string targetPrimarySmtpAddress, UserPhotoSize photoSize = UserPhotoSize.HR48x48)
		{
			byte[] array = null;
			ClientContext clientContext = ClientContext.Create(context.User.ClientSecurityContextWrapper.ClientSecurityContext, context.User.ExchangePrincipal.MailboxInfo.OrganizationId, null, null, CultureInfo.InvariantCulture, Guid.NewGuid().ToString());
			clientContext.RequestSchemaVersion = ExchangeVersionType.Exchange2012;
			if (!SmtpAddress.IsValidSmtpAddress(targetPrimarySmtpAddress))
			{
				AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.ProtocolTracer, null, "Target SMTP address is not valid: {0}", targetPrimarySmtpAddress);
				return array;
			}
			GetUserPhotoQuery getUserPhotoQuery = new GetUserPhotoQuery(clientContext, new PhotoRequest
			{
				Requestor = new PhotoPrincipal
				{
					EmailAddresses = new string[]
					{
						context.User.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString().ToString()
					},
					OrganizationId = context.User.ExchangePrincipal.MailboxInfo.OrganizationId
				},
				Size = photoSize,
				TargetSmtpAddress = targetPrimarySmtpAddress,
				Trace = ExTraceGlobals.ProtocolTracer.IsTraceEnabled(TraceType.DebugTrace),
				HostOwnedTargetMailboxSessionGetter = new Func<ExchangePrincipal, IMailboxSession>(AirSyncPhotoRetriever.GetCachedMailboxSessionForPhotoRequest)
			}, null, false, AirSyncPhotoRetriever.PhotosConfiguration, ExTraceGlobals.ProtocolTracer);
			try
			{
				array = getUserPhotoQuery.Execute();
				if (array == null || array.Length == 0)
				{
					AirSyncDiagnostics.TraceError(ExTraceGlobals.ProtocolTracer, "Unable to retrieve thumbnailPhoto via GetUserPhoto for {0}", targetPrimarySmtpAddress);
					return null;
				}
				return array;
			}
			catch (AccessDeniedException arg)
			{
				AirSyncDiagnostics.TraceError<AccessDeniedException>(ExTraceGlobals.ProtocolTracer, "Access denied retrieving thumbnailPhoto via GetUserPhoto for {0}.  Exception: {1}", targetPrimarySmtpAddress, arg);
			}
			return null;
		}

		internal static StatusCode GetThumbnailPhotoFromMailbox(IAirSyncContext context, string targetPrimarySmtpAddress, out byte[] thumbnailPhoto)
		{
			thumbnailPhoto = null;
			ClientContext clientContext = ClientContext.Create(context.User.ClientSecurityContextWrapper.ClientSecurityContext, context.User.ExchangePrincipal.MailboxInfo.OrganizationId, null, null, CultureInfo.InvariantCulture, Guid.NewGuid().ToString());
			clientContext.RequestSchemaVersion = ExchangeVersionType.Exchange2012;
			if (!SmtpAddress.IsValidSmtpAddress(targetPrimarySmtpAddress))
			{
				AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.ProtocolTracer, null, "Target SMTP address is not valid: {0}", targetPrimarySmtpAddress);
				return StatusCode.NoPicture;
			}
			GetUserPhotoQuery getUserPhotoQuery = new GetUserPhotoQuery(clientContext, new PhotoRequest
			{
				Requestor = new PhotoPrincipal
				{
					EmailAddresses = new string[]
					{
						context.User.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString()
					},
					OrganizationId = context.User.ExchangePrincipal.MailboxInfo.OrganizationId
				},
				Size = UserPhotoSize.HR648x648,
				TargetSmtpAddress = targetPrimarySmtpAddress,
				Trace = ExTraceGlobals.ProtocolTracer.IsTraceEnabled(TraceType.DebugTrace),
				HostOwnedTargetMailboxSessionGetter = new Func<ExchangePrincipal, IMailboxSession>(AirSyncPhotoRetriever.GetCachedMailboxSessionForPhotoRequest)
			}, null, false, AirSyncPhotoRetriever.PhotosConfiguration, ExTraceGlobals.ProtocolTracer);
			StatusCode result;
			try
			{
				thumbnailPhoto = getUserPhotoQuery.Execute();
				if (thumbnailPhoto == null || thumbnailPhoto.Length == 0)
				{
					AirSyncDiagnostics.TraceError(ExTraceGlobals.ProtocolTracer, "Unable to retrieve thumbnailPhoto via GetUserPhoto for {0}", targetPrimarySmtpAddress);
					result = StatusCode.NoPicture;
				}
				else
				{
					result = StatusCode.Success;
				}
			}
			catch (AccessDeniedException arg)
			{
				AirSyncDiagnostics.TraceError<AccessDeniedException>(ExTraceGlobals.ProtocolTracer, "Access denied retrieving thumbnailPhoto via GetUserPhoto for {0}.  Exception: {1}", targetPrimarySmtpAddress, arg);
				result = StatusCode.NoPicture;
			}
			return result;
		}

		private bool VerifyUserPermissions(string userEmail, OrganizationId userOrganizationId, string targetUserEmailAddress)
		{
			bool result;
			try
			{
				if (!SmtpAddress.IsValidSmtpAddress(targetUserEmailAddress))
				{
					result = false;
				}
				else
				{
					PhotoPrincipal requestor = new PhotoPrincipal
					{
						EmailAddresses = new List<string>
						{
							userEmail
						},
						OrganizationId = userOrganizationId
					};
					PhotoPrincipal target = new PhotoPrincipal
					{
						EmailAddresses = new List<string>
						{
							targetUserEmailAddress
						},
						OrganizationId = userOrganizationId
					};
					new PhotoAuthorization(OrganizationIdCache.Singleton, ExTraceGlobals.ProtocolTracer).Authorize(requestor, target);
					AirSyncDiagnostics.TraceDebug<string, string>(ExTraceGlobals.ProtocolTracer, null, "AirSyncPhotoRetriever::VerifyUserPermissions - {0} has permissiosn to retrieve photos for user {2}.", userEmail, targetUserEmailAddress);
					result = true;
				}
			}
			catch (AccessDeniedException arg)
			{
				AirSyncDiagnostics.TraceError<AccessDeniedException, string>(ExTraceGlobals.ProtocolTracer, "Access denied verifying user's permissions to retrieve thumbnailPhoto via GetUserPhoto for {0}.  Exception: {1}. Current user :{2}", targetUserEmailAddress, arg, userEmail);
				result = false;
			}
			return result;
		}

		private void LogDataToProtocolLogger()
		{
			if (this.Context != null)
			{
				this.Context.ProtocolLogger.SetValue(ProtocolLoggerData.PhotosFromCache, this.NumberOfPhotosFromCache);
				this.Context.ProtocolLogger.SetValue(ProtocolLoggerData.TotalPhotoRequests, this.NumberOfPhotosRequested);
				this.Context.ProtocolLogger.SetValue(ProtocolLoggerData.SuccessfulPhotoRequests, this.NumberOfPhotosSuccess);
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.LogDataToProtocolLogger();
				this.delegatesCollection.Clear();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AirSyncPhotoRetriever>(this);
		}

		private static readonly PhotosConfiguration PhotosConfiguration = new PhotosConfiguration(ExchangeSetupContext.InstallPath);

		private static object lockObject = new object();

		private ConcurrentDictionary<string, IAsyncResult> delegatesCollection = new ConcurrentDictionary<string, IAsyncResult>();

		private static MruDictionaryCache<string, List<AirSyncPhotoRetriever.UserPhotoWithSize>> userPhotosCache = new MruDictionaryCache<string, List<AirSyncPhotoRetriever.UserPhotoWithSize>>(GlobalSettings.HDPhotoCacheMaxSize, (int)GlobalSettings.HDPhotoCacheExpirationTimeOut.TotalMinutes);

		internal class UserPhotoWithSize
		{
			public string UserEmail { get; set; }

			public byte[] UserPhotoBytes { get; set; }

			public UserPhotoSize PhotoSize { get; set; }
		}

		private delegate byte[] GetThumbnailPhotoFromMailboxDelegate(IAirSyncContext context, string targetSmtpAddress, UserPhotoSize photoSize);
	}
}
