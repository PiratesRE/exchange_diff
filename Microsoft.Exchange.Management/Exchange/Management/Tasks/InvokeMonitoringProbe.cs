using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring.Management.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Invoke", "MonitoringProbe")]
	public sealed class InvokeMonitoringProbe : Task
	{
		[ValidateNotNullOrEmpty]
		[Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public string Identity
		{
			get
			{
				return (string)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		[ValidateNotNullOrEmpty]
		public ServerIdParameter Server
		{
			get
			{
				return this.serverId;
			}
			set
			{
				this.serverId = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false)]
		public string ItemTargetExtension
		{
			get
			{
				return (string)base.Fields["ItemTargetExtension"];
			}
			set
			{
				base.Fields["ItemTargetExtension"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false)]
		public string Account
		{
			get
			{
				return (string)base.Fields["Account"];
			}
			set
			{
				base.Fields["Account"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public string Password
		{
			get
			{
				return (string)base.Fields["Password"];
			}
			set
			{
				base.Fields["Password"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false)]
		public string Endpoint
		{
			get
			{
				return (string)base.Fields["Endpoint"];
			}
			set
			{
				base.Fields["Endpoint"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false)]
		public string SecondaryAccount
		{
			get
			{
				return (string)base.Fields["SecondaryAccount"];
			}
			set
			{
				base.Fields["SecondaryAccount"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public string SecondaryPassword
		{
			get
			{
				return (string)base.Fields["SecondaryPassword"];
			}
			set
			{
				base.Fields["SecondaryPassword"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public string SecondaryEndpoint
		{
			get
			{
				return (string)base.Fields["SecondaryEndpoint"];
			}
			set
			{
				base.Fields["SecondaryEndpoint"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public string TimeOutSeconds
		{
			get
			{
				return (string)base.Fields["TimeOutSeconds"];
			}
			set
			{
				base.Fields["TimeOutSeconds"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public string PropertyOverride
		{
			get
			{
				return (string)base.Fields["PropertyOverride"];
			}
			set
			{
				base.Fields["PropertyOverride"] = value;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				if (!MonitoringItemIdentity.MonitorIdentityId.IsValidFormat(this.Identity))
				{
					base.WriteError(new ArgumentException(Strings.InvalidMonitorIdentity(this.Identity)), (ErrorCategory)1000, null);
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
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
					reply = RpcInvokeMonitoringProbe.Invoke(this.Server.Fqdn, this.Identity, text, this.ItemTargetExtension, 300000);
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
					this.WriteWarning(ex.LocalizedString);
				}
				else if (!string.IsNullOrEmpty(reply.ErrorMessage))
				{
					base.WriteWarning(reply.ErrorMessage);
				}
				else
				{
					MonitoringProbeResult sendToPipeline = new MonitoringProbeResult(this.Server.Fqdn, reply.ProbeResult);
					base.WriteObject(sendToPipeline);
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is InvalidVersionException || exception is InvalidIdentityException || exception is InvalidDurationException || base.IsKnownException(exception);
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

		private static void AddToPropertyBag(string value, string propName, Dictionary<string, string> propertyBag)
		{
			if (value != null && propertyBag != null)
			{
				propertyBag.Add(propName, value);
			}
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
					base.WriteError(new ArgumentException(Strings.InvalidPropertyOverrideValue(this.PropertyOverride)), (ErrorCategory)1000, null);
				}
				string[] array3 = text.Split(new char[]
				{
					'='
				}, StringSplitOptions.RemoveEmptyEntries);
				if (array3.Length != 2 || string.IsNullOrWhiteSpace(array3[0]) || string.IsNullOrWhiteSpace(array3[1]))
				{
					base.WriteError(new ArgumentException(Strings.InvalidPropertyOverrideValue(this.PropertyOverride)), (ErrorCategory)1000, null);
				}
				array3[0].Trim();
				array3[1].Trim();
				propertyBag[array3[0]] = array3[1].Replace("'", "").Replace("\"", "");
			}
		}

		private ServerIdParameter serverId;
	}
}
