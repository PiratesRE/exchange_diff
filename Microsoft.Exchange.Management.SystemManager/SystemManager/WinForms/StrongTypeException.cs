using System;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class StrongTypeException : Exception
	{
		public StrongTypeException(string message, bool isTargetProperty) : base(message)
		{
			this.isTargetProperty = isTargetProperty;
		}

		public bool IsTargetProperty
		{
			get
			{
				return this.isTargetProperty;
			}
		}

		private bool isTargetProperty;
	}
}
