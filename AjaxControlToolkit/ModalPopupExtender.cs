using System;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI;

namespace AjaxControlToolkit
{
	[RequiredScript(typeof(CommonToolkitScripts))]
	[TargetControlType(typeof(Control))]
	[Designer("AjaxControlToolkit.ModalPopupDesigner, AjaxControlToolkit")]
	[ClientScriptResource("AjaxControlToolkit.ModalPopupBehavior", "AjaxControlToolkit.ModalPopup.ModalPopupBehavior.js")]
	public class ModalPopupExtender : ExtenderControlBase
	{
		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddProperty("PopupControlID", this.PopupControlID, true);
			descriptor.AddProperty("BackgroundCssClass", this.BackgroundCssClass, true);
			if (this.X != -1)
			{
				descriptor.AddProperty("X", this.X);
			}
			if (this.Y != -1)
			{
				descriptor.AddProperty("Y", this.Y);
			}
			descriptor.AddProperty("ButtonIDs", this.ButtonIDs, true);
		}

		[RequiredProperty]
		public string PopupControlID
		{
			get
			{
				return base.GetPropertyValue<string>("PopupControlID", string.Empty);
			}
			set
			{
				base.SetPropertyValue<string>("PopupControlID", value);
			}
		}

		public string BackgroundCssClass
		{
			get
			{
				return base.GetPropertyValue<string>("BackgroundCssClass", string.Empty);
			}
			set
			{
				base.SetPropertyValue<string>("BackgroundCssClass", value);
			}
		}

		public int X
		{
			get
			{
				return base.GetPropertyValue<int>("X", -1);
			}
			set
			{
				base.SetPropertyValue<int>("X", value);
			}
		}

		public int Y
		{
			get
			{
				return base.GetPropertyValue<int>("Y", -1);
			}
			set
			{
				base.SetPropertyValue<int>("Y", value);
			}
		}

		public string ButtonIDs
		{
			get
			{
				return base.GetPropertyValue<string>("ButtonIDs", string.Empty);
			}
			set
			{
				base.SetPropertyValue<string>("ButtonIDs", value);
			}
		}

		public void Show()
		{
			this.ChangeVisibility(true);
		}

		public void Hide()
		{
			this.ChangeVisibility(false);
		}

		private void ChangeVisibility(bool show)
		{
			if (base.TargetControl == null)
			{
				throw new ArgumentNullException("TargetControl", "TargetControl property cannot be null");
			}
			string text = show ? "show" : "hide";
			if (ScriptManager.GetCurrent(this.Page).IsInAsyncPostBack)
			{
				ScriptManager.GetCurrent(this.Page).RegisterDataItem(base.TargetControl, text);
				return;
			}
			string script = string.Format(CultureInfo.InvariantCulture, "(function() {{var fn = function() {{AjaxControlToolkit.ModalPopupBehavior.invokeViaServer('{0}', {1}); Sys.Application.remove_load(fn);}};Sys.Application.add_load(fn);}})();", new object[]
			{
				base.BehaviorID,
				show ? "true" : "false"
			});
			ScriptManager.RegisterStartupScript(this, typeof(ModalPopupExtender), text + base.BehaviorID, script, true);
		}

		private const string StringPopupControlID = "PopupControlID";

		private const string StringBackgroundCssClass = "BackgroundCssClass";

		private const string StringX = "X";

		private const string StringY = "Y";

		private const string StringButtonIDs = "ButtonIDs";
	}
}
