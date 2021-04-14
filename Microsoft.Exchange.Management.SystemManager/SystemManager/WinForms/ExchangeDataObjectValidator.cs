using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.SnapIn;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ExchangeDataObjectValidator : IDataObjectValidator
	{
		public ValidationError[] Validate(object dataObject)
		{
			IConfigurable configurable = dataObject as IConfigurable;
			if (configurable != null && PSConnectionInfoSingleton.GetInstance().Type != OrganizationType.Cloud)
			{
				return configurable.Validate();
			}
			return new ValidationError[0];
		}
	}
}
