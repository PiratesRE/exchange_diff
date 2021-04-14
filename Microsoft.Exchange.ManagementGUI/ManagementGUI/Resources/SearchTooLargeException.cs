using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.ManagementGUI.Resources
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SearchTooLargeException : LocalizedException
	{
		public SearchTooLargeException(int max) : base(Strings.SearchTooLargeError(max))
		{
			this.max = max;
		}

		public SearchTooLargeException(int max, Exception innerException) : base(Strings.SearchTooLargeError(max), innerException)
		{
			this.max = max;
		}

		protected SearchTooLargeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.max = (int)info.GetValue("max", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("max", this.max);
		}

		public int Max
		{
			get
			{
				return this.max;
			}
		}

		private readonly int max;
	}
}
