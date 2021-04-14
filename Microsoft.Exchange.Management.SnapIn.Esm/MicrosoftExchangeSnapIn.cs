using System;
using Microsoft.Exchange.Management.SnapIn.Esm.Toolbox;
using Microsoft.Exchange.Management.SystemManager;
using Microsoft.Exchange.ManagementGUI;
using Microsoft.ManagementConsole;

namespace Microsoft.Exchange.Management.SnapIn.Esm
{
	[SnapInAbout("Microsoft.Exchange.Management.NativeResources.dll", ApplicationBaseRelative = true, VendorId = 101, VersionId = 102, DisplayNameId = 903, DescriptionId = 904, IconId = 910, LargeFolderBitmapId = 911, SmallFolderBitmapId = 912, SmallFolderSelectedBitmapId = 912, FolderBitmapsColorMask = 16711935)]
	[PublishesNodeType("714FA079-DC14-470f-851C-B7EAAA4177C1", Description = "Microsoft Exchange")]
	[SnapInSettings("714FA079-DC14-470f-851C-B7EAAA4177C1", DisplayName = "Microsoft Exchange", Description = "Microsoft Exchange.", UseCustomHelp = true)]
	public class MicrosoftExchangeSnapIn : ExchangeSnapIn
	{
		public MicrosoftExchangeSnapIn()
		{
			GC.KeepAlive(ManagementGuiSqmSession.Instance);
			base.RootNode = new ToolboxNode(EnvironmentAnalyzer.IsWorkGroup());
		}

		public override string SnapInGuidString
		{
			get
			{
				return "714FA079-DC14-470f-851C-B7EAAA4177C1";
			}
		}

		public const string NodeGuid = "714FA079-DC14-470f-851C-B7EAAA4177C1";

		public const string SnapInDisplayName = "Microsoft Exchange";

		public const string SnapInDescription = "Microsoft Exchange.";
	}
}
