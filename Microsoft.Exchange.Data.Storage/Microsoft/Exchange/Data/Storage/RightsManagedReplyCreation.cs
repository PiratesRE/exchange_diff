using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RightsManagedReplyCreation : ReplyCreation
	{
		internal RightsManagedReplyCreation(RightsManagedMessageItem originalItem, RightsManagedMessageItem message, ReplyForwardConfiguration parameters, bool isReplyAll) : base(originalItem, message, parameters, isReplyAll, false, true)
		{
		}

		internal static void CopyDrmProperties(Item originalItem, Item newItem)
		{
			Util.ThrowOnNullArgument(originalItem, "originalItem");
			Util.ThrowOnNullArgument(newItem, "newItem");
			int? valueAsNullable = originalItem.GetValueAsNullable<int>(MessageItemSchema.DRMRights);
			if (valueAsNullable != null)
			{
				newItem[MessageItemSchema.DRMRights] = valueAsNullable;
			}
			ExDateTime? valueAsNullable2 = originalItem.GetValueAsNullable<ExDateTime>(MessageItemSchema.DRMExpiryTime);
			if (valueAsNullable2 != null)
			{
				newItem[MessageItemSchema.DRMExpiryTime] = valueAsNullable2;
			}
			byte[] valueOrDefault = originalItem.GetValueOrDefault<byte[]>(MessageItemSchema.DRMPropsSignature);
			if (valueOrDefault != null)
			{
				newItem[MessageItemSchema.DRMPropsSignature] = valueOrDefault;
			}
			if (!PropertyError.IsPropertyNotFound(originalItem.TryGetProperty(MessageItemSchema.DRMServerLicenseCompressed)))
			{
				using (Stream stream = originalItem.OpenPropertyStream(MessageItemSchema.DRMServerLicenseCompressed, PropertyOpenMode.ReadOnly))
				{
					using (Stream stream2 = newItem.OpenPropertyStream(MessageItemSchema.DRMServerLicenseCompressed, PropertyOpenMode.Create))
					{
						if (stream != null && stream2 != null)
						{
							using (BinaryReader binaryReader = new BinaryReader(stream))
							{
								using (BinaryWriter binaryWriter = new BinaryWriter(stream2))
								{
									binaryWriter.Write(binaryReader.ReadBytes(checked((int)stream.Length)));
								}
							}
						}
					}
				}
			}
		}

		protected override void BuildBody(BodyConversionCallbacks callbacks)
		{
			RightsManagedMessageItem rightsManagedMessageItem = this.originalItem as RightsManagedMessageItem;
			RightsManagedMessageItem rightsManagedMessageItem2 = this.newItem as RightsManagedMessageItem;
			ReplyForwardCommon.CopyBodyWithPrefix(rightsManagedMessageItem.ProtectedBody, rightsManagedMessageItem2.ProtectedBody, this.parameters, callbacks);
		}

		protected override void BuildAttachments(BodyConversionCallbacks callbacks, InboundConversionOptions optionsForSmime)
		{
			RightsManagedMessageItem rightsManagedMessageItem = this.originalItem as RightsManagedMessageItem;
			RightsManagedMessageItem rightsManagedMessageItem2 = this.newItem as RightsManagedMessageItem;
			base.CopyAttachments(callbacks, rightsManagedMessageItem.ProtectedAttachmentCollection, rightsManagedMessageItem2.ProtectedAttachmentCollection, true, this.parameters.TargetFormat == BodyFormat.TextPlain, optionsForSmime);
		}

		protected override BodyConversionCallbacks GetCallbacks()
		{
			RightsManagedMessageItem rightsManagedMessageItem = this.originalItem as RightsManagedMessageItem;
			return base.GetCallbacksInternal(rightsManagedMessageItem.ProtectedBody, rightsManagedMessageItem.ProtectedAttachmentCollection);
		}

		protected override void UpdateNewItemProperties()
		{
			base.UpdateNewItemProperties();
			RightsManagedReplyCreation.CopyDrmProperties(this.originalItem, this.newItem);
		}
	}
}
