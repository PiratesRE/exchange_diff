using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	internal class MailAppsArgument : DiagnosableArgument
	{
		public MailAppsArgument(string argument)
		{
			base.Initialize(argument);
		}

		protected override void InitializeSchema(Dictionary<string, Type> schema)
		{
			schema["org"] = typeof(string);
			schema["usr"] = typeof(string);
			schema["cmd"] = typeof(string);
			schema["len"] = typeof(int);
		}

		public const string OrgArgument = "org";

		public const string UserArgument = "usr";

		public const string CommandArgument = "cmd";

		public const string LengthArgument = "len";
	}
}
