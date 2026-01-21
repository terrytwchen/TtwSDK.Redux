/* TtwSDK.Redux
 * Copyright (c) 2025 terrytw. Licensed under the MIT License.
 * See LICENSE file for details.
 */

namespace TtwSDK.Redux.Core;

/// <summary>
/// Defines the contract for a central dispatcher in the Redux architecture.
/// It is responsible for receiving actions from Views/Components and routing them to the appropriate handlers.
/// </summary>
public interface IDispatcher
{
    /// <summary>
    /// Dispatches a pure action to the system synchronously.
    /// </summary>
    /// <param name="action">The pure <see cref="IAction"/> to process. For async actions, use the extension method <c>DispatchAsync</c>.</param>
    void Dispatch(IAction action);

    /// <summary>
    /// Retrieves the current state of a specific type from the registered stores.
    /// </summary>
    /// <typeparam name="TState">The type of the state to retrieve.</typeparam>
    /// <returns>The current state object.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no store is registered for the specified state type.</exception>
    TState GetState<TState>();
}