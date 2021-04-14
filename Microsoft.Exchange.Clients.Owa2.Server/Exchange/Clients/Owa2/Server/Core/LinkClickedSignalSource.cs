using System;
using System.Collections.Generic;
using Microsoft.Exchange.SharePointSignalStore;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class LinkClickedSignalSource : IAnalyticsSignalSource
	{
		public LinkClickedSignalSource(string sender, string url, string title, string desc, string imgurl, string imgdimensions, List<string> recipients)
		{
			this.sender = sender;
			this.url = url;
			this.title = string.Empty;
			this.desc = string.Empty;
			this.imgurl = string.Empty;
			this.imgwidth = string.Empty;
			this.imgheight = string.Empty;
			if (string.IsNullOrEmpty(sender))
			{
				throw new ArgumentException("Sender email address can not be null or empty.");
			}
			if (string.IsNullOrEmpty(url))
			{
				throw new ArgumentException("Url can not be null or empty.");
			}
			if (recipients == null || recipients.Count == 0)
			{
				throw new ArgumentException("Recipients can not be null or empty.");
			}
			if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(desc))
			{
				this.title = title;
				this.desc = desc;
				if (!string.IsNullOrEmpty(imgurl) && !string.IsNullOrEmpty(imgdimensions) && !imgdimensions.Equals("0x0", StringComparison.InvariantCulture))
				{
					this.imgurl = imgurl;
					string[] array = imgdimensions.Split(new char[]
					{
						'x'
					});
					this.imgwidth = array[0];
					this.imgheight = array[1];
				}
			}
			this.recipients = string.Join(";", recipients);
		}

		public IEnumerable<AnalyticsSignal> GetSignals()
		{
			Dictionary<string, string> properties = new Dictionary<string, string>
			{
				{
					"SharedBy",
					this.sender
				},
				{
					"Recipients",
					this.recipients
				},
				{
					"Context",
					"Click data from OWA"
				}
			};
			Dictionary<string, string> properties2 = new Dictionary<string, string>
			{
				{
					"Title",
					this.title
				},
				{
					"Description",
					this.desc
				},
				{
					"ImageUrl",
					this.imgurl
				},
				{
					"ImageWidth",
					this.imgwidth
				},
				{
					"ImageHeight",
					this.imgheight
				}
			};
			AnalyticsSignal.AnalyticsActor actor = new AnalyticsSignal.AnalyticsActor
			{
				Id = null,
				Properties = SharePointSignalRestDataProvider.CreateSignalProperties(null)
			};
			AnalyticsSignal.AnalyticsAction action = new AnalyticsSignal.AnalyticsAction
			{
				ActionType = "SharedAndClicked",
				Properties = SharePointSignalRestDataProvider.CreateSignalProperties(properties)
			};
			AnalyticsSignal.AnalyticsItem item = new AnalyticsSignal.AnalyticsItem
			{
				Id = this.url,
				Properties = SharePointSignalRestDataProvider.CreateSignalProperties(properties2)
			};
			AnalyticsSignal item2 = new AnalyticsSignal
			{
				Actor = actor,
				Action = action,
				Item = item,
				Source = this.GetSourceName()
			};
			return new List<AnalyticsSignal>
			{
				item2
			};
		}

		public string GetSourceName()
		{
			return "OWA";
		}

		private readonly string sender;

		private readonly string url;

		private readonly string title;

		private readonly string desc;

		private readonly string imgurl;

		private readonly string imgwidth;

		private readonly string imgheight;

		private readonly string recipients;
	}
}
