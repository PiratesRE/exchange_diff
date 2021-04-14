using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.RightsManagement.Protectors;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal static class WacUtilities
	{
		public static string LocalServerName
		{
			get
			{
				WacUtilities.EnsureLocalServerNameAndVersion();
				return WacUtilities.localServerName;
			}
		}

		public static string LocalServerVersion
		{
			get
			{
				WacUtilities.EnsureLocalServerNameAndVersion();
				return WacUtilities.localServerVersion;
			}
		}

		public static string GetExchangeSessionId(string authToken)
		{
			return authToken.GetHashCode().ToString("X8");
		}

		public static ExchangePrincipal GetExchangePrincipal(WacRequest wacRequest, out ADSessionSettings adSessionSettings, bool isArchive)
		{
			return WacUtilities.GetExchangePrincipal(wacRequest.MailboxSmtpAddress.ToString(), out adSessionSettings, isArchive);
		}

		public static ExchangePrincipal GetExchangePrincipal(string smtpAddress, out ADSessionSettings adSessionSettings, bool isArchive)
		{
			adSessionSettings = Directory.SessionSettingsFromAddress(smtpAddress);
			ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromProxyAddress(adSessionSettings, ProxyAddressPrefix.Smtp.PrimaryPrefix + ":" + smtpAddress);
			if (exchangePrincipal == null)
			{
				throw new OwaADUserNotFoundException("PrimarySmtpAddress=" + smtpAddress);
			}
			if (isArchive)
			{
				return exchangePrincipal.GetArchiveExchangePrincipal();
			}
			return exchangePrincipal;
		}

		public static string GetOwnerIdForMailbox(string ewsAttachmentId)
		{
			IdHeaderInformation idHeaderInformation = ServiceIdConverter.ConvertFromConcatenatedId(ewsAttachmentId, BasicTypes.Attachment, null);
			return "ExchangeMbx_" + idHeaderInformation.MailboxId.MailboxGuid;
		}

		public static Stream GetStreamForProtectedAttachment(StreamAttachment attachment, IExchangePrincipal exchangePrincipal)
		{
			UseLicenseAndUsageRights useLicenseAndUsageRights;
			bool flag;
			return StreamAttachment.OpenRestrictedAttachment(attachment, exchangePrincipal.MailboxInfo.OrganizationId, exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString(), exchangePrincipal.Sid, exchangePrincipal.RecipientTypeDetails, out useLicenseAndUsageRights, out flag);
		}

		public static void WriteStreamBody(HttpResponse response, Stream contentStream)
		{
			Stream outputStream = response.OutputStream;
			WacUtilities.WriteStreamBody(outputStream, contentStream);
		}

		public static void WriteStreamBody(Stream responseStream, Stream contentStream)
		{
			byte[] buffer = new byte[65536];
			contentStream.Position = 0L;
			int num;
			do
			{
				num = contentStream.Read(buffer, 0, 65536);
				responseStream.Write(buffer, 0, num);
			}
			while (num > 0);
		}

		public static bool IsIrmRestricted(Item item)
		{
			RightsManagedMessageItem rightsManagedMessageItem = item as RightsManagedMessageItem;
			return rightsManagedMessageItem != null && rightsManagedMessageItem.IsRestricted;
		}

		public static bool ShouldUpdateAttachment(string mailboxSmtpAddress, string ewsAttachmentId, out CobaltStoreSaver saver)
		{
			string cacheKey = CachedAttachmentInfo.GetCacheKey(mailboxSmtpAddress, ewsAttachmentId);
			CachedAttachmentInfo fromCache = CachedAttachmentInfo.GetFromCache(cacheKey);
			if (fromCache == null || fromCache.CobaltStore == null)
			{
				saver = null;
				return false;
			}
			saver = fromCache.CobaltStore.Saver;
			return saver != null;
		}

		public static void ProcessAttachment(IStoreSession session, string ewsAttachmentId, IExchangePrincipal exchangePrincipal, PropertyOpenMode openMode, WacUtilities.AttachmentProcessor attachmentProcessor)
		{
			IdConverterDependencies converterDependencies = new IdConverterDependencies.FromRawData(false, false, null, null, exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString(), session as MailboxSession, session as MailboxSession, session as PublicFolderSession);
			using (AttachmentHandler.IAttachmentRetriever attachmentRetriever = AttachmentRetriever.CreateInstance(ewsAttachmentId, converterDependencies))
			{
				bool flag = WacUtilities.IsIrmRestricted(attachmentRetriever.RootItem);
				if (openMode == PropertyOpenMode.Modify)
				{
					attachmentRetriever.RootItem.OpenAsReadWrite();
				}
				StreamAttachment streamAttachment = attachmentRetriever.Attachment as StreamAttachment;
				if (streamAttachment == null)
				{
					attachmentProcessor(exchangePrincipal, attachmentRetriever.Attachment, null, flag);
				}
				else
				{
					using (Stream contentStream = streamAttachment.GetContentStream(openMode))
					{
						bool flag2 = WacUtilities.IsContentProtected(attachmentRetriever.Attachment.FileName, contentStream);
						attachmentProcessor(exchangePrincipal, streamAttachment, contentStream, flag || flag2);
						if (openMode == PropertyOpenMode.Modify)
						{
							attachmentRetriever.RootItem.Save(SaveMode.NoConflictResolution);
						}
					}
				}
			}
		}

		private static bool TryProcessUnprotectedAttachment(IExchangePrincipal exchangePrincipal, Item item, StreamAttachment streamAttachment, PropertyOpenMode openMode, WacUtilities.AttachmentProcessor attachmentProcessor)
		{
			if (openMode == PropertyOpenMode.Modify)
			{
				item.OpenAsReadWrite();
			}
			bool result;
			using (Stream contentStream = streamAttachment.GetContentStream(openMode))
			{
				bool flag = WacUtilities.IsContentProtected(streamAttachment.FileName, contentStream);
				if (flag)
				{
					result = false;
				}
				else
				{
					attachmentProcessor(exchangePrincipal, streamAttachment, contentStream, false);
					if (openMode == PropertyOpenMode.Modify)
					{
						item.Save(SaveMode.NoConflictResolution);
					}
					result = true;
				}
			}
			return result;
		}

		internal static StoreObjectId GetStoreObjectId(string ewsAttachmentId)
		{
			List<AttachmentId> attachmentIds = new List<AttachmentId>();
			IdHeaderInformation idHeaderInformation = ServiceIdConverter.ConvertFromConcatenatedId(ewsAttachmentId, BasicTypes.Attachment, attachmentIds);
			return idHeaderInformation.ToStoreObjectId();
		}

		internal static string GenerateSHA256HashForStream(Stream stream)
		{
			string result;
			using (SHA256Cng sha256Cng = new SHA256Cng())
			{
				stream.Position = 0L;
				byte[] inArray = sha256Cng.ComputeHash(stream);
				result = Convert.ToBase64String(inArray);
			}
			return result;
		}

		internal static void SetEventId(RequestDetailsLogger logger, string eventId)
		{
			OwsLogRegistry.Register(eventId, typeof(WacRequestHandlerMetadata), new Type[0]);
			logger.ActivityScope.SetProperty(ExtensibleLoggerMetadata.EventId, eventId);
		}

		internal static string GetCurrentTimeForFileName()
		{
			return ExDateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss.ffff");
		}

		internal static void ReplaceAttachmentContent(string smtpAddress, string cultureName, string ewsAttachmentID, bool isArchive, Stream source)
		{
			ADSessionSettings adsessionSettings;
			ExchangePrincipal exchangePrincipal = WacUtilities.GetExchangePrincipal(smtpAddress, out adsessionSettings, isArchive);
			CultureInfo cultureInfo = CultureInfo.GetCultureInfo(cultureName);
			List<AttachmentId> attachmentIds = new List<AttachmentId>();
			IdHeaderInformation idHeaderInformation = ServiceIdConverter.ConvertFromConcatenatedId(ewsAttachmentID, BasicTypes.Attachment, attachmentIds);
			idHeaderInformation.ToStoreObjectId();
			using (MailboxSession mailboxSession = MailboxSession.OpenAsSystemService(exchangePrincipal, cultureInfo, "Client=OWA;Action=WAC"))
			{
				WacUtilities.ProcessAttachment(mailboxSession, ewsAttachmentID, exchangePrincipal, PropertyOpenMode.Modify, delegate(IExchangePrincipal principal, Attachment attachment, Stream stream, bool anyContentProtected)
				{
					BinaryReader binaryReader = new BinaryReader(source);
					stream.Seek(0L, SeekOrigin.Begin);
					int num = 10000;
					int num2 = 0;
					byte[] buffer = new byte[num];
					for (;;)
					{
						int num3 = binaryReader.Read(buffer, 0, num);
						num2 += num3;
						if (num3 == 0)
						{
							break;
						}
						stream.Write(buffer, 0, num3);
					}
					stream.SetLength((long)num2);
					attachment.Save();
				});
			}
		}

		internal static bool ItemIsMessageDraft(Item item, Attachment attachment, out string referenceAttachmentWebServiceUrl, out string referenceAttachmentUrl)
		{
			ReferenceAttachment referenceAttachment = attachment as ReferenceAttachment;
			if (referenceAttachment != null)
			{
				referenceAttachmentWebServiceUrl = referenceAttachment.ProviderEndpointUrl;
				referenceAttachmentUrl = referenceAttachment.AttachLongPathName;
			}
			else
			{
				referenceAttachmentWebServiceUrl = null;
				referenceAttachmentUrl = null;
			}
			MessageItem messageItem = item as MessageItem;
			return messageItem != null && messageItem.IsDraft;
		}

		internal static byte[] FromBase64String(string fileRepAsString)
		{
			byte[] result;
			try
			{
				result = Convert.FromBase64String(fileRepAsString);
			}
			catch (FormatException innerException)
			{
				throw new OwaInvalidRequestException("Cannot convert from base 64", innerException);
			}
			return result;
		}

		private static void EnsureLocalServerNameAndVersion()
		{
			if (string.IsNullOrEmpty(WacUtilities.localServerName) || string.IsNullOrEmpty(WacUtilities.localServerVersion))
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 599, "EnsureLocalServerNameAndVersion", "f:\\15.00.1497\\sources\\dev\\clients\\src\\Owa2\\Server\\Core\\attachment\\WacUtilities.cs");
				Server server = topologyConfigurationSession.FindLocalServer();
				WacUtilities.localServerName = server.Fqdn;
				WacUtilities.localServerVersion = WacUtilities.ConvertVersionNumberToString(server.VersionNumber);
			}
		}

		private static string ConvertVersionNumberToString(int versionNumber)
		{
			int value = versionNumber & 32767;
			int value2 = versionNumber >> 16 & 63;
			int value3 = versionNumber >> 22 & 63;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(value3);
			stringBuilder.Append(".");
			stringBuilder.Append(value2);
			stringBuilder.Append(".");
			stringBuilder.Append(value);
			return stringBuilder.ToString();
		}

		private static bool IsContentProtected(string attachmentName, Stream contentStream)
		{
			return ProtectorsManager.Instance.IsProtected(attachmentName, contentStream) == MsoIpiResult.Protected;
		}

		public const string WopiAccessToken = "access_token";

		public const string WopiClientVersion = "X-WOPI-InterfaceVersion";

		public const string WopiRequestMachineName = "X-WOPI-MachineName";

		public const string WopiCorrelationID = "X-WOPI-CorrelationID";

		public const string WopiPerfTraceRequested = "X-WOPI-PerfTraceRequested";

		public const string WopiServerMachineName = "X-WOPI-ServerMachineName";

		public const string WopiServerError = "X-WOPI-ServerError";

		public const string WopiServerVersion = "X-WOPI-ServerVersion";

		public const string WopiPerfTrace = "X-WOPI-PerfTrace";

		public const string WopiUIParameter = "ui";

		public const string MailboxSmtpAddress = "UserEmail";

		public const string FileNameSeparator = "-";

		private const string OwnerIdPrefix = "ExchangeMbx_";

		private static string localServerName;

		private static string localServerVersion;

		public delegate void AttachmentProcessor(IExchangePrincipal exchangePrincipal, Attachment attachment, Stream stream, bool contentProtected);
	}
}
