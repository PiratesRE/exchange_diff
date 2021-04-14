using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RmsSharedIdentityTooManyUsersException : LocalizedException
	{
		public RmsSharedIdentityTooManyUsersException(string firstDn, string secondDn) : base(Strings.RmsSharedIdentityTooManyUsers(firstDn, secondDn))
		{
			this.firstDn = firstDn;
			this.secondDn = secondDn;
		}

		public RmsSharedIdentityTooManyUsersException(string firstDn, string secondDn, Exception innerException) : base(Strings.RmsSharedIdentityTooManyUsers(firstDn, secondDn), innerException)
		{
			this.firstDn = firstDn;
			this.secondDn = secondDn;
		}

		protected RmsSharedIdentityTooManyUsersException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.firstDn = (string)info.GetValue("firstDn", typeof(string));
			this.secondDn = (string)info.GetValue("secondDn", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("firstDn", this.firstDn);
			info.AddValue("secondDn", this.secondDn);
		}

		public string FirstDn
		{
			get
			{
				return this.firstDn;
			}
		}

		public string SecondDn
		{
			get
			{
				return this.secondDn;
			}
		}

		private readonly string firstDn;

		private readonly string secondDn;
	}
}
