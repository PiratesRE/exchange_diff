using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class ReadContactToolbar : Toolbar
	{
		internal ReadContactToolbar(Contact contact) : base(ToolbarType.Form)
		{
			if (contact == null)
			{
				throw new ArgumentNullException("contact");
			}
			this.contact = contact;
		}

		protected override void RenderButtons()
		{
			ToolbarButtonFlags flags = ToolbarButtonFlags.None;
			if (!ReadContactToolbar.CanMailToContact(this.contact))
			{
				flags = ToolbarButtonFlags.Disabled;
			}
			base.RenderButton(ToolbarButtons.NewMessageToContact, flags);
			if (base.UserContext.IsFeatureEnabled(Feature.Calendar))
			{
				base.RenderButton(ToolbarButtons.NewMeetingRequestToContact, flags);
			}
			if (base.UserContext.IsSmsEnabled)
			{
				base.RenderButton(ToolbarButtons.SendATextMessage);
			}
			ToolbarButtonFlags flags2 = ToolbarButtonFlags.None;
			if (!ItemUtility.UserCanDeleteItem(this.contact))
			{
				flags2 = ToolbarButtonFlags.Disabled;
			}
			base.RenderButton(ToolbarButtons.Delete, flags2);
		}

		private static bool CanMailToContact(ContactBase contactBase)
		{
			return contactBase is DistributionList || ReadContactToolbar.ContactHasEmailAddress(contactBase, ContactSchema.Email1) || ReadContactToolbar.ContactHasEmailAddress(contactBase, ContactSchema.Email2) || ReadContactToolbar.ContactHasEmailAddress(contactBase, ContactSchema.Email3);
		}

		private static bool ContactHasEmailAddress(ContactBase contactBase, PropertyDefinition emailProperty)
		{
			Participant participant = contactBase.TryGetProperty(emailProperty) as Participant;
			return participant != null && !string.IsNullOrEmpty(participant.EmailAddress);
		}

		private Contact contact;
	}
}
