using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour {
    
    private static T instance;
    public static T Instance {
        get {
            if (instance == null) {
                instance = (T)FindObjectOfType(typeof(T));
                if (instance == null) {
                    Debug.LogError(typeof(T) + "がシーンに存在しません。");
                }
            }
            return instance;
        }
    }
    
    virtual protected void Awake(){
        // 他のゲームオブジェクトにアタッチされているか調べる
        // アタッチされている場合は破棄する
        CheckInstance();
    }

    protected bool CheckInstance(){
        if (instance == null) {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
            return true;
        } else if (Instance == this) {
            return true;
        }
        Destroy (this);
        return false;
    }
    
}
