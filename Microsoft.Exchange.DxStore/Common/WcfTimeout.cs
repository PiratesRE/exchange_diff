using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.DxStore.Common
{
	[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
	public class WcfTimeout
	{
		[DataMember]
		public TimeSpan? Open { get; set; }

		[DataMember]
		public TimeSpan? Close { get; set; }

		[DataMember]
		public TimeSpan? Send { get; set; }

		[DataMember]
		public TimeSpan? Receive { get; set; }

		[DataMember]
		public TimeSpan? Operation { get; set; }

		public static TimeSpan? StringToTimeSpan(string tsStr, TimeSpan? defaultTs)
		{
			TimeSpan value;
			if (!TimeSpan.TryParse(tsStr, out value))
			{
				return defaultTs;
			}
			return new TimeSpan?(value);
		}

		public static WcfTimeout Parse(string wcfTimeoutStr, WcfTimeout defaultTimeout = null)
		{
			WcfTimeout wcfTimeout = (defaultTimeout != null) ? defaultTimeout.Clone() : new WcfTimeout();
			string[] array = wcfTimeoutStr.Split(new char[]
			{
				';'
			});
			foreach (string text in array)
			{
				string[] array3 = (from s in text.Split(new char[]
				{
					'='
				})
				select s.Trim()).ToArray<string>();
				if (array3.Length > 1)
				{
					string text2 = array3[0];
					string tsStr = array3[1];
					string a;
					if ((a = text2) != null)
					{
						if (!(a == "open"))
						{
							if (!(a == "close"))
							{
								if (!(a == "send"))
								{
									if (!(a == "receive"))
									{
										if (a == "operation")
										{
											wcfTimeout.Receive = WcfTimeout.StringToTimeSpan(tsStr, wcfTimeout.Receive);
										}
									}
									else
									{
										wcfTimeout.Receive = WcfTimeout.StringToTimeSpan(tsStr, wcfTimeout.Receive);
									}
								}
								else
								{
									wcfTimeout.Send = WcfTimeout.StringToTimeSpan(tsStr, wcfTimeout.Send);
								}
							}
							else
							{
								wcfTimeout.Close = WcfTimeout.StringToTimeSpan(tsStr, wcfTimeout.Close);
							}
						}
						else
						{
							wcfTimeout.Open = WcfTimeout.StringToTimeSpan(tsStr, wcfTimeout.Open);
						}
					}
				}
			}
			return wcfTimeout;
		}

		public WcfTimeout Clone()
		{
			return (WcfTimeout)base.MemberwiseClone();
		}
	}
}
