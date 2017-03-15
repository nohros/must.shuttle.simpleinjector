using System;
using Shuttle.Esb;

namespace Nohros.Shuttle.SimpleInjector
{
  internal static class MessageHandlers
  {
    static readonly Type msg_type_ = typeof (IMessageHandler<>);

    public static bool IsMessageHandler(this Type type) {
      var interfaces = type.GetInterfaces();

      foreach (Type t in interfaces) {
        if (t.IsGenericType && t.GetGenericTypeDefinition() == msg_type_) {
          return true;
        }
      }

      Type base_type = type.BaseType;
      if (base_type == null) {
        return false;
      }
      return IsMessageHandler(base_type);
    }

    public static bool IsMessageHandler(this Type type, Type t_type) {
      var interfaces = type.GetInterfaces();

      foreach (Type t in interfaces) {
        if (t.IsGenericType && t.GetGenericTypeDefinition() == msg_type_) {
          Type[] args = t.GetGenericArguments();
          if (args.Length == 1 && args[0] == t_type) {
            return true;
          }
          return false;
        }
      }

      Type base_type = type.BaseType;
      if (base_type == null) {
        return false;
      }
      return IsMessageHandler(base_type, t_type);
    }
  }
}
