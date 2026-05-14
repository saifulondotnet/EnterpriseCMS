namespace EnterpriseCMS.Plugins.Core;

public interface ICmsEvent { string EventName { get; } }
public interface IEventDispatcher
{
    Task DispatchAsync<TEvent>(TEvent evt, CancellationToken ct = default) where TEvent : ICmsEvent;
}
