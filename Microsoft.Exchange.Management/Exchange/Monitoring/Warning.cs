using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Monitoring
{
	internal class Warning
	{
		internal Warning(string value)
		{
			this.warningStr = string.Format("[{1}] : {0}", value, ExDateTime.Now.ToString("HH:mm:ss.fff"));
		}

		internal string Message
		{
			get
			{
				return this.warningStr;
			}
		}

		public override string ToString()
		{
			return this.Message;
		}

		private readonly string warningStr;
	}
}
