using UnityEngine;

public class Master : MonoBehaviour {
    public static Master master;

    void Awake() {
        if (master == null) {
            DontDestroyOnLoad(gameObject);
            master = this;

        } else if (master != this) {
            Destroy(gameObject);
        }
    }
}