using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Audio;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.UM.UMCommon.FaultInjection;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class DRMUtils
	{
		internal static string GetProtectedUMFileNameToUse(string attachmentFileName)
		{
			if (string.IsNullOrEmpty(attachmentFileName))
			{
				return attachmentFileName;
			}
			attachmentFileName = attachmentFileName.Trim();
			string extension;
			string str;
			if (!Attachment.TryFindFileExtension(attachmentFileName, out extension, out str))
			{
				return attachmentFileName;
			}
			string str2;
			if (AudioFile.TryGetDRMFileExtension(extension, out str2))
			{
				return str + str2;
			}
			return attachmentFileName;
		}

		internal static string GetProtectedUMVoiceMessageAttachmentOrder(string currentOrder)
		{
			if (currentOrder == null)
			{
				return null;
			}
			string result = currentOrder;
			string[] array = currentOrder.Split(new char[]
			{
				';'
			}, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length > 0)
			{
				StringBuilder stringBuilder = new StringBuilder(DRMUtils.GetProtectedUMFileNameToUse(array[0]));
				for (int i = 1; i < array.Length; i++)
				{
					stringBuilder.Append(";").Append(DRMUtils.GetProtectedUMFileNameToUse(array[i]));
				}
				result = stringBuilder.ToString();
			}
			return result;
		}

		internal static MessageItem OpenRestrictedContent(MessageItem restrictedContentItem, OrganizationId orgId)
		{
			MessageItem result = null;
			try
			{
				RestrictionInfo restrictionInfo = null;
				FaultInjectionUtils.FaultInjectException(4108725565U);
				bool flag;
				UseLicenseAndUsageRights useLicenseAndUsageRights;
				result = ItemConversion.OpenRestrictedContent(restrictedContentItem, orgId, true, out flag, out useLicenseAndUsageRights, out restrictionInfo);
			}
			catch (CorruptDataException ex)
			{
				throw new OpenRestrictedContentException(ex.Message, ex);
			}
			catch (RightsManagementPermanentException ex2)
			{
				throw new OpenRestrictedContentException(ex2.Message, ex2);
			}
			catch (RightsManagementTransientException ex3)
			{
				throw new OpenRestrictedContentException(ex3.Message, ex3);
			}
			catch (InvalidOperationException ex4)
			{
				throw new OpenRestrictedContentException(ex4.Message, ex4);
			}
			catch (LocalizedException ex5)
			{
				throw new OpenRestrictedContentException(ex5.Message, ex5);
			}
			return result;
		}

		internal static Stream OpenProtectedAttachment(Attachment sourceAttachment, OrganizationId orgId)
		{
			Stream stream = null;
			try
			{
				FaultInjectionUtils.FaultInjectException(2900766013U);
				stream = StreamAttachment.OpenRestrictedContent(sourceAttachment as StreamAttachment, orgId);
			}
			catch (RightsManagementPermanentException ex)
			{
				throw new OpenRestrictedContentException(ex.Message, ex);
			}
			catch (RightsManagementTransientException ex2)
			{
				throw new OpenRestrictedContentException(ex2.Message, ex2);
			}
			if (stream == null)
			{
				throw new OpenRestrictedContentException("Attachment Stream is null");
			}
			return stream;
		}
	}
}
