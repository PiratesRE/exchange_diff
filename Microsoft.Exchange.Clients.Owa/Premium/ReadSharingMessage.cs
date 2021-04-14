using System;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class ReadSharingMessage : ReadMessage
	{
		protected override void OnLoad(EventArgs e)
		{
			string queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "id", true);
			if (OwaStoreObjectId.CreateFromString(queryStringParameter).IsPublic)
			{
				throw new OwaInvalidRequestException("Cannot open item in public folder with this form");
			}
			base.Message = (this.sharingMessageItem = base.Initialize<SharingMessageItem>(ReadSharingMessage.PrefetchProperties));
			base.InitializeReadMessageFormElements();
			this.sharingMessageWriter = new SharingMessageWriter(this.sharingMessageItem, base.UserContext);
			if (!this.sharingMessageItem.IsDraft)
			{
				this.AddMessagesToInfobar();
			}
		}

		protected SharingMessageWriter SharingMessageWriter
		{
			get
			{
				return this.sharingMessageWriter;
			}
		}

		protected override void AddMessagesToInfobar()
		{
			base.AddMessagesToInfobar();
			this.sharingMessageWriter.AddSharingInfoToInfobar(base.Infobar);
		}

		protected bool ShouldRenderToolbar
		{
			get
			{
				return !base.IsPreviewForm;
			}
		}

		protected bool IsDraft
		{
			get
			{
				return this.sharingMessageItem.IsDraft;
			}
		}

		protected string BrowseUrl
		{
			get
			{
				return this.sharingMessageItem.BrowseUrl;
			}
		}

		protected string RedirectBrowseUrl
		{
			get
			{
				return Redir.BuildRedirUrl(base.UserContext, this.sharingMessageItem.BrowseUrl);
			}
		}

		protected bool IsPublishing
		{
			get
			{
				return this.sharingMessageItem.IsPublishing;
			}
		}

		protected void RenderSharingToolbar()
		{
			this.sharingMessageWriter.SharingToolbar.Render(base.Response.Output);
		}

		protected override void RenderSubject()
		{
			RenderingUtilities.RenderSubject(base.Response.Output, this.sharingMessageItem);
		}

		protected bool IsInvitationOrAcceptOfRequest
		{
			get
			{
				return this.sharingMessageItem.SharingMessageType.IsInvitationOrAcceptOfRequest;
			}
		}

		public override IEnumerable<string> ExternalScriptFiles
		{
			get
			{
				return this.externalScriptFiles;
			}
		}

		public override SanitizedHtmlString Title
		{
			get
			{
				string subject = this.sharingMessageItem.Subject;
				if (string.IsNullOrEmpty(subject))
				{
					return SanitizedHtmlString.FromStringId(730745110);
				}
				return new SanitizedHtmlString(subject);
			}
		}

		public override string PageType
		{
			get
			{
				return "ReadSharingMessagePage";
			}
		}

		private const string IdQueryParameter = "id";

		private SharingMessageItem sharingMessageItem;

		private SharingMessageWriter sharingMessageWriter;

		internal static StorePropertyDefinition[] PrefetchProperties = new StorePropertyDefinition[]
		{
			ItemSchema.IsClassified,
			ItemSchema.Classification,
			ItemSchema.ClassificationDescription,
			ItemSchema.ClassificationGuid,
			StoreObjectSchema.EffectiveRights,
			ItemSchema.FlagStatus,
			ItemSchema.FlagCompleteTime,
			ItemSchema.Categories,
			MessageItemSchema.IsReadReceiptPending,
			StoreObjectSchema.PolicyTag,
			ItemSchema.RetentionDate
		};

		private string[] externalScriptFiles = new string[]
		{
			"freadsharingmsg.js"
		};
	}
}
