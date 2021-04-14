using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public abstract class PolicyContextMenuBase : ContextMenu
	{
		private protected bool InheritChecked { protected get; private set; }

		private protected Guid PolicyChecked { protected get; private set; }

		protected PolicyContextMenuBase(string menuId, UserContext userContext, string clientClassName) : base(menuId, userContext)
		{
			if (string.IsNullOrEmpty(clientClassName))
			{
				throw new ArgumentException("clientClassName");
			}
			this.clientScript = "Owa.UIControls." + clientClassName + ".get_Instance(arguments[0]);";
			this.shouldScroll = true;
		}

		internal static string GetDefaultDisplayName(PolicyTag policyTag)
		{
			return string.Format(LocalizedStrings.GetNonEncoded(-1468060031), policyTag.Name, PolicyContextMenuBase.TimeSpanToString(policyTag.TimeSpanForRetention));
		}

		internal static string TimeSpanToString(EnhancedTimeSpan timeSpan)
		{
			if (timeSpan == EnhancedTimeSpan.Zero)
			{
				return LocalizedStrings.GetNonEncoded(1491852291);
			}
			int num = (int)(timeSpan.TotalDays / 365.0);
			if (num > 0)
			{
				return string.Format(LocalizedStrings.GetNonEncoded((num > 1) ? 376120709 : 1543927794), num);
			}
			int num2 = (int)(timeSpan.TotalDays % 365.0 / 30.0);
			if (num2 > 0)
			{
				return string.Format(LocalizedStrings.GetNonEncoded((num2 > 1) ? 1489284924 : -228948015), num2);
			}
			int num3 = (int)timeSpan.TotalDays;
			return string.Format(LocalizedStrings.GetNonEncoded((num3 > 1) ? -620305904 : -912690831), num3);
		}

		internal void SetStates(bool isInherited, Guid? tagGuid)
		{
			if (isInherited || tagGuid == null)
			{
				this.InheritChecked = true;
				this.PolicyChecked = Guid.Empty;
				return;
			}
			this.InheritChecked = false;
			this.PolicyChecked = tagGuid.Value;
		}

		protected void RenderPolicyTagMenuItem(TextWriter output, Guid policyGuid, string policyDisplayName, bool disableChecked)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (policyGuid == Guid.Empty)
			{
				throw new ArgumentException("policyGuid");
			}
			if (string.IsNullOrEmpty(policyDisplayName))
			{
				throw new ArgumentException("policyDisplayName");
			}
			base.RenderMenuItem(output, policyDisplayName, object.Equals(policyGuid, this.PolicyChecked) ? ThemeFileId.MeetingAccept : ThemeFileId.Clear, policyGuid.ToString(), policyGuid.ToString(), disableChecked, null, null);
		}

		protected void RenderInheritPolicyMenuItem(TextWriter output, bool withDivider, bool disableChecked)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (withDivider)
			{
				ContextMenu.RenderMenuDivider(output, "divS1");
			}
			base.RenderMenuItem(output, 1562439829, this.InheritChecked ? ThemeFileId.MeetingAccept : ThemeFileId.Clear, "inheritPolicy", "inheritPolicy", disableChecked);
		}

		private const string InheritPolicy = "inheritPolicy";

		private const ThemeFileId Checked = ThemeFileId.MeetingAccept;

		private const ThemeFileId Unchecked = ThemeFileId.Clear;
	}
}
