using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DataConversionException : DataValidationException
	{
		public new PropertyConversionError Error
		{
			get
			{
				return (PropertyConversionError)base.Error;
			}
		}

		public DataConversionException(PropertyConversionError error) : base(error, error.Exception)
		{
		}

		protected DataConversionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
