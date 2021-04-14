using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class EditSharingMessageToolbar : EditMessageToolbar
	{
		internal EditSharingMessageToolbar(Importance importance, Markup currentMarkup, bool isPublishing) : base(importance, currentMarkup)
		{
			base.IsComplianceButtonAllowedInForm = false;
			this.isPublishing = isPublishing;
		}

		protected override string HelpId
		{
			get
			{
				if (!this.isPublishing)
				{
					return HelpIdsLight.DefaultLight.ToString();
				}
				return HelpIdsLight.DefaultLight.ToString();
			}
		}

		private bool isPublishing;
	}
}
