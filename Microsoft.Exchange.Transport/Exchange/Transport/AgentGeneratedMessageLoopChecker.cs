using System;
using Microsoft.Exchange.Data.Mime;

namespace Microsoft.Exchange.Transport
{
	internal class AgentGeneratedMessageLoopChecker
	{
		public AgentGeneratedMessageLoopChecker(AgentGeneratedMessageLoopCheckerConfig config)
		{
			this.config = config;
		}

		public bool IsEnabledInSubmission()
		{
			return this.config.GetIsEnabledInSubmission();
		}

		public bool IsEnabledInSmtp()
		{
			return this.config.GetIsEnabledInSmtp();
		}

		public bool CheckAndStampInSubmission(HeaderList originalMailItemHeaders, HeaderList sideEffectMailItemHeaders, string agentName)
		{
			bool flag = this.CheckAndStampInSubmission(originalMailItemHeaders, sideEffectMailItemHeaders, agentName, "X-MS-Exchange-Organization-Generated-Message-Source", true, true);
			bool flag2 = this.CheckAndStampInSubmission(originalMailItemHeaders, sideEffectMailItemHeaders, agentName, "X-MS-Exchange-Generated-Message-Source", false, false);
			return flag || flag2;
		}

		private bool CheckAndStampInSubmission(HeaderList originalMailItemHeaders, HeaderList sideEffectMailItemHeaders, string agentName, string headerName, bool isOrgHeader, bool stampDiagnosticHeaders)
		{
			bool result = false;
			agentName = agentName.Replace(',', '_');
			Header header = originalMailItemHeaders.FindFirst(headerName);
			string[] array = (header == null) ? new string[0] : header.Value.Split(new char[]
			{
				','
			});
			if ((long)array.Length >= (long)((ulong)this.config.GetMaxAllowedMessageDepth()))
			{
				result = true;
			}
			else if (isOrgHeader)
			{
				int num = 0;
				foreach (string a in array)
				{
					if (string.Equals(a, agentName, StringComparison.InvariantCultureIgnoreCase))
					{
						num++;
					}
				}
				if ((long)num >= (long)((ulong)this.config.GetMaxAllowedMessageDepthPerAgent()))
				{
					result = true;
				}
			}
			string value = (header == null) ? agentName : (header.Value + ',' + agentName);
			sideEffectMailItemHeaders.AppendChild(new TextHeader(headerName, value));
			if (stampDiagnosticHeaders)
			{
				Header header2 = originalMailItemHeaders.FindFirst(HeaderId.MessageId);
				if (header2 != null)
				{
					sideEffectMailItemHeaders.AppendChild(new AsciiTextHeader("X-MS-Exchange-Parent-Message-Id", header2.Value));
				}
				Header header3 = originalMailItemHeaders.FindFirst("X-MS-Exchange-Message-Is-Ndr");
				if (header3 != null)
				{
					sideEffectMailItemHeaders.AppendChild(new AsciiTextHeader("X-MS-Exchange-Message-Is-Ndr", header3.Value));
				}
				sideEffectMailItemHeaders.AppendChild(new AsciiTextHeader("Auto-Submitted", "auto-generated"));
			}
			return result;
		}

		public bool CheckInSmtp(HeaderList originalMailItemHeaders)
		{
			bool result = false;
			Header header = originalMailItemHeaders.FindFirst("X-MS-Exchange-Organization-Generated-Message-Source");
			Header header2 = originalMailItemHeaders.FindFirst("X-MS-Exchange-Generated-Message-Source");
			if (header == null && header2 != null)
			{
				string[] array = header2.Value.Split(new char[]
				{
					','
				});
				if ((long)array.Length >= (long)((ulong)this.config.GetMaxAllowedMessageDepth()))
				{
					result = true;
				}
			}
			return result;
		}

		private const char AgentNameSeparator = ',';

		private const char AgentNameSeparatorReplacement = '_';

		private readonly AgentGeneratedMessageLoopCheckerConfig config;
	}
}
