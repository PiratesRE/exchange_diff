using System;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal class UnexpectedTypeException : ConversionException
	{
		public UnexpectedTypeException(string expectedType, object actualObject) : this(expectedType, actualObject, true)
		{
		}

		public UnexpectedTypeException(string expectedType, object actualObject, bool sendInformationalWatson) : base(string.Format("Unexpected type: expected = '{0}', actual = '{1}'", expectedType, (actualObject == null) ? "NULL" : actualObject.GetType().ToString()))
		{
			this.SendInformationalWatson = sendInformationalWatson;
		}

		public UnexpectedTypeException(string expectedType, object actualObject, string tagName) : this(expectedType, actualObject, tagName, true)
		{
		}

		public UnexpectedTypeException(string expectedType, object actualObject, string tagName, bool sendInformationalWatson) : base(string.Format("Unexpected type: expected = '{0}', actual = '{1}', tag name = '{2}'", expectedType, (actualObject == null) ? "NULL" : actualObject.GetType().ToString(), tagName))
		{
			this.SendInformationalWatson = sendInformationalWatson;
		}

		public bool SendInformationalWatson { get; set; }
	}
}
