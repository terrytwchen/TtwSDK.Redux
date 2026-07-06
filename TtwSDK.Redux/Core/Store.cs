/* TtwSDK.Redux
 * Copyright (c) 2025 terrytw. Licensed under the MIT License.
 * See LICENSE file for details.
 */

namespace TtwSDK.Redux.Core;

/// <summary>
/// Represents a generic Redux store that manages the application state.
/// Thread-safe implementation using locks.
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
        _reducer = reducer ?? throw new ArgumentNullException(nameof(reducer));
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
    /// <para>
    /// The event is invoked outside of the lock, using a captured snapshot of the new state. This keeps the lock's
    /// hold time short and avoids deadlocks when a subscriber re-enters <see cref="Dispatch"/> from within the
    /// callback (e.g. a UI repaint that triggers another dispatch).
    /// </para>
    /// </remarks>
    /// <param name="action">The action to be processed.</param>
    public void Dispatch(IAction action)
    {
        TState? snapshot = default;
        bool changed = false;

        lock (_lock)
        {
            var previousState = State;
            var newState = _reducer(previousState, action);

            // Reference equality check or structural equality check depends on TState implementation (record vs class)
            if (!EqualityComparer<TState>.Default.Equals(previousState, newState))
            {
                State = newState;
                snapshot = newState;
                changed = true;
            }
        }

        if (changed)
        {
            OnStateChanged?.Invoke(snapshot!);
        }
    }
}