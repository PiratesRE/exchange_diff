using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Security.RightsManagement.Protectors;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class AttachmentWell
	{
		private AttachmentWell()
		{
		}

		public static void RenderAttachmentWell(TextWriter output, AttachmentWellType wellType, ArrayList attachmentList, UserContext userContext)
		{
			AttachmentWell.RenderAttachmentWell(output, wellType, attachmentList, userContext, false);
		}

		public static void RenderAttachmentWell(TextWriter output, AttachmentWellType wellType, ArrayList attachmentList, UserContext userContext, bool isConversationAndIrmCopyRestricted)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (wellType == AttachmentWellType.ReadOnly && AttachmentUtility.IsLevelOneAndBlockOnly(attachmentList))
			{
				return;
			}
			AttachmentWell.RenderAttachmentWell(output, wellType, attachmentList, userContext, false, isConversationAndIrmCopyRestricted);
		}

		public static void RenderEmptyAttachmentWell(TextWriter output, AttachmentWellType wellType)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			AttachmentWell.RenderAttachmentWell(output, wellType, null, null, true, false);
		}

		private static void RenderAttachmentWell(TextWriter output, AttachmentWellType wellType, ArrayList attachmentList, UserContext userContext, bool renderEmptyWell, bool isConversationAndIrmCopyRestricted)
		{
			output.Write("<div id=\"divAtch\" ");
			if (wellType == AttachmentWellType.ReadWrite)
			{
				output.Write("_isRW=\"1\" class=\"awRW\" ");
			}
			else
			{
				output.Write("class=\"awRO\" ");
			}
			if (Utilities.GetEmbeddedDepth(HttpContext.Current.Request) < AttachmentPolicy.MaxEmbeddedDepth)
			{
				output.Write("_isEventAttach=\"1\" ");
			}
			output.Write(">");
			if (!renderEmptyWell)
			{
				AttachmentWell.RenderAttachments(output, wellType, attachmentList, userContext, isConversationAndIrmCopyRestricted);
			}
			output.Write("</div>");
		}

		internal static ArrayList GetAttachmentInformation(Item item, IList<AttachmentLink> attachmentLinks, bool isLoggedOnFromPublicComputer)
		{
			return AttachmentWell.GetAttachmentInformation(item, attachmentLinks, isLoggedOnFromPublicComputer, false);
		}

		internal static ArrayList GetAttachmentInformation(Item item, IList<AttachmentLink> attachmentLinks, bool isLoggedOnFromPublicComputer, bool isEmbeddedItem)
		{
			return AttachmentWell.GetAttachmentInformation(item, attachmentLinks, isLoggedOnFromPublicComputer, isEmbeddedItem, false);
		}

		internal static ArrayList GetAttachmentInformation(Item item, IList<AttachmentLink> attachmentLinks, bool isLoggedOnFromPublicComputer, bool isEmbeddedItem, bool forceEnableItemLink)
		{
			if (item.AttachmentCollection == null)
			{
				return null;
			}
			return AttachmentUtility.GetAttachmentList(item, attachmentLinks, isLoggedOnFromPublicComputer, isEmbeddedItem, true, forceEnableItemLink);
		}

		internal static ArrayList GetAttachmentInformation(OwaStoreObjectId owaConversationId, ItemPart itemPart, bool isLoggedOnFromPublicComputer, bool isEmbeddedItem, bool forceEnableItemLink)
		{
			if (itemPart.Attachments == null || itemPart.Attachments.Count == 0)
			{
				return null;
			}
			return AttachmentUtility.GetAttachmentList(owaConversationId, itemPart, isLoggedOnFromPublicComputer, isEmbeddedItem, true, forceEnableItemLink);
		}

		public static void RenderInfobar(TextWriter output, ArrayList attachmentList, UserContext userContext)
		{
			AttachmentWell.RenderInfobar(output, attachmentList, null, userContext);
		}

		public static void RenderInfobar(TextWriter output, ArrayList attachmentList, SanitizedHtmlString errorInAttachments, UserContext userContext)
		{
			if (attachmentList == null)
			{
				return;
			}
			int count = attachmentList.Count;
			if (count <= 0 && errorInAttachments == null)
			{
				return;
			}
			Infobar infobar = new Infobar();
			if (errorInAttachments != null)
			{
				infobar.AddMessage(errorInAttachments, InfobarMessageType.Warning, AttachmentWell.AttachmentInfobarErrorHtmlTag);
			}
			bool flag = AttachmentUtility.IsLevelOneAndBlock(attachmentList);
			if (flag)
			{
				infobar.AddMessage(SanitizedHtmlString.FromStringId(-2118248931), InfobarMessageType.Informational, AttachmentWell.AttachmentInfobarHtmlTag);
			}
			infobar.Render(output);
		}

		public static void RenderAttachments(TextWriter output, AttachmentWellType wellType, ArrayList attachmentList, UserContext userContext)
		{
			AttachmentWell.RenderAttachments(output, wellType, attachmentList, userContext, false);
		}

		public static void RenderAttachments(TextWriter output, AttachmentWellType wellType, ArrayList attachmentList, UserContext userContext, bool isConversationAndIrmCopyRestricted)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (attachmentList == null || attachmentList.Count <= 0)
			{
				return;
			}
			bool flag = true;
			Utilities.GetEmbeddedDepth(HttpContext.Current.Request);
			ArrayList previousAttachmentDisplayNames = new ArrayList();
			Item item = null;
			try
			{
				foreach (object obj in attachmentList)
				{
					AttachmentWellInfo attachmentWellInfo = (AttachmentWellInfo)obj;
					AttachmentUtility.AttachmentLinkFlags attachmentLinkFlag = AttachmentUtility.GetAttachmentLinkFlag(wellType, attachmentWellInfo);
					if (AttachmentUtility.AttachmentLinkFlags.Skip != (AttachmentUtility.AttachmentLinkFlags.Skip & attachmentLinkFlag))
					{
						ItemAttachment itemAttachment = null;
						Item item2 = null;
						try
						{
							if (attachmentWellInfo.AttachmentType == AttachmentType.EmbeddedMessage)
							{
								if (attachmentWellInfo.MessageId != null)
								{
									if (item != null)
									{
										StoreObjectId storeObjectId = StoreId.GetStoreObjectId(item.Id);
										if (!storeObjectId.Equals(attachmentWellInfo.MessageId.StoreObjectId))
										{
											item.Dispose();
											item = null;
										}
									}
									if (item == null)
									{
										item = Utilities.GetItem<Item>(userContext, attachmentWellInfo.MessageId, false, null);
									}
									AttachmentCollection attachmentCollection = item.AttachmentCollection;
									if (userContext.IsIrmEnabled && Utilities.IrmDecryptIfRestricted(item, userContext))
									{
										attachmentCollection = ((RightsManagedMessageItem)item).ProtectedAttachmentCollection;
									}
									itemAttachment = (ItemAttachment)attachmentCollection.Open(attachmentWellInfo.AttachmentId);
								}
								else
								{
									itemAttachment = (ItemAttachment)attachmentWellInfo.OpenAttachment();
								}
								item2 = itemAttachment.GetItemAsReadOnly(new PropertyDefinition[]
								{
									TaskSchema.TaskType
								});
							}
							if (!flag)
							{
								output.Write("; ");
							}
							else
							{
								flag = false;
							}
							output.Write("<span id=spnAtmt tabindex=\"-1\" ");
							if (wellType == AttachmentWellType.ReadOnly && AttachmentUtility.AttachmentLinkFlags.AttachmentClickLink != (AttachmentUtility.AttachmentLinkFlags.AttachmentClickLink & attachmentLinkFlag))
							{
								output.Write("class=\"dsbl\"");
							}
							if (attachmentWellInfo.MessageId != null)
							{
								output.Write(" _sId=\"");
								Utilities.SanitizeHtmlEncode(attachmentWellInfo.MessageId.ToString(), output);
								output.Write("\"");
							}
							string text;
							if (item2 != null)
							{
								text = AttachmentUtility.GetEmbeddedAttachmentDisplayName(item2);
							}
							else
							{
								text = attachmentWellInfo.AttachmentName;
							}
							output.Write(" _attid=\"");
							output.Write(Utilities.SanitizeHtmlEncode(attachmentWellInfo.AttachmentId.ToBase64String()));
							output.Write("\" _level=");
							output.Write((int)attachmentWellInfo.AttachmentLevel);
							output.Write(" _attname=\"");
							output.Write(Utilities.SanitizeHtmlEncode(text));
							output.Write("\" _attsize=\"");
							output.Write(attachmentWellInfo.Size);
							output.Write("\" _fIsItem=\"");
							output.Write(item2 != null);
							output.Write("\"");
							if (attachmentWellInfo.AttachmentType != AttachmentType.EmbeddedMessage)
							{
								output.Write(" _protectable=\"");
								bool flag2 = false;
								try
								{
									flag2 = ProtectorsManager.Instance.IsProtectorRegistered(attachmentWellInfo.FileExtension);
								}
								catch (AttachmentProtectionException)
								{
								}
								output.Write(flag2 ? "1" : "0");
								output.Write("\"");
							}
							if (item2 != null)
							{
								Task task = item2 as Task;
								if ((task != null && !TaskUtilities.IsAssignedTask(task)) || item2 is Contact || item2 is DistributionList)
								{
									output.Write(" _qs=\"ae=PreFormAction&a=OpenEmbedded&t=");
								}
								else
								{
									output.Write(" _qs=\"ae=Item&t=");
								}
								Utilities.SanitizeHtmlEncode(Utilities.UrlEncode(item2.ClassName), output);
								if (task != null && TaskUtilities.IsAssignedTask(task))
								{
									output.Write("&s=Assigned");
								}
								output.Write("\"");
							}
							output.Write(">");
							bool flag3 = AttachmentUtility.AttachmentLinkFlags.AttachmentClickLink == (AttachmentUtility.AttachmentLinkFlags.AttachmentClickLink & attachmentLinkFlag);
							if (flag3)
							{
								string text2 = null;
								if (item2 != null || attachmentWellInfo.AttachmentLevel == AttachmentPolicy.Level.Allow)
								{
									text2 = "3";
								}
								else if (attachmentWellInfo.AttachmentLevel == AttachmentPolicy.Level.ForceSave)
								{
									text2 = "2";
								}
								if (text2 != null)
								{
									output.Write("<a class=\"lnkClickAtmt\" id=\"lnkAtmt\" _fAllwCM=\"1\" href=\"#\" target=_blank ");
									Utilities.RenderScriptHandler(output, "onclick", string.Concat(new string[]
									{
										"atLnk(event, ",
										text2,
										", ",
										isConversationAndIrmCopyRestricted ? "1" : "0",
										");"
									}));
									output.Write(" title=\"");
									Utilities.SanitizeHtmlEncode(text, output);
									if (text2 == "3" & item2 != null)
									{
										output.Write("\" oncontextmenu=\"return false;");
									}
									output.Write("\">");
								}
							}
							if (item2 != null)
							{
								SmallIconManager.RenderItemIcon(output, userContext, item2.ClassName, false, "sI", new string[0]);
							}
							else
							{
								string fileExtension = string.Empty;
								if (attachmentWellInfo.FileExtension != null)
								{
									fileExtension = attachmentWellInfo.FileExtension;
								}
								SmallIconManager.RenderFileIcon(output, userContext, fileExtension, "sI", new string[0]);
							}
							if (item2 != null)
							{
								Utilities.SanitizeHtmlEncode(AttachmentUtility.TrimAttachmentDisplayName(text, previousAttachmentDisplayNames, true), output);
							}
							else
							{
								Utilities.SanitizeHtmlEncode(AttachmentUtility.TrimAttachmentDisplayName(text, previousAttachmentDisplayNames, false), output);
								long size = attachmentWellInfo.Size;
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
							if (flag3)
							{
								output.Write("</a>");
							}
							if (item2 == null && AttachmentUtility.AttachmentLinkFlags.OpenAsWebPageLink == (AttachmentUtility.AttachmentLinkFlags.OpenAsWebPageLink & attachmentLinkFlag))
							{
								AttachmentWell.RenderWebReadyLink(output, userContext);
							}
							if (wellType == AttachmentWellType.ReadWrite)
							{
								output.Write("<span class=\"delLnk\">");
								userContext.RenderThemeImage(output, ThemeFileId.DeleteSmall, null, new object[]
								{
									"id=delImg"
								});
								output.Write("</span>");
							}
							output.Write("</span>");
						}
						catch (ObjectNotFoundException)
						{
						}
						catch (StorageTransientException)
						{
						}
						finally
						{
							if (item2 != null)
							{
								item2.Dispose();
								item2 = null;
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
			finally
			{
				if (item != null)
				{
					item.Dispose();
					item = null;
				}
			}
		}

		private static void RenderWebReadyLink(TextWriter output, UserContext userContext)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			output.Write("<span class=\"wvsn\">");
			output.Write(userContext.DirectionMark);
			output.Write(SanitizedHtmlString.FromStringId(1698127316));
			output.Write("<a id=\"wvLnk\" href=\"#\" ");
			Utilities.RenderScriptHandler(output, "onclick", "opnWRDV(_this);");
			output.Write(">");
			output.Write(SanitizedHtmlString.FromStringId(1547877601));
			output.Write("</a>");
			output.Write(userContext.DirectionMark);
			output.Write(SanitizedHtmlString.FromStringId(-1056669576));
			output.Write("</span>");
		}

		public static string GetWebReadyLink(UserContext userContext)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			string result;
			using (StringWriter stringWriter = new StringWriter())
			{
				AttachmentWell.RenderWebReadyLink(stringWriter, userContext);
				result = stringWriter.ToString();
			}
			return result;
		}

		public static readonly string AttachmentInfobarHtmlTag = "divIDA";

		public static readonly string AttachmentInfobarErrorHtmlTag = "divDAErr";
	}
}
