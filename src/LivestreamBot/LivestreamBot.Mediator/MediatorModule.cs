using LivestreamBot.Core.Modules;
using LivestreamBot.Mediator.Pipeline;

using MediatR;
using MediatR.Pipeline;

using SimpleInjector;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace LivestreamBot.Mediator
{
    public class MediatorModule : IModule
    {

        public void Register(Container container, IList<Assembly> assemblies)
        {
            container.RegisterSingleton<IMediator, MediatR.Mediator>();
            container.Register(typeof(IRequestHandler<,>), assemblies);

            RegisterHandlers(container, typeof(INotificationHandler<>), assemblies);
            RegisterHandlers(container, typeof(IRequestExceptionAction<,>), assemblies);
            RegisterHandlers(container, typeof(IRequestExceptionHandler<,,>), assemblies);

            //Pipeline
            container.Collection.Register(typeof(IPipelineBehavior<,>), new[]
            {
                typeof(GenericPipelineBehavior<,>)
                
            });

            container.Register(() => new ServiceFactory(container.GetInstance), Lifestyle.Singleton);
        }

        private static void RegisterHandlers(Container container, Type collectionType, IList<Assembly> assemblies)
        {
            // we have to do this because by default, generic type definitions (such as the Constrained Notification Handler) won't be registered
            var handlerTypes = container.GetTypesToRegister(collectionType, assemblies, new TypesToRegisterOptions
            {
                IncludeGenericTypeDefinitions = true,
                IncludeComposites = false,
            });

            container.Collection.Register(collectionType, handlerTypes);
        }
    }
}
