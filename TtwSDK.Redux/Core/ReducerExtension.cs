/* TtwSDK.Redux
 * Copyright (c) 2025 terrytw. Licensed under the MIT License.
 * See LICENSE file for details.
 */

namespace TtwSDK.Redux.Core;

/// <summary>
/// Represents a pure function that determines the next state based on the current state and the dispatched action.
/// </summary>
/// <typeparam name="TState">The type of the state.</typeparam>
/// <param name="state">The current state.</param>
/// <param name="action">The action dispatched to trigger the state change.</param>
/// <returns>The new state resulting from the action.</returns>
public delegate TState Reducer<TState>(TState state, IAction action);

/// <summary>
/// Provides extension methods for combining and transforming reducers.
/// </summary>
public static class ReducerExtensions
{
    /// <summary>
    /// Composes a collection of reducers into a single reducer.
    /// The reducers are executed in the order they appear in the collection, passing the state from one to the next.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <param name="reducers">The collection of reducers to compose.</param>
    /// <returns>A single reducer that represents the combined logic of all input reducers.</returns>
    public static Reducer<TState> Compose<TState>(this IEnumerable<Reducer<TState>> reducers)
    {
        return (state, action) =>
        {
            TState currentState = state;
            foreach (var reducer in reducers)
            {
                currentState = reducer(currentState, action);
            }
            return currentState;
        };
    }

    /// <summary>
    /// Adapts a reducer capable of handling a sub-state (child) to work within the context of a larger state (parent).
    /// This allows for separation of concerns by keeping sub-reducers unaware of the global state structure.
    /// </summary>
    /// <typeparam name="TParent">The type of the parent (global) state.</typeparam>
    /// <typeparam name="TChild">The type of the sub-state (slice).</typeparam>
    /// <param name="childReducer">The reducer that handles the logic for the sub-state.</param>
    /// <param name="selector">A function to extract the sub-state from the parent state.</param>
    /// <param name="updater">A function to merge the updated sub-state back into the parent state.</param>
    /// <returns>A reducer that accepts the parent state but delegates logic to the child reducer.</returns>
    public static Reducer<TParent> Slice<TParent, TChild>(
        this Reducer<TChild> childReducer,
        Func<TParent, TChild> selector,
        Func<TParent, TChild, TParent> updater)
    {
        return (parentState, action) =>
        {
            var childState = selector(parentState);

            var newChildState = childReducer(childState, action);

            // Optimization: If the child state has not changed (determined by EqualityComparer), 
            // return the original parent state reference to avoid unnecessary object creation.
            if (EqualityComparer<TChild>.Default.Equals(childState, newChildState))
            {
                return parentState;
            }

            return updater(parentState, newChildState);
        };
    }
}