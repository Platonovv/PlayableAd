using System;
using System.Collections.Generic;

namespace Project.Core
{
    /// <summary>
    /// Типизированная шина сигналов: декаплинг геймплея от UI, звука и аналитики без реактивных фреймворков.
    /// </summary>
    public sealed class SignalBus
    {
        private readonly Dictionary<Type, Delegate> _handlers = new Dictionary<Type, Delegate>();

        public void Subscribe<T>(Action<T> handler) where T : struct
        {
            _handlers.TryGetValue(typeof(T), out var existing);
            _handlers[typeof(T)] = (Action<T>)existing + handler;
        }

        public void Unsubscribe<T>(Action<T> handler) where T : struct
        {
            if (!_handlers.TryGetValue(typeof(T), out var existing)) return;
            var updated = (Action<T>)existing - handler;
            if (updated == null) _handlers.Remove(typeof(T));
            else _handlers[typeof(T)] = updated;
        }

        public void Fire<T>(T signal) where T : struct
        {
            if (_handlers.TryGetValue(typeof(T), out var existing))
                ((Action<T>)existing)?.Invoke(signal);
        }
    }
}
