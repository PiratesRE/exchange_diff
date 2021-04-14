using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public sealed class PreFormActionResponse
	{
		public PreFormActionResponse()
		{
		}

		public PreFormActionResponse(HttpRequest request, params string[] parameters)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			if (parameters == null || parameters.Length == 0)
			{
				throw new ArgumentException("parameters may not be null or empty array");
			}
			foreach (string name in parameters)
			{
				string queryStringParameter = Utilities.GetQueryStringParameter(request, name, false);
				if (queryStringParameter != null)
				{
					this.AddParameter(name, queryStringParameter);
				}
			}
		}

		public ApplicationElement ApplicationElement
		{
			get
			{
				return this.applicationElement;
			}
			set
			{
				this.applicationElement = value;
			}
		}

		public string Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		public string State
		{
			get
			{
				return this.state;
			}
			set
			{
				this.state = value;
			}
		}

		public string Action
		{
			get
			{
				return this.action;
			}
			set
			{
				this.action = value;
			}
		}

		public string GetUrl()
		{
			return this.GetUrl(true);
		}

		public string GetUrl(bool needApplicationElement)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (needApplicationElement && this.applicationElement != ApplicationElement.NotSet)
			{
				PreFormActionResponse.AppendUrlParameter("ae", FormsRegistry.ApplicationElementParser.GetString((int)this.applicationElement), stringBuilder);
			}
			PreFormActionResponse.AppendUrlParameter("t", this.type, stringBuilder);
			PreFormActionResponse.AppendUrlParameter("a", this.action, stringBuilder);
			PreFormActionResponse.AppendUrlParameter("s", this.state, stringBuilder);
			if (this.parameters != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in this.parameters)
				{
					PreFormActionResponse.AppendUrlParameter(keyValuePair.Key, keyValuePair.Value, stringBuilder);
				}
			}
			return stringBuilder.ToString();
		}

		public void AddParameter(string name, string value)
		{
			if (this.parameters == null)
			{
				this.parameters = new Dictionary<string, string>();
			}
			this.parameters.Add(name, value);
		}

		private static void AppendUrlParameter(string name, string value, StringBuilder builder)
		{
			if (!string.IsNullOrEmpty(value))
			{
				if (builder.Length > 0)
				{
					builder.Append('&');
				}
				builder.Append(name);
				builder.Append('=');
				builder.Append(Utilities.UrlEncode(value));
			}
		}

		private ApplicationElement applicationElement;

		private string type;

		private string state;

		private string action;

		private Dictionary<string, string> parameters;
	}
}
