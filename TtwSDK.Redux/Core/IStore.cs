/* TtwSDK.Redux
 * Copyright (c) 2025 terrytw. Licensed under the MIT License.
 * See LICENSE file for details.
 */

namespace TtwSDK.Redux.Core;

/// <summary>
/// Defines the fundamental contract for a Redux store.
/// This non-generic interface allows the <see cref="IDispatcher"/> to manage a heterogeneous collection of stores 
/// and broadcast actions to all of them without knowing their specific state types.
/// </summary>
public interface IStore
{
    /// <summary>
    /// Dispatches an action to the store.
    /// The store will forward this action to its internal reducer to calculate the new state.
    /// </summary>
    /// <param name="action">The action to be processed.</param>
    void Dispatch(IAction action);
}

/// <summary>
/// Defines a read-only contract for a Redux store.
/// Views should depend on this interface to prevent accidental dispatching bypassing the central dispatcher.
/// </summary>
/// <typeparam name="TState">The type of the state.</typeparam>
public interface IReadOnlyStore<out TState>
{
    /// <summary>
    /// Gets the current snapshot of the state.
    /// </summary>
    TState State { get; }

    /// <summary>
    /// Occurs when the state has changed.
    /// </summary>
    event Action<TState>? OnStateChanged;
}

/// <summary>
/// Represents a strongly-typed Redux store that manages a specific type of state.
/// </summary>
/// <typeparam name="TState">The type of the state managed by this store. Ideally, this should be an immutable record or struct.</typeparam>
public interface IStore<TState> : IStore, IReadOnlyStore<TState>
{
}