/* TtwSDK.Redux
 * Copyright (c) 2025 terrytw. Licensed under the MIT License.
 * See LICENSE file for details.
 */

namespace TtwSDK.Redux.Core;

/// <summary>
/// The default implementation of the <see cref="IDispatcher"/>.
/// Acts as a central hub (Middleware) that manages a list of stores and broadcasts pure <see cref="IAction"/> to all registered stores.
/// </summary>
public class Dispatcher : IDispatcher
{
    private readonly List<IStore> _stores = new();

    /// <summary>
    /// Registers a store to receive broadcasted actions.
    /// If the store is already registered, it will not be added again.
    /// </summary>
    /// <param name="store">The store instance to register.</param>
    public void AddStore(IStore store)
    {
        if (!_stores.Contains(store))
        {
            _stores.Add(store);
        }
    }

    /// <summary>
    /// Unregisters a store so it no longer receives broadcasted actions.
    /// </summary>
    /// <param name="store">The store instance to remove.</param>
    public void RemoveStore(IStore store)
    {
        _stores.Remove(store);
    }

    /// <summary>
    /// Synchronously broadcasts the given action to all registered stores.
    /// </summary>
    /// <param name="action">The action to dispatch.</param>
    public void Dispatch(IAction action)
    {
        foreach (var store in _stores)
        {
            store.Dispatch(action);
        }
    }

    /// <summary>
    /// Retrieves the current state of a specific type from the registered stores.
    /// </summary>
    /// <typeparam name="TState">The type of the state to retrieve.</typeparam>
    /// <returns>The current state object.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no store is registered for the specified state type.</exception>
    public TState GetState<TState>()
    {
        var store = _stores.OfType<IStore<TState>>().FirstOrDefault();
        if (store == null)
        {
            throw new InvalidOperationException($"No store registered for state type {typeof(TState).Name}");
        }
        return store.State;
    }
}