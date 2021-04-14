using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ServerResolverRow : AdObjectResolverRow
	{
		public ServerResolverRow(ADRawEntry aDRawEntry) : base(aDRawEntry)
		{
			this.OperationalState = Strings.No;
		}

		[DataMember]
		public string Name
		{
			get
			{
				return (string)base.ADRawEntry[ADObjectSchema.Name];
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string OperationalState { get; set; }

		[DataMember]
		public string ServerRole
		{
			get
			{
				ServerRole serverRole = (ServerRole)base.ADRawEntry[ExchangeServerSchema.CurrentServerRole];
				if (!(bool)base.ADRawEntry[ExchangeServerSchema.IsE15OrLater])
				{
					return serverRole.ToString();
				}
				return ExchangeServer.ConvertE15ServerRoleToOutput(serverRole).ToString();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Site
		{
			get
			{
				return ((ADObjectId)base.ADRawEntry[ExchangeServerSchema.Site]).ToString();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string AdminDisplayVersion
		{
			get
			{
				ServerVersion serverVersion = (ServerVersion)base.ADRawEntry[ExchangeServerSchema.AdminDisplayVersion];
				return serverVersion.ToString();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public new static PropertyDefinition[] Properties = new List<PropertyDefinition>(AdObjectResolverRow.Properties)
		{
			ADObjectSchema.Name,
			ExchangeServerSchema.CurrentServerRole,
			ExchangeServerSchema.IsE15OrLater,
			ExchangeServerSchema.Site,
			ExchangeServerSchema.AdminDisplayVersion
		}.ToArray();
	}
}
