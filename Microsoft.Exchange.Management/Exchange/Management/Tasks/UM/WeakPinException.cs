using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class WeakPinException : LocalizedException
	{
		public WeakPinException(string details) : base(Strings.ErrorWeakPassword(details))
		{
			this.details = details;
		}

		public WeakPinException(string details, Exception innerException) : base(Strings.ErrorWeakPassword(details), innerException)
		{
			this.details = details;
		}

		protected WeakPinException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.details = (string)info.GetValue("details", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("details", this.details);
		}

		public string Details
		{
			get
			{
				return this.details;
			}
		}

		private readonly string details;
	}
}
