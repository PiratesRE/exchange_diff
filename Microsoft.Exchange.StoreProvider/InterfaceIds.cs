using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class InterfaceIds
	{
		public static readonly Guid IStreamGuid = new Guid("0000000c-0000-0000-C000-000000000046");

		public static readonly Guid IStorageGuid = new Guid("0000000b-0000-0000-C000-000000000046");

		public static readonly Guid IMessageGuid = new Guid("00020307-0000-0000-C000-000000000046");

		public static readonly Guid IMAPIFolderGuid = new Guid("0002030c-0000-0000-C000-000000000046");

		public static readonly Guid IAttachGuid = new Guid("00020308-0000-0000-C000-000000000046");

		public static readonly Guid IMAPIContainerGuid = new Guid("0002030B-0000-0000-C000-000000000046");

		public static readonly Guid IMsgStoreGuid = new Guid("00020306-0000-0000-C000-000000000046");

		public static readonly Guid IExchangeModifyTable = new Guid("2d734cb0-53fd-101b-b19d-08002b3056e3");

		public static readonly Guid IExchangeExportChanges = new Guid("a3ea9cc0-d1b2-11cd-80fc-00aa004bba0b");

		public static readonly Guid IExchangeExportManifest = new Guid("82D370F5-6F10-457d-99F9-11977856A7AA");

		public static readonly Guid IExchangeExportManifestEx = new Guid("17E58114-B412-40ac-918C-C0B170DD2026");

		public static readonly Guid IExchangeExportHierManifestEx = new Guid("2DC76CDD-1AA6-4157-808F-E68D2AD29FE8");

		public static readonly Guid IExchangeImportContentsChanges = new Guid("f75abfa0-d0e0-11cd-80fc-00aa004bba0b");

		public static readonly Guid IExchangeImportContentsChanges3 = new Guid("361487fc-888a-4746-8ab3-2a198c91585a");

		public static readonly Guid IExchangeImportContentsChanges4 = new Guid("F5F9FFFE-D1AF-45d3-B790-E4D489D38B7E");

		public static readonly Guid IExchangeImportHierarchyChanges = new Guid("85a66cf0-d0e0-11cd-80fc-00aa004bba0b");

		public static readonly Guid IExchangeImportHierarchyChanges2 = new Guid("7846EDBA-8287-4d76-BD5F-1E0513D10E0C");

		public static readonly Guid IExchangeMessageConversion = new Guid("3532b360-d114-11cf-a83b-00c04fd65597");

		public static readonly Guid IExRpcConnection = new Guid("DCBB456B-FBDA-4c0c-BCF2-90EEF6BDCC07");

		public static readonly Guid IExRpcMessage = new Guid("83BB0082-568A-4227-A830-C1A3844B9331");

		public static readonly Guid IExRpcFolder = new Guid("E9972C72-4A7D-464c-9350-ADD5ABABF6D8");

		public static readonly Guid IExRpcTable = new Guid("E2E6C3BD-835E-4921-9F86-A08DBAB67EB7");

		public static readonly Guid IExRpcMsgStore = new Guid("37FB08C3-F6C8-4de8-B8DA-AB7E41D01ECE");

		public static readonly Guid ILastErrorInfo = new Guid("42A2AEE7-E53B-49e3-9011-8DF591F16085");

		public static readonly Guid IExchangeFastTransferEx = new Guid("1AD3079C-5325-4b68-A57E-E8FF2BD58E53");

		public static readonly Guid IExchangeExportContentsChangesEx = new Guid("C4BB0442-D823-4e4c-81AD-059072399DC5");

		public static readonly Guid IExchangeExportHierarchyChangesEx = new Guid("72616CCF-43D6-4f02-9486-A23E89965973");

		public static readonly Guid IFastTransferStream = new Guid("a91d38a5-c92b-45eb-8426-1bfa5b17bc3c");
	}
}
