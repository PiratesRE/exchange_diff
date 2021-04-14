using System;
using System.Web;

namespace Microsoft.Exchange.Common
{
	internal class ExWebHealthResponseWrapper : IExWebHealthResponseWrapper
	{
		internal ExWebHealthResponseWrapper(HttpResponse response)
		{
			this.response = response;
		}

		public int StatusCode
		{
			get
			{
				return this.response.StatusCode;
			}
			set
			{
				this.response.StatusCode = value;
			}
		}

		public void AddHeader(string name, string value)
		{
			this.response.AddHeader(name, value);
		}

		public string GetHeaderValue(string name)
		{
			return this.response.Headers[name];
		}

		private HttpResponse response;
	}
}
