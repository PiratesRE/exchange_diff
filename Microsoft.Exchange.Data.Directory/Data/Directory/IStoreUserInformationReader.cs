using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal interface IStoreUserInformationReader
	{
		object[] ReadUserInformation(Guid databaseGuid, Guid userInformationGuid, uint[] propertyTags);
	}
}
