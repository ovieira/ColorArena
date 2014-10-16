using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public Transform hex_prefab;

    public float unit_lenght = 1;

    //public int sizeX, sizeY;

    public int player = 0;

    public int number_of_rows;
    public float offsetX;
    // Use this for initialization
    void Start() {

        unit_lenght = hex_prefab.renderer.bounds.size.x / Mathf.Sqrt(3);
        offsetX = Mathf.Sqrt(3) * unit_lenght * 0.5f;

        for (int i = 0; i < number_of_rows; i++) {
            float posy = i * 1.5f * unit_lenght - (number_of_rows * 1.5f * unit_lenght) / 2;
            int number_of_hexs = 1 + i;
            float posx = -offsetX * i;

            Vector3 init_pos = new Vector3(posx, posy, 0);
            Instantiate(hex_prefab, init_pos, Quaternion.identity);

            for (int j = 1; j < number_of_hexs; j++) {
                init_pos.x += (offsetX * 2);
                Instantiate(hex_prefab, init_pos, Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetButtonDown("Fire1")) {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.zero);
            if (hit.collider != null) {
                Debug.Log(hit.collider.gameObject.tag);
                setColor(hit.collider);
                player = (player + 1) % 3;
            }
        }
    }

    private void setColor(Collider2D col) {
        //if (col.GetComponent) {
            //col.SendMessage("setColor", playerColor()); 
        //}
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
