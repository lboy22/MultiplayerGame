using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using UnityEngine.SceneManagement;


public class SimplePlayer : NetworkBehaviour
{
    public GameObject spawnPointOne;
    public GameObject spawnPointTwo;

    // From line 14 to line 36, the code is to verify that the connectivity is working as intended, and data is being passed/received correctly as intended.
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

    // A set spawning position is specified to both the host and clients.
    private void Start()
    {
        if(IsOwner)
        {
            transform.position = spawnPointOne.transform.position;
        }
        else
        {
            transform.position = spawnPointTwo.transform.position;
        }

    }
    
    // Player movement (using keys A and D) is defined in the update method.
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

    // Winning condition is set as the first player to touch the ground finishes the game/level.
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Ground")
        {
            Destroy(this.gameObject);
            SceneManager.LoadScene("Credits");
        }

    }
}
