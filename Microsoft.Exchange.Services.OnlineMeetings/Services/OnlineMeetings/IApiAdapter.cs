using System;
using System.Threading.Tasks;

namespace Microsoft.Exchange.Services.OnlineMeetings
{
	internal interface IApiAdapter
	{
		Task<Uri> FindTokenAsync(string token);

		Task<TResponse> SendRequestToTokenAsync<TResponse>(string token, string method, object request = null) where TResponse : class;

		Task<TResponse> SendRequestAsync<TResponse>(Uri uri, string method, object request = null) where TResponse : class;
	}
}
