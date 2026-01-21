/* Project Folio Lite (ProjFolioLite)
 * Copyright (c) 2025 terrytw. All Rights Reserved.
 *
 * This source code is for portfolio and demonstration purposes only.
 * Unauthorized copying, modification, or distribution is strictly prohibited.
 * See LICENSE file in the project root for full terms and conditions.
 */

namespace TtwSDK.Redux.Core;

/// <summary>
/// A framework-agnostic observer that manages subscriptions to a Redux store.
/// It handles the logic of selecting specific state slices and triggering callbacks only when values change (diffing).
/// This class is designed to be used by UI adapters (e.g., Blazor components, Unity MonoBehaviours).
/// </summary>
public class StateObserver : IDisposable
{
    private readonly List<Action> _disposables = new();
    private readonly Action _onStateChangedCallback;

    /// <summary>
    /// Initializes a new instance of the <see cref="StateObserver"/> class.
    /// </summary>
    /// <param name="onStateChangedCallback">
    /// The callback to invoke when any observed state slice changes.
    /// For Blazor, this is typically `StateHasChanged`.
    /// For Unity, this might be a repaint call.
    /// </param>
    public StateObserver(Action onStateChangedCallback)
    {
        _onStateChangedCallback = onStateChangedCallback ?? throw new ArgumentNullException(nameof(onStateChangedCallback));
    }

    /// <summary>
    /// Subscribes to a specific slice of the state within a store.
    /// The <see cref="_onStateChangedCallback"/> is triggered only if the selected value changes compared to the previous state.
    /// </summary>
    /// <typeparam name="TState">The type of the state held by the store.</typeparam>
    /// <typeparam name="TValue">The type of the specific value to observe.</typeparam>
    /// <param name="store">The store instance to observe.</param>
    /// <param name="selector">A pure function to extract the desired value from the state.</param>
    public void Observe<TState, TValue>(IStore<TState> store, Func<TState, TValue> selector)
    {
        // Capture the initial value to perform diffing later.
        TValue lastValue = selector(store.State);

        void HandleChange(TState newState)
        {
            TValue newValue = selector(newState);

            // Use the default equality comparer to check if the value has effectively changed.
            // This prevents unnecessary UI updates if the state reference changed but the specific value did not.
            if (!EqualityComparer<TValue>.Default.Equals(lastValue, newValue))
            {
                lastValue = newValue;
                _onStateChangedCallback.Invoke();
            }
        }

        // Subscribe to the store's event.
        store.OnStateChanged += HandleChange;

        // Register the unsubscription logic to be called when this observer is disposed.
        _disposables.Add(() => store.OnStateChanged -= HandleChange);
    }

    /// <summary>
    /// Unsubscribes from all observed stores and clears the disposable list.
    /// </summary>
    public void Dispose()
    {
        foreach (var disposeAction in _disposables)
        {
            disposeAction.Invoke();
        }
        _disposables.Clear();
        GC.SuppressFinalize(this);
    }
}