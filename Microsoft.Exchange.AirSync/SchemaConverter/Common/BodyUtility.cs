using System;
using System.Collections;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.AirSync.SchemaConverter.XSO;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal class BodyUtility
	{
		public static bool GetHigherBodyType(ref BodyType type)
		{
			if (type == BodyType.Rtf)
			{
				return false;
			}
			type++;
			return true;
		}

		public static bool GetLowerBodyType(ref BodyType type)
		{
			if (BodyUtility.fidelity[(int)type] == 0)
			{
				return false;
			}
			type = (BodyType)BodyUtility.fidelity[(int)type];
			return true;
		}

		public static bool IsAskingForMIMEData(IMIMERelatedProperty property, IDictionary options)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			if (options == null)
			{
				throw new ArgumentNullException("options");
			}
			if (options.Contains("MIMESupport"))
			{
				if ((MIMESupportValue)options["MIMESupport"] == MIMESupportValue.SendMimeDataForAllMessages)
				{
					return true;
				}
				if ((MIMESupportValue)options["MIMESupport"] == MIMESupportValue.SendMimeDataForSmimeMessagesOnly && property.IsOnSMIMEMessage)
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsClearSigned(Item item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			return ObjectClass.IsOfClass(item.ClassName, "IPM.Note.Secure.Sign") || ObjectClass.IsSmimeClearSigned(item.ClassName);
		}

		public static Stream ConvertItemToMime(StoreObject xsoItem)
		{
			Item item = xsoItem as Item;
			if (item == null)
			{
				throw new UnexpectedTypeException("Item", xsoItem);
			}
			OutboundConversionOptions outboundConversionOptions = AirSyncUtility.GetOutboundConversionOptions();
			PolicyData policyData = ADNotificationManager.GetPolicyData(Command.CurrentCommand.User);
			if (policyData != null && !policyData.AttachmentsEnabled)
			{
				outboundConversionOptions.FilterAttachmentHandler = ((Item item1, Attachment attachment) => false);
			}
			item.Load(StoreObjectSchema.ContentConversionProperties);
			AirSyncStream airSyncStream = new AirSyncStream();
			try
			{
				if (BodyConversionUtilities.IsMessageRestrictedAndDecoded(item))
				{
					ItemConversion.ConvertItemToMime(((RightsManagedMessageItem)item).DecodedItem, airSyncStream, outboundConversionOptions);
				}
				else
				{
					ItemConversion.ConvertItemToMime(item, airSyncStream, outboundConversionOptions);
				}
			}
			catch (ConversionFailedException innerException)
			{
				throw new ConversionException(string.Format(CultureInfo.InvariantCulture, "MIME conversion failed for Item {0}", new object[]
				{
					item
				}), innerException);
			}
			return airSyncStream;
		}

		private static readonly int[] fidelity = new int[]
		{
			0,
			0,
			1,
			2
		};
	}
}
