using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("UMCallDataRecord", "Microsoft.Exchange.Management.ControlPanel.Client.UnifiedMessaging.js")]
	[RequiredScript(typeof(CommonToolkitScripts))]
	public class UMCallDataRecord : SlabControl, IScriptControl
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			BindingCollection filterParameters = this.listViewDataSource.FilterParameters;
			if (this.pickerUMMailboxToSearch != null)
			{
				filterParameters.Add(new ComponentBinding(this.pickerUMMailboxToSearch, "value")
				{
					Name = "Mailbox"
				});
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			Command command = this.listView.Commands.FindCommandByName("Refresh");
			command.Visible = false;
			ScriptManager current = ScriptManager.GetCurrent(this.Page);
			current.RegisterScriptControl<UMCallDataRecord>(this);
			base.EnsureID();
		}

		protected override void Render(HtmlTextWriter writer)
		{
			this.AddAttributesToRender(writer);
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			base.Render(writer);
			writer.RenderEndTag();
			if (!base.DesignMode)
			{
				ScriptManager.GetCurrent(this.Page).RegisterScriptDescriptors(this);
			}
		}

		protected void AddAttributesToRender(HtmlTextWriter writer)
		{
			if (this.ID != null)
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
			}
			writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "100%");
			foreach (object obj in base.Attributes.Keys)
			{
				string text = (string)obj;
				writer.AddAttribute(text, base.Attributes[text]);
			}
		}

		public virtual IEnumerable<ScriptReference> GetScriptReferences()
		{
			return ScriptObjectBuilder.GetScriptReferences(base.GetType());
		}

		public IEnumerable<ScriptDescriptor> GetScriptDescriptors()
		{
			ClientScriptResourceAttribute clientScriptResourceAttribute = (ClientScriptResourceAttribute)TypeDescriptor.GetAttributes(this)[typeof(ClientScriptResourceAttribute)];
			ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor(clientScriptResourceAttribute.ComponentType, this.ClientID);
			this.BuildScriptDescriptor(scriptControlDescriptor);
			return new ScriptDescriptor[]
			{
				scriptControlDescriptor
			};
		}

		private void BuildScriptDescriptor(ScriptControlDescriptor descriptor)
		{
			descriptor.AddComponentProperty("ListView", this.listView.ClientID, true);
			descriptor.AddComponentProperty("ListViewDataSource", this.listViewDataSource.ClientID, true);
			descriptor.AddComponentProperty("ListViewRefreshMethod", this.listViewDataSource.RefreshWebServiceMethod.ClientID, true);
			if (this.pickerUMMailboxToSearch != null)
			{
				descriptor.AddComponentProperty("UMMailboxPicker", this.pickerUMMailboxToSearch.ClientID, true);
			}
		}

		protected EcpSingleSelect pickerUMMailboxToSearch;

		protected WebServiceListSource listViewDataSource;

		protected ListView listView;
	}
}
