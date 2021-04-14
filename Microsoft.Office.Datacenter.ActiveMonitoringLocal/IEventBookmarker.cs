using System;
using System.Diagnostics.Eventing.Reader;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public interface IEventBookmarker
	{
		void Initialize(string baseLocation);

		EventBookmark Read(string bookmarkName);

		void Write(string bookmarkName, EventBookmark bookmark);

		void Delete(string bookmarkName);
	}
}
