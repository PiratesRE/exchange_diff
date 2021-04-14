using System;
using Microsoft.Exchange.InfoWorker.Common.MailTips;

namespace Microsoft.Exchange.Management.DDIService
{
	public class RenderingDisabledResolver : IRenderingDisabledResolver
	{
		private RenderingDisabledResolver()
		{
		}

		internal static IRenderingDisabledResolver Instance { get; set; } = new RenderingDisabledResolver();

		public bool value
		{
			get
			{
				return Utility.RenderingDisabled;
			}
		}
	}
}
