using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring.Management.Common;

namespace Microsoft.Office.Datacenter.ActiveMonitoring.Management
{
	[OutputType(new Type[]
	{
		typeof(MonitorHealthEntry)
	})]
	[Cmdlet("Invoke", "MonitoringProbe")]
	public sealed class InvokeMonitoringProbe : PSCmdlet
	{
		[ValidateNotNullOrEmpty]
		[Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public string Identity { get; set; }

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true)]
		public string Server { get; set; }

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public string ItemTargetExtension { get; set; }

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public string Account { get; set; }

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public string Password { get; set; }

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public string Endpoint { get; set; }

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public string SecondaryAccount { get; set; }

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public string SecondaryPassword { get; set; }

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public string SecondaryEndpoint { get; set; }

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public string TimeOutSeconds { get; set; }

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false)]
		public string PropertyOverride { get; set; }

		protected override void BeginProcessing()
		{
			if (!MonitoringItemIdentity.MonitorIdentityId.IsValidFormat(this.Identity))
			{
				base.WriteError(new ErrorRecord(new ArgumentException(Strings.InvalidMonitorIdentity(this.Identity)), string.Empty, ErrorCategory.InvalidArgument, null));
			}
		}

		protected override void EndProcessing()
		{
			RpcInvokeMonitoringProbe.Reply reply = null;
			LocalizedException ex = null;
			try
			{
				Dictionary<string, string> dictionary = this.CreatePropertyBag();
				string text = string.Empty;
				if (dictionary != null && dictionary.Count != 0)
				{
					text = CrimsonHelper.ConvertDictionaryToXml(dictionary);
				}
				this.ItemTargetExtension = (string.IsNullOrWhiteSpace(this.ItemTargetExtension) ? string.Empty : this.ItemTargetExtension);
				text = (string.IsNullOrWhiteSpace(text) ? string.Empty : text);
				reply = RpcInvokeMonitoringProbe.Invoke(this.Server, this.Identity, text, this.ItemTargetExtension, 300000);
			}
			catch (ActiveMonitoringServerException ex2)
			{
				ex = ex2;
			}
			catch (ActiveMonitoringServerTransientException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				base.WriteWarning(ex.LocalizedString);
				return;
			}
			if (!string.IsNullOrEmpty(reply.ErrorMessage))
			{
				base.WriteWarning(reply.ErrorMessage);
				return;
			}
			MonitoringProbeResult sendToPipeline = new MonitoringProbeResult(this.Server, reply.ProbeResult);
			base.WriteObject(sendToPipeline);
		}

		private static void AddToPropertyBag(string value, string propName, Dictionary<string, string> propertyBag)
		{
			if (value != null && propertyBag != null)
			{
				propertyBag.Add(propName, value);
			}
		}

		private Dictionary<string, string> CreatePropertyBag()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
			InvokeMonitoringProbe.AddToPropertyBag(this.Account, "Account", dictionary);
			InvokeMonitoringProbe.AddToPropertyBag(this.Password, "Password", dictionary);
			InvokeMonitoringProbe.AddToPropertyBag(this.Endpoint, "Endpoint", dictionary);
			InvokeMonitoringProbe.AddToPropertyBag(this.SecondaryAccount, "SecondaryAccount", dictionary);
			InvokeMonitoringProbe.AddToPropertyBag(this.SecondaryPassword, "SecondaryPassword", dictionary);
			InvokeMonitoringProbe.AddToPropertyBag(this.SecondaryEndpoint, "SecondaryEndpoint", dictionary);
			InvokeMonitoringProbe.AddToPropertyBag(this.TimeOutSeconds, "TimeoutSeconds", dictionary);
			InvokeMonitoringProbe.AddToPropertyBag(MonitoringItemIdentity.MonitorIdentityId.GetHealthSet(this.Identity), "ServiceName", dictionary);
			InvokeMonitoringProbe.AddToPropertyBag(MonitoringItemIdentity.MonitorIdentityId.GetMonitor(this.Identity), "Name", dictionary);
			InvokeMonitoringProbe.AddToPropertyBag(MonitoringItemIdentity.MonitorIdentityId.GetTargetResource(this.Identity), "TargetResource", dictionary);
			if (!string.IsNullOrWhiteSpace(this.PropertyOverride))
			{
				this.ParseAndAddPropertyOverrides(dictionary);
			}
			return dictionary;
		}

		private void ParseAndAddPropertyOverrides(Dictionary<string, string> propertyBag)
		{
			string[] array = this.PropertyOverride.Split(new char[]
			{
				','
			}, StringSplitOptions.RemoveEmptyEntries);
			foreach (string text in array)
			{
				if (!text.Contains("="))
				{
					base.WriteError(new ErrorRecord(new ArgumentException(Strings.InvalidPropertyOverrideValue(this.PropertyOverride)), string.Empty, (ErrorCategory)1000, null));
				}
				string[] array3 = text.Split(new char[]
				{
					'='
				}, StringSplitOptions.RemoveEmptyEntries);
				if (array3.Length != 2 || string.IsNullOrWhiteSpace(array3[0]) || string.IsNullOrWhiteSpace(array3[1]))
				{
					base.WriteError(new ErrorRecord(new ArgumentException(Strings.InvalidPropertyOverrideValue(this.PropertyOverride)), string.Empty, (ErrorCategory)1000, null));
				}
				array3[0].Trim();
				array3[1].Trim();
				propertyBag[array3[0]] = array3[1].Replace("'", string.Empty).Replace("\"", string.Empty);
			}
		}
	}
}
