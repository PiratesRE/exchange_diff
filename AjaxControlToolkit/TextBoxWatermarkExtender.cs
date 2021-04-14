using System;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI;

namespace AjaxControlToolkit
{
	[TargetControlType(typeof(IEditableTextControl))]
	[Designer("AjaxControlToolkit.TextBoxWatermarkExtenderDesigner, AjaxControlToolkit")]
	[ClientScriptResource("AjaxControlToolkit.TextBoxWatermarkBehavior", "AjaxControlToolkit.TextboxWatermark.TextboxWatermark.js")]
	public class TextBoxWatermarkExtender : ExtenderControlBase
	{
		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddProperty("WatermarkText", this.WatermarkText, true);
			descriptor.AddProperty("WatermarkCssClass", this.WatermarkCssClass, true);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			string key = string.Format(CultureInfo.InvariantCulture, "{0}_onSubmit", new object[]
			{
				this.ID
			});
			string script = string.Format(CultureInfo.InvariantCulture, "var o = $find('{0}'); if(o) {{ o._onSubmit(); }}", new object[]
			{
				base.BehaviorID
			});
			ScriptManager.RegisterOnSubmitStatement(this, typeof(TextBoxWatermarkExtender), key, script);
		}

		[RequiredProperty]
		public string WatermarkText
		{
			get
			{
				return base.GetPropertyValue<string>("WatermarkText", string.Empty);
			}
			set
			{
				base.SetPropertyValue<string>("WatermarkText", value);
			}
		}

		public string WatermarkCssClass
		{
			get
			{
				return base.GetPropertyValue<string>("WatermarkCssClass", string.Empty);
			}
			set
			{
				base.SetPropertyValue<string>("WatermarkCssClass", value);
			}
		}

		private const string StringWatermarkText = "WatermarkText";

		private const string StringWatermarkCssClass = "WatermarkCssClass";
	}
}
