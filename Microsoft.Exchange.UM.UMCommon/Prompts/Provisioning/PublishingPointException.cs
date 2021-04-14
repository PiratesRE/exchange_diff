using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.Prompts.Provisioning
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PublishingPointException : PublishingException
	{
		public PublishingPointException(string moreInfo) : base(Strings.ErrorAccessingPublishingPoint(moreInfo))
		{
			this.moreInfo = moreInfo;
		}

		public PublishingPointException(string moreInfo, Exception innerException) : base(Strings.ErrorAccessingPublishingPoint(moreInfo), innerException)
		{
			this.moreInfo = moreInfo;
		}

		protected PublishingPointException(SerializationInfo info, StreamingContext context) : base(info, context)
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
