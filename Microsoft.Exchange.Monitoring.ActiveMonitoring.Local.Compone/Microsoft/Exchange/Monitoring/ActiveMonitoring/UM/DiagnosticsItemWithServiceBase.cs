using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UM
{
	internal abstract class DiagnosticsItemWithServiceBase : DiagnosticsItemBase
	{
		public string FullService
		{
			get
			{
				return base.GetValue("service");
			}
		}

		public string Service
		{
			get
			{
				string fullService = this.FullService;
				int num = fullService.IndexOf('/');
				if (num > 0)
				{
					return fullService.Substring(0, num);
				}
				return fullService;
			}
		}

		public string Version
		{
			get
			{
				string fullService = this.FullService;
				int num = fullService.IndexOf('/');
				if (num > 0)
				{
					return fullService.Substring(num + 1);
				}
				return string.Empty;
			}
		}
	}
}
