using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ParsingInvalidFilterOperator : ParsingException
	{
		public ParsingInvalidFilterOperator(string op, string invalidQuery, int position) : base(DataStrings.ExceptionInvalidFilterOperator(op, invalidQuery, position))
		{
			this.op = op;
			this.invalidQuery = invalidQuery;
			this.position = position;
		}

		public ParsingInvalidFilterOperator(string op, string invalidQuery, int position, Exception innerException) : base(DataStrings.ExceptionInvalidFilterOperator(op, invalidQuery, position), innerException)
		{
			this.op = op;
			this.invalidQuery = invalidQuery;
			this.position = position;
		}

		protected ParsingInvalidFilterOperator(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.op = (string)info.GetValue("op", typeof(string));
			this.invalidQuery = (string)info.GetValue("invalidQuery", typeof(string));
			this.position = (int)info.GetValue("position", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("op", this.op);
			info.AddValue("invalidQuery", this.invalidQuery);
			info.AddValue("position", this.position);
		}

		public string Op
		{
			get
			{
				return this.op;
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

		private readonly string op;

		private readonly string invalidQuery;

		private readonly int position;
	}
}
