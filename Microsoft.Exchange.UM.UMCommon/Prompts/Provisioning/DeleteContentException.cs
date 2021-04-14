using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.Prompts.Provisioning
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DeleteContentException : PublishingException
	{
		public DeleteContentException(string moreInfo) : base(Strings.DeleteContentException(moreInfo))
		{
			this.moreInfo = moreInfo;
		}

		public DeleteContentException(string moreInfo, Exception innerException) : base(Strings.DeleteContentException(moreInfo), innerException)
		{
			this.moreInfo = moreInfo;
		}

		protected DeleteContentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.moreInfo = (string)info.GetValue("moreInfo", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("moreInfo", this.moreInfo);
		}

		public string MoreInfo
		{
			get
			{
				return this.moreInfo;
			}
		}

		private readonly string moreInfo;
	}
}
