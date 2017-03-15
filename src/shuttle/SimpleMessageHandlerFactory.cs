using System;
using System.Collections.Generic;
using System.Reflection;
using Nohros.Collections;
using Nohros.Extensions;
using Shuttle.Esb;
using SimpleInjector;

namespace Nohros.Shuttle.SimpleInjector
{
  /// <summary>
  /// Provides an implementation of the <see cref="IMessageHandlerFactory"/>
  /// class that uses the Simple Injector DI.
  /// </summary>
  /// <seealso cref="Builder"/>
  public partial class SimpleMessageHandlerFactory : MessageHandlerFactory
  {
    readonly Container container_;
    readonly IReadOnlyList<Type> registered_;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="SimpleMessageHandlerFactory"/> by using the given list of
    /// <paramref name="registered"/> types and <paramref name="container"/>.
    /// </summary>
    /// <param name="registered">
    /// A list of types that was previously registered on the given
    /// <paramref name="container"/> and should be resolved as a concrete
    /// <see cref="IMessageHandler{T}"/>.
    /// </param>
    /// <param name="container">
    /// A <seealso cref="Container"/> that can be used to resolve types into
    /// a concrete object.
    /// </param>
    /// <remarks>
    /// Simple Injector does not allow types to be registered after the
    /// container is used for the first time. We cannot ensure that shuttle
    /// will always call the the <see cref="RegisterHandlers"/> before the
    /// container is used, so we will force the message handlers types to be
    /// registered before the container is locked. This way users can follow
    /// the normal registration process. The <see cref="Builder"/> class could
    /// be used automatically register all classes that implements the
    /// <see cref="IMessageHandler{T}"/>.
    /// </remarks>
    /// <seealso cref="Builder"/>
    public SimpleMessageHandlerFactory(Container container,
      IEnumerable<Type> registered) {
      container_ = container;
      registered_ = registered.AsReadOnlyList();
      foreach (Type type in registered_) {
        if (!type.IsMessageHandler()) {
          throw new ArgumentException(R.Arg_ShouldImplementType.Fmt(type,
            typeof (IMessageHandler<>)));
        }
      }
    }

    /// <inheritdoc/>
    public override object CreateHandler(object message) {
      foreach (Type type in registered_) {
        if (type.IsMessageHandler(message.GetType())) {
          return container_.GetInstance(type);
        }
      }
      return null;
    }

    /// <inheritdoc/>
    public override IMessageHandlerFactory RegisterHandlers(Assembly assembly) {
      return this;
    }

    /// <inheritdoc/>
    public override IMessageHandlerFactory RegisterHandler(Type type) {
      return this;
    }

    /// <inheritdoc/>
    public override IEnumerable<Type> MessageTypesHandled {
      get { return registered_; }
    }
  }
}
