using System;
using System.Net;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class IPBinding : IPEndPoint
	{
		public IPBinding(string expression) : base(IPAddress.Any, 0)
		{
			this.ParseAndValidateIPBinding(expression);
		}

		public IPBinding() : base(IPAddress.None, 0)
		{
		}

		public static IPBinding Parse(string ipRange)
		{
			return new IPBinding(ipRange);
		}

		public IPBinding(IPAddress address, int port) : base(address, port)
		{
		}

		public new int Port
		{
			get
			{
				return base.Port;
			}
			set
			{
				if (value < 0 || value > 65535)
				{
					throw new FormatException(DataStrings.ExceptionEndPointPortOutOfRange(0, 65535, value));
				}
				base.Port = value;
			}
		}

		private void ParseAndValidateIPBinding(string ipBinding)
		{
			ipBinding = ipBinding.Trim();
			if (ipBinding.Length > 0 && ipBinding[ipBinding.Length - 1] == ':')
			{
				ipBinding = ipBinding.Substring(0, ipBinding.Length - 1);
			}
			int num = ipBinding.LastIndexOf(':');
			if (num == -1)
			{
				throw new FormatException(DataStrings.ExceptionEndPointMissingSeparator(ipBinding));
			}
			string text = ipBinding.Substring(0, num);
			IPAddress address;
			if (string.IsNullOrEmpty(text))
			{
				address = new IPAddress(0L);
			}
			else if (!IPAddress.TryParse(text, out address))
			{
				throw new FormatException(DataStrings.ExceptionEndPointInvalidIPAddress(ipBinding));
			}
			int port;
			if (!int.TryParse(ipBinding.Substring(num + 1), out port))
			{
				throw new FormatException(DataStrings.ExceptionEndPointInvalidPort(ipBinding));
			}
			this.Port = port;
			base.Address = address;
		}
	}
}
