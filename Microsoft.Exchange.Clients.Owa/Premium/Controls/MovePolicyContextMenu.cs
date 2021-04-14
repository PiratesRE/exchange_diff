using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class MovePolicyContextMenu : PolicyContextMenuBase
	{
		private MovePolicyContextMenu(UserContext userContext) : base("divMvPtgM", userContext, "MovePolicyContextMenu")
		{
		}

		internal static MovePolicyContextMenu Create(UserContext userContext)
		{
			return new MovePolicyContextMenu(userContext);
		}

		internal static void RenderAsSubmenu(TextWriter output, UserContext userContext, RenderMenuItemDelegate renderMenuItem)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (renderMenuItem == null)
			{
				throw new ArgumentNullException("renderMenuItem");
			}
			if (PolicyProvider.MovePolicyProvider.IsPolicyEnabled(userContext.MailboxSession) && Utilities.HasArchive(userContext))
			{
				renderMenuItem(output, 1060619217, ThemeFileId.None, "divMvPtg", null, false, null, null, MovePolicyContextMenu.Create(userContext));
			}
		}

		protected override void RenderMenuItems(TextWriter output)
		{
			if (PolicyProvider.MovePolicyProvider.IsPolicyEnabled(this.userContext.MailboxSession))
			{
				PolicyTagList allPolicies = PolicyProvider.MovePolicyProvider.GetAllPolicies(this.userContext.MailboxSession);
				List<PolicyTag> list = new List<PolicyTag>(allPolicies.Values.Count);
				foreach (PolicyTag policyTag in allPolicies.Values)
				{
					if (policyTag.IsVisible || object.Equals(policyTag.PolicyGuid, base.PolicyChecked))
					{
						list.Add(policyTag);
					}
				}
				list.Sort(new Comparison<PolicyTag>(MovePolicyContextMenu.CompareMovePolicyValues));
				foreach (PolicyTag policyTag2 in list)
				{
					base.RenderPolicyTagMenuItem(output, policyTag2.PolicyGuid, PolicyContextMenuBase.GetDefaultDisplayName(policyTag2), false);
				}
				base.RenderInheritPolicyMenuItem(output, true, false);
			}
		}

		private static int CompareMovePolicyValues(PolicyTag policyTag1, PolicyTag policyTag2)
		{
			return policyTag1.TimeSpanForRetention.CompareTo(policyTag2.TimeSpanForRetention);
		}
	}
}
