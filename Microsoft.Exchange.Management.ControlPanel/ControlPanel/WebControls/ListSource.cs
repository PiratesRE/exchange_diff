using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("ListSource", "Microsoft.Exchange.Management.ControlPanel.Client.List.js")]
	public class ListSource : DataBoundControl, IScriptControl
	{
		public string[] DataFields { get; set; }

		public string JsonData { get; protected set; }

		public ListSource()
		{
			base.RequiresDataBinding = true;
			this.JsonData = string.Empty;
		}

		protected override void PerformDataBinding(IEnumerable data)
		{
			base.PerformDataBinding(data);
			ArrayList arrayList = new ArrayList();
			PropertyDescriptorCollection listItemProperties = ListBindingHelper.GetListItemProperties(data);
			foreach (object obj in data)
			{
				if (this.DataFields != null && this.DataFields.Length == 1 && this.DataFields[0] == "ToString()")
				{
					arrayList.Add(obj.ToString());
				}
				else
				{
					Dictionary<string, object> dictionary = new Dictionary<string, object>();
					foreach (object obj2 in listItemProperties)
					{
						PropertyDescriptor propertyDescriptor = (PropertyDescriptor)obj2;
						if (this.DataFields == null || this.DataFields.Length <= 0 || this.DataFields.Contains(propertyDescriptor.Name))
						{
							object value = propertyDescriptor.GetValue(obj);
							if (value != null)
							{
								if (ListSource.knownTypes.Contains(value.GetType()))
								{
									dictionary[propertyDescriptor.Name] = value;
								}
								else
								{
									dictionary[propertyDescriptor.Name] = value.ToString();
								}
							}
							else
							{
								dictionary[propertyDescriptor.Name] = null;
							}
						}
					}
					arrayList.Add(dictionary);
				}
			}
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			this.JsonData = javaScriptSerializer.Serialize(arrayList);
		}

		protected override void AddAttributesToRender(HtmlTextWriter writer)
		{
			base.AddAttributesToRender(writer);
			writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);
			writer.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
			writer.AddAttribute(HtmlTextWriterAttribute.Value, this.JsonData);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (this.Page.IsPostBack)
			{
				this.JsonData = this.Context.Request[this.UniqueID];
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			if (base.RequiresDataBinding && string.IsNullOrEmpty(this.DataSourceID) && this.DataSource != null)
			{
				this.DataBind();
			}
			ScriptManager.GetCurrent(this.Page).RegisterScriptControl<ListSource>(this);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			base.Render(writer);
			if (!base.DesignMode)
			{
				ScriptManager.GetCurrent(this.Page).RegisterScriptDescriptors(this);
			}
		}

		protected override HtmlTextWriterTag TagKey
		{
			get
			{
				return HtmlTextWriterTag.Input;
			}
		}

		IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
		{
			return this.GetScriptDescriptors();
		}

		protected virtual IEnumerable<ScriptDescriptor> GetScriptDescriptors()
		{
			ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor(base.GetType().Name, this.ClientID);
			return new ScriptDescriptor[]
			{
				scriptControlDescriptor
			};
		}

		IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
		{
			return ScriptObjectBuilder.GetScriptReferences(base.GetType());
		}

		private static Type[] knownTypes = new Type[]
		{
			typeof(bool),
			typeof(char),
			typeof(int),
			typeof(float),
			typeof(string),
			typeof(Guid),
			typeof(DateTime),
			typeof(Identity)
		};
	}
}
