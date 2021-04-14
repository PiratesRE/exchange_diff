using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DataClassificationFingerprintsMissingException : LocalizedException
	{
		public DataClassificationFingerprintsMissingException(string name) : base(Strings.DataClassificationFingerprintsMissing(name))
		{
			this.name = name;
		}

		public DataClassificationFingerprintsMissingException(string name, Exception innerException) : base(Strings.DataClassificationFingerprintsMissing(name), innerException)
		{
			this.name = name;
		}

		protected DataClassificationFingerprintsMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
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
