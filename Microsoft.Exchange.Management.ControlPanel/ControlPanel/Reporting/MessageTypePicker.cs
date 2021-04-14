using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Data.Search.AqsParser;

namespace Microsoft.Exchange.Management.ControlPanel.Reporting
{
	[RequiredScript(typeof(CommonToolkitScripts))]
	[ClientScriptResource("MessageTypePicker", "Microsoft.Exchange.Management.ControlPanel.Client.Reporting.js")]
	public class MessageTypePicker : BaseForm
	{
		public MessageTypePicker()
		{
			base.HideFieldValidationAssistant = true;
			base.CommitButtonText = Strings.OkButtonText;
		}

		public string AllTypesRadioButtonID
		{
			get
			{
				return this.rbMessageTypeAll.ClientID;
			}
		}

		public string SpecificTypesRadioButtonID
		{
			get
			{
				return this.rbMessageTypeSpecific.ClientID;
			}
		}

		public string[] Value { get; set; }

		protected void Page_Load(object sender, EventArgs e)
		{
			if (base.Request["types"] == null)
			{
				throw new BadQueryParameterException("types");
			}
			try
			{
				StringArrayConverter stringArrayConverter = new StringArrayConverter();
				this.Value = (string[])stringArrayConverter.ConvertFrom(base.Request["types"]);
			}
			catch (NotSupportedException innerException)
			{
				throw new BadQueryParameterException("types", innerException);
			}
			try
			{
				foreach (string value2 in this.Value)
				{
					Enum.Parse(typeof(KindKeyword), value2, true);
				}
			}
			catch (ArgumentException innerException2)
			{
				throw new BadQueryParameterException("types", innerException2);
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			this.typeEnumToControlMapping = new Dictionary<string, string>(Enum.GetValues(typeof(KindKeyword)).Length);
			this.typeEnumToControlMapping.Add(KindKeyword.email.ToString(), this.cbxMessageTypeEmail.ClientID);
			this.typeEnumToControlMapping.Add(KindKeyword.meetings.ToString(), this.cbxMessageTypeMeetings.ClientID);
			this.typeEnumToControlMapping.Add(KindKeyword.tasks.ToString(), this.cbxMessageTypeTasks.ClientID);
			this.typeEnumToControlMapping.Add(KindKeyword.notes.ToString(), this.cbxMessageTypeNotes.ClientID);
			this.typeEnumToControlMapping.Add(KindKeyword.docs.ToString(), this.cbxMessageTypeDocs.ClientID);
			this.typeEnumToControlMapping.Add(KindKeyword.journals.ToString(), this.cbxMessageTypeJournal.ClientID);
			this.typeEnumToControlMapping.Add(KindKeyword.contacts.ToString(), this.cbxMessageTypeContacts.ClientID);
			this.typeEnumToControlMapping.Add(KindKeyword.im.ToString(), this.cbxMessageTypeIMs.ClientID);
			if (this.Value == null || this.Value.Length == 0)
			{
				this.rbMessageTypeAll.Checked = true;
				return;
			}
			this.rbMessageTypeSpecific.Checked = true;
			int length = ((CommonMaster)base.Master).ContentPlaceHolder.ClientID.Length;
			foreach (string key in this.Value)
			{
				string text = this.typeEnumToControlMapping[key];
				CheckBox checkBox = (CheckBox)base.ContentPanel.FindControl(text.Substring(length + 1, text.Length - length - 1));
				if (checkBox != null)
				{
					checkBox.Checked = true;
				}
			}
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddElementProperty("AllTypesRadioButton", this.AllTypesRadioButtonID, true);
			descriptor.AddElementProperty("SpecificTypesRadioButton", this.SpecificTypesRadioButtonID, true);
			descriptor.AddProperty("value", this.Value);
			descriptor.AddProperty("TypeEnumToControlMapping", this.typeEnumToControlMapping);
		}

		private Dictionary<string, string> typeEnumToControlMapping;

		protected RadioButton rbMessageTypeAll;

		protected RadioButton rbMessageTypeSpecific;

		protected CheckBox cbxMessageTypeEmail;

		protected CheckBox cbxMessageTypeMeetings;

		protected CheckBox cbxMessageTypeTasks;

		protected CheckBox cbxMessageTypeNotes;

		protected CheckBox cbxMessageTypeDocs;

		protected CheckBox cbxMessageTypeJournal;

		protected CheckBox cbxMessageTypeContacts;

		protected CheckBox cbxMessageTypeIMs;

		protected CheckBox cbxMessageTypeVoiceMail;

		protected CheckBox cbxMessageTypeFaxes;

		protected CheckBox cbxMessageTypePosts;

		protected CheckBox cbxMessageTypeRssFeeds;
	}
}
