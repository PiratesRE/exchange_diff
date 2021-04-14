using System;
using System.Collections;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class PrintMessage : OwaForm, IRegistryOnlyForm
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			string type = base.OwaContext.FormsRegistryContext.Type;
			StorePropertyDefinition[] array = new StorePropertyDefinition[]
			{
				ItemSchema.BlockStatus,
				BodySchema.Codepage,
				BodySchema.InternetCpid,
				MessageItemSchema.SenderTelephoneNumber,
				ItemSchema.FlagStatus,
				ItemSchema.FlagCompleteTime
			};
			if (ObjectClass.IsMessage(type, false))
			{
				this.message = base.Initialize<MessageItem>(array);
			}
			else
			{
				this.message = base.InitializeAsMessageItem(array);
			}
			this.IrmDecryptIfRestricted();
			this.recipientWell = new PrintRecipientWell(new MessageRecipientWell(this.message));
			this.attachmentWellRenderObjects = AttachmentWell.GetAttachmentInformation(this.message, base.AttachmentLinks, base.UserContext.IsPublicLogon, base.IsEmbeddedItem);
			this.shouldRenderAttachmentWell = (!this.IsRestrictedButIrmFeatureDisabledOrDecryptionFailed && PrintAttachmentWell.ShouldRenderAttachments(this.attachmentWellRenderObjects));
			if (this.message.Importance == Importance.High)
			{
				this.importanceString = LocalizedStrings.GetHtmlEncoded(-77932258);
			}
			else if (this.message.Importance == Importance.Low)
			{
				this.importanceString = LocalizedStrings.GetHtmlEncoded(1502599728);
			}
			switch (this.message.Sensitivity)
			{
			case Sensitivity.Personal:
				this.sensitivityString = LocalizedStrings.GetHtmlEncoded(567923294);
				break;
			case Sensitivity.Private:
				this.sensitivityString = LocalizedStrings.GetHtmlEncoded(-1268489823);
				break;
			case Sensitivity.CompanyConfidential:
				this.sensitivityString = LocalizedStrings.GetHtmlEncoded(-819101664);
				break;
			}
			this.categoriesString = ItemUtility.GetCategoriesAsString(this.message);
			if (this.message.Id != null && !this.message.IsRead && !base.IsPreviewForm)
			{
				this.message.MarkAsRead(Utilities.ShouldSuppressReadReceipt(base.UserContext, this.message), false);
			}
		}

		protected void LoadMessageBodyIntoStream(TextWriter writer)
		{
			if (this.IsRestrictedButIrmFeatureDisabledOrDecryptionFailed)
			{
				this.RenderAlternateBodyForIrm(writer);
				return;
			}
			BodyConversionUtilities.GeneratePrintMessageBody(this.message, writer, base.OwaContext, base.IsEmbeddedItem, base.IsEmbeddedItem ? base.RenderEmbeddedUrl() : null, base.ForceAllowWebBeacon, base.ForceEnableItemLink);
		}

		protected void RenderSender(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (Utilities.IsOnBehalfOf(this.message.Sender, this.message.From))
			{
				writer.Write(LocalizedStrings.GetHtmlEncoded(-165544498), RenderingUtilities.GetDisplaySenderName(this.message.Sender), RenderingUtilities.GetDisplaySenderName(this.message.From));
				return;
			}
			writer.Write(RenderingUtilities.GetDisplaySenderName(this.message.Sender));
		}

		protected void RenderSubject(bool isTitle)
		{
			if (isTitle)
			{
				RenderingUtilities.RenderSubject(base.Response.Output, this.message, LocalizedStrings.GetNonEncoded(730745110));
				return;
			}
			RenderingUtilities.RenderSubject(base.Response.Output, this.message);
		}

		protected void RenderOwaPlainTextStyle()
		{
			OwaPlainTextStyle.WriteLocalizedStyleIntoHeadForPlainTextBody(this.message, base.Response.Output, "DIV#divBdy");
		}

		protected RecipientWell RecipientWell
		{
			get
			{
				return this.recipientWell;
			}
		}

		protected bool ShouldRenderAttachmentWell
		{
			get
			{
				return this.shouldRenderAttachmentWell;
			}
		}

		protected ArrayList AttachmentWellRenderObjects
		{
			get
			{
				return this.attachmentWellRenderObjects;
			}
		}

		protected string ImportanceString
		{
			get
			{
				return this.importanceString;
			}
		}

		protected string SensitivityString
		{
			get
			{
				return this.sensitivityString;
			}
		}

		protected string CategoriesString
		{
			get
			{
				return this.categoriesString;
			}
		}

		protected ExDateTime MessageSentTime
		{
			get
			{
				return this.message.SentTime;
			}
		}

		private void RenderAlternateBodyForIrm(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write(Utilities.GetAlternateBodyForIrm(base.UserContext, Microsoft.Exchange.Data.Storage.BodyFormat.TextHtml, this.irmDecryptionStatus, ObjectClass.IsVoiceMessage(this.message.ClassName)));
		}

		private bool IsRestrictedButIrmFeatureDisabledOrDecryptionFailed
		{
			get
			{
				return this.message.IsRestricted && (!base.UserContext.IsIrmEnabled || this.irmDecryptionStatus.Failed);
			}
		}

		private void IrmDecryptIfRestricted()
		{
			if (!Utilities.IsIrmRestricted(this.message))
			{
				return;
			}
			RightsManagedMessageItem rightsManagedMessageItem = (RightsManagedMessageItem)this.message;
			if (!base.UserContext.IsIrmEnabled)
			{
				this.irmDecryptionStatus = RightsManagedMessageDecryptionStatus.FeatureDisabled;
				return;
			}
			if (!rightsManagedMessageItem.CanDecode)
			{
				this.irmDecryptionStatus = RightsManagedMessageDecryptionStatus.NotSupported;
				return;
			}
			try
			{
				if (Utilities.IrmDecryptIfRestricted(this.message, base.UserContext, true))
				{
					RightsManagedMessageItem rightsManagedMessageItem2 = (RightsManagedMessageItem)this.message;
					if (!rightsManagedMessageItem2.UsageRights.IsUsageRightGranted(ContentRight.Print))
					{
						this.irmDecryptionStatus = new RightsManagedMessageDecryptionStatus(RightsManagementFailureCode.UserRightNotGranted, null);
					}
				}
			}
			catch (RightsManagementPermanentException exception)
			{
				this.irmDecryptionStatus = RightsManagedMessageDecryptionStatus.CreateFromException(exception);
			}
			catch (RightsManagementTransientException exception2)
			{
				this.irmDecryptionStatus = RightsManagedMessageDecryptionStatus.CreateFromException(exception2);
			}
		}

		protected bool IsCopyRestricted
		{
			get
			{
				return this.IsUsageRightRestricted(ContentRight.Extract);
			}
		}

		private bool IsUsageRightRestricted(ContentRight right)
		{
			RightsManagedMessageItem rightsManagedMessageItem = this.message as RightsManagedMessageItem;
			if (rightsManagedMessageItem == null || !rightsManagedMessageItem.IsRestricted)
			{
				return false;
			}
			if (!base.OwaContext.UserContext.IsIrmEnabled)
			{
				return false;
			}
			if (this.irmDecryptionStatus.Failed)
			{
				return !right.IsUsageRightGranted(ContentRight.Extract) && !right.IsUsageRightGranted(ContentRight.Print);
			}
			return !rightsManagedMessageItem.UsageRights.IsUsageRightGranted(right);
		}

		private static readonly PropertyDefinition[] prefetchProperties = new PropertyDefinition[]
		{
			ItemSchema.BlockStatus,
			BodySchema.Codepage,
			BodySchema.InternetCpid,
			MessageItemSchema.IsDraft,
			MessageItemSchema.IsRead
		};

		private MessageItem message;

		private RecipientWell recipientWell;

		private bool shouldRenderAttachmentWell;

		private ArrayList attachmentWellRenderObjects;

		private string sensitivityString;

		private string importanceString;

		private string categoriesString;

		private RightsManagedMessageDecryptionStatus irmDecryptionStatus;
	}
}
