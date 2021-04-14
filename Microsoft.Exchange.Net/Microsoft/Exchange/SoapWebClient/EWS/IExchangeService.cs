using System;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[CLSCompliant(false)]
	public interface IExchangeService
	{
		int Timeout { get; set; }

		string Url { get; set; }

		GetFolderResponseType GetFolder(GetFolderType getFolder);

		CreateFolderResponseType CreateFolder(CreateFolderType createFolder);

		CreateItemResponseType CreateItem(CreateItemType createItem);

		FindFolderResponseType FindFolder(FindFolderType findFolder);

		FindItemResponseType FindItem(FindItemType findItem);

		GetItemResponseType GetItem(GetItemType getItem);

		ExportItemsResponseType ExportItems(ExportItemsType items);

		UploadItemsResponseType UploadItems(UploadItemsType items);
	}
}
