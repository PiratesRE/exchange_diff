using System;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class ServiceTagScope : IDisposable
	{
		public ServiceTagScope(string newServiceTag)
		{
			if (newServiceTag != null)
			{
				this.oldServiceTag = DalHelper.ServiceTagContext;
				DalHelper.ServiceTagContext = newServiceTag;
			}
		}

		public void Dispose()
		{
			if (this.oldServiceTag != null)
			{
				DalHelper.ServiceTagContext = this.oldServiceTag;
				this.oldServiceTag = null;
			}
		}

		private string oldServiceTag;
	}
}
