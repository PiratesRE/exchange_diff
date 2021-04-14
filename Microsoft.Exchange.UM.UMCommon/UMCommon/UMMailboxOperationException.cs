using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UMMailboxOperationException : LocalizedException
	{
		public UMMailboxOperationException(string moreInfo) : base(Strings.ErrorPerformingUMMailboxOperation(moreInfo))
		{
			this.moreInfo = moreInfo;
		}

		public UMMailboxOperationException(string moreInfo, Exception innerException) : base(Strings.ErrorPerformingUMMailboxOperation(moreInfo), innerException)
		{
			this.moreInfo = moreInfo;
		}

		protected UMMailboxOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
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
