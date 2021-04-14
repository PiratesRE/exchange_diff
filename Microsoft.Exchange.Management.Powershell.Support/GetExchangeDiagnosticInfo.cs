using System;
using System.Management.Automation;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Cmdlet("Get", "ExchangeDiagnosticInfo")]
	public sealed class GetExchangeDiagnosticInfo : GetTaskBase<ExchangeDiagnosticInfoResult>
	{
		[Parameter(Mandatory = false, ValueFromPipeline = true)]
		[ValidateNotNullOrEmpty]
		public ServerIdParameter Server
		{
			get
			{
				return (ServerIdParameter)base.Fields["Server"];
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateLength(0, 256)]
		public string Process
		{
			get
			{
				return (string)base.Fields["Process"];
			}
			set
			{
				base.Fields["Process"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateLength(0, 256)]
		public string Component
		{
			get
			{
				return (string)base.Fields["Component"];
			}
			set
			{
				base.Fields["Component"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateLength(0, 1048576)]
		public string Argument
		{
			get
			{
				return (string)base.Fields["Argument"];
			}
			set
			{
				base.Fields["Argument"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Unlimited
		{
			get
			{
				return (SwitchParameter)(base.Fields["Unlimited"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Unlimited"] = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 112, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Support\\DiagnosticTasks\\GetExchangeDiagnosticInfo.cs");
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (this.Server == null)
			{
				this.Server = new ServerIdParameter();
			}
			Server server = (Server)base.GetDataObject<Server>(this.Server, base.DataSession as IConfigurationSession, null, new LocalizedString?(Strings.ErrorServerNotFound(this.Server.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(this.Server.ToString())));
			if (string.IsNullOrEmpty(server.Fqdn))
			{
				base.WriteError(new LocalizedException(Strings.ErrorMissingServerFqdn(this.Server.ToString())), ErrorCategory.InvalidOperation, this.Server);
				return;
			}
			this.serverFqdn = server.Fqdn;
			this.serverVersion = server.AdminDisplayVersion;
		}

		protected override void InternalProcessRecord()
		{
			string userIdentity = (base.ExchangeRunspaceConfig == null) ? string.Empty : base.ExchangeRunspaceConfig.IdentityName;
			bool componentRestrictedData = GetExchangeDiagnosticInfo.CheckDataRedactionEnabled(this.serverVersion) && !base.NeedSuppressingPiiData;
			string xml = ProcessAccessManager.ClientRunProcessCommand(this.serverFqdn, this.Process, this.Component, this.Argument, componentRestrictedData, this.Unlimited, userIdentity);
			string result = GetExchangeDiagnosticInfo.ReformatXml(xml);
			this.WriteResult(new ExchangeDiagnosticInfoResult(result));
		}

		private static string ReformatXml(string xml)
		{
			string result;
			try
			{
				XDocument xdocument = XDocument.Parse(xml);
				result = xdocument.ToString(SaveOptions.None);
			}
			catch (XmlException)
			{
				result = xml;
			}
			return result;
		}

		public static bool CheckDataRedactionEnabled(ServerVersion target)
		{
			ServerVersion b = new ServerVersion(15, 0, 586, 0);
			return !(target == null) && ServerVersion.Compare(target, b) >= 0;
		}

		private string serverFqdn;

		private ServerVersion serverVersion;
	}
}
