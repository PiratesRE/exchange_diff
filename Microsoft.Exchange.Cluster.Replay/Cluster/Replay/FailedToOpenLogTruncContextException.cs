using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToOpenLogTruncContextException : LogTruncationException
	{
		public FailedToOpenLogTruncContextException(string dbSrc, uint hresult, string optionalFriendlyError) : base(ReplayStrings.FailedToOpenLogTruncContext(dbSrc, hresult, optionalFriendlyError))
		{
			this.dbSrc = dbSrc;
			this.hresult = hresult;
			this.optionalFriendlyError = optionalFriendlyError;
		}

		public FailedToOpenLogTruncContextException(string dbSrc, uint hresult, string optionalFriendlyError, Exception innerException) : base(ReplayStrings.FailedToOpenLogTruncContext(dbSrc, hresult, optionalFriendlyError), innerException)
		{
			this.dbSrc = dbSrc;
			this.hresult = hresult;
			this.optionalFriendlyError = optionalFriendlyError;
		}

		protected FailedToOpenLogTruncContextException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbSrc = (string)info.GetValue("dbSrc", typeof(string));
			this.hresult = (uint)info.GetValue("hresult", typeof(uint));
			this.optionalFriendlyError = (string)info.GetValue("optionalFriendlyError", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbSrc", this.dbSrc);
			info.AddValue("hresult", this.hresult);
			info.AddValue("optionalFriendlyError", this.optionalFriendlyError);
		}

		public string DbSrc
		{
			get
			{
				return this.dbSrc;
			}
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

		private readonly string dbSrc;

		private readonly uint hresult;

		private readonly string optionalFriendlyError;
	}
}
