/* Project Folio Lite (ProjFolioLite)
 * Copyright (c) 2025 terrytw. All Rights Reserved.
 *
 * This source code is for portfolio and demonstration purposes only.
 * Unauthorized copying, modification, or distribution is strictly prohibited.
 * See LICENSE file in the project root for full terms and conditions.
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
/// Represents a strongly-typed Redux store that manages a specific type of state.
/// </summary>
/// <typeparam name="TState">The type of the state managed by this store. Ideally, this should be an immutable record or struct.</typeparam>
public interface IStore<TState> : IStore
{
    /// <summary>
    /// Gets the current snapshot of the state.
    /// This property provides read-only access to the state for Views or Selectors.
    /// </summary>
    TState State { get; }

    /// <summary>
    /// Occurs when the state has changed after an action was processed by the reducer.
    /// Components should subscribe to this event to trigger UI updates (re-rendering).
    /// </summary>
    event Action<TState>? OnStateChanged;
}