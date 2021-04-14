using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToTruncateLocallyException : LogTruncationException
	{
		public FailedToTruncateLocallyException(uint hresult, string optionalFriendlyError) : base(ReplayStrings.FailedToTruncateLocallyException(hresult, optionalFriendlyError))
		{
			this.hresult = hresult;
			this.optionalFriendlyError = optionalFriendlyError;
		}

		public FailedToTruncateLocallyException(uint hresult, string optionalFriendlyError, Exception innerException) : base(ReplayStrings.FailedToTruncateLocallyException(hresult, optionalFriendlyError), innerException)
		{
			this.hresult = hresult;
			this.optionalFriendlyError = optionalFriendlyError;
		}

		protected FailedToTruncateLocallyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.hresult = (uint)info.GetValue("hresult", typeof(uint));
			this.optionalFriendlyError = (string)info.GetValue("optionalFriendlyError", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("hresult", this.hresult);
			info.AddValue("optionalFriendlyError", this.optionalFriendlyError);
		}

		public uint Hresult
		{
			get
			{
				return this.hresult;
			}
		}

		public string OptionalFriendlyError
		{
			get
			{
				return this.optionalFriendlyError;
			}
		}

		private readonly uint hresult;

		private readonly string optionalFriendlyError;
	}
}
