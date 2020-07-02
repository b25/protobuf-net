﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using ProtoBuf.Formatters;
using ProtoBuf.Meta;
using System;

namespace Microsoft.Extensions.DependencyInjection // guidance is to use this namespace
{
    /// <summary>
    /// Provides support methods for registering protobuf-net with ASP.NET Core
    /// </summary>
    public static class ProtoBufMvcBuilderExtensions
    {
        /// <summary>
        /// Register protobuf-net formatters with a <see cref="IMvcCoreBuilder"/> with all common content-types
        /// </summary>
        public static IMvcCoreBuilder AddProtoBufNet(this IMvcCoreBuilder builder, Action<MvcProtoBufNetOptionsSetup> setupAction = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<MvcOptions>, MvcProtoBufNetOptionsSetup>());
            if (setupAction is object) builder.Services.Configure(setupAction);
            return builder;
        }

        /// <summary>
        /// Register protobuf-net formatters with a <see cref="IMvcBuilder"/> with all common content-types
        /// </summary>
        public static IMvcBuilder AddProtoBufNet(this IMvcBuilder builder, Action<MvcProtoBufNetOptionsSetup> setupAction = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<MvcOptions>, MvcProtoBufNetOptionsSetup>());
            if (setupAction is object) builder.Services.Configure(setupAction);
            return builder;
        }
    }

    /// <summary>
    /// Options for configuring protobuf-net usage inside ASP.NET Core
    /// </summary>
    public sealed class MvcProtoBufNetOptionsSetup : IConfigureOptions<MvcOptions>
    {
        /// <summary>
        /// The type-model to use for serialization and deserialization; if omitted, the default model is assumed
        /// </summary>
        public TypeModel Model { get; set; }

        /// <summary>
        /// The maximum length of payloads to write; no limit by default
        /// </summary>
        public long MaxWriteLength { get; set; } = -1;

        void IConfigureOptions<MvcOptions>.Configure(MvcOptions options)
        {
            var model = Model ?? RuntimeTypeModel.Default;

            var output = new ProtoOutputFormatter(model, MaxWriteLength);
            AddMediaTypes(output.SupportedMediaTypes);
            options.OutputFormatters.Add(output);

            var input = new ProtoInputFormatter(model);
            AddMediaTypes(input.SupportedMediaTypes);
            options.InputFormatters.Add(input);

            options.FormatterMappings.SetMediaTypeMappingForFormat("protobuf", "application/protobuf");

            static void AddMediaTypes(MediaTypeCollection mediaTypes)
            {
                mediaTypes.Add("application/protobuf");
                mediaTypes.Add("application/x-protobuf");
                mediaTypes.Add("application/vnd.google.protobuf");
            }
        }
    }
}
