using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class AddContact : OwaForm, IRegistryOnlyForm
	{
		public AddContact() : base(false)
		{
		}

		protected Infobar Infobar
		{
			get
			{
				return this.infobar;
			}
		}

		protected AddContactToolbar Toolbar
		{
			get
			{
				return this.toolbar;
			}
		}

		protected RecipientWell RecipientWell
		{
			get
			{
				return this.recipientWell;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}
			base.OnLoad(e);
			Utilities.MakePageCacheable(base.Response);
			this.toolbar = new AddContactToolbar();
			this.recipientWell = new MessageRecipientWell();
		}

		private Infobar infobar = new Infobar();

		private AddContactToolbar toolbar;

		private MessageRecipientWell recipientWell;
	}
}
