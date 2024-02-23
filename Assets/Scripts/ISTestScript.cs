using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ISTestScript : MonoBehaviour
{
    TestInput tiasset;
    // Start is called before the first frame update
    void Start()
    {
        tiasset = new TestInput();
        tiasset.gameplay.Enable();
    }

    public float speed = 5.0f;
    // Update is called once per frame
    void Update()
    {
        var val = tiasset.gameplay.move.ReadValue<Vector2>();
        Vector3 dir = new Vector3(val.x, 0, val.y);
        Debug.Log(dir);
        transform.Translate(dir.normalized * speed * Time.deltaTime);
    }
}
