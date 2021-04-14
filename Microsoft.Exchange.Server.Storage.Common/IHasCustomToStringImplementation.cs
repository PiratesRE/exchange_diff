using System;
using System.Text;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public interface IHasCustomToStringImplementation
	{
		void AppendAsString(StringBuilder sb);
	}
}
