using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Security;
using System.Threading;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public sealed class RpsProbeHelper
	{
		public RpsProbeHelper(ProbeWorkItem probe, bool delegated)
		{
			if (probe == null)
			{
				string text = "The ProbeWorkItem is null.";
				this.TraceError(text);
				throw new ArgumentNullException(text);
			}
			this.probeWorkItem = probe;
			this.delegated = delegated;
		}

		public void DoWork(CancellationToken cancellationToken)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			this.TraceDebug(string.Format("RemotePowershellProbe started: {0}", DateTime.UtcNow));
			try
			{
				this.GetExtensionAttributes();
				foreach (RpsProbeHelper.PSCmdlet pscmdlet in this.extensionAttributes.WorkContext.Cmdlets)
				{
					DateTime dateTime = DateTime.UtcNow;
					TimeSpan timeout = new TimeSpan(0L);
					if (!string.IsNullOrWhiteSpace(pscmdlet.RetryTimeInSeconds))
					{
						dateTime = dateTime.AddSeconds(Convert.ToDouble(pscmdlet.RetryTimeInSeconds));
					}
					if (!string.IsNullOrWhiteSpace(pscmdlet.SleepTimeInSeconds))
					{
						timeout = new TimeSpan(0, 0, Convert.ToInt32(pscmdlet.SleepTimeInSeconds));
					}
					IEnumerable<string> enumerable = new List<string>();
					do
					{
						cancellationToken.ThrowIfCancellationRequested();
						IEnumerable<PSObject> enumerable2 = this.ExecutePSCmdlet(pscmdlet, dateTime);
						if (enumerable2 != null)
						{
							enumerable = this.VerifyPSResult(enumerable2, pscmdlet);
							if (enumerable.Count<string>() == 0)
							{
								break;
							}
						}
						Thread.Sleep(timeout);
					}
					while (dateTime > DateTime.UtcNow);
					if (enumerable.Count<string>() != 0)
					{
						throw new ApplicationException(string.Format("PS cmdlet {0} failed - {1}", pscmdlet.Name, string.Join(",", enumerable)));
					}
					this.TraceDebug(string.Format("Remote PowerShell for cmdlet {0} passed.", pscmdlet.Name));
					if (pscmdlet.Logout != null && pscmdlet.Logout.Equals("true", StringComparison.InvariantCultureIgnoreCase) && string.IsNullOrWhiteSpace(pscmdlet.EndpointUri))
					{
						this.powerShellLookup[Tuple.Create<string, string>(pscmdlet.EndpointUri, pscmdlet.UserName)].Dispose();
					}
				}
			}
			catch (Exception ex)
			{
				this.TraceError(ex.ToString());
				throw;
			}
			finally
			{
				this.CloseRemotePowerShell();
				stopwatch.Stop();
				this.TraceDebug(string.Format("RemotePowershellProbe finished in: {0}", stopwatch.Elapsed));
			}
		}

		private void CloseRemotePowerShell()
		{
			foreach (RemotePowershellWrapper remotePowershellWrapper in this.powerShellLookup.Values)
			{
				remotePowershellWrapper.Dispose();
			}
		}

		private IEnumerable<PSObject> ExecutePSCmdlet(RpsProbeHelper.PSCmdlet cmdlet, DateTime endTime)
		{
			IEnumerable<PSObject> enumerable = null;
			RemotePowershellWrapper remotePowershellWrapper = this.defaultRps;
			try
			{
				if (!string.IsNullOrWhiteSpace(cmdlet.EndpointUri))
				{
					remotePowershellWrapper = this.OpenRemotePowerShell(cmdlet.EndpointUri, cmdlet.UserName, cmdlet.Password, null);
				}
				enumerable = remotePowershellWrapper.Execute(this.CreatePSCommand(cmdlet));
			}
			catch (Exception ex)
			{
				this.TraceDebug(string.Format("Remote PowerShell for cmdlet {0} failed. {1}", cmdlet.Name, ex.ToString()));
				if (this.IsExpectedStatusPass(cmdlet))
				{
					if (endTime >= DateTime.UtcNow)
					{
						return null;
					}
					if (ex.InnerException != null)
					{
						throw ex.InnerException;
					}
					throw;
				}
			}
			if (this.IsExpectedStatusPass(cmdlet))
			{
				if (enumerable == null)
				{
					throw new ApplicationException(string.Format("Remote PowerShell returned no results for {0} user with cmdlet {1}.", this.userName, cmdlet.Name));
				}
			}
			else if (enumerable != null)
			{
				throw new ApplicationException(string.Format("Remote PowerShell returned results for {0} user with cmdlet {1} when it shoud have failed.", this.userName, cmdlet.Name));
			}
			return enumerable;
		}

		private RemotePowershellWrapper OpenRemotePowerShell(string endpointUri, string userName, string password, string delegatedTenant)
		{
			Tuple<string, string> key = Tuple.Create<string, string>(endpointUri, userName);
			if (!this.powerShellLookup.ContainsKey(key))
			{
				this.powerShellLookup.Add(key, new RemotePowershellWrapper(endpointUri, userName, this.ConvertStringToSecure(password), delegatedTenant));
			}
			return this.powerShellLookup[key];
		}

		private bool IsExpectedStatusPass(RpsProbeHelper.PSCmdlet cmdlet)
		{
			return cmdlet.Status == null || cmdlet.Status.Equals("pass", StringComparison.OrdinalIgnoreCase);
		}

		private void GetExtensionAttributes()
		{
			this.extensionAttributes = this.LoadContextFromXml(this.probeWorkItem.Definition.ExtensionAttributes);
			if (this.delegated)
			{
				this.userName = this.VerifyXmlIsNotNullOrEmpty(RpsProbeHelper.DelegatedAdminName, this.extensionAttributes.WorkContext.DelegatedAdmin);
				this.delegatedTenant = this.VerifyXmlIsNotNullOrEmpty(RpsProbeHelper.DelegatedTenantName, this.extensionAttributes.WorkContext.DelegatedTenant);
			}
			else
			{
				this.userName = this.VerifyXmlIsNotNullOrEmpty(RpsProbeHelper.TenantAdminName, this.extensionAttributes.WorkContext.TenantAdmin);
				this.delegatedTenant = null;
			}
			this.password = this.VerifyXmlIsNotNullOrEmpty(RpsProbeHelper.PasswordName, this.extensionAttributes.WorkContext.Password);
			this.endPointUri = this.VerifyXmlIsNotNullOrEmpty(RpsProbeHelper.EndPointUriName, this.extensionAttributes.WorkContext.EndpointUri);
			this.defaultRps = this.OpenRemotePowerShell(this.endPointUri, this.userName, this.password, this.delegatedTenant);
		}

		private RpsProbeHelper.ExtensionAttributes LoadContextFromXml(string xml)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(RpsProbeHelper.ExtensionAttributes));
			return (RpsProbeHelper.ExtensionAttributes)xmlSerializer.Deserialize(new StringReader(xml));
		}

		private IEnumerable<string> VerifyPSResult(IEnumerable<PSObject> results, RpsProbeHelper.PSCmdlet cmdlet)
		{
			List<string> list = new List<string>();
			if (cmdlet.ExpectedResults != null && results != null)
			{
				foreach (RpsProbeHelper.CmdletExpectedResult cmdletExpectedResult in cmdlet.ExpectedResults)
				{
					foreach (PSObject psobject in results)
					{
						if (psobject.Properties[cmdletExpectedResult.Property] != null && string.Compare(psobject.Properties[cmdletExpectedResult.Property].Value.ToString(), cmdletExpectedResult.Value, true) != 0)
						{
							list.Add(string.Format("The property '{0}' did  not have the expected value. Expected:{1}, Actual:{2} ", cmdletExpectedResult.Property, cmdletExpectedResult.Value, psobject.Properties[cmdletExpectedResult.Property].Value.ToString()));
						}
					}
				}
			}
			return list;
		}

		private SecureString ConvertStringToSecure(string password)
		{
			char[] array = password.ToCharArray();
			SecureString secureString = new SecureString();
			foreach (char c in array)
			{
				secureString.AppendChar(c);
			}
			return secureString;
		}

		private PSCommand CreatePSCommand(RpsProbeHelper.PSCmdlet cmdlet)
		{
			PSCommand pscommand = new PSCommand();
			pscommand.AddCommand(cmdlet.Name);
			if (cmdlet.Parameters != null)
			{
				foreach (RpsProbeHelper.CmdletParameter cmdletParameter in cmdlet.Parameters)
				{
					if (string.IsNullOrEmpty(cmdletParameter.Value))
					{
						pscommand.AddParameter(cmdletParameter.Name);
					}
					else
					{
						pscommand.AddParameter(cmdletParameter.Name, cmdletParameter.Value);
					}
				}
			}
			return pscommand;
		}

		private string VerifyXmlIsNotNullOrEmpty(string name, string value)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				string text = string.Format("The {0} is not set in the ExtensionAttributes: {1}", name, this.probeWorkItem.Definition.ExtensionAttributes);
				this.TraceError(text);
				throw new ArgumentException(text);
			}
			return value;
		}

		private void TraceDebug(string debugMsg)
		{
			ProbeResult result = this.probeWorkItem.Result;
			result.ExecutionContext = result.ExecutionContext + debugMsg + " ";
			WTFDiagnostics.TraceDebug(ExTraceGlobals.RPSTracer, this.probeWorkItem.TraceContext, debugMsg, null, "TraceDebug", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\RPS\\RpsProbeHelper.cs", 442);
		}

		private void TraceError(string errorMsg)
		{
			ProbeResult result = this.probeWorkItem.Result;
			result.ExecutionContext = result.ExecutionContext + errorMsg + " ";
			WTFDiagnostics.TraceError(ExTraceGlobals.RPSTracer, this.probeWorkItem.TraceContext, errorMsg, null, "TraceError", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\RPS\\RpsProbeHelper.cs", 452);
		}

		internal static readonly string TenantAdminName = "TenantAdmin";

		internal static readonly string PasswordName = "Password";

		internal static readonly string DelegatedAdminName = "DelegatedAdmin";

		internal static readonly string DelegatedTenantName = "DelegatedTenant";

		internal static readonly string EndPointUriName = "EndpointUri";

		private readonly bool delegated;

		private readonly ProbeWorkItem probeWorkItem;

		private string userName;

		private string delegatedTenant;

		private string endPointUri;

		private RpsProbeHelper.ExtensionAttributes extensionAttributes;

		private string password;

		private Dictionary<Tuple<string, string>, RemotePowershellWrapper> powerShellLookup = new Dictionary<Tuple<string, string>, RemotePowershellWrapper>();

		private RemotePowershellWrapper defaultRps;

		[XmlType(AnonymousType = true)]
		public class ExtensionAttributes
		{
			[XmlElement(Form = XmlSchemaForm.Unqualified)]
			public RpsProbeHelper.WorkContext WorkContext { get; set; }
		}

		[XmlType(AnonymousType = true)]
		public class WorkContext
		{
			[XmlElement(Form = XmlSchemaForm.Unqualified)]
			public string DelegatedTenant { get; set; }

			[XmlElement(Form = XmlSchemaForm.Unqualified)]
			public string DelegatedAdmin { get; set; }

			[XmlElement(Form = XmlSchemaForm.Unqualified)]
			public string TenantAdmin { get; set; }

			[XmlElement(Form = XmlSchemaForm.Unqualified)]
			public string Password { get; set; }

			[XmlElement(Form = XmlSchemaForm.Unqualified)]
			public string EndpointUri { get; set; }

			[XmlArrayItem("Cmdlet", Form = XmlSchemaForm.Unqualified, IsNullable = false)]
			public RpsProbeHelper.PSCmdlet[] Cmdlets { get; set; }
		}

		[XmlType(AnonymousType = true)]
		public class PSCmdlet
		{
			[XmlElement(Form = XmlSchemaForm.Unqualified)]
			public string Logout { get; set; }

			[XmlElement(Form = XmlSchemaForm.Unqualified)]
			public string UserName { get; set; }

			[XmlElement(Form = XmlSchemaForm.Unqualified)]
			public string Password { get; set; }

			[XmlElement(Form = XmlSchemaForm.Unqualified)]
			public string EndpointUri { get; set; }

			[XmlElement(Form = XmlSchemaForm.Unqualified)]
			public string RetryTimeInSeconds { get; set; }

			[XmlElement(Form = XmlSchemaForm.Unqualified)]
			public string SleepTimeInSeconds { get; set; }

			[XmlArrayItem("Parameter", Form = XmlSchemaForm.Unqualified, IsNullable = true)]
			public RpsProbeHelper.CmdletParameter[] Parameters { get; set; }

			[XmlArrayItem("ExpectedResult", Form = XmlSchemaForm.Unqualified, IsNullable = true)]
			public RpsProbeHelper.CmdletExpectedResult[] ExpectedResults { get; set; }

			[XmlAttribute]
			public string Name { get; set; }

			[XmlAttribute]
			public string Status { get; set; }
		}

		[XmlType(AnonymousType = true)]
		public class CmdletParameter
		{
			[XmlAttribute]
			public string Name { get; set; }

			[XmlAttribute]
			public string Value { get; set; }
		}

		[XmlType(AnonymousType = true)]
		public class CmdletExpectedResult
		{
			[XmlAttribute]
			public string Property { get; set; }

			[XmlAttribute]
			public string Value { get; set; }

			[XmlAttribute]
			public string Type { get; set; }
		}
	}
}
