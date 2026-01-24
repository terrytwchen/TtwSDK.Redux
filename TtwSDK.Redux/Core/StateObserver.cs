/* TtwSDK.Redux
 * Copyright (c) 2025 terrytw. Licensed under the MIT License.
 * See LICENSE file for details.
 */

namespace TtwSDK.Redux.Core;

/// <summary>
/// A framework-agnostic observer that manages subscriptions to Redux stores.
/// It acts as a bridge between the Store (Model) and the View (UI), handling diffing and lifecycle management.
/// </summary>
public class StateObserver : IDisposable
{
    private readonly List<Action> _disposables = new();
    private readonly Action? _onGlobalStateChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="StateObserver"/> class.
    /// </summary>
    /// <param name="onStateChanged">
    /// An optional callback invoked whenever *any* observed state changes.
    /// Useful for framework-level repaints (e.g., Blazor's StateHasChanged, Unity's Repaint).
    /// </param>
    public StateObserver(Action? onStateChanged = null)
    {
        _onGlobalStateChanged = onStateChanged;
    }

    /// <summary>
    /// Subscribes to a specific slice of the state.
    /// Checks for equality before triggering callbacks to avoid unnecessary updates.
    /// </summary>
    /// <typeparam name="TState">The type of the store's state.</typeparam>
    /// <typeparam name="TValue">The type of the value to select.</typeparam>
    /// <param name="store">The store to observe.</param>
    /// <param name="selector">A pure function to extract the value from the state.</param>
    /// <param name="onValueChanged">
    /// An optional callback invoked specifically when this selected value changes.
    /// Passes the new value as an argument.
    /// </param>
    /// <returns>The current value of the selected state slice (useful for initialization).</returns>
    public TValue Observe<TState, TValue>(
        IReadOnlyStore<TState> store, 
        Func<TState, TValue> selector, 
        Action<TValue>? onValueChanged = null)
    {
        // 1. Capture initial value for diffing
        TValue lastValue = selector(store.State);

        // 2. Define the handler logic
        void HandleStoreUpdate(TState newState)
        {
            TValue newValue = selector(newState);

            // Diffing: Only trigger if value actually changed
            if (!EqualityComparer<TValue>.Default.Equals(lastValue, newValue))
            {
                lastValue = newValue;
                
                // Specific callback (Push Model)
                onValueChanged?.Invoke(newValue);
                
                // Global callback (Pull Model / Repaint Signal)
                _onGlobalStateChanged?.Invoke();
            }
        }

        // 3. Subscribe to the store
        store.OnStateChanged += HandleStoreUpdate;

        // 4. Register cleanup
        _disposables.Add(() => store.OnStateChanged -= HandleStoreUpdate);

        return lastValue;
    }

    /// <summary>
    /// Unsubscribes from all observed stores.
    /// </summary>
    public void Dispose()
    {
        foreach (var dispose in _disposables)
        {
            dispose.Invoke();
        }
        _disposables.Clear();
        GC.SuppressFinalize(this);
    }
}