using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public GameObject hex_prefab;

    public float unit_lenght = 1;

    //public int sizeX, sizeY;

    public int player = 0;

    public int number_of_rows;
    public float offsetX;

    public GameObject RedPlayer, GreenPlayer, BluePlayer;

    public bool firstround = true;
    private int count = 0;

    public GameObject[,] grid;
    // Use this for initialization

    private Vector2 dir1 = new Vector2(1, 0);
    private Vector2 dir2 = new Vector2(1, -1);
    private Vector2 dir3 = new Vector2(0, -1);
    
    void Start() {
        grid = new GameObject[11, 11];
        GenerateGrid();
    }

    private void GenerateGrid() {
        unit_lenght = hex_prefab.renderer.bounds.size.x / Mathf.Sqrt(3);
        offsetX = Mathf.Sqrt(3) * unit_lenght * 0.5f;
        int y = 10;
        for (int i = 0; i < number_of_rows; i++) {
            float posy = i * 1.5f * unit_lenght - (number_of_rows * 1.5f * unit_lenght) / 2;
            int number_of_hexs = 1 + i;
            float posx = -offsetX * i;
            int x = 0;
            Vector3 init_pos = new Vector3(posx, posy, 0);

            GameObject o = (GameObject)Instantiate(hex_prefab, init_pos, Quaternion.identity);
            o.name = "hex " + count.ToString();
            StoreInGrid(x, y, o);
            count++;
            //grid[x,y] = GameObject.Find
            //Debug.Log(x.ToString() + " " + y.ToString());
            x++;
            for (int j = 1; j < number_of_hexs; j++) {
                init_pos.x += (offsetX * 2);
                o = (GameObject)Instantiate(hex_prefab, init_pos, Quaternion.identity);
                o.name = "hex " + count.ToString();
                StoreInGrid(x, y, o);
                count++;
                //Debug.Log(x.ToString() + " " + y.ToString());
                x++;
            }
            y--;
        }
    }

    private void StoreInGrid(int x, int y, GameObject o) {
        print("storing: " + o.gameObject.name);
        //GameObject obj = GameObject.Find("s");
        //if (obj == null) {
        //    print(s + " not found");
        //}
        //else {
        grid[x, y] = o;
        o.SendMessage("SetGridPosition", new Vector2(x, y));
        //    Debug.Log(obj.name);
        //}
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetButtonDown("Fire1")) {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.zero);
            if (hit.collider != null) {
                Debug.Log(hit.collider.gameObject.tag);
                chooseTile(hit.collider);
            }
        }
    }

    public void IncPlayer() {
        player = (player + 1) % 3;
    }

    private void chooseTile(Collider2D col) {
        if (firstround) {
            if (isStartTile(col)) {
                switch (getPlayerColor()) {
                    case hexagon_script.HexagonColor.WHITE:
                        break;
                    case hexagon_script.HexagonColor.RED:
                        RedPlayer = col.gameObject;
                        break;
                    case hexagon_script.HexagonColor.GREEN:
                        GreenPlayer = col.gameObject;
                        break;
                    case hexagon_script.HexagonColor.BLUE:
                        BluePlayer = col.gameObject;
                        break;
                    default:
                        break;
                }
                if (BluePlayer != null && GreenPlayer != null && BluePlayer != null) {
                    firstround = false;
                }
                SetTileColor(col, getPlayerColor());
            }
        }
        else {
            SetTileColor(col);
        }
    }

    private void SetTileColor(Collider2D col, hexagon_script.HexagonColor color) {
        hexagon_script s = col.GetComponent<hexagon_script>();
        s.captureTile(color);
        IncPlayer();
    }

    private void SetTileColor(Collider2D col) {
        hexagon_script s = col.GetComponent<hexagon_script>();
        if (s.isWhite()) {
            s.captureTile(getPlayerColor());
            checkAdjacentTiles(s.GetGridPosition());
            IncPlayer();
        }
        //col.SendMessage("setColor", playerColor());
    }

    private void checkAdjacentTiles(Vector2 origin) {
        SpreadCheck(origin, dir1);
        SpreadCheck(origin, dir2);
        SpreadCheck(origin, dir3);
        SpreadCheck(origin, dir1 * -1);
        SpreadCheck(origin, dir2 * -1);
        SpreadCheck(origin, dir3 * -1);
    }

    private void SpreadCheck(Vector2 origin, Vector2 dir) {
        List<Vector2> tilesToColor = new List<Vector2>();
        Vector2 currentPos;
        bool canCapture = false;
        if (!IncrementPos(out currentPos, origin, dir)){
            return;
	    }
        while (true) {
            hexagon_script s = GetHexScript(grid[(int)currentPos.x, (int)currentPos.y]);
            if (s.isWhite()) {
                return;
            }
            if (s.sameColor(getPlayerColor())){
                canCapture = true;
                break;
            }
            else {
                tilesToColor.Add(currentPos);
                if (!IncrementPos(out currentPos, currentPos, dir)) 
                    break;
            }
        }
        if (canCapture) {
            CaptureTiles(tilesToColor);
        }
    }

    private void CaptureTiles(List<Vector2> l) {
        foreach (Vector2 item in l) {
            grid[(int)item.x, (int)item.y].SendMessage("captureTile", getPlayerColor());
            checkAdjacentTiles(item);
        }
    }

    private bool IncrementPos(out Vector2 currentPos, Vector2 paux, Vector2 dir) {
        Vector2 newpos = paux + dir;
        if (newpos.x < 0 || newpos.y < 0|| newpos.x > 10 || newpos.y > 10) {
            currentPos = Vector2.zero;
            return false;
        }
        else {
            if (grid[(int)newpos.x, (int)newpos.y] == null) {
                currentPos = Vector2.zero;
                return false;
            }
            else {
                currentPos = newpos;
                return true;
            }
        }
    }

    private hexagon_script GetHexScript(GameObject go) {
        return go.GetComponent<hexagon_script>();
    }

    private bool isStartTile(Collider2D col) {
        string nmr = col.name;
        if (nmr == "hex 0" || nmr == "hex 55" || nmr == "hex 65")
            return true;
        return false;
    }

    public hexagon_script.HexagonColor getPlayerColor() {
        hexagon_script.HexagonColor c = hexagon_script.HexagonColor.WHITE;
        switch (player) {
            case 0:
                c = hexagon_script.HexagonColor.RED;
                break;
            case 1:
                c = hexagon_script.HexagonColor.GREEN;
                break;
            case 2:
                c = hexagon_script.HexagonColor.BLUE;
                break;
            default:
                break;
        }

        return c;
    }

    void OnGUI() {
        // Make a background box
        GUI.Box(new Rect(10, 10, 100, 120), "Loader Menu");
        // Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
        if (GUI.Button(new Rect(15, 40, 90, 20), "Reset Game")) {
            Application.LoadLevel("Main");
        }
        GUI.Label(new Rect(10, 80, 100, 40), "Current Player: " + (player + 1).ToString());
    }
}
