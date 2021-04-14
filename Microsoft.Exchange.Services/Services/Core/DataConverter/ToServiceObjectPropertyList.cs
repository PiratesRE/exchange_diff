using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class ToServiceObjectPropertyList : ToServiceObjectPropertyListBase
	{
		private ToServiceObjectPropertyList()
		{
		}

		public ToServiceObjectPropertyList(Shape shape, ResponseShape responseShape, IParticipantResolver participantResolver) : base(shape, responseShape)
		{
			this.participantResolver = participantResolver;
		}

		public char[] CharBuffer { get; set; }

		public CommandOptions CommandOptions { get; set; }

		protected override ToServiceObjectCommandSettingsBase GetCommandSettings()
		{
			return new ToServiceObjectCommandSettings();
		}

		protected override ToServiceObjectCommandSettingsBase GetCommandSettings(PropertyPath propertyPath)
		{
			return new ToServiceObjectCommandSettings(propertyPath);
		}

		protected override bool ValidateProperty(PropertyInformation propertyInformation, bool returnErrorForInvalidProperty)
		{
			bool implementsToServiceObjectCommand = propertyInformation.ImplementsToServiceObjectCommand;
			if (!implementsToServiceObjectCommand && returnErrorForInvalidProperty)
			{
				throw new InvalidPropertyForOperationException(propertyInformation.PropertyPath);
			}
			return implementsToServiceObjectCommand;
		}

		internal bool IgnoreCorruptPropertiesWhenRendering { get; set; }

		private IParticipantResolver ParticipantResolver
		{
			get
			{
				if (this.participantResolver == null)
				{
					this.participantResolver = StaticParticipantResolver.DefaultInstance;
				}
				return this.participantResolver;
			}
		}

		public IList<IToServiceObjectCommand> CreatePropertyCommands(IdAndSession idAndSession, StoreObject storeObject, ServiceObject serviceObject)
		{
			List<IToServiceObjectCommand> list = new List<IToServiceObjectCommand>();
			foreach (CommandContext commandContext in this.commandContextsOrdered)
			{
				ToServiceObjectCommandSettings toServiceObjectCommandSettings = (ToServiceObjectCommandSettings)commandContext.CommandSettings;
				toServiceObjectCommandSettings.IdAndSession = idAndSession;
				toServiceObjectCommandSettings.StoreObject = storeObject;
				toServiceObjectCommandSettings.ServiceObject = serviceObject;
				toServiceObjectCommandSettings.ResponseShape = this.responseShape;
				toServiceObjectCommandSettings.CommandOptions = this.CommandOptions;
				list.Add((IToServiceObjectCommand)commandContext.PropertyInformation.CreatePropertyCommand(commandContext));
			}
			return list;
		}

		private void ConvertStoreObjectPropertiesToServiceObject(ServiceObject serviceObject, IdAndSession idAndSession, StoreObject storeObject)
		{
			IList<IToServiceObjectCommand> list = this.CreatePropertyCommands(idAndSession, storeObject, serviceObject);
			List<Participant> list2 = new List<Participant>();
			foreach (IToServiceObjectCommand toServiceObjectCommand in list)
			{
				IPregatherParticipants pregatherParticipants = toServiceObjectCommand as IPregatherParticipants;
				if (pregatherParticipants != null)
				{
					pregatherParticipants.Pregather(storeObject, list2);
				}
			}
			this.ParticipantResolver.LoadAdDataIfNeeded(list2);
			foreach (IToServiceObjectCommand toServiceObjectCommand2 in list)
			{
				IRequireCharBuffer requireCharBuffer = toServiceObjectCommand2 as IRequireCharBuffer;
				if (requireCharBuffer != null)
				{
					requireCharBuffer.CharBuffer = this.CharBuffer;
				}
				this.ConvertPropertyCommandToServiceObject(toServiceObjectCommand2);
			}
		}

		public ServiceObject ConvertStoreObjectPropertiesToServiceObject(IdAndSession idAndSession, StoreObject storeObject, ServiceObject serviceObject)
		{
			this.ConvertStoreObjectPropertiesToServiceObject(serviceObject, idAndSession, storeObject);
			return serviceObject;
		}

		protected virtual void ConvertPropertyCommandToServiceObject(IToServiceObjectCommand propertyCommand)
		{
			try
			{
				propertyCommand.ToServiceObject();
			}
			catch (PropertyRequestFailedException ex)
			{
				if (!this.IgnoreCorruptPropertiesWhenRendering)
				{
					throw;
				}
				if (ExTraceGlobals.ServiceCommandBaseDataTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.ServiceCommandBaseDataTracer.TraceError<string, string>((long)this.GetHashCode(), "[ToServiceObjectPropertyList::ConvertStoreObjectPropertiesToServiceObject] Encountered PropertyRequestFailedException.  Message: '{0}'. Data: {1} IgnoreCorruptPropertiesWhenRendering is true, so processing will continue.", ex.Message, ((IProvidePropertyPaths)ex).GetPropertyPathsString());
				}
			}
		}

		public static ToServiceObjectPropertyList Empty = new ToServiceObjectPropertyList();

		private IParticipantResolver participantResolver;
	}
}
