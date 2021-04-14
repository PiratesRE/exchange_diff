using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class EnumObject
	{
		public EnumObject(Enum e)
		{
			this.e = e;
		}

		[DataMember]
		public string Description
		{
			get
			{
				return LocalizedDescriptionAttribute.FromEnum(this.e.GetType(), this.e);
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		[DataMember]
		public string Value
		{
			get
			{
				return this.e.ToString();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public override bool Equals(object obj)
		{
			EnumObject enumObject = obj as EnumObject;
			return enumObject != null && enumObject.Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		private Enum e;
	}
}
