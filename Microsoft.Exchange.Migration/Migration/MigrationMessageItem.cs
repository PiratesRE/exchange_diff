using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationMessageItem : MigrationStoreObject, IMigrationMessageItem, IMigrationStoreObject, IDisposable, IPropertyBag, IReadOnlyPropertyBag, IMigrationAttachmentMessage
	{
		internal MigrationMessageItem(MessageItem message)
		{
			this.Message = message;
		}

		internal MigrationMessageItem(MigrationDataProvider dataProvider, StoreObjectId id)
		{
			this.Initialize(dataProvider, id, MigrationStoreObject.IdPropertyDefinition);
		}

		internal MigrationMessageItem(MigrationDataProvider dataProvider, StoreObjectId id, PropertyDefinition[] propertyDefinitions)
		{
			this.Initialize(dataProvider, id, propertyDefinitions);
		}

		public override string Name
		{
			get
			{
				return this.Message.ClassName;
			}
		}

		protected override StoreObject StoreObject
		{
			get
			{
				return this.Message;
			}
		}

		private MessageItem Message { get; set; }

		public override void OpenAsReadWrite()
		{
			this.Message.OpenAsReadWrite();
		}

		public override void Save(SaveMode saveMode)
		{
			this.Message.Save(saveMode);
		}

		public IMigrationAttachment CreateAttachment(string name)
		{
			base.CheckDisposed();
			return MigrationMessageHelper.CreateAttachment(this.Message, name);
		}

		public bool TryGetAttachment(string name, PropertyOpenMode openMode, out IMigrationAttachment attachment)
		{
			base.CheckDisposed();
			return MigrationMessageHelper.TryGetAttachment(this.Message, name, openMode, out attachment);
		}

		public IMigrationAttachment GetAttachment(string name, PropertyOpenMode openMode)
		{
			base.CheckDisposed();
			return MigrationMessageHelper.GetAttachment(this.Message, name, openMode);
		}

		public void DeleteAttachment(string name)
		{
			base.CheckDisposed();
			MigrationMessageHelper.DeleteAttachment(this.Message, name);
		}

		public override XElement GetDiagnosticInfo(ICollection<PropertyDefinition> properties, MigrationDiagnosticArgument argument)
		{
			base.CheckDisposed();
			XElement diagnosticInfo = base.GetDiagnosticInfo(properties, argument);
			diagnosticInfo.Add(MigrationMessageHelper.GetAttachmentDiagnosticInfo(this.Message, argument));
			return diagnosticInfo;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.Message != null)
			{
				this.Message.Dispose();
				this.Message = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MigrationMessageItem>(this);
		}

		private void Initialize(MigrationDataProvider dataProvider, StoreObjectId id, PropertyDefinition[] properties)
		{
			bool flag = false;
			try
			{
				MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
				MigrationUtil.ThrowOnNullArgument(id, "id");
				MigrationUtil.ThrowOnNullArgument(properties, "properties");
				this.Message = MessageItem.Bind(dataProvider.MailboxSession, id, properties);
				flag = true;
			}
			catch (ArgumentException ex)
			{
				MigrationLogger.Log(MigrationEventType.Error, ex, "Encountered an argument exception when trying to find message with id={0}", new object[]
				{
					id.ToString()
				});
				throw new ObjectNotFoundException(ServerStrings.ExItemNotFound, ex);
			}
			finally
			{
				if (!flag)
				{
					this.Dispose();
				}
			}
		}
	}
}
