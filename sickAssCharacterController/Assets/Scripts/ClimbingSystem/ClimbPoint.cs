using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClimbPoint : MonoBehaviour
{
    [SerializeField] List<Neighbour> neighbours;

    private void Awake()
    {
        var twoWayNeighbours = neighbours.Where(n => n.isTwoWay);
        foreach (var neighbour in neighbours)
        {
            neighbour.point?.CreateConnection(this, -neighbour.direction, neighbour.connectionType, neighbour.isTwoWay);
        }
    }

    public void CreateConnection(ClimbPoint point, Vector2 direction, ConnectionType connectionType,
        bool isTwoWay = true)
    {
        var neighbour = new Neighbour()
        {
            point = point,
            direction = direction,
            connectionType = connectionType,
            isTwoWay = isTwoWay
        };

        neighbours.Add(neighbour); 
    }

    public Neighbour GetNeighbourInDirection(Vector2 direction)
    {
        Neighbour neighbour = null;

        // This way is kinda more accurate but let's follow the course ig?
        //neighbour = neighbours.FirstOrDefault(n => n.direction == direction);


        if(direction.y!=0)
            neighbour = neighbours.FirstOrDefault(n => n.direction.y == direction.y);

        if(neighbour==null && direction.x!=0)
            neighbour = neighbours.FirstOrDefault(n => n.direction.x == direction.x);

        return neighbour;
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.blue);

        foreach (var neighbour in neighbours)
        {
            if(neighbour.point != null)
            {
                Debug.DrawLine(transform.position, neighbour.point.transform.position, (neighbour.isTwoWay) ? Color.green : Color.red );
            }
        }
    }

}

[System.Serializable]   
public class Neighbour
{
    public ClimbPoint point;
    public Vector2 direction;

    public ConnectionType connectionType;
    public bool isTwoWay;

    // direction Array is created in Utils
   
}

public enum ConnectionType
{
    Jump,
    Move,
}

