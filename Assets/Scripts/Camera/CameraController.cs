using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public GameObject objective;
    public float speed = 2f;
    public float ZOriginal = 0;
    public float smoothRate = 0.3f;
    private Vector3 speedCamera;


    // Start is called before the first frame update
    void Start() {
        ZOriginal = transform.position.z;
        speedCamera = new Vector3(speed, speed, 0);

    }

    // Update is called once per frame
    void Update() {
        if (objective == null) {
            return;
        }

        Vector3 tmp = Vector3.SmoothDamp(transform.position, objective.transform.position, ref speedCamera, this.smoothRate);
        tmp.z = ZOriginal;
        this.transform.position = tmp;

    }
}
