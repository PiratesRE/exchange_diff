using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class UserConfigurationDescriptor
	{
		private UserConfigurationDescriptor(string configName, UserConfigurationTypes configurationTypes)
		{
			this.configurationName = configName;
			this.configurationTypes = configurationTypes;
		}

		public string ConfigurationName
		{
			get
			{
				return this.configurationName;
			}
		}

		public UserConfigurationTypes Types
		{
			get
			{
				return this.configurationTypes;
			}
		}

		public static UserConfigurationDescriptor CreateMailboxDescriptor(string configName, UserConfigurationTypes types)
		{
			return new UserConfigurationDescriptor.DefaultFolderDescriptor(configName, types, DefaultFolderType.Configuration);
		}

		public static UserConfigurationDescriptor CreateDefaultFolderDescriptor(string configName, UserConfigurationTypes types, DefaultFolderType folderType)
		{
			return new UserConfigurationDescriptor.DefaultFolderDescriptor(configName, types, folderType);
		}

		public static UserConfigurationDescriptor CreateFolderDescriptor(string configName, UserConfigurationTypes types, StoreObjectId folderId)
		{
			return new UserConfigurationDescriptor.FolderIdDescriptor(configName, types, folderId);
		}

		public static UserConfigurationDescriptor FromMemento(UserConfigurationDescriptor.MementoClass memento)
		{
			return new UserConfigurationDescriptor.FolderIdDescriptor(memento.ConfigurationName, memento.Types, memento.FolderId);
		}

		public UserConfigurationDescriptor.MementoClass ToMemento(IMailboxSession session)
		{
			return new UserConfigurationDescriptor.MementoClass
			{
				ConfigurationName = this.ConfigurationName,
				Types = this.Types,
				FolderId = this.GetFolderId(session)
			};
		}

		public abstract StoreObjectId GetFolderId(IMailboxSession session);

		public abstract void Validate(IUserConfiguration config);

		public IUserConfiguration GetConfiguration(IUserConfigurationManager manager)
		{
			IUserConfiguration result = null;
			StoreObjectId folderId = this.GetFolderId(manager.MailboxSession);
			if (folderId != null)
			{
				result = manager.GetFolderConfiguration(this.ConfigurationName, this.Types, folderId);
			}
			return result;
		}

		public IUserConfiguration Rebuild(IUserConfigurationManager manager)
		{
			IUserConfiguration result = null;
			StoreId folderId = this.GetFolderId(manager.MailboxSession);
			if (folderId != null)
			{
				manager.DeleteFolderConfigurations(folderId, new string[]
				{
					this.ConfigurationName
				});
				result = manager.CreateFolderConfiguration(this.ConfigurationName, this.Types, folderId);
			}
			return result;
		}

		private readonly string configurationName;

		private readonly UserConfigurationTypes configurationTypes;

		[DataContract]
		public class MementoClass : IEquatable<UserConfigurationDescriptor.MementoClass>
		{
			[DataMember]
			public string ConfigurationName { get; set; }

			[DataMember]
			public StoreObjectId FolderId { get; set; }

			[DataMember]
			public UserConfigurationTypes Types { get; set; }

			public bool Equals(UserConfigurationDescriptor.MementoClass other)
			{
				return !object.ReferenceEquals(other, null) && ((this.ConfigurationName ?? string.Empty).Equals(other.ConfigurationName) && this.Types.Equals(other.Types)) && ((this.FolderId == null && other.FolderId == null) || this.FolderId.Equals(other.FolderId));
			}

			public bool IsSuperSetOf(UserConfigurationDescriptor.MementoClass other)
			{
				return !object.ReferenceEquals(other, null) && ((this.ConfigurationName ?? string.Empty).Equals(other.ConfigurationName) && ((this.FolderId == null && other.FolderId == null) || this.FolderId.Equals(other.FolderId))) && (this.Types & other.Types) == other.Types;
			}

			public override bool Equals(object obj)
			{
				return this.Equals(obj as UserConfigurationDescriptor.MementoClass);
			}

			public override int GetHashCode()
			{
				return (this.ConfigurationName ?? string.Empty).GetHashCode() ^ this.Types.GetHashCode() ^ ((this.FolderId == null) ? 0 : this.FolderId.GetHashCode());
			}
		}

		public class UserConfigurationDescriptorEqualityComparer : IEqualityComparer<UserConfigurationDescriptor>
		{
			public UserConfigurationDescriptorEqualityComparer(IMailboxSession session)
			{
				this.session = session;
			}

			public bool Equals(UserConfigurationDescriptor x, UserConfigurationDescriptor y)
			{
				return object.ReferenceEquals(x, y) || (!object.ReferenceEquals(x, null) && (string.Equals(x.ConfigurationName, y.ConfigurationName, StringComparison.OrdinalIgnoreCase) && x.Types == y.Types) && object.Equals(x.GetFolderId(this.session), y.GetFolderId(this.session)));
			}

			public int GetHashCode(UserConfigurationDescriptor obj)
			{
				if (obj == null)
				{
					return 0;
				}
				return obj.ConfigurationName.GetHashCode() ^ obj.Types.GetHashCode();
			}

			private readonly IMailboxSession session;
		}

		private class DefaultFolderDescriptor : UserConfigurationDescriptor
		{
			public DefaultFolderDescriptor(string configName, UserConfigurationTypes types, DefaultFolderType folderType) : base(configName, types)
			{
				this.folderType = folderType;
			}

			public override void Validate(IUserConfiguration config)
			{
				if ((base.Types & UserConfigurationTypes.Dictionary) != (UserConfigurationTypes)0)
				{
					config.GetDictionary();
				}
			}

			public override StoreObjectId GetFolderId(IMailboxSession session)
			{
				return session.GetDefaultFolderId(this.folderType);
			}

			private readonly DefaultFolderType folderType;
		}

		private class FolderIdDescriptor : UserConfigurationDescriptor
		{
			public FolderIdDescriptor(string configName, UserConfigurationTypes types, StoreObjectId folderId) : base(configName, types)
			{
				this.folderId = folderId;
			}

			public override void Validate(IUserConfiguration config)
			{
			}

			public override StoreObjectId GetFolderId(IMailboxSession session)
			{
				return this.folderId;
			}

			private readonly StoreObjectId folderId;
		}
	}
}
