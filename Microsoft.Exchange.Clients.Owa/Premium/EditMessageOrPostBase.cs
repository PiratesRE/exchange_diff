using System;
using System.Collections;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public abstract class EditMessageOrPostBase : EditItemForm, IRegistryOnlyForm
	{
		protected static int ImportanceLow
		{
			get
			{
				return 0;
			}
		}

		protected static int ImportanceNormal
		{
			get
			{
				return 1;
			}
		}

		protected static int ImportanceHigh
		{
			get
			{
				return 2;
			}
		}

		protected static int StoreObjectTypeMessage
		{
			get
			{
				return 9;
			}
		}

		protected static int StoreObjectTypeMeetingResponse
		{
			get
			{
				return 12;
			}
		}

		protected static int StoreObjectTypeMeetingRequest
		{
			get
			{
				return 11;
			}
		}

		protected static int StoreObjectTypeMeetingCancellation
		{
			get
			{
				return 13;
			}
		}

		protected static int StoreObjectTypeApprovalReply
		{
			get
			{
				return 27;
			}
		}

		protected bool AddSignatureToBody
		{
			get
			{
				return this.addSignatureToBody;
			}
		}

		protected bool ShouldAddSignatureToBody(Markup bodymarkup, NewItemType newitemType)
		{
			return base.UserContext.IsFeatureEnabled(Feature.Signature) && base.UserContext.UserOptions.AutoAddSignature;
		}

		internal EditMessageOrPostBase()
		{
		}

		internal EditMessageOrPostBase(bool setNoCacheNoStore) : base(setNoCacheNoStore)
		{
		}

		protected void LoadMessageBodyIntoStream(TextWriter writer)
		{
			bool flag = BodyConversionUtilities.GenerateEditableMessageBodyAndRenderInfobarMessages(base.Item, writer, this.newItemType, base.OwaContext, ref this.shouldPromptUserForUnblockingOnFormLoad, ref this.hasInlineImages, this.infobar, base.ForceAllowWebBeacon, this.bodyMarkup);
			if (flag)
			{
				base.Item.Load();
			}
		}

		protected NewItemType NewItemType
		{
			get
			{
				return this.newItemType;
			}
		}

		protected ArrayList AttachmentWellRenderObjects
		{
			get
			{
				return this.attachmentWellRenderObjects;
			}
		}

		protected Infobar Infobar
		{
			get
			{
				return this.infobar;
			}
		}

		protected bool ShouldPromptUser
		{
			get
			{
				return this.shouldPromptUserForUnblockingOnFormLoad;
			}
		}

		protected bool HasInlineImages
		{
			get
			{
				return this.hasInlineImages;
			}
		}

		protected const string ReplyAction = "Reply";

		protected const string ReplyAllAction = "ReplyAll";

		protected const string ForwardAction = "Forward";

		protected const string DraftState = "Draft";

		protected bool addSignatureToBody;

		protected ArrayList attachmentWellRenderObjects;

		protected Markup bodyMarkup;

		protected Infobar infobar = new Infobar();

		protected bool shouldPromptUserForUnblockingOnFormLoad;

		protected bool hasInlineImages;

		protected NewItemType newItemType;
	}
}
