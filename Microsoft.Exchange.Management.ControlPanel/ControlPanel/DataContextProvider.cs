using System;
using System.Collections.Generic;
using System.Web.UI;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.DDIService;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class DataContextProvider : ScriptControlBase
	{
		protected override void OnPreRender(EventArgs e)
		{
			if (!string.IsNullOrEmpty(this.ViewModel))
			{
				if (this.ServiceUrl != null)
				{
					this["ServiceUrl"] = EcpUrl.ProcessUrl(this.ServiceUrl.ServiceUrl);
				}
				AttributeCollection bindingContextAttributes = this.GetBindingContextAttributes();
				bindingContextAttributes.Add("data-type", this.ViewModel);
				foreach (KeyValuePair<string, object> keyValuePair in this.dataContextProperties)
				{
					object value = keyValuePair.Value;
					string value2;
					if (value is string)
					{
						value2 = (string)value;
					}
					else if (value is bool)
					{
						value2 = value.ToString().ToLower();
					}
					else if (value is int || value is uint || value is long || value is ulong || value is float || value is double)
					{
						value2 = value.ToString();
					}
					else
					{
						value2 = "json:" + keyValuePair.Value.ToJsonString(DDIService.KnownTypes.Value);
					}
					bindingContextAttributes.Add("vm-" + keyValuePair.Key, value2);
				}
				if (bindingContextAttributes != base.Attributes)
				{
					List<string> list = new List<string>(base.Attributes.Count);
					foreach (object obj in base.Attributes.Keys)
					{
						string text = obj as string;
						if (text != null && text.StartsWith("vm-"))
						{
							bindingContextAttributes.Add(text, base.Attributes[text]);
							list.Add(text);
						}
					}
					foreach (string key in list)
					{
						base.Attributes.Remove(key);
					}
				}
			}
			base.OnPreRender(e);
		}

		private AttributeCollection GetBindingContextAttributes()
		{
			if (this.Page is BaseForm)
			{
				return this.Page.Form.Attributes;
			}
			return base.Attributes;
		}

		public WebServiceReference ServiceUrl { get; set; }

		public string ViewModel { get; set; }

		protected object this[string propertyName]
		{
			get
			{
				object result = null;
				this.dataContextProperties.TryGetValue(propertyName, out result);
				return result;
			}
			set
			{
				this.dataContextProperties[propertyName] = value;
			}
		}

		private Dictionary<string, object> dataContextProperties = new Dictionary<string, object>();
	}
}
