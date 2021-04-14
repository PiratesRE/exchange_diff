using System;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PingAsyncOperation : AsyncOperation
	{
		public PingAsyncOperation(HttpContextBase context) : base(context, "/mapi/", AsyncOperationCookieFlags.AllowSession)
		{
		}

		public override string RequestType
		{
			get
			{
				return "PING";
			}
		}

		public override void ParseRequest(WorkBuffer requestBuffer)
		{
		}

		public override Task ExecuteAsync()
		{
			return Task.FromResult<bool>(true);
		}

		public override void SerializeResponse(out WorkBuffer[] responseBuffers)
		{
			responseBuffers = null;
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<PingAsyncOperation>(this);
		}
	}
}
