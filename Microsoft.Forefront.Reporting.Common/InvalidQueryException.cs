using System;

namespace Microsoft.Forefront.Reporting.Common
{
	public class InvalidQueryException : ReportingException
	{
		internal InvalidQueryException()
		{
			this.ErrorCode = InvalidQueryException.InvalidQueryErrorCode.InvalidQueryFormat;
		}

		internal InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode errorCode) : base(errorCode.ToString())
		{
			this.ErrorCode = errorCode;
			this.Position = 0;
			this.ErrorProperty = string.Empty;
		}

		internal InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode errorCode, string propertyName, int position) : base(string.Format("Error:{0} PropertyName:{1} Position:{2}", errorCode, propertyName, position))
		{
			this.ErrorCode = errorCode;
			this.ErrorProperty = propertyName;
			this.Position = position;
		}

		internal InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode errorCode, string propertyName, int position, Exception innerException) : base(string.Format("Error:{0} PropertyName:{1} Position:{2}", errorCode, propertyName, position), innerException)
		{
			this.ErrorCode = errorCode;
			this.ErrorProperty = propertyName;
			this.Position = position;
		}

		public InvalidQueryException.InvalidQueryErrorCode ErrorCode { get; internal set; }

		public string ErrorProperty { get; internal set; }

		public int Position { get; internal set; }

		public enum InvalidQueryErrorCode
		{
			InvalidQueryFormat,
			EmptySearchDefinition,
			MissingRequiredProperty,
			MissingQuote,
			InvalidValue,
			UnsupportedValue,
			ValueOutRange,
			InvalidProperty,
			UnsupportedProperty,
			InvalidGrouper,
			UnpairedParenthese,
			DoesNotMeetMinimalQueryRequirement,
			DuplicateField,
			InvalidQueryType,
			ToManyPropertiesInGroup,
			ToManyGroups
		}
	}
}
