/* Project Folio Lite (ProjFolioLite)
 * Copyright (c) 2025 terrytw. All Rights Reserved.
 *
 * This source code is for portfolio and demonstration purposes only.
 * Unauthorized copying, modification, or distribution is strictly prohibited.
 * See LICENSE file in the project root for full terms and conditions.
 */

using TtwSDK.Redux.Core;

namespace TtwSDK.Redux.Thunks;

/// <summary>
/// Represents an asynchronous action that encapsulates side effects, such as API calls, delays, or complex logic flows.
/// Unlike a pure <see cref="IAction"/> which is handled by a reducer, an <see cref="IAsyncAction"/> is intercepted and executed by the <see cref="IDispatcher"/>.
/// </summary>
public interface IAsyncAction : IAction
{
    /// <summary>
    /// Executes the side effect logic defined in this action.
    /// </summary>
    /// <param name="dispatcher">
    /// The dispatcher instance. This allows the IAsyncAction to dispatch subsequent actions 
    /// (e.g., dispatching a 'Success' or 'Failure' action after an API call completes).
    /// </param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Execute(IDispatcher dispatcher);
}