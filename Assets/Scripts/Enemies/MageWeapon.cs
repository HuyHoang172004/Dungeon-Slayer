using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageWeapon : MonoBehaviour {

    public GameObject boss;
    public Animator animator;
    public GameObject ballPrefab;
    public GameObject sound;

    void ShootBall() {
        // Shoot the magic ball
        Instantiate(ballPrefab, boss.transform.position, boss.transform.rotation);

        //Sound
        Instantiate(sound);
    }
}
