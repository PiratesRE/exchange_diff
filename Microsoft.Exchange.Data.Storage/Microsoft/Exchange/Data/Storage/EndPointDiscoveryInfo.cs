using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class EndPointDiscoveryInfo
	{
		internal EndPointDiscoveryInfo()
		{
			this.messages = new List<string>();
			this.Status = EndPointDiscoveryInfo.DiscoveryStatus.Success;
		}

		public EndPointDiscoveryInfo.DiscoveryStatus Status { get; private set; }

		public string Message
		{
			get
			{
				return string.Join(" ", this.messages);
			}
		}

		internal void AddInfo(EndPointDiscoveryInfo.DiscoveryStatus status, string message)
		{
			this.Status = status;
			this.messages.Add(message);
		}

		private List<string> messages;

		public enum DiscoveryStatus
		{
			Success,
			Error,
			IocNotFound,
			IocNoUri,
			IocException,
			OrNotFound,
			OrNoUri
		}
	}
}
