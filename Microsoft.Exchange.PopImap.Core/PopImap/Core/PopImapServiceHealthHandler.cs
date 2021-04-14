using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PopImap.Core
{
	internal sealed class PopImapServiceHealthHandler : ExchangeDiagnosableWrapper<PopImapServiceHealth>
	{
		protected override string UsageText
		{
			get
			{
				return "This diagnostics handler returns info about PopImap health. The handler supports \"ShowError\" argument to return error details. Below are examples for using this diagnostics handler: ";
			}
		}

		protected override string UsageSample
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder("Example 1: Returns POp3 Service health without error details.");
				stringBuilder.AppendLine("Get-ExchangeDiagnosticInfo -Process Microsoft.Exchange.Pop3 -Component PopImapServiceHealth");
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("Example 2: Returns Pop3 Service health with error details.");
				stringBuilder.AppendLine("Get-ExchangeDiagnosticInfo -Process Microsoft.Exchange.Pop3 -Component PopImapServiceHealth -Argument ShowError");
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("Returns IMAP4 Service health without error details.");
				stringBuilder.AppendLine("Get-ExchangeDiagnosticInfo -Process Microsoft.Exchange.Imap4 -Component PopImapServiceHealth");
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("Example 2: Returns IMAP4 Service health with error details.");
				stringBuilder.AppendLine("Get-ExchangeDiagnosticInfo -Process Microsoft.Exchange.Imap4 -Component PopImapServiceHealth -Argument ShowError");
				return stringBuilder.ToString();
			}
		}

		public static PopImapServiceHealthHandler GetInstance()
		{
			if (PopImapServiceHealthHandler.instance == null)
			{
				lock (PopImapServiceHealthHandler.lockObject)
				{
					if (PopImapServiceHealthHandler.instance == null)
					{
						PopImapServiceHealthHandler.instance = new PopImapServiceHealthHandler();
					}
				}
			}
			return PopImapServiceHealthHandler.instance;
		}

		private PopImapServiceHealthHandler()
		{
		}

		protected override string ComponentName
		{
			get
			{
				return "PopImapServiceHealth";
			}
		}

		internal override PopImapServiceHealth GetExchangeDiagnosticsInfoData(DiagnosableParameters arguments)
		{
			PopImapServiceHealth serviceHealth = new PopImapServiceHealth();
			serviceHealth.ServerName = Environment.MachineName;
			if (PopImapRequestCache.Instance.Values.Count == 0)
			{
				return serviceHealth;
			}
			serviceHealth.AverageLdapLatency = PopImapRequestCache.Instance.Values.Average((PopImapRequestData request) => request.LdapLatency);
			serviceHealth.AverageRpcLatency = PopImapRequestCache.Instance.Values.Average((PopImapRequestData request) => request.RpcLatency);
			serviceHealth.NumberOfErroredRequests = (from request in PopImapRequestCache.Instance.Values
			where request.HasErrors || (request.ResponseType != null && !string.Equals(request.ResponseType, "OK", StringComparison.InvariantCultureIgnoreCase))
			select request).Count<PopImapRequestData>();
			serviceHealth.NumberOfRequests = (long)PopImapRequestCache.Instance.Values.Count;
			serviceHealth.AverageRequestTime = PopImapRequestCache.Instance.Values.Average((PopImapRequestData request) => request.RequestTime);
			serviceHealth.OKResponseRatio = ((serviceHealth.NumberOfRequests > 0L) ? ((double)(serviceHealth.NumberOfRequests - (long)serviceHealth.NumberOfErroredRequests) / (double)serviceHealth.NumberOfRequests) : 0.0);
			if (arguments.Argument.Equals("ShowError", StringComparison.InvariantCultureIgnoreCase))
			{
				serviceHealth.ErrorDetails = new List<ErrorDetail>();
				PopImapRequestCache.Instance.Values.ForEach(delegate(PopImapRequestData request)
				{
					if (request.ErrorDetails != null)
					{
						serviceHealth.ErrorDetails.AddRange(request.ErrorDetails);
					}
				});
			}
			return serviceHealth;
		}

		private const string ShowErrorArgument = "ShowError";

		private static PopImapServiceHealthHandler instance;

		private static object lockObject = new object();
	}
}
