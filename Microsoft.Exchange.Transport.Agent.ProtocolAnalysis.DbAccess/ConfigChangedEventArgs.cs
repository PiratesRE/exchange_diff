using System;
using Microsoft.Exchange.Configuration.Common;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess
{
	internal sealed class ConfigChangedEventArgs : EventArgs
	{
		public ConfigChangedEventArgs(PropertyBag fields)
		{
			this.fields = fields;
		}

		public PropertyBag Fields
		{
			get
			{
				return this.fields;
			}
		}

		private readonly PropertyBag fields;
	}
}
