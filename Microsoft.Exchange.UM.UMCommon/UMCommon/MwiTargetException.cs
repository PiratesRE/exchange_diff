using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MwiTargetException : MwiDeliveryException
	{
		public MwiTargetException(string targetName, int responseCode, string responseText) : base(Strings.UMRpcError(targetName, responseCode, responseText))
		{
			this.targetName = targetName;
			this.responseCode = responseCode;
			this.responseText = responseText;
		}

		public MwiTargetException(string targetName, int responseCode, string responseText, Exception innerException) : base(Strings.UMRpcError(targetName, responseCode, responseText), innerException)
		{
			this.targetName = targetName;
			this.responseCode = responseCode;
			this.responseText = responseText;
		}

		protected MwiTargetException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.targetName = (string)info.GetValue("targetName", typeof(string));
			this.responseCode = (int)info.GetValue("responseCode", typeof(int));
			this.responseText = (string)info.GetValue("responseText", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("targetName", this.targetName);
			info.AddValue("responseCode", this.responseCode);
			info.AddValue("responseText", this.responseText);
		}

		public string TargetName
		{
			get
			{
				return this.targetName;
			}
		}

		public int ResponseCode
		{
			get
			{
				return this.responseCode;
			}
		}

		public string ResponseText
		{
			get
			{
				return this.responseText;
			}
		}

		private readonly string targetName;

		private readonly int responseCode;

		private readonly string responseText;
	}
}
