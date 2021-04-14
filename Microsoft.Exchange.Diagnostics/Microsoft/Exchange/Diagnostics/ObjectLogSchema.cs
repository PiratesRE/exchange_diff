using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Exchange.Diagnostics
{
	internal abstract class ObjectLogSchema
	{
		public ObjectLogSchema()
		{
		}

		public virtual string Software
		{
			get
			{
				return "Microsoft Exchange Server";
			}
		}

		public virtual string Version
		{
			get
			{
				return ObjectLogSchema.DefaultVersion;
			}
		}

		public abstract string LogType { get; }

		public virtual HashSet<string> ExcludedProperties
		{
			get
			{
				return null;
			}
		}

		public const string DefaultSoftware = "Microsoft Exchange Server";

		private static readonly string DefaultVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
	}
}
