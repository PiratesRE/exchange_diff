using System;
using System.IO;
using System.ServiceModel.Channels;
using System.Web;

namespace Microsoft.Exchange.Services.Wcf
{
	internal abstract class BaseResponseRenderer
	{
		internal abstract void Render(Message message, Stream stream);

		internal abstract void Render(Message message, Stream stream, HttpResponse response);
	}
}
