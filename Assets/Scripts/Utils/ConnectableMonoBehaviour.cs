using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;


public abstract class ConnectableMonoBehaviour<T> : MonoBehaviour {

    private CompositeDisposable onResetDependent = null;

    protected Subject<T> onReceiveSubject = new Subject<T>();
    public IObservable<T> OnReceiveObservable { get { return onReceiveSubject; } }
    public bool IsConnecting { get; private set; } = false;


    protected virtual void OnDestroy() {
        onResetDependent?.Dispose();
    }


    public void Issue(T value) {
        if (IsConnecting) return;
        onReceiveSubject.OnNext(value);
    }

    public void SetDependent(ConnectableMonoBehaviour<T> component) {
        onResetDependent = new CompositeDisposable();
        IsConnecting = true;
        component.OnReceiveObservable.Subscribe(onReceiveSubject).AddTo(onResetDependent);
        // 依存先コンポーネントが破壊された際に、依存関係を切断する
        component.OnDestroyAsObservable().Subscribe(_ => ResetDependent()).AddTo(onResetDependent);
    }

    public void ResetDependent() {
        IsConnecting = false;
        onResetDependent?.Dispose();
    }

}
