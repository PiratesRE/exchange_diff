using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Xml;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.RightsManagement;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ItemConversion
	{
		private static void InternalConvertAnyMimeToItem(Item itemOut, EmailMessage messageIn, InboundConversionOptions options, MimePromotionFlags promotionFlags, bool isStreamToStream)
		{
			ConversionLimitsTracker limitsTracker = new ConversionLimitsTracker(options.Limits);
			itemOut.CharsetDetector.DetectionOptions = options.DetectionOptions;
			new InboundMimeConverter(itemOut, messageIn, options, limitsTracker, isStreamToStream ? ConverterFlags.IsStreamToStreamConversion : ConverterFlags.None)
			{
				IsResolvingParticipants = true
			}.ConvertToItem(promotionFlags);
		}

		public static void ConvertAnyMimeToItem(Item itemOut, EmailMessage messageIn, InboundConversionOptions options)
		{
			StorageGlobals.ContextTraceInformation(ExTraceGlobals.CcInboundGenericTracer, "ItemConversion::ConvertAnyMimeToItem.1");
			Util.ThrowOnNullArgument(itemOut, "itemOut");
			Util.ThrowOnNullArgument(messageIn, "messageIn");
			Util.ThrowOnNullArgument(options, "options");
			if (!options.IgnoreImceaDomain)
			{
				Util.ThrowOnNullOrEmptyArgument(options.ImceaEncapsulationDomain, "options.ImceaEncapsulationDomain");
			}
			try
			{
				ItemConversion.InternalConvertAnyMimeToItem(itemOut, messageIn, options, MimePromotionFlags.Default, false);
			}
			catch (StoragePermanentException exc)
			{
				ItemConversion.SaveFailedMimeDocument(messageIn.RootPart, options, exc);
				throw;
			}
		}

		public static void ConvertAnyMimeToItem(Item itemOut, MimeDocument documentIn, InboundConversionOptions options)
		{
			ItemConversion.ConvertAnyMimeToItem(itemOut, documentIn, options, MimePromotionFlags.Default);
		}

		private static void ConvertAnyMimeToItem(Item itemOut, MimeDocument documentIn, InboundConversionOptions options, MimePromotionFlags flags)
		{
			StorageGlobals.ContextTraceInformation(ExTraceGlobals.CcInboundGenericTracer, "ItemConversion::ConvertAnyMimeToItem.2");
			Util.ThrowOnNullArgument(itemOut, "itemOut");
			Util.ThrowOnNullArgument(documentIn, "documentIn");
			Util.ThrowOnNullArgument(options, "options");
			if (!options.IgnoreImceaDomain)
			{
				Util.ThrowOnNullOrEmptyArgument(options.ImceaEncapsulationDomain, "options.ImceaEncapsulationDomain");
			}
			try
			{
				ConvertUtils.CallCts(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter::ConvertToItem", ServerStrings.ConversionCorruptContent, delegate
				{
					EmailMessage messageIn = EmailMessage.Create(documentIn);
					ItemConversion.InternalConvertAnyMimeToItem(itemOut, messageIn, options, flags, false);
				});
			}
			catch (StoragePermanentException exc)
			{
				ItemConversion.SaveFailedMimeDocument(documentIn.RootPart, options, exc);
				throw;
			}
		}

		public static void ConvertICalToItem(Item item, InboundConversionOptions options, string iCalContents)
		{
			ItemConversion.<>c__DisplayClass5 CS$<>8__locals1 = new ItemConversion.<>c__DisplayClass5();
			CS$<>8__locals1.item = item;
			CS$<>8__locals1.options = options;
			CS$<>8__locals1.addressCache = new InboundAddressCache(CS$<>8__locals1.options, new ConversionLimitsTracker(CS$<>8__locals1.options.Limits), MimeMessageLevel.TopLevelMessage);
			CS$<>8__locals1.errorMessage = LocalizedString.Empty;
			CS$<>8__locals1.success = false;
			using (MemoryStream iCalStream = new MemoryStream())
			{
				using (StreamWriter streamWriter = new StreamWriter(iCalStream))
				{
					streamWriter.Write(iCalContents);
					streamWriter.Flush();
					iCalStream.Seek(0L, SeekOrigin.Begin);
					ConvertUtils.CallCts(ExTraceGlobals.ICalTracer, "ItemConversion::ICalToItem", ServerStrings.ConversionCorruptContent, delegate
					{
						CS$<>8__locals1.success = CalendarDocument.ICalToItem(iCalStream, CS$<>8__locals1.item, CS$<>8__locals1.addressCache, false, CS$<>8__locals1.options.DefaultCharset.Name, out CS$<>8__locals1.errorMessage);
					});
				}
			}
			if (!CS$<>8__locals1.success)
			{
				throw new ConversionFailedException(ConversionFailureReason.CorruptContent, CS$<>8__locals1.errorMessage, null);
			}
			CS$<>8__locals1.addressCache.Resolve();
			CS$<>8__locals1.addressCache.CopyDataToItem(CS$<>8__locals1.item.CoreItem);
		}

		public static void ConvertAnyMimeToItem(Item itemOut, Stream mimeIn, InboundConversionOptions options)
		{
			StorageGlobals.ContextTraceInformation(ExTraceGlobals.CcInboundGenericTracer, "ItemConversion::ConvertAnyMimeToItem.3");
			Util.ThrowOnNullArgument(itemOut, "itemOut");
			Util.ThrowOnNullArgument(mimeIn, "mimeIn");
			Util.ThrowOnNullArgument(options, "options");
			if (!options.IgnoreImceaDomain)
			{
				Util.ThrowOnNullOrEmptyArgument(options.ImceaEncapsulationDomain, "options.ImceaEncapsulationDomain");
			}
			MimeDocument document = null;
			try
			{
				ConvertUtils.CallCts(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter::ConvertToItem", ServerStrings.ConversionCorruptContent, delegate
				{
					document = ItemConversion.LoadInboundMimeDocument(mimeIn, options);
					EmailMessage messageIn = EmailMessage.Create(document);
					ItemConversion.InternalConvertAnyMimeToItem(itemOut, messageIn, options, MimePromotionFlags.Default, false);
				});
			}
			catch (StoragePermanentException exc)
			{
				ItemConversion.SaveFailedMimeDocument((document != null) ? document.RootPart : null, options, exc);
				throw;
			}
			finally
			{
				if (document != null)
				{
					document.Dispose();
				}
			}
		}

		public static void ConvertMsgStorageToItem(Stream msgStorageIn, Item itemOut, InboundConversionOptions conversionOptions)
		{
			StorageGlobals.ContextTraceInformation(ExTraceGlobals.CcOutboundMimeTracer, "ItemConversion::ConvertMsgStorageToItem");
			Util.ThrowOnNullArgument(msgStorageIn, "msgStorageIn");
			Util.ThrowOnNullArgument(itemOut, "itemOut");
			Util.ThrowOnNullArgument(conversionOptions, "conversionOptions");
			ConvertUtils.CallCts(ExTraceGlobals.CcOutboundMimeTracer, "ItemConversion::ConvertMsgStorageToItem", ServerStrings.ConversionCorruptContent, delegate
			{
				using (InboundMessageWriter inboundMessageWriter = new InboundMessageWriter(itemOut.CoreItem, conversionOptions, MimeMessageLevel.TopLevelMessage))
				{
					inboundMessageWriter.ForceParticipantResolution = true;
					InboundMsgConverter inboundMsgConverter = new InboundMsgConverter(inboundMessageWriter);
					inboundMsgConverter.ConvertToItem(msgStorageIn);
					inboundMessageWriter.Commit();
				}
			});
		}

		private static void CheckOutboundArguments(Item itemIn, Stream mimeOut, OutboundConversionOptions options)
		{
			Util.ThrowOnNullArgument(itemIn, "itemIn");
			Util.ThrowOnNullArgument(mimeOut, "mimeOut");
			Util.ThrowOnNullArgument(options, "options");
			Util.ThrowOnNullOrEmptyArgument(options.ImceaEncapsulationDomain, "options.ImceaEncapsulationDomain");
			if (!itemIn.HasAllPropertiesLoaded)
			{
				throw new InvalidOperationException(ServerStrings.ConversionMustLoadAllPropeties);
			}
		}

		public static bool IsItemClassConvertibleToMime(string itemClass)
		{
			if (itemClass == null)
			{
				throw new ArgumentNullException("itemClass");
			}
			StorageGlobals.ContextTraceInformation<string>(ExTraceGlobals.CcGenericTracer, "ItemConversion::IsItemClassConvertibleToMime: {0}.", itemClass);
			switch (ObjectClass.GetObjectType(itemClass))
			{
			case StoreObjectType.Unknown:
				return ObjectClass.IsGenericMessage(itemClass);
			case StoreObjectType.Folder:
				return false;
			case StoreObjectType.CalendarFolder:
				return false;
			case StoreObjectType.ContactsFolder:
				return false;
			case StoreObjectType.TasksFolder:
				return false;
			case StoreObjectType.NotesFolder:
				return false;
			case StoreObjectType.JournalFolder:
				return false;
			case StoreObjectType.SearchFolder:
				return false;
			case StoreObjectType.OutlookSearchFolder:
				return false;
			case StoreObjectType.Message:
				return true;
			case StoreObjectType.MeetingMessage:
				return true;
			case StoreObjectType.MeetingRequest:
				return true;
			case StoreObjectType.MeetingResponse:
				return true;
			case StoreObjectType.MeetingCancellation:
				return true;
			case StoreObjectType.ConflictMessage:
				return true;
			case StoreObjectType.CalendarItem:
				return false;
			case StoreObjectType.CalendarItemOccurrence:
				return false;
			case StoreObjectType.Contact:
				return false;
			case StoreObjectType.DistributionList:
				return false;
			case StoreObjectType.Task:
				return false;
			case StoreObjectType.TaskRequest:
				return true;
			case StoreObjectType.Note:
				return false;
			case StoreObjectType.Post:
				return true;
			case StoreObjectType.Report:
				return true;
			case StoreObjectType.MeetingForwardNotification:
				return true;
			case StoreObjectType.ConversationActionItem:
				return false;
			case StoreObjectType.SharingMessage:
				return true;
			case StoreObjectType.MeetingInquiryMessage:
				return false;
			case StoreObjectType.OofMessage:
				return true;
			case StoreObjectType.ExternalOofMessage:
				return true;
			case StoreObjectType.ReminderMessage:
				return true;
			case StoreObjectType.MeetingRequestSeries:
				return true;
			case StoreObjectType.MeetingResponseSeries:
				return true;
			case StoreObjectType.MeetingCancellationSeries:
				return true;
			case StoreObjectType.MeetingForwardNotificationSeries:
				return true;
			}
			return false;
		}

		private static ConversionResult InternalConvertItemToSummaryTnef(Item itemIn, Stream mimeOut, OutboundConversionOptions options)
		{
			ItemConversion.CheckOutboundArguments(itemIn, mimeOut, options);
			ConversionLimitsTracker limitsTracker = new ConversionLimitsTracker(options.Limits);
			ConversionResult result = new ConversionResult();
			ConvertUtils.CallCts(ExTraceGlobals.CcOutboundTnefTracer, "ItemConversion::ConvertItemToSummaryTnef", ServerStrings.ConversionCorruptContent, delegate
			{
				if (ItemConversion.IsLimitCheckSuppressedForThisClass(itemIn.ClassName))
				{
					limitsTracker.SuppressLimitChecks();
				}
				using (Stream stream = new StreamWrapper(mimeOut, false))
				{
					using (ItemToMimeConverter itemToMimeConverter = new ItemToMimeConverter(itemIn, options, ConverterFlags.None))
					{
						using (MimeStreamWriter mimeStreamWriter = new MimeStreamWriter(stream, itemToMimeConverter.GetItemMimeEncodingOptions(options), MimeStreamWriter.Flags.ForceMime))
						{
							result = itemToMimeConverter.ConvertItemToSummaryTnef(mimeStreamWriter, limitsTracker, false);
							mimeStreamWriter.Flush();
						}
					}
				}
			});
			return result;
		}

		private static void InternalConvertItemToLegacyTnef(Item itemIn, Stream mimeOut, OutboundConversionOptions options, bool isStreamToStream)
		{
			ItemConversion.CheckOutboundArguments(itemIn, mimeOut, options);
			ConversionLimitsTracker limitsTracker = new ConversionLimitsTracker(options.Limits);
			ConvertUtils.CallCts(ExTraceGlobals.CcOutboundTnefTracer, "ItemConversion::ConvertItemToLegacyTnef", ServerStrings.ConversionCorruptContent, delegate
			{
				using (Stream stream = new StreamWrapper(mimeOut, false))
				{
					using (ItemToMimeConverter itemToMimeConverter = new ItemToMimeConverter(itemIn, options, isStreamToStream ? ConverterFlags.IsStreamToStreamConversion : ConverterFlags.None))
					{
						using (MimeStreamWriter mimeStreamWriter = new MimeStreamWriter(stream, itemToMimeConverter.GetItemMimeEncodingOptions(options), MimeStreamWriter.Flags.ForceMime))
						{
							itemToMimeConverter.ConvertItemToLegacyTnef(mimeStreamWriter, limitsTracker);
							mimeStreamWriter.Flush();
						}
					}
				}
			});
		}

		private static ConversionResult InternalConvertItemToMime(Item itemIn, Stream mimeOut, OutboundConversionOptions options, bool isStreamToStream)
		{
			ItemConversion.CheckOutboundArguments(itemIn, mimeOut, options);
			ConversionLimitsTracker limitsTracker = new ConversionLimitsTracker(options.Limits);
			if (ItemConversion.IsLimitCheckSuppressedForThisClass(itemIn.ClassName))
			{
				limitsTracker.SuppressLimitChecks();
			}
			MimeStreamWriter.Flags flags = MimeStreamWriter.Flags.ForceMime;
			if (itemIn.GetValueOrDefault<bool>(InternalSchema.IsSingleBodyICal))
			{
				flags = MimeStreamWriter.Flags.None;
			}
			ConverterFlags converterFlags = ConverterFlags.None;
			if (isStreamToStream)
			{
				converterFlags |= ConverterFlags.IsStreamToStreamConversion;
			}
			Stream skeletonStream = null;
			ConversionResult result = new ConversionResult();
			try
			{
				if (options.GenerateMimeSkeleton)
				{
					PropertyError propertyError = itemIn.TryGetProperty(InternalSchema.MimeSkeleton) as PropertyError;
					if (propertyError != null && propertyError.PropertyErrorCode == PropertyErrorCode.NotFound)
					{
						skeletonStream = itemIn.OpenPropertyStream(InternalSchema.MimeSkeleton, PropertyOpenMode.Create);
						result.ItemWasModified = true;
						converterFlags |= ConverterFlags.GenerateSkeleton;
					}
				}
				using (Stream wrappedMimeOut = new StreamWrapper(mimeOut, false))
				{
					ConvertUtils.CallCts(ExTraceGlobals.CcOutboundMimeTracer, "ItemConversion::ConvertItemToMime", ServerStrings.ConversionCorruptContent, delegate
					{
						using (ItemToMimeConverter itemToMimeConverter = new ItemToMimeConverter(itemIn, options, converterFlags))
						{
							using (MimeStreamWriter mimeStreamWriter = new MimeStreamWriter(wrappedMimeOut, skeletonStream, itemToMimeConverter.GetItemMimeEncodingOptions(options), flags))
							{
								result.AddSubResult(itemToMimeConverter.ConvertItemToMime(mimeStreamWriter, limitsTracker));
								mimeStreamWriter.Flush();
							}
						}
					});
				}
			}
			finally
			{
				if (skeletonStream != null)
				{
					skeletonStream.Dispose();
				}
			}
			return result;
		}

		public static ConversionResult ConvertItemToSummaryTnef(Item itemIn, Stream mimeOut, OutboundConversionOptions options)
		{
			StorageGlobals.ContextTraceInformation(ExTraceGlobals.CcOutboundTnefTracer, "ItemConversion::ConvertItemToSummaryTnef");
			ConversionResult result;
			try
			{
				result = ItemConversion.InternalConvertItemToSummaryTnef(itemIn, mimeOut, options);
			}
			catch (StoragePermanentException exc)
			{
				ItemConversion.SaveFailedItem(itemIn, options, exc);
				throw;
			}
			return result;
		}

		public static void ConvertItemToLegacyTnef(Item itemIn, Stream mimeOut, OutboundConversionOptions options)
		{
			StorageGlobals.ContextTraceInformation(ExTraceGlobals.CcOutboundTnefTracer, "ItemConversion::ConvertItemToLegacyTnef");
			try
			{
				ItemConversion.InternalConvertItemToLegacyTnef(itemIn, mimeOut, options, false);
			}
			catch (StoragePermanentException exc)
			{
				ItemConversion.SaveFailedItem(itemIn, options, exc);
				throw;
			}
		}

		public static ConversionResult ConvertItemToMime(Item itemIn, Stream mimeOut, OutboundConversionOptions options)
		{
			StorageGlobals.ContextTraceInformation(ExTraceGlobals.CcOutboundMimeTracer, "ItemConversion::ConvertItemToMime");
			ConversionResult result;
			try
			{
				result = ItemConversion.InternalConvertItemToMime(itemIn, mimeOut, options, false);
			}
			catch (StoragePermanentException exc)
			{
				ItemConversion.SaveFailedItem(itemIn, options, exc);
				throw;
			}
			return result;
		}

		private static void InternalConvertAnyMimeToMime(EmailMessage messageIn, Stream mimeOut, OutboundConversionOptions outboundOptions)
		{
			using (StorageGlobals.SetTraceContext("ConvertAnyMimeToMime"))
			{
				InboundConversionOptions options = InboundConversionOptions.FromOutboundOptions(outboundOptions);
				using (MessageItem messageItem = MessageItem.CreateInMemory(StoreObjectSchema.ContentConversionProperties))
				{
					ItemConversion.InternalConvertAnyMimeToItem(messageItem, messageIn, options, MimePromotionFlags.Default, true);
					messageItem.Save(SaveMode.NoConflictResolution);
					messageItem.Load(StoreObjectSchema.ContentConversionProperties);
					ItemConversion.InternalConvertItemToMime(messageItem, mimeOut, outboundOptions, true);
				}
			}
		}

		private static void InternalConvertAnyMimeToLegacyTnef(EmailMessage messageIn, Stream mimeOut, OutboundConversionOptions outboundOptions)
		{
			using (StorageGlobals.SetTraceContext("ConvertAnyMimeToLegacyTnef"))
			{
				InboundConversionOptions options = InboundConversionOptions.FromOutboundOptions(outboundOptions);
				using (MessageItem messageItem = MessageItem.CreateInMemory(StoreObjectSchema.ContentConversionProperties))
				{
					ItemConversion.InternalConvertAnyMimeToItem(messageItem, messageIn, options, MimePromotionFlags.Default, true);
					messageItem.Save(SaveMode.NoConflictResolution);
					messageItem.Load(StoreObjectSchema.ContentConversionProperties);
					ItemConversion.InternalConvertItemToLegacyTnef(messageItem, mimeOut, outboundOptions, true);
				}
			}
		}

		public static void ConvertAnyMimeToMime(Stream mimeIn, Stream mimeOut, OutboundConversionOptions outboundOptions)
		{
			StorageGlobals.ContextTraceInformation(ExTraceGlobals.CcGenericTracer, "ItemConversion::ConvertAnyMimeToMime.3");
			Util.ThrowOnNullArgument(mimeIn, "mimeIn");
			Util.ThrowOnNullArgument(mimeOut, "mimeOut");
			Util.ThrowOnNullArgument(outboundOptions, "outboundOptions");
			Util.ThrowOnNullOrEmptyArgument(outboundOptions.ImceaEncapsulationDomain, "outboundOptions.ImceaEncapsulationDomain");
			MimeDocument document = null;
			try
			{
				ConvertUtils.CallCts(ExTraceGlobals.CcOutboundMimeTracer, "ItemConversion::ConvertAnyMimeToMime", ServerStrings.ConversionCorruptContent, delegate
				{
					InboundConversionOptions inboundOptions = InboundConversionOptions.FromOutboundOptions(outboundOptions);
					document = ItemConversion.LoadInboundMimeDocument(mimeIn, inboundOptions);
					EmailMessage messageIn = EmailMessage.Create(document);
					ItemConversion.InternalConvertAnyMimeToMime(messageIn, mimeOut, outboundOptions);
				});
			}
			catch (StoragePermanentException exc)
			{
				ItemConversion.SaveFailedMimeDocument((document != null) ? document.RootPart : null, outboundOptions, exc);
				throw;
			}
			finally
			{
				if (document != null)
				{
					document.Dispose();
				}
			}
		}

		public static void ConvertAnyMimeToMime(MimeDocument documentIn, Stream mimeOut, OutboundConversionOptions outboundOptions)
		{
			StorageGlobals.ContextTraceInformation(ExTraceGlobals.CcGenericTracer, "ItemConversion::ConvertAnyMimeToMime.2");
			Util.ThrowOnNullArgument(documentIn, "documentIn");
			Util.ThrowOnNullArgument(mimeOut, "mimeOut");
			Util.ThrowOnNullArgument(outboundOptions, "outboundOptions");
			Util.ThrowOnNullOrEmptyArgument(outboundOptions.ImceaEncapsulationDomain, "outboundOptions.ImceaEncapsulationDomain");
			try
			{
				ConvertUtils.CallCts(ExTraceGlobals.CcOutboundMimeTracer, "ItemConversion::ConvertAnyMimeToMime", ServerStrings.ConversionCorruptContent, delegate
				{
					EmailMessage messageIn = EmailMessage.Create(documentIn);
					ItemConversion.InternalConvertAnyMimeToMime(messageIn, mimeOut, outboundOptions);
				});
			}
			catch (StoragePermanentException exc)
			{
				ItemConversion.SaveFailedMimeDocument(documentIn.RootPart, outboundOptions, exc);
				throw;
			}
		}

		public static void ConvertAnyMimeToMime(EmailMessage messageIn, Stream mimeOut, OutboundConversionOptions outboundOptions)
		{
			StorageGlobals.ContextTraceInformation(ExTraceGlobals.CcGenericTracer, "ItemConversion::ConvertAnyMimeToMime.1");
			Util.ThrowOnNullArgument(messageIn, "messageIn");
			Util.ThrowOnNullArgument(mimeOut, "mimeOut");
			Util.ThrowOnNullArgument(outboundOptions, "outboundOptions");
			Util.ThrowOnNullOrEmptyArgument(outboundOptions.ImceaEncapsulationDomain, "outboundOptions.ImceaEncapsulationDomain");
			try
			{
				ItemConversion.InternalConvertAnyMimeToMime(messageIn, mimeOut, outboundOptions);
			}
			catch (StoragePermanentException exc)
			{
				ItemConversion.SaveFailedMimeDocument(messageIn.RootPart, outboundOptions, exc);
				throw;
			}
		}

		public static void ConvertAnyMimeToLegacyTnef(Stream mimeIn, Stream mimeOut, OutboundConversionOptions outboundOptions)
		{
			StorageGlobals.ContextTraceInformation(ExTraceGlobals.CcGenericTracer, "ItemConversion::ConvertAnyMimeToLegacyTnef.3");
			Util.ThrowOnNullArgument(mimeIn, "mimeIn");
			Util.ThrowOnNullArgument(mimeOut, "mimeOut");
			Util.ThrowOnNullArgument(outboundOptions, "outboundOptions");
			Util.ThrowOnNullOrEmptyArgument(outboundOptions.ImceaEncapsulationDomain, "outboundOptions.ImceaEncapsulationDomain");
			MimeDocument document = null;
			try
			{
				ConvertUtils.CallCts(ExTraceGlobals.CcOutboundMimeTracer, "ItemConversion::ConvertAnyMimeToLegacyTnef", ServerStrings.ConversionCorruptContent, delegate
				{
					InboundConversionOptions inboundOptions = InboundConversionOptions.FromOutboundOptions(outboundOptions);
					document = ItemConversion.LoadInboundMimeDocument(mimeIn, inboundOptions);
					EmailMessage messageIn = EmailMessage.Create(document);
					ItemConversion.InternalConvertAnyMimeToLegacyTnef(messageIn, mimeOut, outboundOptions);
				});
			}
			catch (StoragePermanentException exc)
			{
				ItemConversion.SaveFailedMimeDocument((document != null) ? document.RootPart : null, outboundOptions, exc);
				throw;
			}
			finally
			{
				if (document != null)
				{
					document.Dispose();
				}
			}
		}

		public static void ConvertAnyMimeToLegacyTnef(MimeDocument documentIn, Stream mimeOut, OutboundConversionOptions outboundOptions)
		{
			StorageGlobals.ContextTraceInformation(ExTraceGlobals.CcGenericTracer, "ItemConversion::ConvertAnyMimeToLegacyTnef.2");
			Util.ThrowOnNullArgument(documentIn, "documentIn");
			Util.ThrowOnNullArgument(mimeOut, "mimeOut");
			Util.ThrowOnNullArgument(outboundOptions, "outboundOptions");
			Util.ThrowOnNullOrEmptyArgument(outboundOptions.ImceaEncapsulationDomain, "outboundOptions.ImceaEncapsulationDomain");
			try
			{
				ConvertUtils.CallCts(ExTraceGlobals.CcOutboundMimeTracer, "ItemConversion::ConvertAnyMimeToLegacyTnef", ServerStrings.ConversionCorruptContent, delegate
				{
					EmailMessage messageIn = EmailMessage.Create(documentIn);
					ItemConversion.InternalConvertAnyMimeToLegacyTnef(messageIn, mimeOut, outboundOptions);
				});
			}
			catch (StoragePermanentException exc)
			{
				ItemConversion.SaveFailedMimeDocument(documentIn.RootPart, outboundOptions, exc);
				throw;
			}
		}

		public static void ConvertAnyMimeToLegacyTnef(EmailMessage emailMessage, Stream mimeOut, OutboundConversionOptions outboundOptions)
		{
			StorageGlobals.ContextTraceInformation(ExTraceGlobals.CcGenericTracer, "ItemConversion::ConvertAnyMimeToLegacyTnef.1");
			Util.ThrowOnNullArgument(emailMessage, "emailMessage");
			Util.ThrowOnNullArgument(mimeOut, "mimeOut");
			Util.ThrowOnNullArgument(outboundOptions, "outboundOptions");
			Util.ThrowOnNullOrEmptyArgument(outboundOptions.ImceaEncapsulationDomain, "outboundOptions.ImceaEncapsulationDomain");
			try
			{
				ItemConversion.InternalConvertAnyMimeToLegacyTnef(emailMessage, mimeOut, outboundOptions);
			}
			catch (StoragePermanentException exc)
			{
				ItemConversion.SaveFailedMimeDocument(emailMessage.RootPart, outboundOptions, exc);
				throw;
			}
		}

		public static void CleanItemForMimeConversion(Item item)
		{
			item.AttachmentCollection.RemoveAll();
			if (item is MessageItem)
			{
				(item as MessageItem).Recipients.Clear();
			}
			List<NativeStorePropertyDefinition> list = new List<NativeStorePropertyDefinition>();
			foreach (NativeStorePropertyDefinition nativeStorePropertyDefinition in item.AllNativeProperties)
			{
				if (ItemConversion.DoesPropertyAffectMime(nativeStorePropertyDefinition))
				{
					list.Add(nativeStorePropertyDefinition);
				}
			}
			foreach (NativeStorePropertyDefinition propertyDefinition in list)
			{
				item.Delete(propertyDefinition);
			}
			item.Body.Reset();
			item.SaveFlags |= PropertyBagSaveFlags.IgnoreMapiComputedErrors;
		}

		internal static bool DoesPropertyAffectMime(NativeStorePropertyDefinition prop)
		{
			if (ItemConversion.mimeAffectingProperties.Contains(prop))
			{
				return true;
			}
			if (prop.SpecifiedWith == PropertyTypeSpecifier.GuidString)
			{
				GuidNamePropertyDefinition guidNamePropertyDefinition = (GuidNamePropertyDefinition)prop;
				if (guidNamePropertyDefinition.Guid == WellKnownPropertySet.InternetHeaders && ItemToMimeConverter.IsValidHeader(guidNamePropertyDefinition.PropertyName) && !ItemToMimeConverter.IsReservedHeader(guidNamePropertyDefinition.PropertyName))
				{
					return true;
				}
			}
			return false;
		}

		internal static bool IsLimitCheckSuppressedForThisClass(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Replication");
		}

		internal static MimeDocument LoadInboundMimeDocument(Stream mimeIn, InboundConversionOptions inboundOptions)
		{
			MimeDocument mimeDocument = null;
			bool flag = false;
			long num = -1L;
			if (mimeIn.CanSeek)
			{
				num = mimeIn.Position;
			}
			if (inboundOptions == null)
			{
				throw new ArgumentException("inboundOptions may not both be null");
			}
			try
			{
				DecodingOptions headerDecodingOptions = new DecodingOptions(DecodingOptions.Default.DecodingFlags, inboundOptions.DefaultCharset.Name);
				mimeDocument = new MimeDocument(headerDecodingOptions, inboundOptions.Limits.MimeLimits);
				mimeDocument.Load(mimeIn, mimeIn.CanSeek ? CachingMode.Source : CachingMode.Copy);
				flag = true;
			}
			finally
			{
				if (!flag && mimeDocument != null)
				{
					mimeDocument.Dispose();
					mimeDocument = null;
				}
				if (mimeIn.CanSeek && num >= 0L)
				{
					mimeIn.Position = num;
				}
			}
			return mimeDocument;
		}

		internal static void SaveFailedMimeDocument(MimePart rootPart, InboundConversionOptions options, Exception exc)
		{
			ItemConversion.SaveFailedMimeDocument(rootPart, options.ToString(), exc.ToString(), options.LogDirectoryPath);
		}

		internal static void SaveFailedMimeDocument(MimePart rootPart, OutboundConversionOptions options, Exception exc)
		{
			ItemConversion.SaveFailedMimeDocument(rootPart, options.ToString(), exc.ToString(), options.LogDirectoryPath);
		}

		public static void SaveFailedMimeDocument(EmailMessage message, Exception exc, string logDirectoryPath)
		{
			ItemConversion.SaveFailedMimeDocument(message.RootPart, null, exc.ToString(), logDirectoryPath);
		}

		public static bool TryOpenSMimeContent(MessageItem messageItem, string imceaDomain, out Item item)
		{
			return ItemConversion.TryOpenSMimeContent(messageItem.CoreItem, imceaDomain, out item);
		}

		public static bool TryOpenSMimeContent(ICoreItem sourceCoreItem, string imceaDomain, out Item item)
		{
			Util.ThrowOnNullOrEmptyArgument(imceaDomain, "imceaDomain");
			return ItemConversion.TryOpenSMimeContent(sourceCoreItem, new InboundConversionOptions(imceaDomain), out item);
		}

		public static bool TryOpenSMimeContent(MessageItem messageItem, InboundConversionOptions options, out Item item)
		{
			return ItemConversion.TryOpenSMimeContent(messageItem.CoreItem, options, out item);
		}

		public static bool TryOpenSMimeContent(ICoreItem sourceCoreItem, InboundConversionOptions options, out Item item)
		{
			item = null;
			Util.ThrowOnNullArgument(sourceCoreItem, "sourceCoreItem");
			Util.ThrowOnNullArgument(options, "options");
			if (!options.IgnoreImceaDomain)
			{
				Util.ThrowOnNullOrEmptyArgument(options.ImceaEncapsulationDomain, "options.ImceaEncapsulationDomain");
			}
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				bool isClearSigned;
				bool flag;
				StreamAttachment streamAttachment;
				if (!ConvertUtils.IsSmimeMessage(sourceCoreItem, out isClearSigned, out flag, out streamAttachment))
				{
					StorageGlobals.ContextTraceError(ExTraceGlobals.CcInboundMimeTracer, "ItemConversion::TryOpenSmimeContent: message is not multipart/signed or opaque signed.");
					return false;
				}
				disposeGuard.Add<StreamAttachment>(streamAttachment);
				sourceCoreItem.PropertyBag.Load(InternalSchema.ContentConversionProperties);
				using (MessageItem message = MessageItem.CreateInMemory(InternalSchema.ContentConversionProperties))
				{
					foreach (NativeStorePropertyDefinition nativeStorePropertyDefinition in CoreObject.GetPersistablePropertyBag(sourceCoreItem).AllNativeProperties)
					{
						if (!Body.BodyPropSet.Contains(nativeStorePropertyDefinition) && nativeStorePropertyDefinition != InternalSchema.NativeBodyInfo && nativeStorePropertyDefinition != InternalSchema.TransportMessageHeaders)
						{
							PersistablePropertyBag.CopyProperty(CoreObject.GetPersistablePropertyBag(sourceCoreItem), nativeStorePropertyDefinition, message.PropertyBag);
						}
					}
					Stream signedContent = null;
					using (signedContent = streamAttachment.GetContentStream())
					{
						bool parseSuccess = true;
						try
						{
							ConvertUtils.CallCts(ExTraceGlobals.CcInboundMimeTracer, "ItemConversion::TryOpenSmimeContent", ServerStrings.ConversionInvalidSmimeClearSignedContent, delegate
							{
								using (MimeDocument mimeDocument = new MimeDocument())
								{
									mimeDocument.Load(signedContent, signedContent.CanSeek ? CachingMode.Source : CachingMode.Copy);
									if (isClearSigned)
									{
										MimePart mimePart = mimeDocument.RootPart.FirstChild as MimePart;
										if (mimePart == null)
										{
											StorageGlobals.ContextTraceError(ExTraceGlobals.CcInboundMimeTracer, "ItemConversion::TryOpenSmimeContent: no root part.");
											parseSuccess = false;
											goto IL_112;
										}
										mimePart.RemoveFromParent();
										using (MimeDocument mimeDocument2 = new MimeDocument())
										{
											mimeDocument2.RootPart = mimePart;
											ItemConversion.ConvertAnyMimeToItem(message, mimeDocument2, options, MimePromotionFlags.SkipMessageHeaders);
											goto IL_112;
										}
									}
									PooledMemoryStream pooledMemoryStream2;
									PooledMemoryStream pooledMemoryStream = pooledMemoryStream2 = new PooledMemoryStream(6);
									try
									{
										signedContent.Position = 0L;
										if (MimeHelpers.GetOpaqueContent(signedContent, pooledMemoryStream))
										{
											pooledMemoryStream.Position = 0L;
											ItemConversion.ConvertAnyMimeToItem(message, pooledMemoryStream, options);
										}
										else
										{
											parseSuccess = false;
										}
									}
									finally
									{
										if (pooledMemoryStream2 != null)
										{
											((IDisposable)pooledMemoryStream2).Dispose();
										}
									}
									IL_112:;
								}
							});
						}
						catch (ConversionFailedException)
						{
							return false;
						}
						if (!parseSuccess)
						{
							return false;
						}
					}
					PersistablePropertyBag.CopyProperty(CoreObject.GetPersistablePropertyBag(sourceCoreItem), InternalSchema.TransportMessageHeaders, message.PropertyBag);
					PersistablePropertyBag.CopyProperties(CoreObject.GetPersistablePropertyBag(sourceCoreItem), message.PropertyBag, new PropertyDefinition[]
					{
						InternalSchema.From,
						InternalSchema.Sender
					});
					message.CoreItem.Recipients.CopyRecipientsFrom(sourceCoreItem.Recipients);
					if (!string.Equals(message.ClassName, "IPM.Note", StringComparison.OrdinalIgnoreCase))
					{
						string text = sourceCoreItem.ClassName();
						if (text.EndsWith("SMIME.MultipartSigned", StringComparison.OrdinalIgnoreCase))
						{
							int num = text.Length - "SMIME.MultipartSigned".Length;
							if (text[num - 1] == '.')
							{
								num--;
							}
							text = text.Substring(0, num);
						}
						message.ClassName = text;
					}
					message.Save(SaveMode.NoConflictResolution);
					message.Load(InternalSchema.ContentConversionProperties);
					item = Item.TransferOwnershipOfCoreItem(message);
				}
			}
			return true;
		}

		public static void ReplayInboundContent(Item replayItem, Item targetItem)
		{
			Util.ThrowOnNullArgument(replayItem, "replayItem");
			Util.ThrowOnNullArgument(targetItem, "targetItem");
			targetItem.LocationIdentifierHelperInstance.SetLocationIdentifier(45173U, LastChangeAction.ReplayInboundContent);
			Item.CopyItemContent(replayItem, targetItem);
			targetItem.CharsetDetector.DetectionOptions = replayItem.CharsetDetector.DetectionOptions;
			targetItem.SaveFlags |= (replayItem.SaveFlags | PropertyBagSaveFlags.IgnoreMapiComputedErrors);
		}

		public static void SaveFailedItem(Item item, OutboundConversionOptions options, Exception exc)
		{
			string failedOutboundConversionsDirectory = ConvertUtils.GetFailedOutboundConversionsDirectory(options.LogDirectoryPath);
			if (failedOutboundConversionsDirectory != null)
			{
				try
				{
					string str = failedOutboundConversionsDirectory + Guid.NewGuid().ToString();
					string path = str + ".txt";
					string filePath = str + ".msg";
					using (FileStream fileStream = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
					{
						using (StreamWriter streamWriter = new StreamWriter(fileStream))
						{
							streamWriter.WriteLine(exc.ToString());
							streamWriter.WriteLine();
							streamWriter.WriteLine(options.ToString());
							streamWriter.Close();
						}
						fileStream.Close();
					}
					MapiMessage mapiMessage = item.MapiMessage;
					if (mapiMessage != null)
					{
						StoreSession session = item.Session;
						object thisObject = null;
						bool flag = false;
						try
						{
							if (session != null)
							{
								session.BeginMapiCall();
								session.BeginServerHealthCall();
								flag = true;
							}
							if (StorageGlobals.MapiTestHookBeforeCall != null)
							{
								StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
							}
							mapiMessage.SaveMessageToFile(filePath, true);
						}
						catch (MapiPermanentException ex)
						{
							throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSaveMessageStream, ex, session, thisObject, "{0}. MapiException = {1}.", new object[]
							{
								string.Format("ItemConversion.SaveFailedItem().", new object[0]),
								ex
							});
						}
						catch (MapiRetryableException ex2)
						{
							throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSaveMessageStream, ex2, session, thisObject, "{0}. MapiException = {1}.", new object[]
							{
								string.Format("ItemConversion.SaveFailedItem().", new object[0]),
								ex2
							});
						}
						finally
						{
							try
							{
								if (session != null)
								{
									session.EndMapiCall();
									if (flag)
									{
										session.EndServerHealthCall();
									}
								}
							}
							finally
							{
								if (StorageGlobals.MapiTestHookAfterCall != null)
								{
									StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
								}
							}
						}
					}
				}
				catch (IOException)
				{
				}
				catch (StoragePermanentException)
				{
				}
				catch (StorageTransientException)
				{
				}
			}
		}

		public static void ConvertItemToMsgStorage(Item itemIn, Stream msgStorageStream, OutboundConversionOptions conversionOptions)
		{
			StorageGlobals.ContextTraceInformation(ExTraceGlobals.CcOutboundMimeTracer, "ItemConversion::ConvertItemToMsgStorage");
			Util.ThrowOnNullArgument(itemIn, "itemIn");
			Util.ThrowOnNullArgument(msgStorageStream, "msgStorageStream");
			Util.ThrowOnNullArgument(conversionOptions, "conversionOptions");
			if (!itemIn.HasAllPropertiesLoaded)
			{
				throw new InvalidOperationException(ServerStrings.ConversionMustLoadAllPropeties);
			}
			if (!msgStorageStream.CanWrite || !msgStorageStream.CanRead)
			{
				throw new ArgumentException("msgStorageStream", "must be both writable and readable");
			}
			ConvertUtils.CallCts(ExTraceGlobals.CcOutboundMimeTracer, "ItemConversion::ConvertItemToMsgStorage", ServerStrings.ConversionCorruptContent, delegate
			{
				OutboundMsgConverter outboundMsgConverter = new OutboundMsgConverter(conversionOptions);
				outboundMsgConverter.ConvertItemToMsgStorage(itemIn, msgStorageStream);
			});
		}

		internal static void SaveFailedMimeDocument(MimePart rootPart, string options, string errorDescription, string logDirectoryPath)
		{
			string failedInboundConversionsDirectory = ConvertUtils.GetFailedInboundConversionsDirectory(logDirectoryPath);
			if (failedInboundConversionsDirectory != null)
			{
				try
				{
					string str = failedInboundConversionsDirectory + Guid.NewGuid().ToString();
					string path = str + ".txt";
					string text = str + ".eml";
					using (FileStream fileStream = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
					{
						using (StreamWriter streamWriter = new StreamWriter(fileStream))
						{
							streamWriter.WriteLine(errorDescription);
							if (options != null)
							{
								streamWriter.WriteLine(options);
							}
							streamWriter.Close();
						}
						fileStream.Close();
					}
					using (FileStream fileStream2 = new FileStream(text, FileMode.CreateNew, FileAccess.Write))
					{
						if (rootPart != null)
						{
							rootPart.WriteTo(fileStream2);
						}
						fileStream2.Close();
						StorageGlobals.ContextTraceDebug<string>(ExTraceGlobals.CcInboundGenericTracer, "InboundMimeConverter::SaveFailedMimeDocument - filename: {0}", text);
					}
				}
				catch (IOException)
				{
				}
				catch (ExchangeDataException)
				{
				}
			}
		}

		public static MessageItem OpenRestrictedContent(MessageItem sourceMessage, OrganizationId organizationId, bool acquireLicenses, out bool licenseAcquired, out UseLicenseAndUsageRights useLicenseValue, out RestrictionInfo restrictionInfo)
		{
			Util.ThrowOnNullArgument(sourceMessage, "sourceMessage");
			Util.ThrowOnNullArgument(organizationId, "organizationId");
			if (!sourceMessage.IsRestricted)
			{
				throw new ArgumentException(ServerStrings.MessageNotRightsProtected, "sourceMessage");
			}
			licenseAcquired = false;
			useLicenseValue = null;
			StoreSession session = sourceMessage.Session;
			if (session == null)
			{
				ICoreItem topLevelItem = sourceMessage.CoreItem.TopLevelItem;
				if (topLevelItem == null || topLevelItem.Session == null)
				{
					StorageGlobals.ContextTraceError(ExTraceGlobals.CcGenericTracer, "Cannot decode in-memory message.");
					throw new NotSupportedException("Cannot decode in-memory message.");
				}
				session = topLevelItem.Session;
			}
			MailboxSession sessionAsMailboxSession = session as MailboxSession;
			if (sessionAsMailboxSession == null)
			{
				StorageGlobals.ContextTraceError(ExTraceGlobals.CcGenericTracer, "Cannot decode public folder item.");
				throw new NotSupportedException("Cannot decode public folder item.");
			}
			if (sessionAsMailboxSession.MailboxOwner.RecipientTypeDetails != RecipientTypeDetails.GroupMailbox && sessionAsMailboxSession.LogonType == LogonType.Delegated)
			{
				StorageGlobals.ContextTraceError(ExTraceGlobals.CcGenericTracer, "Delegate logon not supported.");
				throw new NotSupportedException("Delegate logon not supported.");
			}
			string userIdentity = sessionAsMailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString();
			if (string.IsNullOrEmpty(userIdentity))
			{
				StorageGlobals.ContextTraceError(ExTraceGlobals.CcGenericTracer, "PrimarySmtpAddress must not be empty.");
				throw new RightsManagementPermanentException(RightsManagementFailureCode.ADUserNotFound, ServerStrings.UnableToLoadDrmMessage);
			}
			SecurityIdentifier userSid = sessionAsMailboxSession.MailboxOwner.Sid;
			RecipientTypeDetails userType = sessionAsMailboxSession.MailboxOwner.RecipientTypeDetails;
			MessageItem convertedItem2;
			using (Attachment attachment = sourceMessage.AttachmentCollection.TryOpenFirstAttachment(Microsoft.Exchange.Data.Storage.AttachmentType.Stream))
			{
				StreamAttachmentBase streamAttachmentBase = attachment as StreamAttachmentBase;
				if (streamAttachmentBase == null)
				{
					StorageGlobals.ContextTraceError(ExTraceGlobals.CcGenericTracer, "message.rpmsg attachment not of expected type.");
					throw new RightsManagementPermanentException(RightsManagementFailureCode.CorruptData, ServerStrings.MessageRpmsgAttachmentIncorrectType);
				}
				using (Stream contentStream = streamAttachmentBase.GetContentStream(PropertyOpenMode.ReadOnly))
				{
					using (DrmEmailMessageContainer drmMessageContainer = new DrmEmailMessageContainer())
					{
						ItemConversion.<>c__DisplayClass43 CS$<>8__locals3 = new ItemConversion.<>c__DisplayClass43();
						if (contentStream == null)
						{
							StorageGlobals.ContextTraceError(ExTraceGlobals.CcGenericTracer, "message.rpmsg attachment stream is null.");
							throw new RightsManagementPermanentException(RightsManagementFailureCode.CorruptData, ServerStrings.InvalidRpMsgFormat);
						}
						try
						{
							drmMessageContainer.Load(contentStream, (object param0) => Streams.CreateTemporaryStorageStream());
						}
						catch (InvalidRpmsgFormatException ex)
						{
							StorageGlobals.ContextTraceError<string>(ExTraceGlobals.CcGenericTracer, "Failed to load DrmMessageContainer. Exception: {0}", ex.Message);
							throw new RightsManagementPermanentException(ServerStrings.InvalidRpMsgFormat, ex);
						}
						if (string.IsNullOrEmpty(drmMessageContainer.PublishLicense))
						{
							StorageGlobals.ContextTraceError(ExTraceGlobals.CcGenericTracer, "Publish license must not be empty.");
							throw new RightsManagementPermanentException(RightsManagementFailureCode.CorruptData, ServerStrings.UnableToLoadDrmMessage);
						}
						string publishLicense = drmMessageContainer.PublishLicense;
						ItemConversion.<>c__DisplayClass43 CS$<>8__locals4 = CS$<>8__locals3;
						int? valueAsNullable = sourceMessage.GetValueAsNullable<int>(MessageItemSchema.DRMRights);
						CS$<>8__locals4.usageRights = ((valueAsNullable != null) ? new ContentRight?((ContentRight)valueAsNullable.GetValueOrDefault()) : null);
						CS$<>8__locals3.expiryTime = sourceMessage.GetValueAsNullable<ExDateTime>(MessageItemSchema.DRMExpiryTime);
						byte[] valueOrDefault = sourceMessage.GetValueOrDefault<byte[]>(MessageItemSchema.DRMPropsSignature);
						CS$<>8__locals3.useLicense = null;
						CS$<>8__locals3.licAcquired = false;
						CS$<>8__locals3.useLicenseAndUsageRights = null;
						if (!PropertyError.IsPropertyNotFound(sourceMessage.TryGetProperty(MessageItemSchema.DRMServerLicenseCompressed)))
						{
							using (Stream stream = sourceMessage.OpenPropertyStream(MessageItemSchema.DRMServerLicenseCompressed, PropertyOpenMode.ReadOnly))
							{
								if (stream != null)
								{
									CS$<>8__locals3.useLicense = DrmEmailCompression.DecompressUseLicense(stream);
								}
							}
						}
						CS$<>8__locals3.context = new RmsClientManagerContext(organizationId, RmsClientManagerContext.ContextId.MessageId, sourceMessage.InternetMessageId, publishLicense);
						CS$<>8__locals3.decryptorHandleFromExistingLicense = null;
						try
						{
							if (CS$<>8__locals3.usageRights != null && CS$<>8__locals3.expiryTime != null && valueOrDefault != null && !string.IsNullOrEmpty(CS$<>8__locals3.useLicense))
							{
								try
								{
									CS$<>8__locals3.decryptorHandleFromExistingLicense = RmsClientManager.VerifyDRMPropsSignatureAndGetDecryptor(CS$<>8__locals3.context, userSid, userType, userIdentity, CS$<>8__locals3.usageRights.Value, CS$<>8__locals3.expiryTime.Value, valueOrDefault, CS$<>8__locals3.useLicense, publishLicense, UsageRightsSignatureVerificationOptions.LookupSidHistory, ItemConversion.EmptySidList);
									goto IL_570;
								}
								catch (BadDRMPropsSignatureException ex2)
								{
									StorageGlobals.ContextTraceError<string>(ExTraceGlobals.CcGenericTracer, "DRMProps signature not verified.  Exception: {0}", ex2.Message);
									CS$<>8__locals3.usageRights = null;
									CS$<>8__locals3.expiryTime = null;
									CS$<>8__locals3.useLicense = null;
									goto IL_570;
								}
								catch (RightsManagementException ex3)
								{
									if (ex3.IsPermanent)
									{
										StorageGlobals.ContextTraceError<string>(ExTraceGlobals.CcGenericTracer, "Failed to verify DRMProps signature.  Permanent exception: {0}", ex3.Message);
										throw new RightsManagementPermanentException(ServerStrings.FailedToVerifyDRMPropsSignature(userIdentity, (int)ex3.FailureCode), ex3);
									}
									StorageGlobals.ContextTraceError<string>(ExTraceGlobals.CcGenericTracer, "Failed to verify DRMProps signature.  Transient exception: {0}", ex3.Message);
									throw new RightsManagementTransientException(ServerStrings.FailedToVerifyDRMPropsSignature(userIdentity, (int)ex3.FailureCode), ex3);
								}
								catch (ExchangeConfigurationException ex4)
								{
									StorageGlobals.ContextTraceError<string>(ExTraceGlobals.CcGenericTracer, "Failed to verify DRMProps signature.  Transient exception: {0}", ex4.Message);
									throw new RightsManagementTransientException(ServerStrings.FailedToVerifyDRMPropsSignature(userIdentity, -2147160044), ex4);
								}
							}
							CS$<>8__locals3.usageRights = null;
							CS$<>8__locals3.expiryTime = null;
							CS$<>8__locals3.useLicense = null;
							IL_570:
							if (CS$<>8__locals3.usageRights == null || CS$<>8__locals3.expiryTime == null || string.IsNullOrEmpty(CS$<>8__locals3.useLicense))
							{
								if (!acquireLicenses)
								{
									StorageGlobals.ContextTraceError(ExTraceGlobals.CcGenericTracer, "Missing licenses.");
									throw new MissingRightsManagementLicenseException(sourceMessage.StoreObjectId, sessionAsMailboxSession.MailboxOwner.MailboxInfo.IsArchive, sessionAsMailboxSession.MailboxOwnerLegacyDN, publishLicense);
								}
								if (sessionAsMailboxSession.IsGroupMailbox())
								{
									userIdentity = ItemConversion.GetUserIdentityForGroupMailbox(sessionAsMailboxSession, CS$<>8__locals3.context);
								}
								try
								{
									CS$<>8__locals3.useLicenseAndUsageRights = RmsClientManager.AcquireUseLicenseAndUsageRights(CS$<>8__locals3.context, publishLicense, userIdentity, userSid, userType);
									CS$<>8__locals3.useLicense = CS$<>8__locals3.useLicenseAndUsageRights.UseLicense;
									CS$<>8__locals3.usageRights = new ContentRight?(CS$<>8__locals3.useLicenseAndUsageRights.UsageRights);
									CS$<>8__locals3.expiryTime = new ExDateTime?(CS$<>8__locals3.useLicenseAndUsageRights.ExpiryTime);
									CS$<>8__locals3.licAcquired = true;
								}
								catch (RightsManagementException ex5)
								{
									if (ex5.FailureCode == RightsManagementFailureCode.ServerRightNotGranted)
									{
										StorageGlobals.ContextTraceError<string>(ExTraceGlobals.CcGenericTracer, "AcquireUseLicenseAndUsageRights: Super user is not configured.  Exception: {0}", ex5.Message);
										throw new RightsManagementPermanentException(ServerStrings.FailedToRetrieveServerLicense((int)ex5.FailureCode), ex5);
									}
									if (ex5.IsPermanent)
									{
										StorageGlobals.ContextTraceError<string>(ExTraceGlobals.CcGenericTracer, "Failed to acquire use license and/or usage rights.  Permanent exception: {0}", ex5.Message);
										throw new RightsManagementPermanentException(ServerStrings.FailedToRetrieveUserLicense(userIdentity, (int)ex5.FailureCode), ex5);
									}
									StorageGlobals.ContextTraceError<string>(ExTraceGlobals.CcGenericTracer, "Failed to acquire use license and/or usage rights.  Transient exception: {0}", ex5.Message);
									throw new RightsManagementTransientException(ServerStrings.FailedToRetrieveUserLicense(userIdentity, (int)ex5.FailureCode), ex5);
								}
								catch (ExchangeConfigurationException ex6)
								{
									StorageGlobals.ContextTraceError<string>(ExTraceGlobals.CcGenericTracer, "Failed to acquire use license and/or usage rights.  Transient exception: {0}", ex6.Message);
									throw new RightsManagementTransientException(ServerStrings.FailedToRetrieveUserLicense(userIdentity, -2147160044), ex6);
								}
							}
							Uri licenseUri = null;
							MsgToRpMsgConverter.CallRM(delegate
							{
								XmlNode[] array;
								bool flag;
								RmsClientManager.GetLicensingUri(organizationId, drmMessageContainer.PublishLicense, out licenseUri, out array, out flag);
							}, ServerStrings.FailedToFindIssuanceLicenseAndURI);
							MessageItem convertedItem = null;
							MsgToRpMsgConverter.CallRM(delegate
							{
								using (DisposableTenantLicensePair disposableTenantLicensePair = RmsClientManager.AcquireTenantLicenses(CS$<>8__locals3.context, licenseUri))
								{
									try
									{
										RpMsgToMsgConverter rpMsgToMsgConverter = new RpMsgToMsgConverter(drmMessageContainer, organizationId, false);
										if (CS$<>8__locals3.decryptorHandleFromExistingLicense != null)
										{
											convertedItem = rpMsgToMsgConverter.ConvertRpmsgToMsg(sourceMessage, CS$<>8__locals3.decryptorHandleFromExistingLicense, CS$<>8__locals3.useLicense);
										}
										else
										{
											convertedItem = rpMsgToMsgConverter.ConvertRpmsgToMsg(sourceMessage, CS$<>8__locals3.useLicense, disposableTenantLicensePair.EnablingPrincipalRac);
										}
									}
									catch (RightsManagementException ex7)
									{
										StorageGlobals.ContextTraceError<string>(ExTraceGlobals.CcGenericTracer, "Failed to decrypt message.  Exception: {0}", ex7.Message);
										if (!acquireLicenses)
										{
											StorageGlobals.ContextTraceError(ExTraceGlobals.CcGenericTracer, "Existing use license in DRMServerCompressedLicense is not good.  Throwing MissingRightsManagementLicenseException.");
											throw new MissingRightsManagementLicenseException(sourceMessage.StoreObjectId, sessionAsMailboxSession.MailboxOwner.MailboxInfo.IsArchive, sessionAsMailboxSession.MailboxOwnerLegacyDN, drmMessageContainer.PublishLicense, ex7);
										}
										CS$<>8__locals3.useLicenseAndUsageRights = RmsClientManager.AcquireUseLicenseAndUsageRights(CS$<>8__locals3.context, drmMessageContainer.PublishLicense, userIdentity, userSid, userType);
										CS$<>8__locals3.useLicense = CS$<>8__locals3.useLicenseAndUsageRights.UseLicense;
										CS$<>8__locals3.usageRights = new ContentRight?(CS$<>8__locals3.useLicenseAndUsageRights.UsageRights);
										CS$<>8__locals3.expiryTime = new ExDateTime?(CS$<>8__locals3.useLicenseAndUsageRights.ExpiryTime);
										CS$<>8__locals3.licAcquired = true;
										RpMsgToMsgConverter rpMsgToMsgConverter2 = new RpMsgToMsgConverter(drmMessageContainer, organizationId, false);
										convertedItem = rpMsgToMsgConverter2.ConvertRpmsgToMsg(sourceMessage, CS$<>8__locals3.useLicense, disposableTenantLicensePair.EnablingPrincipalRac);
									}
								}
							}, ServerStrings.GenericFailureRMDecryption);
							string conversationOwner = string.Empty;
							MsgToRpMsgConverter.CallRM(delegate
							{
								conversationOwner = DrmClientUtils.GetConversationOwnerFromPublishLicense(drmMessageContainer.PublishLicense);
							}, ServerStrings.GenericFailureRMDecryption);
							restrictionInfo = new RestrictionInfo(CS$<>8__locals3.usageRights.Value, CS$<>8__locals3.expiryTime.Value, conversationOwner);
							if (CS$<>8__locals3.licAcquired)
							{
								licenseAcquired = true;
								useLicenseValue = CS$<>8__locals3.useLicenseAndUsageRights;
							}
							convertedItem2 = convertedItem;
						}
						finally
						{
							if (CS$<>8__locals3.decryptorHandleFromExistingLicense != null)
							{
								CS$<>8__locals3.decryptorHandleFromExistingLicense.Close();
								CS$<>8__locals3.decryptorHandleFromExistingLicense = null;
							}
						}
					}
				}
			}
			return convertedItem2;
		}

		private static string GetUserIdentityForGroupMailbox(MailboxSession sessionAsMailboxSession, RmsClientManagerContext context)
		{
			if (sessionAsMailboxSession.DelegateUser != null)
			{
				return sessionAsMailboxSession.DelegateUser.PrimarySmtpAddress.ToString();
			}
			Result<ADRecipient>[] array = context.RecipientSession.FindADRecipientsByLegacyExchangeDNs(new string[]
			{
				sessionAsMailboxSession.UserLegacyDN
			});
			if (array != null && array.Any<Result<ADRecipient>>())
			{
				return array[0].Data.PrimarySmtpAddress.ToString();
			}
			throw new RightsManagementPermanentException(RightsManagementFailureCode.ADUserNotFound, ServerStrings.UnableToLoadDrmMessage);
		}

		private const string MailFileExtension = ".msg";

		private const string MimeFileExtension = ".eml";

		private const string ErrorInfoExtension = ".txt";

		private static readonly SecurityIdentifier[] EmptySidList = Array<SecurityIdentifier>.Empty;

		private static HashSet<NativeStorePropertyDefinition> mimeAffectingProperties = new HashSet<NativeStorePropertyDefinition>(new NativeStorePropertyDefinition[]
		{
			InternalSchema.ItemClass,
			InternalSchema.TransportMessageHeaders,
			InternalSchema.SentRepresentingDisplayName,
			InternalSchema.SentRepresentingEmailAddress,
			InternalSchema.SentRepresentingType,
			InternalSchema.SentRepresentingEntryId,
			InternalSchema.SentRepresentingSmtpAddress,
			InternalSchema.SipUri,
			InternalSchema.SenderDisplayName,
			InternalSchema.SenderEmailAddress,
			InternalSchema.SenderAddressType,
			InternalSchema.SenderEntryId,
			InternalSchema.SenderSmtpAddress,
			InternalSchema.ReceivedByName,
			InternalSchema.ReceivedByEmailAddress,
			InternalSchema.ReceivedByAddrType,
			InternalSchema.ReceivedByEntryId,
			InternalSchema.ReceivedBySmtpAddress,
			InternalSchema.ReceivedRepresentingDisplayName,
			InternalSchema.ReceivedRepresentingEmailAddress,
			InternalSchema.ReceivedRepresentingAddressType,
			InternalSchema.ReceivedRepresentingEntryId,
			InternalSchema.ReceivedRepresentingSmtpAddress,
			InternalSchema.ReadReceiptDisplayName,
			InternalSchema.ReadReceiptEmailAddress,
			InternalSchema.ReadReceiptAddrType,
			InternalSchema.ReadReceiptEntryId,
			InternalSchema.ReadReceiptSmtpAddress,
			InternalSchema.AutoResponseSuppressInternal,
			InternalSchema.IsReadReceiptRequestedInternal,
			InternalSchema.IsDeliveryReceiptRequestedInternal,
			InternalSchema.MapiSensitivity,
			InternalSchema.InfoPathFormName,
			InternalSchema.ContentClass,
			InternalSchema.SenderTelephoneNumber,
			InternalSchema.XSenderTelephoneNumber,
			InternalSchema.VoiceMessageDuration,
			InternalSchema.XVoiceMessageDuration,
			InternalSchema.VoiceMessageSenderName,
			InternalSchema.XVoiceMessageSenderName,
			InternalSchema.FaxNumberOfPages,
			InternalSchema.XFaxNumberOfPages,
			InternalSchema.VoiceMessageAttachmentOrder,
			InternalSchema.XVoiceMessageAttachmentOrder,
			InternalSchema.CallId,
			InternalSchema.XCallId,
			InternalSchema.XRequireProtectedPlayOnPhone,
			InternalSchema.XMsExchangeUMPartnerContent,
			InternalSchema.XMsExchangeUMPartnerContext,
			InternalSchema.XMsExchangeUMPartnerStatus,
			InternalSchema.XMsExchangeUMPartnerAssignedID,
			InternalSchema.XMsExchangeUMDialPlanLanguage,
			InternalSchema.XMsExchangeUMCallerInformedOfAnalysis,
			InternalSchema.SentTime,
			InternalSchema.DeferredDeliveryTime,
			InternalSchema.MapiSubject,
			InternalSchema.SubjectPrefixInternal,
			InternalSchema.NormalizedSubjectInternal,
			InternalSchema.ConversationTopic,
			InternalSchema.ConversationIndex,
			InternalSchema.ListHelp,
			InternalSchema.ListSubscribe,
			InternalSchema.ListUnsubscribe,
			InternalSchema.InternetMessageId,
			InternalSchema.InternetReferences,
			InternalSchema.InReplyTo,
			InternalSchema.MapiReplyToNames,
			InternalSchema.MapiReplyToBlob,
			InternalSchema.AcceptLanguage,
			InternalSchema.MessageLocaleId,
			InternalSchema.IsClassified,
			InternalSchema.Classification,
			InternalSchema.ClassificationDescription,
			InternalSchema.ClassificationGuid,
			InternalSchema.ClassificationKeep,
			InternalSchema.AttachPayloadProviderGuidString,
			InternalSchema.AttachPayloadClass,
			InternalSchema.XMsExchOrganizationAuthAs,
			InternalSchema.XMsExchOrganizationAuthDomain,
			InternalSchema.XMsExchOrganizationAuthMechanism,
			InternalSchema.XMsExchOrganizationAuthSource,
			InternalSchema.ApprovalAllowedDecisionMakers,
			InternalSchema.ApprovalRequestor,
			InternalSchema.XMsExchangeOrganizationRightsProtectMessage,
			InternalSchema.IsAutoForwarded,
			InternalSchema.SenderIdStatus,
			InternalSchema.SpamConfidenceLevel,
			InternalSchema.OriginalScl,
			InternalSchema.ContentFilterPcl,
			InternalSchema.PurportedSenderDomain,
			InternalSchema.MapiInternetCpid,
			InternalSchema.Codepage,
			InternalSchema.TextBody,
			InternalSchema.HtmlBody,
			InternalSchema.RtfBody,
			InternalSchema.RtfInSync,
			InternalSchema.InboundICalStream,
			InternalSchema.AppointmentRecurrenceBlob,
			InternalSchema.AppointmentCounterStartWhole,
			InternalSchema.AppointmentCounterEndWhole,
			InternalSchema.MapiStartTime,
			InternalSchema.MapiPRStartDate,
			InternalSchema.MapiEndTime,
			InternalSchema.MapiPREndDate,
			InternalSchema.Location,
			InternalSchema.LocationURL,
			InternalSchema.GlobalObjectId,
			InternalSchema.AppointmentSequenceNumber,
			InternalSchema.AppointmentLastSequenceNumber,
			InternalSchema.OwnerCriticalChangeTime,
			InternalSchema.AttendeeCriticalChangeTime,
			InternalSchema.StartRecurTime,
			InternalSchema.Transparent,
			InternalSchema.AppointmentStateInternal,
			InternalSchema.Contact,
			InternalSchema.ContactURL,
			InternalSchema.OwnerAppointmentID,
			InternalSchema.FreeBusyStatus,
			InternalSchema.IntendedFreeBusyStatus,
			InternalSchema.MapiIsAllDayEvent,
			InternalSchema.AppointmentRecurring,
			InternalSchema.IsRecurring,
			InternalSchema.IsException,
			InternalSchema.ReminderMinutesBeforeStartInternal,
			InternalSchema.NamedContentType,
			InternalSchema.IsSingleBodyICal,
			InternalSchema.ReportingMta,
			InternalSchema.ParentKey,
			InternalSchema.OriginalMessageId,
			InternalSchema.OriginalSubject,
			InternalSchema.RemoteMta,
			InternalSchema.ReportTime,
			InternalSchema.BodyContentId,
			InternalSchema.XMSExchangeOutlookProtectionRuleVersion,
			InternalSchema.XMSExchangeOutlookProtectionRuleConfigTimestamp,
			InternalSchema.XMSExchangeOutlookProtectionRuleOverridden,
			InternalSchema.XSharingBrowseUrl,
			InternalSchema.XSharingCapabilities,
			InternalSchema.XSharingFlavor,
			InternalSchema.XSharingInstanceGuid,
			InternalSchema.XSharingLocalType,
			InternalSchema.XSharingProviderGuid,
			InternalSchema.XSharingProviderName,
			InternalSchema.XSharingProviderUrl,
			InternalSchema.XSharingRemoteName,
			InternalSchema.XSharingRemotePath,
			InternalSchema.XSharingRemoteType,
			InternalSchema.ExchangeApplicationFlags,
			InternalSchema.XGroupMailboxSmtpAddressId
		});
	}
}
