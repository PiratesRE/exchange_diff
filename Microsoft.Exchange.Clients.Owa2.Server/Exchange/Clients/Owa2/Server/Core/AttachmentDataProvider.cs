using System;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[KnownType(typeof(OneDriveProAttachmentDataProvider))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[KnownType(typeof(MockAttachmentDataProvider))]
	[SimpleConfiguration("OWA.AttachmentDataProvider", "AttachmentDataProvider")]
	public class AttachmentDataProvider
	{
		public AttachmentDataProvider()
		{
		}

		public AttachmentDataProvider(string id, AttachmentDataProviderType type)
		{
			this.Id = id;
			this.Type = type;
		}

		[SimpleConfigurationProperty("id")]
		[DataMember]
		public string Id { get; set; }

		[DataMember]
		[SimpleConfigurationProperty("type")]
		public AttachmentDataProviderType Type { get; set; }

		[SimpleConfigurationProperty("EndPointUrl")]
		[DataMember]
		public string EndPointUrl { get; set; }

		[SimpleConfigurationProperty("displayName")]
		[DataMember]
		public string DisplayName { get; set; }

		[DataMember]
		public AttachmentDataProviderScope[] Scopes { get; set; }

		[DataMember]
		public int DefaultScopeIndex { get; set; }

		public virtual bool FileExists(string fileUrl)
		{
			throw new NotImplementedException();
		}

		internal virtual string GetLinkingUrl(UserContext userContext, string fileUrl, string endpointUrl, string itemId = null, string parentItemId = null)
		{
			throw new NotImplementedException();
		}

		internal virtual GetAttachmentDataProviderItemsResponse GetItems(AttachmentItemsPagingDetails paging, AttachmentDataProviderScope scope, MailboxSession mailboxSession)
		{
			throw new NotImplementedException();
		}

		public virtual Task<DownloadItemAsyncResult> DownloadItemAsync(string location, string itemId, string parentItemId, string providerEndpointUrl, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		internal virtual Task<UploadItemAsyncResult> UploadItemAsync(byte[] file, string fileName, CancellationToken cancellationToken, CallContext callContext)
		{
			throw new NotImplementedException();
		}

		public virtual UploadItemAsyncResult UploadItemSync(byte[] fileContent, string fileName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public virtual Task<UpdatePermissionsAsyncResult> UpdateDocumentPermissionsAsync(string[] resources, AttachmentPermissionAssignment[] attachmentPermissionAssignments, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public virtual AttachmentDataProviderItem[] GetRecentItems()
		{
			throw new NotImplementedException();
		}

		public virtual string GetEndpointUrlFromItemLocation(string location)
		{
			throw new NotImplementedException();
		}

		internal virtual string GetItemAbsoulteUrl(UserContext userContext, string location, string providerEndpointUrl, string itemId = null, string parentItemId = null)
		{
			throw new NotImplementedException();
		}

		internal virtual void PostInitialize(GetAttachmentDataProvidersRequest request)
		{
			throw new NotImplementedException();
		}
	}
}
