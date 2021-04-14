using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class InstantMessagePresenceContextMenu : ContextMenu
	{
		public InstantMessagePresenceContextMenu(UserContext userContext) : base("divMeCardmnu", userContext)
		{
		}

		protected override void RenderMenuItems(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			bool flag = base.UserContext.IsSenderPhotosFeatureEnabled(Feature.SetPhoto);
			bool flag2 = base.UserContext.IsFeatureEnabled(Feature.ExplicitLogon);
			if (base.UserContext.IsInstantMessageEnabled())
			{
				if (base.UserContext.InstantMessagingType == InstantMessagingTypeOptions.Ocs)
				{
					this.RenderOCSMenuItems(output);
				}
				ContextMenu.RenderMenuDivider(output, "divD1");
				base.RenderMenuItem(output, 1846371062, ThemeFileId.None, "divSOIM", "signoutim");
				base.RenderMenuItem(output, -1727973461, ThemeFileId.None, "divSIIM", "signinim");
				if (flag || flag2)
				{
					ContextMenu.RenderMenuDivider(output, "divD2");
				}
			}
			if (flag)
			{
				base.RenderMenuItem(output, -1336564560, ThemeFileId.None, "divCDP", "changepic");
				if (flag2)
				{
					ContextMenu.RenderMenuDivider(output, "divD3");
				}
			}
			if (flag2)
			{
				base.RenderMenuItem(output, -157319556, ThemeFileId.None, "divOthrMbx", "opnMbx");
			}
		}

		private void RenderOCSMenuItems(TextWriter output)
		{
			base.RenderMenuItem(output, 394229313, ThemeFileId.PresenceAvailable, "divAVLBL", "available");
			base.RenderMenuItem(output, 2052801377, ThemeFileId.PresenceBusy, "divBSY", "busy");
			base.RenderMenuItem(output, 519406893, ThemeFileId.PresenceDoNotDisturb, "divDND", "donotdisturb");
			base.RenderMenuItem(output, -1044294068, ThemeFileId.PresenceAway, "divBRB", "berightback");
			base.RenderMenuItem(output, 796898289, ThemeFileId.PresenceAway, "divAWAY", "away");
		}

		private void RenderMSNMenuItems(TextWriter output)
		{
			base.RenderMenuItem(output, 923793291, ThemeFileId.PresenceAvailable, "divAVLBL", "online");
			base.RenderMenuItem(output, 2052801377, ThemeFileId.PresenceBusy, "divBSY", "busy");
			base.RenderMenuItem(output, 930407740, ThemeFileId.PresenceAway, "divAWAY", "away");
			base.RenderMenuItem(output, -740975890, ThemeFileId.PresenceOffline, "divOFFL", "appearoffline");
		}
	}
}
