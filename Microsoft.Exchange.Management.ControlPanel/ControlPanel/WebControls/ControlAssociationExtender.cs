using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[RequiredScript(typeof(CommonToolkitScripts))]
	[TargetControlType(typeof(Control))]
	[ClientScriptResource("ControlAssociationBehavior", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	public class ControlAssociationExtender : ExtenderControlBase
	{
		public ControlAssociationExtender()
		{
			this.Trigger = "click";
		}

		public string SectionID { get; set; }

		public bool InvokeOnInit { get; set; }

		public bool BindToControl { get; set; }

		public string Trigger { get; set; }

		public string AssociatedControlId { get; set; }

		public string InvokedMethod { get; set; }

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			if (this.InvokeOnInit)
			{
				descriptor.AddProperty("InvokeOnInit", true);
			}
			if (this.BindToControl)
			{
				descriptor.AddProperty("BindToControl", true);
			}
			if (this.Trigger != "click")
			{
				descriptor.AddProperty("Trigger", this.Trigger);
			}
			descriptor.AddElementProperty("AssociatedControl", this.AssociatedControlId, this);
		}

		protected override void OnLoad(EventArgs e)
		{
			if (!this.SectionID.IsNullOrBlank())
			{
				Control control = this;
				while (control != null)
				{
					if (control is Section)
					{
						Section section = control.Parent.FindControl(this.SectionID) as Section;
						if (section == null)
						{
							break;
						}
						if (this.AssociatedControlId.IsNullOrBlank())
						{
							this.AssociatedControlId = section.ClientID;
							break;
						}
						this.AssociatedControlId = section.FindControl(this.AssociatedControlId).ClientID;
						break;
					}
					else
					{
						control = control.NamingContainer;
					}
				}
			}
			if (this.AssociatedControlId.IsNullOrBlank())
			{
				throw new NotSupportedException("Associated Control cannot be found.");
			}
		}

		protected override IEnumerable<ScriptDescriptor> GetScriptDescriptors(Control targetControl)
		{
			IEnumerable<ScriptDescriptor> scriptDescriptors = base.GetScriptDescriptors(targetControl);
			ScriptBehaviorDescriptor scriptBehaviorDescriptor = scriptDescriptors.First<ScriptDescriptor>() as ScriptBehaviorDescriptor;
			scriptBehaviorDescriptor.AddScriptProperty("InvokedMethod", this.InvokedMethod);
			if (base.TargetControl is RadioButtonList)
			{
				scriptBehaviorDescriptor.AddProperty("IsRadioBtnList", true);
			}
			return scriptDescriptors;
		}
	}
}
