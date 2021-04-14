using System;
using System.Text;
using System.Xml;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.TransportSync
{
	public class SlaDiagnosticInfo
	{
		internal SlaDiagnosticInfo(XmlReader xmlReader)
		{
			if (xmlReader.Name != "DatabaseQueueManager")
			{
				throw new ArgumentException("Invalid Xml Node was passed in", "DatabaseQueueManager");
			}
			while (xmlReader.Read())
			{
				string name = xmlReader.Name;
				string a;
				if ((a = name) != null)
				{
					if (!(a == "databaseId"))
					{
						if (!(a == "workType"))
						{
							if (!(a == "nextPollingTime"))
							{
								if (!(a == "itemsOutOfSla"))
								{
									if (a == "itemsOutOfSlaPercent")
									{
										this.ItemsOutOfSlaPercent = new int?(int.Parse(xmlReader.ReadString()));
									}
								}
								else
								{
									this.ItemsOutOfSla = new int?(int.Parse(xmlReader.ReadString()));
								}
							}
							else
							{
								string text = xmlReader.ReadString();
								if (!string.IsNullOrEmpty(text))
								{
									this.NextPollingTime = ExDateTime.Parse(text);
								}
							}
						}
						else
						{
							this.WorkType = xmlReader.ReadString();
						}
					}
					else
					{
						this.MdbGuid = new Guid(xmlReader.ReadString());
					}
				}
				if (this.WorkType == DiagnosticInfoConstants.WorkTypeName && name == "PollingQueue")
				{
					if (ExDateTime.UtcNow.AddMinutes(-5.0) > this.NextPollingTime && !this.NextPollingTime.Equals(ExDateTime.MinValue))
					{
						this.OutOfSlaTime = ExDateTime.UtcNow.Subtract(this.NextPollingTime);
						this.IsOutOfSla = true;
						return;
					}
					break;
				}
				else if (name == "DatabaseQueueManager")
				{
					return;
				}
			}
		}

		public Guid MdbGuid { get; private set; }

		public bool IsOutOfSla { get; private set; }

		public int? ItemsOutOfSla { get; private set; }

		public int? ItemsOutOfSlaPercent { get; private set; }

		public ExDateTime NextPollingTime { get; private set; }

		public TimeSpan OutOfSlaTime { get; private set; }

		public string WorkType { get; private set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{0}:{1}; ", "MDBGuid", this.MdbGuid);
			stringBuilder.AppendFormat("{0}:{1}; ", "WorkType", this.WorkType);
			stringBuilder.AppendFormat("{0}:{1}; ", "IsOutOfSla", this.IsOutOfSla);
			stringBuilder.AppendFormat("{0}:{1}; ", "OutOfSlaTime", this.OutOfSlaTime);
			stringBuilder.AppendFormat("{0}:{1}; ", "ItemsOutOfSla", this.ItemsOutOfSla);
			stringBuilder.AppendFormat("{0}:{1}; ", "ItemsOutOfSlaPercent", this.ItemsOutOfSlaPercent);
			return stringBuilder.ToString();
		}

		private const int DefaultOutOfSlaBufferInMinutes = -5;
	}
}
