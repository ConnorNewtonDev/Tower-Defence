using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Start is called before the first frame update
    public float fireRate;
    public int damage;

    public float moveSpeed;
    public GameObject target;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
        this.transform.LookAt(target.transform, Vector3.up);
        //this.transform.position = Vector3.Lerp(this.transform.position, target.transform.position, 0.25f);
        this.transform.position = Vector3.MoveTowards(this.transform.position, target.transform.position, moveSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            other.transform.parent.GetComponent<Enemy>().Damage(damage);
            Destroy(this.gameObject);
        }
    }
}
