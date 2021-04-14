using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ConfigurationSettings
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DiagnosticSettingsContext : SettingsContextBase
	{
		public DiagnosticSettingsContext(IConfigSchema schema, ConfigDiagnosticArgument argument) : base(null)
		{
			this.serverName = argument.GetArgumentOrDefault<string>("servername", null);
			this.serverVersion = DiagnosticSettingsContext.GetServerVersion(argument, "serverversion");
			this.processName = argument.GetArgumentOrDefault<string>("processname", null);
			this.databaseName = argument.GetArgumentOrDefault<string>("dbname", null);
			this.databaseVersion = DiagnosticSettingsContext.GetServerVersion(argument, "dbversion");
			this.organizationName = argument.GetArgumentOrDefault<string>("orgname", null);
			this.organizationVersion = DiagnosticSettingsContext.GetExchangeObjectVersion(argument, "orgversion");
			this.mailboxGuid = argument.GetArgumentOrDefault<Guid?>("mailboxguid", null);
			if (argument.HasArgument("genericscope"))
			{
				this.propertyName = argument.GetArgument<string>("genericscope");
				this.propertyValue = argument.GetArgumentOrDefault<string>("genericscopevalue", null);
				if (!argument.HasArgument("force"))
				{
					this.propertyValue = schema.ParseAndValidateScopeValue(this.propertyName, this.propertyValue);
				}
			}
		}

		public override string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		public override ServerVersion ServerVersion
		{
			get
			{
				return this.serverVersion;
			}
		}

		public override string ProcessName
		{
			get
			{
				return this.processName;
			}
		}

		public override string DatabaseName
		{
			get
			{
				return this.databaseName;
			}
		}

		public override ServerVersion DatabaseVersion
		{
			get
			{
				return this.databaseVersion;
			}
		}

		public override string OrganizationName
		{
			get
			{
				return this.organizationName;
			}
		}

		public override ExchangeObjectVersion OrganizationVersion
		{
			get
			{
				return this.organizationVersion;
			}
		}

		public override Guid? MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		public override string GetGenericProperty(string propertyName)
		{
			if (StringComparer.InvariantCultureIgnoreCase.Equals(propertyName, this.propertyName))
			{
				return this.propertyValue;
			}
			return null;
		}

		public override XElement GetDiagnosticInfo(string argument)
		{
			XElement diagnosticInfo = base.GetDiagnosticInfo(argument);
			if (!string.IsNullOrEmpty(this.propertyName))
			{
				diagnosticInfo.Add(new XElement(this.propertyName, this.propertyValue));
			}
			return diagnosticInfo;
		}

		private static ServerVersion GetServerVersion(ConfigDiagnosticArgument argument, string argumentName)
		{
			if (!argument.HasArgument(argumentName))
			{
				return null;
			}
			return ServerVersion.ParseFromSerialNumber(argument.GetArgument<string>(argumentName));
		}

		private static ExchangeObjectVersion GetExchangeObjectVersion(ConfigDiagnosticArgument argument, string argumentName)
		{
			if (!argument.HasArgument(argumentName))
			{
				return null;
			}
			return ExchangeObjectVersion.Parse(argument.GetArgument<string>(argumentName));
		}

		private readonly string serverName;

		private readonly ServerVersion serverVersion;

		private readonly string processName;

		private readonly string databaseName;

		private readonly ServerVersion databaseVersion;

		private readonly string organizationName;

		private readonly ExchangeObjectVersion organizationVersion;

		private readonly Guid? mailboxGuid;

		private readonly string propertyName;

		private readonly string propertyValue;
	}
}
