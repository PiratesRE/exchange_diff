using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ResPropTypeNotSupportedException : ClusCommonFailException
	{
		public ResPropTypeNotSupportedException(string propType) : base(Strings.ResPropTypeNotSupportedException(propType))
		{
			this.propType = propType;
		}

		public ResPropTypeNotSupportedException(string propType, Exception innerException) : base(Strings.ResPropTypeNotSupportedException(propType), innerException)
		{
			this.propType = propType;
		}

		protected ResPropTypeNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.propType = (string)info.GetValue("propType", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("propType", this.propType);
		}

		public string PropType
		{
			get
			{
				return this.propType;
			}
		}

		private readonly string propType;
	}
}
