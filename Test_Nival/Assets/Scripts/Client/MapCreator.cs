using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreator : MonoBehaviour {

    public Transform tilePrefab;
    public Transform PlayerPrefab;
    public GameObject[] Players;
    public List<UnitController> Player;
    public List<bool> IsMoving; 
    public bool UpdPosition = false;

    private MyWebService Request = new MyWebService();

    public StateClient startState = new StateClient();
    public StateClient reqState = new StateClient();

    public string Json;

    private float outlinePercent = 0.05f;

   
    void OnEnable()
    {
        EventManager.Step += GetUpdatePosition;
    }

    void OnDisable()
    {
        EventManager.Step -= GetUpdatePosition;
    }



    void Awake()
    {
        StartCoroutine(Request.GetNewState((st)=>CreateMap(st)));
    }

    void Start()
    {
        startState = CollectState();
        UpdPosition = false;
    }
    

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            reqState = CollectState();
            HitTarget(Players);

            StartCoroutine(Request.GetNextState(reqState, UpdateUnitProperty));

            reqState.Units.Clear();

        }

    }

    void GetUpdatePosition()
    {
        for (int i = 0; i < Players.Length; i++)
        {
            Player.Add(Players[i].GetComponent<UnitController>());
            IsMoving.Add(Players[i].GetComponent<UnitController>().UnitProperty.IsMoving);
        }
        if (Player.Count > 1)
        {
            for (int i = 0; i < Player.Count - 1; i++)
            {
                if ((Player[i] && IsMoving[i]) || (Player[i + 1] && IsMoving[i + 1]))
                {
                    UpdPosition = true;
                }

            }
        }
        else
        {
            if (Player[0] && IsMoving[0])
            {
                UpdPosition = true;
            }
        }
        if (UpdPosition)
        {
            UpdPosition = false;
            reqState = CollectState();
            string jsonCatch = JsonUtility.ToJson(reqState);
            if (Json != jsonCatch)
            {
                Json = jsonCatch;
                StartCoroutine(Request.GetNextState(reqState, UpdateUnitProperty));
            }
            reqState.Units.Clear();
        }
        Player.Clear();
        IsMoving.Clear();
    }

    public StateClient CollectState()
    {
        if (Players.Length != 0)
        {
            foreach (var unit in Players)
            {
                reqState.Units.Add(unit.GetComponent<UnitController>().UnitProperty);
            }
            reqState.FieldSize = startState.FieldSize;
            reqState.NumberOfPlayer = startState.NumberOfPlayer;
        }
        return reqState;
    }

    public void CreateMap(StateClient state)
    {
        startState = state;
        string holderName = "Ceate Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        for (int x = 0; x < state.FieldSize; x++)
        {
            for (int y = 0; y < state.FieldSize; y++)
            {
                Vector3 tilePosition = CoordToPosition(x, y, state.FieldSize);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                newTile.localScale = Vector3.one * (1 - outlinePercent);
                newTile.parent = mapHolder;
                var tile = newTile.GetComponent<TileClient>();
                tile.X = x;
                tile.Y = y;
            }
        }

        foreach (var unit in state.Units)
        {
            Vector3 obstaclePosition = CoordToPosition(unit.X, unit.Y, state.FieldSize);
            Transform newObstacle = Instantiate(PlayerPrefab, obstaclePosition + Vector3.up * 0.5f, Quaternion.identity) as Transform;
            newObstacle.parent = mapHolder;
            newObstacle.gameObject.tag = "Player";
            
            var v = newObstacle.gameObject.GetComponent<UnitController>();
            v.FieldSize = startState.FieldSize;
            v.UnitProperty.Init(unit);
            
        }
        Players = GameObject.FindGameObjectsWithTag("Player");
    }

    public Vector3 CoordToPosition(int x, int y, int mapSize)
    {
        return new Vector3(-mapSize / 2 + 0.5f + x, 0, -mapSize / 2 + 0.5f + y);
    }

    public void UpdateUnitProperty(StateClient state)
    {
        int i = 0;
        foreach (var unit in state.Units)
        {
            Players[i].GetComponent<UnitController>().UnitProperty.Init(unit);
            i++;
        }
    }

    void HitTarget(GameObject[] Players)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "Tile")
            {
                foreach (var Player in Players)
                {
                    var playerTarget = Player.GetComponent<UnitController>();
                    playerTarget.GetTarget(hit.collider.gameObject);
                }
            }
        }
        else
        {
            foreach(var Player in Players)
            {
                Player.GetComponent<UnitController>().GetTarget(Player);
            }
        }

    }

}
