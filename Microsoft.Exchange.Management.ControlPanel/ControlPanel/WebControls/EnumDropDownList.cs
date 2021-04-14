using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class EnumDropDownList : DropDownList
	{
		public string EnumType { get; set; }

		public bool LocalizedText
		{
			get
			{
				return this.localizedText;
			}
			set
			{
				this.localizedText = value;
			}
		}

		public string AvailabeValues { get; set; }

		public string DefaultValue { get; set; }

		protected override void OnLoad(EventArgs e)
		{
			if (!string.IsNullOrEmpty(this.EnumType))
			{
				Type type = Type.GetType(this.EnumType);
				Enum.GetUnderlyingType(type);
				foreach (object obj in Enum.GetValues(type))
				{
					string value = obj.ToString();
					if (string.IsNullOrEmpty(this.AvailabeValues) || this.AvailabeValues.Split(new char[]
					{
						','
					}).Any((string each) => each.Equals(value, StringComparison.InvariantCultureIgnoreCase)))
					{
						string value2 = this.localizedText ? LocalizedDescriptionAttribute.FromEnum(type, obj) : obj.ToString();
						ListItem item = new ListItem(RtlUtil.ConvertToDecodedBidiString(value2, RtlUtil.IsRtl), value);
						this.Items.Add(item);
						if (this.DefaultValue != null && value == this.DefaultValue)
						{
							this.SelectedValue = value;
						}
					}
				}
			}
			base.OnLoad(e);
		}

		protected override void AddAttributesToRender(HtmlTextWriter writer)
		{
			writer.AddAttribute("role", "combobox");
			base.AddAttributesToRender(writer);
		}

		private bool localizedText = true;
	}
}
