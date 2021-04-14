using System;
using System.Globalization;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class EditMessageToolbar : Toolbar
	{
		internal EditMessageToolbar(Importance importance, Markup currentMarkup, bool isSMimeControlMustUpdate, bool isSMimeEditForm, bool isIrmProtected, bool isNotOwner) : this(importance, currentMarkup)
		{
			this.isSMimeControlMustUpdate = isSMimeControlMustUpdate;
			this.isSMimeEditForm = isSMimeEditForm;
			this.isIrmProtected = isIrmProtected;
			this.isNotOwner = isNotOwner;
		}

		internal EditMessageToolbar(Importance importance)
		{
			this.importance = Importance.Normal;
			this.isComplianceButtonAllowedInForm = true;
			this.isComplianceButtonEnabledInForm = true;
			this.isSendButtonEnabledInForm = true;
			base..ctor(ToolbarType.Form);
			this.importance = importance;
		}

		internal EditMessageToolbar(Importance importance, Markup currentMarkup)
		{
			this.importance = Importance.Normal;
			this.isComplianceButtonAllowedInForm = true;
			this.isComplianceButtonEnabledInForm = true;
			this.isSendButtonEnabledInForm = true;
			base..ctor(ToolbarType.Form);
			this.importance = importance;
			this.initialMarkup = currentMarkup;
		}

		public bool IsComplianceButtonAllowedInForm
		{
			get
			{
				return this.isComplianceButtonAllowedInForm;
			}
			set
			{
				this.isComplianceButtonAllowedInForm = value;
			}
		}

		public bool IsComplianceButtonEnabledInForm
		{
			get
			{
				return this.isComplianceButtonEnabledInForm;
			}
			set
			{
				this.isComplianceButtonEnabledInForm = value;
			}
		}

		protected override bool IsNarrow
		{
			get
			{
				return true;
			}
		}

		public bool IsSendButtonEnabledInForm
		{
			get
			{
				return this.isSendButtonEnabledInForm;
			}
			set
			{
				this.isSendButtonEnabledInForm = value;
			}
		}

		protected override void RenderButtons()
		{
			bool flag = base.UserContext.BrowserType == BrowserType.IE;
			ToolbarButtonFlags flags = this.isSendButtonEnabledInForm ? ToolbarButtonFlags.None : ToolbarButtonFlags.Disabled;
			base.RenderHelpButton(this.HelpId, string.Empty);
			base.RenderButton(ToolbarButtons.Send, flags);
			base.RenderButton(ToolbarButtons.SaveImageOnly);
			base.RenderButton(ToolbarButtons.AttachFile);
			if (!flag || !this.isSMimeEditForm)
			{
				base.RenderButton(ToolbarButtons.InsertImage);
			}
			base.RenderButton(ToolbarButtons.AddressBook);
			base.RenderButton(ToolbarButtons.CheckNames);
			base.RenderButton(ToolbarButtons.ImportanceHigh, (this.importance == Importance.High) ? ToolbarButtonFlags.Pressed : ToolbarButtonFlags.None);
			base.RenderButton(ToolbarButtons.ImportanceLow, (this.importance == Importance.Low) ? ToolbarButtonFlags.Pressed : ToolbarButtonFlags.None);
			if (flag && (this.isSMimeControlMustUpdate || this.isSMimeEditForm))
			{
				ToolbarButtonFlags flags2 = this.isSMimeControlMustUpdate ? ToolbarButtonFlags.Disabled : ToolbarButtonFlags.None;
				base.RenderButton(ToolbarButtons.MessageDigitalSignature, flags2);
				base.RenderButton(ToolbarButtons.MessageEncryptContents, flags2);
			}
			if (base.UserContext.IsFeatureEnabled(Feature.Signature))
			{
				base.RenderButton(ToolbarButtons.InsertSignature);
			}
			if (flag)
			{
				ToolbarButtonFlags flags3 = base.UserContext.IsFeatureEnabled(Feature.SpellChecker) ? ToolbarButtonFlags.None : ToolbarButtonFlags.Disabled;
				base.RenderButton(ToolbarButtons.SpellCheck, flags3, new Toolbar.RenderMenuItems(base.RenderSpellCheckLanguageDialog));
			}
			if (this.IsComplianceButtonAllowedInForm && base.UserContext.ComplianceReader.IsComplianceFeatureEnabled(base.UserContext.IsIrmEnabled, this.isIrmProtected, CultureInfo.CurrentUICulture))
			{
				if (this.isComplianceButtonEnabledInForm)
				{
					base.RenderButton(ToolbarButtons.Compliance);
				}
				else
				{
					base.RenderButton(ToolbarButtons.Compliance, ToolbarButtonFlags.Disabled);
				}
			}
			base.RenderButton(ToolbarButtons.MessageOptions);
			base.RenderHtmlTextToggle((this.initialMarkup == Markup.Html) ? "0" : "1", this.isSMimeEditForm || this.isNotOwner);
			base.RenderButton(ToolbarButtons.MailTips);
		}

		protected virtual string HelpId
		{
			get
			{
				return HelpIdsLight.MailLight.ToString();
			}
		}

		private Importance importance;

		private Markup initialMarkup;

		private bool isComplianceButtonAllowedInForm;

		private bool isComplianceButtonEnabledInForm;

		private bool isSMimeControlMustUpdate;

		private bool isSMimeEditForm;

		private bool isIrmProtected;

		private bool isNotOwner;

		private bool isSendButtonEnabledInForm;
	}
}
