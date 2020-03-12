using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VehicleBehaviour;

public class WallMechanics : MonoBehaviour
{
    public float pushForce = 10;
    public bool wallside;//false = left, true = right
    public AudioClip clip;
    private AudioSource _source;
    // Use this for initialization
    void Start()
    {
        _source = GetComponent<AudioSource>();
        _source.clip = clip;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision other)
    {
        // GameSystem g_sys = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        // g_sys.speedUp(other.gameObject.tag, 1.05f);
        // if (this.gameObject.tag == "Right Wall")
        // {
        //     Debug.Log(other);

        //     other.gameObject.GetComponent<Rigidbody>().AddForce((Vector3.left) * pushForce, ForceMode.Impulse);
        // }s
        // else if (this.gameObject.tag == "Left Wall")
        // {
        //     other.gameObject.GetComponent<Rigidbody>().AddForce((Vector3.right) * pushForce, ForceMode.Impulse);
        // }
        // Debug.Log("Hit wall");
        // Debug.Log(wallside);
        
        WheelVehicle w = other.gameObject.GetComponent<WheelVehicle>();
        // Debug.Log(w.gameObject);
        Vector3 norm = -other.GetContact(0).normal.normalized;
        Vector3 indir = (other.GetContact(0).point) - (w.transform.position);
        Vector3 dir = (indir - 2 * Vector3.Dot(indir, norm) * norm);
        Vector3 fwdF = (wallside ? Quaternion.Euler(0, -90, 0) * norm : Quaternion.Euler(0, 90, 0) * norm);
        float co = 1f * w._rb.velocity.y * w._rb.velocity.y / (w._rb.velocity.x * w._rb.velocity.x + w._rb.velocity.z * w._rb.velocity.z);
        fwdF.y = Mathf.Pow(co * (fwdF.x * fwdF.x + fwdF.z * fwdF.z), 0.3f);
        if (fwdF.y > 0)
            fwdF.y *= -1;
        dir.y = Mathf.Pow(co * (dir.x * dir.x + dir.z * dir.z), 0.5f);
        if (dir.y > 0)
            dir.y *= -1;
        Debug.Log(w);
        Debug.Log(norm);
        Debug.Log(other.GetContact(0).point);
        Debug.Log(w.transform.position);
        Debug.Log(indir);
        Debug.Log(dir);
        if (w == null)
            return;
        w._rb.rotation = Quaternion.LookRotation(fwdF.normalized);
        w.Cart_speedup(15f * dir + 1f* fwdF, 0.25f);
        w.Cart_speedup(3f * fwdF, Mathf.Pow(Vector3.Distance(Vector3.zero, w._rb.velocity), 0.25f) *1f);

        _source.pitch = Random.Range(0.8f, 1.2f);
        _source.Play();
    }
}
