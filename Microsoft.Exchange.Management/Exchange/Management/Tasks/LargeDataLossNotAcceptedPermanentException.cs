using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LargeDataLossNotAcceptedPermanentException : MailboxReplicationPermanentException
	{
		public LargeDataLossNotAcceptedPermanentException(string badItemLimitParamName, string badItemLimitValue, string acceptLargeDataLossParamName, string requestorIdentity) : base(Strings.ErrorLargeDataLossNotAccepted(badItemLimitParamName, badItemLimitValue, acceptLargeDataLossParamName, requestorIdentity))
		{
			this.badItemLimitParamName = badItemLimitParamName;
			this.badItemLimitValue = badItemLimitValue;
			this.acceptLargeDataLossParamName = acceptLargeDataLossParamName;
			this.requestorIdentity = requestorIdentity;
		}

		public LargeDataLossNotAcceptedPermanentException(string badItemLimitParamName, string badItemLimitValue, string acceptLargeDataLossParamName, string requestorIdentity, Exception innerException) : base(Strings.ErrorLargeDataLossNotAccepted(badItemLimitParamName, badItemLimitValue, acceptLargeDataLossParamName, requestorIdentity), innerException)
		{
			this.badItemLimitParamName = badItemLimitParamName;
			this.badItemLimitValue = badItemLimitValue;
			this.acceptLargeDataLossParamName = acceptLargeDataLossParamName;
			this.requestorIdentity = requestorIdentity;
		}

		protected LargeDataLossNotAcceptedPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.badItemLimitParamName = (string)info.GetValue("badItemLimitParamName", typeof(string));
			this.badItemLimitValue = (string)info.GetValue("badItemLimitValue", typeof(string));
			this.acceptLargeDataLossParamName = (string)info.GetValue("acceptLargeDataLossParamName", typeof(string));
			this.requestorIdentity = (string)info.GetValue("requestorIdentity", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("badItemLimitParamName", this.badItemLimitParamName);
			info.AddValue("badItemLimitValue", this.badItemLimitValue);
			info.AddValue("acceptLargeDataLossParamName", this.acceptLargeDataLossParamName);
			info.AddValue("requestorIdentity", this.requestorIdentity);
		}

		public string BadItemLimitParamName
		{
			get
			{
				return this.badItemLimitParamName;
			}
		}

		public string BadItemLimitValue
		{
			get
			{
				return this.badItemLimitValue;
			}
		}

		public string AcceptLargeDataLossParamName
		{
			get
			{
				return this.acceptLargeDataLossParamName;
			}
		}

		public string RequestorIdentity
		{
			get
			{
				return this.requestorIdentity;
			}
		}

		private readonly string badItemLimitParamName;

		private readonly string badItemLimitValue;

		private readonly string acceptLargeDataLossParamName;

		private readonly string requestorIdentity;
	}
}
