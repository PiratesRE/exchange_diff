using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AutoAttendantExistsException : LocalizedException
	{
		public AutoAttendantExistsException(string name, string dialplan) : base(Strings.AutoAttendantAlreadyExistsException(name, dialplan))
		{
			this.name = name;
			this.dialplan = dialplan;
		}

		public AutoAttendantExistsException(string name, string dialplan, Exception innerException) : base(Strings.AutoAttendantAlreadyExistsException(name, dialplan), innerException)
		{
			this.name = name;
			this.dialplan = dialplan;
		}

		protected AutoAttendantExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
			this.dialplan = (string)info.GetValue("dialplan", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
			info.AddValue("dialplan", this.dialplan);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string Dialplan
		{
			get
			{
				return this.dialplan;
			}
		}

		private readonly string name;

		private readonly string dialplan;
	}
}
