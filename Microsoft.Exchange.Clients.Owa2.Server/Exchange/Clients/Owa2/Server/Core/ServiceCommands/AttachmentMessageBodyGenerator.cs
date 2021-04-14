using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.ServiceCommands
{
	internal static class AttachmentMessageBodyGenerator
	{
		public static string GenerateBodyForInvitation(string fileName, string invitationLink)
		{
			string arg = string.Format("<a href=\"{0}\" target=\"_blank\" style=\"text-decoration: none;color:#0072C6\">{1}</a>", invitationLink, Strings.SignIn);
			string arg2 = string.Format(Strings.GuestSharingInvitationBody, fileName, arg);
			string arg3 = string.Format("<div style=\"background-color:#C2F299;border:1px solid #99CC62\"><div style=\"padding: 12px 5px 8px 12px;\">{0}</div></div>", arg2);
			return string.Format("<div style=\"width:570px;font-size: 17px; font-family: 'Segoe UI', 'Segoe UI WPC', Tahoma, 'Microsoft Sans Serif', Verdana, sans-serif; font-weight:lighter;color: #333333\">{0}</div>", arg3);
		}

		public static string GenerateBodyForAttachmentNotFound(List<AttachmentFile> attachmentFiles)
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			string headingText;
			if (attachmentFiles.Count == 1)
			{
				headingText = Strings.OneAttachmentNotFoundInbox;
				list.Add(Strings.OneAttachmentNotFoundSmallText);
				list2.Add(Strings.OneAttachmentNotFoundLargeText);
			}
			else
			{
				headingText = Strings.AttachmentsNotFoundInbox;
				list.Add(Strings.AttachmentsNotFoundSmallText);
				list2.Add(Strings.AttachmentsNotFoundLargeText);
			}
			return AttachmentMessageBodyGenerator.GenerateNDRBody(headingText, list, attachmentFiles, false, null, list2);
		}

		public static string GenerateBodyForCatchAll(List<AttachmentFile> attachmentFiles)
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			string headingText;
			if (attachmentFiles.Count == 1)
			{
				headingText = Strings.OneAttachmentCatchAllInbox;
				list.Add(Strings.OneAttachmentCatchAllSmallText);
				string arg = string.Format("<a href=\"{0}\" target=\"_blank\" style=\"text-decoration: none;color:#0072C6\">{1}</a>", attachmentFiles[0].FileURL, attachmentFiles[0].FileName);
				list2.Add(string.Format(Strings.OneAttachmentCatchAllLargeText, arg));
			}
			else
			{
				headingText = Strings.AttachmentsCatchAllInbox;
				list.Add(Strings.AttachmentsCatchAllSmallText);
				list2.Add(Strings.AttachmentsCatchAllLargeText);
			}
			return AttachmentMessageBodyGenerator.GenerateNDRBody(headingText, list, attachmentFiles, true, null, list2);
		}

		public static string GenerateBodyForSetWrongPermission(List<AttachmentFile> attachmentFiles, List<string> recipients)
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			string headingText;
			if (attachmentFiles.Count == 1)
			{
				headingText = Strings.OneAttachmentSetWrongPermissionInbox;
				list.Add(Strings.OneAttachmentSetWrongPermissionSmallText);
				list2.Add(Strings.OneAttachmentSetWrongPermissionLargeText1);
				list2.Add(Strings.OneAttachmentSetWrongPermissionLargeText2);
				list2.Add(Strings.OneAttachmentSetWrongPermissionLargeText3);
			}
			else
			{
				headingText = Strings.AttachmentsSetWrongPermissionInbox;
				list.Add(Strings.AttachmentsSetWrongPermissionSmallText);
				list2.Add(Strings.OneAttachmentSetWrongPermissionLargeText1);
				list2.Add(Strings.AttachmentsSetWrongPermissionLargeText2);
				list2.Add(Strings.AttachmentsSetWrongPermissionLargeText3);
			}
			return AttachmentMessageBodyGenerator.GenerateNDRBody(headingText, list, attachmentFiles, true, recipients, list2);
		}

		public static string GenerateBodyForSentToTooLargeDL(List<AttachmentFile> attachmentFiles, string distributionListName)
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			string headingText;
			if (attachmentFiles.Count == 1)
			{
				headingText = Strings.OneAttachmentSentToTooLargeDLInbox;
				list.Add(string.Format(Strings.OneAttachmentSentToTooLargeDLSmallText, 100, distributionListName));
				list.Add(Strings.OneAttachmentSentToTooLargeDLSmallText1);
				string arg = string.Format("<a href=\"{0}\" target=\"_blank\" style=\"text-decoration: none;color:#0072C6\">{1}</a>", attachmentFiles[0].FileURL, attachmentFiles[0].FileName);
				list2.Add(string.Format(Strings.OneAttachmentSentToTooLargeDLLargeText, arg));
			}
			else
			{
				headingText = Strings.AttachmentsSentToTooLargeDLInbox;
				list.Add(string.Format(Strings.OneAttachmentSentToTooLargeDLSmallText, 100, distributionListName));
				list.Add(Strings.AttachmentsSentToTooLargeDLSmallText2);
				list2.Add(Strings.AttachmentsSentToTooLargeDLLargeText);
			}
			return AttachmentMessageBodyGenerator.GenerateNDRBody(headingText, list, attachmentFiles, true, null, list2);
		}

		private static string GenerateNDRBody(string headingText, List<string> smallText, List<AttachmentFile> attachmentFiles, bool showHyperLink, List<string> recipientList, List<string> largeTexts)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Format("<div style=\"background-color:#F8D4D4;border:1px solid #F5A6A7\"><div style=\"padding: 10px 5px 13px 12px;\">{0}</div></div>", headingText));
			stringBuilder.Append(string.Format("<div style=\"padding-top: 20px;line-height:20px\">{0}</div>", smallText[0]));
			for (int i = 1; i < smallText.Count; i++)
			{
				stringBuilder.Append(string.Format("<div style=\"padding-top: 10px;line-height:20px\">{0}</div>", smallText[i]));
			}
			if (recipientList != null && attachmentFiles.Count == 1)
			{
				StringBuilder stringBuilder2 = new StringBuilder();
				foreach (string arg in recipientList)
				{
					stringBuilder2.Append(string.Format("<div>{0}</div>", arg));
				}
				stringBuilder.Append(string.Format("<div style=\"padding-top: 10px;\">{0}</div>", stringBuilder2.ToString()));
			}
			else if (attachmentFiles.Count > 1)
			{
				StringBuilder stringBuilder3 = new StringBuilder();
				if (showHyperLink)
				{
					using (List<AttachmentFile>.Enumerator enumerator2 = attachmentFiles.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							AttachmentFile attachmentFile = enumerator2.Current;
							string arg2 = string.Format("<a href=\"{0}\" target=\"_blank\" style=\"text-decoration: none;color:#0072C6\">{1}</a>", attachmentFile.FileURL, attachmentFile.FileName);
							stringBuilder3.Append(string.Format("<div>{0}</div>", arg2));
						}
						goto IL_17B;
					}
				}
				foreach (AttachmentFile attachmentFile2 in attachmentFiles)
				{
					stringBuilder3.Append(string.Format("<div>{0}</div>", attachmentFile2.FileName));
				}
				IL_17B:
				stringBuilder.Append(string.Format("<div style=\"padding-top: 10px;\">{0}</div>", stringBuilder3.ToString()));
			}
			if (largeTexts.Count == 1)
			{
				stringBuilder.Append(string.Format("<div style=\"padding-top: 15px; font-size: 21px;line-height:24px\">{0}</div>", largeTexts[0]));
			}
			else
			{
				StringBuilder stringBuilder4 = new StringBuilder();
				stringBuilder4.Append(largeTexts[0]);
				StringBuilder stringBuilder5 = new StringBuilder();
				for (int j = 1; j < largeTexts.Count; j++)
				{
					stringBuilder5.Append(string.Format("<li>{0}</li>", largeTexts[j]));
				}
				stringBuilder4.Append(string.Format("<ul style=\"position:relative;top:-20px\">{0}</ul>", stringBuilder5.ToString()));
				stringBuilder.Append(string.Format("<div style=\"padding-top: 15px; font-size: 21px;line-height:24px\">{0}</div>", stringBuilder4.ToString()));
			}
			return string.Format("<div style=\"width:570px;font-size: 17px; font-family: 'Segoe UI', 'Segoe UI WPC', Tahoma, 'Microsoft Sans Serif', Verdana, sans-serif; font-weight:lighter;color: #333333\">{0}</div>", stringBuilder.ToString());
		}

		private const string MainBody = "<div style=\"width:570px;font-size: 17px; font-family: 'Segoe UI', 'Segoe UI WPC', Tahoma, 'Microsoft Sans Serif', Verdana, sans-serif; font-weight:lighter;color: #333333\">{0}</div>";

		private const string NDRInboxElement = "<div style=\"background-color:#F8D4D4;border:1px solid #F5A6A7\"><div style=\"padding: 10px 5px 13px 12px;\">{0}</div></div>";

		private const string InvitationInboxElement = "<div style=\"background-color:#C2F299;border:1px solid #99CC62\"><div style=\"padding: 12px 5px 8px 12px;\">{0}</div></div>";

		private const string FirstSmallTextElement = "<div style=\"padding-top: 20px;line-height:20px\">{0}</div>";

		private const string OtherSmallTextElement = "<div style=\"padding-top: 10px;line-height:20px\">{0}</div>";

		private const string ListElement = "<div style=\"padding-top: 10px;\">{0}</div>";

		private const string ListItem = "<div>{0}</div>";

		private const string LargeTextElement = "<div style=\"padding-top: 15px; font-size: 21px;line-height:24px\">{0}</div>";

		private const string HyperLink = "<a href=\"{0}\" target=\"_blank\" style=\"text-decoration: none;color:#0072C6\">{1}</a>";

		private const string UnorderedList = "<ul style=\"position:relative;top:-20px\">{0}</ul>";

		private const string UnorderedListItem = "<li>{0}</li>";
	}
}
