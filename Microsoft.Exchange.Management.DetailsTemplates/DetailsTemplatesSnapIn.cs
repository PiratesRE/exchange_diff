using System;
using Microsoft.Exchange.Management.SnapIn;
using Microsoft.ManagementConsole;

namespace Microsoft.Exchange.Management.DetailsTemplates
{
	[SnapInAbout("Microsoft.Exchange.Management.NativeResources.dll", ApplicationBaseRelative = true, VendorId = 101, VersionId = 102, DisplayNameId = 1013, DescriptionId = 1014, IconId = 1010, LargeFolderBitmapId = 1011, SmallFolderBitmapId = 1012, SmallFolderSelectedBitmapId = 1012, FolderBitmapsColorMask = 16711935)]
	[SnapInSettings("8AC8AAAE-D130-48f2-9CD4-9375DA3F9BAE", DisplayName = "Details Templates Editor", Description = "Allows management of Exchange Server 2007 Details Templates. © 2006 Microsoft Corporation. All rights reserved.", UseCustomHelp = true)]
	public class DetailsTemplatesSnapIn : ExchangeDynamicServerSnapIn
	{
		public DetailsTemplatesSnapIn()
		{
			base.RootNode = new DetailsTemplatesRootNode();
		}

		public override string SnapInGuidString
		{
			get
			{
				return "8AC8AAAE-D130-48f2-9CD4-9375DA3F9BAE";
			}
		}

		public const string SnapInGuid = "8AC8AAAE-D130-48f2-9CD4-9375DA3F9BAE";

		public const string SnapInDisplayName = "Details Templates Editor";

		public const string SnapInDescription = "Allows management of Exchange Server 2007 Details Templates. © 2006 Microsoft Corporation. All rights reserved.";
	}
}
