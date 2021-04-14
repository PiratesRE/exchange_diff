using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class AttachmentLinksBuilder
	{
		private static string LinkIconCdnLocation
		{
			get
			{
				string text = ConfigurationManager.AppSettings["ContentDeliveryNetworkEndpoint"];
				string arg = string.IsNullOrEmpty(text) ? "https://r4.res.outlook.com/" : text;
				return string.Format("{0}{1}", arg, "owa/prem/images");
			}
		}

		internal static string InsertReferenceAttachmentLinks(string body, Item item)
		{
			body = AttachmentLinksBuilder.attachmentLinksRegex.Replace(body, string.Empty);
			if (item.IAttachmentCollection.Count <= 0)
			{
				return body;
			}
			string value = AttachmentLinksBuilder.BuildLinksHtml(item.IAttachmentCollection);
			if (string.IsNullOrEmpty(value))
			{
				return body;
			}
			Match match = AttachmentLinksBuilder.bodyTagRegex.Match(body);
			body = (match.Success ? body.Insert(match.Index + match.Length, value) : body.Insert(0, value));
			return body;
		}

		private static string BuildLinksHtml(IAttachmentCollection attachments)
		{
			string format = CoreResources.ReferenceLinkSharedFrom;
			string text = string.Empty;
			foreach (AttachmentHandle attachmentHandle in attachments)
			{
				if (attachmentHandle.AttachMethod == 7)
				{
					using (IReferenceAttachment referenceAttachment = (IReferenceAttachment)attachments.OpenIAttachment(attachmentHandle))
					{
						string text2 = HttpUtility.HtmlEncode(Path.GetFileName(referenceAttachment.FileName));
						string dataProviderDisplayName = AttachmentLinksBuilder.GetDataProviderDisplayName(referenceAttachment.ProviderType);
						string attachLongPathName = referenceAttachment.AttachLongPathName;
						string iconNameForFileExtension = AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.GetIconNameForFileExtension(Path.GetExtension(referenceAttachment.FileName));
						object obj = referenceAttachment.TryGetProperty(AttachmentSchema.AttachContentId);
						string arg = string.Empty;
						if (!(obj is PropertyError))
						{
							arg = (string)obj;
						}
						text += string.Format("<a href=\"{0}\" target=\"_blank\"><table align=left style=\"margin-right: 15px; margin-bottom:10px; border-color: rgb(204, 204, 204); border-width: 1px; border-style: solid; height:50px; background-color: rgb(255, 255, 255); border-spacing: 0px\"><tr valign=\"top\"><td style=\"padding: 0px;\"><div style=\"background-color: rgb(255, 255, 255); height: 50px; width: 50px; max-height: 50px;\"><a href=\"{0}\" target=\"_blank\">{1}</a></div></td><td><div id=\"OwaReferenceAttachmentFileName\" style=\"margin: 0px 4px; font-size: 18px; font-family: 'Segoe UI', 'Segoe WP', 'Segoe UI WPC', Tahoma, Arial, sans-serif; color: rgb(9, 151, 245);\"><a href=\"{0}\" target=\"_blank\" style=\"text-decoration: none; margin: 0px; font-size: 18px; font-family: 'Segoe UI', 'Segoe WP', 'Segoe UI WPC', Tahoma, Arial, sans-serif; color: rgb(9, 151, 245);\">{2}</a></div><div style=\"font-size: 10px; font-family: 'Segoe UI', 'Segoe WP', 'Segoe UI WPC', Tahoma, Arial, sans-serif; color: rgb(102, 102, 102); margin: 0px 4px;\">{3}</div></td><td style=\"display:none;visibility:hidden;\" width=0 height=0>{4}</td></tr></table></a>", new object[]
						{
							attachLongPathName,
							string.Format("<img width=\"50\" style=\"border:0px;\" src=\"{0}/{1}\">", AttachmentLinksBuilder.LinkIconCdnLocation, iconNameForFileExtension),
							text2,
							string.Format(format, dataProviderDisplayName),
							string.Format("<img width=\"0\" height=\"0\" style=\"visibility:hidden;border:0px;display:none\" src=\"dummy.jpg\" originalSrc=\"cid:{0}\" title=\"{1}\">", arg, referenceAttachment.FileName)
						});
					}
				}
			}
			string result = string.Empty;
			if (!string.IsNullOrEmpty(text))
			{
				result = string.Format("<div id=\"OwaReferenceAttachments\" contenteditable=\"false\"><table><tr valign=\"top\"><td>{0}</td></tr></table></div><div id=\"OwaReferenceAttachmentsEnd\" style=\"display:none;visibility:hidden;\"></div>", text);
			}
			return result;
		}

		private static string GetDataProviderDisplayName(string providerType)
		{
			if (providerType.Equals("OneDrivePro", StringComparison.OrdinalIgnoreCase))
			{
				return CoreResources.OneDriveProAttachmentDataProviderName;
			}
			return null;
		}

		private static readonly Regex attachmentLinksRegex = new Regex("<div id=\\\"OwaReferenceAttachments.+?OwaReferenceAttachmentsEnd.+?/div>", RegexOptions.Compiled | RegexOptions.Singleline);

		private static readonly Regex bodyTagRegex = new Regex("<body[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private static class AttachmentsWellKnownFileTypes
		{
			static AttachmentsWellKnownFileTypes()
			{
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".accdb"] = "c_access_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".accde"] = "c_access_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".acs"] = "c_access_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".xlsx"] = "c_excel_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".xlsm"] = "c_excel_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".xlsb"] = "c_excel_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".xls"] = "c_excel_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".xltx"] = "c_excel_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".xltm"] = "c_excel_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".xlt"] = "c_excel_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".xlam"] = "c_excel_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".xla"] = "c_excel_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".ods"] = "c_excel_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".pptx"] = "c_powerpoint_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".pptm"] = "c_powerpoint_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".ppt"] = "c_powerpoint_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".potx"] = "c_powerpoint_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".potm"] = "c_powerpoint_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".pot"] = "c_powerpoint_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".ppsx"] = "c_powerpoint_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".ppsm"] = "c_powerpoint_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".pps"] = "c_powerpoint_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".ppam"] = "c_powerpoint_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".ppa"] = "c_powerpoint_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".odp"] = "c_powerpoint_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".docx"] = "c_word_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".docm"] = "c_word_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".doc"] = "c_word_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".dot"] = "c_word_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".dotx"] = "c_word_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".dotm"] = "c_word_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".wps"] = "c_word_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".odt"] = "c_word_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".one"] = "c_onenote_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".onepkg"] = "c_onenote_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".pdf"] = "c_pdf_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".wav"] = "c_wv_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".aiff"] = "c_wv_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".mp3"] = "c_wv_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".mp4"] = "c_wv_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".aac"] = "c_wv_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".wma"] = "c_wv_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".m4a"] = "c_wv_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".ogg"] = "c_wv_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".flac"] = "c_wv_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".ape"] = "c_wv_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".mpg"] = "c_mpg_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".avi"] = "c_mpg_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".mov"] = "c_mpg_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".wmv"] = "c_mpg_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".3g2"] = "c_mpg_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".3gp"] = "c_mpg_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".asf"] = "c_mpg_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".asx"] = "c_mpg_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".flv"] = "c_mpg_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".mp4"] = "c_mpg_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".srt"] = "c_mpg_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".swf"] = "c_mpg_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".vob"] = "c_mpg_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".zip"] = "c_zip_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".rpmsg"] = "c_irm_msg_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".txt"] = "c_txt_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".rtf"] = "c_txt_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".jpg"] = "c_jpg_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".jpeg"] = "c_jpg_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".jpe"] = "c_jpg_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".jfif"] = "c_jpg_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".gif"] = "c_jpg_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".png"] = "c_jpg_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".bmp"] = "c_jpg_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".ico"] = "c_jpg_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".vdx"] = "c_visio_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".vsd"] = "c_visio_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".vsdx"] = "c_visio_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".vsl"] = "c_visio_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".vss"] = "c_visio_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".vst"] = "c_visio_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".mpp"] = "c_project_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".pub"] = "c_publisher_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".lync"] = "c_lync_cloud";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".msg"] = "c_msg_cloud.png";
				AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[".eml"] = "c_msg_cloud.png";
			}

			public static string GetIconNameForFileExtension(string extension)
			{
				if (AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap.ContainsKey(extension))
				{
					return AttachmentLinksBuilder.AttachmentsWellKnownFileTypes.fileExtensionToIconNameMap[extension];
				}
				return "c_generic_cloud.png";
			}

			private static readonly Dictionary<string, string> fileExtensionToIconNameMap = new Dictionary<string, string>();
		}
	}
}
