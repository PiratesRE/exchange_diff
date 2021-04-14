using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TUC_SipOptionsError : LocalizedException
	{
		public TUC_SipOptionsError(string targetUri, string error) : base(Strings.SipOptionsError(targetUri, error))
		{
			this.targetUri = targetUri;
			this.error = error;
		}

		public TUC_SipOptionsError(string targetUri, string error, Exception innerException) : base(Strings.SipOptionsError(targetUri, error), innerException)
		{
			this.targetUri = targetUri;
			this.error = error;
		}

		protected TUC_SipOptionsError(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.targetUri = (string)info.GetValue("targetUri", typeof(string));
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("targetUri", this.targetUri);
			info.AddValue("error", this.error);
		}

		public string TargetUri
		{
			get
			{
				return this.targetUri;
			}
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string targetUri;

		private readonly string error;
	}
}
