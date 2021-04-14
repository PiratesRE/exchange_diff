using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.Diagnostics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class LoadBalanceDiagnosableArgumentBase : DiagnosableArgument
	{
		public bool Verbose
		{
			get
			{
				return base.HasArgument("verbose");
			}
		}

		public bool TraceEnabled
		{
			get
			{
				return base.HasArgument("trace");
			}
		}

		protected override bool FailOnMissingArgument
		{
			get
			{
				return true;
			}
		}

		protected override void InitializeSchema(Dictionary<string, Type> schema)
		{
			schema["verbose"] = typeof(bool);
			schema["trace"] = typeof(bool);
			this.ExtendSchema(schema);
		}

		protected abstract void ExtendSchema(Dictionary<string, Type> schema);

		internal const string VerboseArgument = "verbose";

		internal const string TraceArgument = "trace";
	}
}
