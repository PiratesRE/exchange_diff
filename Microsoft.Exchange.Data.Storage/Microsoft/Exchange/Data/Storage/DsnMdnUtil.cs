using System;
using System.Collections.Generic;
using Microsoft.Exchange.Core;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class DsnMdnUtil
	{
		public static void GetMapiDsnRecipientStatusCode(string statusString, out MapiDiagnosticCode diagCode, out NdrReasonCode reasonCode, out int statusCode)
		{
			diagCode = MapiDiagnosticCode.NoDiagnostic;
			reasonCode = NdrReasonCode.TransferFailed;
			EnhancedStatusCode enhancedStatusCode;
			string[] array;
			int num;
			int num2;
			int num3;
			if (!EnhancedStatusCode.TryParse(statusString, out enhancedStatusCode) || (array = statusString.Split(new char[]
			{
				'.'
			})).Length != 3 || !int.TryParse(array[0], out num) || !int.TryParse(array[1], out num2) || !int.TryParse(array[2], out num3))
			{
				StorageGlobals.ContextTraceError<string>(ExTraceGlobals.CcInboundMimeTracer, "DsnMdnUtil::GetMapiDsnRecipientStatusCode: incorrect status string, {0}", statusString);
				throw new ConversionFailedException(ConversionFailureReason.CorruptContent, null);
			}
			switch (num2)
			{
			case 1:
				switch (num3)
				{
				case 1:
					diagCode = MapiDiagnosticCode.MailRecipientUnknown;
					reasonCode = NdrReasonCode.TransferImpossible;
					goto IL_288;
				case 2:
					diagCode = MapiDiagnosticCode.BadDestinationSystemAddress;
					goto IL_288;
				case 3:
					diagCode = MapiDiagnosticCode.MailAddressIncorrect;
					goto IL_288;
				case 4:
					diagCode = MapiDiagnosticCode.OrNameAmbiguous;
					goto IL_288;
				case 6:
					diagCode = MapiDiagnosticCode.MailRecipientMoved;
					goto IL_288;
				}
				diagCode = MapiDiagnosticCode.OrNameUnrecognized;
				break;
			case 2:
				switch (num3)
				{
				case 1:
					diagCode = MapiDiagnosticCode.MailRefused;
					break;
				case 2:
					diagCode = MapiDiagnosticCode.LengthConstraintViolatd;
					break;
				case 3:
					diagCode = MapiDiagnosticCode.LengthConstraintViolatd;
					break;
				case 4:
					diagCode = MapiDiagnosticCode.ExpansionFailed;
					break;
				default:
					diagCode = MapiDiagnosticCode.MailRefused;
					break;
				}
				break;
			case 3:
				switch (num3)
				{
				case 1:
					diagCode = MapiDiagnosticCode.MailRefused;
					break;
				case 2:
					break;
				case 3:
					diagCode = MapiDiagnosticCode.CriticalFuncUnsupported;
					break;
				case 4:
					diagCode = MapiDiagnosticCode.LengthConstraintViolatd;
					break;
				case 5:
					diagCode = MapiDiagnosticCode.LoopDetected;
					break;
				default:
					diagCode = MapiDiagnosticCode.MailRefused;
					break;
				}
				break;
			case 4:
				switch (num3)
				{
				case 0:
				case 4:
					goto IL_288;
				case 3:
					reasonCode = NdrReasonCode.DirectoryOperatnFailed;
					goto IL_288;
				case 6:
				case 8:
					diagCode = MapiDiagnosticCode.LoopDetected;
					goto IL_288;
				case 7:
					diagCode = MapiDiagnosticCode.MaximumTimeExpired;
					goto IL_288;
				}
				diagCode = MapiDiagnosticCode.MtsCongested;
				break;
			case 5:
				switch (num3)
				{
				case 3:
					diagCode = MapiDiagnosticCode.TooManyRecipients;
					goto IL_288;
				case 4:
					diagCode = MapiDiagnosticCode.ParametersInvalid;
					goto IL_288;
				}
				diagCode = MapiDiagnosticCode.NoBilateralAgreement;
				break;
			case 6:
				switch (num3)
				{
				case 1:
					diagCode = MapiDiagnosticCode.ContentTypeUnsupported;
					break;
				case 2:
					diagCode = MapiDiagnosticCode.ProhibitedToConvert;
					break;
				case 3:
					diagCode = MapiDiagnosticCode.ImpracticalToConvert;
					break;
				case 4:
					diagCode = MapiDiagnosticCode.MultipleInfoLosses;
					break;
				case 5:
					reasonCode = NdrReasonCode.ConversionNotPerformed;
					break;
				default:
					diagCode = MapiDiagnosticCode.ContentTypeUnsupported;
					break;
				}
				break;
			case 7:
				switch (num3)
				{
				case 1:
					diagCode = MapiDiagnosticCode.SubmissionProhibited;
					break;
				case 2:
					diagCode = MapiDiagnosticCode.ExpansionProhibited;
					break;
				case 3:
					diagCode = MapiDiagnosticCode.ReassignmentProhibited;
					break;
				default:
					diagCode = MapiDiagnosticCode.SecureMessagingError;
					break;
				}
				break;
			}
			IL_288:
			if (num2 < 0 || num2 > 9)
			{
				num2 = 0;
			}
			if (num3 < 0 || num3 > 9)
			{
				num3 = 0;
			}
			statusCode = num * 100 + num2 * 10 + num3;
		}

		public static int GetMimeDsnRecipientStatusCode(MapiDiagnosticCode diagCode, NdrReasonCode reasonCode)
		{
			switch (diagCode)
			{
			case MapiDiagnosticCode.NoDiagnostic:
				switch (reasonCode)
				{
				case NdrReasonCode.TransferImpossible:
					return 510;
				case NdrReasonCode.ConversionNotPerformed:
				case NdrReasonCode.PhysicalRenditnNotDone:
					return 565;
				case NdrReasonCode.PhysicalDelivNotDone:
					return 520;
				case NdrReasonCode.RestrictedDelivery:
					return 530;
				case NdrReasonCode.DirectoryOperatnFailed:
					return 443;
				}
				return 540;
			case MapiDiagnosticCode.OrNameUnrecognized:
			case MapiDiagnosticCode.RecipientUnavailable:
			case MapiDiagnosticCode.MailOfficeIncorOrInvd:
			case MapiDiagnosticCode.MailRecipientDeceased:
			case MapiDiagnosticCode.MailUnclaimed:
			case MapiDiagnosticCode.MailRecipientTravelling:
			case MapiDiagnosticCode.MailRecipientDeparted:
				return 510;
			case MapiDiagnosticCode.OrNameAmbiguous:
				return 514;
			case MapiDiagnosticCode.MtsCongested:
				return 445;
			case MapiDiagnosticCode.LoopDetected:
			case MapiDiagnosticCode.RedirectionLoopDetected:
				return 446;
			case MapiDiagnosticCode.MaximumTimeExpired:
				return 447;
			case MapiDiagnosticCode.EitsUnsupported:
			case MapiDiagnosticCode.MailOrganizationExpired:
			case MapiDiagnosticCode.MailRefused:
				return 530;
			case MapiDiagnosticCode.ContentTooLong:
			case MapiDiagnosticCode.ConversionUnsubscribed:
			case MapiDiagnosticCode.ContentSyntaxInError:
			case MapiDiagnosticCode.NumberConstraintViolatd:
			case MapiDiagnosticCode.LineTooLong:
			case MapiDiagnosticCode.PageTooLong:
				return 560;
			case MapiDiagnosticCode.ImpracticalToConvert:
				return 563;
			case MapiDiagnosticCode.ProhibitedToConvert:
			case MapiDiagnosticCode.ConversionLossProhib:
				return 562;
			case MapiDiagnosticCode.ParametersInvalid:
				return 554;
			case MapiDiagnosticCode.LengthConstraintViolatd:
				return 534;
			case MapiDiagnosticCode.ContentTypeUnsupported:
			case MapiDiagnosticCode.RenditionUnsupported:
				return 561;
			case MapiDiagnosticCode.TooManyRecipients:
				return 553;
			case MapiDiagnosticCode.NoBilateralAgreement:
				return 555;
			case MapiDiagnosticCode.CriticalFuncUnsupported:
				return 533;
			case MapiDiagnosticCode.PictorialSymbolLost:
			case MapiDiagnosticCode.PunctuationSymbolLost:
			case MapiDiagnosticCode.AlphabeticCharacterLost:
			case MapiDiagnosticCode.MultipleInfoLosses:
				return 464;
			case MapiDiagnosticCode.ReassignmentProhibited:
				return 573;
			case MapiDiagnosticCode.ExpansionProhibited:
				return 572;
			case MapiDiagnosticCode.SubmissionProhibited:
				return 571;
			case MapiDiagnosticCode.ExpansionFailed:
				return 524;
			case MapiDiagnosticCode.MailAddressIncorrect:
			case MapiDiagnosticCode.MailAddressIncomplete:
				return 513;
			case MapiDiagnosticCode.MailRecipientUnknown:
				return 511;
			case MapiDiagnosticCode.MailRecipientMoved:
			case MapiDiagnosticCode.MailNewAddressUnknown:
				return 516;
			case MapiDiagnosticCode.SecureMessagingError:
				return 570;
			case MapiDiagnosticCode.BadDestinationSystemAddress:
				return 512;
			}
			return 500;
		}

		public static EnhancedStatusCode GetMimeDsnRecipientStatusCode(Recipient recipient)
		{
			int mimeDsnRecipientStatusCode = DsnMdnUtil.GetMimeDsnRecipientStatusCode(recipient.GetValueOrDefault<MapiDiagnosticCode>(InternalSchema.NdrDiagnosticCode, MapiDiagnosticCode.NoDiagnostic), recipient.GetValueOrDefault<NdrReasonCode>(InternalSchema.NdrReasonCode));
			EnhancedStatusCode result;
			if ((result = DsnMdnUtil.TryParseMimeStatusCode(recipient.GetValueAsNullable<int>(InternalSchema.NdrStatusCode))) == null)
			{
				result = (DsnMdnUtil.TryParseMimeStatusCode(new int?(mimeDsnRecipientStatusCode)) ?? DsnMdnUtil.TryParseMimeStatusCode(new int?(500)));
			}
			return result;
		}

		public static string MimeStatusCodeToString(int mimeStatusCode)
		{
			return string.Format("{0}.{1}.{2}", mimeStatusCode / 100, mimeStatusCode / 10 % 10, mimeStatusCode % 10);
		}

		public static bool TryGetSubjectPrefix(string itemClass, out LocalizedString subjectPrefix)
		{
			foreach (KeyValuePair<string, SystemMessages.IDs> keyValuePair in DsnMdnUtil.reportClassOrSuffixToSubjectPrefix)
			{
				if (ObjectClass.IsReport(itemClass, keyValuePair.Key))
				{
					subjectPrefix = SystemMessages.GetLocalizedString(keyValuePair.Value);
					return true;
				}
			}
			subjectPrefix = LocalizedString.Empty;
			return false;
		}

		private static EnhancedStatusCode TryParseMimeStatusCode(int? mimeStatusCode)
		{
			EnhancedStatusCode result;
			if (mimeStatusCode != null && EnhancedStatusCode.TryParse(DsnMdnUtil.MimeStatusCodeToString(mimeStatusCode.Value), out result))
			{
				return result;
			}
			return null;
		}

		private const int DefaultRecipientNdrStatusCode = 500;

		private static KeyValuePair<string, SystemMessages.IDs>[] reportClassOrSuffixToSubjectPrefix = new KeyValuePair<string, SystemMessages.IDs>[]
		{
			new KeyValuePair<string, SystemMessages.IDs>("NDR", SystemMessages.IDs.FailedSubject),
			new KeyValuePair<string, SystemMessages.IDs>("Expanded.DR", (SystemMessages.IDs)3301930267U),
			new KeyValuePair<string, SystemMessages.IDs>("Relayed.DR", SystemMessages.IDs.RelayedSubject),
			new KeyValuePair<string, SystemMessages.IDs>("Delayed.DR", (SystemMessages.IDs)3017333248U),
			new KeyValuePair<string, SystemMessages.IDs>("DR", (SystemMessages.IDs)2296356702U),
			new KeyValuePair<string, SystemMessages.IDs>("IPNRN", SystemMessages.IDs.ReadSubject),
			new KeyValuePair<string, SystemMessages.IDs>("IPNNRN", (SystemMessages.IDs)3244754815U)
		};
	}
}
