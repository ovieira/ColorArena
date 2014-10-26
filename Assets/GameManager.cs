using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public GameObject hex_prefab;

    public float unit_lenght = 1;

    //public int sizeX, sizeY;

    public int player = 0;

    public int number_of_rows;
    public float offsetX;

    public GameObject RedPlayer, GreenPlayer, BluePlayer;

    public bool choosePlayerTile = true;
    private int count=0;

    public GameObject[,] grid;
    // Use this for initialization
    void Start() {
        grid = new GameObject[11,11];
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
                setColor(hit.collider);
            }
        }
    }

    public void IncPlayer() {
        player = (player + 1) % 3;
    }

    private void setColor(Collider2D col) {
        if (choosePlayerTile) {
            if (isStartTile(col)) {
                switch (playerColor()) {
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
                    choosePlayerTile = false;
                }
                col.SendMessage("setColor", playerColor());
            }
        }
        else {
            col.SendMessage("setColor", playerColor());
        }
    }

    private bool isStartTile(Collider2D col) {
        string nmr = col.name;
        if (nmr == "hex 1" || nmr == "hex 56" || nmr == "hex 66")
            return true;
        return false;
    }

    public hexagon_script.HexagonColor playerColor() {
        hexagon_script.HexagonColor c = hexagon_script.HexagonColor.BLUE;
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
        GUI.Label(new Rect(10, 80, 100, 40), "Current Player: " + (player+1).ToString());
    }
}
