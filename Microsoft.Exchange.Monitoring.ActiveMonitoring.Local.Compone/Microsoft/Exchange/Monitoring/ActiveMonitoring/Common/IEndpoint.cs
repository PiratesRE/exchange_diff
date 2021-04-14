using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	public interface IEndpoint
	{
		void Initialize();

		bool DetectChange();

		bool RestartOnChange { get; }

		Exception Exception { get; set; }
	}
}
