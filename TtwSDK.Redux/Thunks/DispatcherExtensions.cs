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
/// Provides extension methods for the <see cref="IDispatcher"/> to support asynchronous actions (Thunks).
/// </summary>
public static class DispatcherExtensions
{
    /// <summary>
    /// Dispatches an asynchronous action (Thunk) that can perform side effects.
    /// </summary>
    /// <param name="dispatcher">The dispatcher instance.</param>
    /// <param name="action">The asynchronous action to execute.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if dispatcher or action is null.</exception>
    public static async Task DispatchAsync(this IDispatcher dispatcher, IAsyncAction action)
    {
        if (dispatcher == null)
            throw new ArgumentNullException(nameof(dispatcher));
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        await action.Execute(dispatcher);
    }
}