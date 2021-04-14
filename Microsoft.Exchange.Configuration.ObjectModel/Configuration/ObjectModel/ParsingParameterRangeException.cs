using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Common.LocStrings;

namespace Microsoft.Exchange.Configuration.ObjectModel
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ParsingParameterRangeException : ParsingException
	{
		public ParsingParameterRangeException(string invalidQuery, int position) : base(Strings.ExceptionParameterRange(invalidQuery, position))
		{
			this.invalidQuery = invalidQuery;
			this.position = position;
		}

		public ParsingParameterRangeException(string invalidQuery, int position, Exception innerException) : base(Strings.ExceptionParameterRange(invalidQuery, position), innerException)
		{
			this.invalidQuery = invalidQuery;
			this.position = position;
		}

		protected ParsingParameterRangeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.invalidQuery = (string)info.GetValue("invalidQuery", typeof(string));
			this.position = (int)info.GetValue("position", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("invalidQuery", this.invalidQuery);
			info.AddValue("position", this.position);
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

		private readonly string invalidQuery;

		private readonly int position;
	}
}
