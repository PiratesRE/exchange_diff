using System;
using System.Globalization;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	public class SmtpConnectivityStatus : ConfigurableObject
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return SmtpConnectivityStatus.schema;
			}
		}

		public SmtpConnectivityStatus(Server server, ReceiveConnector receiveConnector, IPBinding binding, IPEndPoint endPoint) : base(new SimpleProviderPropertyBag())
		{
			string text = binding.ToString();
			string text2 = endPoint.ToString();
			string identity = string.Format("{0}\\{1}\\{2}", new object[]
			{
				receiveConnector.Identity,
				text,
				text2,
				CultureInfo.CurrentUICulture
			});
			this[this.propertyBag.ObjectIdentityPropertyDefinition] = new ConfigObjectId(identity);
			this.Server = server.Name;
			this.ReceiveConnector = receiveConnector.Name;
			this.Binding = text;
			this.EndPoint = text2;
		}

		public string Server
		{
			get
			{
				return (string)this[SmtpConnectivityStatusSchema.Server];
			}
			private set
			{
				this[SmtpConnectivityStatusSchema.Server] = value;
			}
		}

		public string ReceiveConnector
		{
			get
			{
				return (string)this[SmtpConnectivityStatusSchema.ReceiveConnector];
			}
			private set
			{
				this[SmtpConnectivityStatusSchema.ReceiveConnector] = value;
			}
		}

		public string Binding
		{
			get
			{
				return (string)this[SmtpConnectivityStatusSchema.Binding];
			}
			private set
			{
				this[SmtpConnectivityStatusSchema.Binding] = value;
			}
		}

		public string EndPoint
		{
			get
			{
				return (string)this[SmtpConnectivityStatusSchema.EndPoint];
			}
			private set
			{
				this[SmtpConnectivityStatusSchema.EndPoint] = value;
			}
		}

		public SmtpConnectivityStatusCode StatusCode
		{
			get
			{
				return (SmtpConnectivityStatusCode)this[SmtpConnectivityStatusSchema.StatusCode];
			}
			internal set
			{
				this[SmtpConnectivityStatusSchema.StatusCode] = value;
			}
		}

		public string Details
		{
			get
			{
				return (string)this[SmtpConnectivityStatusSchema.Details];
			}
			internal set
			{
				this[SmtpConnectivityStatusSchema.Details] = value;
			}
		}

		private static SmtpConnectivityStatusSchema schema = ObjectSchema.GetInstance<SmtpConnectivityStatusSchema>();
	}
}
