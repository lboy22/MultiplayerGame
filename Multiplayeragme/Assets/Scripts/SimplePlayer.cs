using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using UnityEngine.SceneManagement;


public class SimplePlayer : NetworkBehaviour
{
    private Animator animator;

    public GameObject spawnPointOne;
    public GameObject spawnPointTwo;

    private NetworkVariable<MyCustomData> randomValue = new NetworkVariable<MyCustomData>(new MyCustomData { _int = 56, _bool = true}, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public struct MyCustomData : INetworkSerializable
    {
        public int _int;
        public bool _bool;
        public FixedString128Bytes message; 
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _bool);
            serializer.SerializeValue(ref message);
        }
    }

    public override void OnNetworkSpawn()
    {
        randomValue.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) =>
        {
            Debug.Log(OwnerClientId + "; RandomValue: " + newValue._int + "; " + newValue._bool);
        };
    }

    private void Start()
    {
        if(!IsOwner)
        {
            transform.position = spawnPointOne.transform.position;
        }
        else
        {
            transform.position = spawnPointTwo.transform.position;
        }
        animator = GetComponent<Animator>();

    }
    private void Update()
    {
        Debug.Log(OwnerClientId + "; Randome value" + randomValue);
        if (!IsOwner) { return; }


        Vector3 moveDirection = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.A))
        {
            moveDirection.x = -5f;
            randomValue.Value = new MyCustomData
            {
                _int = 10,
                _bool = false,
                message = "You beling to me!"
            };

        }
        if (Input.GetKey(KeyCode.D)) { moveDirection.x = +5f; }

        float moveSpeed = 3f;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Ground")
        {
            Destroy(this.gameObject);
            SceneManager.LoadScene("Credits");
        }

    }
}
