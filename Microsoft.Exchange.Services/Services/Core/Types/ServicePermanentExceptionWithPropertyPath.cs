using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal abstract class ServicePermanentExceptionWithPropertyPath : ServicePermanentException, IProvidePropertyPaths
	{
		public ServicePermanentExceptionWithPropertyPath(ResponseCodeType responseCode, Enum messageId, Exception innerException) : base(responseCode, messageId, innerException)
		{
		}

		public ServicePermanentExceptionWithPropertyPath(ResponseCodeType responseCode, Enum messageId) : base(responseCode, messageId)
		{
		}

		public ServicePermanentExceptionWithPropertyPath(ResponseCodeType responseCode, LocalizedString message, PropertyPath propertyPath) : base(responseCode, message)
		{
			this.propertyPaths.Add(propertyPath);
		}

		public ServicePermanentExceptionWithPropertyPath(ResponseCodeType responseCode, Enum messageId, PropertyPath propertyPath) : base(responseCode, messageId)
		{
			this.propertyPaths.Add(propertyPath);
		}

		public ServicePermanentExceptionWithPropertyPath(Enum messageId, PropertyPath propertyPath) : base(messageId)
		{
			this.propertyPaths.Add(propertyPath);
		}

		public ServicePermanentExceptionWithPropertyPath(Enum messageId, PropertyPath propertyPath, Exception innerException) : base(messageId, innerException)
		{
			this.propertyPaths.Add(propertyPath);
		}

		public ServicePermanentExceptionWithPropertyPath(ResponseCodeType responseCode, Enum messageId, PropertyPath propertyPath, Exception innerException) : base(responseCode, messageId, innerException)
		{
			this.propertyPaths.Add(propertyPath);
		}

		public ServicePermanentExceptionWithPropertyPath(Enum messageId, PropertyPath[] propertyPaths, Exception innerException) : base(messageId, innerException)
		{
			foreach (PropertyPath item in propertyPaths)
			{
				this.propertyPaths.Add(item);
			}
		}

		PropertyPath[] IProvidePropertyPaths.PropertyPaths
		{
			get
			{
				return this.propertyPaths.ToArray();
			}
		}

		string IProvidePropertyPaths.GetPropertyPathsString()
		{
			if (this.propertyPaths == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			foreach (PropertyPath arg in this.propertyPaths)
			{
				if (num == this.propertyPaths.Count - 1)
				{
					stringBuilder.AppendFormat("'{0}'", arg);
				}
				else
				{
					stringBuilder.AppendFormat("'{0}', ", arg);
				}
				num++;
			}
			return stringBuilder.ToString();
		}

		private List<PropertyPath> propertyPaths = new List<PropertyPath>();
	}
}
