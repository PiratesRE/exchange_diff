using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DefaultPolicyCreationException : LocalizedException
	{
		public DefaultPolicyCreationException(string moreInfo) : base(Strings.DefaultPolicyCreation(moreInfo))
		{
			this.moreInfo = moreInfo;
		}

		public DefaultPolicyCreationException(string moreInfo, Exception innerException) : base(Strings.DefaultPolicyCreation(moreInfo), innerException)
		{
			this.moreInfo = moreInfo;
		}

		protected DefaultPolicyCreationException(SerializationInfo info, StreamingContext context) : base(info, context)
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
