using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class UserToCallsMap
	{
		internal int GetPhoneCallCount(string smtpAddress)
		{
			IList<Guid> list = null;
			lock (this.map)
			{
				if (!this.map.TryGetValue(smtpAddress, out list))
				{
					return 0;
				}
			}
			return list.Count;
		}

		internal void AddPhoneCall(string smtpAddress, Guid call)
		{
			lock (this.map)
			{
				IList<Guid> list = null;
				if (!this.map.TryGetValue(smtpAddress, out list))
				{
					list = new List<Guid>();
					this.map.Add(smtpAddress, list);
				}
				list.Add(call);
			}
		}

		internal void RemovePhoneCall(string smtpAddress, Guid call)
		{
			if (smtpAddress == null)
			{
				return;
			}
			lock (this.map)
			{
				IList<Guid> list = null;
				if (this.map.TryGetValue(smtpAddress, out list))
				{
					if (list.Count == 1)
					{
						this.map.Remove(smtpAddress);
					}
					else
					{
						list.Remove(call);
					}
				}
			}
		}

		private Dictionary<string, IList<Guid>> map = new Dictionary<string, IList<Guid>>();
	}
}
