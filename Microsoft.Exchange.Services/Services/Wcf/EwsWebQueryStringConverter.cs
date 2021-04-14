using System;
using System.ServiceModel.Dispatcher;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class EwsWebQueryStringConverter : QueryStringConverter
	{
		public EwsWebQueryStringConverter(QueryStringConverter originalConverter)
		{
			if (originalConverter == null)
			{
				throw new ArgumentNullException("originalConverter");
			}
			this.originalConverter = originalConverter;
		}

		public override object ConvertStringToValue(string parameter, Type parameterType)
		{
			if (parameterType == typeof(UserPhotoSize))
			{
				return this.ConvertStringToUserPhotoSize(parameter);
			}
			return this.originalConverter.ConvertStringToValue(parameter, parameterType);
		}

		private UserPhotoSize ConvertStringToUserPhotoSize(string value)
		{
			UserPhotoSize result;
			if (!Enum.TryParse<UserPhotoSize>(value, true, out result))
			{
				throw FaultExceptionUtilities.CreateFault(new ServiceArgumentException(CoreResources.IDs.ErrorInvalidPhotoSize, CoreResources.ErrorInvalidPhotoSize), FaultParty.Sender);
			}
			return result;
		}

		private readonly QueryStringConverter originalConverter;
	}
}
