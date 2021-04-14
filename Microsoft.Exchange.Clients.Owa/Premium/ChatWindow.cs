using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class ChatWindow : OwaForm, IRegistryOnlyForm
	{
		public ChatWindow() : base(false)
		{
		}

		protected Infobar Infobar
		{
			get
			{
				return this.infobar;
			}
		}

		protected RecipientWell EditRecipientWell
		{
			get
			{
				return this.editRecipientWell;
			}
		}

		protected RecipientWell ParticipantRecipientWell
		{
			get
			{
				return this.participantRecipientWell;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			Utilities.MakePageCacheable(base.Response);
			this.editRecipientWell = null;
			this.participantRecipientWell = new MessageRecipientWell();
		}

		private Infobar infobar = new Infobar();

		private MessageRecipientWell editRecipientWell;

		private MessageRecipientWell participantRecipientWell;
	}
}
