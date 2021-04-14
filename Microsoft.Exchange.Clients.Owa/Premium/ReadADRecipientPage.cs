using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Directory;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class ReadADRecipientPage : OwaForm, IRegistryOnlyForm
	{
		internal ADObjectId ADObjectId
		{
			get
			{
				return this.adObjectId;
			}
			private set
			{
				this.adObjectId = value;
			}
		}

		internal ADRecipient ADRecipient
		{
			get
			{
				return this.adRecipient;
			}
			private set
			{
				this.adRecipient = value;
			}
		}

		internal IRecipientSession ADRecipientSession
		{
			get
			{
				return this.adRecipientSession;
			}
			private set
			{
				this.adRecipientSession = value;
			}
		}

		private protected bool RecipientOutOfSearchScope
		{
			protected get
			{
				return this.recipientOutOfSearchScope;
			}
			private set
			{
				this.recipientOutOfSearchScope = value;
			}
		}

		protected string Base64EncodedId
		{
			get
			{
				return Utilities.GetBase64StringFromADObjectId(this.adObjectId);
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			this.adObjectId = DirectoryAssistance.ParseADObjectId(Utilities.GetQueryStringParameter(base.Request, "id"));
			if (this.adObjectId == null)
			{
				throw new OwaInvalidRequestException();
			}
			string queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "email", false);
			this.adRecipientSession = Utilities.CreateADRecipientSession(Microsoft.Exchange.Clients.Owa.Core.Culture.GetUserCulture().LCID, true, ConsistencyMode.FullyConsistent, true, base.UserContext, false);
			this.adRecipient = Utilities.CreateADRecipientFromProxyAddress(this.adObjectId, queryStringParameter, this.adRecipientSession);
			if (this.adRecipient == null)
			{
				if (base.UserContext.MailboxIdentity.GetOWAMiniRecipient().QueryBaseDN != null)
				{
					this.adRecipientSession = Utilities.CreateADRecipientSession(Microsoft.Exchange.Clients.Owa.Core.Culture.GetUserCulture().LCID, true, ConsistencyMode.FullyConsistent, false, base.UserContext);
					this.adRecipient = Utilities.CreateADRecipientFromProxyAddress(this.adObjectId, queryStringParameter, this.adRecipientSession);
					if (this.adRecipient != null)
					{
						this.RecipientOutOfSearchScope = true;
					}
				}
			}
			else
			{
				this.adObjectId = (ADObjectId)this.adRecipient[ADObjectSchema.Id];
				if (this.adRecipient.HiddenFromAddressListsEnabled)
				{
					this.RecipientOutOfSearchScope = true;
				}
			}
			if (this.adRecipient == null)
			{
				throw new OwaADObjectNotFoundException();
			}
			this.adRecipientSession = Utilities.CreateADRecipientSession(Microsoft.Exchange.Clients.Owa.Core.Culture.GetUserCulture().LCID, true, ConsistencyMode.IgnoreInvalid, !this.RecipientOutOfSearchScope, base.UserContext);
			string action = base.OwaContext.FormsRegistryContext.Action;
			base.IsPreviewForm = (action != null && action.Equals("Preview"));
		}

		protected void RenderPhoneticDisplayName()
		{
			if (base.UserContext.IsPhoneticNamesEnabled && !string.IsNullOrEmpty(this.adRecipient.PhoneticDisplayName))
			{
				Utilities.HtmlEncode(this.adRecipient.PhoneticDisplayName, base.Response.Output);
			}
		}

		protected void RenderDisplayName()
		{
			Utilities.HtmlEncode(this.adRecipient.DisplayName, base.Response.Output);
		}

		protected void RenderPresenceAndPhoto()
		{
			bool flag = this.adRecipient.RecipientType == RecipientType.Contact || this.adRecipient.RecipientType == RecipientType.UserMailbox || this.adRecipient.RecipientType == RecipientType.MailUser || this.adRecipient.RecipientType == RecipientType.User || this.adRecipient.RecipientType == RecipientType.MailContact;
			if (flag)
			{
				bool flag2 = base.UserContext.IsSenderPhotosFeatureEnabled(Feature.DisplayPhotos);
				if (base.UserContext.IsInstantMessageEnabled())
				{
					RenderingUtilities.RenderPresenceJellyBean(base.Response.Output, base.UserContext, false, string.Empty, flag2, "RcpJb");
				}
				if (flag2)
				{
					bool flag3 = this.adRecipient.ThumbnailPhoto != null && this.adRecipient.ThumbnailPhoto.Length > 0;
					bool flag4 = string.Equals(this.adRecipient.LegacyExchangeDN, base.UserContext.ExchangePrincipal.LegacyDn, StringComparison.OrdinalIgnoreCase);
					string srcUrl = (!flag3) ? string.Empty : RenderingUtilities.GetADPictureUrl(this.adRecipient.LegacyExchangeDN, "EX", base.UserContext, flag4);
					RenderingUtilities.RenderDisplayPicture(base.Response.Output, base.UserContext, srcUrl, 64, flag4, flag ? ThemeFileId.DoughboyPerson : ThemeFileId.DoughboyDL);
				}
			}
		}

		private ADObjectId adObjectId;

		private ADRecipient adRecipient;

		private IRecipientSession adRecipientSession;

		private bool recipientOutOfSearchScope;
	}
}
