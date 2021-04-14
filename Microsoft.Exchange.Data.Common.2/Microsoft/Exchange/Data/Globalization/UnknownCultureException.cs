using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.Globalization
{
	[Serializable]
	public class UnknownCultureException : ExchangeDataException
	{
		public UnknownCultureException(int localeId) : base(GlobalizationStrings.InvalidLocaleId(localeId))
		{
			this.localeId = localeId;
		}

		public UnknownCultureException(string cultureName) : base(GlobalizationStrings.InvalidCultureName((cultureName == null) ? "<null>" : cultureName))
		{
			this.cultureName = cultureName;
		}

		public UnknownCultureException(int localeId, string message) : base(message)
		{
			this.localeId = localeId;
		}

		public UnknownCultureException(string cultureName, string message) : base(message)
		{
			this.cultureName = cultureName;
		}

		public UnknownCultureException(int localeId, string message, Exception innerException) : base(message, innerException)
		{
			this.localeId = localeId;
		}

		public UnknownCultureException(string cultureName, string message, Exception innerException) : base(message, innerException)
		{
			this.cultureName = cultureName;
		}

		protected UnknownCultureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.localeId = info.GetInt32("localeId");
			this.cultureName = info.GetString("cultureName");
		}

		public int LocaleId
		{
			get
			{
				return this.localeId;
			}
		}

		public string CultureName
		{
			get
			{
				return this.cultureName;
			}
		}

		private int localeId;

		private string cultureName;
	}
}
