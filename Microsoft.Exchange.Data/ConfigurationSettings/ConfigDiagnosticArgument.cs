using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ConfigurationSettings
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConfigDiagnosticArgument : DiagnosableArgument
	{
		public ConfigDiagnosticArgument(string argument)
		{
			base.Initialize(argument);
		}

		protected override void InitializeSchema(Dictionary<string, Type> schema)
		{
			schema["force"] = typeof(bool);
			schema["invokescan"] = typeof(bool);
			schema["configname"] = typeof(string);
			schema["servername"] = typeof(string);
			schema["serverversion"] = typeof(string);
			schema["processname"] = typeof(string);
			schema["dbname"] = typeof(string);
			schema["dbversion"] = typeof(string);
			schema["orgname"] = typeof(string);
			schema["orgversion"] = typeof(string);
			schema["mailboxguid"] = typeof(Guid);
			schema["genericscope"] = typeof(string);
			schema["genericscopevalue"] = typeof(string);
		}

		public const string InvokeScan = "invokescan";

		public const string ConfigName = "configname";

		public const string ServerName = "servername";

		public const string ServerVersion = "serverversion";

		public const string ProcessName = "processname";

		public const string DatabaseName = "dbname";

		public const string DatabaseVersion = "dbversion";

		public const string OrganizationName = "orgname";

		public const string OrganizationVersion = "orgversion";

		public const string MailboxGuid = "mailboxguid";

		public const string GenericScope = "genericscope";

		public const string GenericScopeValue = "genericscopevalue";

		public const string Force = "force";
	}
}
