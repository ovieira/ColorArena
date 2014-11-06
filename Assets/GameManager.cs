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
    private List<Vector2> UsableGridPositions;
    private List<hexagon_script> RedFinalTiles, GreenFinalTiles, BlueFinalTiles;

    public bool firstround = true;
    private int count;

    public GameObject[,] grid;
    // Use this for initialization

    private Vector2 dir1 = new Vector2(1, 0);
    private Vector2 dir2 = new Vector2(1, -1);
    private Vector2 dir3 = new Vector2(0, -1);
    private int playsLeft;
    private int gameMode;

    private struct TargetPositions {
        public Vector2 FirstPosition;
        public List<Vector2> targets;
        public hexagon_script.HexagonColor playercolor;
    }

    TargetPositions redTargets, greenTargets, blueTargets;

    void Start() {
        grid = new GameObject[11, 11];
        UsableGridPositions = new List<Vector2>();
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
        playsLeft = count;
    }

    private void StoreInGrid(int x, int y, GameObject o) {
        //print("storing: " + o.gameObject.name);
        //GameObject obj = GameObject.Find("s");
        //if (obj == null) {
        //    print(s + " not found");
        //}
        //else {
        grid[x, y] = o;
        Vector2 v = new Vector2(x, y);
        o.SendMessage("SetGridPosition", v  );
        UsableGridPositions.Add(v);
        //    Debug.Log(obj.name);
        //}
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetButtonDown("Fire1") && playsLeft > 0) {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.zero);
            if (hit.collider != null) {
                Debug.Log(hit.collider.gameObject.tag);
                chooseTile(hit.collider);
            }
        }
    }

    public void IncPlayer() {
        player = (player + 1) % 3;
        playsLeft--;
        print(playsLeft.ToString() + " plays left.");
        checkWinner();
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
                    SetTargetPositions();
                }
                SetTileColor(col, getPlayerColor());
            }
        }
        else {
            SetTileColor(col);
        }
    }

    private void SetTargetPositions() {
        hexagon_script red = GetHexScript(RedPlayer);
        hexagon_script green = GetHexScript(GreenPlayer);
        hexagon_script blue = GetHexScript(BluePlayer);

        setTargets(red.grid_pos, out redTargets, hexagon_script.HexagonColor.RED);
        setTargets(green.grid_pos, out greenTargets, hexagon_script.HexagonColor.GREEN);
        setTargets(blue.grid_pos, out blueTargets, hexagon_script.HexagonColor.BLUE);
    }

    private void setTargets(Vector2 v, out TargetPositions targs, hexagon_script.HexagonColor c) {
        TargetPositions tp = new TargetPositions();
        if (v == new Vector2(0,10)) {
            tp.FirstPosition = v;
            tp.targets = new List<Vector2>();
            Vector2 init = Vector2.zero;
            tp.targets.Add(init);
            while (IncrementPositionForWinnerCheck(out init, init, Vector2.right)) {
                tp.targets.Add(init);
            }
        }
        else if (v == new Vector2(0,0)) {
            tp.FirstPosition = v;
            tp.targets = new List<Vector2>();
            Vector2 init = new Vector2(0,10);
            tp.targets.Add(init);
            while (IncrementPositionForWinnerCheck(out init, init, new Vector2(1,-1))) {
                tp.targets.Add(init);
            }
        }
        else if (v == new Vector2(10, 0)) {
            tp.FirstPosition = v;
            tp.targets = new List<Vector2>();
            Vector2 init = new Vector2(0, 0);
            tp.targets.Add(init);
            while (IncrementPositionForWinnerCheck(out init, init, new Vector2(0, 1))) {
                tp.targets.Add(init);
            }
        }
        tp.playercolor = c;
        targs = tp;
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

    private void checkWinner() {
        if (gameMode == 0) {
            MostTilesWin(); 
        }
        else {
            FirtConnectedToEdgeWin();
        }
    }

    private void FirtConnectedToEdgeWin() {
        if (SpreadCheckWinner(redTargets)) {
            print("Red Wins");
        }
    }

    private bool SpreadCheckWinner(TargetPositions targs) {
        List<Vector2> tilesCaptured = new List<Vector2>();
        Vector2 currentPos = targs.FirstPosition;

        tilesCaptured = RecursiveSpread(targs.FirstPosition, targs.playercolor, tilesCaptured);

        foreach (Vector2 item in tilesCaptured) {
            if (targs.targets.Contains(item)) {
                return true;
            }
        }

        return false;
    }

    private List<Vector2> RecursiveSpread(Vector2 pos, hexagon_script.HexagonColor c, List<Vector2> tilesCaptured) {
        //RecursiveSpreadAux(pos, c, ref tilesCaptured);

        List<Vector2> nb = getNeighbors(pos);
        List<Vector2> nbcopy = getNeighbors(pos);


        foreach (Vector2 item in nb) {
            if (!tilesCaptured.Contains(item)) {
                if (GetHexScript(item).sameColor(c)) {
                    tilesCaptured.Add(item);
                    Debug.Log(item.x.ToString() + " " + item.y.ToString());
                }
                else
                    nbcopy.Remove(item);
            }
            else
                nbcopy.Remove(item);
        }
        foreach (Vector2 item in nbcopy) {
           tilesCaptured = RecursiveSpread(item, c, tilesCaptured);
        }

        return tilesCaptured;
    }

    public List<Vector2> getNeighbors(Vector2 pos) {
        List<Vector2> v = new List<Vector2>();
        v = AddGridNeighbor(v, pos + dir1);
        v = AddGridNeighbor(v, pos - dir1);
        v = AddGridNeighbor(v, pos + dir2);
        v = AddGridNeighbor(v, pos - dir2);
        v = AddGridNeighbor(v, pos + dir3);
        v = AddGridNeighbor(v, pos - dir3);
        return v;
    }

    [ContextMenu("tst")]
    public void test() {
        SpreadCheckWinner(redTargets);
    }

    private List<Vector2> AddGridNeighbor(List<Vector2> lst, Vector2 v) {
        if (isTile(v)) {
            lst.Add(v);
        }
        return lst;
    }

    private bool isTile(Vector2 v) {
        if (v.x < 0 || v.y < 0 || v.x > 10 || v.y > 10) {
            return false;
        }
        return grid[(int)v.x, (int)v.y] != null;
    }

    private void MostTilesWin() {
        if (playsLeft > 0)
            return;

        int redcount = 0;
        int greencount = 0;
        int bluecount = 0;
        RedFinalTiles = new List<hexagon_script>();
        GreenFinalTiles = new List<hexagon_script>();
        BlueFinalTiles = new List<hexagon_script>();

        foreach (Vector2 item in UsableGridPositions) {
            hexagon_script s = GetHexScript(item);
            switch (s.currentColor) {
                case hexagon_script.HexagonColor.WHITE:
                    break;
                case hexagon_script.HexagonColor.RED:
                    redcount++;
                    RedFinalTiles.Add(s);
                    break;
                case hexagon_script.HexagonColor.GREEN:
                    greencount++;
                    GreenFinalTiles.Add(s);
                    break;
                case hexagon_script.HexagonColor.BLUE:
                    bluecount++;
                    BlueFinalTiles.Add(s);
                    break;
                default:
                    break;
            }
        }
        if (redcount > greencount || redcount > bluecount) {
            print("Red Wins!!!!");
            WinnerAnimation(RedFinalTiles);
            LooserAnimation(GreenFinalTiles);
            LooserAnimation(BlueFinalTiles);
        }
        else if (greencount > bluecount) {
            print("Green Wins!!!!");
            WinnerAnimation(GreenFinalTiles);
            LooserAnimation(BlueFinalTiles);
            LooserAnimation(RedFinalTiles);

        }
        else {
            print("Blue Wins!!!!");
            WinnerAnimation(BlueFinalTiles);
            LooserAnimation(RedFinalTiles);
            LooserAnimation(GreenFinalTiles);

        }
    }

    private void LooserAnimation(List<hexagon_script> list) {
        foreach (hexagon_script item in list) {
            item.LoosingTile();
        }
    }

    private void WinnerAnimation(List<hexagon_script> list) {
        foreach (hexagon_script item in list) {
            item.WinningTile();
        }
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
        if (!IncrementPositionForSpreadCheck(out currentPos, origin, dir)){
            return;
	    }
        while (true) {
            hexagon_script s = GetHexScript(currentPos);
            if (s.isWhite()) {
                return;
            }
            if (s.sameColor(getPlayerColor())){
                canCapture = true;
                break;
            }
            else {
                tilesToColor.Add(currentPos);
                if (!IncrementPositionForSpreadCheck(out currentPos, currentPos, dir)) 
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

    private bool IncrementPositionForSpreadCheck(out Vector2 currentPos, Vector2 paux, Vector2 dir) {
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

    private bool IncrementPositionForWinnerCheck(out Vector2 currentPos, Vector2 paux, Vector2 dir) {
        Vector2 newpos = paux + dir;
        if (newpos.x < 0 || newpos.y < 0 || newpos.x > 10 || newpos.y > 10) {
            currentPos = Vector2.zero;
            return false;
        }
        else {
            currentPos = newpos;
            return true;
        }
    }

    private hexagon_script GetHexScript(GameObject go) {
        return go.GetComponent<hexagon_script>();
    }

    private hexagon_script GetHexScript(Vector2 v) {
        return GetHexScript(grid[(int)v.x, (int)v.y]);
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
        GUI.Label(new Rect(10, 80, 100, 40), "Current Player: " + getPlayerColor().ToString());
        if (GUI.Button(new Rect(15, 130, 150, 20), "Most Tiles Win")) {
            gameMode = 0;
        }
        if (GUI.Button(new Rect(15, 150, 150, 20), "First Connected Wins")) {
            gameMode = 1;
        }  
    }
}
