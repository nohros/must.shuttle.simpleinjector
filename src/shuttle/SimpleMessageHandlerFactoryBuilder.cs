using System;
using System.Collections.Generic;
using Nohros.Configuration;
using Shuttle.Esb;
using SimpleInjector;

namespace Nohros.Shuttle.SimpleInjector
{
  public partial class SimpleMessageHandlerFactory
  {
    /// <summary>
    /// Provides a way to buid a <see cref="SimpleMessageHandlerFactory"/> and
    /// automatically register all implementations of the
    /// <see cref="IMessageHandler{T}"/>.
    /// </summary>
    public class Builder
    {
      readonly Container container_;
      readonly AssemblyScanner scanner_;

      /// <summary>
      /// Initializes a new instance of the <see cref="Builder"/> class by
      /// using the given DI <paramref name="container"/> and an
      /// <see cref="AssemblyScanner"/> that will scan all the assemblies of
      /// the application's running directory to find implementations of the
      /// <see cref="IMessageHandler{T}"/>.
      /// </summary>
      /// <param name="container">
      /// A unlocked <see cref="Container"/> that can be used to register new
      /// types.
      /// </param>
      public Builder(Container container)
        : this(container, new AssemblyScanner.Builder().Build()) {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="Builder"/> class by
      /// using the given DI <paramref name="container"/> and assembly
      /// <paramref name="scanner"/>.
      /// </summary>
      /// <param name="container">
      /// A unlocked <see cref="Container"/> that can be used to register new
      /// types.
      /// </param>
      /// <param name="scanner">
      /// A <see cref="AssemblyScanner"/> that can be used to retrieve a list
      /// of assemblies that should be scanned to find implementations of the
      /// <see cref="IMessageHandler{T}"/>.
      /// </param>
      public Builder(Container container, AssemblyScanner scanner) {
        container_ = container;
        scanner_ = scanner;
      }

      public SimpleMessageHandlerFactory Build() {
        var types = scanner_.GetScannableAssemblies().Types;

        // register all types that implements the [IMessageHandler<>] interface
        // into the DI container.
        var handlers = new HashSet<Type>();
        foreach (Type type in types) {
          if (type.IsMessageHandler() && !type.IsInterface) {
            handlers.Add(type);
            container_.Register(type);
          }
        }
        return new SimpleMessageHandlerFactory(container_, handlers);
      }
    }
  }
}
