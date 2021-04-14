using System;

namespace Microsoft.Exchange.Common
{
	public interface IExWebHealthResponseWrapper
	{
		int StatusCode { get; set; }

		void AddHeader(string name, string value);

		string GetHeaderValue(string name);
	}
}
