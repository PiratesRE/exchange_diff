using System;
using Microsoft.Exchange.Management.SnapIn;
using Microsoft.ManagementConsole;

namespace Microsoft.Exchange.Management.Edge.SystemManager
{
	[SnapInSettings("7F96F1BD-0EDA-4cb0-8393-632B83BD54EE", DisplayName = "Exchange Queue Viewer", Description = "Allow management of Exchange transport queues", UseCustomHelp = true)]
	[SnapInAbout("Microsoft.Exchange.Management.NativeResources.dll", ApplicationBaseRelative = true, VendorId = 101, VersionId = 102, DisplayNameId = 103, DescriptionId = 104, IconId = 110, LargeFolderBitmapId = 111, SmallFolderBitmapId = 112, SmallFolderSelectedBitmapId = 112, FolderBitmapsColorMask = 16711935)]
	public sealed class QueueViewerSnapIn : ExchangeDynamicServerSnapIn
	{
		public QueueViewerSnapIn()
		{
			base.RootNode = new QueueViewerNode();
		}

		public override string SnapInGuidString
		{
			get
			{
				return "7F96F1BD-0EDA-4cb0-8393-632B83BD54EE";
			}
		}

		public const string SnapInGuid = "7F96F1BD-0EDA-4cb0-8393-632B83BD54EE";

		public const string SnapInDisplayName = "Exchange Queue Viewer";

		public const string SnapInDescription = "Allow management of Exchange transport queues";
	}
}
