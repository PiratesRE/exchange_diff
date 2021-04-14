using System;
using System.IO;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Inference.PeopleRelevance
{
	internal class MdbPeopleModelDataBinder : MdbPeopleBaseModelDataBinder<PeopleModelItem>
	{
		public MdbPeopleModelDataBinder(MailboxSession session) : base("Inference.PeopleModel", session)
		{
		}

		protected override Version MinimumSupportedVersion
		{
			get
			{
				return PeopleModelItem.MinimumSupportedVersion;
			}
		}

		internal override Stream GetModelStreamFromUserConfig(UserConfiguration config)
		{
			return config.GetXmlStream();
		}

		protected override UserConfigurationTypes GetUserConfigurationType()
		{
			return UserConfigurationTypes.XML;
		}

		protected override void WriteModelData(DataContractSerializer serializer, Stream stream, PeopleModelItem modelData)
		{
			serializer.WriteObject(stream, modelData);
			stream.SetLength(stream.Position);
		}

		protected override PeopleModelItem ReadModelData(DataContractSerializer serializer, Stream stream)
		{
			return serializer.ReadObject(stream) as PeopleModelItem;
		}
	}
}
