using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class OwaQueryStringParameters : QueryStringParameters
	{
		public ApplicationElement ApplicationElement
		{
			get
			{
				return (ApplicationElement)FormsRegistry.ApplicationElementParser.Parse(base.GetValue("ae"));
			}
			set
			{
				base["ae"] = value.ToString();
			}
		}

		public void SetApplicationElement(string value)
		{
			base["ae"] = value;
		}

		public string ItemClass
		{
			get
			{
				return base.GetValue("t");
			}
			set
			{
				base["t"] = value;
			}
		}

		public string Action
		{
			get
			{
				return base.GetValue("a");
			}
			set
			{
				base["a"] = value;
			}
		}

		public string State
		{
			get
			{
				return base.GetValue("s");
			}
			set
			{
				base["s"] = value;
			}
		}

		public string Id
		{
			get
			{
				return base.GetValue("id");
			}
			set
			{
				base["id"] = value;
			}
		}

		private const string ApplicationElementParameter = "ae";

		private const string ItemClassParameter = "t";

		private const string ActionParameter = "a";

		private const string StateParameter = "s";

		private const string IdParameter = "id";
	}
}
