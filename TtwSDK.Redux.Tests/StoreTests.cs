/* TtwSDK.Redux
 * Copyright (c) 2025 terrytw. Licensed under the MIT License.
 * See LICENSE file for details.
 */

using TtwSDK.Redux.Core;

namespace TtwSDK.Redux.Tests;

public class StoreTests
{
    private sealed record CounterState(int Count);

    private sealed class IncrementAction : IAction { }

    private sealed class NoopAction : IAction { }

    private static CounterState IdentityOrIncrementReducer(CounterState state, IAction action)
    {
        return action switch
        {
            IncrementAction => new CounterState(state.Count + 1),
            _ => state,
        };
    }

    [Fact]
    public void Dispatch_WithStateChangingAction_UpdatesStateAndRaisesOnStateChanged()
    {
        var store = new Store<CounterState>(new CounterState(0), IdentityOrIncrementReducer);

        CounterState? observed = null;
        store.OnStateChanged += state => observed = state;

        store.Dispatch(new IncrementAction());

        Assert.Equal(1, store.State.Count);
        Assert.NotNull(observed);
        Assert.Equal(1, observed!.Count);
    }

    [Fact]
    public void Dispatch_WithMultipleStateChangingActions_AccumulatesState()
    {
        var store = new Store<CounterState>(new CounterState(0), IdentityOrIncrementReducer);

        store.Dispatch(new IncrementAction());
        store.Dispatch(new IncrementAction());

        Assert.Equal(2, store.State.Count);
    }

    [Fact]
    public void Dispatch_WithNoStateChange_DoesNotRaiseOnStateChanged()
    {
        var store = new Store<CounterState>(new CounterState(0), IdentityOrIncrementReducer);

        var raised = false;
        store.OnStateChanged += _ => raised = true;

        store.Dispatch(new NoopAction());

        Assert.False(raised);
        Assert.Equal(0, store.State.Count);
    }

    [Fact]
    public void Dispatch_WithNoStateChange_ReturnsSameStateReference()
    {
        var initialState = new CounterState(0);
        var store = new Store<CounterState>(initialState, IdentityOrIncrementReducer);

        store.Dispatch(new NoopAction());

        Assert.Same(initialState, store.State);
    }

    [Fact]
    public async Task Dispatch_CalledReentrantlyFromOnStateChanged_DoesNotDeadlock()
    {
        // Regression test for REDUX-ISSUE-001: OnStateChanged must fire outside the lock,
        // otherwise a subscriber that re-enters Dispatch (e.g. UI repaint triggering another
        // dispatch) would deadlock on the store's internal lock.
        var store = new Store<CounterState>(new CounterState(0), IdentityOrIncrementReducer);

        var reentrantCallCount = 0;
        store.OnStateChanged += _ =>
        {
            if (reentrantCallCount == 0)
            {
                reentrantCallCount++;
                store.Dispatch(new IncrementAction());
            }
        };

        var dispatchTask = Task.Run(() => store.Dispatch(new IncrementAction()));
        var completedTask = await Task.WhenAny(dispatchTask, Task.Delay(TimeSpan.FromSeconds(5)));

        Assert.Same(dispatchTask, completedTask);
        Assert.Equal(2, store.State.Count);
    }
}
