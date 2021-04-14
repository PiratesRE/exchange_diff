using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AggregatedUserConfigurationDescriptor
	{
		public AggregatedUserConfigurationDescriptor(string name, IEnumerable<UserConfigurationDescriptor> sources)
		{
			this.name = name;
			this.sources = sources;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public IEnumerable<UserConfigurationDescriptor> Sources
		{
			get
			{
				return this.sources;
			}
		}

		private readonly string name;

		private readonly IEnumerable<UserConfigurationDescriptor> sources;
	}
}
