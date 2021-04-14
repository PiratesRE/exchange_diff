using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.Components.Common.Probes
{
	public class FilteredGenericEventLogProbe : GenericEventLogProbe
	{
		static FilteredGenericEventLogProbe()
		{
			Configuration configuration = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
			FilteredEventSection section = (FilteredEventSection)configuration.GetSection("filteredEvents");
			IEnumerable<string> collection = from key in section.ControlPanelRedEvent4.AllKeys
			select section.ControlPanelRedEvent4[key].Value;
			FilteredGenericEventLogProbe.filteredEventHashKeys = new HashSet<string>(collection);
		}

		protected override void OnRedEvent(CentralEventLogWatcher.EventRecordMini redEvent)
		{
			string empty = string.Empty;
			string redEventString = redEvent.CustomizedProperties[2];
			FilteredGenericEventLogProbe.RedEventException exception = new FilteredGenericEventLogProbe.RedEventException(redEventString);
			if (WatsonExceptionReport.TryStringHashFromStackTrace(exception, false, out empty))
			{
				if (FilteredGenericEventLogProbe.filteredEventHashKeys.Contains(empty))
				{
					return;
				}
				redEvent.CustomizedProperties[0] = SuppressingPiiData.Redact(redEvent.CustomizedProperties[0]);
				base.Result.StateAttribute12 = string.Format("Exception StackTrace Hash Key: {0}", empty);
			}
			base.OnRedEvent(redEvent);
		}

		private const int StackTrace = 2;

		private const int UserInformation = 0;

		private static readonly HashSet<string> filteredEventHashKeys;

		private class RedEventException : Exception
		{
			public RedEventException(string redEventString)
			{
				this.stackTrace = redEventString;
			}

			public override string StackTrace
			{
				get
				{
					return this.stackTrace;
				}
			}

			private readonly string stackTrace = string.Empty;
		}
	}
}
