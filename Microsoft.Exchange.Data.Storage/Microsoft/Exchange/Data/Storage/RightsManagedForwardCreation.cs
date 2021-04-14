using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RightsManagedForwardCreation : ForwardCreation
	{
		internal RightsManagedForwardCreation(RightsManagedMessageItem originalItem, RightsManagedMessageItem message, ReplyForwardConfiguration parameters) : base(originalItem, message, parameters)
		{
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
			base.CopyAttachments(callbacks, rightsManagedMessageItem.ProtectedAttachmentCollection, rightsManagedMessageItem2.ProtectedAttachmentCollection, false, this.parameters.TargetFormat == BodyFormat.TextPlain, optionsForSmime);
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
