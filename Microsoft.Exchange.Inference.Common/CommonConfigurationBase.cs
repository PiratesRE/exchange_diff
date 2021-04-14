using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Inference.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class CommonConfigurationBase : ICommonConfiguration
	{
		public virtual bool OutlookActivityProcessingEnabled
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual TimeSpan OutlookActivityProcessingCutoffWindow
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual bool OutlookActivityProcessingEnabledInEba
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual bool PersistedLabelsEnabled
		{
			get
			{
				throw new NotImplementedException();
			}
		}
	}
}
