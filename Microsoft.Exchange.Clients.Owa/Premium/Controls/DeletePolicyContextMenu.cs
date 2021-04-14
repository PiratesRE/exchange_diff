using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class DeletePolicyContextMenu : PolicyContextMenuBase
	{
		private DeletePolicyContextMenu(UserContext userContext) : base("divDelPtgM", userContext, "DeletePolicyContextMenu")
		{
		}

		internal static DeletePolicyContextMenu Create(UserContext userContext)
		{
			return new DeletePolicyContextMenu(userContext);
		}

		internal bool RenderCheckedOnly { get; set; }

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
			if (PolicyProvider.DeletePolicyProvider.IsPolicyEnabled(userContext.MailboxSession))
			{
				renderMenuItem(output, 1463778657, ThemeFileId.None, "divDelPtg", null, false, null, null, DeletePolicyContextMenu.Create(userContext));
			}
		}

		protected override void RenderMenuItems(TextWriter output)
		{
			if (PolicyProvider.DeletePolicyProvider.IsPolicyEnabled(this.userContext.MailboxSession))
			{
				PolicyTagList allPolicies = PolicyProvider.DeletePolicyProvider.GetAllPolicies(this.userContext.MailboxSession);
				List<PolicyTag> list = new List<PolicyTag>(allPolicies.Values.Count);
				foreach (PolicyTag policyTag in allPolicies.Values)
				{
					if ((!this.RenderCheckedOnly && policyTag.IsVisible) || object.Equals(policyTag.PolicyGuid, base.PolicyChecked))
					{
						list.Add(policyTag);
					}
				}
				list.Sort(new Comparison<PolicyTag>(DeletePolicyContextMenu.CompareDeletePolicyValues));
				foreach (PolicyTag policyTag2 in list)
				{
					base.RenderPolicyTagMenuItem(output, policyTag2.PolicyGuid, PolicyContextMenuBase.GetDefaultDisplayName(policyTag2), this.RenderCheckedOnly);
				}
				if (!this.RenderCheckedOnly || base.InheritChecked)
				{
					base.RenderInheritPolicyMenuItem(output, !this.RenderCheckedOnly, this.RenderCheckedOnly);
				}
			}
		}

		private static int CompareDeletePolicyValues(PolicyTag policyTag1, PolicyTag policyTag2)
		{
			return string.Compare(policyTag1.Name, policyTag2.Name, StringComparison.CurrentCultureIgnoreCase);
		}
	}
}
