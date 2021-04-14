using System;
using System.ComponentModel;
using System.Web.Configuration;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[TargetControlType(typeof(Control))]
	[ClientScriptResource("FVASettings", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	public class FieldValidationAssistantExtender : ExtenderControlBase
	{
		public static Func<string, string, string> HelpUrlBuilder { get; set; } = new Func<string, string, string>(HelpUtil.BuildFVAEhcHref);

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			if (this.DebugMode)
			{
				descriptor.AddProperty("DebugMode", true);
			}
			descriptor.AddProperty("LocStringsResource", this.LocStringsResource, true);
			descriptor.AddProperty("Canvas", this.Canvas, true);
			descriptor.AddProperty("IndentCssClass", this.IndentCssClass, true);
			descriptor.AddProperty("HelpLinkPrefix", this.HelpLinkPrefix, true);
		}

		[Browsable(false)]
		public bool DebugMode
		{
			get
			{
				return FieldValidationAssistantExtender.compilationSection.Debug;
			}
		}

		[Browsable(false)]
		public string LocStringsResource
		{
			get
			{
				return this.locStringsResource;
			}
			set
			{
				this.locStringsResource = value;
			}
		}

		[Browsable(false)]
		public string Canvas
		{
			get
			{
				return this.canvas;
			}
			set
			{
				this.canvas = value;
			}
		}

		[Browsable(false)]
		public string IndentCssClass
		{
			get
			{
				return this.indentCssClass;
			}
			set
			{
				this.indentCssClass = value;
			}
		}

		[Browsable(false)]
		public string HelpLinkPrefix
		{
			get
			{
				return this.helpLinkPrefix;
			}
			private set
			{
				this.helpLinkPrefix = value;
			}
		}

		[DefaultValue(EACHelpId.Default)]
		[Browsable(false)]
		public string HelpId
		{
			get
			{
				return this.helpId;
			}
			set
			{
				this.helpId = value;
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			this.HelpLinkPrefix = FieldValidationAssistantExtender.HelpUrlBuilder(this.HelpId, "{0}");
		}

		private static CompilationSection compilationSection = (CompilationSection)WebConfigurationManager.GetSection("system.web/compilation");

		private string locStringsResource;

		private string canvas;

		private string indentCssClass;

		private string helpLinkPrefix;

		private string helpId = EACHelpId.Default.ToString();
	}
}
