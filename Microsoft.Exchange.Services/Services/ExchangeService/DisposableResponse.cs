using System;

namespace Microsoft.Exchange.Services.ExchangeService
{
	internal class DisposableResponse<TResponse> : IDisposableResponse<TResponse>, IDisposable
	{
		internal IDisposable Command { get; set; }

		public virtual TResponse Response { get; set; }

		public DisposableResponse(IDisposable command, TResponse response)
		{
			this.Command = command;
			this.Response = response;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.Command != null)
			{
				this.Command.Dispose();
				this.Command = null;
			}
		}
	}
}
