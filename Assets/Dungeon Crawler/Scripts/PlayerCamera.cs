using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public GameObject Player;   
    public float OffsetX = -35;
    public float OffsetZ = -40;
    public float OffsetY = 30;
    public float MaximumDistance = 2;
    public float PlayerVelocity = 10;
    private float _movmentX;
    private float _movmentY;
    private float _movmentZ;
    // Use this for initialization
    void Start()
    {

    }

    
    void LateUpdate()
    {
        _movmentX = ((Player.transform.position.x + OffsetX - this.transform.position.x)) / MaximumDistance;
        _movmentY = ((Player.transform.position.y + OffsetY - this.transform.position.y)) / MaximumDistance;
        _movmentZ = ((Player.transform.position.z + OffsetZ - this.transform.position.z)) / MaximumDistance;
        this.transform.position += new Vector3(
            (_movmentX * PlayerVelocity * Time.deltaTime), 
            (_movmentY * PlayerVelocity * Time.deltaTime), 
            (_movmentZ * PlayerVelocity * Time.deltaTime));
    }
}