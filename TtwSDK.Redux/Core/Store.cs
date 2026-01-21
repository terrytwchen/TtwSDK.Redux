/* TtwSDK.Redux
 * Copyright (c) 2025 terrytw. Licensed under the MIT License.
 * See LICENSE file for details.
 */

namespace TtwSDK.Redux.Core;

/// <summary>
/// Represents a generic Redux store that manages the application state.
/// This implementation uses a lock to ensure thread safety during state updates.
/// </summary>
/// <typeparam name="TState">The type of the state managed by this store. Ideally, this should be an immutable record or class.</typeparam>
public class Store<TState> : IStore<TState>
{
    /// <summary>
    /// Gets the current snapshot of the state.
    /// </summary>
    public TState State { get; private set; }

    /// <summary>
    /// Occurs when the state has changed after an action is processed.
    /// Components should subscribe to this event to trigger UI updates.
    /// </summary>
    public event Action<TState>? OnStateChanged;

    private readonly Reducer<TState> _reducer;
    private readonly object _lock = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Store{TState}"/> class.
    /// </summary>
    /// <param name="initialState">The initial state of the store.</param>
    /// <param name="reducer">The root reducer function used to calculate the new state based on incoming actions.</param>
    public Store(TState initialState, Reducer<TState> reducer)
    {
        State = initialState;
        _reducer = reducer;
    }

    /// <summary>
    /// Dispatches an action to the store, triggering the reducer to calculate the new state.
    /// </summary>
    /// <remarks>
    /// This method is thread-safe using a lock.
    /// <para>
    /// The <see cref="OnStateChanged"/> event is invoked only if the new state is different from the previous state 
    /// (checked via <see cref="EqualityComparer{T}.Default"/>).
    /// </para>
    /// </remarks>
    /// <param name="action">The action to be processed.</param>
    public void Dispatch(IAction action)
    {
        lock (_lock)
        {
            var previousState = State;
            var newState = _reducer(previousState, action);

            if (!EqualityComparer<TState>.Default.Equals(previousState, newState))
            {
                State = newState;
                OnStateChanged?.Invoke(State);
            }
        }
    }
}