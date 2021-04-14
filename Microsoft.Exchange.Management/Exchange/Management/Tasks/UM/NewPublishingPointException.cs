using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NewPublishingPointException : LocalizedException
	{
		public NewPublishingPointException(string shareName, string moreInfo) : base(Strings.NewPublishingPointException(shareName, moreInfo))
		{
			this.shareName = shareName;
			this.moreInfo = moreInfo;
		}

		public NewPublishingPointException(string shareName, string moreInfo, Exception innerException) : base(Strings.NewPublishingPointException(shareName, moreInfo), innerException)
		{
			this.shareName = shareName;
			this.moreInfo = moreInfo;
		}

		protected NewPublishingPointException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.shareName = (string)info.GetValue("shareName", typeof(string));
			this.moreInfo = (string)info.GetValue("moreInfo", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("shareName", this.shareName);
			info.AddValue("moreInfo", this.moreInfo);
		}

		public string ShareName
		{
			get
			{
				return this.shareName;
			}
		}

		public string MoreInfo
		{
			get
			{
				return this.moreInfo;
			}
		}

		private readonly string shareName;

		private readonly string moreInfo;
	}
}
