using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class FreeBusyViewOptions
	{
		public FreeBusyViewOptions()
		{
			this.Init();
		}

		public FreeBusyViewOptions(Duration timeWindow, int mergedFreeBusyIntervalInMinutes, FreeBusyViewType requestedView)
		{
			this.Init();
			this.timeWindow = timeWindow;
			this.mergedFreeBusyIntervalInMinutes = mergedFreeBusyIntervalInMinutes;
			this.requestedView = requestedView;
			this.Validate();
		}

		[DataMember]
		public Duration TimeWindow
		{
			get
			{
				return this.timeWindow;
			}
			set
			{
				this.timeWindow = value;
			}
		}

		[DataMember]
		public int MergedFreeBusyIntervalInMinutes
		{
			get
			{
				return this.mergedFreeBusyIntervalInMinutes;
			}
			set
			{
				this.mergedFreeBusyIntervalInMinutes = value;
			}
		}

		[IgnoreDataMember]
		public FreeBusyViewType RequestedView
		{
			get
			{
				return this.requestedView;
			}
			set
			{
				this.requestedView = value;
			}
		}

		[XmlIgnore]
		[DataMember(Name = "RequestedView")]
		public string RequestedViewString
		{
			get
			{
				return EnumUtil.ToString<FreeBusyViewType>(this.RequestedView);
			}
			set
			{
				this.RequestedView = EnumUtil.Parse<FreeBusyViewType>(value);
			}
		}

		internal static bool IsMerged(FreeBusyViewType viewType)
		{
			return (viewType & FreeBusyViewType.MergedOnly) > FreeBusyViewType.None;
		}

		internal static FreeBusyViewOptions CreateDefaultForMeetingSuggestions(Duration timeWindow)
		{
			return new FreeBusyViewOptions(timeWindow, 30, FreeBusyViewType.MergedOnly);
		}

		internal void Validate()
		{
			if (this.requestedView == FreeBusyViewType.None)
			{
				throw new InvalidFreeBusyViewTypeException();
			}
			FreeBusyViewOptions.IsMerged(this.requestedView);
			if (this.requestedView == FreeBusyViewType.None)
			{
				throw new ArgumentException("RequestedView can not be None", "RequestedView");
			}
			if (this.mergedFreeBusyIntervalInMinutes < 5 || this.mergedFreeBusyIntervalInMinutes > 1440)
			{
				throw new InvalidMergedFreeBusyIntervalException(5, 1440);
			}
			if (this.timeWindow == null)
			{
				throw new MissingArgumentException(Strings.descMissingArgument("FreeBusyViewOptions.TimeWindow"));
			}
			this.timeWindow.Validate("FreeBusyViewOptions.TimeWindow");
			TimeSpan timeSpan = this.timeWindow.EndTime.Subtract(this.timeWindow.StartTime);
			if (timeSpan.Days > Configuration.MaximumQueryIntervalDays)
			{
				throw new TimeIntervalTooBigException("FreeBusyViewOptions.TimeWindow", Configuration.MaximumQueryIntervalDays, timeSpan.Days);
			}
			if (timeSpan.TotalMinutes < (double)this.mergedFreeBusyIntervalInMinutes)
			{
				throw new InvalidMergedFreeBusyIntervalException(5, 1440);
			}
		}

		[OnDeserializing]
		private void Init(StreamingContext context)
		{
			this.Init();
		}

		private void Init()
		{
			this.mergedFreeBusyIntervalInMinutes = 30;
			this.requestedView = FreeBusyViewType.FreeBusy;
		}

		internal const int MergedFreeBusyFlag = 1;

		private Duration timeWindow;

		private int mergedFreeBusyIntervalInMinutes;

		private FreeBusyViewType requestedView;
	}
}
