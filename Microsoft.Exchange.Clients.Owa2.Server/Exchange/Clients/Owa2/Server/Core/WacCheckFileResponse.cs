using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract]
	internal class WacCheckFileResponse
	{
		public WacCheckFileResponse(string fileName, long fileSize, string sha256Hash, string downloadUrl, string ownerId, string userId, string userDisplayName, string userPuid, bool isMessageIrmProtected, bool directFileAccessEnabled, bool externalServicesEnabled, bool wacOMEXEnabled)
		{
			this.BaseFileName = fileName;
			this.Size = fileSize;
			this.Version = sha256Hash;
			this.SHA256 = sha256Hash;
			if (!isMessageIrmProtected && directFileAccessEnabled)
			{
				this.DownloadUrl = downloadUrl;
			}
			this.OwnerId = ownerId;
			this.UserId = userId;
			this.UserFriendlyName = userDisplayName;
			this.HostAuthenticationId = userPuid;
			this.ClientUrl = string.Empty;
			this.CloseUrl = "about:blank";
			this.HostViewUrl = string.Empty;
			this.HostEditUrl = string.Empty;
			this.HostEmbeddedViewUrl = string.Empty;
			this.HostEmbeddedEditUrl = string.Empty;
			this.EmbeddingRequiresShareChanges = false;
			this.FileUrl = string.Empty;
			this.PrivacyUrl = string.Empty;
			this.TermsOfUseUrl = string.Empty;
			this.UserCanWrite = false;
			this.ReadOnly = true;
			this.UserCanPresent = false;
			this.UserCanAttend = false;
			this.SupportsUpdate = false;
			this.SupportsLocks = false;
			this.SupportsCobalt = false;
			this.SupportsFolders = false;
			this.CloseButtonClosesWindow = false;
			this.DisableBrowserCachingOfUserContent = true;
			this.DisablePrint = !directFileAccessEnabled;
			this.ProtectInClient = isMessageIrmProtected;
			this.DisableTranslation = !externalServicesEnabled;
			this.AllowExternalMarketplace = wacOMEXEnabled;
		}

		[DataMember]
		public bool CloseButtonClosesWindow { get; set; }

		[DataMember]
		public bool DisableBrowserCachingOfUserContent { get; set; }

		[DataMember]
		public bool DisablePrint { get; set; }

		[DataMember]
		public bool ProtectInClient { get; set; }

		[DataMember]
		public string Version { get; set; }

		[DataMember]
		public string BaseFileName { get; set; }

		[DataMember]
		public string OwnerId { get; set; }

		[DataMember]
		public string UserId { get; set; }

		[DataMember]
		public string UserFriendlyName { get; set; }

		public string HostAuthenticationId { get; set; }

		[DataMember]
		public long Size { get; set; }

		[DataMember]
		public string SHA256 { get; set; }

		[DataMember]
		public string ClientUrl { get; set; }

		[DataMember]
		public string DownloadUrl { get; set; }

		[DataMember]
		public string CloseUrl { get; set; }

		[DataMember]
		public string HostViewUrl { get; set; }

		[DataMember]
		public string HostEditUrl { get; set; }

		[DataMember]
		public string HostEmbeddedViewUrl { get; set; }

		[DataMember]
		public string HostEmbeddedEditUrl { get; set; }

		public bool EmbeddingRequiresShareChanges { get; set; }

		[DataMember]
		public string FileUrl { get; set; }

		[DataMember]
		public string PrivacyUrl { get; set; }

		[DataMember]
		public string TermsOfUseUrl { get; set; }

		[DataMember]
		public bool UserCanWrite { get; set; }

		[DataMember]
		public bool ReadOnly { get; set; }

		[DataMember]
		public bool UserCanPresent { get; set; }

		[DataMember]
		public bool UserCanAttend { get; set; }

		[DataMember]
		public bool SupportsUpdate { get; set; }

		[DataMember]
		public bool SupportsLocks { get; set; }

		[DataMember]
		public bool SupportsCobalt { get; set; }

		[DataMember]
		public bool SupportsFolders { get; set; }

		[DataMember]
		public bool AllowExternalMarketplace { get; set; }

		[DataMember]
		public bool DisableTranslation { get; set; }
	}
}
