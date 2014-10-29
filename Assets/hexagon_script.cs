using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class hexagon_script : MonoBehaviour {

    public Sprite white, red, green, blue;

    public enum HexagonColor {
        WHITE,
        RED,
        GREEN,
        BLUE
    }

    public HexagonColor currentColor;

    //public GameObject[] adjacent;
    public List<GameObject> adjacent;
    public int count;

    public Vector2 grid_pos;
    public Vector2 screen_pos;
    public GUIStyle _style;

    // Use this for initialization
    void Start() {
        currentColor = HexagonColor.WHITE;
        //adjacent = new GameObject[6];
        screen_pos = Camera.main.WorldToScreenPoint(transform.position);
    }

    // Update is called once per frame
    void Update() {

    }

    public void setColor(HexagonColor c) {
        captureTile(c);
        GameObject.FindGameObjectWithTag("GameManager").SendMessage("IncPlayer");
    }

    public void captureTile(HexagonColor c) {
        switch (c) {
            case HexagonColor.RED:
                GetComponent<SpriteRenderer>().sprite = red;
                currentColor = HexagonColor.RED;
                break;
            case HexagonColor.GREEN:
                GetComponent<SpriteRenderer>().sprite = green;
                currentColor = HexagonColor.GREEN;
                break;
            case HexagonColor.BLUE:
                GetComponent<SpriteRenderer>().sprite = blue;
                currentColor = HexagonColor.BLUE;
                break;
            default:
                break;
        }
    }

    private void checkAdjacentTiles(HexagonColor c) {

    }

    public void SetGridPosition(Vector2 pos) {
        grid_pos = pos;
    }

    public Vector2 GetGridPosition() {
        return grid_pos;
    }

    public bool isWhite() {
        return currentColor == HexagonColor.WHITE;
    }

    public bool sameColor(HexagonColor c) {
        return currentColor == c;
    }

    //void OnGUI() {
    //    GUI.Label(new Rect(screen_pos.x - 15, Screen.height - screen_pos.y, 10, 20), currentColor.ToString(), _style);
    //}
}
