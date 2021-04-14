using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.TextConverters;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal abstract class OwaSafeHtmlCallbackBase : HtmlCallbackBase
	{
		public OwaSafeHtmlCallbackBase()
		{
			this.Initialize();
		}

		public OwaSafeHtmlCallbackBase(Item item) : base(item)
		{
			this.Initialize();
		}

		public OwaSafeHtmlCallbackBase(AttachmentCollection attachmentCollection, Body body) : base(attachmentCollection, body)
		{
			this.Initialize();
		}

		private void Initialize()
		{
			base.ClearInlineOnUnmarkedAttachments = true;
			base.RemoveUnlinkedAttachments = false;
			OwaSafeHtmlCallbackBase.blankImageFileName = ThemeManager.GetBaseThemeFileUrl(ThemeFileId.Clear1x1);
			base.InitializeAttachmentLinks(null);
		}

		protected static bool IsUrlTag(HtmlTagId tagId, HtmlTagContextAttribute attribute)
		{
			return (tagId == HtmlTagId.A && attribute.Id == HtmlAttributeId.Href) || (tagId == HtmlTagId.Area && attribute.Id == HtmlAttributeId.Href);
		}

		protected static bool IsImageTag(HtmlTagId tagId, HtmlTagContextAttribute attribute)
		{
			return (tagId == HtmlTagId.Img && attribute.Id == HtmlAttributeId.Src) || (tagId == HtmlTagId.Img && attribute.Id == HtmlAttributeId.DynSrc) || (tagId == HtmlTagId.Img && attribute.Id == HtmlAttributeId.LowSrc);
		}

		protected static bool IsBackgroundAttribute(HtmlTagContextAttribute attribute)
		{
			return attribute.Id == HtmlAttributeId.Background;
		}

		protected static bool IsSanitizingAttribute(HtmlTagContextAttribute attribute)
		{
			return attribute.Id == HtmlAttributeId.Border || attribute.Id == HtmlAttributeId.Width || attribute.Id == HtmlAttributeId.Height;
		}

		public static bool IsBaseTag(HtmlTagId tagId, HtmlTagContextAttribute attribute)
		{
			return tagId == HtmlTagId.Base && attribute.Id == HtmlAttributeId.Href;
		}

		public virtual bool HasBlockedImages
		{
			get
			{
				return this.hasBlockedImages;
			}
		}

		public bool HasBlockedInlineAttachments
		{
			get
			{
				return this.hasBlockedInlineAttachments;
			}
		}

		public virtual bool HasRtfEmbeddedImages
		{
			get
			{
				return this.hasRtfEmbeddedImages;
			}
		}

		public bool ApplyAttachmentsUpdates(Item item)
		{
			bool flag = false;
			if (base.NeedsSave() && !Utilities.IsClearSigned(item))
			{
				item.OpenAsReadWrite();
				CalendarItemBase calendarItemBase = item as CalendarItemBase;
				if (calendarItemBase != null)
				{
					Utilities.ValidateCalendarItemBaseStoreObject(calendarItemBase);
				}
				try
				{
					flag = this.SaveChanges();
				}
				catch (AccessDeniedException)
				{
				}
				if (flag)
				{
					try
					{
						Utilities.SaveItem(item, false);
					}
					catch (AccessDeniedException)
					{
					}
				}
			}
			return flag;
		}

		protected static readonly string LocalUrlPrefix = "#";

		protected static readonly string DoubleBlank = "  ";

		protected static readonly string AttachmentBaseUrl = "attachment.ashx?id=";

		protected static readonly string JSLocalLink = "javascript:parent.onLocalLink";

		protected static readonly string JSMethodPrefix = "('";

		protected static readonly string JSMethodSuffix = "',window.frameElement)";

		protected static string blankImageFileName;

		protected bool hasBlockedImages;

		protected bool hasBlockedInlineAttachments;

		protected bool hasRtfEmbeddedImages;

		protected string inlineRTFattachmentScheme = "objattph://";

		protected string embeddedRTFImage = "rtfimage://";

		protected string inlineHTMLAttachmentScheme = "cid:";
	}
}
