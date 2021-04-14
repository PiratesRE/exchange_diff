using System;
using System.IO;
using System.Net;
using System.ServiceModel.Channels;
using System.Text;
using System.Web;
using System.Xml;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class SoapWcfResponseRenderer : BaseResponseRenderer
	{
		internal HttpStatusCode? StatusCode { get; private set; }

		public static SoapWcfResponseRenderer Create(HttpStatusCode statusCode)
		{
			return new SoapWcfResponseRenderer(new HttpStatusCode?(statusCode));
		}

		private SoapWcfResponseRenderer(HttpStatusCode? statusCode)
		{
			this.StatusCode = statusCode;
		}

		internal override void Render(Message message, Stream stream)
		{
			XmlDictionaryWriter xmlDictionaryWriter = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8, false);
			if (this.StatusCode != null && HttpContext.Current != null)
			{
				HttpContext.Current.Response.StatusCode = (int)this.StatusCode.Value;
			}
			EWSSettings.WritingToWire = true;
			xmlDictionaryWriter.WriteStartDocument();
			message.WriteMessage(xmlDictionaryWriter);
			xmlDictionaryWriter.WriteEndDocument();
			xmlDictionaryWriter.Flush();
		}

		internal override void Render(Message message, Stream stream, HttpResponse response)
		{
			this.Render(message, stream);
		}

		public static SoapWcfResponseRenderer Singleton
		{
			get
			{
				return SoapWcfResponseRenderer.singleton;
			}
		}

		private static SoapWcfResponseRenderer singleton = new SoapWcfResponseRenderer(null);
	}
}
