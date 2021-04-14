using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ErrorInvalidFingerprintException : LocalizedException
	{
		public ErrorInvalidFingerprintException(string content) : base(Strings.ErrorInvalidFingerprint(content))
		{
			this.content = content;
		}

		public ErrorInvalidFingerprintException(string content, Exception innerException) : base(Strings.ErrorInvalidFingerprint(content), innerException)
		{
			this.content = content;
		}

		protected ErrorInvalidFingerprintException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.content = (string)info.GetValue("content", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("content", this.content);
		}

		public string Content
		{
			get
			{
				return this.content;
			}
		}

		private readonly string content;
	}
}
