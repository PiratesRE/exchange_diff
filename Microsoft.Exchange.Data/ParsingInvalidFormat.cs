using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ParsingInvalidFormat : ParsingException
	{
		public ParsingInvalidFormat(string token, Type type, string invalidQuery, int position) : base(DataStrings.ExceptionInvalidFormat(token, type, invalidQuery, position))
		{
			this.token = token;
			this.type = type;
			this.invalidQuery = invalidQuery;
			this.position = position;
		}

		public ParsingInvalidFormat(string token, Type type, string invalidQuery, int position, Exception innerException) : base(DataStrings.ExceptionInvalidFormat(token, type, invalidQuery, position), innerException)
		{
			this.token = token;
			this.type = type;
			this.invalidQuery = invalidQuery;
			this.position = position;
		}

		protected ParsingInvalidFormat(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.token = (string)info.GetValue("token", typeof(string));
			this.type = (Type)info.GetValue("type", typeof(Type));
			this.invalidQuery = (string)info.GetValue("invalidQuery", typeof(string));
			this.position = (int)info.GetValue("position", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("token", this.token);
			info.AddValue("type", this.type);
			info.AddValue("invalidQuery", this.invalidQuery);
			info.AddValue("position", this.position);
		}

		public string Token
		{
			get
			{
				return this.token;
			}
		}

		public Type Type
		{
			get
			{
				return this.type;
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

		private readonly string token;

		private readonly Type type;

		private readonly string invalidQuery;

		private readonly int position;
	}
}
