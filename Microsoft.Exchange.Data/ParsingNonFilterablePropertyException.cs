using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ParsingNonFilterablePropertyException : ParsingException
	{
		public ParsingNonFilterablePropertyException(string propertyName, string invalidQuery, int position) : base(DataStrings.ExceptionParseNonFilterablePropertyError(propertyName, invalidQuery, position))
		{
			this.propertyName = propertyName;
			this.invalidQuery = invalidQuery;
			this.position = position;
		}

		public ParsingNonFilterablePropertyException(string propertyName, string invalidQuery, int position, Exception innerException) : base(DataStrings.ExceptionParseNonFilterablePropertyError(propertyName, invalidQuery, position), innerException)
		{
			this.propertyName = propertyName;
			this.invalidQuery = invalidQuery;
			this.position = position;
		}

		protected ParsingNonFilterablePropertyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.propertyName = (string)info.GetValue("propertyName", typeof(string));
			this.invalidQuery = (string)info.GetValue("invalidQuery", typeof(string));
			this.position = (int)info.GetValue("position", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("propertyName", this.propertyName);
			info.AddValue("invalidQuery", this.invalidQuery);
			info.AddValue("position", this.position);
		}

		public string PropertyName
		{
			get
			{
				return this.propertyName;
			}
		}

		public string InvalidQuery
		{
			get
			{
				return this.invalidQuery;
			}
		}

		public int Position
		{
			get
			{
				return this.position;
			}
		}

		private readonly string propertyName;

		private readonly string invalidQuery;

		private readonly int position;
	}
}
