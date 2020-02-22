using System.Collections.Generic;
using UnityEngine;
public class Agent : MonoBehaviour
{
    public int moveButton = 0;
    public float moveSpeed = 3;
    private Astar Astar = new Astar();
    private List<Vector2Int> path = new List<Vector2Int>();
    private Plane ground = new Plane(Vector3.up, 0f);
    private MeshRenderer renderer;
    private GameObject targetVisual;
    private MazeGeneration maze;
    private LineRenderer line;
    private void Awake()
    {
        maze = FindObjectOfType<MazeGeneration>();
        renderer = GetComponentInChildren<MeshRenderer>();
        targetVisual = GameObject.CreatePrimitive(PrimitiveType.Cube);
        targetVisual.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        targetVisual.GetComponent<MeshRenderer>().material.color = renderer.material.color;
        line = GetComponent<LineRenderer>();
        line.material.color = renderer.material.color;
        line.material.color = renderer.material.color;
    }

    private void Start()
    {
    }

    public void FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        path = Astar.FindPathToTarget(startPos, endPos, grid);
        DrawPath();
    }

    private void DrawPath()
    {
        if (path != null && path.Count > 0)
        {
            line.positionCount = path.Count;
            for (int i = 0; i < path.Count; i++)
            {
                line.SetPosition(i, Vector2IntToVector3(path[i], 0.1f));
            }
        }
    }


    //Move to clicked position
    public void Update()
    {
        if (Input.GetMouseButtonDown(moveButton))
        {
            Debug.Log("Click");
            Ray r = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10));

            Vector3 mousePos = MouseToWorld();
            Vector2Int targetPos = Vector3ToVector2Int(mousePos);
            targetVisual.transform.position = Vector2IntToVector3(targetPos);
            FindPathToTarget(Vector3ToVector2Int(transform.position), targetPos, maze.grid);
        }

        if (path != null && path.Count > 0)
        {
            if (transform.position != Vector2IntToVector3(path[0]))
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Vector2IntToVector3(path[0]) - transform.position), 360f * Time.deltaTime);
                transform.position = Vector3.MoveTowards(transform.position, Vector2IntToVector3(path[0]), moveSpeed * Time.deltaTime);
            }
            else
            {
                path.RemoveAt(0);
                DrawPath();
            }
        }

    }
    public Vector3 MouseToWorld()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        float distToGround = -1f;
        ground.Raycast(ray, out distToGround);
        Vector3 worldPos = ray.GetPoint(distToGround);

        return worldPos;
    }

    private Vector2Int Vector3ToVector2Int(Vector3 pos)
    {
        return new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.z));
    }
    private Vector3 Vector2IntToVector3(Vector2Int pos, float YPos = 0)
    {
        return new Vector3(Mathf.RoundToInt(pos.x), YPos, Mathf.RoundToInt(pos.y));
    }
    private void OnDrawGizmos()
    {
        if (path != null && path.Count > 0)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                Gizmos.color = renderer.material.color;
                Gizmos.DrawLine(Vector2IntToVector3(path[i], 0.5f), Vector2IntToVector3(path[i + 1], 0.5f));
            }
        }
    }
}