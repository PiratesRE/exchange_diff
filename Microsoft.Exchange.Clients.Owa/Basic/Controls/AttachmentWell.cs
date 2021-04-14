using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	public sealed class AttachmentWell
	{
		private AttachmentWell()
		{
		}

		public static void RenderAttachmentWell(TextWriter output, AttachmentWellType wellType, ArrayList attachmentList, string itemId, UserContext userContext)
		{
			AttachmentWell.RenderAttachmentWell(output, wellType, attachmentList, itemId, userContext, AttachmentWell.AttachmentWellFlags.None);
		}

		public static void RenderAttachmentWell(TextWriter output, AttachmentWellType wellType, ArrayList attachmentList, string itemId, UserContext userContext, AttachmentWell.AttachmentWellFlags flags)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (itemId == null)
			{
				throw new ArgumentNullException("itemId");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (wellType == AttachmentWellType.ReadOnly && AttachmentUtility.IsLevelOneAndBlockOnly(attachmentList))
			{
				return;
			}
			output.Write("<div id=\"divAtt\" class=\"aw\">");
			AttachmentWell.RenderAttachments(output, wellType, attachmentList, itemId, userContext, flags);
			output.Write("</div>");
		}

		internal static ArrayList GetAttachmentInformation(Item item, IList<AttachmentLink> attachmentLinks, bool isLoggedOnFromPublicComputer)
		{
			return AttachmentWell.GetAttachmentInformation(item, attachmentLinks, isLoggedOnFromPublicComputer, false);
		}

		internal static ArrayList GetAttachmentInformation(Item item, IList<AttachmentLink> attachmentLinks, bool isLoggedOnFromPublicComputer, bool isEmbeddedItem)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (item.AttachmentCollection == null)
			{
				return null;
			}
			int count = item.AttachmentCollection.Count;
			ArrayList arrayList = new ArrayList();
			return AttachmentUtility.GetAttachmentList(item, attachmentLinks, isLoggedOnFromPublicComputer, isEmbeddedItem, false);
		}

		private static void RenderAttachments(TextWriter output, AttachmentWellType wellType, ArrayList attachmentList, string itemId, UserContext userContext, AttachmentWell.AttachmentWellFlags flags)
		{
			if (attachmentList == null)
			{
				return;
			}
			int count = attachmentList.Count;
			if (count <= 0)
			{
				return;
			}
			if (Utilities.GetEmbeddedDepth(HttpContext.Current.Request) >= AttachmentPolicy.MaxEmbeddedDepth)
			{
				flags |= AttachmentWell.AttachmentWellFlags.RenderReachedMaxEmbeddedDepth;
			}
			ArrayList previousAttachmentDisplayNames = new ArrayList();
			bool prependSemicolon = false;
			foreach (object obj in attachmentList)
			{
				AttachmentWellInfo attachmentWellInfo = (AttachmentWellInfo)obj;
				AttachmentUtility.AttachmentLinkFlags attachmentLinkFlag = AttachmentUtility.GetAttachmentLinkFlag(wellType, attachmentWellInfo);
				if (AttachmentUtility.AttachmentLinkFlags.Skip != (AttachmentUtility.AttachmentLinkFlags.Skip & attachmentLinkFlag) && (!attachmentWellInfo.IsInline || (flags & AttachmentWell.AttachmentWellFlags.RenderInLine) == AttachmentWell.AttachmentWellFlags.RenderInLine) && ((flags & AttachmentWell.AttachmentWellFlags.RenderInLine) == AttachmentWell.AttachmentWellFlags.RenderInLine || (!attachmentWellInfo.IsInline && attachmentWellInfo.AttachmentType != AttachmentType.Ole)))
				{
					Item item = null;
					ItemAttachment itemAttachment = null;
					try
					{
						if (attachmentWellInfo.AttachmentType == AttachmentType.EmbeddedMessage)
						{
							itemAttachment = (ItemAttachment)attachmentWellInfo.OpenAttachment();
							item = itemAttachment.GetItemAsReadOnly(null);
						}
						if (item != null)
						{
							AttachmentWell.RenderAttachmentLinkForItem(output, attachmentWellInfo, item, itemId, userContext, previousAttachmentDisplayNames, flags, prependSemicolon);
						}
						else
						{
							AttachmentWell.RenderAttachmentLink(output, wellType, attachmentWellInfo, itemId, userContext, previousAttachmentDisplayNames, flags | AttachmentWell.AttachmentWellFlags.RenderAttachmentSize, prependSemicolon);
						}
						prependSemicolon = true;
					}
					catch (ObjectNotFoundException)
					{
					}
					finally
					{
						if (item != null)
						{
							item.Dispose();
							item = null;
						}
						if (itemAttachment != null)
						{
							itemAttachment.Dispose();
							itemAttachment = null;
						}
					}
				}
			}
		}

		internal static void RenderAttachmentLinkForItem(TextWriter output, AttachmentWellInfo attachmentInfoObject, Item item, string itemId, UserContext userContext, ArrayList previousAttachmentDisplayNames, AttachmentWell.AttachmentWellFlags flags, bool prependSemicolon = false)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (attachmentInfoObject == null)
			{
				throw new ArgumentNullException("attachmentInfoObject");
			}
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			SanitizedHtmlString value;
			bool flag;
			if ((attachmentInfoObject.AttachmentLevel == AttachmentPolicy.Level.ForceSave || attachmentInfoObject.AttachmentLevel == AttachmentPolicy.Level.Allow) && AttachmentWell.AttachmentWellFlags.RenderEmbeddedItem == (flags & AttachmentWell.AttachmentWellFlags.RenderEmbeddedItem))
			{
				string format = string.Empty;
				if (attachmentInfoObject.AttachmentLevel == AttachmentPolicy.Level.Allow)
				{
					format = "<a id=\"{0}\" href=\"#\" onclick=\"{1}\" title=\"{2}\" oncontextmenu=\"return false;\">{3}</a>";
				}
				else
				{
					format = "<a id=\"{0}\" href=\"#\" onclick=\"{1}\" title=\"{2}\">{3}</a>";
				}
				SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
				sanitizingStringBuilder.Append("?ae=Item&t=");
				sanitizingStringBuilder.Append(Utilities.UrlEncode(item.ClassName));
				sanitizingStringBuilder.Append("&atttyp=embdd");
				if (ObjectClass.IsCalendarItemCalendarItemOccurrenceOrRecurrenceException(item.ClassName))
				{
					sanitizingStringBuilder.Append("&a=Read");
				}
				SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder2 = new SanitizingStringBuilder<OwaHtml>();
				if ((flags & AttachmentWell.AttachmentWellFlags.RenderReachedMaxEmbeddedDepth) == (AttachmentWell.AttachmentWellFlags)0)
				{
					sanitizingStringBuilder2.Append("return onClkEmbItem('");
					sanitizingStringBuilder2.Append<SanitizedHtmlString>(sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>());
					sanitizingStringBuilder2.Append("','");
					sanitizingStringBuilder2.Append(Utilities.UrlEncode(attachmentInfoObject.AttachmentId.ToBase64String()));
					sanitizingStringBuilder2.Append("');");
				}
				else
				{
					sanitizingStringBuilder2.Append("return alert(L_ErrRchMxEmbDpth)");
				}
				string embeddedAttachmentDisplayName = AttachmentUtility.GetEmbeddedAttachmentDisplayName(item);
				value = SanitizedHtmlString.Format(format, new object[]
				{
					"lnkAtmt",
					sanitizingStringBuilder2.ToSanitizedString<SanitizedHtmlString>(),
					embeddedAttachmentDisplayName,
					AttachmentUtility.TrimAttachmentDisplayName(embeddedAttachmentDisplayName, previousAttachmentDisplayNames, true)
				});
				flag = false;
			}
			else
			{
				value = Utilities.SanitizeHtmlEncode(AttachmentUtility.TrimAttachmentDisplayName(AttachmentUtility.GetEmbeddedAttachmentDisplayName(item), previousAttachmentDisplayNames, true));
				flag = true;
			}
			if (prependSemicolon)
			{
				output.Write("; ");
			}
			output.Write("<span id=\"spnAtmt\" tabindex=\"-1\" level=\"3\"");
			if (flag)
			{
				output.Write(" class=\"dsbl\"");
			}
			output.Write(">");
			output.Write("<img class=\"sI\" src=\"");
			SmallIconManager.RenderItemIconUrl(output, userContext, item.ClassName);
			output.Write("\" alt=\"\">");
			output.Write(value);
			output.Write("</span>");
		}

		internal static void RenderAttachmentLink(TextWriter output, AttachmentWellType wellType, AttachmentWellInfo attachmentInfoObject, string itemId, UserContext userContext, ArrayList previousAttachmentDisplayNames, AttachmentWell.AttachmentWellFlags flags, bool prependSemicolon = false)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (attachmentInfoObject == null)
			{
				throw new ArgumentNullException("attachmentInfoObject");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (string.IsNullOrEmpty(itemId))
			{
				throw new ArgumentException("itemId may not be null or empty string");
			}
			SanitizedHtmlString value = null;
			string fileExtension = string.Empty;
			SanitizedHtmlString sanitizedHtmlString = null;
			AttachmentUtility.AttachmentLinkFlags attachmentLinkFlag = AttachmentUtility.GetAttachmentLinkFlag(wellType, attachmentInfoObject);
			if ((flags & AttachmentWell.AttachmentWellFlags.RenderReachedMaxEmbeddedDepth) != (AttachmentWell.AttachmentWellFlags)0)
			{
				string format = "<a id=\"{0}\" href=\"#\" onclick=\"{1}\" title=\"{2}\">{3}";
				string text = AttachmentUtility.TrimAttachmentDisplayName(attachmentInfoObject.AttachmentName, previousAttachmentDisplayNames, false);
				value = SanitizedHtmlString.Format(format, new object[]
				{
					"lnkAtmt",
					"return alert(L_ErrRchMxEmbDpth)",
					attachmentInfoObject.AttachmentName,
					text
				});
			}
			else
			{
				string format2 = "<a id=\"{0}\" href=\"attachment.ashx?attach=1&{1}\" target=_blank onclick=\"{2}\" title=\"{3}\">{4}";
				SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
				if (AttachmentWell.AttachmentWellFlags.RenderEmbeddedAttachment != (flags & AttachmentWell.AttachmentWellFlags.RenderEmbeddedAttachment))
				{
					sanitizingStringBuilder.Append("id=");
					sanitizingStringBuilder.Append(Utilities.UrlEncode(itemId));
					sanitizingStringBuilder.Append("&attid0=");
					sanitizingStringBuilder.Append(Utilities.UrlEncode(attachmentInfoObject.AttachmentId.ToBase64String()));
					sanitizingStringBuilder.Append("&attcnt=1");
				}
				else
				{
					sanitizingStringBuilder.Append(AttachmentWell.RenderEmbeddedQueryString(itemId));
					sanitizingStringBuilder.Append(Utilities.UrlEncode(attachmentInfoObject.AttachmentId.ToBase64String()));
				}
				sanitizedHtmlString = sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>();
				if (attachmentInfoObject.AttachmentLevel == AttachmentPolicy.Level.ForceSave)
				{
					string text2 = AttachmentUtility.TrimAttachmentDisplayName(attachmentInfoObject.AttachmentName, previousAttachmentDisplayNames, false);
					value = SanitizedHtmlString.Format(format2, new object[]
					{
						"lnkAtmt",
						sanitizedHtmlString,
						"return onClkAtmt(2)",
						attachmentInfoObject.AttachmentName,
						text2
					});
				}
				if (attachmentInfoObject.AttachmentLevel == AttachmentPolicy.Level.Allow)
				{
					string text3 = AttachmentUtility.TrimAttachmentDisplayName(attachmentInfoObject.AttachmentName, previousAttachmentDisplayNames, false);
					value = SanitizedHtmlString.Format(format2, new object[]
					{
						"lnkAtmt",
						sanitizedHtmlString,
						"return onClkAtmt(3)",
						attachmentInfoObject.AttachmentName,
						text3
					});
				}
			}
			if (prependSemicolon)
			{
				output.Write("; ");
			}
			output.Write("<span id=\"spnAtmt\" tabindex=\"-1\" level=\"");
			output.Write((int)attachmentInfoObject.AttachmentLevel);
			if (AttachmentUtility.AttachmentLinkFlags.AttachmentClickLink != (AttachmentUtility.AttachmentLinkFlags.AttachmentClickLink & attachmentLinkFlag) && (flags & AttachmentWell.AttachmentWellFlags.RenderReachedMaxEmbeddedDepth) == (AttachmentWell.AttachmentWellFlags)0)
			{
				output.Write("\" class=\"dsbl");
			}
			output.Write("\">");
			output.Write("<img class=\"sI\" src=\"");
			if (attachmentInfoObject.FileExtension != null)
			{
				fileExtension = attachmentInfoObject.FileExtension;
			}
			SmallIconManager.RenderFileIconUrl(output, userContext, fileExtension);
			output.Write("\" alt=\"\">");
			if (AttachmentUtility.AttachmentLinkFlags.AttachmentClickLink == (AttachmentUtility.AttachmentLinkFlags.AttachmentClickLink & attachmentLinkFlag) || (flags & AttachmentWell.AttachmentWellFlags.RenderReachedMaxEmbeddedDepth) != (AttachmentWell.AttachmentWellFlags)0)
			{
				output.Write(value);
			}
			else
			{
				Utilities.SanitizeHtmlEncode(AttachmentUtility.TrimAttachmentDisplayName(attachmentInfoObject.AttachmentName, previousAttachmentDisplayNames, false), output);
			}
			if (AttachmentWell.AttachmentWellFlags.RenderAttachmentSize == (flags & AttachmentWell.AttachmentWellFlags.RenderAttachmentSize))
			{
				long size = attachmentInfoObject.Size;
				if (size > 0L)
				{
					output.Write(userContext.DirectionMark);
					output.Write(" ");
					output.Write(SanitizedHtmlString.FromStringId(6409762));
					Utilities.RenderSizeWithUnits(output, size, true);
					output.Write(userContext.DirectionMark);
					output.Write(SanitizedHtmlString.FromStringId(-1023695022));
				}
			}
			output.Write("</a>");
			if (AttachmentUtility.AttachmentLinkFlags.OpenAsWebPageLink == (AttachmentUtility.AttachmentLinkFlags.OpenAsWebPageLink & attachmentLinkFlag))
			{
				output.Write("<span class=\"wvsn\">[<a id=\"wvLnk\" href=\"#\" onclick=\"");
				output.Write("opnWin('WebReadyView.aspx?t=att&");
				output.Write(sanitizedHtmlString);
				output.Write("')\">");
				output.Write(SanitizedHtmlString.FromStringId(1547877601));
				output.Write("</a>]</span>");
			}
			output.Write("</span>");
		}

		public static string RenderEmbeddedUrl(string parentItemId)
		{
			return AttachmentWell.RenderEmbeddedUrl(parentItemId, false);
		}

		public static string RenderEmbeddedQueryString(string parentItemId)
		{
			return AttachmentWell.RenderEmbeddedUrl(parentItemId, true);
		}

		private static string RenderEmbeddedUrl(string parentItemId, bool queryStringOnly)
		{
			string queryStringParameter = Utilities.GetQueryStringParameter(HttpContext.Current.Request, "attcnt");
			int num;
			if (!int.TryParse(queryStringParameter, out num))
			{
				throw new OwaInvalidRequestException("Invalid attachment count querystring parameter");
			}
			StringBuilder stringBuilder = new StringBuilder(200);
			if (!queryStringOnly)
			{
				stringBuilder.Append("attachment.ashx?attach=1&");
			}
			stringBuilder.Append("attcnt=");
			stringBuilder.Append(num + 1);
			stringBuilder.Append("&id=");
			stringBuilder.Append(Utilities.UrlEncode(parentItemId));
			for (int i = 0; i < num; i++)
			{
				string text = "attid" + i.ToString(CultureInfo.InvariantCulture);
				string queryStringParameter2 = Utilities.GetQueryStringParameter(HttpContext.Current.Request, text);
				stringBuilder.Append("&");
				stringBuilder.Append(text);
				stringBuilder.Append("=");
				stringBuilder.Append(Utilities.UrlEncode(queryStringParameter2));
			}
			stringBuilder.Append("&attid" + num.ToString(CultureInfo.InvariantCulture) + "=");
			return stringBuilder.ToString();
		}

		public const string AttachmentSemicolonSeparator = "; ";

		public static readonly string AttachmentInfobarHtmlTag = "divIDA";

		[Flags]
		public enum AttachmentWellFlags
		{
			None = 1,
			RenderEmbeddedItem = 2,
			RenderEmbeddedAttachment = 4,
			RenderInLine = 8,
			RenderReachedMaxEmbeddedDepth = 16,
			RenderAttachmentSize = 32
		}
	}
}
