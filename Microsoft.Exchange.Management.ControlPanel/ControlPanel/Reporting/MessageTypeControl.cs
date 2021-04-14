using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Data.Search.AqsParser;

namespace Microsoft.Exchange.Management.ControlPanel.Reporting
{
	[ClientScriptResource("MessageTypeControl", "Microsoft.Exchange.Management.ControlPanel.Client.Reporting.js")]
	public class MessageTypeControl : WebControl, IScriptControl, INamingContainer
	{
		public MessageTypeControl()
		{
			this.locStringMapping = new Dictionary<string, string>();
			this.locStringMapping.Add(KindKeyword.email.ToString(), Strings.MessageTypeEmail);
			this.locStringMapping.Add(KindKeyword.meetings.ToString(), Strings.MessageTypeMeetings);
			this.locStringMapping.Add(KindKeyword.tasks.ToString(), Strings.MessageTypeTasks);
			this.locStringMapping.Add(KindKeyword.notes.ToString(), Strings.MessageTypeNotes);
			this.locStringMapping.Add(KindKeyword.docs.ToString(), Strings.MessageTypeDocs);
			this.locStringMapping.Add(KindKeyword.journals.ToString(), Strings.MessageTypeJournal);
			this.locStringMapping.Add(KindKeyword.contacts.ToString(), Strings.MessageTypeContacts);
			this.locStringMapping.Add(KindKeyword.im.ToString(), Strings.MessageTypeInstantMessage);
			this.locStringMapping.Add(KindKeyword.voicemail.ToString(), Strings.MessageTypeVoiceMail);
			this.locStringMapping.Add(KindKeyword.posts.ToString(), Strings.MessageTypePosts);
			this.locStringMapping.Add(KindKeyword.rssfeeds.ToString(), Strings.MessageTypeRssFeeds);
			this.locStringMapping.Add(KindKeyword.faxes.ToString(), Strings.MessageTypeFaxes);
		}

		[TypeConverter(typeof(StringArrayConverter))]
		public string[] DefaultValue { get; set; }

		[TypeConverter(typeof(StringArrayConverter))]
		public string[] Value { get; set; }

		protected override HtmlTextWriterTag TagKey
		{
			get
			{
				return HtmlTextWriterTag.Div;
			}
		}

		IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
		{
			ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor("MessageTypeControl", this.ClientID);
			scriptControlDescriptor.AddElementProperty("LaunchButton", this.btnLaunch.ClientID);
			scriptControlDescriptor.AddElementProperty("ValueLabel", this.lblValue.ClientID);
			scriptControlDescriptor.AddProperty("MessageTypePickerUrl", base.ResolveClientUrl("~/Reporting/MessageTypePicker.aspx"));
			scriptControlDescriptor.AddProperty("ValueArray", this.Value);
			scriptControlDescriptor.AddProperty("LocStringMapping", this.locStringMapping);
			return new ScriptDescriptor[]
			{
				scriptControlDescriptor
			};
		}

		IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
		{
			return ScriptObjectBuilder.GetScriptReferences(typeof(MessageTypeControl));
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.promptText = new Literal();
			this.promptText.ID = "promptText";
			this.promptText.Text = Strings.MessageTypesToSearch;
			this.lblValue = new Label();
			this.lblValue.ID = "lblValue";
			this.lblValue.CssClass = "messageTypePromptLabel";
			this.btnLaunch = new HtmlButton();
			this.btnLaunch.ID = "btnLaunch";
			this.btnLaunch.InnerText = Strings.MessageTypesToSearchButtonText;
			this.btnLaunch.CausesValidation = false;
			this.btnLaunch.Attributes["onClick"] = "javascript:return false;";
			this.btnLaunch.Attributes.Add("class", "messageTypeLaunchButton");
			Table table = new Table();
			table.CellPadding = 0;
			table.CellSpacing = 0;
			TableRow tableRow = new TableRow();
			TableCell tableCell = new TableCell();
			tableCell.Controls.Add(this.promptText);
			tableCell.Controls.Add(this.lblValue);
			tableRow.Cells.Add(tableCell);
			TableRow tableRow2 = new TableRow();
			TableCell tableCell2 = new TableCell();
			tableCell2.Controls.Add(this.btnLaunch);
			tableRow2.Cells.Add(tableCell2);
			table.Rows.Add(tableRow);
			table.Rows.Add(tableRow2);
			this.Controls.Add(table);
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (!this.Page.IsPostBack && this.DefaultValue != null && this.DefaultValue.Length > 0)
			{
				foreach (string value in this.DefaultValue)
				{
					Enum.Parse(typeof(KindKeyword), value, true);
				}
				this.Value = this.DefaultValue;
			}
			base.OnPreRender(e);
			if (!base.DesignMode)
			{
				ScriptManager.GetCurrent(this.Page).RegisterScriptControl<MessageTypeControl>(this);
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			this.btnLaunch.Attributes["class"] = (this.Enabled ? "messageTypeLaunchButton" : "messageTypeLaunchButtonDisabled");
			this.btnLaunch.Disabled = !this.Enabled;
			base.Render(writer);
			if (!base.DesignMode)
			{
				ScriptManager.GetCurrent(this.Page).RegisterScriptDescriptors(this);
			}
		}

		private const string MessageTypePickerUrl = "~/Reporting/MessageTypePicker.aspx";

		private Literal promptText;

		private Label lblValue;

		private HtmlButton btnLaunch;

		private Dictionary<string, string> locStringMapping;
	}
}
