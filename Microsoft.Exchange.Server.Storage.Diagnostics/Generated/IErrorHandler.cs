using System;

namespace Microsoft.Exchange.Server.Storage.Diagnostics.Generated
{
	public interface IErrorHandler
	{
		int ErrNum { get; }

		int WrnNum { get; }

		void AddError(string msg, int lin, int col, int len, int severity);
	}
}
