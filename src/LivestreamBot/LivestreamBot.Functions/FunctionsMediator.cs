﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using LivestreamBot.Core.DI;

using MediatR;

using Microsoft.Extensions.Configuration;

using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace LivestreamBot.Functions
{
    public static class FunctionsMediator
    {
        private static readonly Container container;
        private static readonly string assemblyPrefix = "LivestreamBot.";

        static FunctionsMediator()
        {
            container = new Container();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            var assemblies = Directory.EnumerateFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), assemblyPrefix + "*.dll")
                .Select(filename => Assembly.Load(AssemblyName.GetAssemblyName(filename)))
                .ToList();

            container.RegisterModules(assemblies);

            container.Verify();
        }

        public static async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken)
        {
            using (AsyncScopedLifestyle.BeginScope(container))
            {
                await container.GetInstance<IMediator>().Publish(notification, cancellationToken);
            }
        }

        public static async Task Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
        {
            using (AsyncScopedLifestyle.BeginScope(container))
            {
                await container.GetInstance<IMediator>().Send(request, cancellationToken);
            }
        }
    }
}
