using System;
using System.Collections.Generic;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ClientAccess
{
	internal class CafeExtraDetailsParser : ExtraDetailsEscalateResponder.IProbeMonitorResultParser
	{
		public string GetHelpUrlForError(string errorStr)
		{
			if (CafeExtraDetailsParser.oneNoteLinks.ContainsKey(errorStr))
			{
				return CafeExtraDetailsParser.oneNoteLinks[errorStr];
			}
			return "http://aka.ms/cafehelp";
		}

		public string GetImpactedServerFqdn(ProbeResult probeResult, MonitorResult monitorResult)
		{
			return DirectoryGeneralUtils.GetLocalFQDN();
		}

		public string GetUsername(ProbeResult probeResult, MonitorResult monitorResult)
		{
			if (!string.IsNullOrEmpty(probeResult.StateAttribute2))
			{
				return (string)probeResult.StateAttribute2.Split(new char[]
				{
					' '
				}).GetValue(0);
			}
			return "<unset>";
		}

		public string GetPassword(ProbeResult probeResult, MonitorResult monitorResult)
		{
			if (!string.IsNullOrEmpty(probeResult.StateAttribute2))
			{
				return (string)probeResult.StateAttribute2.Split(new char[]
				{
					' '
				}).GetValue(1);
			}
			return "<unset>";
		}

		public string GetRequestId(ProbeResult probeResult, MonitorResult monitorResult)
		{
			if (!string.IsNullOrEmpty(probeResult.StateAttribute1))
			{
				return probeResult.StateAttribute1;
			}
			return "<unset>";
		}

		private const string CafeHelpUrl = "http://aka.ms/cafehelp";

		private static readonly Dictionary<string, string> oneNoteLinks = new Dictionary<string, string>
		{
			{
				"The remote server returned an error: (401) Unauthorized.",
				"https://msft.spoppe.com/teams/EXOos/_layouts/15/WopiFrame.aspx?sourcedoc=%2Fteams%2FEXOos%2FShared%20Documents%2FCafe%20Alert%20Troubleshooting&action=edit&wd=target%28%2FErrors.one%7C766eadef-134a-4cde-aa6b-98b2f6a5b893%2F%28401%5C%29%20Unauthorized.%7Cb3c2c384-ce80-43bc-b8b6-5b7f4cbbd0b9%2F%29"
			},
			{
				"The remote server returned an error: (403) Forbidden.",
				"https://msft.spoppe.com/teams/EXOos/_layouts/15/WopiFrame.aspx?sourcedoc=%2Fteams%2FEXOos%2FShared%20Documents%2FCafe%20Alert%20Troubleshooting&action=edit&wd=target%28%2FErrors.one%7C766eadef-134a-4cde-aa6b-98b2f6a5b893%2F%28403%5C%29%20Forbidden.%7C5565eb64-cbc7-4aea-8cd0-f8641778f937%2F%29"
			},
			{
				"The remote server returned an error: (404) Not Found.",
				"https://msft.spoppe.com/teams/EXOos/_layouts/15/WopiFrame.aspx?sourcedoc=%2Fteams%2FEXOos%2FShared%20Documents%2FCafe%20Alert%20Troubleshooting&action=edit&wd=target%28%2FErrors.one%7C766eadef-134a-4cde-aa6b-98b2f6a5b893%2F%28404%5C%29%20Not%20Found.%7Ceb8373ea-5a1b-489e-a3ea-9f7f749117ba%2F%29"
			},
			{
				"The remote server returned an error: (500) Internal Server Error.",
				"https://msft.spoppe.com/teams/EXOos/_layouts/15/WopiFrame.aspx?sourcedoc=%2Fteams%2FEXOos%2FShared%20Documents%2FCafe%20Alert%20Troubleshooting&action=edit&wd=target%28%2FErrors.one%7C766eadef-134a-4cde-aa6b-98b2f6a5b893%2F%28500%5C%29%20Internal%20Server%20Error.%7C74ac56c4-6b5f-4779-8c42-85080a0db95b%2F%29"
			},
			{
				"The remote server returned an error: (503) Server Unavailable.",
				"https://msft.spoppe.com/teams/EXOos/_layouts/15/WopiFrame.aspx?sourcedoc=%2Fteams%2FEXOos%2FShared%20Documents%2FCafe%20Alert%20Troubleshooting&action=edit&wd=target%28%2FErrors.one%7C766eadef-134a-4cde-aa6b-98b2f6a5b893%2F%28503%5C%29%20Server%20Unavailable.%7Ccfc37412-ef87-43d4-84bd-65f62414741e%2F%29"
			}
		};
	}
}
