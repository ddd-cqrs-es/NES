using System;
using System.Collections.Generic;
using System.Linq;

namespace NES
{
    public class UnitOfWork : IUnitOfWork
    {
        private static readonly ILogger Logger = LoggerFactory.Create(typeof(UnitOfWork));
        private readonly ICommandContextProvider _commandContextProvider;
        private readonly IEventSourceMapper _eventSourceMapper;
        private CommandContext _commandContext;
        private readonly HashSet<IEventSource> _eventSources = new HashSet<IEventSource>();

        public UnitOfWork(ICommandContextProvider commandContextProvider, IEventSourceMapper eventSourceMapper)
        {
            _commandContextProvider = commandContextProvider;
            _eventSourceMapper = eventSourceMapper;
        }

        public T Get<T>(Guid id) where T : class, IEventSource
        {
            Logger.Debug("Get event source Id '{0}', Type '{1}'", id, typeof(T).Name);

            var eventSource = _eventSources.OfType<T>().SingleOrDefault(s => s.Id == id) ?? _eventSourceMapper.Get<T>(id);
            
            Register(eventSource);

            return eventSource;
        }

        public void Register<T>(T eventSource) where T : class, IEventSource
        {
            if (eventSource != null)
            {
                Logger.Debug("Register event source Id '{0}', Version '{1}', Type '{2}'", eventSource.Id, eventSource.Version, eventSource.GetType().Name);

                _eventSources.Add(eventSource);
            }

            if (_commandContext == null)
            {
                _commandContext = _commandContextProvider.Get();
            }
        }

        public void Commit()
        {
            Logger.Debug("Commit event sources");

            foreach (var eventSource in _eventSources)
            {
                _eventSourceMapper.Set(_commandContext, eventSource);
            }
        }
    }
}