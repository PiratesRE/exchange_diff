using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public interface IBaseFormContentControl
	{
		WebServiceMethod RefreshWebServiceMethod { get; }

		WebServiceMethod SaveWebServiceMethod { get; }

		bool ReadOnly { get; }

		bool HasSaveMethod { get; }

		SectionCollection Sections { get; }
	}
}
