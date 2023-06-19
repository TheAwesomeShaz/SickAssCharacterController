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
        //Debug.Log(direction.x+" "+direction.y);
       
        // if input has up direction and there is a neighbour above this ledge irrespective of top left or top right
        // then we pick that as the neighbour
        if (direction.y != 0)
            neighbour = neighbours.FirstOrDefault(n => n.direction.y == direction.y);

        if (neighbour == null && direction.x != 0)
            neighbour = neighbours.FirstOrDefault(n => n.direction.x == direction.x);

        Debug.Log(neighbour==null?"Neighbour is null what point u will get now ":neighbour.point);
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

