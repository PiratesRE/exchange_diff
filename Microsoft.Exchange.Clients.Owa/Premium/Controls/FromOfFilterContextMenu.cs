using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class FromOfFilterContextMenu : ContextMenu
	{
		public FromOfFilterContextMenu(UserContext userContext) : base("divFltrFrmMnu", userContext)
		{
			this.recipientCache = AutoCompleteCache.TryGetCache(userContext);
			if (this.recipientCache != null)
			{
				this.recipientCache.Sort();
			}
		}

		protected override void RenderMenuItemExpandoData(TextWriter output)
		{
			if (this.recipientCache == null || this.currentMenuItemIndex >= this.recipientCache.CacheLength || this.currentMenuItemIndex >= 10)
			{
				return;
			}
			RecipientInfoCacheEntry recipientInfoCacheEntry = this.recipientCache.CacheEntries[this.currentMenuItemIndex];
			output.Write(" _dn=\"");
			Utilities.HtmlEncode((recipientInfoCacheEntry.DisplayName == null) ? string.Empty : recipientInfoCacheEntry.DisplayName, output);
			output.Write("\" _em=\"");
			Utilities.HtmlEncode((recipientInfoCacheEntry.RoutingAddress == null) ? string.Empty : recipientInfoCacheEntry.RoutingAddress, output);
			output.Write("\" _rt=\"");
			Utilities.HtmlEncode(recipientInfoCacheEntry.RoutingType, output);
			output.Write("\" _sa=\"");
			Utilities.HtmlEncode((recipientInfoCacheEntry.SmtpAddress == null) ? string.Empty : recipientInfoCacheEntry.SmtpAddress, output);
			output.Write("\" _ao=");
			output.Write((int)recipientInfoCacheEntry.AddressOrigin);
			output.Write(" _rf=");
			output.Write(recipientInfoCacheEntry.RecipientFlags);
			output.Write(" _id=\"");
			Utilities.HtmlEncode((recipientInfoCacheEntry.ItemId == null) ? string.Empty : recipientInfoCacheEntry.ItemId, output);
			output.Write("\" _ei=");
			output.Write((int)recipientInfoCacheEntry.EmailAddressIndex);
			output.Write(" _uri=\"");
			Utilities.HtmlEncode((recipientInfoCacheEntry.SipUri == null) ? string.Empty : recipientInfoCacheEntry.SipUri, output);
			output.Write("\" _mo=\"");
			Utilities.HtmlEncode((recipientInfoCacheEntry.MobilePhoneNumber == null) ? string.Empty : recipientInfoCacheEntry.MobilePhoneNumber, output);
			output.Write("\"");
		}

		protected override void RenderMenuItems(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			output.Write("<div _lnk=0 id=\"divFrmTtl\">");
			output.Write(LocalizedStrings.GetHtmlEncoded(-729399910));
			output.Write("</div>");
			output.Write("<div class=\"cmLnk cmRcp\" _lnk=0><span class=cmIco id=spnImg>");
			this.userContext.RenderThemeImage(output, ThemeFileId.AddressBook, "curPointer", new object[]
			{
				"id=imgAddrBook"
			});
			output.Write("</span>");
			EmptyRecipientWell emptyRecipientWell = new EmptyRecipientWell();
			emptyRecipientWell.Render(output, base.UserContext, RecipientWellType.To, RecipientWell.RenderFlags.None, "FltrRw");
			output.Write("</div>");
			bool flag = false;
			if (!this.userContext.IsFeatureEnabled(Feature.Contacts))
			{
				flag = true;
			}
			bool flag2 = false;
			if (this.recipientCache != null)
			{
				this.currentMenuItemIndex = 0;
				while (this.currentMenuItemIndex < this.recipientCache.CacheLength && this.currentMenuItemIndex < 10)
				{
					if ((!flag || this.recipientCache.CacheEntries[this.currentMenuItemIndex].AddressOrigin != AddressOrigin.Store) && !(this.recipientCache.CacheEntries[this.currentMenuItemIndex].RoutingType == "MAPIPDL"))
					{
						if (!flag2)
						{
							ContextMenu.RenderMenuDivider(output, "divS41");
						}
						flag2 = true;
						base.RenderMenuItem(output, this.recipientCache.CacheEntries[this.currentMenuItemIndex].DisplayName, ThemeFileId.None, "fltrrcp" + this.currentMenuItemIndex, "fltrrcp" + this.currentMenuItemIndex, false, null, null);
					}
					this.currentMenuItemIndex++;
				}
			}
		}

		private const int MaxItemCount = 10;

		private readonly RecipientCache recipientCache;

		private int currentMenuItemIndex;
	}
}
