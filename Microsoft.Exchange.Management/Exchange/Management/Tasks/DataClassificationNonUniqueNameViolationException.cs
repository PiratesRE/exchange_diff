using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DataClassificationNonUniqueNameViolationException : LocalizedException
	{
		public DataClassificationNonUniqueNameViolationException(string name) : base(Strings.DataClassificationNonUniqueNameViolation(name))
		{
			this.name = name;
		}

		public DataClassificationNonUniqueNameViolationException(string name, Exception innerException) : base(Strings.DataClassificationNonUniqueNameViolation(name), innerException)
		{
			this.name = name;
		}

		protected DataClassificationNonUniqueNameViolationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		private readonly string name;
	}
}
