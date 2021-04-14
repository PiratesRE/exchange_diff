using System;

namespace Microsoft.Exchange.Management.ControlPanel.DDI
{
	public class LambdaExpressionException : Exception
	{
		public LambdaExpressionException(string errorExpression, Exception innerException) : base("\r\nSystem throws an exception at calculating Lambda Expression : [ " + errorExpression + " ]\r\n" + innerException.Message, innerException)
		{
		}
	}
}
