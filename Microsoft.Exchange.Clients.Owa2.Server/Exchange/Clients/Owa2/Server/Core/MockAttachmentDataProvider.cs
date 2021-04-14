using System;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[SimpleConfiguration("OWA.AttachmentDataProvider", "MockAttachmentDataProvider")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class MockAttachmentDataProvider : AttachmentDataProvider
	{
		public static MockAttachmentDataProvider CreateMockDataProvider()
		{
			return new MockAttachmentDataProvider
			{
				DisplayName = "MockDrive Pro",
				EndPointUrl = "http://MockEndPointUrl:14792",
				Id = Guid.NewGuid().ToString(),
				Type = AttachmentDataProviderType.OneDrivePro
			};
		}

		internal override GetAttachmentDataProviderItemsResponse GetItems(AttachmentItemsPagingDetails paging, AttachmentDataProviderScope scope, MailboxSession mailboxSession)
		{
			AttachmentDataProviderItem[] dummyItems = this.GetDummyItems();
			return new GetAttachmentDataProviderItemsResponse
			{
				ResultCode = AttachmentResultCode.Success,
				Items = dummyItems,
				PagingMetadata = null,
				TotalItemCount = dummyItems.Length
			};
		}

		public override AttachmentDataProviderItem[] GetRecentItems()
		{
			return this.GetDummyItems();
		}

		internal override string GetItemAbsoulteUrl(UserContext userContext, string location, string providerEndpointUrl, string itemId = null, string parentItemId = null)
		{
			string arg = string.IsNullOrEmpty(providerEndpointUrl) ? base.EndPointUrl : providerEndpointUrl;
			return string.Format("{0}{1}", arg, location);
		}

		public override Task<DownloadItemAsyncResult> DownloadItemAsync(string location, string itemId, string parentItemId, string providerEndpointUrl, CancellationToken cancellationToken)
		{
			return Task.Run<DownloadItemAsyncResult>(() => new DownloadItemAsyncResult
			{
				ResultCode = AttachmentResultCode.Success,
				Item = new FileAttachmentDataProviderItem
				{
					Name = "file.txt",
					AttachmentProviderId = this.Id,
					Location = "/dummy/location/file.txt",
					ProviderType = this.Type,
					Size = 3L,
					ProviderEndpointUrl = (string.IsNullOrEmpty(providerEndpointUrl) ? this.EndPointUrl : providerEndpointUrl),
					Id = itemId
				},
				Bytes = new byte[]
				{
					1,
					2,
					3
				}
			}, cancellationToken);
		}

		internal override Task<UploadItemAsyncResult> UploadItemAsync(byte[] file, string fileName, CancellationToken cancellationToken, CallContext callContext)
		{
			return Task.Run<UploadItemAsyncResult>(() => this.UploadItemSync(file, fileName, cancellationToken), cancellationToken);
		}

		public override UploadItemAsyncResult UploadItemSync(byte[] file, string fileName, CancellationToken cancellationToken)
		{
			return new UploadItemAsyncResult
			{
				ResultCode = AttachmentResultCode.Success,
				Item = new FileAttachmentDataProviderItem
				{
					Name = fileName,
					AttachmentProviderId = base.Id,
					Location = this.GetMockLocation(fileName),
					ProviderType = base.Type,
					Size = file.LongLength,
					ProviderEndpointUrl = base.EndPointUrl,
					Id = "10"
				}
			};
		}

		public override bool FileExists(string fileUrl)
		{
			return true;
		}

		public override Task<UpdatePermissionsAsyncResult> UpdateDocumentPermissionsAsync(string[] resources, AttachmentPermissionAssignment[] attachmentPermissionAssignments, CancellationToken cancellationToken)
		{
			return Task.Run<UpdatePermissionsAsyncResult>(() => new UpdatePermissionsAsyncResult
			{
				ResultCode = AttachmentResultCode.Success
			}, cancellationToken);
		}

		internal override void PostInitialize(GetAttachmentDataProvidersRequest request)
		{
		}

		public string GetMockLocation(string fileName)
		{
			return string.Format("/dummy/location/{0}", fileName);
		}

		public override string GetEndpointUrlFromItemLocation(string location)
		{
			return base.EndPointUrl;
		}

		private AttachmentDataProviderItem[] GetDummyItems()
		{
			return new AttachmentDataProviderItem[]
			{
				new FolderAttachmentDataProviderItem
				{
					Name = "folder",
					AttachmentProviderId = base.Id,
					Location = "/dummy/location/folder",
					ProviderType = base.Type,
					ProviderEndpointUrl = base.EndPointUrl,
					Id = "1"
				},
				new FolderAttachmentDataProviderItem
				{
					Name = "folder1",
					AttachmentProviderId = base.Id,
					Location = "/dummy/location/folder1",
					ProviderType = base.Type,
					ProviderEndpointUrl = base.EndPointUrl,
					Id = "2"
				},
				new FileAttachmentDataProviderItem
				{
					Name = "file.txt",
					AttachmentProviderId = base.Id,
					Location = "/dummy/location/file.txt",
					ProviderType = base.Type,
					Size = 299L,
					ProviderEndpointUrl = base.EndPointUrl,
					Id = "3"
				},
				new FileAttachmentDataProviderItem
				{
					Name = "file_image.jpg",
					AttachmentProviderId = base.Id,
					Location = "/dummy/location/file_image.jpg",
					ProviderType = base.Type,
					Size = 299L,
					ProviderEndpointUrl = base.EndPointUrl,
					Id = "4"
				},
				new FileAttachmentDataProviderItem
				{
					Name = "file_word.docx",
					AttachmentProviderId = base.Id,
					Location = "/dummy/location/file_word.docx",
					ProviderType = base.Type,
					Size = 299L,
					ProviderEndpointUrl = base.EndPointUrl,
					Id = "5"
				},
				new FileAttachmentDataProviderItem
				{
					Name = "file_excel.xlsx",
					AttachmentProviderId = base.Id,
					Location = "/dummy/location/file_excel.xlsx",
					ProviderType = base.Type,
					Size = 299L,
					ProviderEndpointUrl = base.EndPointUrl,
					Id = "6"
				},
				new FileAttachmentDataProviderItem
				{
					Name = "file_powerpoint.pptx",
					AttachmentProviderId = base.Id,
					Location = "/dummy/location/file_powerpoint.pptx",
					ProviderType = base.Type,
					Size = 299L,
					ProviderEndpointUrl = base.EndPointUrl,
					Id = "7"
				},
				new FileAttachmentDataProviderItem
				{
					Name = "file_pdf.pdf",
					AttachmentProviderId = base.Id,
					Location = "/dummy/location/file_pdf.pdf",
					ProviderType = base.Type,
					Size = 299L,
					ProviderEndpointUrl = base.EndPointUrl,
					Id = "8"
				}
			};
		}
	}
}
