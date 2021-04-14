using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PropertyErrorException : StoragePermanentException
	{
		private PropertyErrorException(params PropertyError[] errors) : base(PropertyErrorException.DescribePropertyErrors(errors))
		{
			this.errors = errors;
		}

		private PropertyErrorException(LocalizedString message, params PropertyError[] errors) : base(LocalizedString.Join(string.Empty, new object[]
		{
			message,
			PropertyErrorException.DescribePropertyErrors(errors)
		}))
		{
			this.errors = errors;
		}

		public PropertyError[] PropertyErrors
		{
			get
			{
				return this.errors;
			}
		}

		internal static PropertyErrorException FromPropertyErrorsInternal(params PropertyError[] errors)
		{
			return new PropertyErrorException(errors);
		}

		internal static PropertyErrorException FromPropertyErrorsInternal(LocalizedString message, params PropertyError[] errors)
		{
			return new PropertyErrorException(message, errors);
		}

		private static LocalizedString DescribePropertyErrors(PropertyError[] errors)
		{
			object[] array = new object[errors.Length];
			for (int i = 0; i < errors.Length; i++)
			{
				array[i] = errors[i].ToLocalizedString();
			}
			return LocalizedString.Join(", ", array);
		}

		private readonly PropertyError[] errors;
	}
}
